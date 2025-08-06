---
sidebar_position: 6
---

# Best Practices

Learn the recommended practices for writing maintainable, efficient, and secure Utah scripts.

## Code Organization

### Project Structure

```text
project/
├── src/
│   ├── main.shx
│   ├── utils/
│   │   ├── helpers.shx
│   │   └── validators.shx
│   └── config/
│       └── settings.shx
├── test/
│   ├── unit/
│   └── integration/
├── docs/
└── scripts/
    ├── deploy.shx
    └── setup.shx
```

### Module Organization

```typescript
// src/utils/string-utils.shx
export function trim(str: string): string {
  return str.trim();
}

export function capitalize(str: string): string {
  return str.charAt(0).toUpperCase() + str.slice(1);
}

// src/main.shx
import "utils/string-utils.shx";

let name: string = trim("  Alice  ");
let greeting: string = capitalize("hello");
```

### Configuration Management

```typescript
// config/base.shx
const CONFIG: object = {
  timeout: 30,
  retries: 3,
  logLevel: "info"
};

// config/production.shx
import "base.shx";

CONFIG.timeout = 60;
CONFIG.logLevel = "error";

// config/development.shx
import "base.shx";

CONFIG.timeout = 10;
CONFIG.logLevel = "debug";
```

## Error Handling

### Defensive Programming

```typescript
// Always validate inputs
function divide(a: number, b: number): number {
  if (typeof a !== "number" || typeof b !== "number") {
    throw new Error("Arguments must be numbers");
  }

  if (b === 0) {
    throw new Error("Division by zero is not allowed");
  }

  return a / b;
}

// Check file existence before operations
function processFile(filename: string): void {
  if (!fs.exists(filename)) {
    throw new Error("File not found: ${filename}");
  }

  // Process file
  let content: string = fs.readFile(filename);
  // ...
}
```

### Graceful Error Recovery

```typescript
// Implement retry logic
function retryOperation(operation: Function, maxRetries: number = 3): any {
  let attempts: number = 0;

  while (attempts < maxRetries) {
    try {
      return operation();
    } catch (error) {
      attempts++;

      if (attempts >= maxRetries) {
        throw error;
      }

      console.log("Attempt ${attempts} failed, retrying...");
      utility.sleep(1000); // Wait 1 second before retry
    }
  }
}

// Use fallback values
function getConfigValue(key: string, defaultValue: string = ""): string {
  try {
    return system.env(key);
  } catch (error) {
    console.log("Warning: Could not get ${key}, using default");
    return defaultValue;
  }
}
```

### Comprehensive Error Logging

```typescript
// Structured error logging
function logError(error: Error, context: object = {}): void {
  let errorInfo: object = {
    timestamp: utility.dateString(),
    message: error.message,
    stack: error.stack,
    context: context
  };

  let logEntry: string = json.stringify(errorInfo);
  fs.appendFile("/var/log/app.log", logEntry + "\n");
}

// Usage
try {
  processFile("important.txt");
} catch (error) {
  logError(error, { operation: "processFile", filename: "important.txt" });
  throw error;
}
```

## Performance Optimization

### Efficient Resource Usage

```typescript
// Use defer for automatic cleanup
function processFile(filename: string): void {
  let tempDir = "$(mktemp -d)";
  defer "$(rm -rf ${tempDir})";  // Cleanup temp directory

  // Process file - cleanup happens automatically
  let content: string = fs.readFile(filename);
}

// Use streams for large files
function processLargeFile(filename: string): void {
  // For large files, process in chunks
  let content: string = fs.readFile(filename);
  let lines: string[] = string.split(content, "\n");

  for (let line: string in lines) {
    processLine(line);
  }
}

// Minimize system calls
function batchFileOperations(files: string[]): void {
  let commands: string[] = [];

  for (let file of files) {
    commands.push("cp ${file} /destination/");
  }

  // Execute all commands in one call
  system.execute(commands.join(" && "));
}
```

### Memory Management

