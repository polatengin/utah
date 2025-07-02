# Project Utah

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

## 🚀 How It Works

Write code in `.shx` using modern, friendly typescript-like syntax.

The tool parses `.shx` file using a custom parser into an `Abstract Syntax Tree` (AST).

It then transpiles the AST into valid bash code.

The generated `.sh` file is saved alongside the original.

## 🧪 Example

Input (`examples/input_0.shx`):

```shx
let name: string = "Alice";
let greetMsg: string = `Hello, ${name}`;

function greet(name: string): void {
  console.log(`Hi, ${name}!`);
}

if (name == "Alice") {
  greet(name);
} else {
  console.log("Unknown user");
}
```

Output (input_0.sh):

```bash
name="Alice"
greetMsg="Hello, ${name}"

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

## ✅ Features & Language Support

📦 File Extension: `.shx`

## 🧠 Supported Language Constructs

| Feature | Supported | Notes
|---------|-----------|-----------------------
| let declarations | ✅ | With types: string, number, boolean
| Function definitions | ✅ | With typed params and optional return
| Return statements | ✅ | return value; for simple values
| Boolean logic | ✅ | if, else, ==, !=, true, false
| Console logging | ✅ | console.log("...") and template strings
| String interpolation | ✅ | Use backticks: `Hello, ${name}`
| Function calls | ✅ | Arguments passed positionally
| Exit script | ✅ | exit(1); or exit(0);
| Comments | 🚧 | Not supported yet
| Loops (for, while) | 🚧 | Planned

## 🧩 Supported Types

| Type | Bash Mapping
|------|--------------
| string | Quoted Bash string
| number | Quoted number string
| boolean | true / false (as strings)

## 🛣 Roadmap

[] Support for for, while, and break

[] Arrays and basic data structures

[] Comment support (`//` and `/* */`)

[] import syntax for splitting .shx files

[] Typed return values and richer type checking

[] Error handling: try/catch, trap

[] More complex expressions and operators

[] Process Substitution and environment variables

[] Lastpipe operator

[] Parameter expansion and manipulation

[] Support for functions with multiple return values

[] Enhanced string manipulation functions

[] File I/O operations (read/write files)

[] Support for command substitution

[] Improved error messages and debugging support

[] Support for async/await patterns

[] Job queueing and parallel execution

[] Time and Resource monitoring

[] More built-in functions and utilities

[] VS Code extension for syntax highlighting
