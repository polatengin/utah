# Project Utah

[![Release Utah CLI](https://github.com/polatengin/utah/actions/workflows/release.yml/badge.svg)](https://github.com/polatengin/utah/actions/workflows/release.yml) [![Deploy to GitHub Pages](https://github.com/polatengin/utah/actions/workflows/deploy-docs.yml/badge.svg)](https://github.com/polatengin/utah/actions/workflows/deploy-docs.yml) [![Latest Release](https://img.shields.io/github/v/tag/polatengin/utah?label=release&sort=semver)](https://github.com/polatengin/utah/releases) [![Number of tests](https://img.shields.io/badge/Number%20of%20tests-108-blue?logo=codeigniter&logoColor=white)](https://github.com/polatengin/utah)

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 File Extension: `.shx`

## 📄 Documentation

For detailed documentation on how to use `utah`, please refer to the [Utah shx documentation](https://utahshx.com).

## 🚀 Installation

You can install the latest version of the `utah` CLI with this one-liner:

```bash
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
```

## 🚀 How It Works

Write code in `.shx` using modern, friendly typescript-like syntax.

The tool parses `.shx` file using a custom parser into an `Abstract Syntax Tree` (AST).

It then transpiles the AST into valid bash code.

The generated `.sh` file is saved alongside the original.

## 🧪 Testing

To run the entire test suite, use the `make test` command. This will build the CLI and run all positive and negative regression tests.

```bash
make test
```

You can also run a specific test file by providing the `FILE` variable:

```bash
make test FILE=a_test_file
```

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

## ⤵️ Import System

Utah supports a module system that allows you to organize your code across multiple `.shx` files using import statements. This enables code reuse, better organization, and modular development.

### Basic Import Syntax

```typescript
import "filename.shx";
```

### Multiple Imports

You can have multiple import statements in a single file:

```typescript
import "utils.shx";
import "math.shx";
import "config.shx";
```

### Import Resolution

- **Relative paths**: Imports are resolved relative to the importing file's directory
- **Absolute paths**: Full file paths are also supported
- **Circular import prevention**: Files are only included once to prevent infinite loops
- **Preprocessing**: Imports are resolved before compilation, merging all content

### Import Example

**utils.shx** (utility functions):

```typescript
function greet(name: string): string {
  return "Hello, " + name + "!";
}

function add(a: number, b: number): number {
  return a + b;
}

let sharedMessage: string = "This comes from utils.shx";
```

**math.shx** (mathematical functions):

```typescript
function multiply(x: number, y: number): number {
  return x * y;
}

function square(n: number): number {
  return multiply(n, n);
}

let mathConstant: number = 42;
```

**main.shx** (main application):

```typescript
import "utils.shx";
import "math.shx";

// Use imported functions and variables
let result: number = add(5, 3);
let squared: number = square(4);

console.log("Addition result: " + result);
console.log("Square of 4: " + squared);
console.log(greet("Utah Language"));
console.log(sharedMessage);
console.log("Math constant: " + mathConstant);
```

### Nested Imports

Files can import other files that themselves have imports:

**advanced_math.shx**:

```typescript
import "math.shx";  // This imports multiply() and square()

function cube(n: number): number {
  return multiply(n, square(n));
}
```

**main.shx**:

```typescript
import "utils.shx";
import "advanced_math.shx";  // This also brings in math.shx

let cubed: number = cube(3);
console.log("Cube of 3: " + cubed);
```

### Generated Bash Code for Imports

The import system works by preprocessing files and merging content before compilation:

**Input files**:

- `utils.shx` with greet() function
- `main.shx` importing utils.shx and calling greet()

**Generated main.sh**:

```bash
#!/bin/bash

# Content from utils.shx is merged first
greet() {
  local name="$1"
  echo "Hello, ${name}!"
}
sharedMessage="This comes from utils.shx"

# Content from main.shx follows
greeting=$(greet "Utah Language")
echo "${greeting}"
echo "${sharedMessage}"
```

### Import Features

- **Order preservation**: Imported content appears before the importing file's content
- **Dependency resolution**: Nested imports are handled automatically
- **Circular import prevention**: Prevents infinite loops with smart duplicate detection
- **Path flexibility**: Supports both relative and absolute import paths
- **Error handling**: Clear error messages for missing import files

### Import Best Practices

1. **Organize by functionality**: Group related functions in separate files
2. **Use descriptive names**: Choose clear, descriptive filenames for imported modules
3. **Avoid deep nesting**: Keep import chains reasonably shallow for maintainability
4. **Document dependencies**: Comment your imports to explain their purpose

## 🔄️ For Loops

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
let fruits: string = string.split(fruitString, ",");

for (let fruit: string in fruits) {
  console.log(`Fruit: ${fruit}`);
}

// Split by spaces
let words: string = "hello world utah";
let wordArray: string = string.split(words, " ");

for (let word: string in wordArray) {
  console.log(`Word: ${word}`);
}

// Split by custom delimiter
let data: string = "one|two|three|four";
let items: string = string.split(data, "|");

for (let item: string in items) {
  console.log(`Item: ${item}`);
}
```

### Example Generated Bash Code

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

## ➿ While Loops and Break Statements

Utah supports while loops for conditional iteration and break statements for early loop termination. These provide flexible control flow for loops that need to continue until a specific condition is met.

### Basic While Loop

```typescript
let i: number = 0;
while (i < 5) {
  console.log(`Count: ${i}`);
  i = i + 1;
}
```

### While Loop with Break Statement

Use `break` to exit a loop early when a certain condition is met:

```typescript
let i: number = 0;
while (i < 10) {
  console.log(`Processing item ${i}`);

  if (i == 5) {
    console.log("Reached target, stopping early");
    break;
  }

  i = i + 1;
}
console.log("Loop completed");
```

### While Loop with Complex Conditions

```typescript
let count: number = 0;
let found: boolean = false;

while (count < 100 && !found) {
  console.log(`Searching... attempt ${count}`);

  // Simulate finding something
  if (count == 42) {
    found = true;
    console.log("Found the answer!");
    break;
  }

  count = count + 1;
}

if (found) {
  console.log(`Success! Found after ${count} attempts`);
} else {
  console.log("Search completed without finding target");
}
```

### Nested While Loops with Break

Break statements only exit the innermost loop:

```typescript
let outer: number = 0;
while (outer < 3) {
  console.log(`Outer loop: ${outer}`);

  let inner: number = 0;
  while (inner < 5) {
    console.log(`  Inner loop: ${inner}`);

    if (inner == 2) {
      console.log("  Breaking inner loop");
      break;  // Only exits the inner while loop
    }

    inner = inner + 1;
  }

  outer = outer + 1;
}
```

### User Input Processing

While loops are perfect for input validation and menu systems:

```typescript
let userChoice: string = "";
let validInput: boolean = false;

while (!validInput) {
  // In a real script, you would get user input here
  // For this example, we'll simulate it
  console.log("Enter your choice (a, b, or c):");

  // Simulate user input
  userChoice = "b";  // This would be actual input in practice

  if (userChoice == "a" || userChoice == "b" || userChoice == "c") {
    validInput = true;
    console.log(`Valid choice: ${userChoice}`);
  } else {
    console.log("Invalid choice, please try again");
  }

  // For this example, break to avoid infinite loop
  break;
}
```

### Generated Bash Code for While Loops

Utah while loops transpile to efficient bash while loops:

```bash
# Basic while loop becomes:
i=0
while [ ${i} -lt 5 ]; do
  echo "Count: ${i}"
  i=$((i + 1))
done

# While loop with break becomes:
i=0
while [ ${i} -lt 10 ]; do
  echo "Processing item ${i}"

  if [ ${i} -eq 5 ]; then
    echo "Reached target, stopping early"
    break
  fi

  i=$((i + 1))
done
echo "Loop completed"

# Complex condition becomes:
count=0
found="false"
while [ ${count} -lt 100 ] && [ "${found}" != "true" ]; do
  echo "Searching... attempt ${count}"

  if [ ${count} -eq 42 ]; then
    found="true"
    echo "Found the answer!"
    break
  fi

  count=$((count + 1))
done
```

### While Loop Best Practices

- **Always modify loop variables**: Ensure the loop condition will eventually become false
- **Use break for early termination**: Exit loops when the desired condition is met
- **Guard against infinite loops**: Include safeguards or maximum iteration counts
- **Combine with conditionals**: Use if statements inside while loops for complex logic
- **Prefer for loops for counting**: Use while loops when the iteration count is unknown

### Common While Loop Patterns

#### Counter with Condition

```typescript
let attempt: number = 0;
let maxAttempts: number = 5;

while (attempt < maxAttempts) {
  console.log(`Attempt ${attempt + 1} of ${maxAttempts}`);

  // Simulate some operation that might succeed
  if (attempt == 3) {
    console.log("Operation successful!");
    break;
  }

  attempt = attempt + 1;
}
```

#### Flag-based Loop

```typescript
let processing: boolean = true;
let itemCount: number = 0;

while (processing) {
  console.log(`Processing item ${itemCount}`);
  itemCount = itemCount + 1;

  // Stop processing after 10 items
  if (itemCount >= 10) {
    processing = false;
  }
}
```

## 🏗️ Defer Statements: Cleanup and Resource Management

Utah supports the `defer` keyword for scheduling cleanup operations that execute automatically when a function exits, regardless of how the function exits (normal return, early return, or error). This is inspired by Go's defer mechanism and provides a reliable way to handle resource cleanup, finalization tasks, and guaranteed execution of critical operations.

### Basic Defer Syntax

```typescript
function processFile(filename: string): void {
  defer console.log("Function cleanup completed");
  defer fs.delete("temp.txt");

  console.log("Processing file...");
  fs.writeFile("temp.txt", "temporary data");

  // Defer statements execute here when function ends
  // Order: fs.delete("temp.txt"), then console.log("Function cleanup completed")
}
```

### Defer Execution Order

Defer statements execute in **LIFO (Last-In-First-Out)** order - the last defer statement declared executes first:

```typescript
function demonstrateOrder(): void {
  defer console.log("First defer - executes LAST");
  defer console.log("Second defer - executes SECOND");
  defer console.log("Third defer - executes FIRST");

  console.log("Function body executes normally");
}

demonstrateOrder();
// Output:
// Function body executes normally
// Third defer - executes FIRST
// Second defer - executes SECOND
// First defer - executes LAST
```

### Defer with Early Returns

Defer statements execute even when functions exit early via return statements:

```typescript
function validateAndProcess(data: string): void {
  defer console.log("Cleanup always runs");
  defer fs.delete("temp.log");

  fs.writeFile("temp.log", "Processing started");

  if (data == "") {
    console.log("Invalid data - exiting early");
    return; // Defer statements still execute!
  }

  console.log("Processing valid data");
  // Defer statements execute here too
}

validateAndProcess(""); // Cleanup runs even with early return
validateAndProcess("valid"); // Cleanup runs with normal exit
```

### Resource Management Pattern

Defer is perfect for resource cleanup and ensuring operations always complete:

```typescript
function backupAndProcess(): void {
  defer fs.delete("backup.tmp");
  defer fs.delete("processing.lock");
  defer console.log("Backup and processing complete");

  // Create temporary files
  fs.writeFile("processing.lock", "locked");
  fs.copy("important.txt", "backup.tmp");

  // Process files (might fail or exit early)
  console.log("Processing files...");

  // Cleanup always happens via defer statements
}
```

### Transaction-Style Operations

Use defer for rollback scenarios in deployment or configuration scripts:

```typescript
function deployApplication(): void {
  defer git.undoLastCommit(); // Rollback if deploy fails
  defer console.log("Deployment attempt finished");

  console.log("Starting deployment...");

  // Deploy application (multiple steps that might fail)
  if (someValidationFails()) {
    console.log("Validation failed - rolling back");
    return; // git.undoLastCommit() still executes
  }

  console.log("Deployment successful");
}
```

### File Processing with Guaranteed Cleanup

```typescript
function processLogFiles(): void {
  defer fs.delete("temp.log");
  defer fs.delete("processed.tmp");
  defer timer.stop();
  defer console.log("Log processing cleanup completed");

  timer.start();

  // Create temporary files
  fs.writeFile("temp.log", "Starting log analysis");
  fs.writeFile("processed.tmp", "");

  // Process logs (might encounter errors)
  let logContent: string = fs.readFile("system.log");

  if (logContent.length == 0) {
    console.log("No logs to process");
    return; // All cleanup still happens
  }

  // Continue processing...
  fs.writeFile("processed.tmp", logContent);
  console.log("Log processing completed");

  // Cleanup executes automatically
}
```

### Generated Bash Code for Defer

Defer statements compile to efficient bash using `trap` for automatic cleanup:

```bash
processFile() {
  local filename="$1"
  echo "Processing file..."
  echo "temporary data" > "temp.txt"

  # Defer cleanup function
  _utah_defer_cleanup_processFile_1() {
    echo "Function cleanup completed" || true
    rm -rf "temp.txt" || true
  }

  # Set up trap to execute defer statements on function exit
  trap '_utah_defer_cleanup_processFile_1' RETURN
}
```

### Defer Best Practices

1. **Use for cleanup operations**: File deletion, connection closing, lock releasing
2. **Resource management**: Ensure temporary resources are always cleaned up
3. **Logging and monitoring**: Guarantee completion messages and timing
4. **Transaction rollback**: Undo operations when errors occur
5. **Error resilience**: Individual defer failures don't stop other defers
6. **LIFO order**: Remember that defers execute in reverse order of declaration

### Defer Limitations

- **Function-scoped only**: Defer statements can only be used inside function bodies
- **No conditional registration**: Defer statements always register when encountered
- **No parameters**: Defer statements capture values at registration time
- **Error isolation**: Defer errors don't propagate to the main function

### Common Defer Patterns

#### Temporary File Management

```typescript
function processData(): void {
  defer fs.delete("data.tmp");
  defer fs.delete("results.tmp");

  fs.writeFile("data.tmp", "processing...");
  // Process data
  fs.writeFile("results.tmp", "results");
}
```

#### Timing and Logging

```typescript
function timedOperation(): void {
  defer timer.stop();
  defer console.log("Operation completed");

  timer.start();
  // Perform operation
}
```

#### Lock Management

```typescript
function criticalSection(): void {
  defer fs.delete("process.lock");

  fs.writeFile("process.lock", "locked");
  // Critical operations
}
```

## 🫴 Error Handling: Try/Catch Blocks

Utah supports try/catch blocks for error handling, allowing you to gracefully handle failures and continue script execution. Try/catch blocks are useful for handling command failures, network errors, file operations, and other operations that might fail.

### Basic Try/Catch Syntax

```typescript
try {
  // Code that might fail
  console.log("Attempting risky operation");
  exit(1); // This will cause an error
  console.log("This won't execute");
}
catch {
  console.log("Error caught and handled");
}

console.log("Script continues normally");
```

### Try/Catch with File Operations

```typescript
try {
  let content: string = fs.readFile("/nonexistent/file.txt");
  console.log("File content: " + content);
}
catch {
  console.log("Failed to read file - using default content");
  let content: string = "default content";
}
```

### Try/Catch with System Commands

```typescript
try {
  console.log("Checking if directory exists");
  // Raw bash commands that might fail
  ls /some/directory;
  console.log("Directory exists");
}
catch {
  console.log("Directory not found - creating it");
  // Handle the error by creating the directory
}
```

### Try/Catch with Custom Error Messages

```typescript
try {
  console.log("Starting critical operation");
  exit(2);
}
catch {
  console.log("Handled critical failure");
}
```

### Multiple Try/Catch Blocks

```typescript
try {
  console.log("First operation");
  let result: string = web.get("https://api.example.com/data");
}
catch {
  console.log("API call failed");
}

try {
  console.log("Second operation");
  fs.writeFile("output.txt", "some data");
}
catch {
  console.log("File write failed");
}

console.log("Both operations completed (with or without errors)");
```

### Generated Bash Code for Try/Catch

Utah try/catch blocks transpile to bash functions with subshells and proper error handling:

```bash
# Try/catch becomes bash functions with subshells
_utah_try_block_1() {
  (
    set -e  # Exit on any error within the try block
    echo "Attempting risky operation"
    exit 1
    echo "This won't execute"
  )
}

utah_catch_1() {
  echo "Error caught and handled"
}

_utah_try_block_1 || utah_catch_1
echo "Script continues normally"

# Raw bash commands get proper error handling
_utah_try_block_2() {
  (
    set -e
    echo "Checking if directory exists"
    ls /some/directory || exit 1
    echo "Directory exists"
  )
}

utah_catch_2() {
  echo "Directory not found - creating it"
}

_utah_try_block_2 || utah_catch_2
```

### Try/Catch Best Practices

- **Use for recoverable errors**: Try/catch is best for operations where you can provide alternative behavior
- **Keep try blocks focused**: Include only the specific operations that might fail
- **Provide meaningful error handling**: Use catch blocks to log errors, set defaults, or retry operations
- **Don't catch everything**: Only catch errors you can meaningfully handle
- **Clean up resources**: Use catch blocks to clean up temporary files or connections
- **Log for debugging**: Include error logging in catch blocks for troubleshooting

### Common Try/Catch Patterns

#### File Operations with Fallback

```typescript
try {
  let config: string = fs.readFile("config.json");
  console.log("Using custom config");
}
catch {
  console.log("Config file not found - using defaults");
  let config: string = '{"debug": false}';
}
```

#### Network Operations with Retry

```typescript
let attempt: number = 0;
let maxAttempts: number = 3;
let success: boolean = false;

while (attempt < maxAttempts && !success) {
  try {
    let data: string = web.get("https://api.example.com/data");
    console.log("API call successful");
    success = true;
  }
  catch {
    attempt = attempt + 1;
    console.log("Attempt " + attempt + " failed");
    if (attempt < maxAttempts) {
      console.log("Retrying...");
    }
  }
}
```

#### Conditional Operations

```typescript
let hasDocker: boolean = os.isInstalled("docker");

if (hasDocker) {
  try {
    console.log("Starting Docker container");
    // docker run commands here
  }
  catch {
    console.log("Docker operation failed");
  }
} else {
  console.log("Docker not available - skipping container operations");
}
```

## 📃 Arrays

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

Get the length of an array using the `array.length()` function:

```typescript
let items: string[] = ["apple", "banana", "cherry"];
let count: number = array.length(items);  // Gets 3
```

Check if an array is empty using the `array.isEmpty()` function:

```typescript
let emptyArray: string[] = [];
let filledArray: string[] = ["apple", "banana", "cherry"];

let emptyCheck: boolean = array.isEmpty(emptyArray);  // Gets true
let filledCheck: boolean = array.isEmpty(filledArray); // Gets false

// Use in conditionals
if (array.isEmpty(emptyArray)) {
  console.log("The array is empty");
}

if (!array.isEmpty(filledArray)) {
  console.log("The array has items");
}
```

Reverse an array using the `array.reverse()` function:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let reversed: number[] = array.reverse(numbers);  // Gets [5, 4, 3, 2, 1]

let fruits: string[] = ["apple", "banana", "cherry"];
let reversedFruits: string[] = array.reverse(fruits);  // Gets ["cherry", "banana", "apple"]

// Use in assignments and conditionals
let emptyArray: string[] = [];
let reversedEmpty: string[] = array.reverse(emptyArray);  // Gets []

if (!array.isEmpty(array.reverse(numbers))) {
  console.log("Reversed array is not empty");
}
```

Check if an array contains a specific element using the `array.contains()` function:

```typescript
let fruits: string[] = ["apple", "banana", "cherry"];
let numbers: number[] = [1, 2, 3, 4, 5];
let flags: boolean[] = [true, false, true];

// Check for specific values
let hasApple: boolean = array.contains(fruits, "apple");      // Gets true
let hasGrape: boolean = array.contains(fruits, "grape");      // Gets false
let hasThree: boolean = array.contains(numbers, 3);           // Gets true
let hasTen: boolean = array.contains(numbers, 10);            // Gets false
let hasTrue: boolean = array.contains(flags, true);           // Gets true

// Use in conditionals
if (array.contains(fruits, "apple")) {
  console.log("Found apple in fruits array");
}

if (!array.contains(numbers, 99)) {
  console.log("99 is not in the numbers array");
}

// Use with variables
let searchFruit: string = "cherry";
let searchNumber: number = 2;

if (array.contains(fruits, searchFruit)) {
  console.log(`Found ${searchFruit} in fruits`);
}

if (array.contains(numbers, searchNumber)) {
  console.log(`Found ${searchNumber} in numbers`);
}
```

Join array elements into a string using the `array.join()` function:

```typescript
let fruits: string[] = ["apple", "banana", "cherry"];
let numbers: number[] = [1, 2, 3, 4, 5];
let flags: boolean[] = [true, false, true];

// Join with default separator (comma)
let defaultJoin: string = array.join(fruits);           // Gets "apple,banana,cherry"
let commaJoin: string = array.join(fruits, ",");          // Gets "apple,banana,cherry"

// Join with custom separators
let pipeJoin: string = array.join(fruits, " | ");         // Gets "apple | banana | cherry"
let dashJoin: string = array.join(fruits, "-");           // Gets "apple-banana-cherry"
let spaceJoin: string = array.join(fruits, " ");          // Gets "apple banana cherry"
let noSeparator: string = array.join(fruits, "");         // Gets "applebananacherry"

// Join different array types
let numberString: string = array.join(numbers, "-");      // Gets "1-2-3-4-5"
let booleanString: string = array.join(flags, " & ");     // Gets "true & false & true"

// Use in string interpolation
console.log(`Fruits: ${array.join(fruits, ", ")}`);       // "Fruits: apple, banana, cherry"
console.log(`Numbers: ${array.join(numbers, " -> ")}`);   // "Numbers: 1 -> 2 -> 3 -> 4 -> 5"

// Edge cases
let empty: string[] = [];
let emptyJoin: string = array.join(empty, ",");            // Gets ""

let single: string[] = ["only"];
let singleJoin: string = array.join(single, ",");          // Gets "only"

// Use in conditionals and assignments
let csvData: string = array.join(fruits, ",");
if (csvData.length > 0) {
  console.log(`CSV data: ${csvData}`);
}
```

Sort array elements using the `array.sort()` function:

```typescript
let numbers: number[] = [3, 1, 4, 1, 5, 9, 2, 6];
let fruits: string[] = ["banana", "apple", "cherry", "date"];
let flags: boolean[] = [true, false, true, false];

// Sort with default order (ascending)
let numbersAsc: number[] = array.sort(numbers);         // Gets [1, 1, 2, 3, 4, 5, 6, 9]
let fruitsAsc: string[] = array.sort(fruits);           // Gets ["apple", "banana", "cherry", "date"]
let flagsAsc: boolean[] = array.sort(flags);            // Gets [false, false, true, true]

// Sort with explicit ascending order
let numbersAscExplicit: number[] = array.sort(numbers, "asc");  // Gets [1, 1, 2, 3, 4, 5, 6, 9]
let fruitsAscExplicit: string[] = array.sort(fruits, "asc");   // Gets ["apple", "banana", "cherry", "date"]

// Sort with descending order
let numbersDesc: number[] = array.sort(numbers, "desc");  // Gets [9, 6, 5, 4, 3, 2, 1, 1]
let fruitsDesc: string[] = array.sort(fruits, "desc");    // Gets ["date", "cherry", "banana", "apple"]
let flagsDesc: boolean[] = array.sort(flags, "desc");     // Gets [true, true, false, false]

// Use sorted arrays in expressions
let maxNumber: number = array.sort(numbers, "desc")[0];   // Gets the largest number
let minNumber: number = array.sort(numbers, "asc")[0];    // Gets the smallest number
let lastFruit: string = array.sort(fruits, "desc")[0];    // Gets the last fruit alphabetically

// Original arrays remain unchanged
console.log(`Original numbers: ${array.join(numbers, ", ")}`);     // Still [3, 1, 4, 1, 5, 9, 2, 6]
console.log(`Sorted ascending: ${array.join(numbersAsc, ", ")}`);  // [1, 1, 2, 3, 4, 5, 6, 9]
console.log(`Sorted descending: ${array.join(numbersDesc, ", ")}`); // [9, 6, 5, 4, 3, 2, 1, 1]

// Edge cases
let empty: string[] = [];
let emptySorted: string[] = array.sort(empty);          // Gets []

let single: number[] = [42];
let singleSorted: number[] = array.sort(single);        // Gets [42]

// Use in conditionals and string interpolation
let sortedFruits: string[] = fruits.sort();
if (!sortedFruits.isEmpty()) {
  console.log(`Sorted fruits: ${sortedFruits.join(", ")}`);
}

// Chain with other array methods
let topThree: string = numbers.sort("desc").join(", ");  // Gets first three highest numbers as string
```

Merge two arrays into a new array using the `array.merge()` function:

```typescript
let fruits: string[] = ["apple", "banana"];
let vegetables: string[] = ["carrot", "broccoli"];
let food: string[] = array.merge(fruits, vegetables);  // Gets ["apple", "banana", "carrot", "broccoli"]

let numbers1: number[] = [1, 2, 3];
let numbers2: number[] = [4, 5, 6];
let allNumbers: number[] = array.merge(numbers1, numbers2);  // Gets [1, 2, 3, 4, 5, 6]

let flags1: boolean[] = [true, false];
let flags2: boolean[] = [true, true];
let allFlags: boolean[] = array.merge(flags1, flags2);  // Gets [true, false, true, true]

// Order preservation: first array elements come first, then second array elements
let priorityItems: string[] = ["urgent", "high"];
let normalItems: string[] = ["medium", "low"];
let allItems: string[] = array.merge(priorityItems, normalItems);  // Gets ["urgent", "high", "medium", "low"]

// Edge cases with empty arrays
let empty: string[] = [];
let items: string[] = ["item1", "item2"];
let mergedWithEmpty: string[] = array.merge(empty, items);  // Gets ["item1", "item2"]
let mergedReversed: string[] = array.merge(items, empty);   // Gets ["item1", "item2"]
let mergedBothEmpty: string[] = array.merge(empty, empty); // Gets []

// Use with variables and conditionals
let baseConfig: string[] = ["setting1", "setting2"];
let additionalConfig: string[] = ["setting3", "setting4"];
if (array.isEmpty(additionalConfig) == false) {
  let fullConfig: string[] = array.merge(baseConfig, additionalConfig);
  console.log(`Configuration: ${array.join(fullConfig, ", ")}`);
}

// Original arrays remain unchanged
console.log(`Original fruits: ${array.join(fruits, ", ")}`);      // Still ["apple", "banana"]
console.log(`Original vegetables: ${array.join(vegetables, ", ")}`); // Still ["carrot", "broccoli"]
console.log(`Merged food: ${array.join(food, ", ")}`);            // ["apple", "banana", "carrot", "broccoli"]
```

Shuffle array elements randomly using the `array.shuffle()` function:

```typescript
// Basic shuffling with different types
let numbers: number[] = [1, 2, 3, 4, 5];
let shuffled: number[] = array.shuffle(numbers);  // Gets random order like [3, 1, 5, 2, 4]

let fruits: string[] = ["apple", "banana", "cherry"];
let shuffledFruits: string[] = array.shuffle(fruits);  // Gets random order like ["cherry", "apple", "banana"]

let flags: boolean[] = [true, false, true, false];
let shuffledFlags: boolean[] = array.shuffle(flags);  // Gets random order like [false, true, false, true]

// Edge cases
let emptyArray: string[] = [];
let shuffledEmpty: string[] = array.shuffle(emptyArray);  // Gets []

let single: number[] = [42];
let shuffledSingle: number[] = array.shuffle(single);  // Gets [42]

// Original array remains unchanged
console.log(`Original: ${array.join(numbers, ", ")}`);    // Still [1, 2, 3, 4, 5]
console.log(`Shuffled: ${array.join(shuffled, ", ")}`);   // Random order

// Each shuffle produces different results
let cards: string[] = ["A", "K", "Q", "J"];
let deal1: string[] = array.shuffle(cards);
let deal2: string[] = array.shuffle(cards);
console.log(`Deal 1: ${array.join(deal1, ", ")}`);  // Random order like ["Q", "A", "J", "K"]
console.log(`Deal 2: ${array.join(deal2, ", ")}`);  // Different random order

// Chaining with other array functions
let sorted: number[] = array.sort([3, 1, 4, 1, 5]);
let shuffledSorted: number[] = array.shuffle(sorted);
console.log(`Sorted then shuffled: ${array.join(shuffledSorted, ", ")}`);

// Use in conditionals
if (!array.isEmpty(array.shuffle(numbers))) {
  console.log("Shuffled array is not empty");
}

// Gaming applications
let deck: string[] = ["A♠", "K♠", "Q♠", "J♠", "10♠", "9♠", "8♠", "7♠"];
let shuffledDeck: string[] = array.shuffle(deck);
console.log(`Shuffled deck: ${array.join(shuffledDeck, ", ")}`);

// Random selection from array
let choices: string[] = ["red", "blue", "green", "yellow"];
let randomized: string[] = array.shuffle(choices);
let randomChoice: string = randomized[0];  // First element of shuffled array
console.log(`Random choice: ${randomChoice}`);
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

# Array isEmpty() uses bash array length check:
emptyCheck=$([ ${#emptyArray[@]} -eq 0 ] && echo "true" || echo "false")

# Array isEmpty() in conditionals:
if [ "$([ ${#emptyArray[@]} -eq 0 ] && echo "true" || echo "false")" = "true" ]; then
  echo "The array is empty"
fi

# Array reverse() uses bash array and command substitution:
reversed=($(for ((i=${#numbers[@]}-1; i>=0; i--)); do echo "${numbers[i]}"; done))

# Array reverse() with empty arrays:
reversedEmpty=($(for ((i=${#emptyArray[@]}-1; i>=0; i--)); do echo "${emptyArray[i]}"; done))

# Array contains() uses bash case statement for efficient searching:
hasApple=$(case " ${fruits[@]} " in *" apple "*) echo "true" ;; *) echo "false" ;; esac)
hasThree=$(case " ${numbers[@]} " in *" 3 "*) echo "true" ;; *) echo "false" ;; esac)

# Array merge() combines arrays using printf for proper element handling:
fruits=("apple" "banana")
vegetables=("carrot" "broccoli")
food=($(printf '%s\n' "${fruits[@]}" "${vegetables[@]}"))

# Array merge() with empty arrays:
empty=()
items=("item1" "item2")
mergedWithEmpty=($(printf '%s\n' "${empty[@]}" "${items[@]}"))  # Results in ("item1" "item2")

# Array merge() preserves order - first array elements come first:
merged=($(printf '%s\n' "${array1[@]}" "${array2[@]}"))

# Array contains() in conditionals:
if [ "$(case " ${fruits[@]} " in *" apple "*) echo "true" ;; *) echo "false" ;; esac)" = "true" ]; then
  echo "Found apple in fruits array"
fi

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

Utah provides a comprehensive set of string manipulation functions through the `string.*` namespace.

#### Core Utilities

- `string.length(value)` - Get the length of a string
- `string.trim(value)` - Remove leading and trailing whitespace
- `string.isEmpty(value)` - Check if string is empty or contains only whitespace

#### Case Conversion

- `string.toUpperCase(value)` - Convert to uppercase
- `string.toLowerCase(value)` - Convert to lowercase
- `string.capitalize(value)` - Capitalize first letter

#### Search and Testing

- `string.startsWith(value, prefix)` - Check if string starts with prefix
- `string.endsWith(value, suffix)` - Check if string ends with suffix
- `string.includes(value, substring)` - Check if string contains substring
- `string.indexOf(value, substring)` - Find index of first occurrence

#### Extraction and Manipulation

- `string.substring(value, start, length?)` - Extract substring by position
- `string.slice(value, start, end?)` - Extract portion between indices
- `string.replace(value, search, replacement)` - Replace first occurrence
- `string.replaceAll(value, search, replacement)` - Replace all occurrences
- `string.split(value, delimiter)` - Split string into array

#### Advanced Operations

- `string.padStart(value, length, pad?)` - Pad string from start
- `string.padEnd(value, length, pad?)` - Pad string from end
- `string.repeat(value, count)` - Repeat string multiple times

### Example Usage

```typescript
const message: string = "Hello, World!";
const email: string = "  user@example.com  ";

// Core utilities
let length: number = string.length(message);
let cleaned: string = string.trim(email);
let isEmpty: boolean = string.isEmpty("");

// Case conversion
let upper: string = string.toUpperCase(message); // "HELLO, WORLD!"
let lower: string = string.toLowerCase(message); // "hello, world!"
let capitalized: string = string.capitalize("hello"); // "Hello"

// Search and testing
let startsWithHello: boolean = string.startsWith(message, "Hello"); // true
let endsWithExclamation: boolean = string.endsWith(message, "!"); // true
let containsWorld: boolean = string.includes(message, "World"); // true
let worldIndex: number = string.indexOf(message, "World"); // 7

// Extraction and manipulation
let greeting: string = string.substring(message, 0, 5); // "Hello"
let portion: string = string.slice(message, 7, 12); // "World"
let newMsg: string = string.replace(message, "World", "Universe");
let parts: string[] = string.split("a,b,c", ","); // ["a", "b", "c"]

// Advanced operations
let padded: string = string.padStart("42", 5, "0"); // "00042"
let repeated: string = string.repeat("Hi", 3); // "HiHiHi"
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
- `fs.copy(sourcePath, targetPath)` - Copy a file or directory from source to target path, creates directories if needed, returns boolean
- `fs.move(sourcePath, targetPath)` - Move/rename a file or directory from source to target path, creates directories if needed, returns boolean
- `fs.rename(oldName, newName)` - Rename a file or directory within the same location, returns boolean
- `fs.delete(path)` - Delete a file or directory recursively, returns boolean
- `fs.exists(filepath)` - Check if a file or directory exists, returns boolean
- `fs.createTempFolder(prefix?, baseDir?)` - Create a secure temporary directory and return its absolute path

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

// Copy a file
fs.copy("config.txt", "backup/config.txt");

// Move/rename a file
fs.move("temp.txt", "archive/processed.txt");

// Rename a file in the same directory
fs.rename("old-file.txt", "new-file.txt");

// Copy file and check if successful
let success: boolean = fs.copy("important.doc", "archive/important.doc");
if (success) {
  console.log("File copied successfully");
} else {
  console.log("File copy failed");
}

// Move file and check if successful
let moveSuccess: boolean = fs.move("draft.md", "final/document.md");
if (moveSuccess) {
  console.log("File moved successfully");
} else {
  console.log("File move failed");
}

// Rename file and check if successful
let renameSuccess: boolean = fs.rename("report_draft.pdf", "report_final.pdf");
if (renameSuccess) {
  console.log("File renamed successfully");
} else {
  console.log("File rename failed");
}

// Delete file and check if successful
let deleteSuccess: boolean = fs.delete("temp.txt");
if (deleteSuccess) {
  console.log("File deleted successfully");
} else {
  console.log("File delete failed");
}

// Write variable content to file
let logMessage: string = "Application started";
fs.writeFile("app.log", logMessage);

// Read and process file content
let logContent: string = fs.readFile("app.log");
console.log("Log:", logContent);

// Copy log file for backup
fs.copy("app.log", "logs/backup/" + "app.log");

// Move old log files to archive
fs.move("app.log", "archive/logs/" + "app-" + utility.timestamp() + ".log");

// Delete temporary files
fs.delete("temp/processing.tmp");

// Delete entire directory
fs.delete("old-cache");

// Create a temporary working directory
let tmpDir: string = fs.createTempFolder();
console.log("Temp dir:", tmpDir);
// ... use temp dir ...
fs.delete(tmpDir); // cleanup

// Check if files exist before operations
let configExists: boolean = fs.exists("config.txt");
if (configExists) {
  let config: string = fs.readFile("config.txt");
  console.log("Config found:", config);
} else {
  console.log("Config file not found, using defaults");
}

// Check if directory exists
let backupDirExists: boolean = fs.exists("/backup");
console.log("Backup directory available:", backupDirExists);
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

# File copy operations:
mkdir -p $(dirname "backup/config.txt")
cp "config.txt" "backup/config.txt"

success=$(mkdir -p $(dirname "archive/important.doc") && cp "important.doc" "archive/important.doc" && echo "true" || echo "false")
if [ "$success" = "true" ]; then
  echo "File copied successfully"
else
  echo "File copy failed"
fi

logMessage="Application started"
echo "$logMessage" > "app.log"
logContent=$(cat "app.log")
echo "Log: $logContent"

# File existence checks:
configExists=$([ -e "config.txt" ] && echo "true" || echo "false")
if [ "$configExists" = "true" ]; then
  config=$(cat "config.txt")
  echo "Config found: $config"
else
  echo "Config file not found, using defaults"
fi

# Create secure temporary directory:
_utah_tmp_base="${TMPDIR:-/tmp}"
_utah_prefix=utah
_utah_prefix=$(echo "${_utah_prefix}" | tr -cd '[:alnum:]_.-')
[ -z "${_utah_prefix}" ] && _utah_prefix=utah
if command -v mktemp >/dev/null 2>&1; then
  dir=$(mktemp -d -t "${_utah_prefix}.XXXXXXXX" 2>/dev/null) || dir=$(mktemp -d "${_utah_tmp_base%/}/${_utah_prefix}.XXXXXXXX" 2>/dev/null)
fi
if [ -z "${dir}" ]; then
  for _i in 1 2 3 4 5 6 7 8 9 10; do
    _suf=$(LC_ALL=C tr -dc 'a-z0-9' </dev/urandom | head -c12)
    [ -z "${_suf}" ] && _suf=$$
    _cand="${_utah_tmp_base%/}/${_utah_prefix}-${_suf}"
    if mkdir -m 700 "${_cand}" 2>/dev/null; then dir="${_cand}"; break; fi
  done
fi
if [ -z "${dir}" ]; then echo "Error: Could not create temporary directory" >&2; exit 1; fi
echo "${dir}"

backupDirExists=$([ -e "/backup" ] && echo "true" || echo "false")
echo "Backup directory available: $backupDirExists"

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

## 📝 Template Functions

Utah provides template functions for processing files with variable substitution. These functions use `envsubst` to replace `${VARIABLE}` placeholders with environment variable values, making them perfect for configuration file generation and dynamic content creation.

### Available Template Functions

- `template.update(sourceFilePath, targetFilePath)` - Process a template file and write the result with environment variable substitution

### Template Functions Usage

```typescript
#!/usr/bin/env utah

// Set environment variables for template substitution
export APP_NAME="MyApplication"
export APP_VERSION="1.2.3"
export PORT="8080"

// Create a template file with variable placeholders
fs.writeFile("config.template", `
server:
  name: \${APP_NAME}
  version: \${APP_VERSION}
  port: \${PORT}
  environment: \${NODE_ENV:-production}
`);

// Process the template file and generate the final configuration
template.update("config.template", "config.yml");

// Use template.update() as an expression to check success
let success = template.update("config.template", "backup-config.yml");
if (success === "true") {
  console.log("Template processing completed successfully");
} else {
  console.log("Template processing failed");
}

// Read and display the generated configuration
let generatedConfig = fs.readFile("config.yml");
console.log("Generated configuration:", generatedConfig);
```

### Generated Bash Code for Template Functions

Template functions transpile to `envsubst` commands for efficient variable substitution:

```bash
#!/bin/bash

# Set environment variables for template substitution
export APP_NAME="MyApplication"
export APP_VERSION="1.2.3"
export PORT="8080"

# Create a template file with variable placeholders
echo "
server:
  name: \${APP_NAME}
  version: \${APP_VERSION}
  port: \${PORT}
  environment: \${NODE_ENV:-production}
" > "config.template"

# Process the template file and generate the final configuration
envsubst < "config.template" > "config.yml"

# Use template.update() as an expression to check success
success=$(_utah_template_result_1=$(envsubst < "config.template" > "backup-config.yml" && echo "true" || echo "false"); echo ${_utah_template_result_1})
if [ "${success}" = "true" ]; then
  echo "Template processing completed successfully"
else
  echo "Template processing failed"
fi

# Read and display the generated configuration
generatedConfig=$(cat "config.yml")
echo "Generated configuration: ${generatedConfig}"
```

### Template Functions Use Cases

**Configuration Management:**

```typescript
// Generate different configurations for different environments
export NODE_ENV="development"
export DATABASE_URL="localhost:5432"
export API_KEY="dev-key-123"

template.update("app.config.template", "app.config.json");
```

**CI/CD Pipeline Scripts:**

```typescript
// Generate deployment scripts with environment-specific values
export DEPLOYMENT_TARGET="staging"
export BUILD_VERSION="v2.1.0"
export DOCKER_REGISTRY="myregistry.com"

template.update("deploy.template.sh", "deploy.sh");
```

**Documentation Generation:**

```typescript
// Generate README files with dynamic content
export PROJECT_NAME="utah-lang"
export CURRENT_VERSION="1.0.0"
export AUTHOR_NAME="Utah Team"

template.update("README.template.md", "README.md");
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

## 🔨 Utility Functions

Utah provides utility functions for common operations like generating random numbers and other general-purpose tasks.

### Available Utility Functions

#### Random Number Generation

- `utility.random(min?, max?)` - Generate a random number within a specified range

### Utility Functions Usage

```typescript
// Generate random number between 0 and 32767 (default range)
let randomDefault: number = utility.random();
console.log(`Random (default): ${randomDefault}`);

// Generate random number between 0 and max
let randomMax: number = utility.random(100);
console.log(`Random 0-100: ${randomMax}`);

// Generate random number between min and max
let randomRange: number = utility.random(50, 150);
console.log(`Random 50-150: ${randomRange}`);

// Use in conditional logic
let dice: number = utility.random(1, 6);
if (dice == 6) {
  console.log("Lucky six!");
} else {
  console.log(`Rolled: ${dice}`);
}

// Generate multiple random numbers
for (let i: number = 0; i < 5; i++) {
  let num: number = utility.random(1, 10);
  console.log(`Random ${i + 1}: ${num}`);
}
```

### Random Number Examples

```typescript
// Simulate coin flip (0 or 1)
let coinFlip: number = utility.random(0, 1);
let result: string = coinFlip == 0 ? "heads" : "tails";
console.log(`Coin flip: ${result}`);

// Generate random percentage
let percentage: number = utility.random(0, 100);
console.log(`Random percentage: ${percentage}%`);

// Random delay simulation
let delay: number = utility.random(1000, 5000);
console.log(`Simulating ${delay}ms delay...`);

// Random selection from range
let temperature: number = utility.random(-10, 35);
console.log(`Random temperature: ${temperature}°C`);
```

### Generated Bash Code for Utility Functions

The utility functions transpile to efficient bash commands with built-in validation:

```bash
# utility.random() - no parameters (0 to 32767)
_utah_random_min_1=0
_utah_random_max_1=32767
if [ $_utah_random_min_1 -gt $_utah_random_max_1 ]; then
  echo "Error: min value ($_utah_random_min_1) cannot be greater than max value ($_utah_random_max_1) in utility.random()" >&2
  exit 100
fi
randomDefault=$((RANDOM * (_utah_random_max_1 - _utah_random_min_1 + 1) / 32768 + _utah_random_min_1))

# utility.random(100) - max only (0 to 100)
_utah_random_min_2=0
_utah_random_max_2=100
if [ $_utah_random_min_2 -gt $_utah_random_max_2 ]; then
  echo "Error: min value ($_utah_random_min_2) cannot be greater than max value ($_utah_random_max_2) in utility.random()" >&2
  exit 100
fi
randomMax=$((RANDOM * (_utah_random_max_2 - _utah_random_min_2 + 1) / 32768 + _utah_random_min_2))

# utility.random(50, 150) - min and max (50 to 150)
_utah_random_min_3=50
_utah_random_max_3=150
if [ $_utah_random_min_3 -gt $_utah_random_max_3 ]; then
  echo "Error: min value ($_utah_random_min_3) cannot be greater than max value ($_utah_random_max_3) in utility.random()" >&2
  exit 100
fi
randomRange=$((RANDOM * (_utah_random_max_3 - _utah_random_min_3 + 1) / 32768 + _utah_random_min_3))
randomRange=$(((RANDOM * (_utah_random_max_3 - _utah_random_min_3 + 1) / 32768) + _utah_random_min_3))

# Multiple calls use unique variable names to avoid conflicts
for i in $(seq 1 5); do
  _utah_random_min_4=1
  _utah_random_max_4=10
  num=$(((RANDOM * (_utah_random_max_4 - _utah_random_min_4 + 1) / 32768) + _utah_random_min_4))
  echo "Random ${i}: ${num}"
done
```

### Random Function Parameters

- **No parameters**: `utility.random()` returns 0-32767
- **One parameter**: `utility.random(max)` returns 0-max
- **Two parameters**: `utility.random(min, max)` returns min-max
- **Range inclusive**: Both min and max values are included in possible results
- **Unique variables**: Each call generates unique internal variables to prevent conflicts
- **Range validation**: If min > max, the script exits with code 100 and displays an error message

### Range Validation

Utah automatically validates that the minimum value is not greater than the maximum value:

```typescript
// Valid ranges - these work correctly
let valid1: number = utility.random(1, 10);    // OK: 1 ≤ 10
let valid2: number = utility.random(0, 100);   // OK: 0 ≤ 100
let valid3: number = utility.random(-5, 5);    // OK: -5 ≤ 5

// Invalid range - this will exit with code 100
let invalid: number = utility.random(150, 50); // Error: 150 > 50

// The script exits immediately with this error message:
// Error: min value (150) cannot be greater than max value (50) in utility.random()
```

The validation also works with variables:

```typescript
let min: number = 100;
let max: number = 50;
let result: number = utility.random(min, max); // Will exit with code 100
```

### Use Cases for Random Numbers

- **Testing and Simulation**: Generate test data with random values
- **Game Development**: Dice rolls, random events, procedural generation
- **Load Testing**: Random delays and intervals
- **Sampling**: Random selection from ranges
- **Security**: Random timeouts and delays (not cryptographically secure)
- **User Experience**: Random tips, messages, or content selection

#### UUID Generation

- `utility.uuid()` - Generate a universally unique identifier (UUID)

#### Text Hashing

- `utility.hash(text, algorithm?)` - Generate hash of text using specified algorithm (default: sha256)

#### Base64 Encoding/Decoding

- `utility.base64Encode(text)` - Encode text to Base64
- `utility.base64Decode(text)` - Decode Base64 text

### Additional Utility Functions Usage

```typescript
// Generate UUIDs
let sessionId: string = utility.uuid();
let requestId: string = utility.uuid();
console.log(`Session ID: ${sessionId}`);
console.log(`Request ID: ${requestId}`);

// Hash text with different algorithms
let text: string = "Hello, World!";
let md5Hash: string = utility.hash(text, "md5");
let sha1Hash: string = utility.hash(text, "sha1");
let sha256Hash: string = utility.hash(text, "sha256");
let sha512Hash: string = utility.hash(text, "sha512");

// Default algorithm is sha256
let defaultHash: string = utility.hash(text);

console.log(`Original text: ${text}`);
console.log(`MD5 hash: ${md5Hash}`);
console.log(`SHA1 hash: ${sha1Hash}`);
console.log(`SHA256 hash: ${sha256Hash}`);
console.log(`SHA512 hash: ${sha512Hash}`);
console.log(`Default hash: ${defaultHash}`);

// Base64 encoding and decoding
let original: string = "Hello, Utah!";
let encoded: string = utility.base64Encode(original);
let decoded: string = utility.base64Decode(encoded);

console.log(`Original: ${original}`);
console.log(`Encoded: ${encoded}`);
console.log(`Decoded: ${decoded}`);

// Working with secrets
let secret: string = "This is a secret message";
let secretHash: string = utility.hash(secret, "sha256");
let secretEncoded: string = utility.base64Encode(secret);

console.log(`Secret hash: ${secretHash}`);
console.log(`Secret encoded: ${secretEncoded}`);
```

### Generated Bash Code for New Utility Functions

```bash
#!/bin/bash

# utility.uuid() - generates UUID with fallbacks
sessionId=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)

# utility.hash(text, algorithm) - generates hash using specified algorithm
md5Hash=$(echo -n ${text} | case "md5" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "md5"" >&2; exit 1 ;; esac)

# utility.base64Encode(text) - encodes text to Base64
encoded=$(echo -n ${original} | base64 -w 0)

# utility.base64Decode(text) - decodes Base64 text
decoded=$(echo -n ${encoded} | base64 -d)
```

### Utility Functions Use Cases

#### UUID Generation Use Cases

- **Session Management**: Unique session identifiers
- **Request Tracking**: Unique request IDs for logging and monitoring
- **File Naming**: Unique file names to prevent conflicts
- **Database Keys**: Primary keys for database records
- **API Keys**: Temporary API keys and tokens

#### Text Hashing Use Cases

- **Data Integrity**: Verify file integrity with checksums
- **Password Hashing**: Hash passwords before storage (use with salt)
- **Content Fingerprinting**: Generate unique fingerprints for content
- **Cache Keys**: Generate cache keys from content
- **Data Deduplication**: Identify duplicate content

#### Base64 Encoding Use Cases

- **Data Transmission**: Encode binary data for text-based protocols
- **Configuration Files**: Store binary data in text configuration files
- **API Communication**: Encode payloads for API requests
- **Email Attachments**: Encode files for email transmission
- **Web Development**: Data URIs for embedded resources

## 🌐 Web Functions

Utah provides web functions for making HTTP requests and interacting with web APIs and resources.

### Available Web Functions

#### HTTP Requests

- `web.get(url)` - Perform an HTTP GET request to the specified URL

### Web Functions Usage

```typescript
// Make a simple GET request to a URL
let response: string = web.get("https://api.github.com/users/octocat");
console.log(`Response: ${response}`);

// Use with variables
let apiUrl: string = "https://httpbin.org/get";
let data: string = web.get(apiUrl);
console.log(`API response: ${data}`);

// Check response and handle with conditionals
let healthCheck: string = web.get("https://httpbin.org/status/200");
if (healthCheck != "") {
  console.log("Service is available");
} else {
  console.log("Service might be down");
}

// Use in loops for multiple endpoints
let endpoints: string[] = ["https://httpbin.org/get", "https://httpbin.org/uuid"];
for (let endpoint: string in endpoints) {
  let result: string = web.get(endpoint);
  console.log(`Endpoint ${endpoint}: ${result}`);
}
```

### Web Function Examples

```typescript
// Fetch JSON data from an API
let userApi: string = "https://jsonplaceholder.typicode.com/users/1";
let userData: string = web.get(userApi);
console.log(`User data: ${userData}`);

// Download content from a URL
let textUrl: string = "https://httpbin.org/robots.txt";
let robotsTxt: string = web.get(textUrl);
console.log(`Robots.txt content: ${robotsTxt}`);

// Health check endpoints
let status200: string = web.get("https://httpbin.org/status/200");
let status404: string = web.get("https://httpbin.org/status/404");

if (status200 != "") {
  console.log("200 endpoint is working");
}

// Use with string interpolation
let baseUrl: string = "https://httpbin.org";
let endpoint: string = "json";
let fullUrl: string = `${baseUrl}/${endpoint}`;
let response: string = web.get(fullUrl);
console.log(`Response from ${fullUrl}: ${response}`);
```

### Generated Bash Code for Web Functions

The web functions transpile to efficient bash commands using `curl`:

```bash
# web.get("https://api.github.com/users/octocat")
response=$(curl -s "https://api.github.com/users/octocat")

# web.get() with variables
apiUrl="https://httpbin.org/get"
data=$(curl -s "${apiUrl}")

# Multiple requests in a loop
for endpoint in "https://httpbin.org/get" "https://httpbin.org/uuid"; do
  result=$(curl -s "${endpoint}")
  echo "Endpoint ${endpoint}: ${result}"
done
```

### Web Function Features

- **Silent operation**: Uses `curl -s` to suppress progress output
- **Variable support**: Works with both string literals and variables
- **String interpolation**: Compatible with string interpolation and variable substitution
- **Error handling**: Failed requests return empty strings that can be checked
- **Cross-platform**: Uses `curl` which is available on most Unix-like systems

### Use Cases for Web Functions

- **API Integration**: Fetch data from REST APIs and web services
- **Health Monitoring**: Check if web services and endpoints are responding
- **Content Fetching**: Download text content, configuration files, or data
- **Automation**: Integrate web requests into deployment and maintenance scripts
- **Testing**: Verify API responses and web service functionality
- **Data Collection**: Gather information from multiple web sources

## ⚡️ Parallel Function Calls

Utah supports parallel execution of user-defined functions using the `parallel` keyword. This allows you to run functions in the background, enabling concurrent operations and non-blocking flows.

### Usage

Prefix a function call with `parallel` to run it in the background:

```typescript
function slowEcho(msg: string): void {
  console.log(`Start: ${msg}`);
  let _ = `$(sleep 1)`;
  console.log(`End: ${msg}`);
}

console.log("Main start");
parallel slowEcho("A");
parallel slowEcho("B");
console.log("Main end");
let _ = `$(wait)`;
console.log("All done");
```

### Generated Bash Code

```bash
slowEcho() {
  local msg="$1"
  echo "Start: ${msg}"
  _="$(sleep 1)"
  echo "End: ${msg}"
}
echo "Main start"
slowEcho "A" &
slowEcho "B" &
echo "Main end"
_="$(wait)"
echo "All done"
```

### How It Works

- Each `parallel` function call is run in the background using Bash's `&` operator.
- The main script flow continues immediately after launching parallel jobs.
- Use `wait` to block until all background jobs finish.

### Use Cases

- Running multiple tasks concurrently (e.g., downloads, processing)
- Improving script performance for independent operations
- Non-blocking UI or logging

## 📜 Script Metadata and Argument Parsing

Utah provides a powerful argument parsing system that allows you to create professional command-line scripts with help text, default values, and type validation. You can also add metadata to your scripts for better documentation.

### Script Description

Add a description to your script using `script.description()`:

```typescript
script.description("User Management Tool - Creates and manages user accounts");
```

This description will be displayed when users run your script with the `--help` flag.

### Argument Definition

Define command-line arguments using `args.define()` with the following syntax:

```typescript
args.define(longFlag, shortFlag, description, type, required, defaultValue);
```

#### Basic Argument Definition

```typescript
// Simple flag without short version
args.define("--verbose", "", "Enable verbose output");

// Flag with both long and short versions
args.define("--help", "-h", "Show this help message");

// String argument with default value
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");

// Number argument with default value
args.define("--port", "-p", "Server port number", "number", false, 8080);

// Boolean flag (presence indicates true)
args.define("--debug", "-d", "Enable debug mode", "boolean", false, false);
```

#### Advanced Argument Examples

```typescript
// Required argument
args.define("--config", "-c", "Configuration file path", "string", true);

// Optional argument with no default (will be empty if not provided)
args.define("--output", "-o", "Output file path", "string", false);

// Multiple argument types in one script
args.define("--host", "-h", "Database host", "string", false, "localhost");
args.define("--port", "-p", "Database port", "number", false, 5432);
args.define("--ssl", "", "Use SSL connection", "boolean", false, false);
args.define("--timeout", "-t", "Connection timeout in seconds", "number", false, 30);
```

### Checking for Arguments

Use `args.has()` to check if an argument was provided:

```typescript
if (args.has("--help")) {
  args.showHelp();
  exit(0);
}

if (args.has("--verbose")) {
  console.log("Verbose mode enabled");
}

// Check for boolean flags
if (args.has("--debug")) {
  console.log("Debug mode is ON");
}
```

### Getting Argument Values

Use `args.get()` to retrieve argument values. If not provided, returns the default value:

```typescript
let userName: string = args.get("--name");      // Returns "Anonymous" if not provided
let port: number = args.get("--port");          // Returns 8080 if not provided
let configFile: string = args.get("--config"); // Returns provided value or empty string
```

### Displaying Help

Show automatically generated help text using `args.showHelp()`:

```typescript
if (args.has("--help")) {
  args.showHelp();
  exit(0);
}
```

The help output includes:

- Script description
- Usage syntax
- All defined arguments with descriptions
- Default values for optional arguments

### Getting All Arguments

Retrieve all command-line arguments as passed to the script:

```typescript
console.log("All arguments: " + args.all());
```

### Complete Example

Here's a comprehensive example showing all argument parsing features:

```typescript
script.description("User Management Tool - Creates and manages user accounts");

// Define various types of arguments
args.define("--version", "-v", "Show the application version");
args.define("--help", "-h", "Show this help message");
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");
args.define("--age", "", "Specify the user's age", "number", false, 25);
args.define("--admin", "", "Create user with admin privileges", "boolean", false, false);
args.define("--config", "-c", "Configuration file path", "string", true);

// Handle help and version
if (args.has("--help")) {
  args.showHelp();
  exit(0);
}

if (args.has("--version")) {
  console.log("User Management Tool v1.0.0");
  exit(0);
}

// Get argument values
let userName: string = args.get("--name");
let userAge: number = args.get("--age");
let isAdmin: boolean = args.has("--admin");
let configPath: string = args.get("--config");

// Use the values
console.log(`Creating user: ${userName}, age: ${userAge}, admin: ${isAdmin}`);
console.log(`Using config: ${configPath}`);
console.log("All arguments: " + args.all());
```

### Generated Help Output

When users run your script with `--help`, they'll see:

```text
User Management Tool - Creates and manages user accounts

Usage: script.sh [OPTIONS]

Options:
  -v, --version            Show the application version
  -h, --help               Show this help message
  -n, --name               Specify the user's name (default: Anonymous)
      --age                Specify the user's age (default: 25)
      --admin              Create user with admin privileges
  -c, --config             Configuration file path
```

### Generated Bash Code for Arguments

The argument parsing system generates efficient bash code:

```bash
# Argument metadata arrays
__UTAH_ARG_NAMES=("--name" "--port" "--debug")
__UTAH_ARG_SHORT_NAMES=("-n" "-p" "-d")
__UTAH_ARG_DESCRIPTIONS=("User name" "Port number" "Debug mode")
__UTAH_ARG_TYPES=("string" "number" "boolean")
__UTAH_ARG_REQUIRED=("false" "false" "false")
__UTAH_ARG_DEFAULTS=("Anonymous" "8080" "false")

# Helper functions for argument parsing
__utah_has_arg() {
  # Checks if an argument was provided
}

__utah_get_arg() {
  # Gets argument value or returns default
}

__utah_show_help() {
  # Displays formatted help text
}

# Script description
__UTAH_SCRIPT_DESCRIPTION="Your script description"

# Usage in script
userName=$(__utah_get_arg "--name" "$@")
isDebug=$(__utah_has_arg "--debug" "$@" && echo "true" || echo "false")
```

### Argument Parsing Features

- **Type Support**: String, number, and boolean argument types
- **Default Values**: Automatic fallback values when arguments aren't provided
- **Short and Long Flags**: Support for both `-n` and `--name` style arguments
- **Required Arguments**: Mark arguments as mandatory
- **Auto-generated Help**: Professional help text with usage and descriptions
- **POSIX Compliance**: Generated bash code works on all POSIX-compliant shells
- **Flexible Syntax**: Support for `--arg=value` and `--arg value` formats

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

## ⏰ Job Queueing and Scheduling

Utah provides powerful job scheduling capabilities that allow you to create cron jobs directly from your scripts. The scheduler functions compile to robust bash implementations that create persistent scheduled tasks.

### Available Scheduler Functions

#### Cron Job Scheduling

- `scheduler.cron("cronPattern", lambda)` - Schedule a job using standard cron pattern syntax

### Scheduler Usage

```typescript
// Schedule a job every 6 hours
scheduler.cron("0 */6 * * *", () => {
  console.log("Running every 6 hours...");
  console.log("Performing maintenance tasks...");
});

// Schedule daily at midnight
scheduler.cron("0 0 * * *", () => {
  console.log("Daily maintenance starting...");
  let backupTime: string = "00:00";
  console.log(`Backup started at ${backupTime}`);
});

// Schedule every Monday at 2 AM
scheduler.cron("0 2 * * 1", () => {
  console.log("Weekly report generation");
  let reportDate: string = "Monday Report";
  console.log(`Generating: ${reportDate}`);
});

// Schedule every 15 minutes during business hours
scheduler.cron("*/15 9-17 * * 1-5", () => {
  console.log("Business hours health check");
});
```

### Practical Scheduling Examples

```typescript
// System maintenance script with multiple scheduled tasks
script.description("Automated System Maintenance Scheduler");

// Daily log cleanup at 2 AM
scheduler.cron("0 2 * * *", () => {
  console.log("Starting daily log cleanup...");
  // Cleanup logic would go here
  console.log("Log cleanup completed");
});

// Weekly database backup every Sunday at 3 AM
scheduler.cron("0 3 * * 0", () => {
  console.log("Starting weekly database backup...");
  let timestamp: string = "$(date +%Y%m%d_%H%M%S)";
  console.log(`Backup timestamp: ${timestamp}`);
  console.log("Database backup completed");
});

// Hourly health check during business hours
scheduler.cron("0 9-17 * * 1-5", () => {
  console.log("Performing hourly health check...");
  let status: string = "healthy";
  console.log(`System status: ${status}`);
});

// Monthly report generation on the 1st at 6 AM
scheduler.cron("0 6 1 * *", () => {
  console.log("Generating monthly report...");
  console.log("Monthly report completed");
});
```

### Scheduler Integration with Other Utah Functions

```typescript
// Comprehensive maintenance script with privilege checks
let hasPrivileges: boolean = console.isSudo();

if (!hasPrivileges) {
  console.log("Warning: Some maintenance tasks require sudo privileges");
}

// Schedule system updates (requires sudo)
scheduler.cron("0 3 * * 0", () => {
  let isSudo: boolean = console.isSudo();
  if (isSudo) {
    console.log("Starting system updates...");
    // Update commands would go here
    console.log("System updates completed");
  } else {
    console.log("Skipped system updates - insufficient privileges");
  }
});

// Schedule application-level maintenance (no sudo required)
scheduler.cron("0 1 * * *", () => {
  console.log("Starting application maintenance...");

  // Clean temporary files
  console.log("Cleaning temporary files...");

  // Rotate logs
  console.log("Rotating application logs...");

  console.log("Application maintenance completed");
});

// User confirmation for critical operations
let confirmScheduling: boolean = console.promptYesNo("Schedule automatic maintenance tasks?");

if (confirmScheduling) {
  console.log("Scheduling maintenance tasks...");

  scheduler.cron("0 2 * * *", () => {
    console.log("Automated maintenance running...");
  });

  console.log("Maintenance scheduling complete!");
} else {
  console.log("Maintenance scheduling cancelled");
}
```

### Generated Bash Code for Scheduler

The scheduler functions compile to robust bash implementations using the "Alternative Implementation (More Robust)" approach:

```bash
# scheduler.cron("0 */6 * * *", lambda) becomes:

# Create Utah cron directory
_utah_cron_dir="$HOME/.utah/cron"
mkdir -p "${_utah_cron_dir}"
_utah_cron_script="${_utah_cron_dir}/job_$(date +%s)_$$.sh"

# Generate the cron job script
cat > "${_utah_cron_script}" << 'EOF'
#!/bin/bash
# Generated by Utah - scheduler.cron("0 */6 * * *")
echo "Running every 6 hours..."
echo "Performing maintenance tasks..."
EOF

# Make the script executable
chmod +x "${_utah_cron_script}"

# Check if similar cron job already exists
_utah_cron_pattern="0 \*/6 \* \* \*.*utah.*job_"
if ! crontab -l 2>/dev/null | grep -q "${_utah_cron_pattern}"; then
    # Add to crontab
    (crontab -l 2>/dev/null; echo "0 */6 * * * ${_utah_cron_script}") | crontab -
    echo "Cron job installed: 0 */6 * * * ${_utah_cron_script}"
else
    echo "Similar cron job already exists"
fi
```

### Cron Pattern Reference

| Pattern | Description | Example |
|---------|-------------|---------|
| `"0 */6 * * *"` | Every 6 hours | Every 6 hours starting at midnight |
| `"0 0 * * *"` | Daily at midnight | Every day at 12:00 AM |
| `"0 2 * * 1"` | Every Monday at 2 AM | Weekly on Monday at 2:00 AM |
| `"*/15 * * * *"` | Every 15 minutes | Every quarter hour |
| `"0 9-17 * * 1-5"` | Business hours | Every hour from 9 AM to 5 PM, Monday to Friday |
| `"0 6 1 * *"` | Monthly | First day of every month at 6:00 AM |
| `"0 0 * * 0"` | Weekly | Every Sunday at midnight |

### Scheduler Features

- **Persistent Jobs**: Creates actual cron jobs that survive script execution
- **Duplicate Prevention**: Checks for existing similar jobs before installation
- **Unique Scripts**: Generates timestamp-based script names to avoid conflicts
- **Standard Cron**: Uses standard cron syntax for maximum compatibility
- **Error Handling**: Graceful handling of crontab operations
- **Clean Organization**: Stores all Utah-generated scripts in `~/.utah/cron/`

### Use Cases for Scheduler

- **System Maintenance**: Automated cleanup, updates, and monitoring
- **Data Processing**: ETL jobs, report generation, data synchronization
- **DevOps Automation**: Build triggers, deployment schedules, health checks
- **Backup Operations**: Database backups, file synchronization, archiving
- **Monitoring**: Log analysis, performance monitoring, alert generation
- **Batch Processing**: Large data processing, batch file operations

### Scheduler Best Practices

- **Use descriptive lambda content** to make generated scripts self-documenting
- **Check system privileges** when scheduling system-level operations
- **Test cron patterns** using online cron expression validators
- **Monitor script execution** by checking generated scripts in `~/.utah/cron/`
- **Use absolute paths** in scheduled scripts for reliability
- **Handle errors gracefully** in scheduled tasks
- **Document schedule reasoning** in script comments

### Scheduler Technical Notes

- **Script Storage**: Generated scripts are stored in `$HOME/.utah/cron/`
- **Naming Convention**: Scripts use `job_$(date +%s)_$$.sh` format for uniqueness
- **Crontab Integration**: Uses standard `crontab` command for job installation
- **Duplicate Detection**: Checks existing crontab entries before adding new jobs
- **Standard Compliance**: Generated cron entries follow POSIX cron syntax

## 🔐 Console System Functions

Utah provides console system functions for checking system privileges and performing system-level operations within your scripts.

### Available Console System Functions

#### Screen Control

- `console.clear()` - Clear the terminal screen

#### Privilege Checking

- `console.isSudo()` - Check if the script is running with root/sudo privileges
- `console.isInteractive()` - Check if the script is running in an interactive terminal session

#### System Detection

- `console.getShell()` - Get the name of the current shell (bash, zsh, fish, etc.)

#### User Interaction

- `console.promptYesNo("prompt text")` - Display a yes/no prompt and return user's choice as boolean

### Console System Functions Usage

```typescript
// Clear the terminal screen
console.clear();
console.log("Screen has been cleared - this is the first visible line");

// Use console.clear() in conditional logic
let shouldClear: boolean = true;
if (shouldClear) {
  console.log("About to clear the screen...");
  console.clear();
  console.log("Screen cleared!");
}

// Clear screen in functions
function resetDisplay(): void {
  console.clear();
  console.log("Display reset");
}

resetDisplay();

// Check if running with sudo privileges
let isSudo: boolean = console.isSudo();

if (isSudo) {
  console.log("Running with elevated privileges");
  // Perform admin operations
} else {
  console.log("Running with normal user privileges");
  // Prompt for sudo or exit
}

// Check if running in interactive terminal
let isInteractive: boolean = console.isInteractive();

if (isInteractive) {
  console.log("Running in interactive mode - can show prompts");
  let userName: string = console.promptText("Enter your name");
  console.log(`Hello, ${userName}!`);
} else {
  console.log("Running in non-interactive mode (pipe/background)");
  console.log("Using default configuration...");
}

// Prompt user for yes/no confirmation
let proceed: boolean = console.promptYesNo("Do you want to continue?");

if (proceed) {
  console.log("User chose to continue");
  // Continue with operation
} else {
  console.log("User chose to cancel");
  // Cancel operation or exit
}

// Use in conditional operations
if (console.isSudo()) {
  console.log("Installing system packages...");
} else {
  console.log("Please run with sudo for system operations");
}

// Combine interactive and privilege checks
if (console.isInteractive() && console.isSudo()) {
  console.log("Interactive elevated session - can show prompts");
} else if (!console.isInteractive()) {
  console.log("Non-interactive session - using defaults");
} else {
  console.log("Interactive but no elevated privileges");
}

// Detect current shell for shell-specific features
let currentShell: string = console.getShell();
console.log(`Running in: ${currentShell}`);
if (currentShell == "zsh") {
  console.log("Using Zsh-specific autocomplete features");
} else if (currentShell == "bash") {
  console.log("Using Bash-compatible syntax");
}

// Chain prompts for user interaction
let confirmInstall: boolean = console.promptYesNo("Install new packages?");
if (confirmInstall) {
  let confirmRestart: boolean = console.promptYesNo("Restart system after install?");
  console.log(`Install: ${confirmInstall}, Restart: ${confirmRestart}`);
}

// Store result for multiple checks
let hasRootAccess: boolean = console.isSudo();
console.log(`Root access: ${hasRootAccess}`);
```

### Security and Admin Operations Example

```typescript
// Script that requires elevated privileges and user confirmation
let isRoot: boolean = console.isSudo();

if (!isRoot) {
  console.log("Error: This script requires sudo privileges");
  console.log("Please run: sudo utah compile script.shx && sudo ./script.sh");
  exit(1);
}

console.log("Verified: Running with administrator privileges");

// Confirm dangerous operation with user
let confirmAction: boolean = console.promptYesNo("This will modify system files. Continue?");

if (!confirmAction) {
  console.log("Operation cancelled by user");
  exit(0);
}

console.log("Proceeding with system configuration...");

// Additional confirmation for destructive actions
let confirmDelete: boolean = console.promptYesNo("Delete existing configuration files?");
if (confirmDelete) {
  console.log("Removing old configuration...");
} else {
  console.log("Keeping existing configuration...");
}

console.log("Installation complete!");
```

### Generated Bash Code for Console System Functions

The console system functions transpile to efficient bash commands:

```bash
# console.isSudo() becomes:
isSudo=$([ "$(id -u)" -eq 0 ] && echo "true" || echo "false")

# console.isInteractive() becomes:
isInteractive=$([ -t 0 ] && echo "true" || echo "false")

# console.getShell() becomes:
currentShell=$(basename "${SHELL:-$0}")

# console.promptYesNo("prompt text") becomes:
proceed=$(while true; do read -p "Do you want to continue? (y/n): " yn; case $yn in [Yy]* ) echo "true"; break;; [Nn]* ) echo "false"; break;; * ) echo "Please answer yes or no.";; esac; done)