```typescript
// Avoid loading large data structures
function processDataInChunks(data: string[], chunkSize: number = 100): void {
  for (let i: number = 0; i < data.length; i += chunkSize) {
    let chunk: string[] = data.slice(i, i + chunkSize);

    // Process chunk
    processChunk(chunk);

    // Clear chunk from memory
    chunk = null;
  }
}

// Use lazy evaluation
function* generateNumbers(max: number): Generator<number> {
  for (let i: number = 0; i < max; i++) {
    yield i;
  }
}

// Process numbers without loading all into memory
for (let num of generateNumbers(1000000)) {
  if (num % 1000 === 0) {
    console.log(num);
  }
}
```

### Caching Strategies

```typescript
// Simple memoization
let cache: object = {};

function expensiveOperation(input: string): string {
  if (cache[input]) {
    return cache[input];
  }

  // Perform expensive operation
  let result: string = performCalculation(input);

  // Cache result
  cache[input] = result;

  return result;
}

// File-based caching
function getCachedData(key: string, ttl: number = 3600): string {
  let cacheFile: string = "/tmp/cache_${key}";

  if (fs.exists(cacheFile)) {
    let fileModTime: string = "$(stat -c %Y ${cacheFile})";
    let currentTime: string = "$(date +%s)";
    let fileAge: number = parseInt(currentTime) - parseInt(fileModTime);

    if (fileAge < ttl) {
      return fs.readFile(cacheFile);
    }
  }

  // Fetch fresh data
  let data: string = fetchDataFromSource(key);

  // Cache data
  fs.writeFile(cacheFile, data);

  return data;
}
```

## Security Best Practices

### Input Validation

```typescript
// Sanitize user inputs
function sanitizeInput(input: string): string {
  // Remove dangerous characters
  let cleaned: string = input.replace(/[;&|`$]/, "");

  // Escape special characters
  cleaned = cleaned.replace(/'/g, "\\'");

  return cleaned;
}

// Validate file paths
function validatePath(path: string): boolean {
  // Prevent path traversal
  if (path.includes("..")) {
    return false;
  }

  // Only allow certain directories
  let allowedDirs: string[] = ["/tmp", "/var/log", "/home/user"];

  for (let dir of allowedDirs) {
    if (path.startsWith(dir)) {
      return true;
    }
  }

  return false;
}
```

### Secure Configuration

```typescript
// Use environment variables for secrets
function getSecret(key: string): string {
  let secret: string = system.env(key);

  if (!secret) {
    throw new Error("Required secret ${key} not found");
  }

  return secret;
}

// Secure file permissions
function createSecureFile(filename: string, content: string): void {
  fs.writeFile(filename, content);

  // Set restrictive permissions
  system.execute("chmod 600 ${filename}");
}

// Hash sensitive data
function hashPassword(password: string): string {
  let salt: string = utility.randomString(16);
  let hash: string = utility.hash(password + salt);

  return "${salt}:${hash}";
}
```

### Audit and Logging

```typescript
// Audit security-sensitive operations
function auditLog(operation: string, user: string, resource: string): void {
  let entry: object = {
    timestamp: utility.dateString(),
    operation: operation,
    user: user,
    resource: resource,
    sourceIP: system.env("SSH_CLIENT") || "unknown"
  };

  let logEntry: string = json.stringify(entry);
  fs.appendFile("/var/log/audit.log", logEntry + "\n");
}

// Usage
auditLog("file_access", "admin", "/etc/passwd");
```

## Code Quality

### Function Design

```typescript
// Write small, focused functions
function calculateTax(amount: number, rate: number): number {
  return amount * rate;
}

function formatCurrency(amount: number): string {
  return "$${amount.toFixed(2)}";
}

function calculateTotal(subtotal: number, taxRate: number): string {
  let tax: number = calculateTax(subtotal, taxRate);
  let total: number = subtotal + tax;

  return formatCurrency(total);
}

// Use descriptive names
function getUserByEmailAddress(email: string): object {
  // Implementation
}

function sendWelcomeEmailToNewUser(user: object): void {
  // Implementation
}
```

### Documentation

```typescript
/**
 * Processes a CSV file and returns parsed data
 * @param filename - Path to the CSV file
 * @param delimiter - Column delimiter (default: comma)
 * @param hasHeaders - Whether first row contains headers
 * @returns Array of objects representing rows
 * @throws Error if file cannot be read
 */
