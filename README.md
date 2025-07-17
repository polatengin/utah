# Project Utah

[![Release Test Results](https://github.com/polatengin/utah/actions/workflows/release.yml/badge.svg)](https://github.com/polatengin/utah/actions/workflows/release.yml)

`utah` is a CLI tool built with .NET 9 that allows to write shell scripts in a strongly typed, typescript-inspired language (`.shx`). It then transpiles `.shx` code into clean, standard `.sh` bash scripts.

📦 File Extension: `.shx`

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

Get the length of an array using the `.length` property:

```typescript
let items: string[] = ["apple", "banana", "cherry"];
let count: number = items.length;  // Gets 3
```

Check if an array is empty using the `.isEmpty()` method:

```typescript
let emptyArray: string[] = [];
let filledArray: string[] = ["apple", "banana", "cherry"];

let emptyCheck: boolean = emptyArray.isEmpty();  // Gets true
let filledCheck: boolean = filledArray.isEmpty(); // Gets false

// Use in conditionals
if (emptyArray.isEmpty()) {
  console.log("The array is empty");
}

if (!filledArray.isEmpty()) {
  console.log("The array has items");
}
```

Reverse an array using the `.reverse()` method:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let reversed: number[] = numbers.reverse();  // Gets [5, 4, 3, 2, 1]

let fruits: string[] = ["apple", "banana", "cherry"];
let reversedFruits: string[] = fruits.reverse();  // Gets ["cherry", "banana", "apple"]

// Use in assignments and conditionals
let emptyArray: string[] = [];
let reversedEmpty: string[] = emptyArray.reverse();  // Gets []

if (!numbers.reverse().isEmpty()) {
  console.log("Reversed array is not empty");
}
```

Check if an array contains a specific element using the `.contains()` method:

```typescript
let fruits: string[] = ["apple", "banana", "cherry"];
let numbers: number[] = [1, 2, 3, 4, 5];
let flags: boolean[] = [true, false, true];

// Check for specific values
let hasApple: boolean = fruits.contains("apple");      // Gets true
let hasGrape: boolean = fruits.contains("grape");      // Gets false
let hasThree: boolean = numbers.contains(3);           // Gets true
let hasTen: boolean = numbers.contains(10);            // Gets false
let hasTrue: boolean = flags.contains(true);           // Gets true

// Use in conditionals
if (fruits.contains("apple")) {
  console.log("Found apple in fruits array");
}

if (!numbers.contains(99)) {
  console.log("99 is not in the numbers array");
}

// Use with variables
let searchFruit: string = "cherry";
let searchNumber: number = 2;

if (fruits.contains(searchFruit)) {
  console.log(`Found ${searchFruit} in fruits`);
}

if (numbers.contains(searchNumber)) {
  console.log(`Found ${searchNumber} in numbers`);
}
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
- `fs.exists(filepath)` - Check if a file or directory exists, returns boolean

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
- **String interpolation**: Compatible with template literals and variable substitution
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

# console.promptYesNo("prompt text") becomes:
proceed=$(while true; do read -p "Do you want to continue? (y/n): " yn; case $yn in [Yy]* ) echo "true"; break;; [Nn]* ) echo "false"; break;; * ) echo "Please answer yes or no.";; esac; done)

# Complete example:
isSudo=$([ "$(id -u)" -eq 0 ] && echo "true" || echo "false")

if [ "${isSudo}" = "true" ]; then
  echo "Running with elevated privileges"
else
  echo "Running with normal user privileges"
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
- **Security Checks**: Validate user permissions for sensitive operations
- **Admin Scripts**: Build installation and configuration scripts that require root access
- **Conditional Logic**: Execute different code paths based on privilege level
- **Error Prevention**: Fail gracefully when insufficient privileges are detected
- **User Guidance**: Provide helpful messages about required permissions

### Security Best Practices

- **Always check privileges** before performing system-level operations
- **Fail early** if required privileges are not available
- **Provide clear error messages** when privilege checks fail
- **Use `console.isSudo()`** instead of assuming user permissions
- **Document privilege requirements** in script comments and usage instructions

### Technical Notes

- Uses the standard Unix `id -u` command to check effective user ID
- Returns `true` when effective user ID is 0 (root)
- Returns `false` for all non-root users
- Works consistently across all Unix-like systems (Linux, macOS, BSD)
- Lightweight check with minimal performance impact

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

# Format in place (overwrite original)
utah format script.shx --in-place

