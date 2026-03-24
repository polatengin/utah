using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class SignatureHelpHandler : ISignatureHelpHandler
{
  private static readonly TextDocumentSelector _selector = new(new TextDocumentFilter
  {
    Language = "utah",
    Pattern = "**/*.shx"
  });

  public SignatureHelpRegistrationOptions GetRegistrationOptions(SignatureHelpCapability capability, ClientCapabilities clientCapabilities)
  {
    return new SignatureHelpRegistrationOptions
    {
      DocumentSelector = _selector,
      TriggerCharacters = new Container<string>("(", ","),
      RetriggerCharacters = new Container<string>(",")
    };
  }

  public async Task<SignatureHelp?> Handle(SignatureHelpParams request, CancellationToken cancellationToken)
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
      if (request.Position.Line >= lines.Length)
        return null;

      var line = lines[(int)request.Position.Line];
      var cursorPos = (int)request.Position.Character;
      if (cursorPos > line.Length)
        cursorPos = line.Length;

      var textBeforeCursor = line[..cursorPos];

      // Find the function call context: walk backwards to find the matching open paren
      var (functionName, namespaceName, activeParam) = FindCallContext(textBeforeCursor);
      if (functionName == null)
        return null;

      SignatureInformation? signature = null;

      if (namespaceName != null)
      {
        // Built-in namespace function
        signature = GetBuiltInSignature(namespaceName, functionName);
      }

      if (signature == null)
      {
        // User-defined function
        signature = FindUserDefinedSignature(lines, functionName);
      }

      if (signature == null)
        return null;

      var paramCount = signature.Parameters?.Count() ?? 0;

      return new SignatureHelp
      {
        Signatures = new Container<SignatureInformation>(signature),
        ActiveSignature = 0,
        ActiveParameter = Math.Min(activeParam, Math.Max(0, paramCount - 1))
      };
    }
    catch
    {
      return null;
    }
  }

  private static (string? functionName, string? namespaceName, int activeParam) FindCallContext(string textBeforeCursor)
  {
    // Walk backwards from cursor to find the nearest unmatched open paren
    int depth = 0;
    int parenPos = -1;

    for (int i = textBeforeCursor.Length - 1; i >= 0; i--)
    {
      var c = textBeforeCursor[i];
      if (c == ')') depth++;
      else if (c == '(')
      {
        if (depth == 0)
        {
          parenPos = i;
          break;
        }
        depth--;
      }
    }

    if (parenPos < 0)
      return (null, null, 0);

    // Count commas between parenPos and cursor to determine active parameter
    int activeParam = CountActiveParameter(textBeforeCursor, parenPos + 1);

    // Extract function name (and optional namespace) before the paren
    var beforeParen = textBeforeCursor[..parenPos].TrimEnd();
    if (beforeParen.Length == 0)
      return (null, null, 0);

    // Extract identifier backwards
    int end = beforeParen.Length;
    int start = end;
    while (start > 0 && IsIdentifierChar(beforeParen[start - 1]))
      start--;

    if (start == end)
      return (null, null, 0);

    var funcName = beforeParen[start..end];

    // Check for namespace.function pattern
    string? nsName = null;
    if (start >= 2 && beforeParen[start - 1] == '.')
    {
      int nsEnd = start - 1;
      int nsStart = nsEnd;
      while (nsStart > 0 && IsIdentifierChar(beforeParen[nsStart - 1]))
        nsStart--;

      if (nsStart < nsEnd)
        nsName = beforeParen[nsStart..nsEnd];
    }

    return (funcName, nsName, activeParam);
  }

  private static int CountActiveParameter(string text, int startAfterParen)
  {
    int commas = 0;
    int depth = 0;
    bool inSingleQuote = false;
    bool inDoubleQuote = false;
    bool inBacktick = false;

    for (int i = startAfterParen; i < text.Length; i++)
    {
      var c = text[i];

      if (c == '\\' && i + 1 < text.Length)
      {
        i++;
        continue;
      }

      if (c == '\'' && !inDoubleQuote && !inBacktick) inSingleQuote = !inSingleQuote;
      else if (c == '"' && !inSingleQuote && !inBacktick) inDoubleQuote = !inDoubleQuote;
      else if (c == '`' && !inSingleQuote && !inDoubleQuote) inBacktick = !inBacktick;

      if (inSingleQuote || inDoubleQuote || inBacktick)
        continue;

      if (c == '(' || c == '[') depth++;
      else if (c == ')' || c == ']') depth--;
      else if (c == ',' && depth == 0) commas++;
    }

    return commas;
  }

  private static SignatureInformation? GetBuiltInSignature(string namespaceName, string functionName)
  {
    var completions = GetCompletionsForNamespace(namespaceName);
    if (completions == null) return null;

    var match = completions.Find(c => c.Label == functionName);
    if (match == null) return null;

    var detail = match.Detail?.ToString();
    if (detail == null) return null;

    return BuildSignatureFromDetail($"{namespaceName}.{detail}", match.Documentation?.ToString());
  }

  private static SignatureInformation? FindUserDefinedSignature(string[] lines, string functionName)
  {
    var pattern = new Regex($@"^function\s+{Regex.Escape(functionName)}\s*\(([^)]*)\)(?:\s*:\s*(\w+(?:\[\])?))?");

    foreach (var line in lines)
    {
      var trimmed = line.TrimStart();
      var match = pattern.Match(trimmed);
      if (match.Success)
      {
        var paramsStr = match.Groups[1].Value;
        var returnType = match.Groups[2].Success ? match.Groups[2].Value : "void";
        var label = $"{functionName}({paramsStr}): {returnType}";

        var parameters = ParseParameters(paramsStr);

        return new SignatureInformation
        {
          Label = label,
          Documentation = $"User-defined function `{functionName}`",
          Parameters = new Container<ParameterInformation>(parameters)
        };
      }
    }

    return null;
  }

  private static SignatureInformation BuildSignatureFromDetail(string detail, string? documentation)
  {
    // detail is like "namespace.funcName(param: type, ...): returnType"
    var parenStart = detail.IndexOf('(');
    var parenEnd = detail.LastIndexOf(')');

    if (parenStart < 0 || parenEnd < 0 || parenEnd <= parenStart)
    {
      return new SignatureInformation
      {
        Label = detail,
        Documentation = documentation,
        Parameters = new Container<ParameterInformation>()
      };
    }

    var paramsStr = detail[(parenStart + 1)..parenEnd];
    var parameters = ParseParameters(paramsStr);

    return new SignatureInformation
    {
      Label = detail,
      Documentation = documentation,
      Parameters = new Container<ParameterInformation>(parameters)
    };
  }

  private static List<ParameterInformation> ParseParameters(string paramsStr)
  {
    var parameters = new List<ParameterInformation>();
    if (string.IsNullOrWhiteSpace(paramsStr))
      return parameters;

    // Split on commas, respecting nested generics like Array<string>
    var parts = SplitParameters(paramsStr);

    foreach (var part in parts)
    {
      var trimmed = part.Trim();
      if (string.IsNullOrEmpty(trimmed))
        continue;

      // Extract parameter name and type from "name: type" or "name?: type"
      var colonIdx = trimmed.IndexOf(':');
      string paramName;
      string paramDoc;

      if (colonIdx > 0)
      {
        paramName = trimmed[..colonIdx].TrimEnd().TrimEnd('?');
        var paramType = trimmed[(colonIdx + 1)..].Trim();
        var isOptional = trimmed[..colonIdx].TrimEnd().EndsWith("?");
        paramDoc = isOptional ? $"`{paramName}` (optional): {paramType}" : $"`{paramName}`: {paramType}";
      }
      else
      {
        paramName = trimmed;
        paramDoc = $"`{paramName}`";
      }

      parameters.Add(new ParameterInformation
      {
        Label = trimmed,
        Documentation = paramDoc
      });
    }

    return parameters;
  }

  private static List<string> SplitParameters(string paramsStr)
  {
    var parts = new List<string>();
    int depth = 0;
    int start = 0;

    for (int i = 0; i < paramsStr.Length; i++)
    {
      var c = paramsStr[i];
      if (c == '<' || c == '(' || c == '[') depth++;
      else if (c == '>' || c == ')' || c == ']') depth--;
      else if (c == ',' && depth == 0)
      {
        parts.Add(paramsStr[start..i]);
        start = i + 1;
      }
    }

    if (start < paramsStr.Length)
      parts.Add(paramsStr[start..]);

    return parts;
  }

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

  private static bool IsIdentifierChar(char c) =>
    char.IsLetterOrDigit(c) || c == '_';
}