function processCSVFile(
  filename: string,
  delimiter: string = ",",
  hasHeaders: boolean = true
): object[] {
  // Implementation
}

/**
 * Configuration object for API client
 */
interface APIConfig {
  baseURL: string;
  timeout: number;
  retries: number;
  apiKey: string;
}
```

### Testing

```typescript
// Write testable code
function addNumbers(a: number, b: number): number {
  return a + b;
}

// Test pure functions
function testAddNumbers(): void {
  assert(addNumbers(2, 3) === 5, "Should add numbers correctly");
  assert(addNumbers(-1, 1) === 0, "Should handle negative numbers");
  assert(addNumbers(0, 0) === 0, "Should handle zero");
}

// Mock external dependencies
function mockFileSystem(): void {
  fs.readFile = (path: string): string => {
    return "mocked content";
  };
}
```

## Deployment Best Practices

### Environment-Specific Configuration

```typescript
// Use environment-specific settings
let environment: string = system.env("ENVIRONMENT") || "development";
let config: object = loadConfig(environment);

function loadConfig(env: string): object {
  let configFile: string = "config/${env}.json";

  if (!fs.exists(configFile)) {
    throw new Error("Configuration file not found: ${configFile}");
  }

  return json.parse(fs.readFile(configFile));
}
```

### Version Management

```typescript
// Include version information
const VERSION: string = "1.2.3";
const BUILD_DATE: string = "2024-01-15";

function printVersion(): void {
  console.log("Application Version: ${VERSION}");
  console.log("Build Date: ${BUILD_DATE}");
}

// Check compatibility
function checkCompatibility(): void {
  let requiredVersion: string = "1.0.0";

  if (compareVersions(VERSION, requiredVersion) < 0) {
    throw new Error("Version ${requiredVersion} or higher required");
  }
}
```

### Health Checks

```typescript
// Implement health checks
function healthCheck(): boolean {
  try {
    // Check database connectivity
    let dbHealth: boolean = checkDatabase();

    // Check external services
    let apiHealth: boolean = checkExternalAPI();

    // Check disk space
    let diskHealth: boolean = checkDiskSpace();

    return dbHealth && apiHealth && diskHealth;
  } catch (error) {
    console.log("Health check failed: ${error}");
    return false;
  }
}

function checkDiskSpace(): boolean {
  let usage: string = system.execute("df -h / | tail -1 | awk '{print $5}'");
  let usagePercent: number = parseInt(usage.replace("%", ""));

  return usagePercent < 90;
}
```

## Monitoring and Observability

### Structured Logging

```typescript
// Use structured logging
function logMessage(level: string, message: string, context: object = {}): void {
  let logEntry: object = {
    timestamp: utility.dateString(),
    level: level,
    message: message,
    context: context,
    service: "my-service",
    version: VERSION
  };

  console.log(json.stringify(logEntry));
}

// Usage
logMessage("INFO", "User logged in", { userId: "123", ip: "192.168.1.1" });
logMessage("ERROR", "Database connection failed", { error: "timeout" });
```

### Metrics Collection

```typescript
// Track metrics
function recordMetric(name: string, value: number, tags: object = {}): void {
  let metric: object = {
    timestamp: utility.timestamp(),
    name: name,
    value: value,
    tags: tags
  };

  // Send to monitoring system
  sendMetric(metric);
}

// Usage
recordMetric("request_duration", 150, { endpoint: "/api/users", method: "GET" });
recordMetric("error_count", 1, { service: "auth", error_type: "validation" });
```

## Code Review Checklist

### Before Submitting

- [ ] Code follows project conventions
- [ ] All functions have proper error handling
- [ ] Sensitive data is properly secured
- [ ] Tests are written and passing
- [ ] Documentation is updated
- [ ] Performance impact is considered
- [ ] Security implications are reviewed

### Review Focus Areas

- Error handling and edge cases
- Input validation and sanitization
- Performance and resource usage
- Security and access control
- Code readability and maintainability
- Test coverage and quality
- Documentation completeness
