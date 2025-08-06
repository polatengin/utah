---
layout: default
title: Variables and Types
parent: Language Features
nav_order: 1
---

Utah provides a strong type system inspired by TypeScript, allowing you to write more reliable shell scripts with compile-time type checking.

## Variable Declarations

### `let` - Mutable Variables

Use `let` to declare mutable variables:

```typescript
let userName: string = "Alice";
let count: number = 42;
let isActive: boolean = true;

// Variables can be reassigned
userName = "Bob";
count = count + 1;
isActive = false;
```

**Generated bash:**

```bash
userName="Alice"
count="42"
isActive="true"

userName="Bob"
count=$((count + 1))
isActive="false"
```

### `const` - Immutable Constants

Use `const` for values that won't change:

```typescript
const APP_NAME: string = "MyApplication";
const MAX_RETRIES: number = 3;
const DEBUG_MODE: boolean = false;

// Error: Cannot reassign const variables
// APP_NAME = "OtherApp";  // This would cause an error
```

**Generated bash:**

```bash
readonly APP_NAME="MyApplication"
readonly MAX_RETRIES="3"
readonly DEBUG_MODE="false"
```

## Type Annotations

Utah supports several built-in types:

### Basic Types

#### `string`

Text and character data:

```typescript
let message: string = "Hello, Utah!";
let templateLiteral: string = "Dynamic: ${message}";
```

#### `number`

Numeric values (integers and decimals):

```typescript
let integer: number = 42;
let decimal: number = 3.14;
let negative: number = -10;
let calculated: number = 5 + 3;
```

#### `boolean`

True/false values:

```typescript
let isEnabled: boolean = true;
let isDisabled: boolean = false;
let computed: boolean = count > 0;
```

### Array Types

#### `string[]`

Arrays of strings:

```typescript
let names: string[] = ["Alice", "Bob", "Charlie"];
let fruits: string[] = [];  // Empty array
let colors: string[] = ["red", "green", "blue"];

// Access elements
let firstName: string = names[0];    // "Alice"
let lastColor: string = colors[2];   // "blue"
```

#### `number[]`

Arrays of numbers:

```typescript
let scores: number[] = [85, 92, 78, 90];
let fibonacci: number[] = [1, 1, 2, 3, 5, 8];
let measurements: number[] = [1.5, 2.7, 3.8];

// Array operations
let firstScore: number = scores[0];
let arrayLength: number = scores.length;
```

### Object Type

#### `object`

For JSON/YAML objects:

```typescript
// Parse JSON into object
let configJson: string = "{\"host\": \"localhost\", \"port\": 8080}";
let config: object = json.parse(configJson);

// Parse YAML into object
let yamlConfig: string = "host: localhost\nport: 8080";
let yamlObj: object = yaml.parse(yamlConfig);

// Access object properties
let host: string = json.get(config, ".host");
let port: number = json.get(config, ".port");
```

## Type Inference

While type annotations are recommended, Utah can infer types in some cases:

```typescript
// Explicit typing (preferred)
let name: string = "Utah";
let version: number = 1.0;

// Type inference (less clear)
let name = "Utah";     // Inferred as string
let version = 1.0;     // Inferred as number
```

## String Interpolation

Use template literals with `${}` for string interpolation:

```typescript
let userName: string = "Alice";
let score: number = 95;
let isWinner: boolean = score > 90;

let message: string = "Hello ${userName}!";
let result: string = "Score: ${score}, Winner: ${isWinner}";
let complex: string = "User ${userName} scored ${score} points and ${isWinner ? \"won\" : \"lost\"}";
```

**Generated bash:**

```bash
userName="Alice"
score="95"
isWinner="true"

message="Hello ${userName}!"
result="Score: ${score}, Winner: ${isWinner}"
if [ "${isWinner}" = "true" ]; then
  complex="User ${userName} scored ${score} points and won"
else
  complex="User ${userName} scored ${score} points and lost"
fi
```

## Variable Scope

### Global Variables

Variables declared at the script level are global:

```typescript
let globalVar: string = "Available everywhere";
const GLOBAL_CONSTANT: number = 42;

function useGlobals(): void {
  console.log(globalVar);        // Accessible
  console.log(GLOBAL_CONSTANT);  // Accessible
}
```

