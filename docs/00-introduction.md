---
layout: default
title: Introduction
nav_order: 1
description: "Utah Language - Write shell scripts in TypeScript-like syntax"
permalink: /
slug: /
---

**Utah** is a tool that allows you to write shell scripts in a strongly typed, TypeScript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 **File Extension:** `.shx`

## Quick Start

Get started with Utah in seconds:

- **Install Utah CLI:**

  ```bash
  curl -sL https://utahshx.com/install.sh | sudo bash
  ```

- **Write your first `.shx` script:**

  ```typescript
  const message: string = "Hello, Utah!";
  console.log(message);
  ```

- **Compile and run:**

  ```bash
  utah compile hello.shx
  ./hello.sh
  ```

  > ... or **run directly:**
  >
  > ```bash
  > utah hello.shx
  > ```

## Key Features

### 🎯 **Type Safety**

Write shell scripts with TypeScript-like syntax including type annotations, variables, and function declarations.

### 🔄 **Modern Language Features**

- Import system for modular code organization
- Error handling with try/catch blocks
- Control flow (for/while loops, switch statements)
- Array manipulation and string functions

### 🧩 **Rich Built-in Functions**

- File system operations
- JSON/YAML processing with jq/yq integration
- Web requests and API interactions
- Process management and system information
- Git utilities and DevOps automation

### ⚡ **Parallel Execution**

Execute functions in parallel for improved performance with the `parallel` keyword.

### 📦 **Dependency Management**

Automatic installation and verification of required tools like `jq`, `yq`, and other utilities.

### 📋 **Command-Line Arguments**

Built-in argument parsing with type safety:

- Define arguments with `args.define()` including types, defaults, and descriptions
- Automatic help generation with `args.showHelp()`
- Type-safe argument access with `args.get()` and `args.has()`
- Required and optional argument support

### 🛠️ **Developer Tools**

- **VS Code Extension**: Full IDE integration with syntax highlighting and IntelliSense
- **Language Server**: Real-time error checking and code completion
- **Code Formatting**: Automatic formatting with EditorConfig support

### 📝 **Script Metadata**

- Script descriptions with `script.description()`
- Error handling modes with `script.exitOnError()`
- Debug output control with `script.enableDebug()`
- Fault tolerance options

## Example

**Input** (`example.shx`):

```typescript
const appName: string = "MyApp";
let users: string[] = ["Alice", "Bob", "Charlie"];

function greet(name: string): void {
  console.log("Hello, ${name}! Welcome to ${appName}.");
}

for (let user: string in users) {
  greet(user);
}

// Check if git is available
if (os.isInstalled("git")) {
  console.log("Git is available for version control");
} else {
  console.log("Please install git");
}
```

**Output** (`example.sh`):

```bash
#!/bin/bash
readonly appName="MyApp"
users=("Alice" "Bob" "Charlie")

greet() {
  local name="$1"
  echo "Hello, ${name}! Welcome to ${appName}."
}

for user in "${users[@]}"; do
  greet "${user}"
done

gitInstalled=$(command -v "git" &> /dev/null && echo "true" || echo "false")
if [ "${gitInstalled}" = "true" ]; then
  echo "Git is available for version control"
else
  echo "Please install git"
fi
```

## How It Works

1. **Parse**: Utah parses your `.shx` file using a custom parser into an Abstract Syntax Tree (AST)
2. **Analyze**: The AST is analyzed for type safety and language features
3. **Transpile**: The AST is transpiled into clean, readable bash code
4. **Execute**: The generated `.sh` file can be executed on any POSIX-compliant system

## Documentation Sections

### Language Reference

- [Arrays](./20-language-features/arrays.md)
- [Console Functions](./20-language-features/console.md)
- [Control Flow](./20-language-features/control-flow.md)
- [Defer Statements](./20-language-features/defer.md)
- [Error Handling](./20-language-features/error-handling.md)
- [File System Functions](./20-language-features/filesystem.md)
- [Functions](./20-language-features/functions.md)
- [Git Functions](./20-language-features/git.md)
- [Import System](./20-language-features/imports.md)
- [JSON and YAML Processing](./20-language-features/json-yaml.md)
- [Operating System Functions](./20-language-features/operating-system.md)
- [Strings](./20-language-features/strings.md)
- [System Functions](./20-language-features/system.md)
- [Template Functions](./20-language-features/template.md)
- [Utility Functions](./20-language-features/utilities.md)
- [Variables and Types](./20-language-features/variables.md)
- [Web Functions](./20-language-features/web.md)

### CLI Reference

- [Overview](./30-cli/index.md)
- [Compile Command](./30-cli/compile.md)
- [Run Command](./30-cli/run.md)
- [Format Command](./30-cli/format.md)
- [VS Code Extension](./30-cli/vscode-extension.md)

### Guides

- [Development Guides](./40-guides/index.md)
- [Parallel Execution](./40-guides/parallel.md)
- [Testing Scripts](./40-guides/testing.md)
- [CI/CD Integration](./40-guides/cicd.md)
- [DevOps Automation](./40-guides/devops.md)
- [Best Practices](./40-guides/best-practices.md)

### Getting Started

- [Installation Guide](./10-getting-started/installation.md)
- [Your First Script](./10-getting-started/first-script.md)
- [Basic Syntax](./10-getting-started/syntax.md)

## Community and Support

- **GitHub**: [polatengin/utah](https://github.com/polatengin/utah)
- **Issues**: [Report bugs or request features](https://github.com/polatengin/utah/issues)
- **Discussions**: [Community discussions](https://github.com/polatengin/utah/discussions)

## License

Utah is open source software licensed under the MIT License.
