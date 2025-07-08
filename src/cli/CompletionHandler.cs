using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class CompletionHandler : ICompletionHandler
{
  public CompletionRegistrationOptions GetRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities)
  {
    Console.Error.WriteLine("[LSP] Registering completion handler");

    return new CompletionRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      }),
      TriggerCharacters = new Container<string>("."),
      ResolveProvider = false
    };
  }

  public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
  {
    await Console.Error.WriteLineAsync($"[LSP] Completion request received for {request.TextDocument.Uri}");

    var completionItems = new List<CompletionItem>
        {
            // Keywords
            new CompletionItem
            {
                Label = "let",
                Kind = CompletionItemKind.Keyword,
                Detail = "let variable: type = value",
                Documentation = "Declare a mutable variable"
            },
            new CompletionItem
            {
                Label = "const",
                Kind = CompletionItemKind.Keyword,
                Detail = "const variable: type = value",
                Documentation = "Declare a constant variable"
            },
            new CompletionItem
            {
                Label = "function",
                Kind = CompletionItemKind.Keyword,
                Detail = "function name(params) { ... }",
                Documentation = "Declare a function"
            },
            new CompletionItem
            {
                Label = "if",
                Kind = CompletionItemKind.Keyword,
                Detail = "if (condition) { ... }",
                Documentation = "Conditional statement"
            },
            new CompletionItem
            {
                Label = "else",
                Kind = CompletionItemKind.Keyword,
                Detail = "else { ... }",
                Documentation = "Alternative branch for if statement"
            },
            new CompletionItem
            {
                Label = "for",
                Kind = CompletionItemKind.Keyword,
                Detail = "for (init; condition; increment) { ... }",
                Documentation = "For loop"
            },
            new CompletionItem
            {
                Label = "while",
                Kind = CompletionItemKind.Keyword,
                Detail = "while (condition) { ... }",
                Documentation = "While loop"
            },
            new CompletionItem
            {
                Label = "return",
                Kind = CompletionItemKind.Keyword,
                Detail = "return value",
                Documentation = "Return a value from function"
            },
            new CompletionItem
            {
                Label = "true",
                Kind = CompletionItemKind.Keyword,
                Detail = "true",
                Documentation = "Boolean true value"
            },
            new CompletionItem
            {
                Label = "false",
                Kind = CompletionItemKind.Keyword,
                Detail = "false",
                Documentation = "Boolean false value"
            },

            // Namespaces/Objects
            new CompletionItem
            {
                Label = "console",
                Kind = CompletionItemKind.Module,
                Detail = "console",
                Documentation = "Console operations namespace"
            },
            new CompletionItem
            {
                Label = "utility",
                Kind = CompletionItemKind.Module,
                Detail = "utility",
                Documentation = "Utility functions namespace"
            },
            new CompletionItem
            {
                Label = "timer",
                Kind = CompletionItemKind.Module,
                Detail = "timer",
                Documentation = "Timer operations namespace"
            },
            new CompletionItem
            {
                Label = "string",
                Kind = CompletionItemKind.Module,
                Detail = "string",
                Documentation = "String operations namespace"
            },
            new CompletionItem
            {
                Label = "math",
                Kind = CompletionItemKind.Module,
                Detail = "math",
                Documentation = "Math operations namespace"
            },

            // Common methods (when triggered by dot)
            new CompletionItem
            {
                Label = "log",
                Kind = CompletionItemKind.Method,
                Detail = "log(message: string)",
                Documentation = "Print a message to the console"
            },
            new CompletionItem
            {
                Label = "error",
                Kind = CompletionItemKind.Method,
                Detail = "error(message: string)",
                Documentation = "Print an error message to the console"
            },
            new CompletionItem
            {
                Label = "warn",
                Kind = CompletionItemKind.Method,
                Detail = "warn(message: string)",
                Documentation = "Print a warning message to the console"
            },
            new CompletionItem
            {
                Label = "sleep",
                Kind = CompletionItemKind.Method,
                Detail = "sleep(milliseconds: number)",
                Documentation = "Pause execution for specified milliseconds"
            },
            new CompletionItem
            {
                Label = "length",
                Kind = CompletionItemKind.Property,
                Detail = "length: number",
                Documentation = "Get the length of a string or array"
            },
            new CompletionItem
            {
                Label = "substring",
                Kind = CompletionItemKind.Method,
                Detail = "substring(start: number, end?: number)",
                Documentation = "Extract a substring from a string"
            },
            new CompletionItem
            {
                Label = "split",
                Kind = CompletionItemKind.Method,
                Detail = "split(separator: string)",
                Documentation = "Split a string into an array"
            },
            new CompletionItem
            {
                Label = "join",
                Kind = CompletionItemKind.Method,
                Detail = "join(separator: string)",
                Documentation = "Join array elements into a string"
            }
        };

    await Console.Error.WriteLineAsync($"[LSP] Returning {completionItems.Count} completion items");
    return new CompletionList(completionItems);
  }
}
