---
layout: default
title: Control Flow
parent: Language Features
nav_order: 2
---

Utah provides modern control flow structures that compile to efficient bash code. All control structures use familiar TypeScript-like syntax with proper type checking.

## Conditional Statements

### If Statements

Basic if statements with comparison operators:

```typescript
let temperature: number = 25;

if (temperature > 30) {
  console.log("It's hot outside");
}
```

**Generated Bash:**

```bash
temperature=25
if [ "${temperature}" -gt 30 ]; then
  echo "It's hot outside"
fi
```

### If-Else Statements

Full conditional branching:

```typescript
let score: number = 85;

if (score >= 90) {
  console.log("Excellent!");
} else if (score >= 80) {
  console.log("Good job!");
} else if (score >= 70) {
  console.log("Not bad");
} else {
  console.log("Keep trying");
}
```

### String Comparisons

String equality and pattern matching:

```typescript
let environment: string = "production";

if (environment == "production") {
  console.log("Running in production mode");
} else if (environment == "development") {
  console.log("Running in development mode");
} else {
  console.log("Unknown environment");
}
```

### Boolean Logic

Combine conditions with logical operators:

```typescript
let hasPermission: boolean = true;
let isLoggedIn: boolean = true;
let attempts: number = 3;

if (isLoggedIn && hasPermission) {
  console.log("Access granted");
} else if (!isLoggedIn) {
  console.log("Please log in first");
} else if (attempts < 5) {
  console.log("Permission denied, but you can try again");
} else {
  console.log("Too many attempts");
}
```

## Switch Statements

Switch statements provide clean multi-way branching:

### Basic Switch

```typescript
let action: string = "create";

switch (action) {
  case "create":
    console.log("Creating new file");
    break;
  case "delete":
    console.log("Deleting file");
    break;
  case "update":
    console.log("Updating file");
    break;
  default:
    console.log("Unknown action");
}
```

**Generated Bash:**

```bash
action="create"
case "${action}" in
  "create")
    echo "Creating new file"
    ;;
  "delete")
    echo "Deleting file"
    ;;
  "update")
    echo "Updating file"
    ;;
  *)
    echo "Unknown action"
    ;;
esac
```

### Switch with Fall-through

Omit `break` statements for fall-through behavior:

```typescript
let fileType: string = "jpg";

switch (fileType) {
  case "jpg":
  case "jpeg":
    console.log("JPEG image format");
    break;
  case "png":
  case "gif":
    console.log("Lossless image format");
    break;
  default:
    console.log("Unknown image format");
}
```

## Loop Statements

### For Loops

Traditional C-style for loops:

```typescript
for (let i: number = 0; i < 10; i++) {
  console.log("Iteration ${i}");
}
```

**Generated Bash:**

```bash
for ((i=0; i<10; i++)); do
  echo "Iteration ${i}"
done
```

### For-In Loops

Iterate over arrays and strings:

```typescript
let fruits: string[] = ["apple", "banana", "orange"];

for (let fruit: string in fruits) {
  console.log("Processing: ${fruit}");
}
```

**Generated Bash:**

```bash
fruits=("apple" "banana" "orange")
for fruit in "${fruits[@]}"; do
  echo "Processing: ${fruit}"
done
```

### While Loops

Conditional iteration:

```typescript
let count: number = 0;

while (count < 5) {
  console.log("Count is: ${count}");
  count++;
}
```

**Generated Bash:**

```bash
count=0
while [ "${count}" -lt 5 ]; do
  echo "Count is: ${count}"
  ((count++))
done
```

### Loop Control

Use `break` to exit loops early:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

for (let num: number in numbers) {
  if (num > 5) {
    break;
  }
  console.log("Number: ${num}");
}
```

## Comparison Operators

Utah supports various comparison operators:

### Numeric Comparisons

```typescript
let a: number = 10;
let b: number = 20;

if (a == b) { console.log("Equal"); }
if (a != b) { console.log("Not equal"); }
if (a < b) { console.log("Less than"); }
if (a <= b) { console.log("Less than or equal"); }
if (a > b) { console.log("Greater than"); }
if (a >= b) { console.log("Greater than or equal"); }
```

### String Equality Examples

```typescript
let name1: string = "Alice";
let name2: string = "Bob";

