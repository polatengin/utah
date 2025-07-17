---
layout: default
title: VS Code Extension
parent: CLI Reference
nav_order: 4
---

The Utah VS Code extension provides comprehensive language support for Utah (.shx) files, including syntax highlighting, error checking, code completion, and formatting integration.

## Installation

### From VS Code Marketplace

1. Open VS Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search for "Utah Language Support"
4. Click Install

### Manual Installation

```bash
# Install from VSIX file
code --install-extension utah-language-support.vsix
```

## Features

### Syntax Highlighting

The extension provides full syntax highlighting for Utah files:

- **Keywords:** `let`, `const`, `function`, `if`, `for`, `while`, etc.
- **Types:** `string`, `number`, `boolean`, `array`, `object`
- **Built-in functions:** `console.log`, `fs.readFile`, `json.parse`, etc.
- **Comments:** Single-line (`//`) and multi-line (`/* */`)
- **Strings:** Template literals with interpolation support

### Error Detection

Real-time error checking and diagnostics:

```typescript
// Error: Type mismatch
let name: string = 42;  // Underlined in red

// Error: Undefined variable
console.log(unknownVar);  // Underlined in red

// Error: Missing semicolon
let count: number = 10  // Warning underline
```

### Code Completion

Intelligent code completion for:

- **Built-in functions:** Auto-complete for `console.*`, `fs.*`, `json.*`, etc.
- **Variables:** Context-aware variable suggestions
- **Function parameters:** Parameter hints with type information
- **Import statements:** File path completion for imports

### Language Server Integration

The extension uses Utah's language server (`utah lsp`) for advanced features:

- **Go to definition:** Navigate to function/variable definitions
- **Find references:** Find all usages of symbols
- **Symbol outline:** Document outline in Explorer
- **Hover information:** Type and documentation on hover

### Formatting Support

Automatic code formatting using `utah format`:

- **Format on save:** Automatically format when saving files
- **Format on paste:** Format pasted code
- **Format selection:** Format only selected code
- **Format document:** Format entire document

## Configuration

### VS Code Settings

Configure the extension in your VS Code settings:

```json
{
  // Utah-specific settings
  "utah.languageServer.enabled": true,
  "utah.languageServer.trace": "off",
  "utah.formatting.enabled": true,
  "utah.formatting.onSave": true,

  // Editor settings for .shx files
  "[shx]": {
    "editor.defaultFormatter": "utah-lang.utah-language-support",
    "editor.formatOnSave": true,
    "editor.insertSpaces": true,
    "editor.tabSize": 2,
    "editor.detectIndentation": false
  },

  // File associations
  "files.associations": {
    "*.shx": "shx"
  }
}
```

### Workspace Settings

Project-specific settings in `.vscode/settings.json`:

```json
{
  "utah.projectRoot": "${workspaceFolder}",
  "utah.formatting.indentSize": 2,
  "utah.linting.enabled": true,
  "utah.completion.enabled": true
}
```

### EditorConfig Integration

The extension respects `.editorconfig` files:

```ini
[*.shx]
indent_style = space
indent_size = 2
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
max_line_length = 100
```

## Commands

The extension provides several commands accessible via Command Palette (Ctrl+Shift+P):

| Command | Description | Keybinding |
|---------|-------------|------------|
| `Utah: Compile File` | Compile current .shx file | Ctrl+F7 |
| `Utah: Run File` | Run current .shx file | F5 |
| `Utah: Format Document` | Format current document | Shift+Alt+F |
| `Utah: Restart Language Server` | Restart language server | - |
| `Utah: Show Output` | Show Utah output panel | - |

### Custom Keybindings

Add custom keybindings in `keybindings.json`:

```json
[
  {
    "key": "ctrl+shift+b",
    "command": "utah.compile",
    "when": "resourceExtname == .shx"
  },
  {
    "key": "ctrl+f5",
    "command": "utah.run",
    "when": "resourceExtname == .shx"
  }
]
```

## Development Setup

### Extension Development

To work on the Utah VS Code extension:

```bash
# Clone the repository
git clone https://github.com/polatengin/utah.git
cd utah/src/vscode-extension

# Install dependencies
npm install

# Build the extension
npm run build

# Watch for changes during development
npm run watch
```

### Testing Extension

```bash
# Run extension in development mode
npm run dev

# Run tests
npm test

# Package extension
npm run package
```

### Language Server Development

The language server runs separately from the extension:

```bash
# Start language server manually
utah lsp

# Debug language server
UTAH_DEBUG=1 utah lsp
```

## Troubleshooting

### Common Issues

