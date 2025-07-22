---
layout: default
title: Error Handling
parent: Language Features
nav_order: 5
---

Utah provides robust error handling mechanisms that make shell scripts more reliable and maintainable.

## Try-Catch Blocks

### Basic Syntax

```typescript
try {
  // Code that might fail
  let data: string = fs.readFile("config.json");
  console.log("File loaded successfully");
}
catch {
  // Error handling
  console.log("Failed to load config file");
}
```

### Exception Information

Access error details in catch blocks:

```typescript
try {
  let result: string = `$(curl -f https://api.example.com/data)`;
}
catch (error) {
  console.log(`API call failed: ${error}`);
  // Handle the specific error
}
```

## Error Propagation

### Function Error Handling

Functions can propagate errors to calling code:

```typescript
function loadConfig(filename: string): object {
  try {
    let content: string = fs.readFile(filename);
    return json.parse(content);
  }
  catch {
    console.log(`Cannot load config from ${filename}`);
    throw "Config load failed";
  }
}

try {
  let config: object = loadConfig("app.json");
  console.log("Configuration loaded");
}
catch {
  console.log("Using default configuration");
  // Fallback logic
}
```

### Graceful Degradation

```typescript
function getServerInfo(): object {
  try {
    let info: string = `$(systemctl status nginx)`;
    return { status: "active", details: info };
  }
  catch {
    console.log("Could not get server status");
    return { status: "unknown", details: "" };
  }
}
```

## Exit Codes and Script Termination

### Controlled Exit

```typescript
script.exitOnError(false);  // Don't exit on first error

try {
  // Critical operation
  let backup: string = `$(rsync -av /data/ /backup/)`;
}
catch {
  console.log("Backup failed - exiting");
  exit(1);
}
```

### Exit Code Conventions

```typescript
// Success
exit(0);

// General error
exit(1);

// Misuse of shell command
exit(2);

// Permission denied
exit(77);

// Command not found
exit(127);
```

## Error Context and Logging

### Structured Error Information

```typescript
function processFile(filename: string): boolean {
  try {
    if (!fs.exists(filename)) {
      throw `File not found: ${filename}`;
    }

    let content: string = fs.readFile(filename);
    if (string.length(content) == 0) {
      throw `File is empty: ${filename}`;
    }

    // Process the file
    return true;
  }
  catch (error) {
    console.log(`Error processing ${filename}: ${error}`);
    return false;
  }
}
```

### Error Logging with Timestamps

```typescript
function logError(message: string): void {
  let timestamp: string = timer.current();
  console.log(`[${timestamp}] ERROR: ${message}`);
}

try {
  let data: string = `$(wget -q -O - https://api.service.com/health)`;
}
catch {
  logError("Health check API is unreachable");
}
```

## Validation and Defensive Programming

### Input Validation

```typescript
function deployApplication(environment: string, version: string): void {
  // Validate environment
  let validEnvs: string[] = ["dev", "staging", "prod"];
  if (!array.contains(validEnvs, environment)) {
    throw `Invalid environment: ${environment}`;
  }

  // Validate version format
  if (!string.contains(version, ".")) {
    throw `Invalid version format: ${version}`;
  }

  console.log(`Deploying ${version} to ${environment}`);
}

try {
  deployApplication("prod", "1.2.3");
}
catch (error) {
  console.log(`Deployment failed: ${error}`);
  exit(1);
}
```

### Dependency Checking

```typescript
function ensureDependencies(): void {
  let requiredTools: string[] = ["docker", "kubectl", "helm"];

  for (let tool: string in requiredTools) {
    if (!os.isInstalled(tool)) {
      throw `Required tool not found: ${tool}`;
    }
  }
}

try {
  ensureDependencies();
  console.log("All dependencies are available");
}
catch (error) {
  console.log(`Dependency check failed: ${error}`);
  console.log("Please install missing tools and try again");
  exit(1);
}
```

## Recovery Strategies

### Retry Logic

```typescript
function retryOperation(maxAttempts: number, operation: string): boolean {
  for (let attempt: number = 1; attempt <= maxAttempts; attempt++) {
    try {
      let result: string = `$(${operation})`;
      console.log("Operation succeeded");
      return true;
    }
    catch {
      console.log(`Attempt ${attempt} failed`);
      if (attempt < maxAttempts) {
        console.log("Retrying in 5 seconds...");
        timer.sleep(5000);
      }
    }
  }

  console.log("All retry attempts failed");
  return false;
}