# Check if file is properly formatted (exit 1 if not)
utah format script.shx --check
```

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

- **arrays.shx** - Array literals, access, length, and iteration
- **array_contains.shx** - Array contains() method for checking element presence
- **arrays_isempty.shx** - Array isEmpty() method for checking empty arrays
- **console_issudo.shx** - Console system functions and privilege checking
- **console_prompt_yesno.shx** - User interaction with yes/no prompts
- **const_valid_assignment.shx** - Const variable declarations and immutability
- **environment_variables.shx** - Environment variable operations
- **for_in_loop.shx** - For-in loops with arrays
- **fs_dirname.shx** - File system dirname function
- **fs_exists.shx** - File system file/directory existence checking
- **fs_extension.shx** - File system extension extraction
- **fs_filename.shx** - File system filename extraction
- **fs_functions.shx** - File system read/write operations
- **fs_parentdirname.shx** - File system parent directory name extraction
- **fs_path.shx** - File system path manipulation
- **function_declarations.shx** - Function definitions with typed parameters and return values
- **git_undo_last_commit.shx** - Git undo last commit functionality
- **if_elseif_else.shx** - If-else-if-else conditional statements with proper chaining
- **import_test.shx** - Basic import functionality with multiple files
- **json_manipulation.shx** - JSON property manipulation and modification functions
- **json_parse_validation.shx** - JSON parsing and validation functions
- **json_property_access.shx** - JSON property access and existence checking
- **json_utilities.shx** - JSON utility functions (keys, values, merge)
- **let_reassignment_test.shx** - Variable reassignment and mutability
- **mixed_loops.shx** - Mixed loop types in one file
- **mixed_syntax.shx** - Mixed Utah and bash syntax compatibility
- **nested_for_loop.shx** - Nested for loops
- **nested_import_test.shx** - Nested import functionality with import chains
- **os_getlinuxversion.shx** - Operating system Linux version detection
- **os_getos.shx** - Operating system detection
- **os_isinstalled.shx** - Command/application installation checking
- **parallel_function_call.shx** - Parallel function calls
- **process_info.shx** - Process information functions (ID, CPU, memory, etc.)
- **script_continueonerror.shx** - Script control function for continuing on errors
- **script_description.shx** - Script description metadata function
- **script_disabledebug.shx** - Script control function for disabling debug mode
- **script_disableglobbing.shx** - Script control function for disabling globbing
- **script_enabledebug.shx** - Script control function for enabling debug mode
- **script_enableglobbing.shx** - Script control function for enabling globbing
- **script_exitonerror.shx** - Script control function for exiting on errors
- **simple_console.shx** - Basic console.log functionality
- **simple_for_loop.shx** - Traditional for loops
- **simple_switch.shx** - Basic switch/case/default statements
- **string_functions.shx** - String manipulation functions
- **switch_case.shx** - Complex switch statements with fall-through cases
- **ternary_operators.shx** - Ternary conditional operators
- **timer_start_stop.shx** - Timer functions for performance measurement
- **try_catch.shx** - Try/catch blocks for error handling and graceful failure recovery
- **variable_declaration.shx** - Variable declarations and usage
- **while_loop.shx** - While loops with break statements and conditional logic

### Negative Test Coverage

The negative test fixtures ensure that the compiler correctly handles and reports errors for invalid code:

- **array_type_mismatch.shx** - Boolean array with mixed types (string in boolean array)
- **const_reassignment_test.shx** - Attempt to reassign a const variable after declaration
- **import_missing_file.shx** - Import statement referencing a non-existent file

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

- [x] Console system functions (`console.isSudo()`)

- [x] String interpolation with backticks (`Hello, ${name}`)

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

- [ ] `array.*` functions (`join()`, `sort()`, `merge()`)

- [x] Add shebang (`#!/bin/bash`) to generated scripts

- [ ] Typed return values and richer type checking

- [x] `utility.*` functions (`random()` with min/max parameters)

- [x] Throw error if `min` is greater than max in `utility.random(<min>, <max>)` function

- [x] `script.*` functions (`description()`, `enableDebug()`, `disableDebug()`, `disableGlobbing()`, `enableGlobbing()`, `exitOnError()`, `continueOnError()`)

- [x] `args.*` functions (`get()`, `has()`, `all()`, `define()`, `showHelp()` implemented)

- [x] `web.*` functions (`get()` implemented - `put()`, `speedtest()` coming soon)

- [ ] `console.*` functions (`clear()` implemented, `drawWindow()`, `optionList()` coming soon)

- [ ] `ssh.*` functions (`connect()`, `save()`, `transferFile()`)

- [ ] `git.*` functions (`undoLastCommit()` implemented, `mergePR()`, `forcePush()` coming soon)

- [ ] `@decorator()` syntax for decorators for validation (`@notNullable()`, `@notEmpty()`, `@greaterThan()`, `@ipv4()`, `@numberOnly()`, `@alphaNumeric()`, `@email()`)

- [x] Error handling: try/catch, subshell

- [ ] Typecasting and type conversion

- [x] More complex expressions and operators

- [x] Support for managing environment variables

- [ ] Support for functions with multiple return values

- [x] Enhanced string manipulation functions

- [x] File I/O operations (exists, read/write files)

- [x] File path manipulation functions (dirname, fileName, extension, parentDirName)

- [ ] Template functions to update files with dynamic content

- [x] json parsing and manipulation functions

- [ ] yaml parsing and manipulation functions

- [ ] File I/O operations (watch, delete, copy, move, rename, permissions, etc.)

- [x] Argument parsing and validation (with default values, types, and help generation)

- [x] Parallel execution

- [x] `scheduler.cron()` - Cron job scheduling with persistent jobs

- [x] Time and Resource monitoring

- [x] VS Code extension for syntax highlighting

- [x] Code formatting with EditorConfig support

- [ ] VS Code extension for intellisense and autocompletion
