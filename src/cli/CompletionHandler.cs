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

    var completionItems = new List<CompletionItem>();

    // Check if this was triggered by a dot
    bool isDotTriggered = request.Context?.TriggerCharacter == ".";

    await Console.Error.WriteLineAsync($"[LSP] Completion triggered by dot: {isDotTriggered}");

    if (isDotTriggered)
    {
      // Show only methods and properties when triggered by dot
      completionItems.AddRange(new List<CompletionItem>
      {
        // Console methods
        new CompletionItem
        {
          Label = "log",
          Kind = CompletionItemKind.Method,
          Detail = "log(message: string)",
          Documentation = "Print a message to the console"
        },
        new CompletionItem
        {
          Label = "isSudo",
          Kind = CompletionItemKind.Method,
          Detail = "isSudo(): boolean",
          Documentation = "Check if the script is running with sudo privileges"
        },
        new CompletionItem
        {
          Label = "promptYesNo",
          Kind = CompletionItemKind.Method,
          Detail = "promptYesNo(prompt: string): boolean",
          Documentation = "Display a yes/no prompt and return the user's choice"
        },
        
        // Timer methods
        new CompletionItem
        {
          Label = "sleep",
          Kind = CompletionItemKind.Method,
          Detail = "sleep(milliseconds: number)",
          Documentation = "Pause execution for specified milliseconds"
        },
        new CompletionItem
        {
          Label = "timeout",
          Kind = CompletionItemKind.Method,
          Detail = "timeout(milliseconds: number)",
          Documentation = "Set a timeout"
        },
        
        // String methods
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
          Label = "toUpperCase",
          Kind = CompletionItemKind.Method,
          Detail = "toUpperCase()",
          Documentation = "Convert string to uppercase"
        },
        new CompletionItem
        {
          Label = "toLowerCase",
          Kind = CompletionItemKind.Method,
          Detail = "toLowerCase()",
          Documentation = "Convert string to lowercase"
        },
        new CompletionItem
        {
          Label = "trim",
          Kind = CompletionItemKind.Method,
          Detail = "trim()",
          Documentation = "Remove whitespace from both ends"
        },
        
        // Utility methods
        new CompletionItem
        {
          Label = "getEnv",
          Kind = CompletionItemKind.Method,
          Detail = "getEnv(name: string)",
          Documentation = "Get environment variable value"
        },
        new CompletionItem
        {
          Label = "exit",
          Kind = CompletionItemKind.Method,
          Detail = "exit(code?: number)",
          Documentation = "Exit the program"
        },
        
        // Array methods
        new CompletionItem
        {
          Label = "join",
          Kind = CompletionItemKind.Method,
          Detail = "join(separator: string)",
          Documentation = "Join array elements into a string"
        },
        new CompletionItem
        {
          Label = "push",
          Kind = CompletionItemKind.Method,
          Detail = "push(item: any)",
          Documentation = "Add item to end of array"
        },
        new CompletionItem
        {
          Label = "pop",
          Kind = CompletionItemKind.Method,
          Detail = "pop()",
          Documentation = "Remove and return last item from array"
        }
      });
    }
    else
    {
      // Show keywords and namespaces when not triggered by dot
      completionItems.AddRange(new List<CompletionItem>
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
        new CompletionItem
        {
          Label = "try",
          Kind = CompletionItemKind.Keyword,
          Detail = "try { ... } catch (e) { ... }",
          Documentation = "Try-catch error handling"
        },
        new CompletionItem
        {
          Label = "catch",
          Kind = CompletionItemKind.Keyword,
          Detail = "catch (error) { ... }",
          Documentation = "Catch block for error handling"
        },
        new CompletionItem
        {
          Label = "switch",
          Kind = CompletionItemKind.Keyword,
          Detail = "switch (expression) { ... }",
          Documentation = "Switch statement"
        },
        new CompletionItem
        {
          Label = "case",
          Kind = CompletionItemKind.Keyword,
          Detail = "case value: ...",
          Documentation = "Case clause in switch statement"
        },
        new CompletionItem
        {
          Label = "default",
          Kind = CompletionItemKind.Keyword,
          Detail = "default: ...",
          Documentation = "Default clause in switch statement"
        },
        new CompletionItem
        {
          Label = "break",
          Kind = CompletionItemKind.Keyword,
          Detail = "break",
          Documentation = "Break out of loop or switch"
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
        new CompletionItem
        {
          Label = "fs",
          Kind = CompletionItemKind.Module,
          Detail = "fs",
          Documentation = "File system operations namespace"
        },
        new CompletionItem
        {
          Label = "os",
          Kind = CompletionItemKind.Module,
          Detail = "os",
          Documentation = "Operating system operations namespace"
        },
        new CompletionItem
        {
          Label = "process",
          Kind = CompletionItemKind.Module,
          Detail = "process",
          Documentation = "Process operations namespace"
        }
      });
    }

    await Console.Error.WriteLineAsync($"[LSP] Returning {completionItems.Count} completion items");
    return new CompletionList(completionItems);
  }
}
