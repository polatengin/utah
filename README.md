# Project Utah

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 File Extension: `.shx`

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

## 🛣 Roadmap

- [x] `string` data type

- [x] `number` data type

- [x] `boolean` data type

- [x] `let` declarations with type annotations

- [x] `function` definitions with typed parameters

- [x] `return` statements for simple values

- [x] `if` statements with boolean logic

- [x] `else` statements

- [x] `console.log` for output

- [x] String interpolation with backticks (`Hello, ${name}`)

- [x] Function calls with positional arguments

- [x] Exit script with `exit(0)` or `exit(1)`

- [ ] Support for `var` and `const` declarations

- [ ] Support for for, while, and break

- [ ] Arrays and basic data structures

- [ ] Comment support (`//` and `/* */`)

- [ ] import syntax for splitting .shx files

- [ ] Typed return values and richer type checking

- [ ] Error handling: try/catch, trap

- [ ] More complex expressions and operators

- [ ] Process Substitution and environment variables

- [ ] Lastpipe operator

- [ ] Parameter expansion and manipulation

- [ ] Support for functions with multiple return values

- [ ] Enhanced string manipulation functions

- [ ] File I/O operations (read/write files)

- [ ] Support for command substitution

- [ ] Improved error messages and debugging support

- [ ] Support for async/await patterns

- [ ] Job queueing and parallel execution

- [ ] Time and Resource monitoring

- [ ] More built-in functions and utilities

- [ ] VS Code extension for syntax highlighting
