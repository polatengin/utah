---
layout: default
title: Functions
parent: Language Features
nav_order: 3
---

Utah provides first-class function support with TypeScript-inspired syntax, strong typing, and proper scoping. Functions compile to efficient bash function definitions.

## Function Declaration

### Basic Syntax

Functions are declared with the `function` keyword and require type annotations:

```typescript
function greet(name: string): void {
  console.log(`Hello, ${name}!`);
}

greet("World");
```

**Generated Bash:**

```bash
greet() {
  local name="$1"
  echo "Hello, ${name}!"
}

greet "World"
```

### Return Values

Functions can return typed values:

```typescript
function add(a: number, b: number): number {
  return a + b;
}

let result: number = add(5, 3);
console.log(`Result: ${result}`);
```

**Generated Bash:**

```bash
add() {
  local a="$1"
  local b="$2"
  echo $((a + b))
}

result=$(add 5 3)
echo "Result: ${result}"
```

### String Return Values

```typescript
function getGreeting(name: string): string {
  return `Hello, ${name}!`;
}

let message: string = getGreeting("Alice");
console.log(message);
```

**Generated Bash:**

```bash
getGreeting() {
  local name="$1"
  echo "Hello, ${name}!"
}

message=$(getGreeting "Alice")
echo "${message}"
```

## Parameter Types

### Multiple Parameters

Functions can accept multiple typed parameters:

```typescript
function createUser(name: string, age: number, active: boolean): string {
  let status: string = active ? "active" : "inactive";
  return `User: ${name}, Age: ${age}, Status: ${status}`;
}

let userInfo: string = createUser("Bob", 25, true);
console.log(userInfo);
```

### Array Parameters

Pass arrays as function parameters:

```typescript
function processItems(items: string[]): number {
  let count: number = 0;
  for (let item: string in items) {
    console.log(`Processing: ${item}`);
    count++;
  }
  return count;
}

let files: string[] = ["file1.txt", "file2.txt", "file3.txt"];
let processed: number = processItems(files);
console.log(`Processed ${processed} items`);
```

**Generated Bash:**

```bash
processItems() {
  local items=("$@")
  local count=0
  for item in "${items[@]}"; do
    echo "Processing: ${item}"
    ((count++))
  done
  echo "${count}"
}

files=("file1.txt" "file2.txt" "file3.txt")
processed=$(processItems "${files[@]}")
echo "Processed ${processed} items"
```

## Local Variables and Scoping

### Local Variable Scope

Variables declared inside functions are automatically scoped locally:

```typescript
let globalVar: string = "global";

function testScope(): void {
  let localVar: string = "local";
  globalVar = "modified global";
  console.log(`Local: ${localVar}`);
  console.log(`Global: ${globalVar}`);
}

testScope();
console.log(`Outside function: ${globalVar}`);
```

**Generated Bash:**

```bash
globalVar="global"

testScope() {
  local localVar="local"
  globalVar="modified global"
  echo "Local: ${localVar}"
  echo "Global: ${globalVar}"
}

testScope
echo "Outside function: ${globalVar}"
```

### Variable Shadowing

Local variables can shadow global ones:

```typescript
let name: string = "Global";

function showName(): void {
  let name: string = "Local";
  console.log(`Inside function: ${name}`);
}

showName();
console.log(`Outside function: ${name}`);
```

## Function Composition

### Calling Functions from Functions

Functions can call other functions:

```typescript
function formatName(first: string, last: string): string {
  return `${first} ${last}`;
}

function createWelcome(first: string, last: string): string {
  let fullName: string = formatName(first, last);
  return `Welcome, ${fullName}!`;
}

let welcome: string = createWelcome("John", "Doe");
console.log(welcome);
```

### Helper Functions

Break complex logic into smaller functions:

```typescript
function isValidEmail(email: string): boolean {
  return email.contains("@") && email.contains(".");
}

function isValidAge(age: number): boolean {
  return age >= 0 && age <= 150;
}

function validateUser(email: string, age: number): boolean {
  if (!isValidEmail(email)) {
    console.log("Invalid email format");
    return false;
  }

  if (!isValidAge(age)) {
    console.log("Invalid age");
    return false;
  }

  return true;
}

if (validateUser("user@example.com", 25)) {
  console.log("User is valid");
}
```

## Advanced Function Patterns

### Early Return Pattern

Use early returns to reduce nesting:

```typescript
function processFile(filename: string): boolean {
  if (filename == "") {
    console.log("Filename cannot be empty");
    return false;
  }

  if (!fs.exists(filename)) {
    console.log(`File not found: ${filename}`);
    return false;
  }

  let content: string = fs.readFile(filename);
  if (content == "") {
    console.log("File is empty");
    return false;
  }

  console.log(`Processing file: ${filename}`);
  return true;
}
```

### Factory Functions

