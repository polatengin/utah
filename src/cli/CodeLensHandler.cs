using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class CodeLensHandler : ICodeLensHandler
{
  private static readonly TextDocumentSelector _selector = new(new TextDocumentFilter
  {
    Language = "utah",
    Pattern = "**/*.shx"
  });

  public CodeLensRegistrationOptions GetRegistrationOptions(CodeLensCapability capability, ClientCapabilities clientCapabilities)
  {
    return new CodeLensRegistrationOptions
    {
      DocumentSelector = _selector,
      ResolveProvider = false
    };
  }

  public async Task<CodeLensContainer?> Handle(CodeLensParams request, CancellationToken cancellationToken)
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
      var lenses = new List<CodeLens>();

      for (int i = 0; i < lines.Length; i++)
      {
        var trimmed = lines[i].TrimStart();
        Match match;

        // Function declarations
        match = Regex.Match(trimmed, @"^function\s+(\w+)\s*\(");
        if (match.Success)
        {
          var name = match.Groups[1].Value;
          var col = lines[i].IndexOf("function", StringComparison.Ordinal);
          var refCount = CountReferences(lines, name, i);

          lenses.Add(CreateLens(i, col, name, refCount, "function"));
          continue;
        }

        // Variable declarations: let name or const name
        match = Regex.Match(trimmed, @"^(let|const)\s+(\w+)\s*(?::\s*\w+(?:<[^>]+>)?(?:\[\])?)?\s*=");
        if (match.Success)
        {
          var name = match.Groups[2].Value;
          var col = lines[i].IndexOf(name, StringComparison.Ordinal);
          var refCount = CountReferences(lines, name, i);

          lenses.Add(CreateLens(i, col, name, refCount, "variable"));
          continue;
        }

        // Interface/record declarations
        match = Regex.Match(trimmed, @"^(interface|record)\s+(\w+)\s*\{");
        if (match.Success)
        {
          var name = match.Groups[2].Value;
          var col = lines[i].IndexOf(name, StringComparison.Ordinal);
          var refCount = CountReferences(lines, name, i);

          lenses.Add(CreateLens(i, col, name, refCount, "type"));
          continue;
        }
      }

      return new CodeLensContainer(lenses);
    }
    catch
    {
      return null;
    }
  }

  private static CodeLens CreateLens(int line, int col, string symbolName, int refCount, string symbolKind)
  {
    var label = refCount switch
    {
      0 => "no references",
      1 => "1 reference",
      _ => $"{refCount} references"
    };

    return new CodeLens
    {
      Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
        new Position(line, Math.Max(0, col)),
        new Position(line, Math.Max(0, col) + symbolName.Length)
      ),
      Command = new Command
      {
        Title = label,
        Name = "utah.showReferences",
        Arguments = new Newtonsoft.Json.Linq.JArray(symbolName, line)
      }
    };
  }

  private static int CountReferences(string[] lines, string symbol, int definitionLine)
  {
    int count = 0;
    var pattern = new Regex($@"\b{Regex.Escape(symbol)}\b");

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];
      var trimmed = line.TrimStart();

      // Skip comment lines
      if (trimmed.StartsWith("//")) continue;

      foreach (Match match in pattern.Matches(line))
      {
        var col = match.Index;

        // Skip if inside string literal
        if (IsInsideString(line, col)) continue;

        // Skip the definition itself
        if (i == definitionLine && IsDefinitionOccurrence(trimmed, symbol))
          continue;

        // Skip occurrences that are part of a built-in namespace prefix (e.g., "console" in "console.log")
        if (IsNamespacePrefix(line, col, symbol)) continue;

        count++;
      }
    }

    return count;
  }

  private static bool IsDefinitionOccurrence(string trimmed, string symbol)
  {
    // Check if this line is the definition line for this symbol
    return Regex.IsMatch(trimmed, $@"^(let|const)\s+{Regex.Escape(symbol)}\s*(?::\s*\w+(?:<[^>]+>)?(?:\[\])?)?\s*=") ||
           Regex.IsMatch(trimmed, $@"^function\s+{Regex.Escape(symbol)}\s*\(") ||
           Regex.IsMatch(trimmed, $@"^(interface|record)\s+{Regex.Escape(symbol)}\s*\{{");
  }

  private static bool IsNamespacePrefix(string line, int col, string symbol)
  {
    int end = col + symbol.Length;
    if (end < line.Length && line[end] == '.' && IsBuiltInNamespace(symbol))
      return true;
    return false;
  }

  private static bool IsInsideString(string line, int position)
  {
    bool inSingle = false, inDouble = false, inBacktick = false;

    for (int i = 0; i < position && i < line.Length; i++)
    {
      var c = line[i];
      if (c == '\\' && i + 1 < line.Length) { i++; continue; }

      if (c == '\'' && !inDouble && !inBacktick) inSingle = !inSingle;
      else if (c == '"' && !inSingle && !inBacktick) inDouble = !inDouble;
      else if (c == '`' && !inSingle && !inDouble) inBacktick = !inBacktick;
    }

    return inSingle || inDouble || inBacktick;
  }

  private static bool IsBuiltInNamespace(string word) =>
    word is "console" or "fs" or "web" or "json" or "yaml" or "validate" or "process" or "os"
      or "git" or "docker" or "utility" or "timer" or "system" or "string" or "ssh" or "args"
      or "script" or "template" or "scheduler" or "array" or "math" or "date";
}
