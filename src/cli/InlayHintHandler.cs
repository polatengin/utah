using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class InlayHintHandler : IInlayHintsHandler
{
  public InlayHintRegistrationOptions GetRegistrationOptions(InlayHintClientCapabilities capability, ClientCapabilities clientCapabilities)
  {
    return new InlayHintRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      }),
      ResolveProvider = false
    };
  }

  public async Task<InlayHintContainer?> Handle(InlayHintParams request, CancellationToken cancellationToken)
  {
    try
    {
      var uri = request.TextDocument.Uri;
      var text = DocumentManager.Instance.Get(uri.ToString());
      if (text == null)
      {
        var path = uri.GetFileSystemPath();
        if (File.Exists(path))
          text = await File.ReadAllTextAsync(path, cancellationToken);
        else
          return null;
      }

      var lines = text.Split('\n');
      var startLine = (int)request.Range.Start.Line;
      var endLine = Math.Min((int)request.Range.End.Line, lines.Length - 1);

      var hints = new List<InlayHint>();

      for (int i = startLine; i <= endLine; i++)
      {
        var line = lines[i];
        var trimmed = line.TrimStart();

        if (trimmed.StartsWith("//")) continue;

        AddVariableTypeHints(lines, line, trimmed, i, hints);
        AddParameterNameHints(line, trimmed, i, hints);
      }

      return new InlayHintContainer(hints);
    }
    catch
    {
      return null;
    }
  }

  private static void AddVariableTypeHints(string[] lines, string line, string trimmed, int lineNum, List<InlayHint> hints)
  {
    // Match variable declarations without explicit type: let name = ... or const name = ...
    var match = Regex.Match(trimmed, @"^(let|const)\s+(\w+)\s*=\s*(.+)$");
    if (!match.Success) return;

    // Skip if there's already a type annotation
    var withType = Regex.Match(trimmed, @"^(let|const)\s+\w+\s*:\s*");
    if (withType.Success) return;

    var varName = match.Groups[2].Value;
    var valueExpr = match.Groups[3].Value.TrimEnd();

    var inferredType = InferType(lines, valueExpr);
    if (inferredType == null) return;

    var nameCol = line.IndexOf(varName, StringComparison.Ordinal);
    if (nameCol < 0) return;

    hints.Add(new InlayHint
    {
      Position = new Position(lineNum, nameCol + varName.Length),
      Label = new StringOrInlayHintLabelParts($": {inferredType}"),
      Kind = InlayHintKind.Type,
      PaddingLeft = false,
      PaddingRight = true
    });
  }

  private static void AddParameterNameHints(string line, string trimmed, int lineNum, List<InlayHint> hints)
  {
    // Find namespace.function(...) calls
    var callPattern = new Regex(@"(\w+)\.(\w+)\s*\(");
    foreach (Match callMatch in callPattern.Matches(trimmed))
    {
      var nsName = callMatch.Groups[1].Value;
      var funcName = callMatch.Groups[2].Value;

      if (!IsBuiltInNamespace(nsName)) continue;

      var completions = GetCompletionsForNamespace(nsName);
      if (completions == null) continue;

      var funcItem = completions.Find(c => c.Label == funcName);
      if (funcItem == null) continue;

      var detail = funcItem.Detail?.ToString();
      if (detail == null) continue;

      var paramDefs = ExtractParameterNames(detail);
      if (paramDefs.Count == 0) continue;

      // Find the opening paren position in the original line
      var callIdx = line.IndexOf(callMatch.Value, StringComparison.Ordinal);
      if (callIdx < 0) continue;

      var parenPos = callIdx + callMatch.Value.Length - 1;
      var args = ExtractArguments(line, parenPos);

      for (int p = 0; p < Math.Min(args.Count, paramDefs.Count); p++)
      {
        var arg = args[p].Text.Trim();
        if (string.IsNullOrEmpty(arg)) continue;

        // Don't show hint if argument is already a named parameter or is a simple literal matching the name
        if (arg.Contains(':') && !arg.Contains('?')) continue;

        // Don't show hint if there's only one parameter
        if (paramDefs.Count == 1) continue;

        hints.Add(new InlayHint
        {
          Position = new Position(lineNum, args[p].StartCol),
          Label = new StringOrInlayHintLabelParts($"{paramDefs[p]}:"),
          Kind = InlayHintKind.Parameter,
          PaddingLeft = false,
          PaddingRight = true
        });
      }
    }
  }

  private static string? InferType(string[] lines, string expr)
  {
    expr = expr.TrimEnd(';').Trim();

    // String literals
    if ((expr.StartsWith('"') && expr.EndsWith('"')) ||
        (expr.StartsWith('\'') && expr.EndsWith('\'')) ||
        (expr.StartsWith('`') && expr.EndsWith('`')))
      return "string";

    // Boolean literals
    if (expr is "true" or "false")
      return "boolean";

    // Numeric literals
    if (Regex.IsMatch(expr, @"^-?\d+(\.\d+)?$"))
      return "number";

    // Array literals
    if (expr.StartsWith('['))
      return InferArrayType(expr);

    // Namespace function calls — infer from return type
    var callMatch = Regex.Match(expr, @"^(\w+)\.(\w+)\s*\(");
    if (callMatch.Success)
    {
      var returnType = GetBuiltInReturnType(callMatch.Groups[1].Value, callMatch.Groups[2].Value);
      if (returnType != null) return returnType;
    }

    // User-defined function calls
    var userCallMatch = Regex.Match(expr, @"^(\w+)\s*\(");
    if (userCallMatch.Success)
    {
      var funcName = userCallMatch.Groups[1].Value;
      var returnType = GetUserFunctionReturnType(lines, funcName);
      if (returnType != null) return returnType;
    }

    // Ternary expression — infer from first branch
    var ternaryMatch = Regex.Match(expr, @"\?.+?:\s*(.+)$");
    if (expr.Contains('?') && expr.Contains(':'))
    {
      // Try to infer from the "true" branch value
      var qIdx = FindTernaryQuestion(expr);
      if (qIdx > 0)
      {
        var colonIdx = FindTernaryColon(expr, qIdx + 1);
        if (colonIdx > qIdx)
        {
          var trueBranch = expr[(qIdx + 1)..colonIdx].Trim();
          return InferType(lines, trueBranch);
        }
      }
    }

    // String concatenation or template
    if (expr.Contains("${") || (expr.Contains('+') && expr.Contains('"')))
      return "string";

    // Arithmetic expressions
    if (Regex.IsMatch(expr, @"^[^""'`]*[\+\-\*/%]\s*\d") && !expr.Contains('"'))
      return "number";

    return null;
  }

  private static string InferArrayType(string expr)
  {
    // Try to determine element type from first element
    var inner = expr.TrimStart('[').TrimEnd(']').Trim();
    if (string.IsNullOrEmpty(inner)) return "any[]";

    var firstElem = inner.Split(',')[0].Trim();
    if ((firstElem.StartsWith('"') && firstElem.EndsWith('"')) ||
        (firstElem.StartsWith('\'') && firstElem.EndsWith('\'')))
      return "string[]";
    if (firstElem is "true" or "false")
      return "boolean[]";
    if (Regex.IsMatch(firstElem, @"^-?\d+(\.\d+)?$"))
      return "number[]";

    return "any[]";
  }

  private static string? GetBuiltInReturnType(string namespaceName, string functionName)
  {
    var completions = GetCompletionsForNamespace(namespaceName);
    if (completions == null) return null;

    var match = completions.Find(c => c.Label == functionName);
    if (match == null) return null;

    var detail = match.Detail?.ToString();
    if (detail == null) return null;

    // Extract return type from "funcName(...): returnType"
    var parenEnd = detail.LastIndexOf(')');
    if (parenEnd < 0) return null;

    var afterParen = detail[(parenEnd + 1)..].Trim();
    if (afterParen.StartsWith(':'))
      return afterParen[1..].Trim();

    // No return type means void
    return null;
  }

  private static string? GetUserFunctionReturnType(string[] lines, string functionName)
  {
    var pattern = new Regex($@"^function\s+{Regex.Escape(functionName)}\s*\([^)]*\)\s*:\s*(\w+(?:\[\])?)");
    foreach (var line in lines)
    {
      var match = pattern.Match(line.TrimStart());
      if (match.Success)
        return match.Groups[1].Value;
    }
    return null;
  }

  private static List<string> ExtractParameterNames(string detail)
  {
    var names = new List<string>();
    var parenStart = detail.IndexOf('(');
    var parenEnd = detail.LastIndexOf(')');
    if (parenStart < 0 || parenEnd <= parenStart) return names;

    var paramsStr = detail[(parenStart + 1)..parenEnd];
    if (string.IsNullOrWhiteSpace(paramsStr)) return names;

    var parts = SplitParams(paramsStr);
    foreach (var part in parts)
    {
      var p = part.Trim();
      var colonIdx = p.IndexOf(':');
      if (colonIdx > 0)
        names.Add(p[..colonIdx].TrimEnd().TrimEnd('?'));
      else
        names.Add(p);
    }

    return names;
  }

  private record ArgInfo(string Text, int StartCol);

  private static List<ArgInfo> ExtractArguments(string line, int parenPos)
  {
    var args = new List<ArgInfo>();
    int depth = 0;
    int argStart = parenPos + 1;

    // Skip leading whitespace for first arg
    while (argStart < line.Length && line[argStart] == ' ')
      argStart++;

    int currentArgStart = argStart;
    bool inSingle = false, inDouble = false, inBacktick = false;

    for (int i = parenPos + 1; i < line.Length; i++)
    {
      var c = line[i];
      if (c == '\\' && i + 1 < line.Length) { i++; continue; }

      if (c == '\'' && !inDouble && !inBacktick) inSingle = !inSingle;
      else if (c == '"' && !inSingle && !inBacktick) inDouble = !inDouble;
      else if (c == '`' && !inSingle && !inDouble) inBacktick = !inBacktick;

      if (inSingle || inDouble || inBacktick) continue;

      if (c == '(' || c == '[') depth++;
      else if (c == ']') depth--;
      else if (c == ')')
      {
        if (depth == 0)
        {
          var text = line[currentArgStart..i];
          if (!string.IsNullOrWhiteSpace(text))
            args.Add(new ArgInfo(text, currentArgStart));
          break;
        }
        depth--;
      }
      else if (c == ',' && depth == 0)
      {
        var text = line[currentArgStart..i];
        args.Add(new ArgInfo(text, currentArgStart));
        currentArgStart = i + 1;
        while (currentArgStart < line.Length && line[currentArgStart] == ' ')
          currentArgStart++;
      }
    }

    return args;
  }

  private static int FindTernaryQuestion(string expr)
  {
    int depth = 0;
    bool inStr = false;
    char strChar = '\0';
    for (int i = 0; i < expr.Length; i++)
    {
      var c = expr[i];
      if (c == '\\' && i + 1 < expr.Length) { i++; continue; }
      if (!inStr && (c == '"' || c == '\'' || c == '`')) { inStr = true; strChar = c; continue; }
      if (inStr && c == strChar) { inStr = false; continue; }
      if (inStr) continue;
      if (c == '(' || c == '[') depth++;
      else if (c == ')' || c == ']') depth--;
      else if (c == '?' && depth == 0) return i;
    }
    return -1;
  }

  private static int FindTernaryColon(string expr, int startAfterQuestion)
  {
    int depth = 0;
    bool inStr = false;
    char strChar = '\0';
    for (int i = startAfterQuestion; i < expr.Length; i++)
    {
      var c = expr[i];
      if (c == '\\' && i + 1 < expr.Length) { i++; continue; }
      if (!inStr && (c == '"' || c == '\'' || c == '`')) { inStr = true; strChar = c; continue; }
      if (inStr && c == strChar) { inStr = false; continue; }
      if (inStr) continue;
      if (c == '(' || c == '[') depth++;
      else if (c == ')' || c == ']') depth--;
      else if (c == ':' && depth == 0) return i;
    }
    return -1;
  }

  private static List<string> SplitParams(string paramsStr)
  {
    var parts = new List<string>();
    int depth = 0;
    int start = 0;
    for (int i = 0; i < paramsStr.Length; i++)
    {
      var c = paramsStr[i];
      if (c == '<' || c == '(' || c == '[') depth++;
      else if (c == '>' || c == ')' || c == ']') depth--;
      else if (c == ',' && depth == 0) { parts.Add(paramsStr[start..i]); start = i + 1; }
    }
    if (start < paramsStr.Length) parts.Add(paramsStr[start..]);
    return parts;
  }

  private static bool IsBuiltInNamespace(string word) =>
    word is "console" or "fs" or "web" or "json" or "yaml" or "validate" or "process" or "os"
      or "git" or "docker" or "utility" or "timer" or "system" or "string" or "ssh" or "args"
      or "script" or "template" or "scheduler" or "array" or "math" or "date";

  private static List<CompletionItem>? GetCompletionsForNamespace(string namespaceName)
  {
    return namespaceName switch
    {
      "console" => CompletionHandler.GetConsoleCompletions(),
      "fs" => CompletionHandler.GetFsCompletions(),
      "web" => CompletionHandler.GetWebCompletions(),
      "json" => CompletionHandler.GetJsonCompletions(),
      "yaml" => CompletionHandler.GetYamlCompletions(),
      "validate" => CompletionHandler.GetValidateCompletions(),
      "process" => CompletionHandler.GetProcessCompletions(),
      "os" => CompletionHandler.GetOsCompletions(),
      "git" => CompletionHandler.GetGitCompletions(),
      "docker" => CompletionHandler.GetDockerCompletions(),
      "utility" => CompletionHandler.GetUtilityCompletions(),
      "timer" => CompletionHandler.GetTimerCompletions(),
      "system" => CompletionHandler.GetSystemCompletions(),
      "string" => CompletionHandler.GetStringNamespaceCompletions(),
      "ssh" => CompletionHandler.GetSshNamespaceCompletions(),
      "args" => CompletionHandler.GetArgsCompletions(),
      "script" => CompletionHandler.GetScriptCompletions(),
      "template" => CompletionHandler.GetTemplateCompletions(),
      "scheduler" => CompletionHandler.GetSchedulerCompletions(),
      "array" => CompletionHandler.GetArrayCompletions(),
      "math" => CompletionHandler.GetMathCompletions(),
      "date" => CompletionHandler.GetDateCompletions(),
      _ => null
    };
  }
}
