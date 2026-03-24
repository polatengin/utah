using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class DefinitionHandler : IDefinitionHandler
{
  public DefinitionRegistrationOptions GetRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities)
  {
    return new DefinitionRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      })
    };
  }

  public async Task<LocationOrLocationLinks?> Handle(DefinitionParams request, CancellationToken cancellationToken)
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
      var symbol = ExtractSymbolAtPosition(line, (int)request.Position.Character);
      if (symbol == null)
        return null;

      var definition = FindDefinition(lines, symbol, uri);
      if (definition == null)
        return null;

      return new LocationOrLocationLinks(definition);
    }
    catch
    {
      return null;
    }
  }

  private static string? ExtractSymbolAtPosition(string line, int character)
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

  private static Location? FindDefinition(string[] lines, string symbol, OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri uri)
  {
    // Search for variable declarations: let name: type = ... or const name: type = ...
    var varPattern = new Regex($@"^(let|const)\s+{Regex.Escape(symbol)}\s*(?::\s*\w+(?:<[^>]+>)?(?:\[\])?)?\s*=");

    // Search for function declarations: function name(...)
    var funcPattern = new Regex($@"^function\s+{Regex.Escape(symbol)}\s*\(");

    // Search for structured type declarations: interface Name { or record Name {
    var typePattern = new Regex($@"^(interface|record)\s+{Regex.Escape(symbol)}\s*\{{");

    // Search for for-in loop variable: for (let name in ...)
    var forInPattern = new Regex($@"^for\s*\(\s*(let|const)\s+{Regex.Escape(symbol)}\s");

    // Search for for loop initializer: for (let name: type = ...)
    var forPattern = new Regex($@"^for\s*\(\s*(let|const)\s+{Regex.Escape(symbol)}\s*:");

    // Search for function parameters: function foo(name: type, ...)
    var paramPattern = new Regex($@"^function\s+\w+\s*\([^)]*\b{Regex.Escape(symbol)}\s*:\s*\w+");

    for (int i = 0; i < lines.Length; i++)
    {
      var trimmed = lines[i].TrimStart();
      Match match;

      if ((match = varPattern.Match(trimmed)).Success)
      {
        var col = lines[i].IndexOf(symbol, StringComparison.Ordinal);
        return CreateLocation(uri, i, col, symbol.Length);
      }

      if ((match = funcPattern.Match(trimmed)).Success)
      {
        var col = lines[i].IndexOf(symbol, StringComparison.Ordinal);
        return CreateLocation(uri, i, col, symbol.Length);
      }

      if ((match = typePattern.Match(trimmed)).Success)
      {
        var col = lines[i].IndexOf(symbol, StringComparison.Ordinal);
        return CreateLocation(uri, i, col, symbol.Length);
      }

      if ((match = forInPattern.Match(trimmed)).Success)
      {
        var col = lines[i].IndexOf(symbol, StringComparison.Ordinal);
        return CreateLocation(uri, i, col, symbol.Length);
      }

      if ((match = forPattern.Match(trimmed)).Success)
      {
        var col = lines[i].IndexOf(symbol, StringComparison.Ordinal);
        return CreateLocation(uri, i, col, symbol.Length);
      }

      if ((match = paramPattern.Match(trimmed)).Success)
      {
        // Find the parameter name within the parameter list
        var paramStart = lines[i].IndexOf('(');
        if (paramStart >= 0)
        {
          var paramIdx = lines[i].IndexOf(symbol, paramStart, StringComparison.Ordinal);
          if (paramIdx >= 0)
            return CreateLocation(uri, i, paramIdx, symbol.Length);
        }
      }
    }

    return null;
  }

  private static Location CreateLocation(OmniSharp.Extensions.LanguageServer.Protocol.DocumentUri uri, int line, int col, int length)
  {
    if (col < 0) col = 0;
    return new Location
    {
      Uri = uri,
      Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
        new Position(line, col),
        new Position(line, col + length)
      )
    };
  }

  private static bool IsIdentifierChar(char c) =>
    char.IsLetterOrDigit(c) || c == '_';
}
