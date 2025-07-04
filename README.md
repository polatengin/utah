# Project Utah

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 File Extension: `.shx`

## 🚀 Installation

You can install the latest version of the `utah` CLI with this one-liner:

```bash
cURL -sL https://github.com/polatengin/utah/releases/latest/download/utah -o utah && chmod +x utah && sudo mv utah /usr/local/bin/utah
```

## 🚀 How It Works

Write code in `.shx` using modern, friendly typescript-like syntax.

The tool parses `.shx` file using a custom parser into an `Abstract Syntax Tree` (AST).

It then transpiles the AST into valid bash code.

The generated `.sh` file is saved alongside the original.

## 🧪 Example

Input (`examples/input.shx`):

```typescript
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

```typescript
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

```typescript
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

## 🔀 Switch Statements

Utah supports switch/case/default statements for conditional branching with multiple cases, providing a cleaner alternative to long if-else chains.

### Basic Switch Statement

```typescript
let grade: string = "B";
let message: string = "";

switch (grade) {
  case "A":
    message = "Excellent!";
    break;
  case "B":
    message = "Good job!";
    break;
  case "C":
    message = "Not bad";
    break;
  default:
    message = "Try harder";
    break;
}

console.log(message);
```

### Switch with Numbers

```typescript
let score: number = 85;
let category: string = "";

switch (score) {
  case 100:
    category = "Perfect";
    break;
  case 90:
    category = "Excellent";
    break;
  case 80:
    category = "Good";
    break;
  default:
    category = "Needs improvement";
    break;
}
```

### Fall-Through Cases

You can have multiple case labels that execute the same code by omitting the `break` statement:

```typescript
let day: string = "Monday";
let type: string = "";

switch (day) {
  case "Monday":
  case "Tuesday":
  case "Wednesday":
  case "Thursday":
  case "Friday":
    type = "Weekday";
    break;
  case "Saturday":
  case "Sunday":
    type = "Weekend";
    break;
  default:
    type = "Unknown";
    break;
}
```

### Generated Bash Code for Switch Statements

```bash
# Utah switch statement becomes:
case $grade in
  A)
    message="Excellent!"
    ;;
  B)
    message="Good job!"
    ;;
  C)
    message="Not bad"
    ;;
  *)
    message="Try harder"
    ;;
esac

# Fall-through cases become:
case $day in
  Monday|Tuesday|Wednesday|Thursday|Friday)
    type="Weekday"
    ;;
  Saturday|Sunday)
    type="Weekend"
    ;;
  *)
    type="Unknown"
    ;;
esac
```

## 📋 Arrays

Utah provides comprehensive support for typed arrays including `string[]`, `number[]`, and `boolean[]`. Arrays can be created using array literals or by splitting strings.

### Array Literals

You can create arrays using familiar TypeScript-like syntax:

```typescript
// Create arrays with literals
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Alice", "Bob", "Charlie"];
let flags: boolean[] = [true, false, true];
```

### Array Access

Access array elements using bracket notation:

```typescript
let numbers: number[] = [10, 20, 30];
let first: number = numbers[0];    // Gets 10
let second: number = numbers[1];   // Gets 20
```

### Array Properties

Get the length of an array using the `.length` property:

```typescript
let items: string[] = ["apple", "banana", "cherry"];
let count: number = items.length;  // Gets 3
```

### Array Iteration

Use for-in loops to iterate over arrays:

```typescript
let colors: string[] = ["red", "green", "blue"];

for (let color: string in colors) {
  console.log(`Color: ${color}`);
}
```

### Arrays from String Split

Create arrays by splitting strings - this integrates with the existing `split()` function:

```typescript
// Create an array by splitting a string
let csvData: string = "apple,banana,cherry";
let fruits: string[] = csvData.split(",");

// Iterate over the array
for (let fruit: string in fruits) {
  console.log(`Processing: ${fruit}`);
}
```

The `split()` function works with any delimiter:

```typescript
// Split by spaces
let sentence: string = "hello world utah";
let words: string[] = sentence.split(" ");

// Split by custom delimiter
let path: string = "/usr/local/bin";
let pathParts: string[] = path.split("/");

// Split by dots for file extensions
let filename: string = "document.backup.txt";
let parts: string[] = filename.split(".");
```

### Generated Array Code

Arrays are implemented using bash arrays:

```bash
# Array literals become bash arrays:
numbers=(1 2 3 4 5)
names=("Alice" "Bob" "Charlie")

# Array access uses bash array syntax:
first="${numbers[0]}"
second="${names[1]}"

# Array length uses bash array length syntax:
count="${#numbers[@]}"

