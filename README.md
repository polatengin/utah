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
const appName: string = "MyApp";
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
readonly appName="MyApp"
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

## 🔧 String Manipulation Functions

Utah supports a comprehensive set of string manipulation functions that are familiar to JavaScript/TypeScript developers but transpile to efficient bash code.

### Available String Functions

#### String Properties

- `string.length()` - Get the length of a string

#### String Transformation

- `string.slice(start, end?)` - Extract a substring
- `string.toUpperCase()` - Convert to uppercase
- `string.toLowerCase()` - Convert to lowercase
- `string.trim()` - Remove leading and trailing whitespace
- `string.replace(search, replacement)` - Replace first occurrence
- `string.replaceAll(search, replacement)` - Replace all occurrences

#### String Testing

- `string.startsWith(prefix)` - Check if string starts with prefix
- `string.endsWith(suffix)` - Check if string ends with suffix
- `string.includes(substring)` - Check if string contains substring

#### String Splitting

- `string.split(delimiter)` - Split string into array

### Example Usage

```shx
const message: string = "Hello, World!";
const email: string = "  user@example.com  ";

// Get string length
let length: number = message.length();

// Extract substring
let greeting: string = message.slice(0, 5); // "Hello"

// Case conversion
let upper: string = message.toUpperCase(); // "HELLO, WORLD!"
let lower: string = message.toLowerCase(); // "hello, world!"

// Trim whitespace
let cleanEmail: string = email.trim(); // "user@example.com"

// Replace text
let newMsg: string = message.replace("World", "Universe"); // "Hello, Universe!"

// Boolean checks
let startsWithHello: boolean = message.startsWith("Hello"); // true
let endsWithExclamation: boolean = message.endsWith("!"); // true
let containsWorld: boolean = message.includes("World"); // true
```

### Generated Bash Code

The string functions transpile to efficient bash parameter expansion and built-in commands:

```bash
readonly message="Hello, World!"
readonly email="  user@example.com  "

length="${#message}"
greeting="${message:0:5}"
upper="${message^^}"
lower="${message,,}"
cleanEmail=$(echo "${email}" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
newMsg="${message/World/Universe}"

if [[ "${message}" == "Hello"* ]]; then
  startsWithHello="true"
else
  startsWithHello="false"
fi
```

## 🚧 Roadmap

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

- [x] Support for `const` declarations

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

- [x] Enhanced string manipulation functions

- [ ] File I/O operations (read/write files)

- [ ] Support for command substitution

- [ ] Improved error messages and debugging support

- [ ] Support for async/await patterns

- [ ] Job queueing and parallel execution

- [ ] Time and Resource monitoring

- [ ] More built-in functions and utilities

- [ ] VS Code extension for syntax highlighting
