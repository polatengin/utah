import * as vscode from 'vscode';
import * as path from 'path';
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
  TransportKind
} from 'vscode-languageclient/node';

let client: LanguageClient;

export async function activate(context: vscode.ExtensionContext) {
  try {
    context.subscriptions.push(
      vscode.commands.registerCommand('utah.showReferences',
        (uriStr: string, pos: { line: number; character: number }, locations: Array<{ uri: string; range: { start: { line: number; character: number }; end: { line: number; character: number } } }>) => {
          const uri = vscode.Uri.parse(uriStr);
          const position = new vscode.Position(pos.line, pos.character);
          const locs = (locations ?? []).map(l => new vscode.Location(
            vscode.Uri.parse(l.uri),
            new vscode.Range(l.range.start.line, l.range.start.character, l.range.end.line, l.range.end.character)
          ));
          return vscode.commands.executeCommand('editor.action.showReferences', uri, position, locs);
        }
      )
    );

    const config = vscode.workspace.getConfiguration('utah');
    const configuredPath = config.get<string>('server.path', 'utah');
    const configuredArgs = config.get<string[]>('server.args', ['lsp']);

    const utahPath = path.isAbsolute(configuredPath) ? configuredPath : path.join(__dirname, 'server', configuredPath);

    const serverOptions: ServerOptions = {
      run: {
        command: utahPath,
        args: configuredArgs,
        transport: TransportKind.stdio
      },
      debug: {
        command: utahPath,
        args: configuredArgs,
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
      initializationOptions: {}
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