# Complete example:
isSudo=$([ "$(id -u)" -eq 0 ] && echo "true" || echo "false")
isInteractive=$([ -t 0 ] && echo "true" || echo "false")

if [ "${isSudo}" = "true" ]; then
  echo "Running with elevated privileges"
else
  echo "Running with normal user privileges"
fi

if [ "${isInteractive}" = "true" ]; then
  echo "Running in interactive mode - can show prompts"
else
  echo "Running in non-interactive mode (pipe/background)"
fi

proceed=$(while true; do read -p "Do you want to continue? (y/n): " yn; case $yn in [Yy]* ) echo "true"; break;; [Nn]* ) echo "false"; break;; * ) echo "Please answer yes or no.";; esac; done)

if [ "${proceed}" = "true" ]; then
  echo "User chose to continue"
else
  echo "User chose to cancel"
fi

# Conditional usage:
if [ "$([ "$(id -u)" -eq 0 ] && echo "true" || echo "false")" = "true" ]; then
  echo "Installing system packages..."
else
  echo "Please run with sudo for system operations"
fi
```

### Console System Functions Use Cases

- **Privilege Verification**: Ensure scripts have necessary permissions before execution
- **Interactive Detection**: Determine if the script can display prompts and dialogs
- **Shell Detection**: Identify the current shell to enable shell-specific features
- **Environment Adaptation**: Adjust script behavior based on execution context (interactive vs automated)
- **Security Checks**: Validate user permissions for sensitive operations
- **Admin Scripts**: Build installation and configuration scripts that require root access
- **Conditional Logic**: Execute different code paths based on privilege level and interactivity
- **Error Prevention**: Fail gracefully when insufficient privileges are detected or when prompts can't be shown
- **User Guidance**: Provide helpful messages about required permissions and execution context

### Security Best Practices

- **Always check privileges** before performing system-level operations
- **Check interactivity** before showing prompts to avoid script hangs in automated environments
- **Fail early** if required privileges are not available
- **Provide alternatives** for non-interactive execution (defaults, environment variables, config files)
- **Provide clear error messages** when privilege checks fail or when interactivity is required but not available
- **Use `console.isSudo()`** instead of assuming user permissions
- **Use `console.isInteractive()`** before displaying prompts or dialogs
- **Document privilege requirements** in script comments and usage instructions
- **Test scripts** in both interactive and non-interactive environments

### Technical Notes

- Uses the standard Unix `id -u` command to check effective user ID
- Returns `true` when effective user ID is 0 (root)
- Returns `false` for all non-root users
- Works consistently across all Unix-like systems (Linux, macOS, BSD)
- Lightweight check with minimal performance impact

## 🎯 Console Dialog Functions

Utah provides a comprehensive set of dialog functions for creating interactive terminal user interfaces. These functions automatically detect and use the best available dialog system (`dialog`, `whiptail`, or fallback to basic prompts).

### Message Display Functions

```typescript
// Show informational message
console.showMessage("Welcome", "Welcome to Utah Dialog Demo!");

