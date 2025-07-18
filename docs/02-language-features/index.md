---
layout: default
title: Language Features
nav_order: 3
has_children: true
---

Utah provides modern language features inspired by TypeScript, making shell scripting more reliable, readable, and maintainable.

## Core Language Features

### Type System

- **[Variables and Types](variables.md)** - Strong typing with TypeScript-like syntax

- Type annotations for all variables and function parameters
- Support for strings, numbers, booleans, arrays, and objects
- Compile-time type checking and validation

### Control Flow

- **[Control Flow](control-flow.md)** - Modern control structures

- If/else statements with logical operators
- Switch statements with fall-through support
- For loops (traditional and for-in styles)
- While loops with break statements

### Functions

- **[Functions](functions.md)** - First-class function support

- Type-annotated parameters and return types
- Local variable scoping
- Function composition and reusability

### Data Structures

- **[Arrays and Strings](arrays-strings.md)** - Rich data manipulation

- Typed arrays with built-in methods
- String manipulation and interpolation
- Advanced array operations

### Error Handling

- **[Error Handling](error-handling.md)** - Robust error management

- Try/catch blocks for exception handling
- Graceful error recovery
- Error propagation and logging

### Module System

- **[Import System](imports.md)** - Modular code organization

- File-based import system
- Code reuse across multiple scripts
- Dependency management

## Language Philosophy

### TypeScript-Inspired Syntax

Utah uses familiar TypeScript syntax to make shell scripting more accessible:

```typescript
// Utah script (.shx)
const appName: string = "MyApp";
let users: string[] = ["Alice", "Bob"];

function greet(name: string): void {
  console.log(`Hello, ${name}!`);
}

for (let user: string in users) {
  greet(user);
}
```

### Clean Bash Output

Utah generates readable, standard bash code:

```bash
#!/bin/bash
readonly appName="MyApp"
users=("Alice" "Bob")

greet() {
  local name="$1"
  echo "Hello, ${name}!"
}

for user in "${users[@]}"; do
  greet "${user}"
done
```

## Advanced Features

### String Interpolation

Use template literals for dynamic strings:

```typescript
let name: string = "Utah";
let version: number = 1.0;
let message: string = `Welcome to ${name} v${version}!`;
```

### Parallel Execution

Run functions concurrently for better performance:

```typescript
parallel processFile("file1.txt");
parallel processFile("file2.txt");
parallel processFile("file3.txt");

let _ = `$(wait)`;  // Wait for all to complete
console.log("All files processed");
```

### Built-in Function Integration

Seamlessly use Utah's rich function library:

```typescript
// File operations
if (fs.exists("config.json")) {
  let config: string = fs.readFile("config.json");
  
  // JSON processing
  if (json.isValid(config)) {
    let configObj: object = json.parse(config);
    let dbHost: string = json.get(configObj, ".database.host");
  }
}

// System integration
if (os.isInstalled("git")) {
  console.log("Git is available");
}
```

### Script Metadata

Add metadata and command-line argument support:

```typescript
script.description("File processing utility");

args.define("--input", "-i", "Input file", "string", true);
args.define("--output", "-o", "Output file", "string", false, "output.txt");
args.define("--verbose", "-v", "Enable verbose output");

if (args.has("--help")) {
  args.showHelp();
  exit(0);
}
```

## Best Practices

### 1. Use Type Annotations

Always specify types for clarity and safety:

```typescript
// Good
let userName: string = "Alice";
let count: number = 42;

// Avoid
let userName = "Alice";
let count = 42;
```

### 2. Handle Errors Gracefully

Use try/catch for robust error handling:

```typescript
try {
  let data: string = fs.readFile("important.txt");
  console.log("File loaded successfully");
}
catch {
  console.log("File not found, using defaults");
}
```

### 3. Validate Dependencies

Check for required tools before using them:

```typescript
if (!os.isInstalled("jq")) {
  json.installDependencies();
}
```

### 4. Use Descriptive Names

Make your code self-documenting:

```typescript
// Good
let databaseConnectionString: string = "localhost:5432";
let maxRetryAttempts: number = 3;

// Less clear
let db: string = "localhost:5432";
let max: number = 3;
```

## Language Comparison

### Utah vs Traditional Bash

**Traditional Bash:**

```bash
#!/bin/bash
name="Alice"
if [ "$name" == "Alice" ]; then
  echo "Hello, $name!"
fi
```

**Utah:**

```typescript
let name: string = "Alice";
if (name == "Alice") {
  console.log(`Hello, ${name}!`);
}
```

### Utah vs Other Languages

Utah combines the best of both worlds:

- **Familiar syntax** from TypeScript/JavaScript

- **Shell integration** from bash
- **Type safety** for better reliability
- **Rich built-ins** for common tasks

## Getting Started with Language Features

1. **Start with basics**: [Variables and Types](variables.md)
2. **Add logic**: [Control Flow](control-flow.md)
3. **Organize code**: [Functions](functions.md)
4. **Handle data**: [Arrays and Strings](arrays-strings.md)
5. **Manage errors**: [Error Handling](error-handling.md)
6. **Scale up**: [Import System](imports.md)

## Common Patterns

### Configuration Script

```typescript
script.description("Application configuration manager");

const CONFIG_FILE: string = "app.json";
let config: object;

try {
  let configData: string = fs.readFile(CONFIG_FILE);
  config = json.parse(configData);
  console.log("Configuration loaded");
}
catch {
  console.log("Using default configuration");
  config = json.parse('{"port": 8080, "debug": false}');
}

let port: number = json.get(config, ".port");
console.log(`Server will start on port ${port}`);
```

### Deployment Script

```typescript
script.description("Application deployment automation");

args.define("--env", "-e", "Environment", "string", true);
args.define("--version", "-v", "Version tag", "string", true);

let environment: string = args.get("--env");
let version: string = args.get("--version");

console.log(`Deploying version ${version} to ${environment}`);

// Validate environment
let validEnvs: string[] = ["dev", "staging", "prod"];
if (!validEnvs.contains(environment)) {
  console.log("Invalid environment");
  exit(1);
}

// Check dependencies
if (!os.isInstalled("docker")) {
  console.log("Docker is required for deployment");
  exit(1);
}

console.log("Deployment started...");
```

### Data Processing Script

```typescript
script.description("JSON data processor");

let inputFiles: string[] = ["data1.json", "data2.json", "data3.json"];
let results: object[] = [];

for (let file: string in inputFiles) {
  if (fs.exists(file)) {
    try {
      let content: string = fs.readFile(file);
      let data: object = json.parse(content);
      
      // Process data
      let processed: object = json.set(data, ".processed", true);
      processed = json.set(processed, ".timestamp", timer.current());
      
      results[results.length] = processed;
      console.log(`Processed: ${file}`);
    }
    catch {
      console.log(`Error processing: ${file}`);
    }
  }
}

console.log(`Processed ${results.length} files successfully`);
```

## Next Steps

Explore each language feature in detail:

1. **[Variables and Types](variables.md)** - Foundation of type-safe scripting
2. **[Control Flow](control-flow.md)** - Logic and decision making
3. **[Functions](functions.md)** - Code organization and reuse
4. **[Arrays and Strings](arrays-strings.md)** - Data manipulation
5. **[Error Handling](error-handling.md)** - Robust error management
6. **[Import System](imports.md)** - Modular development