Functions that create and return data:

```typescript
function createConfig(env: string): object {
  let baseConfig: object = json.parse('{"version": "1.0"}');

  if (env == "development") {
    baseConfig = json.set(baseConfig, ".debug", true);
    baseConfig = json.set(baseConfig, ".port", 3000);
  } else if (env == "production") {
    baseConfig = json.set(baseConfig, ".debug", false);
    baseConfig = json.set(baseConfig, ".port", 80);
  }

  return baseConfig;
}

let devConfig: object = createConfig("development");
let prodConfig: object = createConfig("production");
```

### Transform Functions

Functions that process and transform data:

```typescript
function normalizeFilename(filename: string): string {
  // Remove invalid characters
  filename = filename.replace(" ", "_");
  filename = filename.replace("(", "");
  filename = filename.replace(")", "");

  // Convert to lowercase
  filename = filename.toLowerCase();

  return filename;
}

function processFiles(files: string[]): string[] {
  let normalized: string[] = [];

  for (let file: string in files) {
    let clean: string = normalizeFilename(file);
    normalized[normalized.length] = clean;
  }

  return normalized;
}

let originalFiles: string[] = ["My Document (1).txt", "Photo Album.jpg"];
let cleanFiles: string[] = processFiles(originalFiles);
```

## Error Handling in Functions

### Return Error Codes

Use return values to indicate success or failure:

```typescript
function backupFile(source: string, dest: string): number {
  if (!fs.exists(source)) {
    console.log(`Source file not found: ${source}`);
    return 1;
  }

  if (fs.exists(dest)) {
    console.log(`Destination already exists: ${dest}`);
    return 2;
  }

  console.log(`Backing up ${source} to ${dest}`);
  // Copy logic would go here
  return 0;
}

let result: number = backupFile("data.txt", "backup/data.txt");
if (result != 0) {
  console.log(`Backup failed with error code: ${result}`);
  exit(result);
}
```

### Void Functions with Side Effects

Functions that perform actions without returning values:

```typescript
function logMessage(level: string, message: string): void {
  let timestamp: string = timer.current();
  let logEntry: string = `[${timestamp}] ${level}: ${message}`;

  console.log(logEntry);

  // Also write to log file
  fs.appendFile("app.log", logEntry + "\n");
}

function logError(message: string): void {
  logMessage("ERROR", message);
}

function logInfo(message: string): void {
  logMessage("INFO", message);
}

logInfo("Application started");
logError("Database connection failed");
```

## Practical Examples

### Configuration Parser

```typescript
function parseConfigFile(filename: string): object {
  if (!fs.exists(filename)) {
    console.log(`Config file not found: ${filename}, using defaults`);
    return json.parse('{"port": 8080, "debug": false}');
  }

  let content: string = fs.readFile(filename);
  if (!json.isValid(content)) {
    console.log("Invalid JSON in config file, using defaults");
    return json.parse('{"port": 8080, "debug": false}');
  }

  return json.parse(content);
}

function getConfigValue(config: object, key: string, defaultValue: string): string {
  if (json.has(config, key)) {
    return json.get(config, key);
  }
  return defaultValue;
}

// Usage
let config: object = parseConfigFile("app.json");
let port: string = getConfigValue(config, ".port", "8080");
let debug: string = getConfigValue(config, ".debug", "false");

console.log(`Server will run on port ${port} with debug=${debug}`);
```

### File Processing Pipeline

```typescript
function validateFile(filename: string): boolean {
  if (!fs.exists(filename)) {
    console.log(`File not found: ${filename}`);
    return false;
  }

  let size: number = `$(stat -c%s "${filename}")`;
  if (size == 0) {
    console.log(`File is empty: ${filename}`);
    return false;
  }

  return true;
}

function processTextFile(filename: string): string {
  let content: string = fs.readFile(filename);

  // Clean up the content
  content = content.trim();
  content = content.replace("\r\n", "\n");

  return content;
}

function saveProcessedFile(content: string, outputFile: string): boolean {
  try {
    fs.writeFile(outputFile, content);
    console.log(`Saved processed content to: ${outputFile}`);
    return true;
  }
  catch {
    console.log(`Failed to save file: ${outputFile}`);
    return false;
  }
}

function processFilesPipeline(inputFiles: string[]): void {
  for (let file: string in inputFiles) {
    console.log(`Processing: ${file}`);

    if (!validateFile(file)) {
      continue;
    }

    let content: string = processTextFile(file);
    let outputFile: string = `processed_${file}`;

    if (saveProcessedFile(content, outputFile)) {
      console.log(`Successfully processed: ${file}`);
    } else {
      console.log(`Failed to process: ${file}`);
    }
  }
}

// Usage
let files: string[] = ["data1.txt", "data2.txt", "data3.txt"];
processFilesPipeline(files);
```

### System Information Collector