// Show different types of messages
console.showInfo("Information", "Process completed successfully");
console.showWarning("Warning", "This action cannot be undone");
console.showError("Error", "Failed to connect to server");
console.showSuccess("Success", "File uploaded successfully");
```

### User Input Functions

```typescript
// Prompt for text input with default value
let name: string = console.promptText("Enter your name", "John Doe");
console.log("Hello, " + name);

// Prompt for password (input is hidden)
let password: string = console.promptPassword("Enter your password");

// Prompt for numeric input with validation
let age: number = console.promptNumber("Enter your age", 18, 120, 25);
console.log("You are " + age + " years old");

// File and directory selection
let configFile: string = console.promptFile("Select config file", "*.json");
let workDir: string = console.promptDirectory("Choose working directory", "/tmp");
```

### Choice and Confirmation Functions

```typescript
// Single choice selection
let color: string = console.showChoice(
  "Color Selection",
  "Choose your favorite color:",
  "Red,Green,Blue,Yellow",
  0  // default selection index
);
console.log("Selected color: " + color);

// Multi-choice selection
let features: string = console.showMultiChoice(
  "Features",
  "Select features to enable:",
  "Logging,Monitoring,Analytics,Backup",
  "Logging,Monitoring"  // default selections
);

// Yes/No confirmation with custom default
let confirmed: boolean = console.showConfirm(
  "Confirm Action",
  "Are you sure you want to continue?",
  "yes"  // default button
);

