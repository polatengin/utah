---
layout: default
title: Array Operations
parent: Examples
nav_order: 4
description: "Comprehensive guide to working with arrays in Utah"
permalink: /examples/arrays/
---

A comprehensive guide to working with arrays in Utah, including creation, access, iteration, and built-in array methods. This example demonstrates Utah's powerful array handling capabilities.

## Features Demonstrated

- **Typed array creation** with different data types
- **Array access** using index notation
- **Array iteration** with for-in loops
- **Built-in array methods** for length and manipulation
- **String splitting** to create arrays
- **Array population** from various sources

## Complete Script

```typescript
// Test array type support
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Alice", "Bob", "Charlie"];
let flags: boolean[] = [true, false, true];

// Test array access
let firstNumber: number = numbers[0];
let secondName: string = names[1];

// Test array methods
let length: number = array.length(numbers);

// Test array iteration
for (let num: number in numbers) {
  console.log(num);
}

for (let name: string in names) {
  console.log(name);
}

// Test array from split
let csvData: string = "one,two,three";
let items: string[] = string.split(csvData, ",");

for (let item: string in items) {
  console.log(item);
}
```

## Key Array Concepts

### Typed Array Declaration

Utah supports strongly-typed arrays for different data types:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Alice", "Bob", "Charlie"];
let flags: boolean[] = [true, false, true];
```

This provides:

- **Type Safety**: Ensures array elements are the correct type
- **IntelliSense**: Better editor support and autocomplete
- **Runtime Validation**: Prevents type mismatches

### Array Access

Access array elements using index notation (zero-based):

```typescript
let firstNumber: number = numbers[0];      // Gets 1
let secondName: string = names[1];         // Gets "Bob"
let lastFlag: boolean = flags[2];          // Gets true
```

### Array Methods

Utah provides built-in array methods:

```typescript
let length: number = array.length(numbers);        // Get array length
array.push(numbers, 6);                             // Add element to end
let last: number = array.pop(numbers);              // Remove and return last element
```

### Array Iteration

Use for-in loops for clean iteration:

```typescript
for (let num: number in numbers) {
  console.log(`Number: ${num}`);
}

for (let name: string in names) {
  console.log(`Name: ${name}`);
}
```

## Extended Array Examples

### Working with Mixed Data

```typescript
// Server configuration array
let servers: string[] = ["web01", "web02", "db01", "cache01"];
let ports: number[] = [80, 443, 3306, 6379];
let statuses: boolean[] = [true, true, false, true];

console.log("Server Status Report:");
console.log("===================");

for (let i: number = 0; i < array.length(servers); i++) {
  let server: string = servers[i];
  let port: number = ports[i];
  let isOnline: boolean = statuses[i];
  let status: string = isOnline ? "ONLINE" : "OFFLINE";

  console.log(`${server}:${port} - ${status}`);
}
```

### Array Manipulation

```typescript
// Dynamic array building
let logLevels: string[] = [];
array.push(logLevels, "INFO");
array.push(logLevels, "WARN");
array.push(logLevels, "ERROR");

console.log(`Log levels configured: ${array.length(logLevels)}`);

// Process each log level
for (let level: string in logLevels) {
  console.log(`Processing ${level} level logs...`);
}

// Remove last element
let removed: string = array.pop(logLevels);
console.log(`Removed: ${removed}`);
console.log(`Remaining levels: ${array.length(logLevels)}`);
```

### Creating Arrays from Strings

```typescript
// Process CSV data
let csvData: string = "user1@company.com,user2@company.com,admin@company.com";
let emails: string[] = string.split(csvData, ",");

console.log("Email Processing:");
for (let email: string in emails) {
  let username: string = string.substring(email, 0, string.indexOf(email, "@"));
  let domain: string = string.substring(email, string.indexOf(email, "@") + 1);

  console.log(`User: ${username}, Domain: ${domain}`);
}

// Process space-separated data
let commandLine: string = "utah compile script.shx --verbose --output report.txt";
let args: string[] = string.split(commandLine, " ");

console.log("Command Line Arguments:");
for (let arg: string in args) {
  if (string.startsWith(arg, "--")) {
    console.log(`Option: ${arg}`);
  } else if (string.startsWith(arg, "-")) {
    console.log(`Flag: ${arg}`);
  } else {
    console.log(`Value: ${arg}`);
  }
}
```

### Array-Based Configuration

```typescript
// Environment-specific configuration
let environments: string[] = ["development", "staging", "production"];
let databases: string[] = ["dev_db", "staging_db", "prod_db"];
let memoryLimits: number[] = [512, 1024, 4096];

let currentEnv: string = env.get("ENVIRONMENT") ? env.get("ENVIRONMENT") : "development";

// Find environment index
let envIndex: number = -1;
for (let i: number = 0; i < array.length(environments); i++) {
  if (environments[i] == currentEnv) {
    envIndex = i;
    break;
  }
}

