import * as vscode from 'vscode';
import * as path from 'path';
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
  TransportKind
} from 'vscode-languageclient/node';

let client: LanguageClient;

export async function activate() {
  try {
    const utahPath = path.join(__dirname, 'server', 'utah');

    const serverOptions: ServerOptions = {
      run: {
        command: utahPath,
        args: ['lsp'],
        transport: TransportKind.stdio
      },
      debug: {
        command: utahPath,
        args: ['lsp'],
        transport: TransportKind.stdio
      }
    };

    const clientOptions: LanguageClientOptions = {
      documentSelector: [{ scheme: 'file', language: 'utah' }],
      synchronize: {
        fileEvents: vscode.workspace.createFileSystemWatcher('**/*.shx')
      },
      outputChannel: vscode.window.createOutputChannel('Utah Language Server'),
      traceOutputChannel: vscode.window.createOutputChannel('Utah Language Server Trace'),
      revealOutputChannelOn: 1, // RevealOutputChannelOn.Error = 1 (show on errors)
      initializationOptions: {},
      middleware: {
        provideCompletionItem: (document, position, context, token, next) => {
          console.log('Completion middleware called:', {
            document: document.uri.toString(),
            position: position,
            context: context,
            triggerCharacter: context.triggerCharacter,
            triggerKind: context.triggerKind
          });
          return next(document, position, context, token);
        }
      }
    };

    client = new LanguageClient(
      'utah',
      'Utah Language Server',
      serverOptions,
      clientOptions
    );

    const startupTimeout = setTimeout(() => {
      console.error('LSP server startup timeout - server may not be responding');
      vscode.window.showErrorMessage('Utah Language Server startup timeout - server may not be responding');
    }, 10000); // 10 second timeout

    try {
      await client.start();
      clearTimeout(startupTimeout);
      console.log('Utah Language Server started successfully');
      vscode.window.showInformationMessage('Utah Language Server started successfully');
    } catch (error) {
      clearTimeout(startupTimeout);
      console.error('Failed to start LSP client:', error);
      vscode.window.showErrorMessage(`Failed to start Utah Language Server: ${error}`);
      throw error;
    }

    client.onDidChangeState((event) => {
      console.log('LSP client state changed:', event);
    });

    client.onRequest('textDocument/completion', (params) => {
      console.log('Completion request intercepted:', params);
      return params;
    });

    const completionProvider = vscode.languages.registerCompletionItemProvider(
      { scheme: 'file', language: 'utah' },
      {
        provideCompletionItems(document, position, _token, context) {
          console.log('Direct completion provider called:', {
            document: document.uri.toString(),
            position: position,
            context: context,
            triggerCharacter: context.triggerCharacter,
            triggerKind: context.triggerKind
          });

          // Let the LSP handle it, but log that we were called
          return null;
        }
      },
      '.' // Trigger character
    );

    console.log('Registered direct completion provider for debugging');

    // Test document language detection
    const activeEditor = vscode.window.activeTextEditor;
    if (activeEditor) {
      console.log('Active editor document:', {
        uri: activeEditor.document.uri.toString(),
        languageId: activeEditor.document.languageId,
        fileName: activeEditor.document.fileName
      });
    }

    // Listen for active editor changes
    vscode.window.onDidChangeActiveTextEditor((editor) => {
      if (editor) {
        console.log('Active editor changed:', {
          uri: editor.document.uri.toString(),
          languageId: editor.document.languageId,
          fileName: editor.document.fileName
        });
      }
    });
  } catch (error) {
    console.error('Failed to start Utah Language Server:', error);
    vscode.window.showErrorMessage(`Failed to start Utah Language Server: ${error}`);
  }
}

export function deactivate(): Thenable<void> | undefined {
  if (!client) {
    return undefined;
  }
  return client.stop();
}
