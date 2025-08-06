---
layout: default
title: Your First Script
parent: Getting Started
nav_order: 2
---

This guide will walk you through creating and running your first Utah script.

## Hello World

Let's start with the classic "Hello World" example.

### Step 1: Create the Script

Create a new file called `hello.shx`:

```typescript
console.log("Hello, Utah!");
```

### Step 2: Compile the Script

Use the Utah CLI to compile your script:

```bash
utah compile hello.shx
```

This generates `hello.sh` with the following content:

```bash
#!/bin/bash
echo "Hello, Utah!"
```

### Step 3: Run the Script

Execute the generated bash script:

```bash
./hello.sh
```

Output:

```text
Hello, Utah!
```

## Adding Variables

Let's enhance our script with variables and string interpolation:

**Create `variables.shx`:**

```typescript
const appName: string = "Utah";
let userName: string = "Developer";
let version: number = 1.0;

console.log("Welcome to ${appName}!");
console.log("Hello, ${userName}");
console.log("Version: ${version}");
```

**Compile and run:**

```bash
utah compile variables.shx
./variables.sh
```

**Generated bash code (`variables.sh`):**

```bash
#!/bin/bash
readonly appName="Utah"
userName="Developer"
version="1.0"

echo "Welcome to ${appName}!"
echo "Hello, ${userName}"
echo "Version: ${version}"
```

**Output:**

```text
Welcome to Utah!
Hello, Developer
Version: 1.0
```

## Working with Functions

Utah supports function declarations with type annotations:

**Create `functions.shx`:**

```typescript
function greet(name: string): void {
  console.log("Hello, ${name}!");
}

function add(a: number, b: number): number {
  return a + b;
}

// Call functions
greet("Utah User");

let result: number = add(5, 3);
console.log("5 + 3 = ${result}");
```

**Generated bash code:**

```bash
#!/bin/bash

greet() {
  local name="$1"
  echo "Hello, ${name}!"
}

add() {
  local a="$1"
  local b="$2"
  echo $((a + b))
}

greet "Utah User"
result=$(add 5 3)
echo "5 + 3 = ${result}"
```

## Control Flow Examples

### Conditional Logic

**Create `conditionals.shx`:**

```typescript
let score: number = 85;

if (score >= 90) {
  console.log("Excellent!");
} else if (score >= 70) {
  console.log("Good job!");
} else {
  console.log("Keep trying!");
}
```

### Loops

**Create `loops.shx`:**

```typescript
// For loop
for (let i: number = 1; i <= 3; i++) {
  console.log("Count: ${i}");
}

// Array iteration
let fruits: string[] = ["apple", "banana", "orange"];
for (let fruit: string in fruits) {
  console.log("Fruit: ${fruit}");
}
```

## Working with Arrays

**Create `arrays.shx`:**

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Alice", "Bob", "Charlie"];

console.log("First number: ${numbers[0]}");
console.log("Array length: ${numbers.length}");
console.log("Is empty: ${numbers.isEmpty()}");

// Check if array contains an item
if (names.contains("Alice")) {
  console.log("Alice is in the list");
}
```

## Using Built-in Functions

Utah provides many built-in functions for common tasks:

**Create `builtin.shx`:**

```typescript
// File system operations
if (fs.exists("README.md")) {
  console.log("README.md exists");
}

// Check if command is installed
if (os.isInstalled("git")) {
  console.log("Git is available");
} else {
  console.log("Git is not installed");
}

// Get current OS
let currentOS: string = os.getOS();
console.log("Running on: ${currentOS}");

// Generate random number
let randomNum: number = utility.random(1, 10);
console.log("Random number: ${randomNum}");
```

## Error Handling

Use try/catch blocks for robust error handling:

**Create `error-handling.shx`:**

```typescript
try {
  let content: string = fs.readFile("nonexistent.txt");
  console.log(content);
}
catch {
  console.log("Error: File not found");
}

console.log("Script continues...");
```

## Working with JSON

Utah provides excellent JSON support:

**Create `json-example.shx`:**

```typescript
// Ensure JSON dependencies are available
json.installDependencies();

let configJson: string = '{"app": {"name": "MyApp", "port": 8080}}';

