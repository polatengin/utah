---
layout: default
title: Built-in Functions
nav_order: 4
has_children: true
---

Utah provides a comprehensive library of built-in functions organized into logical namespaces. These functions enable you to perform common tasks without writing complex bash code.

## Function Categories

### Data Processing

- **[JSON and YAML](json-yaml.md)** - Parse, manipulate, and process JSON/YAML data
- **[String Functions](strings.md)** - String manipulation and text processing
- **[Array Functions](arrays.md)** - Array operations and data structures

### System Integration

- **[File System](filesystem.md)** - File and directory operations
- **[Operating System](system.md)** - OS information and process management
- **[Web and API](web.md)** - HTTP requests and web service integration

### Development Tools

- **[Git Utilities](git.md)** - Version control operations
- **[Console Functions](console.md)** - Terminal interaction and user input
- **[Utility Functions](utilities.md)** - Random numbers, timers, and helpers

## Function Naming Convention

Utah functions follow a consistent naming pattern:

```typescript
namespace.functionName(parameters)
```

Examples:

- `json.parse(jsonString)` - JSON namespace, parse function
- `fs.readFile(filePath)` - File system namespace, readFile function
- `os.isInstalled(appName)` - Operating system namespace, isInstalled function

## Type Safety

All Utah functions include type annotations:

```typescript
// Function with typed parameters and return value
function greet(name: string): void {
  console.log(`Hello, ${name}!`);
}

// Built-in function usage with types
let content: string = fs.readFile("config.txt");
let isValid: boolean = json.isValid(content);
let randomNum: number = utility.random(1, 100);
```

## Error Handling

Utah functions integrate with try/catch blocks:

```typescript
try {
  let data: string = fs.readFile("important.txt");
  let config: object = json.parse(data);
  console.log("Success: Configuration loaded");
}
catch {
  console.log("Error: Failed to load configuration");
  exit(1);
}
```

## Dependency Management

Some functions require external tools. Utah provides automatic dependency installation:

```typescript
// Ensure JSON processing tools are available
json.installDependencies();

// Ensure YAML processing tools are available
yaml.installDependencies();

// Manual dependency check
if (!os.isInstalled("git")) {
  console.log("Git is required for this script");
  exit(1);
}
```

## Function Composition

Utah functions can be composed and chained:

```typescript
// Chain file operations
let configPath: string = fs.path("config", "app.json");
if (fs.exists(configPath)) {
  let content: string = fs.readFile(configPath);
  let config: object = json.parse(content);
  let appName: string = json.get(config, ".name");
  console.log(`Application: ${appName}`);
}

// Compose with control flow
let files: string[] = ["config.json", "secrets.json", "manifest.yaml"];
for (let file: string in files) {
  if (fs.exists(file)) {
    let ext: string = fs.extension(file);
    if (ext == ".json") {
      let content: string = fs.readFile(file);
      if (json.isValid(content)) {
        console.log(`Valid JSON: ${file}`);
      }
    }
  }
}
```

## Best Practices

### 1. Check Dependencies First

```typescript
// Good - verify tools are available
if (!os.isInstalled("jq")) {
  json.installDependencies();
}

// Then use functions safely
let data: object = json.parse(jsonString);
```

### 2. Handle Errors Gracefully

```typescript
// Good - wrap risky operations
try {
  let result: string = web.get("https://api.example.com/data");
  console.log("API call successful");
}
catch {
  console.log("API call failed, using fallback");
  let result: string = fs.readFile("fallback.json");
}
```

### 3. Use Type Annotations

```typescript
// Good - explicit types
let fileName: string = fs.filename("/path/to/file.txt");
let fileExists: boolean = fs.exists(fileName);

// Less clear
let fileName = fs.filename("/path/to/file.txt");
let fileExists = fs.exists(fileName);
```

### 4. Validate Input Data

```typescript
// Good - validate before processing
if (json.isValid(inputData)) {
  let obj: object = json.parse(inputData);
  // Process safely
} else {
  console.log("Invalid input data");
}
```

## Function Categories Overview

### Essential Functions

Start with these commonly used functions:

```typescript
// Console output
console.log("Message");

// File operations
let exists: boolean = fs.exists("file.txt");
let content: string = fs.readFile("file.txt");

// String manipulation
let upper: string = "hello".toUpperCase();
let parts: string[] = "a,b,c".split(",");

// JSON processing
let valid: boolean = json.isValid('{"key": "value"}');
```

### System Information

Get information about the runtime environment:

```typescript
// Operating system
let osType: string = os.getOS();
let gitInstalled: boolean = os.isInstalled("git");

// Process information
let processId: number = process.id();
let cpuUsage: number = process.cpu();

// Console capabilities
let hasSudo: boolean = console.isSudo();
```

### Data Processing Examples

Work with structured data:

```typescript
// JSON manipulation
let config: object = json.parse(jsonString);
let value: string = json.get(config, ".database.host");

// YAML processing
let manifest: object = yaml.parse(yamlString);
let replicas: number = yaml.get(manifest, ".spec.replicas");

// Array operations
let items: string[] = ["a", "b", "c"];
let length: number = items.length;
let reversed: string[] = items.reverse();
```

### Web and Network

Interact with web services:

```typescript
// HTTP requests
let response: string = web.get("https://api.example.com/data");

// Process API responses
if (json.isValid(response)) {
  let data: object = json.parse(response);
  // Process data
}
```

## Next Steps

Explore specific function categories:

1. **[JSON and YAML Processing](json-yaml.md)** - Essential for configuration and data
2. **[File System Operations](filesystem.md)** - File and directory management
3. **[System Functions](system.md)** - OS integration and process management
4. **[Web Functions](web.md)** - HTTP requests and API integration

## Getting Help

- **Function Reference**: Browse individual function documentation
- **Examples**: Check practical examples in each function category
- **Community**: [Ask questions on GitHub Discussions](https://github.com/polatengin/utah/discussions)
