# Project Utah

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 File Extension: `.shx`

## 🚀 How It Works

Write code in `.shx` using modern, friendly typescript-like syntax.

The tool parses `.shx` file using a custom parser into an `Abstract Syntax Tree` (AST).

It then transpiles the AST into valid bash code.

The generated `.sh` file is saved alongside the original.

## 🧪 Example

Input (`examples/input.shx`):

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

Output (`examples/input.sh`):

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

## 🔄 For Loops

Utah supports both traditional C-style for loops and for-in loops for iterating over space-separated values.

### Traditional For Loops

```shx
// Basic increment
for (let i: number = 0; i < 5; i++) {
  console.log(`Count: ${i}`);
}

// Decrement
for (let j: number = 10; j > 0; j--) {
  console.log(`Countdown: ${j}`);
}

// Custom increment/decrement
for (let k: number = 0; k < 20; k += 3) {
  console.log(`Step by 3: ${k}`);
}

for (let m: number = 15; m >= 0; m -= 5) {
  console.log(`Step down by 5: ${m}`);
}
```

### For-In Loops

For-in loops iterate over arrays created using the `split()` function:

```shx
// Split a comma-separated string
let fruitString: string = "apple,banana,cherry";
let fruits: string = fruitString.split(",");

for (let fruit: string in fruits) {
  console.log(`Fruit: ${fruit}`);
}

// Split by spaces
let words: string = "hello world utah";
let wordArray: string = words.split(" ");

for (let word: string in wordArray) {
  console.log(`Word: ${word}`);
}

// Split by custom delimiter
let data: string = "one|two|three|four";
let items: string = data.split("|");

for (let item: string in items) {
  console.log(`Item: ${item}`);
}
```

### Generated Bash Code

For loops are transpiled to efficient bash while loops and for loops:

```bash
# Traditional for loop becomes:
i=0
while [ $i -lt 5 ]; do
  echo "Count: ${i}"
  i=$((i + 1))
done

# For-in loop with split() becomes:
fruitString="apple,banana,cherry"
IFS=',' read -ra fruits <<< "${fruitString}"
for fruit in "${fruits[@]}"; do
  echo "Fruit: ${fruit}"
done
```

## 📋 Arrays

Utah supports arrays through the `split()` function, which creates bash arrays that can be used with for-in loops:

```shx
// Create an array by splitting a string
let csvData: string = "apple,banana,cherry";
let fruits: string = csvData.split(",");

// Iterate over the array
for (let fruit: string in fruits) {
  console.log(`Processing: ${fruit}`);
}
```

The `split()` function works with any delimiter:

```shx
// Split by spaces
let sentence: string = "hello world utah";
let words: string = sentence.split(" ");

// Split by custom delimiter
let path: string = "/usr/local/bin";
let pathParts: string = path.split("/");

// Split by dots for file extensions
let filename: string = "document.backup.txt";
let parts: string = filename.split(".");
```

### Generated Array Code

Arrays created with `split()` are implemented using bash's `read -ra` command:

```bash
# Utah code:
# let data: string = "a,b,c";
# let items: string = data.split(",");

# Generated bash:
data="a,b,c"
IFS=',' read -ra items <<< "${data}"

# Can then be used in for loops:
for item in "${items[@]}"; do
  echo "Item: ${item}"
done
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

- [x] Support for `for` and `for in` loops

- [ ] Support for `while` and `break`

- [ ] Arrays and basic data structures

- [x] Comment support (`//`)

- [ ] import syntax for splitting .shx files

- [ ] Typed return values and richer type checking

- [ ] Error handling: try/catch, trap

- [x] More complex expressions and operators

- [ ] Process Substitution

- [x] Support for managing environment variables

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

- [x] VS Code extension for syntax highlighting

- [ ] VS Code extension for intellisense and autocompletion

## Development

### Building

To build the Utah CLI:

```bash
make build
```

Or using dotnet directly:

```bash
cd src/cli
dotnet build
```

### Testing

Utah includes a comprehensive regression test suite to ensure that changes don't break existing functionality.

#### Running Tests

Run all tests with detailed output:
```bash
make test
```

#### Test Structure

Tests are organized in the `tests/` directory:

```
tests/
├── fixtures/          # Test input files (.shx)
├── expected/          # Expected compilation outputs (.sh)
└── run_tests.sh       # Main test runner
```

The test framework automatically:
- Builds the CLI
- Compiles all test fixtures
- Compares outputs with expected results
- Reports any differences

This ensures that new features don't break existing functionality and that the compiler produces consistent output.

### Development Helper

The Makefile provides convenient commands for development:

```bash
make help           # Show available commands
make build          # Build the CLI
make test           # Run all tests
make compile FILE=<file>  # Compile a specific file
make clean          # Clean build artifacts
make dev            # Full development cycle (build + test)
make info           # Show project information
```

Use `make help` to see all available targets with descriptions.

### Running Tests

```bash
make test        # Full test output
```

### Test Coverage

Current tests cover:

- **simple_console.shx** - Basic console.log functionality
- **variable_declaration.shx** - Variable declarations and usage
- **simple_for_loop.shx** - Traditional for loops
- **for_in_loop.shx** - For-in loops with arrays
- **nested_for_loop.shx** - Nested for loops
- **string_functions.shx** - String manipulation functions
- **environment_variables.shx** - Environment variable operations
- **ternary_operators.shx** - Ternary conditional operators
- **mixed_loops.shx** - Mixed loop types in one file

### How Tests Work

1. **Build** - The test runner builds the Utah CLI using `dotnet build`
2. **Compile** - Each `.shx` file in `fixtures/` is compiled to `.sh`
3. **Compare** - The generated output is compared with the corresponding file in `expected/`
4. **Report** - Any differences are reported with diff output

### Test Philosophy

- **Regression Testing** - Ensure existing functionality never breaks
- **Deterministic** - Same input should always produce same output
- **Fast** - Tests should run quickly for rapid development feedback
- **Comprehensive** - Cover all major language features
- **Maintainable** - Easy to add new tests and update expectations
