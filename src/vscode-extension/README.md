# Utah Shell Extension

Language support for [Utah](https://utahshx.com) shell scripts (`.shx` files) in Visual Studio Code.

Utah is a CLI transpiler that converts TypeScript-like `.shx` files into clean, standard `.sh` bash scripts.

## Features

- **Syntax Highlighting** — Full syntax highlighting for `.shx` files with TypeScript-like grammar support
- **Language Server** — Integrated language server for code intelligence powered by the Utah CLI
- **File Icons** — Custom file icons for `.shx` files

## Getting Started

1. Install the extension from the VS Code Marketplace
2. Install the Utah CLI:

   ```bash
   curl -sL https://utahshx.com/install.sh | sudo bash
   ```

3. Open any `.shx` file to activate syntax highlighting and language features

## Example

Write code in `.shx` using modern, TypeScript-like syntax:

```typescript
const appName: string = "MyApp";
let name: string = "Alice";

function greet(name: string): void {
  console.log(`Hi, ${name}!`);
}

if (name == "Alice") {
  greet(name);
} else {
  console.log("Unknown user");
}
```

Utah transpiles it into clean bash:

```bash
readonly appName="MyApp"
name="Alice"

greet() {
  local name="$1"
  echo "Hi, ${name}!"
}

if [ "$name" == "Alice" ]; then
  greet "$name"
else
  echo "Unknown user"
fi
```

## Extension Settings

| Setting | Default | Description |
|---------|---------|-------------|
| `utah.server.path` | `utah` | Path to the Utah CLI executable |
| `utah.server.args` | `["lsp"]` | Arguments passed to the Utah CLI for LSP mode |

## Documentation

For detailed documentation, visit [utahshx.com](https://utahshx.com).

## License

MIT