if (confirmed) {
  console.showSuccess("Confirmed", "Operation will proceed");
} else {
  console.showInfo("Cancelled", "Operation was cancelled");
}
```

### Progress Display

```typescript
// Show progress bar (useful with background processes)
console.showProgress("Processing", "Loading data...", 75, false);
```

### Dialog Function Parameters

| Function | Parameters | Returns | Description |
|----------|------------|---------|-------------|
| `console.showMessage(title, message)` | title: string, message: string | void | General message dialog |
| `console.showInfo(title, message)` | title: string, message: string | void | Information message |
| `console.showWarning(title, message)` | title: string, message: string | void | Warning message |
| `console.showError(title, message)` | title: string, message: string | void | Error message |
| `console.showSuccess(title, message)` | title: string, message: string | void | Success message |
| `console.showChoice(title, message, options, defaultIndex?)` | title: string, message: string, options: string, defaultIndex?: number | string | Single choice menu |
| `console.showMultiChoice(title, message, options, defaultSelected?)` | title: string, message: string, options: string, defaultSelected?: string | string | Multi-choice checklist |
| `console.showConfirm(title, message, defaultButton?)` | title: string, message: string, defaultButton?: string | boolean | Yes/no confirmation |
| `console.showProgress(title, message, percent, canCancel?)` | title: string, message: string, percent: number, canCancel?: boolean | void | Progress display |
| `console.promptText(prompt, defaultValue?, validation?)` | prompt: string, defaultValue?: string, validation?: string | string | Text input |
| `console.promptPassword(prompt)` | prompt: string | string | Password input (hidden) |
| `console.promptNumber(prompt, min?, max?, defaultValue?)` | prompt: string, min?: number, max?: number, defaultValue?: number | number | Numeric input with validation |
| `console.promptFile(prompt, filter?)` | prompt: string, filter?: string | string | File selection |
| `console.promptDirectory(prompt, defaultPath?)` | prompt: string, defaultPath?: string | string | Directory selection |

### Dialog System Compatibility

The dialog functions automatically use the best available system:

1. **Primary**: `dialog` command (full-featured TUI)
2. **Secondary**: `whiptail` command (lightweight TUI)
3. **Fallback**: Basic terminal prompts using `read` and `echo`

This ensures your scripts work across different environments while providing the best possible user experience.

### Interactive Script Example

```typescript
#!/usr/bin/env utah run

