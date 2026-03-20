---
layout: default
title: VS Code Extension
parent: CLI Reference
nav_order: 4
---

The Utah VS Code extension currently provides syntax highlighting, `.shx` file association, file icons, and LSP-backed code completion for Utah files. It does **not** currently ship diagnostics, hover, go-to-definition, references, formatting, or command palette commands.

## Current Features

### Syntax Highlighting

The extension ships a TextMate grammar for Utah and highlights:

- Keywords such as `let`, `const`, `function`, `if`, `for`, and `while`
- Primitive types such as `string`, `number`, `boolean`, `array`, and `object`
- Built-in namespaces such as `console`, `fs`, `json`, `yaml`, `web`, and `ssh`
- Comments, strings, and string interpolation

### Code Completion

The extension starts Utah's language server (`utah lsp`) and currently exposes completion support for:

- Built-in namespaces and common namespace members
- String helper methods
- SSH connection members
- Completion requests triggered after `.`

### File Icons and Language Association

The packaged extension contributes:

- The `utah` language id for `.shx` files
- A Utah file icon theme for Utah source files
- A bundled language server startup path used by the extension at runtime

## Current Limitations

The current extension does not yet provide:

- Diagnostics / red squiggles
- Hover information
- Go to definition, find references, or document symbols
- Document formatting or format-on-save integration
- Contributed VS Code commands such as `Utah: Compile File` or `Utah: Restart Language Server`

The package manifest currently exposes `utah.server.path` and `utah.server.args`, but the runtime still launches the bundled Utah binary in `lsp` mode. Treat those settings as reserved until they are wired through.

## Installation and Development

To build the extension locally:

```bash
cd src/vscode-extension
npm install
npm run compile
```

During development:

```bash
npm run watch
```

Run the extension test suite:

```bash
npm test
```

If you already have a packaged VSIX artifact, install it with:

```bash
code --install-extension path/to/utah-language-support.vsix
```

## Using Utah from VS Code

A minimal workspace setup looks like this:

```json
{
  "files.associations": {
    "*.shx": "utah"
  }
}
```

### Recommended Tasks

Add Utah CLI tasks in `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Utah: Compile",
      "type": "shell",
      "command": "utah",
      "args": ["compile", "${file}"],
      "group": "build"
    },
    {
      "label": "Utah: Run",
      "type": "shell",
      "command": "utah",
      "args": ["run", "${file}"],
      "group": "test"
    },
    {
      "label": "Utah: Format",
      "type": "shell",
      "command": "utah",
      "args": ["format", "${file}", "--in-place"],
      "group": "build"
    }
  ]
}
```

## Language Server Development

The language server can be started manually for local debugging:

```bash
utah lsp
UTAH_DEBUG=1 utah lsp
```

## Troubleshooting

**Issue:** Syntax highlighting not working

**Solution:** Ensure the file uses the `.shx` extension and that VS Code has associated it with the `utah` language.

**Issue:** Code completion not working

**Solution:** Open the VS Code output panel and inspect `Utah Language Server` and `Utah Language Server Trace`. Completion is currently strongest after typing `.` on supported namespaces and values.

**Issue:** You need formatting inside VS Code

**Solution:** Use `utah format` from the integrated terminal or a VS Code task. The extension does not currently register a document formatter.

## Language Server Surface Today

The current Utah language server implementation exposes:

- `textDocument/completion`

## Planned Expansion

Advanced editor features are still planned work:

- Diagnostics
- Hover information
- Definition and reference lookup
- Document formatting
- Command palette actions for compile/run/restart