if (!retryOperation(3, "curl -f https://api.example.com/health")) {
  console.log("Service is unavailable");
  exit(1);
}
```

### Fallback Mechanisms

```typescript
function getConfiguration(): object {
  // Try primary config source
  try {
    let config: string = fs.readFile("/etc/app/config.json");
    return json.parse(config);
  }
  catch {
    console.log("Primary config not found, trying backup");
  }

  // Try backup config source
  try {
    let config: string = fs.readFile("/tmp/backup-config.json");
    return json.parse(config);
  }
  catch {
    console.log("Backup config not found, using defaults");
  }

  // Use default configuration
  return json.parse('{"timeout": 30, "retries": 3}');
}
```

## Error Handling Patterns

### Resource Cleanup

```typescript
function processWithCleanup(inputFile: string): void {
  let tempFile: string = `/tmp/processing-${timer.current()}`;

  try {
    // Create temporary file
    fs.writeFile(tempFile, "processing data");

    // Process data
    let result: string = `$(process-data ${inputFile} ${tempFile})`;
    console.log("Processing completed");
  }
  catch (error) {
    console.log(`Processing failed: ${error}`);
  }
  finally {
    // Cleanup temporary file
    if (fs.exists(tempFile)) {
      fs.remove(tempFile);
    }
  }
}
```

### Circuit Breaker Pattern

```typescript
let failureCount: number = 0;
const MAX_FAILURES: number = 5;

function callExternalService(): boolean {
  if (failureCount >= MAX_FAILURES) {
    console.log("Circuit breaker: service calls suspended");
    return false;
  }

  try {
    let response: string = `$(curl -f --connect-timeout 10 https://api.service.com)`;
    failureCount = 0;  // Reset on success
    return true;
  }
  catch {
    failureCount++;
    console.log(`Service call failed (${failureCount}/${MAX_FAILURES})`);
    return false;
  }
}
```

## Script-Level Error Configuration

### Error Handling Modes

```typescript
// Exit immediately on any error
script.exitOnError(true);

// Continue on errors (default behavior)
script.exitOnError(false);

// Enable debug mode for error tracking
script.enableDebug(true);
```

### Global Error Handler

```typescript
script.description("Data processing with comprehensive error handling");

function handleError(message: string): void {
  let timestamp: string = timer.current();
  let logFile: string = "/var/log/script-errors.log";

  fs.appendFile(logFile, `[${timestamp}] ${message}\n`);
  console.log(`ERROR: ${message}`);
}

// Use consistent error handling throughout the script
try {
  // Main script logic
  processData();
}
catch (error) {
  handleError(`Script execution failed: ${error}`);
  exit(1);
}
```

## Best Practices

1. **Always handle expected errors** - Don't let scripts fail unexpectedly
2. **Use specific error messages** - Include context about what failed
3. **Implement fallback strategies** - Provide alternative paths when possible
4. **Log errors appropriately** - Include timestamps and relevant details
5. **Test error paths** - Ensure error handling works as expected
6. **Clean up resources** - Use finally blocks or explicit cleanup
7. **Set appropriate exit codes** - Follow standard conventions
8. **Validate inputs early** - Catch errors before they cause damage

## Common Error Scenarios

### File Operations

```typescript
function safeFileOperation(filename: string): boolean {
  try {
    if (!fs.exists(filename)) {
      console.log(`File not found: ${filename}`);
      return false;
    }

    let content: string = fs.readFile(filename);
    // Process content
    return true;
  }
  catch (error) {
    console.log(`File operation failed: ${error}`);
    return false;
  }
}
```

### Network Operations

```typescript
function fetchData(url: string): string {
  try {
    return `$(curl -f --max-time 30 "${url}")`;
  }
  catch {
    throw `Failed to fetch data from ${url}`;
  }
}
```

### System Commands

```typescript
function runSystemCommand(command: string): boolean {
  try {
    let output: string = `$(${command})`;
    console.log("Command completed successfully");
    return true;
  }
  catch (error) {
    console.log(`Command failed: ${command}`);
    console.log(`Error: ${error}`);
    return false;
  }
}
```

Error handling in Utah makes shell scripts more robust and production-ready, enabling graceful failure recovery and detailed error reporting.