// Welcome the user
console.showMessage("Setup Wizard", "Welcome to the application setup!");

// Get user information
let userName: string = console.promptText("Your name", "User");
let userEmail: string = console.promptText("Your email");

// Choose installation type
let installType: string = console.showChoice(
  "Installation Type",
  "Choose installation type:",
  "Quick,Custom,Advanced",
  0
);

// Confirm installation
let confirmed: boolean = console.showConfirm(
  "Confirm Setup",
  `Install ${installType} setup for ${userName}?`
);

if (confirmed) {
  console.showProgress("Installing", "Please wait...", 100);
  console.showSuccess("Complete", "Setup completed successfully!");
} else {
  console.showInfo("Cancelled", "Setup was cancelled");
}
```

## 🧩 JSON Functions

Utah provides comprehensive JSON parsing and manipulation functions that allow you to work with JSON data directly in your scripts. These functions use `jq` under the hood to provide powerful and efficient JSON operations with jq-style path syntax.

### Available JSON Functions

#### JSON Parsing and Validation

- `json.parse(jsonString)` - Parse a JSON string into an object that can be manipulated
- `json.stringify(jsonObject)` - Convert a JSON object back to a string representation
- `json.isValid(jsonString)` - Check if a string contains valid JSON syntax

#### JSON Property Access and Manipulation

- `json.get(jsonObject, path)` - Get a value from JSON using jq-style path (e.g., ".user.name")
- `json.set(jsonObject, path, value)` - Set a value in JSON object using jq-style path
- `json.has(jsonObject, path)` - Check if a property exists at the given path
- `json.delete(jsonObject, path)` - Remove a property from JSON object

#### JSON Utility Functions

- `json.keys(jsonObject)` - Get all keys from a JSON object as an array
- `json.values(jsonObject)` - Get all values from a JSON object as an array
- `json.merge(jsonObject1, jsonObject2)` - Merge two JSON objects (jsonObject2 overwrites jsonObject1)
- `json.installDependencies()` - Check and install required JSON processing tools (jq)

### JSON Functions Usage

```typescript
// Basic JSON parsing and validation
let configJson: string = '{"debug": true, "port": 8080, "name": "MyApp"}';
let isValid: boolean = json.isValid(configJson);

if (isValid) {
  let config: object = json.parse(configJson);
  console.log("Configuration loaded successfully");

  // Access properties using jq-style paths
  let appName: string = json.get(config, ".name");
  let port: number = json.get(config, ".port");
  let debugMode: boolean = json.get(config, ".debug");

  console.log(`Starting ${appName} on port ${port}`);

  if (debugMode) {
    console.log("Debug mode is enabled");
  }
} else {
  console.log("Invalid JSON configuration");
  exit(1);
}

