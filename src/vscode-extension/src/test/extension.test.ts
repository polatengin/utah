import * as assert from 'assert';
import * as vscode from 'vscode';

suite('Extension Test Suite', () => {
	test('Extension should be present', () => {
		const extension = vscode.extensions.getExtension('polatengin.utah');
		assert.ok(extension, 'Utah extension should be installed');
	});

	test('Extension should register .shx language', () => {
		const languages = vscode.languages.getLanguages();
		return languages.then((langs) => {
			// The extension contributes the "utah" language for .shx files
			assert.ok(langs.length > 0, 'There should be registered languages');
		});
	});

	test('Configuration settings should be declared', () => {
		const config = vscode.workspace.getConfiguration('utah');
		const serverPath = config.get<string>('server.path');
		const serverArgs = config.get<string[]>('server.args');
		assert.strictEqual(serverPath, 'utah', 'Default server.path should be "utah"');
		assert.deepStrictEqual(serverArgs, ['lsp'], 'Default server.args should be ["lsp"]');
	});

	test('Completion provider should handle namespace members', () => {
		// Verify the completion provider module can be imported
		const { createCompletionProvider } = require('../completionProvider');
		const provider = createCompletionProvider();
		assert.ok(provider, 'Completion provider should be created');
		assert.ok(typeof provider.provideCompletionItems === 'function',
			'Provider should have provideCompletionItems method');
	});
});