if (json.isValid(configJson)) {
  let config: object = json.parse(configJson);
  let appName: string = json.get(config, ".app.name");
  let port: number = json.get(config, ".app.port");

  console.log("App: ${appName}");
  console.log("Port: ${port}");
} else {
  console.log("Invalid JSON");
}
```

## Script Metadata and Arguments

Create scripts that accept command-line arguments:

**Create `cli-script.shx`:**

```typescript
script.description("My first CLI script");

// Define arguments
args.define("--name", "-n", "Your name", "string", false, "World");
args.define("--count", "-c", "Number of greetings", "number", false, 1);
args.define("--verbose", "-v", "Enable verbose output");

// Handle help
if (args.has("--help")) {
  args.showHelp();
  exit(0);
}

// Get argument values
let name: string = args.get("--name");
let count: number = args.get("--count");
let verbose: boolean = args.has("--verbose");

if (verbose) {
  console.log("Verbose mode enabled");
}

for (let i: number = 1; i <= count; i++) {
  console.log("Hello, ${name}!");
}
```

**Run with arguments:**

```bash
utah compile cli-script.shx
./cli-script.sh --name "Utah User" --count 3 --verbose
```

## Best Practices for Your First Scripts

### 1. Use Type Annotations

Always specify types for better code clarity:

```typescript
// Good
let userName: string = "Alice";
let count: number = 42;

// Less clear
let userName = "Alice";
let count = 42;
```

### 2. Handle Errors Gracefully

Use try/catch for operations that might fail:

```typescript
try {
  let result: string = someRiskyOperation();
  console.log(result);
}
catch {
  console.log("Operation failed, but script continues");
}
```

### 3. Validate Dependencies

Check for required tools before using them:

```typescript
if (!os.isInstalled("git")) {
  console.log("This script requires git. Please install it first.");
  exit(1);
}
```

### 4. Use Descriptive Variable Names

Make your code self-documenting:

```typescript
// Good
let databaseConnectionString: string = "localhost:5432";
let maxRetryAttempts: number = 3;

// Less clear
let db: string = "localhost:5432";
let max: number = 3;
```

### 5. Add Script Descriptions

Document what your script does:

```typescript
script.description("Automated backup script for project files");
```

## Common Patterns

### Configuration Loading

```typescript
if (fs.exists("config.json")) {
  let configContent: string = fs.readFile("config.json");
  let config: object = json.parse(configContent);
  // Use configuration
} else {
  console.log("No configuration file found, using defaults");
}
```

### Environment Detection

```typescript
let environment: string = env.get("NODE_ENV", "development");
let isProduction: boolean = environment == "production";

if (isProduction) {
  console.log("Running in production mode");
} else {
  console.log("Running in development mode");
}
```

### Backup Operations

```typescript
let timestamp: string = timer.current().toString();
let backupDir: string = "backup_${timestamp}";

if (!fs.exists(backupDir)) {
  // Create backup directory
  console.log("Creating backup directory: ${backupDir}");
}
```

## Next Steps

Now that you've created your first Utah script, you can:

1. **Learn more syntax**: [Basic Syntax Guide](syntax.md)
2. **Explore language features**: [Variables and Types](../02-language-features/variables.md)
3. **Discover built-in functions**: [Function Reference](../03-builtin-functions/index.md)
4. **Study advanced patterns**: [Best Practices Guide](../04-guides/best-practices.md)

## Troubleshooting

### Script Won't Compile

1. Check for syntax errors:

   ```bash
   utah compile --verbose script.shx
   ```

2. Verify file extension is `.shx`

3. Ensure proper type annotations

### Generated Script Won't Run

1. Check file permissions:

   ```bash
   chmod +x script.sh
   ```

2. Verify bash availability:

   ```bash
   which bash
   ```

3. Check for missing dependencies:

   ```bash
   utah run script.shx  # This will show dependency errors
   ```

### Getting Help

- **Utah CLI help**: `utah --help`
- **Function reference**: [Built-in Functions](../03-builtin-functions/index.md)
- **Community support**: [GitHub Discussions](https://github.com/polatengin/utah/discussions)
