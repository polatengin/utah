using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class HoverHandler : IHoverHandler
{
  public HoverRegistrationOptions GetRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities)
  {
    return new HoverRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      })
    };
  }

  public async Task<Hover?> Handle(HoverParams request, CancellationToken cancellationToken)
  {
    try
    {
      var uri = request.TextDocument.Uri;
      var documentText = DocumentManager.Instance.Get(uri.ToString());
      if (documentText == null)
      {
        var documentPath = uri.GetFileSystemPath();
        if (!File.Exists(documentPath))
          return null;
        documentText = await File.ReadAllTextAsync(documentPath, cancellationToken);
      }

      var lines = documentText.Split('\n');
      if (request.Position.Line >= lines.Length)
        return null;

      var line = lines[(int)request.Position.Line];

      // Try built-in namespace hover first
      var (namespaceName, functionName) = ExtractNamespacedSymbol(line, (int)request.Position.Character);
      if (namespaceName != null && functionName != null)
      {
        var completions = GetCompletionsForNamespace(namespaceName);
        if (completions != null)
        {
          var match = completions.Find(c => c.Label == functionName);
          if (match != null)
          {
            var markdown = $"```utah\n{namespaceName}.{match.Detail}\n```\n\n{match.Documentation}";
            return CreateHover(markdown);
          }
        }
      }

      // Try user-defined symbol hover
      var symbol = ExtractSymbol(line, (int)request.Position.Character);
      if (symbol == null)
        return null;

      // Check for keyword hover
      var keywordHover = GetKeywordHover(symbol);
      if (keywordHover != null)
        return CreateHover(keywordHover);

      // Search for user-defined symbol
      var userHover = FindUserSymbolHover(lines, symbol);
      if (userHover != null)
        return CreateHover(userHover);

      // Check if it's a known namespace
      var nsHover = GetNamespaceHover(symbol);
      if (nsHover != null)
        return CreateHover(nsHover);

      return null;
    }
    catch
    {
      return null;
    }
  }

  private static Hover CreateHover(string markdown)
  {
    return new Hover
    {
      Contents = new MarkedStringsOrMarkupContent(new MarkupContent
      {
        Kind = MarkupKind.Markdown,
        Value = markdown
      })
    };
  }

  private static string? FindUserSymbolHover(string[] lines, string symbol)
  {
    for (int i = 0; i < lines.Length; i++)
    {
      var trimmed = lines[i].TrimStart();
      Match match;

      // Variable declaration: let name: type = ...
      match = Regex.Match(trimmed, $@"^(let|const)\s+{Regex.Escape(symbol)}\s*:\s*(\w+(?:<[^>]+>)?(?:\[\])?)\s*=");
      if (match.Success)
      {
        var kind = match.Groups[1].Value;
        var type = match.Groups[2].Value;
        return $"```utah\n{kind} {symbol}: {type}\n```";
      }

      // Variable declaration without type annotation: let name = ...
      match = Regex.Match(trimmed, $@"^(let|const)\s+{Regex.Escape(symbol)}\s*=");
      if (match.Success)
      {
        var kind = match.Groups[1].Value;
        return $"```utah\n{kind} {symbol}\n```";
      }

      // Function declaration: function name(params): returnType {
      match = Regex.Match(trimmed, $@"^function\s+{Regex.Escape(symbol)}\s*\(([^)]*)\)(?:\s*:\s*(\w+(?:\[\])?))?");
      if (match.Success)
      {
        var parameters = match.Groups[1].Value;
        var returnType = match.Groups[2].Success ? match.Groups[2].Value : "void";
        return $"```utah\nfunction {symbol}({parameters}): {returnType}\n```";
      }

      // Structured type: interface Name { or record Name {
      match = Regex.Match(trimmed, $@"^(interface|record)\s+{Regex.Escape(symbol)}\s*\{{");
      if (match.Success)
      {
        var typeKind = match.Groups[1].Value;
        var fields = ExtractTypeFieldsForHover(lines, i + 1);
        var fieldsStr = fields.Count > 0 ? "\n" + string.Join("\n", fields.Select(f => $"  {f.Name}: {f.Type};")) : "";
        return $"```utah\n{typeKind} {symbol} {{{fieldsStr}\n}}\n```";
      }
    }

    return null;
  }

  private static List<(string Name, string Type)> ExtractTypeFieldsForHover(string[] lines, int startLine)
  {
    var fields = new List<(string Name, string Type)>();
    for (int i = startLine; i < lines.Length; i++)
    {
      var trimmed = lines[i].Trim();
      if (trimmed.StartsWith("}"))
        break;
      var match = Regex.Match(trimmed, @"^(\w+)\s*:\s*(.+?)(?:;|$)");
      if (match.Success)
      {
        fields.Add((match.Groups[1].Value, match.Groups[2].Value.TrimEnd(';').Trim()));
      }
    }
    return fields;
  }

  private static string? GetKeywordHover(string symbol)
  {
    return symbol switch
    {
      "let" => "```utah\nlet\n```\n\nDeclares a mutable variable.",
      "const" => "```utah\nconst\n```\n\nDeclares an immutable constant variable.",
      "function" => "```utah\nfunction\n```\n\nDeclares a function.",
      "if" => "```utah\nif\n```\n\nConditional statement.",
      "else" => "```utah\nelse\n```\n\nAlternative branch in an if statement.",
      "for" => "```utah\nfor\n```\n\nLoop statement. Supports C-style `for` and `for-in` loops.",
      "while" => "```utah\nwhile\n```\n\nWhile loop statement.",
      "switch" => "```utah\nswitch\n```\n\nSwitch/case statement.",
      "return" => "```utah\nreturn\n```\n\nReturns a value from a function.",
      "break" => "```utah\nbreak\n```\n\nBreaks out of a loop or switch case.",
      "continue" => "```utah\ncontinue\n```\n\nSkips to the next loop iteration.",
      "import" => "```utah\nimport\n```\n\nImports definitions from another `.shx` file.",
      "interface" => "```utah\ninterface\n```\n\nDeclares a structured type (compile-time only).",
      "record" => "```utah\nrecord\n```\n\nDeclares a record type (compile-time only).",
      "try" => "```utah\ntry\n```\n\nTry/catch error handling block.",
      "catch" => "```utah\ncatch\n```\n\nCatch block for error handling.",
      "defer" => "```utah\ndefer\n```\n\nDefers a statement to execute when the current function returns.",
      "exit" => "```utah\nexit\n```\n\nExits the script with a status code.",
      _ => null
    };
  }

  private static string? GetNamespaceHover(string symbol)
  {
    return symbol switch
    {
      "console" => "```utah\nconsole\n```\n\nConsole I/O namespace. Provides logging, prompts, and dialog functions.",
      "fs" => "```utah\nfs\n```\n\nFile system namespace. Provides file/directory operations.",
      "web" => "```utah\nweb\n```\n\nHTTP/web namespace. Provides GET, POST, PUT, DELETE requests.",
      "json" => "```utah\njson\n```\n\nJSON namespace. Provides parse, stringify, get, set, and merge operations.",
      "yaml" => "```utah\nyaml\n```\n\nYAML namespace. Provides parse, stringify, and manipulation operations.",
      "validate" => "```utah\nvalidate\n```\n\nValidation namespace. Provides email, URL, UUID, and range checks.",
      "process" => "```utah\nprocess\n```\n\nProcess namespace. Provides process management functions.",
      "os" => "```utah\nos\n```\n\nOS namespace. Provides OS detection and package checks.",
      "git" => "```utah\ngit\n```\n\nGit namespace. Provides git operations.",
      "docker" => "```utah\ndocker\n```\n\nDocker namespace. Provides container and image management.",
      "utility" => "```utah\nutility\n```\n\nUtility namespace. Provides random, UUID, hash, and base64 functions.",
      "timer" => "```utah\ntimer\n```\n\nTimer namespace. Provides timing and benchmarking functions.",
      "system" => "```utah\nsystem\n```\n\nSystem namespace. Provides CPU and memory information.",
      "string" => "```utah\nstring\n```\n\nString namespace. Provides string manipulation functions.",
      "ssh" => "```utah\nssh\n```\n\nSSH namespace. Provides remote connection and execution.",
      "args" => "```utah\nargs\n```\n\nArguments namespace. Provides CLI argument parsing.",
      "script" => "```utah\nscript\n```\n\nScript namespace. Provides script configuration (debug, error handling).",
      "template" => "```utah\ntemplate\n```\n\nTemplate namespace. Provides file templating with variable substitution.",
      "scheduler" => "```utah\nscheduler\n```\n\nScheduler namespace. Provides cron-based job scheduling.",
      "array" => "```utah\narray\n```\n\nArray namespace. Provides array manipulation functions.",
      "math" => "```utah\nmath\n```\n\nMath namespace. Provides mathematical operations.",
      "date" => "```utah\ndate\n```\n\nDate namespace. Provides date/time operations.",
      _ => null
    };
  }

  private static (string? namespaceName, string? functionName) ExtractNamespacedSymbol(string line, int character)
  {
    int start = character;
    int end = character;

    while (start > 0 && IsIdentifierChar(line[start - 1]))
      start--;
    while (end < line.Length && IsIdentifierChar(line[end]))
      end++;

    if (start == end)
      return (null, null);

    var word = line[start..end];

    // Check if preceded by "namespace."
    if (start >= 2 && line[start - 1] == '.')
    {
      int nsEnd = start - 1;
      int nsStart = nsEnd;
      while (nsStart > 0 && IsIdentifierChar(line[nsStart - 1]))
        nsStart--;

      if (nsStart < nsEnd)
      {
        var ns = line[nsStart..nsEnd];
        return (ns, word);
      }
    }

    // Check if followed by ".function"
    if (end < line.Length && line[end] == '.')
    {
      int fnStart = end + 1;
      int fnEnd = fnStart;
      while (fnEnd < line.Length && IsIdentifierChar(line[fnEnd]))
        fnEnd++;

      if (fnStart < fnEnd)
      {
        return (word, line[fnStart..fnEnd]);
      }
    }

    return (null, null);
  }

  private static string? ExtractSymbol(string line, int character)
  {
    if (character >= line.Length)
      character = Math.Max(0, line.Length - 1);
    if (line.Length == 0)
      return null;

    int start = character;
    int end = character;

    while (start > 0 && IsIdentifierChar(line[start - 1]))
      start--;
    while (end < line.Length && IsIdentifierChar(line[end]))
      end++;

    if (start == end)
      return null;

    return line[start..end];
  }

  private static bool IsIdentifierChar(char c) =>
    char.IsLetterOrDigit(c) || c == '_';

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
      _ => null
    };
  }
}