### Function Parameters

Function parameters are local to the function:

```typescript
function greet(name: string, age: number): void {
  // 'name' and 'age' are only available within this function
  let greeting: string = "Hello ${name}, you are ${age} years old";
  console.log(greeting);
}

greet("Alice", 30);
// console.log(name);  // Error: 'name' not available here
```

**Generated bash:**

```bash
greet() {
  local name="$1"
  local age="$2"
  local greeting="Hello ${name}, you are ${age} years old"
  echo "${greeting}"
}
```

## Arrays in Detail

### Array Declaration and Initialization

```typescript
// Different ways to declare arrays
let emptyStrings: string[] = [];
let emptyNumbers: number[] = [];

let languages: string[] = ["JavaScript", "TypeScript", "Utah"];
let versions: number[] = [1, 2, 3, 4, 5];

// Mixed initialization
let mixed: string[] = ["first"];
mixed[1] = "second";
mixed[2] = "third";
```

### Array Access and Properties

```typescript
let items: string[] = ["apple", "banana", "cherry"];

// Access by index
let first: string = items[0];     // "apple"
let second: string = items[1];    // "banana"
let last: string = items[2];      // "cherry"

// Array properties
let length: number = items.length;        // 3
let isEmpty: boolean = items.isEmpty();   // false
```

### Array Methods

```typescript
let numbers: number[] = [3, 1, 4, 1, 5];

// Check if array contains item
let hasThree: boolean = numbers.contains(3);  // true
let hasTen: boolean = numbers.contains(10);   // false

// Reverse array
let reversed: number[] = numbers.reverse();   // [5, 1, 4, 1, 3]

// Join array elements
let joined: string = ["a", "b", "c"].join("-");  // "a-b-c"
```

### Array Iteration

```typescript
let fruits: string[] = ["apple", "banana", "orange"];

// For-in loop (recommended)
for (let fruit: string in fruits) {
  console.log("Fruit: ${fruit}");
}

// Traditional for loop
for (let i: number = 0; i < fruits.length; i++) {
  let fruit: string = fruits[i];
  console.log("Index ${i}: ${fruit}");
}
```

## Working with Objects

Objects in Utah are primarily used for JSON and YAML data:

### JSON Objects

```typescript
// Parse JSON
let userJson: string = "{\"name\": \"Alice\", \"age\": 30, \"admin\": true}";
let user: object = json.parse(userJson);

// Access properties
let name: string = json.get(user, ".name");
let age: number = json.get(user, ".age");
let isAdmin: boolean = json.get(user, ".admin");

// Modify objects
user = json.set(user, ".name", "Bob");
user = json.set(user, ".lastLogin", "2023-12-01");

// Convert back to JSON
let updatedJson: string = json.stringify(user);
```

### YAML Objects

```typescript
// Parse YAML
let configYaml: string = `
database:
  host: localhost
  port: 5432
features:
  - logging
  - monitoring
`;

let config: object = yaml.parse(configYaml);

// Access nested properties
let dbHost: string = yaml.get(config, ".database.host");
let dbPort: number = yaml.get(config, ".database.port");
let firstFeature: string = yaml.get(config, ".features[0]");

// Check property existence
let hasDatabase: boolean = yaml.has(config, ".database");
let hasAuth: boolean = yaml.has(config, ".auth");
```

## Type Conversion

Utah handles type conversions automatically in many cases:

### String to Number

```typescript
let input: string = "42";
let num: number = parseInt(input);     // Manual parsing if needed
let calculated: number = 10 + 5;      // Direct numeric operation
```

### Boolean Conversion

```typescript
let flag: string = "true";
let isTrue: boolean = flag == "true";  // String comparison to boolean

let count: number = 5;
let hasItems: boolean = count > 0;     // Numeric comparison to boolean
```

### Template Literal Conversion

```typescript
let count: number = 42;
let active: boolean = true;

// Automatic conversion in template literals
let message: string = "Count: ${count}, Active: ${active}";
// Result: "Count: 42, Active: true"
```

## Environment Variables

Access environment variables with proper typing:

