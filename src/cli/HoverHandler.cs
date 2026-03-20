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
      var documentPath = request.TextDocument.Uri.GetFileSystemPath();
      if (!File.Exists(documentPath))
      {
        return null;
      }

      var documentText = await File.ReadAllTextAsync(documentPath, cancellationToken);
      var lines = documentText.Split('\n');

      if (request.Position.Line >= lines.Length)
      {
        return null;
      }

      var line = lines[(int)request.Position.Line];
      var (namespaceName, functionName) = ExtractSymbolAtPosition(line, (int)request.Position.Character);

      if (namespaceName == null || functionName == null)
      {
        return null;
      }

      var completions = GetCompletionsForNamespace(namespaceName);
      if (completions == null)
      {
        return null;
      }

      var match = completions.Find(c => c.Label == functionName);
      if (match == null)
      {
        return null;
      }

      var markdown = $"```utah\n{namespaceName}.{match.Detail}\n```\n\n{match.Documentation}";

      return new Hover
      {
        Contents = new MarkedStringsOrMarkupContent(new MarkupContent
        {
          Kind = MarkupKind.Markdown,
          Value = markdown
        })
      };
    }
    catch
    {
      return null;
    }
  }

  private static (string? namespaceName, string? functionName) ExtractSymbolAtPosition(string line, int character)
  {
    // Find word boundaries around cursor
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