// Working with nested JSON objects
let userData: string = '{"user": {"name": "Jane", "settings": {"theme": "dark", "notifications": true}}}';
let userObj: object = json.parse(userData);

// Access nested properties
let userName: string = json.get(userObj, ".user.name");
let theme: string = json.get(userObj, ".user.settings.theme");
let notifications: boolean = json.get(userObj, ".user.settings.notifications");

console.log(`User: ${userName}, Theme: ${theme}, Notifications: ${notifications}`);

// Modify JSON properties
let updatedUser: object = json.set(userObj, ".user.name", "John");
updatedUser = json.set(updatedUser, ".user.settings.theme", "light");
updatedUser = json.set(updatedUser, ".user.settings.newFeature", true);

// Check if properties exist
let hasEmail: boolean = json.has(updatedUser, ".user.email");
let hasTheme: boolean = json.has(updatedUser, ".user.settings.theme");

console.log(`Has email: ${hasEmail}, Has theme: ${hasTheme}`);

// Remove properties
let userWithoutTheme: object = json.delete(updatedUser, ".user.settings.theme");
console.log("Theme setting removed");

// Convert back to string
let finalUserJson: string = json.stringify(userWithoutTheme);
console.log("Updated user data: " + finalUserJson);
```

### Working with JSON Arrays and Complex Data

```typescript
// JSON array manipulation
let fruitsJson: string = '{"fruits": ["apple", "banana", "cherry"], "count": 3}';
let data: object = json.parse(fruitsJson);

// Access array elements (using jq array syntax)
let firstFruit: string = json.get(data, ".fruits[0]");
let fruitCount: number = json.get(data, ".count");

console.log(`First fruit: ${firstFruit}, Total count: ${fruitCount}`);

// Add new properties
let updatedData: object = json.set(data, ".fruits[3]", "date");
updatedData = json.set(updatedData, ".count", 4);

// Get all keys and values
let keys: string[] = json.keys(data);
let values: any[] = json.values(data);

for (let key: string in keys) {
  console.log("Property: " + key);
}
```

### JSON Configuration Management

```typescript
// Load and merge configuration files
let defaultConfig: string = fs.readFile("default-config.json");
let userConfig: string = fs.readFile("user-config.json");

// Validate configurations
if (json.isValid(defaultConfig) && json.isValid(userConfig)) {
  let defaultObj: object = json.parse(defaultConfig);
  let userObj: object = json.parse(userConfig);

  // Merge configurations (user settings override defaults)
  let finalConfig: object = json.merge(defaultObj, userObj);

  // Extract specific settings
  let timeout: number = json.get(finalConfig, ".timeout");
  let retries: number = json.get(finalConfig, ".retries");
  let debugMode: boolean = json.get(finalConfig, ".debug");

  console.log(`Timeout: ${timeout}s, Retries: ${retries}, Debug: ${debugMode}`);

  // Save merged configuration
  let configString: string = json.stringify(finalConfig);
  fs.writeFile("merged-config.json", configString);
} else {
  console.log("Invalid configuration files detected");
  exit(1);
}
```

### Integration with Web APIs

```typescript
// Work with JSON API responses
let apiUrl: string = "https://api.github.com/users/octocat";
let response: string = web.get(apiUrl);

if (json.isValid(response)) {
  let userData: object = json.parse(response);

  let username: string = json.get(userData, ".login");
  let name: string = json.get(userData, ".name");
  let followers: number = json.get(userData, ".followers");
  let repos: number = json.get(userData, ".public_repos");

  console.log(`GitHub User: ${name} (@${username})`);
  console.log(`Followers: ${followers}, Public Repos: ${repos}`);

  // Create summary object
  let summary: object = json.parse("{}");
  summary = json.set(summary, ".username", username);
  summary = json.set(summary, ".followers", followers);
  summary = json.set(summary, ".repositories", repos);

  let summaryJson: string = json.stringify(summary);
  fs.writeFile("user-summary.json", summaryJson);
} else {
  console.log("Failed to retrieve valid JSON from API");
}
```

### JSON Dependency Management

Utah provides an automated dependency installation function that ensures all required tools for JSON processing are available:

```typescript
// Check and install JSON dependencies
console.log("Setting up JSON environment...");
let installResult: string = json.installDependencies();
console.log("Installation result: " + installResult);

// Verify dependencies are available
let jqAvailable: boolean = os.isInstalled("jq");
if (jqAvailable) {
  console.log("JSON processing tools are ready");

  // Now you can safely use JSON functions
  let config: string = '{"database": {"host": "localhost", "port": 5432}}';
  let configObj: object = json.parse(config);
  let dbHost: string = json.get(configObj, ".database.host");
  console.log(`Database host: ${dbHost}`);
} else {
  console.log("JSON tools installation failed");
  exit(1);
}

// Use in setup scripts
if (!os.isInstalled("jq")) {
  console.log("Installing JSON dependencies...");
  json.installDependencies();
  console.log("JSON environment ready");
}
```

### Generated Bash Code for JSON Functions

The JSON functions transpile to efficient bash commands using `jq`:

```bash
# json.parse() and json.isValid()
validJson='{"name": "John", "age": 30, "active": true}'
isValidData=$(echo ${validJson} | jq empty >/dev/null 2>&1 && echo "true" || echo "false")

if [ "${isValidData}" = "true" ]; then
  parsedData=$(echo ${validJson} | jq .)
  echo "JSON parsed successfully"
fi

# json.get() with jq-style paths
userName=$(echo "${parsedData}" | jq -r '.name')
userAge=$(echo "${parsedData}" | jq -r '.age')

# json.set() with different value types
updatedData=$(echo "${parsedData}" | jq '.name = "Jane"')
updatedData=$(echo "${updatedData}" | jq '.age = 25')
updatedData=$(echo "${updatedData}" | jq '.active = false')

# json.has() to check property existence
hasName=$(echo "${updatedData}" | jq 'try .name catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
hasEmail=$(echo "${updatedData}" | jq 'try .email catch false | type != "null"' | tr '[:upper:]' '[:lower:]')

# json.delete() to remove properties
dataWithoutAge=$(echo "${updatedData}" | jq 'del(.age)')

# json.keys() and json.values()
keys=$(echo "${updatedData}" | jq -r 'keys[]')
values=$(echo "${updatedData}" | jq -r '.[]')

# json.merge() two objects
defaultSettings='{"timeout": 30, "retries": 3}'
userSettings='{"timeout": 60, "debug": true}'
mergedSettings=$(echo "${defaultSettings}" | jq --argjson obj2 "${userSettings}" '. * $obj2')

# json.stringify() (already in JSON format from jq)
configString=$(echo "${mergedSettings}" | jq -c .)

# json.installDependencies() - automated dependency installation
installResult=$(
if ! command -v jq &> /dev/null; then
  echo "Installing jq for JSON processing..."
  if command -v apt-get &> /dev/null; then
    sudo apt-get update && sudo apt-get install -y jq
  elif command -v yum &> /dev/null; then
    sudo yum install -y jq
  elif command -v dnf &> /dev/null; then
    sudo dnf install -y jq
  elif command -v brew &> /dev/null; then
    brew install jq
  elif command -v pacman &> /dev/null; then
    sudo pacman -S --noconfirm jq
  else
    echo "Error: Unable to install jq. Please install it manually."
    exit 1
  fi
  if command -v jq &> /dev/null; then
    echo "jq installed successfully"
  else
    echo "Error: jq installation failed"
    exit 1
  fi
else
  echo "jq is already installed"
fi
)
```

### JSON Functions Features

- **jq-style paths**: Uses familiar jq path syntax like `.user.name` and `.items[0]`
- **Type preservation**: Maintains proper JSON types (strings, numbers, booleans, objects, arrays)
- **Nested access**: Supports deep object navigation with dot notation
- **Array support**: Works with JSON arrays using bracket notation
- **Error handling**: Integrates with Utah's try/catch system for robust error handling
- **Performance**: Leverages `jq` which is optimized for JSON processing
- **Cross-platform**: Uses `jq` which is available on most Unix-like systems

### JSON Path Examples

```typescript
// Basic property access
json.get(obj, ".name")              // Get top-level property
json.get(obj, ".user.name")         // Get nested property
json.get(obj, ".user.settings.theme") // Get deeply nested property

// Array access
json.get(obj, ".items[0]")          // Get first array element
json.get(obj, ".users[2].name")     // Get property of array element
json.get(obj, ".tags[-1]")          // Get last array element

// Complex paths
json.get(obj, ".config.servers[0].host")  // Nested array access
json.get(obj, ".metadata.created")        // ISO date strings
```

### JSON Functions Use Cases

- **Configuration Management**: Load, validate, and merge JSON configuration files
- **API Integration**: Parse JSON responses from web APIs and extract specific data
- **Data Processing**: Transform and manipulate JSON data structures
- **Settings Management**: Update application settings stored as JSON
- **Log Processing**: Extract information from JSON-formatted log files
- **Microservices**: Handle JSON communication between services
- **DevOps Automation**: Process JSON output from various tools and APIs

### Best Practices for JSON Functions

- **Always validate**: Use `json.isValid()` before parsing untrusted JSON
- **Handle errors**: Wrap JSON operations in try/catch blocks for robust error handling
- **Use specific paths**: Be explicit with jq paths to avoid ambiguity
- **Type awareness**: Remember that all values from `json.get()` are strings; cast as needed
- **Performance**: For repeated operations on the same JSON, parse once and reuse the object

### JSON Functions Technical Notes

- JSON functions require `jq` to be installed on the system
- All JSON objects are stored as bash variables containing the parsed JSON
- Property values are extracted as strings; type conversion happens at the Utah level
- Large JSON objects may impact performance; consider processing in chunks
- The `json.merge()` function performs a deep merge with the second object taking precedence

## 🧩 YAML Functions

Utah provides comprehensive YAML parsing and manipulation functions that mirror the JSON functions but work with YAML data. These functions use `yq` and `jq` under the hood to provide powerful and efficient YAML operations with jq-style path syntax.

### Available YAML Functions

#### YAML Parsing and Validation

- `yaml.parse(yamlString)` - Parse a YAML string into an object that can be manipulated
- `yaml.stringify(yamlObject)` - Convert a YAML object back to a YAML string representation
- `yaml.isValid(yamlString)` - Check if a string contains valid YAML syntax

#### YAML Property Access and Manipulation

- `yaml.get(yamlObject, path)` - Get a value from YAML using jq-style path (e.g., ".user.name")
- `yaml.set(yamlObject, path, value)` - Set a value in YAML object using jq-style path
- `yaml.has(yamlObject, path)` - Check if a property exists at the given path
- `yaml.delete(yamlObject, path)` - Remove a property from YAML object

#### YAML Utility Functions

- `yaml.keys(yamlObject)` - Get all keys from a YAML object as an array
- `yaml.values(yamlObject)` - Get all values from a YAML object as an array
- `yaml.merge(yamlObject1, yamlObject2)` - Merge two YAML objects (yamlObject2 overwrites yamlObject1)
- `yaml.installDependencies()` - Check and install required YAML processing tools (yq and jq)

### YAML Functions Usage

```typescript
// Basic YAML parsing and validation
let configYaml: string = 'debug: true\nport: 8080\nname: MyApp';
let isValid: boolean = yaml.isValid(configYaml);

if (isValid) {
  let config: object = yaml.parse(configYaml);
  console.log("Configuration loaded successfully");

  // Access properties using jq-style paths
  let appName: string = yaml.get(config, ".name");
  let port: number = yaml.get(config, ".port");
  let debugMode: boolean = yaml.get(config, ".debug");

  console.log(`Starting ${appName} on port ${port}`);

  if (debugMode) {
    console.log("Debug mode is enabled");
  }
} else {
  console.log("Invalid YAML configuration");
  exit(1);
}

// Working with nested YAML objects
let userData: string = 'user:\n  name: Jane\n  settings:\n    theme: dark\n    notifications: true';
let userObj: object = yaml.parse(userData);

// Access nested properties
let userName: string = yaml.get(userObj, ".user.name");
let theme: string = yaml.get(userObj, ".user.settings.theme");
let notifications: boolean = yaml.get(userObj, ".user.settings.notifications");

console.log(`User: ${userName}, Theme: ${theme}, Notifications: ${notifications}`);

// Modify YAML properties
let updatedUser: object = yaml.set(userObj, ".user.name", "John");
updatedUser = yaml.set(updatedUser, ".user.settings.theme", "light");
updatedUser = yaml.set(updatedUser, ".user.settings.newFeature", true);

// Check if properties exist
let hasEmail: boolean = yaml.has(updatedUser, ".user.email");
let hasTheme: boolean = yaml.has(updatedUser, ".user.settings.theme");

console.log(`Has email: ${hasEmail}, Has theme: ${hasTheme}`);

// Remove properties
let userWithoutTheme: object = yaml.delete(updatedUser, ".user.settings.theme");
console.log("Theme setting removed");

// Convert back to YAML string
let finalUserYaml: string = yaml.stringify(userWithoutTheme);
console.log("Updated user data: " + finalUserYaml);
```

### Working with YAML Configuration Files

```typescript
// Load and merge YAML configuration files
let defaultConfig: string = fs.readFile("default-config.yaml");
let userConfig: string = fs.readFile("user-config.yaml");

// Validate configurations
if (yaml.isValid(defaultConfig) && yaml.isValid(userConfig)) {
  let defaultObj: object = yaml.parse(defaultConfig);
  let userObj: object = yaml.parse(userConfig);

  // Merge configurations (user settings override defaults)
  let finalConfig: object = yaml.merge(defaultObj, userObj);

  // Extract specific settings
  let timeout: number = yaml.get(finalConfig, ".timeout");
  let retries: number = yaml.get(finalConfig, ".retries");
  let debugMode: boolean = yaml.get(finalConfig, ".debug");

  console.log(`Timeout: ${timeout}s, Retries: ${retries}, Debug: ${debugMode}`);

  // Save merged configuration
  let configString: string = yaml.stringify(finalConfig);
  fs.writeFile("merged-config.yaml", configString);
} else {
  console.log("Invalid YAML configuration files detected");
  exit(1);
}
```

### Kubernetes and DevOps Integration

```typescript
// Work with Kubernetes YAML manifests
let deploymentYaml: string = fs.readFile("deployment.yaml");

if (yaml.isValid(deploymentYaml)) {
  let deployment: object = yaml.parse(deploymentYaml);

  // Update container image
  let newImage: string = "myapp:v2.0.0";
  let updatedDeployment: object = yaml.set(deployment, ".spec.template.spec.containers[0].image", newImage);

  // Update replicas
  updatedDeployment = yaml.set(updatedDeployment, ".spec.replicas", 3);

  // Add environment variable
  updatedDeployment = yaml.set(updatedDeployment, ".spec.template.spec.containers[0].env[0].name", "ENVIRONMENT");
  updatedDeployment = yaml.set(updatedDeployment, ".spec.template.spec.containers[0].env[0].value", "production");

  // Save updated manifest
  let updatedYaml: string = yaml.stringify(updatedDeployment);
  fs.writeFile("deployment-updated.yaml", updatedYaml);

  console.log("Kubernetes deployment updated successfully");
} else {
  console.log("Invalid Kubernetes YAML manifest");
  exit(1);
}
```

### YAML Dependency Management

Utah provides an automated dependency installation function that ensures all required tools for YAML processing are available:

```typescript
// Check and install YAML dependencies
console.log("Setting up YAML environment...");
let installResult: string = yaml.installDependencies();
console.log("Installation result: " + installResult);

// Verify dependencies are available
let yqAvailable: boolean = os.isInstalled("yq");
let jqAvailable: boolean = os.isInstalled("jq");

if (yqAvailable && jqAvailable) {
  console.log("YAML processing tools are ready");

  // Now you can safely use YAML functions
  let config: string = 'database:\n  host: localhost\n  port: 5432\n  ssl: true';
  let configObj: object = yaml.parse(config);
  let dbHost: string = yaml.get(configObj, ".database.host");
  let sslEnabled: boolean = yaml.get(configObj, ".database.ssl");
  console.log(`Database host: ${dbHost}, SSL: ${sslEnabled}`);
} else {
  console.log("YAML tools installation failed");
  exit(1);
}

// Use in DevOps setup scripts
if (!os.isInstalled("yq") || !os.isInstalled("jq")) {
  console.log("Installing YAML dependencies...");
  yaml.installDependencies();
  console.log("YAML environment ready for Kubernetes and CI/CD operations");
}
```

### Generated Bash Code for YAML Functions

The YAML functions transpile to efficient bash commands using `yq` and `jq`:

```bash
# yaml.parse() and yaml.isValid()
validYaml='name: John\nage: 30\nactive: true'
isValidData=$(echo ${validYaml} | yq empty >/dev/null 2>&1 && echo "true" || echo "false")

if [ "${isValidData}" = "true" ]; then
  parsedData=$(echo ${validYaml} | yq -o=json .)
  echo "YAML parsed successfully"
fi

# yaml.get() with jq-style paths (converts to JSON internally)
userName=$(echo "${parsedData}" | yq -o=json . | jq -r '.name')
userAge=$(echo "${parsedData}" | yq -o=json . | jq -r '.age')

# yaml.set() with different value types (converts through JSON)
updatedData=$(echo "${parsedData}" | yq -o=json . | jq '.name = "Jane"' | yq -o=yaml .)
updatedData=$(echo "${updatedData}" | yq -o=json . | jq '.age = 25' | yq -o=yaml .)

# yaml.has() to check property existence
hasName=$(echo "${updatedData}" | yq -o=json . | jq 'try .name catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
hasEmail=$(echo "${updatedData}" | yq -o=json . | jq 'try .email catch false | type != "null"' | tr '[:upper:]' '[:lower:]')

# yaml.delete() to remove properties
dataWithoutAge=$(echo "${updatedData}" | yq -o=json . | jq 'del(.age)' | yq -o=yaml .)

# yaml.keys() and yaml.values()
keys=$(echo "${updatedData}" | yq -o=json . | jq -r 'keys[]')
values=$(echo "${updatedData}" | yq -o=json . | jq -r '.[]')

# yaml.merge() two objects
defaultSettings='timeout: 30\nretries: 3'
userSettings='timeout: 60\ndebug: true'
mergedSettings=$(echo "${defaultSettings}" | yq -o=json . | jq --argjson obj2 "$(echo \"${userSettings}\" | yq -o=json .)" '. * $obj2' | yq -o=yaml .)

# yaml.stringify() (already in YAML format from yq)
configString=$(echo "${mergedSettings}" | yq -o=yaml .)

# yaml.installDependencies() - automated dependency installation
installResult=$(
if ! command -v yq &> /dev/null; then
  echo "Installing yq for YAML processing..."
  if command -v snap &> /dev/null; then
    sudo snap install yq
  elif command -v brew &> /dev/null; then
    brew install yq
  elif command -v wget &> /dev/null; then
    sudo wget -qO /usr/local/bin/yq https://github.com/mikefarah/yq/releases/latest/download/yq_linux_amd64
    sudo chmod +x /usr/local/bin/yq
  else
    echo "Error: Unable to install yq. Please install it manually."
    exit 1
  fi
  if command -v yq &> /dev/null; then
    echo "yq installed successfully"
  else
    echo "Error: yq installation failed"
    exit 1
  fi
else
  echo "yq is already installed"
fi
if ! command -v jq &> /dev/null; then
  echo "Installing jq for JSON processing (required by YAML functions)..."
  if command -v apt-get &> /dev/null; then
    sudo apt-get update && sudo apt-get install -y jq
  elif command -v yum &> /dev/null; then
    sudo yum install -y jq
  elif command -v dnf &> /dev/null; then
    sudo dnf install -y jq
  elif command -v brew &> /dev/null; then
    brew install jq
  elif command -v pacman &> /dev/null; then
    sudo pacman -S --noconfirm jq
  else
    echo "Error: Unable to install jq. Please install it manually."
    exit 1
  fi
  if command -v jq &> /dev/null; then
    echo "jq installed successfully"
  else
    echo "Error: jq installation failed"
    exit 1
  fi
else
  echo "jq is already installed"
fi
)
```

### YAML Functions Features

- **yq-style processing**: Uses `yq` for YAML parsing and `jq` for manipulation
- **jq-style paths**: Same familiar jq path syntax as JSON functions
- **Type preservation**: Maintains proper YAML types (strings, numbers, booleans, objects, arrays)
- **Nested access**: Supports deep object navigation with dot notation
- **Array support**: Works with YAML arrays using bracket notation
- **Error handling**: Integrates with Utah's try/catch system for robust error handling
- **DevOps friendly**: Perfect for Kubernetes, Docker Compose, and CI/CD configurations
- **Cross-platform**: Uses `yq` which is available on most Unix-like systems

### YAML Functions Use Cases

- **Configuration Management**: Load, validate, and merge YAML configuration files
- **Kubernetes Operations**: Manipulate YAML manifests for deployments, services, and configs
- **CI/CD Pipelines**: Process YAML pipeline configurations and workflows
- **Docker Compose**: Update service definitions and environment variables
- **Infrastructure as Code**: Modify YAML-based infrastructure definitions
- **DevOps Automation**: Process YAML output from various DevOps tools
- **OpenAPI Specifications**: Work with OpenAPI/Swagger YAML definitions

### Best Practices for YAML Functions

- **Always validate**: Use `yaml.isValid()` before parsing untrusted YAML
- **Handle errors**: Wrap YAML operations in try/catch blocks for robust error handling
- **Use specific paths**: Be explicit with jq paths to avoid ambiguity
- **Type awareness**: Remember that all values from `yaml.get()` are strings; cast as needed
- **Performance**: For repeated operations on the same YAML, parse once and reuse the object
- **Kubernetes compatibility**: Test YAML changes with `kubectl dry-run` when working with K8s manifests

### YAML Functions Technical Notes

- YAML functions require both `yq` and `jq` to be installed on the system
- YAML is internally converted to JSON for manipulation, then back to YAML for output
- All YAML objects are stored as bash variables containing the parsed data
- Property values are extracted as strings; type conversion happens at the Utah level
- Large YAML files may impact performance; consider processing in chunks
- The `yaml.merge()` function performs a deep merge with the second object taking precedence
- Complex YAML features like anchors and aliases are preserved during simple operations

## 🛠️ Git Utilities

Utah provides git utilities for common version control operations. These functions allow you to perform git operations directly within your scripts, making it easy to automate version control workflows.

### Available Git Functions

#### Version Control Operations

- `git.undoLastCommit()` - Undo the last commit while preserving changes in the staging area

### Git Utilities Usage

```typescript
// Undo the last commit (safe - keeps changes staged)
git.undoLastCommit();
console.log("Last commit has been undone, changes preserved in staging area");

// Use in conditional logic
let hasChanges: boolean = true; // This would typically come from checking git status
if (hasChanges) {
  console.log("Undoing last commit to make corrections...");
  git.undoLastCommit();
  console.log("Last commit undone. Make your changes and commit again.");
}

// Use in error recovery scenarios
console.log("Checking if last commit needs to be fixed...");
// Assume some validation logic here
let commitNeedsFixing: boolean = true;

if (commitNeedsFixing) {
  console.log("Fixing last commit...");
  git.undoLastCommit();
  // Make corrections here
  console.log("Ready to make corrected commit");
}
```

### Practical Git Workflow Examples

```typescript
// Complete workflow: undo, fix, and recommit
console.log("Starting commit correction workflow...");

// Undo the last commit
git.undoLastCommit();
console.log("Last commit undone - changes are staged");

// Show current status (in a real script, you might check git status)
console.log("Files are still staged and ready for a new commit");

// Example: you would make your changes here, then:
console.log("After making corrections, commit again with:");
console.log("git commit -m 'Corrected commit message'");

// Emergency rollback scenario
function emergencyRollback(): void {
  console.log("EMERGENCY: Rolling back last commit");
  git.undoLastCommit();
  console.log("Last commit rolled back. Review changes before recommitting.");
}

// Call rollback if needed
let needsRollback: boolean = false; // This would be determined by your logic
if (needsRollback) {
  emergencyRollback();
}
```

### Integration with Other Utah Functions

```typescript
// Combine with OS utilities and user prompts
let gitInstalled: boolean = os.isInstalled("git");

if (!gitInstalled) {
  console.log("Error: Git is not installed");
  exit(1);
}

console.log("Git is available - proceeding with commit operations");

// Confirm before undoing commit
let shouldUndo: boolean = console.promptYesNo("Undo the last commit?");

if (shouldUndo) {
  console.log("Undoing last commit...");
  git.undoLastCommit();
  console.log("Last commit undone successfully");
} else {
  console.log("Keeping last commit as is");
}

// Use with error handling
try {
  console.log("Attempting to undo last commit...");
  git.undoLastCommit();
  console.log("Commit undone successfully");
}
catch {
  console.log("Failed to undo commit - perhaps no commits exist");
}
```

### Generated Bash Code for Git Utilities

The git utilities transpile to standard git commands:

```bash
# git.undoLastCommit() becomes:
git reset --soft HEAD~1

# Complete example with error handling:
gitInstalled=$(command -v git &> /dev/null && echo "true" || echo "false")

if [ "${gitInstalled}" != "true" ]; then
  echo "Error: Git is not installed"
  exit 1
fi

echo "Git is available - proceeding with commit operations"

shouldUndo=$(while true; do read -p "Undo the last commit? (y/n): " yn; case $yn in [Yy]* ) echo "true"; break;; [Nn]* ) echo "false"; break;; * ) echo "Please answer yes or no.";; esac; done)