# Array iteration uses bash array expansion:
for color in "${colors[@]}"; do
  echo "Color: ${color}"
done

# Arrays from split() use bash read command:
IFS=',' read -ra fruits <<< "${csvData}"
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

```typescript
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

### Generated Bash Code for String Functions

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

## 📂 File System Functions

Utah provides a comprehensive set of file system functions for reading, writing, and manipulating file paths. These functions are designed to be familiar to Node.js developers while transpiling to efficient bash commands.

### Available File System Functions

#### File I/O Operations

- `fs.readFile(filepath)` - Read the contents of a file into a string
- `fs.writeFile(filepath, content)` - Write content to a file (overwrites existing content)

#### Path Manipulation Functions

- `fs.dirname(filepath)` - Get the directory path of a file
- `fs.parentDirName(filepath)` - Get the name of the parent directory
- `fs.extension(filepath)` - Get the file extension (without the dot)
- `fs.fileName(filepath)` - Get the filename with extension

### File I/O Usage

```typescript
// Write content to a file
fs.writeFile("config.txt", "debug=true");

// Read content from a file
let config: string = fs.readFile("config.txt");
console.log("Config:", config);

// Write variable content to file
let logMessage: string = "Application started";
fs.writeFile("app.log", logMessage);

// Read and process file content
let logContent: string = fs.readFile("app.log");
console.log("Log:", logContent);
```

### Path Manipulation Usage

```typescript
let filePath: string = "/home/user/documents/project/readme.txt";

// Get directory path
let directory: string = fs.dirname(filePath);
console.log("Directory:", directory); // "/home/user/documents/project"

// Get parent directory name
let parentName: string = fs.parentDirName(filePath);
console.log("Parent dir:", parentName); // "project"

// Get file extension
let extension: string = fs.extension(filePath);
console.log("Extension:", extension); // "txt"

// Get filename
let filename: string = fs.fileName(filePath);
console.log("Filename:", filename); // "readme.txt"
```

### Generated Bash Code for File System Functions

The file system functions transpile to efficient bash commands:

```bash
# File I/O operations:
echo "debug=true" > "config.txt"
config=$(cat "config.txt")
echo "Config: $config"

logMessage="Application started"
echo "$logMessage" > "app.log"
logContent=$(cat "app.log")
echo "Log: $logContent"

# Path manipulation:
directory=$(dirname "/home/user/documents/project/readme.txt")
echo "Directory: $directory"

parentName=$(basename $(dirname "/home/user/documents/project/readme.txt"))
echo "Parent dir: $parentName"

_temp_path="/home/user/documents/project/readme.txt"
extension="${_temp_path##*.}"
echo "Extension: $extension"

filename=$(basename "/home/user/documents/project/readme.txt")
echo "Filename: $filename"
```

## ⏱️ Timer Functions

Utah provides timer functions for measuring execution time in your scripts. These functions are useful for performance testing, benchmarking, and monitoring script execution duration.

### Available Timer Functions

#### Time Measurement

- `timer.start()` - Start a timer (resets any previous timer)
- `timer.stop()` - Stop the timer and return elapsed time in milliseconds

### Timer Usage

```typescript
// Basic timer usage
timer.start();
console.log("Starting some work...");

// Simulate some work
for (let i: number = 0; i < 1000000; i++) {
  // Some computation
}

const elapsed: number = timer.stop();
console.log(`Work completed in ${elapsed} ms`);

// Timer can be restarted
timer.start();
console.log("Starting more work...");

// More work
for (let j: number = 0; j < 500000; j++) {
  // More computation
}

const elapsed2: number = timer.stop();
console.log(`Second task took ${elapsed2} ms`);
```

### Timer with Conditionals

```typescript
timer.start();

let processCount: number = 1000;
if (processCount > 500) {
  console.log("Processing large dataset...");
  for (let i: number = 0; i < processCount; i++) {
    // Process data
  }
} else {
  console.log("Processing small dataset...");
  for (let i: number = 0; i < processCount; i++) {
    // Process data
  }
}

const duration: number = timer.stop();
console.log(`Processing ${processCount} items took ${duration} ms`);
```

### Generated Bash Code for Timer Functions

The timer functions transpile to efficient bash commands using date:

```bash
# timer.start() becomes:
_utah_timer_start=$(date +%s%3N)

# timer.stop() becomes:
_utah_timer_end=$(date +%s%3N)
elapsed=$((_utah_timer_end - _utah_timer_start))

# Complete example:
_utah_timer_start=$(date +%s%3N)
echo "Starting some work..."

i=0
while [ $i -lt 1000000 ]; do
  i=$((i + 1))
done

