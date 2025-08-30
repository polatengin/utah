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
// Write module exports for splitting .shx files
export function trim(str: string): string {
  return string.trim(str);
}

export function capitalize(str: string): string {
  return string.capitalize(str);
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

### Graceful Error Recovery

```typescript
// Implement retry logic
function retryOperation(operation: Function, maxRetries: number = 3): any {
  let attempts: number = 0;

  while (attempts < maxRetries) {
    try {
      return operation();
    } catch {
      attempts++;

      if (attempts >= maxRetries) {
        console.log("Max retries reached, operation failed");
        exit(1);
      }

      console.log("Attempt ${attempts} failed, retrying...");
      // Wait 1 second before retry - using built-in bash sleep
      let _: string = `$(sleep 1)`;
    }
  }
}

// Use fallback values
function getConfigValue(key: string, defaultValue: string = ""): string {
  try {
    return env.get(key, defaultValue);
  } catch {
    console.log("Warning: Could not get ${key}, using default");
    return defaultValue;
  }
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
    commands[commands.length] = "cp ${file} /destination/";
  }

  // Execute all commands in one call using bash
  let _: string = `$(${array.join(commands, " && ")})`;
}
```

### Memory Management

```typescript
// Process data efficiently in chunks
function processDataInChunks(data: string[], chunkSize: number = 100): void {
  for (let i: number = 0; i < data.length; i += chunkSize) {
    let chunk: string[] = [];
    
    // Build chunk manually
    for (let j: number = i; j < i + chunkSize && j < data.length; j++) {
      // Use array assignment since we're not sure about array.push
      chunk[chunk.length] = data[j];
    }

    // Process chunk
    processChunk(chunk);

    // Note: Utah has automatic memory management
  }
}

// Use for iterative processing
function processNumbers(max: number): void {
  for (let i: number = 0; i < max; i++) {
    if (i % 1000 === 0) {
      console.log(i);
    }
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
    let fileModTime: string = `$(stat -c %Y ${cacheFile})`;
    let currentTime: string = `$(date +%s)`;
    let fileAge: number = Number(currentTime) - Number(fileModTime);

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
  let secret: string = env.get(key);

  if (!secret) {
    console.log("Required secret ${key} not found");
    exit(1);
  }

  return secret;
}

// Secure file permissions
function createSecureFile(filename: string, content: string): void {
  fs.writeFile(filename, content);

  // Set restrictive permissions using bash
  let _: string = `$(chmod 600 ${filename})`;
}

// Hash sensitive data
function hashPassword(password: string): string {
  let salt: string = utility.uuid(); // Use UUID as salt
  let hash: string = utility.hash(password + salt);

  return "${salt}:${hash}";
}
```

### Audit and Logging

```typescript
// Audit security-sensitive operations
function auditLog(operation: string, user: string, resource: string): void {
  let entry: object = {
    timestamp: `$(date -Iseconds)`,
    operation: operation,
    user: user,
    resource: resource,
    sourceIP: env.get("SSH_CLIENT", "unknown")
  };

  let logEntry: string = json.stringify(entry);
  fs.writeFile("/var/log/audit.log", fs.readFile("/var/log/audit.log") + logEntry + "\n");
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
  // Simple currency formatting
  return "$" + amount;
}

function calculateTotal(subtotal: number, taxRate: number): string {
  let tax: number = calculateTax(subtotal, taxRate);
  let total: number = subtotal + tax;

  return formatCurrency(total);
}

// Use descriptive names
function getUserByEmailAddress(email: string): object {
  // Implementation would go here
  return json.parse('{"id": 1, "email": "' + email + '"}');
}

function sendWelcomeEmailToNewUser(user: object): void {
  // Implementation would go here
  console.log("Sending welcome email to " + json.get(user, ".email"));
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
  // Implementation would go here
  let content: string = fs.readFile(filename);
  let lines: string[] = string.split(content, "\n");
  let result: object[] = [];
  
  // Basic CSV parsing for demonstration
  for (let line: string in lines) {
    if (line.trim() != "") {
      let fields: string[] = string.split(line, delimiter);
      result.push(json.parse('{"data": "' + array.join(fields, "|") + '"}'));
    }
  }
  
  return result;
}

/**
 * Configuration object for API client
 */
// Note: Utah uses objects for configuration structures
let APIConfig: object = {
  baseURL: "https://api.example.com",
  timeout: 30000,
  retries: 3,
  apiKey: "your-api-key-here"
};
```

### Testing

```typescript
// Write testable code
function addNumbers(a: number, b: number): number {
  return a + b;
}

// Test pure functions
function testAddNumbers(): void {
  // Basic assertion pattern
  if (addNumbers(2, 3) !== 5) {
    console.log("Test failed: addNumbers(2, 3) should equal 5");
    exit(1);
  }
  if (addNumbers(-1, 1) !== 0) {
    console.log("Test failed: addNumbers(-1, 1) should equal 0");
    exit(1);
  }
  if (addNumbers(0, 0) !== 0) {
    console.log("Test failed: addNumbers(0, 0) should equal 0");
    exit(1);
  }
  console.log("All addNumbers tests passed");
}

// Mock external dependencies for testing
function mockFileSystem(): void {
  // In Utah, you would use test fixtures or temporary files
  fs.writeFile("/tmp/test_file.txt", "mocked content");
}
```

## Deployment Best Practices

### Environment-Specific Configuration

```typescript
// Use environment-specific settings
let environment: string = env.get("ENVIRONMENT", "development");
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

  if (!isVersionGreaterOrEqual(VERSION, requiredVersion)) {
    console.log("Version ${requiredVersion} or higher required");
    exit(1);
  }
}

function isVersionGreaterOrEqual(current: string, required: string): boolean {
  // Simple version comparison for demonstration
  return current >= required;
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
  } catch {
    console.log("Health check failed: ${error}");
    return false;
  }
}

function checkDiskSpace(): boolean {
  let usage: string = `$(df -h / | tail -1 | awk '{print $5}')`;
  let usagePercent: number = Number(string.replace(usage, "%", ""));

  return usagePercent < 90;
}
```

## Monitoring and Observability

### Structured Logging

```typescript
// Use structured logging
function logMessage(level: string, message: string, context: object = {}): void {
  let logEntry: object = {
    timestamp: `$(date -Iseconds)`,
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
    timestamp: `$(date +%s)`,
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