```typescript
// Get environment variable with default
let dbHost: string = env.get("DB_HOST", "localhost");
let dbPort: number = parseInt(env.get("DB_PORT", "5432"));
let debugMode: boolean = env.get("DEBUG", "false") == "true";

// Use environment variables
console.log("Connecting to ${dbHost}:${dbPort}");
if (debugMode) {
  console.log("Debug mode enabled");
}
```

## Best Practices

### 1. Always Use Type Annotations

```typescript
// Good - explicit and clear
let userName: string = "Alice";
let userCount: number = 42;
let isActive: boolean = true;

// Avoid - unclear types
let userName = "Alice";
let userCount = 42;
let isActive = true;
```

### 2. Use Descriptive Variable Names

```typescript
// Good - self-documenting
let databaseConnectionString: string = "localhost:5432";
let maxRetryAttempts: number = 3;
let isUserAuthenticated: boolean = false;

// Avoid - unclear meaning
let db: string = "localhost:5432";
let max: number = 3;
let auth: boolean = false;
```

### 3. Use Constants for Fixed Values

```typescript
// Good - prevents accidental changes
const API_ENDPOINT: string = "https://api.example.com";
const TIMEOUT_SECONDS: number = 30;
const ENABLE_LOGGING: boolean = true;

// Less safe - might be changed accidentally
let apiEndpoint: string = "https://api.example.com";
```

### 4. Initialize Arrays Explicitly

```typescript
// Good - clear intent
let usernames: string[] = [];
let scores: number[] = [0, 0, 0];

// Clear when adding items
usernames[0] = "alice";
usernames[1] = "bob";
```

### 5. Validate Data Before Use

```typescript
// Good - safe data handling
let configData: string = fs.readFile("config.json");
if (json.isValid(configData)) {
  let config: object = json.parse(configData);
  let host: string = json.get(config, ".database.host");
} else {
  console.log("Invalid configuration file");
  exit(1);
}
```

## Common Patterns

### Configuration Loading

```typescript
const CONFIG_FILE: string = "app.json";
const DEFAULT_PORT: number = 8080;

let port: number = DEFAULT_PORT;
let host: string = "localhost";

if (fs.exists(CONFIG_FILE)) {
  let configContent: string = fs.readFile(CONFIG_FILE);
  if (json.isValid(configContent)) {
    let config: object = json.parse(configContent);

    if (json.has(config, ".port")) {
      port = json.get(config, ".port");
    }

    if (json.has(config, ".host")) {
      host = json.get(config, ".host");
    }
  }
}

console.log("Server will start on ${host}:${port}");
```

### Data Processing Pipeline

```typescript
let inputFiles: string[] = ["data1.json", "data2.json", "data3.json"];
let processedCount: number = 0;
let errors: string[] = [];

for (let file: string in inputFiles) {
  try {
    if (fs.exists(file)) {
      let content: string = fs.readFile(file);
      if (json.isValid(content)) {
        let data: object = json.parse(content);
        // Process data here
        processedCount++;
      } else {
        errors[errors.length] = "Invalid JSON in ${file}";
      }
    } else {
      errors[errors.length] = "File not found: ${file}";
    }
  }
  catch {
    errors[errors.length] = "Error processing ${file}";
  }
}

console.log("Processed ${processedCount} files successfully");
if (errors.length > 0) {
  console.log("Errors encountered:");
  for (let error: string in errors) {
    console.log("- ${error}");
  }
}
```

### User Input Validation

```typescript
args.define("--count", "-c", "Number of items", "number", false, 10);
args.define("--name", "-n", "User name", "string", true);

let count: number = args.get("--count");
let userName: string = args.get("--name");

// Validate input
if (count <= 0) {
  console.log("Count must be positive");
  exit(1);
}

if (userName.length == 0) {
  console.log("Name cannot be empty");
  exit(1);
}

console.log("Processing ${count} items for user: ${userName}");
```

## Next Steps

Now that you understand variables and types in Utah:

1. **[Learn about functions](functions.md)** - Function declarations and parameters
2. **[Explore control flow](control-flow.md)** - Conditionals, loops, and logic
3. **[Study arrays](arrays.md)** - Array manipulation and operations
4. **[Study strings](strings.md)** - String processing and manipulation
5. **[Discover built-in functions](./index.md)** - Leverage Utah's function library