_utah_timer_end=$(date +%s%3N)
elapsed=$((_utah_timer_end - _utah_timer_start))
echo "Work completed in ${elapsed} ms"
```

### Timer Best Practices

- **Use for performance testing**: Measure how long different parts of your script take
- **Benchmark optimizations**: Compare execution times before and after code changes
- **Monitor long-running tasks**: Track progress of time-intensive operations
- **Restart timer as needed**: Call `timer.start()` multiple times to measure different sections
- **Millisecond precision**: Timer provides millisecond accuracy for fine-grained measurements

## 📊 Process Information Functions

Utah provides process information functions for monitoring and inspecting the current script's process. These functions are useful for debugging, monitoring, and system administration tasks.

### Available Process Functions

#### Process Information

- `process.id()` - Get the current process ID (PID)
- `process.cpu()` - Get the current CPU usage percentage of the process
- `process.memory()` - Get the current memory usage percentage of the process
- `process.elapsedTime()` - Get the elapsed time since the process started
- `process.command()` - Get the command line that started the process
- `process.status()` - Get the current process status/state

### Process Functions Usage

```typescript
// Get basic process information
let pid: number = process.id();
let cpuUsage: number = process.cpu();
let memoryUsage: number = process.memory();

// Get process timing and command info
let elapsed: string = process.elapsedTime();
let command: string = process.command();
let status: string = process.status();

// Use in conditionals for monitoring
if (cpuUsage > 80.0) {
  console.log("High CPU usage detected");
}

// Log process information
console.log(`Process ${pid} using ${memoryUsage}% memory`);
```

### Process Monitoring Example

```typescript
// Process monitoring script
let processId: number = process.id();
let startCommand: string = process.command();

console.log(`Monitoring process ${processId}`);
console.log(`Started with: ${startCommand}`);

// Check resource usage
let cpu: number = process.cpu();
let memory: number = process.memory();
let runtime: string = process.elapsedTime();

console.log(`Runtime: ${runtime}`);
console.log(`CPU: ${cpu}%, Memory: ${memory}%`);

// Check if process is running efficiently
if (cpu > 90.0) {
  console.log("Warning: High CPU usage!");
}

if (memory > 75.0) {
  console.log("Warning: High memory usage!");
}
```

### Generated Bash Code for Process Functions

The process functions transpile to efficient `ps` commands:

```bash
# process.id() becomes:
pid=$(ps -o pid -p $$ --no-headers | tr -d ' ')

# process.cpu() becomes:
cpuUsage=$(ps -o pcpu -p $$ --no-headers | tr -d ' ')

# process.memory() becomes:
memoryUsage=$(ps -o pmem -p $$ --no-headers | tr -d ' ')

# process.elapsedTime() becomes:
elapsed=$(ps -o etime -p $$ --no-headers | tr -d ' ')

# process.command() becomes:
command=$(ps -o cmd= -p $$)

# process.status() becomes:
status=$(ps -o stat= -p $$)

# Complete example:
processId=$(ps -o pid -p $$ --no-headers | tr -d ' ')
startCommand=$(ps -o cmd= -p $$)

echo "Monitoring process ${processId}"
echo "Started with: ${startCommand}"

cpu=$(ps -o pcpu -p $$ --no-headers | tr -d ' ')
memory=$(ps -o pmem -p $$ --no-headers | tr -d ' ')
runtime=$(ps -o etime -p $$ --no-headers | tr -d ' ')

echo "Runtime: ${runtime}"
echo "CPU: ${cpu}%, Memory: ${memory}%"
```

### Process Functions Use Cases

- **System Monitoring**: Track resource usage of your scripts
- **Performance Analysis**: Monitor CPU and memory consumption over time
- **Debugging**: Identify process information for troubleshooting
- **Logging**: Include process details in log files
- **Alerting**: Set up thresholds for resource usage warnings
- **Process Management**: Gather information for process control decisions

### Process Status Values

The `process.status()` function returns standard Unix process state codes:

- **R** - Running or runnable (on run queue)
- **S** - Interruptible sleep (waiting for an event to complete)
- **D** - Uninterruptible sleep (usually I/O)
- **Z** - Zombie (terminated but not reaped by parent)
- **T** - Stopped (on a signal or by job control)
- **W** - Paging (not valid since Linux 2.6.xx)

### Performance Notes

- Process functions use standard `ps` commands available on all Unix-like systems
- Commands are optimized with `--no-headers` and `tr -d ' '` for clean output
- CPU and memory percentages are relative to system totals
- Elapsed time format is `[DD-]HH:MM:SS` or `MM:SS` for shorter durations

## 💻 Operating System Utilities

Utah provides utilities for interacting with the operating system and checking system capabilities.

### Available OS Functions

#### System Checks

- `os.isInstalled(appName)` - Check if a command-line application is installed
- `os.getOS()` - Get the current operating system (linux, mac, windows, or unknown)
- `os.getLinuxVersion()` - Get the Linux distribution version

### OS Utilities Usage

```typescript
// Check if git is installed
let gitInstalled: boolean = os.isInstalled("git");