if (name1 == name2) { console.log("Same name"); }
if (name1 != name2) { console.log("Different names"); }
```

### Boolean Operations

```typescript
let isActive: boolean = true;
let isVerified: boolean = false;

if (isActive) { console.log("Active"); }
if (!isVerified) { console.log("Not verified"); }
if (isActive && isVerified) { console.log("Active and verified"); }
if (isActive || isVerified) { console.log("Active or verified"); }
```

## Ternary Operator

Conditional expressions for simple branching:

```typescript
let age: number = 20;
let status: string = (age >= 18) ? "adult" : "minor";
console.log("Status: ${status}");
```

**Generated Bash:**

```bash
age=20
if [ "${age}" -ge 18 ]; then
  status="adult"
else
  status="minor"
fi
echo "Status: ${status}"
```

## Nested Conditions

Complex conditional logic:

```typescript
let userType: string = "admin";
let permissions: string[] = ["read", "write", "delete"];

if (userType == "admin") {
  if (array.contains(permissions, "delete")) {
    console.log("Admin with delete permission");
  } else {
    console.log("Admin without delete permission");
  }
} else if (userType == "user") {
  if (array.contains(permissions, "write")) {
    console.log("User with write permission");
  } else {
    console.log("Read-only user");
  }
} else {
  console.log("Unknown user type");
}
```

## Practical Examples

### File Processing

```typescript
let files: string[] = ["config.json", "data.txt", "backup.sql"];

for (let file: string in files) {
  if (fs.exists(file)) {
    let extension: string = fs.extension(file);

    switch (extension) {
      case "json":
        console.log("Processing JSON file: ${file}");
        // Process JSON
        break;
      case "txt":
        console.log("Processing text file: ${file}");
        // Process text
        break;
      case "sql":
        console.log("Processing SQL file: ${file}");
        // Process SQL
        break;
      default:
        console.log("Unknown file type: ${file}");
    }
  } else {
    console.log("File not found: ${file}");
  }
}
```

### User Input Validation

```typescript
args.define("--port", "-p", "Server port", "number", false, 8080);
args.define("--environment", "-e", "Environment", "string", false, "development");

let port: number = args.getNumber("--port");
let env: string = args.getString("--environment");

// Validate port
if (port < 1 || port > 65535) {
  console.log("Error: Port must be between 1 and 65535");
  exit(1);
}

// Validate environment
let validEnvs: string[] = ["development", "staging", "production"];
if (!array.contains(validEnvs, env)) {
  console.log("Error: Invalid environment. Must be one of: development, staging, production");
  exit(1);
}

console.log("Starting server on port ${port} in ${env} mode");
```

### Service Health Check

```typescript
let services: string[] = ["nginx", "mysql", "redis"];
let healthyServices: string[] = [];
let unhealthyServices: string[] = [];

for (let service: string in services) {
  console.log("Checking ${service}...");

  if (os.isInstalled("systemctl")) {
    // Use systemctl for service status
    let status: string = "$(systemctl is-active ${service})";

    if (status == "active") {
      healthyServices[healthyServices.length] = service;
      console.log("✓ ${service} is running");
    } else {
      unhealthyServices[unhealthyServices.length] = service;
      console.log("✗ ${service} is not running");
    }
  } else {
    console.log("Cannot check ${service}: systemctl not available");
  }
}

console.log(`\nHealth Check Summary:`);
console.log("Healthy services: ${healthyServices.length}");
console.log("Unhealthy services: ${unhealthyServices.length}");

if (unhealthyServices.length > 0) {
  console.log("Services need attention:");
  for (let service: string in unhealthyServices) {
    console.log("  - ${service}");
  }
  exit(1);
} else {
  console.log("All services are healthy!");
}
```

### Configuration Management

```typescript
let configFile: string = "app.json";
let defaultConfig: string = "{\"debug\": false, \"port\": 3000, \"timeout\": 30}";