```typescript
function getSystemInfo(): object {
  let info: object = json.parse('{}');

  // Operating system
  if (os.isInstalled("uname")) {
    let osName: string = `$(uname -s)`;
    info = json.set(info, ".os", osName);
  }

  // Memory info
  if (fs.exists("/proc/meminfo")) {
    let memTotal: string = `$(grep MemTotal /proc/meminfo | awk '{print $2}')`;
    info = json.set(info, ".memory_kb", memTotal);
  }

  // Disk space
  if (os.isInstalled("df")) {
    let diskUsage: string = `$(df -h / | tail -1 | awk '{print $5}')`;
    info = json.set(info, ".disk_usage", diskUsage);
  }

  return info;
}

function formatSystemInfo(info: object): void {
  console.log("=== System Information ===");

  if (json.has(info, ".os")) {
    let os: string = json.get(info, ".os");
    console.log(`Operating System: ${os}`);
  }

  if (json.has(info, ".memory_kb")) {
    let memKb: string = json.get(info, ".memory_kb");
    let memMb: number = memKb / 1024;
    console.log(`Total Memory: ${memMb} MB`);
  }

  if (json.has(info, ".disk_usage")) {
    let usage: string = json.get(info, ".disk_usage");
    console.log(`Disk Usage: ${usage}`);
  }
}

// Usage
let systemInfo: object = getSystemInfo();
formatSystemInfo(systemInfo);
```

### Deployment Helper

```typescript
function checkPrerequisites(): boolean {
  let required: string[] = ["git", "docker", "curl"];
  let missing: string[] = [];

  for (let tool: string in required) {
    if (!os.isInstalled(tool)) {
      missing[missing.length] = tool;
    }
  }

  if (missing.length > 0) {
    console.log("Missing required tools:");
    for (let tool: string in missing) {
      console.log(`  - ${tool}`);
    }
    return false;
  }

  return true;
}

function deployApplication(version: string, environment: string): boolean {
  console.log(`Starting deployment of version ${version} to ${environment}`);

  if (!checkPrerequisites()) {
    console.log("Prerequisites check failed");
    return false;
  }

  // Validate environment
  let validEnvs: string[] = ["dev", "staging", "prod"];
  if (!validEnvs.contains(environment)) {
    console.log(`Invalid environment: ${environment}`);
    return false;
  }

  console.log("Building Docker image...");
  let buildResult: number = `$(docker build -t myapp:${version} .)`;
  if (buildResult != 0) {
    console.log("Docker build failed");
    return false;
  }

  console.log("Deploying to environment...");
  // Deployment logic would go here

  console.log(`Deployment completed successfully`);
  return true;
}

// Usage
args.define("--version", "-v", "Version to deploy", "string", true);
args.define("--env", "-e", "Target environment", "string", true);

let version: string = args.getString("--version");
let environment: string = args.getString("--env");

if (!deployApplication(version, environment)) {
  console.log("Deployment failed");
  exit(1);
}
```

## Best Practices

### 1. Use Clear Function Names

```typescript
// Good - descriptive names
function validateEmailAddress(email: string): boolean { }
function calculateTotalPrice(items: object[]): number { }

// Avoid - unclear names
function check(data: string): boolean { }
function calc(items: object[]): number { }
```

### 2. Keep Functions Focused

```typescript
// Good - single responsibility
function readConfigFile(filename: string): string {
  return fs.readFile(filename);
}

function parseJsonConfig(content: string): object {
  return json.parse(content);
}

function validateConfig(config: object): boolean {
  return json.has(config, ".version") && json.has(config, ".database");
}

// Avoid - doing too much
function loadAndValidateConfig(filename: string): object {
  let content: string = fs.readFile(filename);
  let config: object = json.parse(content);
  if (!json.has(config, ".version")) {
    exit(1);
  }
  return config;
}
```

### 3. Handle Edge Cases

```typescript
function divideNumbers(a: number, b: number): number {
  if (b == 0) {
    console.log("Error: Division by zero");
    return 0;
  }
  return a / b;
}

function getArrayElement(arr: string[], index: number): string {
  if (index < 0 || index >= arr.length) {
    console.log("Error: Array index out of bounds");
    return "";
  }
  return arr[index];
}
```

### 4. Use Type Annotations

```typescript
// Good - explicit types
function processUserData(name: string, age: number, active: boolean): object {
  return json.parse(`{"name": "${name}", "age": ${age}, "active": ${active}}`);
}

// Required - Utah enforces type annotations
function calculateScore(points: number[], multiplier: number): number {
  let total: number = 0;
  for (let point: number in points) {
    total += point * multiplier;
  }
  return total;
}
```

Functions are essential building blocks in Utah that enable code reuse, better organization, and maintainable scripts. With proper typing and scoping, they provide a robust foundation for complex automation tasks.