if [ "${shouldUndo}" = "true" ]; then
  echo "Undoing last commit..."
  git reset --soft HEAD~1
  echo "Last commit undone successfully"
else
  echo "Keeping last commit as is"
fi

# With try/catch error handling:
_utah_try_block_1() {
  (
    set -e
    echo "Attempting to undo last commit..."
    git reset --soft HEAD~1 || exit 1
    echo "Commit undone successfully"
  )
}

utah_catch_1() {
  echo "Failed to undo commit - perhaps no commits exist"
}

_utah_try_block_1 || utah_catch_1
```

### What git.undoLastCommit() Does

The `git.undoLastCommit()` function performs a **soft reset** to the previous commit:

- **Undoes the last commit**: Removes the most recent commit from the git history
- **Preserves all changes**: All file modifications remain in the staging area
- **Safe operation**: You don't lose any work - files stay staged and ready to commit
- **Correctable**: Perfect for fixing commit messages or adding forgotten files

### Git Reset Types Comparison

Utah's `git.undoLastCommit()` uses `git reset --soft HEAD~1`, which is the safest option:

| Reset Type | Command | Commit History | Staging Area | Working Directory |
|------------|---------|----------------|--------------|-------------------|
| **Soft** (Utah) | `git reset --soft HEAD~1` | ✅ Undone | ✅ Preserved | ✅ Preserved |
| Mixed | `git reset HEAD~1` | ✅ Undone | ❌ Cleared | ✅ Preserved |
| Hard | `git reset --hard HEAD~1` | ✅ Undone | ❌ Cleared | ❌ Cleared |

### Use Cases for git.undoLastCommit()

- **Fix Commit Messages**: Undo commit, then recommit with correct message
- **Add Forgotten Files**: Undo commit, stage additional files, then recommit
- **Split Large Commits**: Undo large commit, then make smaller, focused commits
- **Correct Mistakes**: Undo accidental commits before pushing to remote
- **Reorder Changes**: Undo commits to reorganize change history
- **Emergency Rollback**: Quick rollback during development workflows

### Script Control Best Practices

- **Use before pushing**: Only undo commits that haven't been pushed to remote repositories
- **Check staging area**: After undoing, verify what's staged with `git status`
- **Commit again**: Remember to make a new commit after making corrections
- **Combine with validation**: Use with other Utah functions to validate before undoing
- **Document reasons**: Add comments explaining why commits are being undone

### Safety Notes

- **Local operation only**: This affects only your local repository
- **No data loss**: All file changes are preserved in the staging area
- **Reversible**: You can immediately recommit if you change your mind
- **Safe for collaboration**: Won't affect other developers until you push
- **Git history**: Only removes the commit, doesn't delete file contents

## 🎛️ Script Control Functions

Utah provides script control functions for managing shell behavior and debugging options during script execution. These functions compile to standard shell `set` commands and allow you to control various aspects of script execution.

### Available Script Control Functions

#### Script Metadata

- `script.description("description")` - Set a description for the script (stored in __UTAH_SCRIPT_DESCRIPTION variable)

#### Debug Control

- `script.enableDebug()` - Enable shell debugging and command tracing (set -x)
- `script.disableDebug()` - Disable shell debugging and command tracing (set +x)

#### Globbing Control

- `script.disableGlobbing()` - Disable filename globbing/expansion (set -f)
- `script.enableGlobbing()` - Enable filename globbing/expansion (set +f)

#### Error Handling Control

- `script.exitOnError()` - Exit script immediately when any command fails (set -e)
- `script.continueOnError()` - Continue script execution even when commands fail (set +e)

### Script Control Functions Usage

```typescript
// Set a description for the script
script.description("Application Health Check Script - Monitors system resources and application status");
console.log("Script description has been set");

// Enable debug mode to trace command execution
script.enableDebug();
console.log("Debug mode is now enabled");

// Disable globbing for literal file pattern matching
script.disableGlobbing();
let pattern: string = "*.txt";
console.log(`Looking for files matching: ${pattern}`);

// Enable strict error handling
script.exitOnError();
console.log("Script will exit on any command failure");

// Disable debug mode
script.disableDebug();
console.log("Debug mode is now disabled");

// Re-enable globbing
script.enableGlobbing();
console.log("Globbing is now enabled");

// Allow script to continue on errors
script.continueOnError();
console.log("Script will continue even if commands fail");
```

### Debug Control Example

```typescript
// Start with debug mode for troubleshooting
script.enableDebug();
console.log("Starting application setup...");

// Perform some operations with debug output
let appName: string = "MyApp";
console.log(`Setting up ${appName}`);

// Disable debug mode for cleaner output
script.disableDebug();
console.log("Setup complete - debug mode disabled");
```

### Error Handling Example

```typescript
// Enable strict error handling for critical operations
script.exitOnError();
console.log("Starting critical deployment process...");

// These commands will cause script to exit if they fail
console.log("Validating configuration...");
console.log("Deploying application...");

// Switch to permissive mode for cleanup operations
script.continueOnError();
console.log("Performing cleanup (errors allowed)...");
```

### Globbing Control Example

```typescript
// Disable globbing when working with literal patterns
script.disableGlobbing();
let searchPattern: string = "*.backup";
console.log(`Searching for files with pattern: ${searchPattern}`);

// Process files without shell expansion
console.log("Processing files literally...");

// Re-enable globbing for normal file operations
script.enableGlobbing();
console.log("Globbing re-enabled for file operations");
```

### Script Metadata Example

```typescript
// Document what your script does
script.description("Server Health Check - Monitors CPU, memory, disk space, and running services");
console.log("Health check script initialized");

// The description is stored in a variable and can be used by other tools
// or displayed in help messages
let appName: string = "HealthChecker";
console.log(`${appName} v1.0 - See __UTAH_SCRIPT_DESCRIPTION for details`);
```

### Generated Bash Code for Script Control Functions

The script control functions transpile to standard bash `set` commands:

```bash
# script.description("description") becomes:
__UTAH_SCRIPT_DESCRIPTION="Application Health Check Script - Monitors system resources and application status"
echo "Script description has been set"

# script.enableDebug() becomes:
set -x
echo "Debug mode is now enabled"

# script.disableGlobbing() becomes:
set -f
pattern="*.txt"
echo "Looking for files matching: ${pattern}"

# script.exitOnError() becomes:
set -e
echo "Script will exit on any command failure"

# script.disableDebug() becomes:
set +x
echo "Debug mode is now disabled"

# script.enableGlobbing() becomes:
set +f
echo "Globbing is now enabled"