// Load configuration
let config: object;
if (fs.exists(configFile)) {
  try {
    let configData: string = fs.readFile(configFile);
    if (json.isValid(configData)) {
      config = json.parse(configData);
      console.log("Configuration loaded from file");
    } else {
      console.log("Invalid JSON in config file, using defaults");
      config = json.parse(defaultConfig);
    }
  }
  catch {
    console.log("Error reading config file, using defaults");
    config = json.parse(defaultConfig);
  }
} else {
  console.log("Config file not found, using defaults");
  config = json.parse(defaultConfig);
}

// Extract and validate configuration values
let debug: boolean = json.getBool(config, ".debug");
let port: number = json.getNumber(config, ".port");
let timeout: number = json.getNumber(config, ".timeout");

// Validate values
if (port < 1 || port > 65535) {
  console.log("Warning: Invalid port in config, using default 3000");
  port = 3000;
}

if (timeout < 1 || timeout > 300) {
  console.log("Warning: Invalid timeout in config, using default 30");
  timeout = 30;
}

console.log("Starting with config: debug=${debug}, port=${port}, timeout=${timeout}");
```

## Best Practices

### 1. Use Explicit Type Comparisons

```typescript
// Good - explicit type checking
if (count == 0) {
  console.log("No items");
}

// Good - boolean variables
if (isEnabled) {
  console.log("Feature enabled");
}

// Avoid - implicit truthiness
if (count) {  // This might not work as expected
  console.log("Has items");
}
```

### 2. Validate Input Early

```typescript
function processFile(filename: string): void {
  // Validate input first
  if (filename == "") {
    console.log("Error: Filename cannot be empty");
    return;
  }

  if (!fs.exists(filename)) {
    console.log("Error: File not found: ${filename}");
    return;
  }

  // Process the file
  console.log("Processing: ${filename}");
}
```

### 3. Use Switch for Multiple Conditions

```typescript
// Good - clear and maintainable
switch (operation) {
  case "create":
    createFile();
    break;
  case "delete":
    deleteFile();
    break;
  case "update":
    updateFile();
    break;
  default:
    console.log("Unknown operation");
}

// Avoid - nested if-else for many conditions
if (operation == "create") {
  createFile();
} else if (operation == "delete") {
  deleteFile();
} else if (operation == "update") {
  updateFile();
} else {
  console.log("Unknown operation");
}
```

### 4. Handle Edge Cases

```typescript
let items: string[] = ["a", "b", "c"];

// Check array is not empty before processing
if (items.length > 0) {
  for (let item: string in items) {
    console.log("Processing: ${item}");
  }
} else {
  console.log("No items to process");
}
```

## Common Patterns

### Early Return Pattern

```typescript
function validateConfig(config: object): boolean {
  if (!json.has(config, ".version")) {
    console.log("Missing version in config");
    return false;
  }

  if (!json.has(config, ".database")) {
    console.log("Missing database config");
    return false;
  }

  let version: string = json.get(config, ".version");
  if (version == "") {
    console.log("Empty version in config");
    return false;
  }

  console.log("Config validation passed");
  return true;
}
```

### Guard Clauses

```typescript
function backupDatabase(dbName: string): void {
  // Guard clauses at the top
  if (dbName == "") {
    console.log("Database name required");
    return;
  }

  if (!os.isInstalled("mysqldump")) {
    console.log("mysqldump not available");
    return;
  }

  if (!fs.exists("/var/backups")) {
    console.log("Backup directory not found");
    return;
  }

  // Main logic here
  console.log("Backing up database: ${dbName}");
}
```

### State Machine Pattern

```typescript
let state: string = "init";
let running: boolean = true;

while (running) {
  switch (state) {
    case "init":
      console.log("Initializing...");
      state = "loading";
      break;

    case "loading":
      console.log("Loading data...");
      state = "processing";
      break;

    case "processing":
      console.log("Processing data...");
      state = "complete";
      break;

    case "complete":
      console.log("Processing complete");
      running = false;
      break;

    default:
      console.log("Unknown state");
      running = false;
  }
}
```

Control flow is the foundation of program logic in Utah. These structures provide the building blocks for creating sophisticated automation scripts while maintaining the readability and type safety that Utah is designed to provide.
