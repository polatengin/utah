---
layout: default
title: Home
nav_order: 1
description: "Utah Language - Write shell scripts in TypeScript-like syntax"
permalink: /
---

**Utah** is a tool that allows you to write shell scripts in a strongly typed, TypeScript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 **File Extension:** `.shx`

## Quick Start

Get started with Utah in seconds:

- **Install Utah CLI:**

  ```bash
  curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
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
  > utah run hello.shx
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
- Type-safe argument access with `args.get()`, `args.getNumber()`, `args.getString()`
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
  console.log(`Hello, ${name}! Welcome to ${appName}.`);
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

### Getting Started

- [Installation Guide](getting-started/installation.md)
- [Your First Script](getting-started/first-script.md)
- [Basic Syntax](getting-started/syntax.md)

### Language Features

- [Variables and Types](language-features/variables.md)
- [Control Flow](language-features/control-flow.md)
- [Functions](language-features/functions.md)
- [Arrays and Strings](language-features/arrays-strings.md)
- [Import System](language-features/imports.md)
- [Error Handling](language-features/error-handling.md)

### Built-in Functions

- [File System](functions/filesystem.md)
- [JSON/YAML Processing](functions/json-yaml.md)
- [Console and User Interaction](functions/console.md)
- [Operating System](functions/operating-system.md)
- [Web and API](functions/web.md)
- [System and Process](functions/system.md)
- [Utilities](functions/utilities.md)

### CLI Reference

- [Overview](cli/index.md)
- [Compile Command](cli/compile.md)
- [Run Command](cli/run.md)
- [Format Command](cli/format.md)
- [VS Code Extension](cli/vscode-extension.md)

### Advanced Guides

- [Development Guides](guides/index.md)
- [Parallel Execution](guides/parallel.md)
- [Testing Scripts](guides/testing.md)
- [CI/CD Integration](guides/cicd.md)
- [DevOps Automation](guides/devops.md)
- [Testing Scripts](guides/testing.md)
- [Best Practices](guides/best-practices.md)

## Community and Support

- **GitHub**: [polatengin/utah](https://github.com/polatengin/utah)
- **Issues**: [Report bugs or request features](https://github.com/polatengin/utah/issues)
- **Discussions**: [Community discussions](https://github.com/polatengin/utah/discussions)

## License

Utah is open source software licensed under the MIT License.
