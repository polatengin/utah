using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class DocumentSymbolHandler : IDocumentSymbolHandler
{
  public DocumentSymbolRegistrationOptions GetRegistrationOptions(DocumentSymbolCapability capability, ClientCapabilities clientCapabilities)
  {
    return new DocumentSymbolRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      })
    };
  }

  public async Task<SymbolInformationOrDocumentSymbolContainer?> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
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

      var symbols = ExtractSymbols(text);
      return new SymbolInformationOrDocumentSymbolContainer(
        symbols.Select(s => new SymbolInformationOrDocumentSymbol(s))
      );
    }
    catch
    {
      return null;
    }
  }

  private static List<DocumentSymbol> ExtractSymbols(string text)
  {
    var symbols = new List<DocumentSymbol>();
    var lines = text.Split('\n');

    for (int i = 0; i < lines.Length; i++)
    {
      var trimmed = lines[i].TrimStart();
      Match match;

      // Function declarations: function name(...): returnType {
      match = Regex.Match(trimmed, @"^function\s+(\w+)\s*\(([^)]*)\)(?:\s*:\s*(\w+(?:\[\])?))?");
      if (match.Success)
      {
        var name = match.Groups[1].Value;
        var parameters = match.Groups[2].Value;
        var returnType = match.Groups[3].Success ? match.Groups[3].Value : "void";
        var endLine = FindClosingBrace(lines, i);
        var col = lines[i].IndexOf("function", StringComparison.Ordinal);

        symbols.Add(new DocumentSymbol
        {
          Name = name,
          Detail = $"({parameters}): {returnType}",
          Kind = SymbolKind.Function,
          Range = CreateRange(i, Math.Max(0, col), endLine, lines[endLine].Length),
          SelectionRange = CreateRange(i, Math.Max(0, lines[i].IndexOf(name, StringComparison.Ordinal)), i, lines[i].IndexOf(name, StringComparison.Ordinal) + name.Length)
        });
        continue;
      }

      // Variable declarations: let name: type = ... or const name: type = ...
      match = Regex.Match(trimmed, @"^(let|const)\s+(\w+)\s*(?::\s*(\w+(?:<[^>]+>)?(?:\[\])?))?");
      if (match.Success)
      {
        var kind = match.Groups[1].Value;
        var name = match.Groups[2].Value;
        var type = match.Groups[3].Success ? match.Groups[3].Value : "any";
        var col = lines[i].IndexOf(name, StringComparison.Ordinal);

        symbols.Add(new DocumentSymbol
        {
          Name = name,
          Detail = $"{kind} {name}: {type}",
          Kind = kind == "const" ? SymbolKind.Constant : SymbolKind.Variable,
          Range = CreateRange(i, 0, i, lines[i].Length),
          SelectionRange = CreateRange(i, Math.Max(0, col), i, col + name.Length)
        });
        continue;
      }

      // Structured type declarations: interface Name { or record Name {
      match = Regex.Match(trimmed, @"^(interface|record)\s+(\w+)\s*\{");
      if (match.Success)
      {
        var typeKind = match.Groups[1].Value;
        var name = match.Groups[2].Value;
        var endLine = FindClosingBrace(lines, i);
        var col = lines[i].IndexOf(name, StringComparison.Ordinal);

        var typeSymbol = new DocumentSymbol
        {
          Name = name,
          Detail = typeKind,
          Kind = typeKind == "interface" ? SymbolKind.Interface : SymbolKind.Struct,
          Range = CreateRange(i, 0, endLine, lines[endLine].Length),
          SelectionRange = CreateRange(i, Math.Max(0, col), i, col + name.Length),
          Children = new Container<DocumentSymbol>(ExtractTypeFields(lines, i + 1, endLine))
        };
        symbols.Add(typeSymbol);
        continue;
      }

      // Import statements
      match = Regex.Match(trimmed, @"^import\s+""([^""]+)""");
      if (match.Success)
      {
        var importPath = match.Groups[1].Value;
        symbols.Add(new DocumentSymbol
        {
          Name = importPath,
          Detail = "import",
          Kind = SymbolKind.Module,
          Range = CreateRange(i, 0, i, lines[i].Length),
          SelectionRange = CreateRange(i, 0, i, lines[i].Length)
        });
        continue;
      }
    }

    return symbols;
  }

  private static List<DocumentSymbol> ExtractTypeFields(string[] lines, int startLine, int endLine)
  {
    var fields = new List<DocumentSymbol>();

    for (int i = startLine; i < endLine && i < lines.Length; i++)
    {
      var trimmed = lines[i].Trim();
      var match = Regex.Match(trimmed, @"^(\w+)\s*:\s*(.+?)(?:;|$)");
      if (match.Success)
      {
        var name = match.Groups[1].Value;
        var type = match.Groups[2].Value.TrimEnd(';').Trim();
        var col = lines[i].IndexOf(name, StringComparison.Ordinal);

        fields.Add(new DocumentSymbol
        {
          Name = name,
          Detail = type,
          Kind = SymbolKind.Field,
          Range = CreateRange(i, 0, i, lines[i].Length),
          SelectionRange = CreateRange(i, Math.Max(0, col), i, col + name.Length)
        });
      }
    }

    return fields;
  }

  private static int FindClosingBrace(string[] lines, int startLine)
  {
    int depth = 0;
    for (int i = startLine; i < lines.Length; i++)
    {
      foreach (char c in lines[i])
      {
        if (c == '{') depth++;
        else if (c == '}') depth--;
      }
      if (depth <= 0)
        return i;
    }
    return Math.Min(startLine + 1, lines.Length - 1);
  }

  private static OmniSharp.Extensions.LanguageServer.Protocol.Models.Range CreateRange(int startLine, int startCol, int endLine, int endCol)
  {
    return new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
      new Position(startLine, Math.Max(0, startCol)),
      new Position(endLine, Math.Max(0, endCol))
    );
  }
}
