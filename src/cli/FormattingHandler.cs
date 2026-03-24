using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class FormattingHandler : IDocumentFormattingHandler
{
  public DocumentFormattingRegistrationOptions GetRegistrationOptions(DocumentFormattingCapability capability, ClientCapabilities clientCapabilities)
  {
    return new DocumentFormattingRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      })
    };
  }

  public async Task<TextEditContainer?> Handle(DocumentFormattingParams request, CancellationToken cancellationToken)
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

      var options = new FormattingOptions();

      // Map LSP formatting options to Utah formatting options
      if (request.Options.InsertSpaces)
      {
        options.IndentStyle = "space";
        options.IndentSize = (int)request.Options.TabSize;
      }
      else
      {
        options.IndentStyle = "tab";
      }

      // Try to load EditorConfig settings from the file path
      try
      {
        var filePath = uri.GetFileSystemPath();
        if (!string.IsNullOrEmpty(filePath))
        {
          options = FormattingOptions.FromEditorConfig(filePath);
          // Still respect LSP options for indent
          if (request.Options.InsertSpaces)
          {
            options.IndentStyle = "space";
            options.IndentSize = (int)request.Options.TabSize;
          }
          else
          {
            options.IndentStyle = "tab";
          }
        }
      }
      catch
      {
        // Ignore EditorConfig errors
      }

      var formatter = new Formatter(options);
      var formatted = formatter.FormatContent(text);

      var lines = text.Split('\n');
      var lastLine = lines.Length > 0 ? lines.Length - 1 : 0;
      var lastCol = lines.Length > 0 ? lines[lastLine].Length : 0;

      return new TextEditContainer(new TextEdit
      {
        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
          new Position(0, 0),
          new Position(lastLine, lastCol)
        ),
        NewText = formatted
      });
    }
    catch
    {
      return null;
    }
  }
}
