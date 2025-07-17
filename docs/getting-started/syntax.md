---
layout: default
title: Basic Syntax
parent: Getting Started
nav_order: 3
---

Utah uses TypeScript-like syntax to write shell scripts. This guide covers the fundamental syntax elements you'll use in every Utah script.

## Variables and Constants

### Variable Declarations

Utah supports three types of variable declarations:

```typescript
// Mutable variable with type annotation
let userName: string = "Alice";

// Immutable constant
const appName: string = "MyApp";

// Variable with type inference (less preferred)
let count = 42;
```

### Generated Bash Code

```bash
userName="Alice"
readonly appName="MyApp"
count="42"
```

### Supported Types

- `string` - Text values
- `number` - Numeric values
- `boolean` - True/false values
- `string[]` - Arrays of strings
- `number[]` - Arrays of numbers
- `object` - JSON/YAML objects

## String Literals and Interpolation

### Basic Strings

```typescript
let message: string = "Hello, World!";
let singleQuoted: string = 'Also works';
```

### Template Literals

Use template literals for string interpolation:

```typescript
let name: string = "Utah";
let version: number = 1.0;
let greeting: string = `Welcome to ${name} v${version}!`;
```

**Generated bash:**

```bash
name="Utah"
version="1.0"
greeting="Welcome to ${name} v${version}!"
```

### Multiline Strings

```typescript
let description: string = `
This is a multiline
string that spans
multiple lines.
`;
```

## Comments

Utah supports both single-line and multi-line comments:

```typescript
// This is a single-line comment

/*
This is a multi-line comment
that can span multiple lines
*/

let value: string = "Hello"; // Inline comment
```

## Functions

### Function Declaration

```typescript
function functionName(param1: type, param2: type): returnType {
  // Function body
  return value; // if returnType is not void
}
```

### Examples

```typescript
// Function with no return value
function greet(name: string): void {
  console.log(`Hello, ${name}!`);
}

// Function with return value
function add(a: number, b: number): number {
  return a + b;
}

// Function with optional parameters (using default values)
function multiply(a: number, b: number = 1): number {
  return a * b;
}
```

### Function Generated Bash Code

```bash
greet() {
  local name="$1"
  echo "Hello, ${name}!"
}

add() {
  local a="$1"
  local b="$2"
  echo $((a + b))
}

multiply() {
  local a="$1"
  local b="${2:-1}"
  echo $((a * b))
}
```

## Control Flow

### If Statements

```typescript
if (condition) {
  // Code block
} else if (anotherCondition) {
  // Another code block
} else {
  // Default code block
}
```

### Comparison Operators

```typescript
let a: number = 5;
let b: number = 10;
let name: string = "Alice";

if (a == b) { /* equal */ }
if (a != b) { /* not equal */ }
if (a < b) { /* less than */ }
if (a > b) { /* greater than */ }
if (a <= b) { /* less than or equal */ }
if (a >= b) { /* greater than or equal */ }

// String comparisons
if (name == "Alice") { /* string equality */ }
if (name != "Bob") { /* string inequality */ }
```

### Logical Operators

```typescript
let isActive: boolean = true;
let isAdmin: boolean = false;

if (isActive && isAdmin) { /* both true */ }
if (isActive || isAdmin) { /* at least one true */ }
if (!isActive) { /* not active */ }
```

### Switch Statements

```typescript
let status: string = "active";

switch (status) {
  case "active":
    console.log("User is active");
    break;
  case "inactive":
    console.log("User is inactive");
    break;
  default:
    console.log("Unknown status");
    break;
}
```

## Loops

### For Loops

Traditional C-style for loops:

```typescript
for (let i: number = 0; i < 10; i++) {
  console.log(`Iteration: ${i}`);
}
```

### For-In Loops

Iterate over arrays:

```typescript
let fruits: string[] = ["apple", "banana", "orange"];

for (let fruit: string in fruits) {
  console.log(`Fruit: ${fruit}`);
}
```

### While Loops

```typescript
let count: number = 0;

while (count < 5) {
  console.log(`Count: ${count}`);
  count++;
}
```

### Break Statements

```typescript
for (let i: number = 0; i < 10; i++) {
  if (i == 5) {
    break;
  }
  console.log(i);
}
```

## Arrays

### Array Declaration

```typescript
// String array
let names: string[] = ["Alice", "Bob", "Charlie"];

// Number array
let numbers: number[] = [1, 2, 3, 4, 5];

// Empty array
let emptyList: string[] = [];
```

### Array Access

```typescript
let fruits: string[] = ["apple", "banana", "orange"];

let first: string = fruits[0];  // "apple"
let second: string = fruits[1]; // "banana"
```

### Array Properties and Methods

```typescript
let items: string[] = ["a", "b", "c"];

let length: number = items.length;
let isEmpty: boolean = items.isEmpty();
let contains: boolean = items.contains("b");

// Reverse array
let reversed: string[] = items.reverse();
```

## Error Handling

### Try-Catch Blocks

```typescript
try {
  // Code that might fail
  let content: string = fs.readFile("file.txt");
  console.log(content);
}
catch {
  // Handle error
  console.log("Failed to read file");
}
```

## Built-in Functions

Utah provides many built-in functions organized into namespaces:

### Console Functions