if (envIndex >= 0) {
  let dbName: string = databases[envIndex];
  let memLimit: number = memoryLimits[envIndex];

  console.log(`Environment: ${currentEnv}`);
  console.log(`Database: ${dbName}`);
  console.log(`Memory Limit: ${memLimit}MB`);
} else {
  console.log(`Unknown environment: ${currentEnv}`);
}
```

## Complete Array Method Reference

| Method | Description | Example |
|--------|-------------|---------|
| `array.length(arr)` | Get array length | `array.length([1,2,3])` → `3` |
| `array.push(arr, item)` | Add item to end | `array.push(arr, "new")` |
| `array.pop(arr)` | Remove and return last item | `array.pop([1,2,3])` → `3` |
| `array.join(arr, separator)` | Join array into string | `array.join(["a","b"], ",")` → `"a,b"` |
| `array.contains(arr, item)` | Check if array contains item | `array.contains([1,2], 2)` → `true` |
| `array.indexOf(arr, item)` | Find index of item | `array.indexOf(["a","b"], "b")` → `1` |

## Advanced Array Patterns

### Array Validation

```typescript
let requiredServices: string[] = ["nginx", "mysql", "redis"];
let missingServices: string[] = [];

for (let service: string in requiredServices) {
  if (!os.isInstalled(service)) {
    array.push(missingServices, service);
  }
}

if (array.length(missingServices) > 0) {
  console.log("❌ Missing required services:");
  for (let missing: string in missingServices) {
    console.log(`  - ${missing}`);
  }
} else {
  console.log("✅ All required services are installed");
}
```

### Array-Based Menu System

```typescript
let menuOptions: string[] = [
  "System Health Check",
  "Update System Packages",
  "Backup Configuration",
  "View System Logs",
  "Exit"
];

console.log("Utah Admin Menu:");
console.log("===============");

for (let i: number = 0; i < array.length(menuOptions); i++) {
  let option: string = menuOptions[i];
  console.log(`${i + 1}. ${option}`);
}

// In a real implementation, you'd get user input here
let choice: number = 1; // Simulated user choice

if (choice > 0 && choice <= array.length(menuOptions)) {
  let selected: string = menuOptions[choice - 1];
  console.log(`Executing: ${selected}`);
} else {
  console.log("Invalid choice");
}
```

## Usage Examples

### Basic Array Operations

```bash
utah compile arrays.shx
./arrays.sh
```

### Expected Output

```text
1
2
3
4
5
Alice
Bob
Charlie
one
two
three
```

### With Additional Processing

```typescript
// Extended version with more processing
let numbers: number[] = [1, 2, 3, 4, 5];

console.log("Original array:");
for (let num: number in numbers) {
  console.log(`  ${num}`);
}

// Add more numbers
array.push(numbers, 6);
array.push(numbers, 7);

console.log(`Array length after additions: ${array.length(numbers)}`);

// Process even numbers
console.log("Even numbers:");
for (let num: number in numbers) {
  if (num % 2 == 0) {
    console.log(`  ${num} is even`);
  }
}
```

## Benefits Over Traditional Bash

Utah arrays provide significant advantages over bash arrays:

### Utah Arrays (Clean and Type-Safe)

```typescript
let servers: string[] = ["web01", "web02", "db01"];
let ports: number[] = [80, 443, 3306];

for (let server: string in servers) {
  console.log(`Server: ${server}`);
}

let count: number = array.length(servers);
```

### Bash Arrays (Complex and Error-Prone)

```bash
servers=("web01" "web02" "db01")
ports=(80 443 3306)

for server in "${servers[@]}"; do
  echo "Server: $server"
done

count=${#servers[@]}
```

### Key Advantages

- **Type Safety**: Arrays are strongly typed
- **Clear Syntax**: No complex `[@]` notation
- **Built-in Methods**: Rich set of array manipulation functions
- **Error Prevention**: Type checking prevents common mistakes
- **Better Iteration**: Clean for-in loop syntax
- **Index Safety**: Bounds checking and validation

## Common Patterns

### Configuration Arrays

```typescript
let configFiles: string[] = [
  "/etc/nginx/nginx.conf",
  "/etc/mysql/my.cnf",
  "/etc/redis/redis.conf"
];

for (let configFile: string in configFiles) {
  if (fs.exists(configFile)) {
    console.log(`✅ ${configFile} exists`);
  } else {
    console.log(`❌ ${configFile} missing`);
  }
}
```

### Data Processing

```typescript
let logFiles: string[] = string.split(env.get("LOG_FILES"), ":");

for (let logFile: string in logFiles) {
  let size: number = fs.size(logFile);
  console.log(`${logFile}: ${size} bytes`);
}
```

## Related Examples

- [Loops](loops) - Advanced iteration patterns with arrays
- [String Processing](string-processing) - Working with string arrays
- [System Health Monitor](system-health-monitor) - Arrays in real-world applications
