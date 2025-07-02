import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {

  console.log('Congratulations, your extension "utah" is now active!');

  const disposable = vscode.commands.registerCommand('utah.helloWorld', () => {
    vscode.window.showInformationMessage('Hello World from utah!');
  });

  context.subscriptions.push(disposable);
}

export function deactivate() { }
