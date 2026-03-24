using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class RenameHandler : IRenameHandler, IPrepareRenameHandler
{
  private static readonly TextDocumentSelector _selector = new(new TextDocumentFilter
  {
    Language = "utah",
    Pattern = "**/*.shx"
  });

  public RenameRegistrationOptions GetRegistrationOptions(RenameCapability capability, ClientCapabilities clientCapabilities)
  {
    return new RenameRegistrationOptions
    {
      DocumentSelector = _selector,
      PrepareProvider = true
    };
  }

  public async Task<RangeOrPlaceholderRange?> Handle(PrepareRenameParams request, CancellationToken cancellationToken)
  {
    try
    {
      var (text, _) = await GetDocumentTextAsync(request.TextDocument.Uri, cancellationToken);
      if (text == null) return null;

      var lines = text.Split('\n');
      if (request.Position.Line >= lines.Length) return null;

      var line = lines[(int)request.Position.Line];
      var (symbol, start, end) = ExtractSymbolAtPosition(line, (int)request.Position.Character);
      if (symbol == null) return null;

      // Only allow renaming user-defined symbols, not keywords or built-in namespaces
      if (IsKeyword(symbol) || IsBuiltInNamespace(symbol)) return null;

      // Verify the symbol is actually defined in the document
      if (!IsUserDefinedSymbol(lines, symbol)) return null;

      return new RangeOrPlaceholderRange(new PlaceholderRange
      {
        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
          new Position(request.Position.Line, start),
          new Position(request.Position.Line, end)
        ),
        Placeholder = symbol
      });
    }
    catch
    {
      return null;
    }
  }

  public async Task<WorkspaceEdit?> Handle(RenameParams request, CancellationToken cancellationToken)
  {
    try
    {
      var (text, uri) = await GetDocumentTextAsync(request.TextDocument.Uri, cancellationToken);
      if (text == null) return null;

      var lines = text.Split('\n');
      if (request.Position.Line >= lines.Length) return null;

      var line = lines[(int)request.Position.Line];
      var (symbol, _, _) = ExtractSymbolAtPosition(line, (int)request.Position.Character);
      if (symbol == null) return null;

      if (IsKeyword(symbol) || IsBuiltInNamespace(symbol)) return null;
      if (!IsUserDefinedSymbol(lines, symbol)) return null;

      var newName = request.NewName;
      if (string.IsNullOrWhiteSpace(newName) || !IsValidIdentifier(newName)) return null;

      var edits = FindAllOccurrences(lines, symbol, uri);
      if (edits.Count == 0) return null;

      var textEdits = edits.Select(e => new TextEdit
      {
        Range = e,
        NewText = newName
      }).ToArray();

      return new WorkspaceEdit
      {
        Changes = new Dictionary<OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri, IEnumerable<TextEdit>>
        {
          [uri] = textEdits
        }
      };
    }
    catch
    {
      return null;
    }
  }

  private static async Task<(string? text, OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri uri)> GetDocumentTextAsync(
    OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri uri, CancellationToken cancellationToken)
  {
    var text = DocumentManager.Instance.Get(uri.ToString());
    if (text == null)
    {
      var path = uri.GetFileSystemPath();
      if (File.Exists(path))
        text = await File.ReadAllTextAsync(path, cancellationToken);
    }
    return (text, uri);
  }

  private static (string? symbol, int start, int end) ExtractSymbolAtPosition(string line, int character)
  {
    if (character >= line.Length)
      character = Math.Max(0, line.Length - 1);
    if (line.Length == 0)
      return (null, 0, 0);

    int start = character;
    int end = character;

    while (start > 0 && IsIdentifierChar(line[start - 1]))
      start--;
    while (end < line.Length && IsIdentifierChar(line[end]))
      end++;

    if (start == end)
      return (null, 0, 0);

    return (line[start..end], start, end);
  }

  private static List<OmniSharp.Extensions.LanguageServer.Protocol.Models.Range> FindAllOccurrences(
    string[] lines, string symbol, OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri uri)
  {
    var ranges = new List<OmniSharp.Extensions.LanguageServer.Protocol.Models.Range>();
    var pattern = new Regex($@"\b{Regex.Escape(symbol)}\b");

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];

      // Skip comment lines
      var trimmed = line.TrimStart();
      if (trimmed.StartsWith("//")) continue;

      foreach (Match match in pattern.Matches(line))
      {
        var col = match.Index;

        // Skip occurrences inside string literals
        if (IsInsideString(line, col)) continue;

        // Skip occurrences that are part of a dotted namespace (e.g., console.log)
        if (IsPartOfNamespace(line, col, symbol)) continue;

        ranges.Add(new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
          new Position(i, col),
          new Position(i, col + symbol.Length)
        ));
      }
    }

    return ranges;
  }

  private static bool IsInsideString(string line, int position)
  {
    bool inSingle = false;
    bool inDouble = false;
    bool inBacktick = false;

    for (int i = 0; i < position && i < line.Length; i++)
    {
      var c = line[i];
      if (c == '\\' && i + 1 < line.Length)
      {
        i++; // skip escaped character
        continue;
      }

      if (c == '\'' && !inDouble && !inBacktick) inSingle = !inSingle;
      else if (c == '"' && !inSingle && !inBacktick) inDouble = !inDouble;
      else if (c == '`' && !inSingle && !inDouble) inBacktick = !inBacktick;
    }

    return inSingle || inDouble || inBacktick;
  }

  private static bool IsPartOfNamespace(string line, int col, string symbol)
  {
    // Check if preceded by a dot (e.g., console.log — "log" is OK but "console" part of namespace)
    if (col > 0 && line[col - 1] == '.') return true;

    // Check if followed by a dot and the symbol is a known namespace
    int end = col + symbol.Length;
    if (end < line.Length && line[end] == '.' && IsBuiltInNamespace(symbol)) return true;

    return false;
  }

  private static bool IsUserDefinedSymbol(string[] lines, string symbol)
  {
    var varPattern = new Regex($@"^(let|const)\s+{Regex.Escape(symbol)}\s*(?::\s*\w+(?:<[^>]+>)?(?:\[\])?)?\s*=");
    var funcPattern = new Regex($@"^function\s+{Regex.Escape(symbol)}\s*\(");
    var typePattern = new Regex($@"^(interface|record)\s+{Regex.Escape(symbol)}\s*\{{");
    var forInPattern = new Regex($@"^for\s*\(\s*(let|const)\s+{Regex.Escape(symbol)}\s");
    var forPattern = new Regex($@"^for\s*\(\s*(let|const)\s+{Regex.Escape(symbol)}\s*:");
    var paramPattern = new Regex($@"^function\s+\w+\s*\([^)]*\b{Regex.Escape(symbol)}\s*:\s*\w+");

    foreach (var line in lines)
    {
      var trimmed = line.TrimStart();
      if (varPattern.IsMatch(trimmed) || funcPattern.IsMatch(trimmed) ||
          typePattern.IsMatch(trimmed) || forInPattern.IsMatch(trimmed) ||
          forPattern.IsMatch(trimmed) || paramPattern.IsMatch(trimmed))
        return true;
    }

    return false;
  }

  private static bool IsValidIdentifier(string name)
  {
    if (string.IsNullOrEmpty(name)) return false;
    if (!char.IsLetter(name[0]) && name[0] != '_') return false;
    return name.All(c => char.IsLetterOrDigit(c) || c == '_');
  }

  private static bool IsKeyword(string word) =>
    word is "let" or "const" or "function" or "if" or "else" or "for" or "while" or "switch"
      or "case" or "default" or "break" or "continue" or "return" or "try" or "catch" or "finally"
      or "defer" or "exit" or "import" or "interface" or "record" or "true" or "false" or "in"
      or "typeof" or "void" or "string" or "number" or "boolean" or "any";

  private static bool IsBuiltInNamespace(string word) =>
    word is "console" or "fs" or "web" or "json" or "yaml" or "validate" or "process" or "os"
      or "git" or "docker" or "utility" or "timer" or "system" or "string" or "ssh" or "args"
      or "script" or "template" or "scheduler" or "array" or "math" or "date";

  private static bool IsIdentifierChar(char c) =>
    char.IsLetterOrDigit(c) || c == '_';
}