# script.continueOnError() becomes:
set +e
echo "Script will continue even if commands fail"
```

### Script Control Functions Use Cases

- **Script Documentation**: Use `script.description()` to document the purpose and functionality of your scripts
- **Debugging**: Enable/disable command tracing to troubleshoot script issues
- **Error Handling**: Control whether scripts exit on command failures
- **File Operations**: Control glob expansion for literal vs. pattern matching
- **Development**: Use debug mode during development, disable for production
- **Conditional Control**: Enable strict error handling for critical sections
- **Legacy Compatibility**: Manage shell behavior for compatibility with different systems

### Best Practices

- **Document your scripts**: Use `script.description()` at the beginning of scripts to explain their purpose
- **Use debug mode sparingly**: Enable only when needed to avoid verbose output
- **Exit on error for critical operations**: Use `script.exitOnError()` for deployment scripts
- **Disable globbing for literal patterns**: Prevent unexpected expansion when working with patterns as strings
- **Document behavior changes**: Comment when changing script control settings
- **Reset after use**: Consider resetting options after specific operations
- **Test both modes**: Test scripts with both strict and permissive error handling

### Shell Option Reference

| Utah Function | Bash Command | Description |
|---------------|--------------|-------------|
| `script.description("desc")` | `__UTAH_SCRIPT_DESCRIPTION="desc"` | Set a description variable for the script |
| `script.enableDebug()` | `set -x` | Print commands and their arguments as they are executed |
| `script.disableDebug()` | `set +x` | Disable command tracing |
| `script.disableGlobbing()` | `set -f` | Disable filename expansion (globbing) |
| `script.enableGlobbing()` | `set +f` | Enable filename expansion (globbing) |
| `script.exitOnError()` | `set -e` | Exit immediately if a command exits with a non-zero status |
| `script.continueOnError()` | `set +e` | Do not exit on command failures (default behavior) |

### Script Control Technical Notes

- Script control functions can be called multiple times throughout a script
- Settings persist until explicitly changed or the script terminates
- Functions can be used in conditional blocks and loops
- All functions compile to standard POSIX shell `set` commands
- No performance impact - these are shell built-in commands

## CLI Commands

### Running Utah Code

Execute Utah (.shx) files or commands directly:

```bash
# Direct execution (recommended)
utah script.shx
utah -c "console.log('Hello, World!')"
utah --command "json.installDependencies()"

# Or using explicit 'run' command
utah run script.shx
utah run -c "console.log('Hello, World!')"
utah run --command "json.installDependencies()"

# More inline command examples
utah -c "os.isInstalled('git')"
utah --command "fs.exists('/path/to/file')"
```

### Compiling

Compile .shx files to bash scripts:

```bash
# Compile to .sh file
utah compile script.shx

# Compile with custom output
utah compile script.shx -o custom-name.sh
utah compile script.shx --output custom-name.sh
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

### Formatting

Utah includes a built-in code formatter that respects EditorConfig settings:

```bash
# Format a file and save to .formatted.shx
utah format script.shx

# Format with custom output path
utah format script.shx -o formatted_script.shx
utah format script.shx --output formatted_script.shx

# Format in place (overwrite original)
utah format script.shx --in-place

# Check if file is properly formatted (exit 1 if not)
utah format script.shx --check

# Format all .shx files recursively from current directory
utah format

# Format all .shx files recursively in place
utah format --in-place

# Check all .shx files recursively are properly formatted
utah format --check
```

**Recursive Formatting**: When no file is specified, Utah will automatically find and format all `.shx` files recursively starting from the current directory. This is particularly useful for formatting entire projects:

- `utah format` - Creates `.formatted.shx` files for all unformatted files
- `utah format --in-place` - Formats all files in place
- `utah format --check` - Checks all files and exits with code 1 if any need formatting

**Note**: The `-o`/`--output` option is not supported when formatting multiple files recursively.

The formatter honors these EditorConfig properties:

- `indent_style` (space/tab)
- `indent_size` (number)
- `end_of_line` (lf/crlf/cr)
- `insert_final_newline` (true/false)
- `trim_trailing_whitespace` (true/false)
- `charset` (utf-8, etc.)

Utah-specific EditorConfig properties:

- `utah_brace_style` (same_line/new_line)
- `utah_space_before_paren` (true/false) - `if(` vs `if (`
- `utah_max_line_length` (number) - for future line wrapping

### Testing

Utah includes a comprehensive regression test suite to ensure that changes don't break existing functionality.

#### Test Structure

Tests are organized in the `tests/` directory:

```text
tests/
├── positive_fixtures/ # Test input files (.shx)
├── negative_fixtures/ # Test input files (.shx)
├── malformatted/      # Malformed test input files (.shx)
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
make help                 # Show available commands
make build                # Build the CLI
make test                 # Run all tests
make compile FILE=<file>  # Compile a specific file
make clean                # Clean build artifacts
make dev                  # Full development cycle (build + test)
make info                 # Show project information
```

Use `make help` to see all available targets with descriptions.

### Running Tests

Run all tests with detailed output:

```bash
make test              # Full test output
```

### Test Coverage

Current tests cover:

- **advanced_math.shx** - Advanced mathematical operations and calculations
- **args_functionality.shx** - Command-line argument parsing and validation
- **array_contains.shx** - Array contains() method for checking element presence
- **array_join.shx** - Array join() method for converting arrays to strings
- **array_sort.shx** - Array sort() method for sorting arrays
- **arrays.shx** - Array literals, access, length, and iteration
- **arrays_isempty.shx** - Array isEmpty() method for checking empty arrays
- **arrays_reverse.shx** - Array reverse() method for reversing array order
- **console_clear.shx** - Console clear() function for clearing terminal screen
- **console_isinteractive.shx** - Console interactive terminal detection for adaptive script behavior
- **console_issudo.shx** - Console system functions and privilege checking
- **console_prompt_yesno.shx** - User interaction with yes/no prompts
- **console_promptdirectory.shx** - Console directory selection dialog with default path
- **console_promptfile.shx** - Console file selection dialog with filter support
- **console_promptnumber.shx** - Console numeric input with validation and range checking
- **console_promptpassword.shx** - Console password input with hidden text
- **console_prompttext.shx** - Console text input dialog with default values
- **console_showchoice.shx** - Console single choice menu with options and default selection
- **console_showconfirm.shx** - Console yes/no confirmation dialog with default button
- **console_showerror.shx** - Console error message dialog display
- **console_showinfo.shx** - Console informational message dialog display
- **console_showmessage.shx** - Console general message dialog display
- **console_showmultichoice.shx** - Console multi-choice checklist with default selections
- **console_showprogress.shx** - Console progress bar display with percentage and cancel option
- **console_showsuccess.shx** - Console success message dialog display
- **console_showwarning.shx** - Console warning message dialog display
- **const_valid_assignment.shx** - Const variable declarations and immutability
- **defer_basic.shx** - Basic defer statement functionality with cleanup execution
- **defer_early_return.shx** - Defer statement execution with early function returns
- **defer_multiple.shx** - Multiple defer statements with LIFO execution order
- **defer_resources.shx** - Resource management and cleanup using defer statements
- **env_get_ternary.shx** - Environment variable operations with ternary operators
- **environment_variables.shx** - Environment variable operations
- **for_in_loop.shx** - For-in loops with arrays
- **fs_copy.shx** - File system copy operations with automatic directory creation and recursive directory support
- **fs_delete.shx** - File system delete operations with recursive directory deletion and boolean return values
- **fs_dirname.shx** - File system dirname function
- **fs_exists.shx** - File system file/directory existence checking
- **fs_extension.shx** - File system extension extraction
- **fs_filename.shx** - File system filename extraction
- **fs_functions.shx** - File system read/write operations
- **fs_move.shx** - File system move/rename operations with automatic directory creation
- **fs_parentdirname.shx** - File system parent directory name extraction
- **fs_path.shx** - File system path manipulation
- **fs_rename.shx** - File system rename operations within the same location
- **function_declarations.shx** - Function definitions with typed parameters and return values
- **git_undo_last_commit.shx** - Git undo last commit functionality
- **if_elseif_else.shx** - If-else-if-else conditional statements with proper chaining
- **json_install_dependencies.shx** - JSON dependency installation and verification
- **json_manipulation.shx** - JSON property manipulation and modification functions
- **json_parse_validation.shx** - JSON parsing and validation functions
- **json_property_access.shx** - JSON property access and existence checking
- **json_utilities.shx** - JSON utility functions (keys, values, merge)
- **let_reassignment_test.shx** - Variable reassignment and mutability
- **math.shx** - Basic mathematical operations and functions
- **mixed_loops.shx** - Mixed loop types in one file
- **mixed_syntax.shx** - Mixed Utah and bash syntax compatibility
- **nested_for_loop.shx** - Nested for loops
- **nested_import.shx** - Nested import functionality with import chains
- **os_getlinuxversion.shx** - Operating system Linux version detection
- **os_getos.shx** - Operating system detection
- **os_isinstalled.shx** - Command/application installation checking
- **parallel_function_call.shx** - Parallel function calls
- **process_info.shx** - Process information functions (ID, CPU, memory, etc.)
- **scheduler_cron.shx** - Cron job scheduling and management
- **script_continueonerror.shx** - Script control function for continuing on errors
- **script_description.shx** - Script description metadata function
- **script_disabledebug.shx** - Script control function for disabling debug mode
- **script_disableglobbing.shx** - Script control function for disabling globbing
- **script_enabledebug.shx** - Script control function for enabling debug mode
- **script_enableglobbing.shx** - Script control function for enabling globbing
- **script_exitonerror.shx** - Script control function for exiting on errors
- **simple_console.shx** - Basic console.log functionality
- **simple_for_loop.shx** - Traditional for loops
- **simple_import.shx** - Basic import functionality with multiple files
- **simple_switch.shx** - Basic switch/case/default statements
- **string_functions.shx** - String manipulation functions
- **switch_case.shx** - Complex switch statements with fall-through cases
- **template_update.shx** - Template file processing and environment variable substitution
- **template_with_vars.shx** - Template processing with variable replacement and dynamic content
- **ternary_operators.shx** - Ternary conditional operators
- **timer_start_stop.shx** - Timer functions for performance measurement
- **try_catch.shx** - Try/catch blocks for error handling and graceful failure recovery
- **utility_functions.shx** - Utility functions for UUID generation, hashing, and Base64 encoding/decoding
- **utility_random.shx** - Utility random number generation with range parameters
- **utils.shx** - General utility functions and helpers
- **variable_declaration.shx** - Variable declarations and usage
- **web_get.shx** - Web HTTP GET requests and API communication
- **while_loop.shx** - While loops with break statements and conditional logic
- **yaml_install_dependencies.shx** - YAML dependency installation and verification
- **yaml_manipulation.shx** - YAML property manipulation and modification functions
- **yaml_parse_validation.shx** - YAML parsing and validation functions
- **yaml_property_access.shx** - YAML property access and existence checking
- **yaml_utilities.shx** - YAML utility functions (keys, values, merge)

### Negative Test Coverage

The negative test fixtures ensure that the compiler correctly handles and reports errors for invalid code:

- **array_type_mismatch.shx** - Boolean array with mixed types (string in boolean array)
- **const_reassignment_test.shx** - Attempt to reassign a const variable after declaration
- **defer_outside_function.shx** - Defer statement used outside function scope (should fail)
- **import_missing_file.shx** - Import statement referencing a non-existent file

### Malformed Test Coverage

The malformed test fixtures ensure that the formatter correctly handles and formats malformed code:

- **arrays_and_expressions.shx** - Malformed array and expression syntax for formatter testing
- **basic_formatting.shx** - Basic formatting issues like spacing and indentation
- **builtin_functions.shx** - Malformed built-in function calls and syntax
- **control_structures.shx** - Malformed control flow structures (if/else, loops, switch)
- **scheduler_formatting.shx** - Malformed scheduler and cron-related syntax

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

- [x] `console.log()` for output

- [x] Console system functions (`console.isSudo()`, `console.isInteractive()`, `console.getShell()`)

- [x] String interpolation with double quotes (`"Hello, ${name}"`)

- [x] Function calls with positional arguments

- [x] Exit script with `exit(0)` or `exit(1)`

- [x] Support for `const` declarations

- [x] Support for `for` and `for in` loops

- [x] Support for `switch/case/default` statements

- [x] Support for `while` loops and `break` statements

- [x] Arrays and basic data structures (`string[]`, `number[]`, `boolean[]`)

- [x] Comment support (`//`)

- [x] import syntax for splitting .shx files

- [x] `array.length` property

- [x] `array.isEmpty()` function

- [x] `array.reverse()` function

- [x] `array.contains()` function

- [x] `array.join()` function

- [x] `array.sort()` function

- [x] `array.merge()` function

- [x] `array.shuffle()` function

- [x] `array.random()` function

- [ ] `array.unique()` function to remove duplicate elements

- [ ] `array.forEach()` function to iterate over elements

- [ ] Multiline string support with triple-double quotes (`"""`)

- [x] Add shebang (`#!/bin/bash`) to generated scripts

- [x] `utility.random()` function

- [x] `utility.uuid()` function

- [x] `utility.hash()` function

- [x] `utility.base64Encode()` function

- [x] `utility.base64Decode()` function

- [x] Throw error if `min` is greater than `max` in `utility.random(<min>, <max>)` function

- [x] `script.*` functions (`description()`, `enableDebug()`, `disableDebug()`, `disableGlobbing()`, `enableGlobbing()`, `exitOnError()`, `continueOnError()`)

- [x] `args.*` functions (`get()`, `has()`, `all()`, `define()`, `showHelp()` implemented)

- [x] `web.get()` function

- [ ] `web.post()` function

- [ ] `web.put()` function

- [ ] `web.delete()` function

- [ ] `web.speedTest()` function

- [x] `console.*` functions

- [ ] `ssh.connect()` function with SSH key support

- [ ] `ssh.execute()` function with command output handling

- [ ] `ssh.upload()` function for file transfers

- [ ] `ssh.download()` function for file transfers

- [ ] `ssh.disconnect()` function to close SSH connections

- [x] `git.undoLastCommit()` function for undoing the last commit

- [ ] `git.mergePR()` function for merging pull requests

- [ ] `git.forcePush()` function for force pushing changes

- [ ] `validate.isEmail()` function for email validation

- [ ] `validate.isURL()` function for URL validation

- [ ] `validate.isUUID()` function for UUID validation

- [ ] `validate.isPhoneNumber()` function for phone number validation

- [ ] `validate.isNull()` function for null checks

- [ ] `validate.isGreaterThan()` function for numeric comparisons

- [ ] `validate.isLessThan()` function for numeric comparisons

- [ ] `validate.isInRange()` function for numeric range checks

- [ ] `validate.isAlphaNumeric()` function for alphanumeric checks

- [x] Error handling: try/catch, subshell

- [x] More complex expressions and operators

- [x] Support for managing environment variables

- [x] Enhanced string manipulation functions

- [x] File I/O operations (exists, read/write files, copy, move, rename)

- [x] File path manipulation functions (dirname, fileName, extension, parentDirName)

- [x] Template functions to update files with dynamic content (`template.update()`)

- [x] json parsing and manipulation functions

- [x] yaml parsing and manipulation functions

- [ ] `fs.delete()` - Delete files or directories with recursive option

- [ ] `fs.watch()` - File system watching for changes

- [ ] `fs.createTempFolder()` - Create temporary folders with unique names

- [ ] `fs.createTempFile()` - Create temporary files with unique names

- [ ] `fs.find()` - Find files matching patterns or conditions

- [ ] `fs.chmod()` - Change file permissions

- [ ] `fs.chown()` - Change file ownership

- [ ] `process.start()` - Start a new process with command and arguments

- [ ] `process.kill()` - Kill a running process by ID or name

- [ ] `process.isRunning()` - Check if a process is currently running

- [ ] `process.waitForExit()` - Wait for a process to exit and get its exit code

- [x] Argument parsing and validation (with default values, types, and help generation)

- [x] Parallel execution

- [x] Deferred execution

- [x] `scheduler.cron()` - Cron job scheduling with persistent jobs

- [x] Time and Resource monitoring

- [x] VS Code extension for syntax highlighting

- [x] Code formatting with EditorConfig support

- [ ] VS Code extension for intellisense and autocompletion
