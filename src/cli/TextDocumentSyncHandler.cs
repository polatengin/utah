using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

public class TextDocumentSyncHandler : IDidOpenTextDocumentHandler, IDidChangeTextDocumentHandler, IDidCloseTextDocumentHandler, IDidSaveTextDocumentHandler
{
  private readonly ILanguageServerFacade _server;

  private static readonly TextDocumentSelector _selector = new(new TextDocumentFilter
  {
    Language = "utah",
    Pattern = "**/*.shx"
  });

  public TextDocumentSyncHandler(ILanguageServerFacade server)
  {
    _server = server;
  }

  public TextDocumentOpenRegistrationOptions GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
  {
    return new TextDocumentOpenRegistrationOptions
    {
      DocumentSelector = _selector
    };
  }

  TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
  {
    return new TextDocumentChangeRegistrationOptions
    {
      DocumentSelector = _selector,
      SyncKind = TextDocumentSyncKind.Full
    };
  }

  TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
  {
    return new TextDocumentSaveRegistrationOptions
    {
      DocumentSelector = _selector,
      IncludeText = true
    };
  }

  TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
  {
    return new TextDocumentCloseRegistrationOptions
    {
      DocumentSelector = _selector
    };
  }

  public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
  {
    var uri = request.TextDocument.Uri.ToString();
    DocumentManager.Instance.Update(uri, request.TextDocument.Text);
    PublishDiagnostics(request.TextDocument.Uri, request.TextDocument.Text);
    return Unit.Task;
  }

  public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
  {
    var uri = request.TextDocument.Uri.ToString();
    if (request.ContentChanges.Any())
    {
      var text = request.ContentChanges.Last().Text;
      DocumentManager.Instance.Update(uri, text);
      PublishDiagnostics(request.TextDocument.Uri, text);
    }
    return Unit.Task;
  }

  public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
  {
    var uri = request.TextDocument.Uri.ToString();
    DocumentManager.Instance.Remove(uri);
    _server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
    {
      Uri = request.TextDocument.Uri,
      Diagnostics = new Container<Diagnostic>()
    });
    return Unit.Task;
  }

  public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
  {
    if (request.Text != null)
    {
      var uri = request.TextDocument.Uri.ToString();
      DocumentManager.Instance.Update(uri, request.Text);
      PublishDiagnostics(request.TextDocument.Uri, request.Text);
    }
    return Unit.Task;
  }

  private void PublishDiagnostics(DocumentUri documentUri, string text)
  {
    var diagnostics = new List<Diagnostic>();

    try
    {
      var parser = new Parser(text);
      var (program, errors) = parser.TryParse();

      foreach (var error in errors)
      {
        diagnostics.Add(new Diagnostic
        {
          Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
            new Position(error.Line, 0),
            new Position(error.Line, 1000)
          ),
          Severity = DiagnosticSeverity.Error,
          Source = "utah",
          Message = error.Message
        });
      }
    }
    catch (Exception ex)
    {
      // Fallback: if TryParse itself fails, try to find the error line
      var errorLine = FindErrorLine(text, ex.Message);
      diagnostics.Add(new Diagnostic
      {
        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(
          new Position(errorLine, 0),
          new Position(errorLine, 1000)
        ),
        Severity = DiagnosticSeverity.Error,
        Source = "utah",
        Message = ex.Message
      });
    }

    _server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
    {
      Uri = documentUri,
      Diagnostics = new Container<Diagnostic>(diagnostics)
    });
  }

  private static int FindErrorLine(string text, string errorMessage)
  {
    // Try to extract a line reference from the error message
    var lines = text.Split('\n');

    // Many error messages include the problematic line content after ": "
    var colonIdx = errorMessage.LastIndexOf(": ");
    if (colonIdx > 0)
    {
      var fragment = errorMessage.Substring(colonIdx + 2).Trim();
      for (int i = 0; i < lines.Length; i++)
      {
        if (lines[i].Trim().Contains(fragment))
          return i;
      }
    }

    return 0;
  }
}