```typescript
console.log("Message");              // Print message
console.clear();                     // Clear screen
let isSudo: boolean = console.isSudo();  // Check privileges
let answer: boolean = console.promptYesNo("Continue?");
```

### File System Functions

```typescript
let exists: boolean = fs.exists("file.txt");
let content: string = fs.readFile("file.txt");
fs.writeFile("output.txt", "content");
```

### OS Functions

```typescript
let isInstalled: boolean = os.isInstalled("git");
let osType: string = os.getOS();
let version: string = os.getLinuxVersion();
```

### Utility Functions

```typescript
let randomNum: number = utility.random(1, 100);
```

### JSON Functions

```typescript
json.installDependencies();  // Ensure jq is installed
let valid: boolean = json.isValid('{"key": "value"}');
let obj: object = json.parse('{"name": "Utah"}');
let value: string = json.get(obj, ".name");
```

### YAML Functions

```typescript
yaml.installDependencies();  // Ensure yq and jq are installed
let valid: boolean = yaml.isValid("name: Utah");
let obj: object = yaml.parse("name: Utah\nversion: 1.0");
let value: string = yaml.get(obj, ".name");
```

## Script Metadata

### Script Description

Add a description to your script:

```typescript
script.description("This script automates deployment tasks");
```

### Command Line Arguments

Define and use command-line arguments:

```typescript
// Define arguments
args.define("--input", "-i", "Input file path", "string", true);
args.define("--output", "-o", "Output file path", "string", false, "output.txt");
args.define("--verbose", "-v", "Enable verbose output");

// Check for help
if (args.has("--help")) {
  args.showHelp();
  exit(0);
}

// Get argument values
let inputFile: string = args.get("--input");
let outputFile: string = args.get("--output");
let verbose: boolean = args.has("--verbose");
```

## Import System

### Basic Import

```typescript
import "utils.shx";
import "config.shx";
```

### Import Resolution

Utah resolves imports in this order:

1. Relative to current file
2. Relative to script directory
3. Standard library (if implemented)

## Ternary Operator

```typescript
let status: string = isActive ? "active" : "inactive";
let message: string = count > 0 ? `Found ${count} items` : "No items found";
```

**Generated bash:**

```bash
if [ "${isActive}" = "true" ]; then
  status="active"
else
  status="inactive"
fi
```

## Exit Statements

```typescript
// Exit with success
exit(0);

// Exit with error
exit(1);

// Conditional exit
if (criticalError) {
  console.log("Critical error occurred");
  exit(1);
}
```

## Parallel Execution

Execute functions in parallel:

```typescript
function slowTask(name: string): void {
  console.log(`Starting ${name}...`);
  // Some work here
  console.log(`Finished ${name}`);
}

// Run tasks in parallel
parallel slowTask("Task A");
parallel slowTask("Task B");
parallel slowTask("Task C");

// Wait for all to complete
let _ = `$(wait)`;
console.log("All tasks completed");
```

## Best Practices

### 1. Always Use Type Annotations

```typescript
// Good
let userName: string = "Alice";
let count: number = 42;

// Avoid
let userName = "Alice";  // Type unclear
```

### 2. Use Descriptive Names

```typescript
// Good
let databaseConnectionString: string = "localhost:5432";
let maxRetryAttempts: number = 3;

// Avoid
let db: string = "localhost:5432";
let max: number = 3;
```

### 3. Handle Errors Appropriately

```typescript
// Good
try {
  let result: string = riskyOperation();
  console.log(result);
}
catch {
  console.log("Operation failed gracefully");
}

// Avoid unhandled failures
let result: string = riskyOperation();  // Might fail silently
```

### 4. Validate Dependencies

```typescript
// Check before using external tools
if (!os.isInstalled("git")) {
  console.log("Git is required for this script");
  exit(1);
}
```

### 5. Use Constants for Fixed Values

```typescript
// Good
const MAX_RETRIES: number = 3;
const CONFIG_FILE: string = "app.json";

// Less clear
let maxRetries: number = 3;  // Might be changed accidentally
```

## Syntax Highlighting

For better development experience, configure your editor for TypeScript syntax highlighting when working with `.shx` files. Most editors will treat `.shx` files as TypeScript files if you associate the extension.

## Next Steps

Now that you understand Utah's basic syntax:

1. **Explore language features**: [Variables and Types](../language-features/variables.md)
2. **Learn about functions**: [Function Guide](../language-features/functions.md)
3. **Study control flow**: [Control Flow](../language-features/control-flow.md)
4. **Discover built-ins**: [Built-in Functions](../functions/)

## Common Syntax Errors

### Missing Type Annotations

```typescript
// Error: Missing type annotation
let userName = "Alice";

// Correct
let userName: string = "Alice";
```

### Incorrect Function Syntax

```typescript
// Error: Missing return type
function add(a: number, b: number) {
  return a + b;
}

// Correct
function add(a: number, b: number): number {
  return a + b;
}
```

### Improper Array Declaration

```typescript
// Error: Incorrect array syntax
let items: Array<string> = ["a", "b"];

// Correct
let items: string[] = ["a", "b"];
```

### Missing Semicolons

While not always required, semicolons improve clarity:

```typescript
// Preferred
let name: string = "Utah";
console.log(name);

// Works but less clear
let name: string = "Utah"
console.log(name)
```