if (gitInstalled) {
  console.log("Git is available");
} else {
  console.log("Git is not installed");
}

// Check multiple tools
let nodeInstalled: boolean = os.isInstalled("node");
let dockerInstalled: boolean = os.isInstalled("docker");
let curlInstalled: boolean = os.isInstalled("curl");

// Get the current OS
let currentOS: string = os.getOS();
console.log(`Current OS: ${currentOS}`);

if (currentOS == "linux") {
  console.log("Running on Linux");
  let linuxVersion: string = os.getLinuxVersion();
  console.log(`Linux version: ${linuxVersion}`);
} else if (currentOS == "mac") {
  console.log("Running on macOS");
} else {
  console.log("Running on another OS");
}
```

### Generated Bash Code for OS Utilities

The OS utilities transpile to efficient bash commands:

```bash
# os.isInstalled("git") becomes:
if command -v git &> /dev/null; then
  gitInstalled="true"
else
  gitInstalled="false"
fi

# os.getOS() becomes:
case "$(uname -s)" in
  Linux*)
    currentOS="linux"
    ;;
  Darwin*)
    currentOS="mac"
    ;;
  CYGWIN*|MINGW*|MSYS*)
    currentOS="windows"
    ;;
  *)
    currentOS="unknown"
    ;;
esac
echo "Current OS: ${currentOS}"

if [ "${currentOS}" == "linux" ]; then
  echo "Running on Linux"
elif [ "${currentOS}" == "mac" ]; then
  echo "Running on macOS"
else
  echo "Running on another OS"
fi

# os.getLinuxVersion() becomes:
if [[ -f /etc/os-release ]]; then
  source /etc/os-release
  version="${VERSION_ID}"
elif type lsb_release >/dev/null 2>&1; then
  version=$(lsb_release -sr)
elif [[ -f /etc/lsb-release ]]; then
  source /etc/lsb-release
  version="${DISTRIB_RELEASE}"
else
  version="unknown"
fi
console.log("Linux version: ${version}")
```

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

#### Test Structure

Tests are organized in the `tests/` directory:

```text
tests/
├── positive_fixtures/ # Test input files (.shx)
├── negative_fixtures/ # Test input files (.shx)
└── expected/          # Expected compilation outputs (.sh)
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
make help              # Show available commands
make build             # Build the CLI
make test              # Run all tests
make compile FILE=<file>  # Compile a specific file
make clean             # Clean build artifacts
make dev               # Full development cycle (build + test)
make info              # Show project information
```

Use `make help` to see all available targets with descriptions.

### Running Tests

Run all tests with detailed output:

```bash
make test              # Full test output
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
- **simple_switch.shx** - Basic switch/case/default statements
- **switch_case.shx** - Complex switch statements with fall-through cases
- **array_support.shx** - Array literals, access, length, and iteration

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

- [x] Support for `switch/case/default` statements

- [ ] Support for `while` and `break`

- [x] Arrays and basic data structures (`string[]`, `number[]`, `boolean[]`)

- [x] Comment support (`//`)

- [ ] import syntax for splitting .shx files

- [ ] `array.*` functions (`length()`, `contains()`, `isEmpty()`, `join()`, `reverse()`, `sort()`, `merge()`)

- [x] Add shebang (`#!/bin/sh`) to generated scripts

- [ ] Typed return values and richer type checking

- [ ] Error handling: try/catch, trap

- [ ] Typecasting and type conversion

- [ ] Check for 100% POSIX compliance

- [x] More complex expressions and operators

- [ ] Process Substitution

- [x] Support for managing environment variables

- [ ] Lastpipe operator

- [ ] Parameter expansion and manipulation

- [ ] Support for functions with multiple return values

- [x] Enhanced string manipulation functions

- [x] File I/O operations (read/write files)

- [x] File path manipulation functions (dirname, fileName, extension, parentDirName)

- [ ] File I/O operations (exists, delete, copy, move, rename, permissions, etc.)

- [ ] Support for command substitution

- [ ] Improved error messages and debugging support

- [ ] Support for async/await patterns

- [ ] Argument parsing and validation

- [ ] Job queueing and parallel execution

- [ ] Time and Resource monitoring

- [x] VS Code extension for syntax highlighting

- [ ] VS Code extension for intellisense and autocompletion