**Issue:** Syntax highlighting not working
**Solution:** Check file is saved with `.shx` extension

**Issue:** Language server not starting
**Solution:** Ensure Utah CLI is installed and in PATH

```bash
# Check Utah installation
which utah
utah --version

# Restart language server
Ctrl+Shift+P > "Utah: Restart Language Server"
```

**Issue:** Formatting not working
**Solution:** Check formatting is enabled in settings

```json
{
  "utah.formatting.enabled": true,
  "[shx]": {
    "editor.defaultFormatter": "utah-lang.utah-language-support"
  }
}
```

**Issue:** Code completion not working
**Solution:** Verify language server is running

```bash
# Check language server process
ps aux | grep "utah lsp"

# Check VS Code output panel
View > Output > Select "Utah Language Server"
```

### Debug Mode

Enable debug output for troubleshooting:

```json
{
  "utah.languageServer.trace": "verbose"
}
```

Check output in: View > Output > Utah Language Server

### Reset Extension

If issues persist, reset the extension:

1. Disable Utah Language Support extension
2. Reload VS Code
3. Re-enable extension
4. Restart language server

## Integration Examples

### Project Setup

Create `.vscode/settings.json` for Utah projects:

```json
{
  "utah.projectRoot": "${workspaceFolder}",
  "utah.formatting.onSave": true,
  "files.associations": {
    "*.shx": "shx"
  },
  "editor.defaultFormatter": "utah-lang.utah-language-support"
}
```

### Build Tasks

Add Utah build tasks in `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Utah: Compile",
      "type": "shell",
      "command": "utah",
      "args": ["compile", "${file}"],
      "group": "build",
      "presentation": {
        "echo": true,
        "reveal": "always",
        "focus": false,
        "panel": "shared"
      }
    },
    {
      "label": "Utah: Run",
      "type": "shell",
      "command": "utah",
      "args": ["run", "${file}"],
      "group": "test",
      "presentation": {
        "echo": true,
        "reveal": "always",
        "focus": false,
        "panel": "shared"
      }
    }
  ]
}
```

### Launch Configuration

Add debug configuration in `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Run Utah Script",
      "type": "node",
      "request": "launch",
      "program": "utah",
      "args": ["run", "${file}"],
      "console": "integratedTerminal",
      "cwd": "${workspaceFolder}"
    }
  ]
}
```

### Recommended Extensions

Extensions that work well with Utah:

```json
{
  "recommendations": [
    "utah-lang.utah-language-support",
    "ms-vscode.vscode-json",
    "editorconfig.editorconfig",
    "ms-vscode.vscode-typescript-next"
  ]
}
```

## Extension API

### Language Server Protocol

The Utah language server implements LSP features:

- **textDocument/completion:** Code completion
- **textDocument/hover:** Hover information
- **textDocument/definition:** Go to definition
- **textDocument/references:** Find references
- **textDocument/formatting:** Document formatting
- **textDocument/diagnostics:** Error checking

### Extension Commands

Available commands for extension integration:

- `utah.compile` - Compile current file
- `utah.run` - Run current file
- `utah.format` - Format current document
- `utah.restartLanguageServer` - Restart language server

### Configuration Schema

The extension contributes configuration options:

```json
{
  "contributes": {
    "configuration": {
      "title": "Utah",
      "properties": {
        "utah.languageServer.enabled": {
          "type": "boolean",
          "default": true,
          "description": "Enable Utah language server"
        },
        "utah.formatting.onSave": {
          "type": "boolean",
          "default": true,
          "description": "Format file on save"
        }
      }
    }
  }
}
```

## Best Practices

### 1. Configure for Team Development

```json
// .vscode/settings.json - Team settings
{
  "utah.formatting.onSave": true,
  "utah.formatting.indentSize": 2,
  "[shx]": {
    "editor.insertSpaces": true,
    "editor.tabSize": 2
  },
  "files.trimTrailingWhitespace": true,
  "files.insertFinalNewline": true
}
```

### 2. Use Consistent File Organization

```text
project/
├── .vscode/
│   ├── settings.json
│   ├── tasks.json
│   └── launch.json
├── src/
│   ├── main.shx
│   └── utils/
│       └── helpers.shx
└── README.md
```

### 3. Enable Auto-formatting

```json
{
  "editor.formatOnSave": true,
  "editor.formatOnPaste": true,
  "utah.formatting.enabled": true
}
```

### 4. Monitor Language Server

Check language server health regularly:

- View > Output > Utah Language Server
- Monitor for error messages
- Restart if issues occur

The VS Code extension provides a complete development environment for Utah, making it easy to write, debug, and maintain Utah scripts with full IDE support.
