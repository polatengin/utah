---
layout: default
title: Import System
parent: Language Features
nav_order: 6
---

Utah's import system enables modular programming by allowing you to split code across multiple files and reuse functionality.

## Basic Import Syntax

### Simple Import

```typescript
import "utils.shx";

// Now you can use functions from utils.shx
let result: string = formatOutput("Hello World");
```

## File Organization

### Project Structure

```text
project/
├── main.shx           # Entry point
├── lib/
│   ├── utils.shx      # Utility functions
│   ├── config.shx     # Configuration handling
│   └── logging.shx    # Logging functionality
└── modules/
    ├── database.shx   # Database operations
    └── api.shx        # API interactions
```

### Import Paths

Utah resolves imports relative to the current file:

```typescript
// From main.shx
import "lib/utils.shx";
import "modules/database.shx";

// From lib/config.shx
import "utils.shx";           // Same directory
import "../modules/api.shx";  // Parent directory navigation
```

## Creating Reusable Modules

### Utility Module (lib/utils.shx)

```typescript
// lib/utils.shx
script.description("Utility functions for the application");

function formatTimestamp(): string {
  return timer.current();
}

function validateEmail(email: string): boolean {
  return string.contains(email, "@") && string.contains(email, ".");
}

function ensureDirectory(path: string): void {
  if (!fs.exists(path)) {
    $(mkdir -p ${path});
    console.log("Created directory: ${path}");
  }
}

function logMessage(level: string, message: string): void {
  let timestamp: string = formatTimestamp();
  console.log("[${timestamp}] ${level}: ${message}");
}
```

### Configuration Module (lib/config.shx)

```typescript
// lib/config.shx
script.description("Configuration management");

const DEFAULT_CONFIG: string = "{\"database\": {\"host\": \"localhost\", \"port\": 5432}, \"logging\": {\"level\": \"info\"}}";

function loadConfig(): object {
  let configFile: string = "config.json";

  if (fs.exists(configFile)) {
    try {
      let content: string = fs.readFile(configFile);
      return json.parse(content);
    }
    catch {
      console.log("Invalid config file, using defaults");
    }
  }

  return json.parse(DEFAULT_CONFIG);
}

function getConfigValue(config: object, key: string): string {
  return json.get(config, key);
}
```

### Database Module (modules/database.shx)

```typescript
// modules/database.shx
import "../lib/utils.shx";
import "../lib/config.shx";

script.description("Database connection and operations");

function connectToDatabase(): object {
  let config: object = loadConfig();
  let host: string = getConfigValue(config, ".database.host");
  let port: string = getConfigValue(config, ".database.port");

  logMessage("INFO", "Connecting to database at ${host}:${port}");

  // Database connection logic here
  return { host: host, port: port, connected: true };
}

function executeQuery(connection: object, query: string): string {
  logMessage("DEBUG", "Executing query: ${query}");

  // Query execution logic
  return $(psql -h ${connection.host} -p ${connection.port} -c \"${query}\");
}
```

## Using Imported Modules

### Main Application (main.shx)

```typescript
// main.shx
import "lib/utils.shx";
import "lib/config.shx";
import "modules/database.shx";

script.description("Main application with modular architecture");

function main(): void {
  logMessage("INFO", "Application starting");

  // Load configuration
  let config: object = loadConfig();

  // Ensure required directories
  ensureDirectory("/var/log/myapp");
  ensureDirectory("/tmp/myapp");

  // Connect to database
  try {
    let db: object = connectToDatabase();

    // Execute some queries
    let result: string = executeQuery(db, "SELECT version()");
    logMessage("INFO", "Database version: ${result}");
  }
  catch (error) {
    logMessage("ERROR", "Database connection failed: ${error}");
    exit(1);
  }

  logMessage("INFO", "Application completed successfully");
}

main();
```

## Advanced Import Patterns

### Conditional Imports

```typescript
// Load different modules based on environment
let environment: string = env.get("APP_ENV", "development");

if (environment == "production") {
  import "modules/prod-logging.shx";
}
else {
  import "modules/dev-logging.shx";
}
```

### Dynamic Import Resolution

```typescript
// Import based on configuration
let config: object = loadConfig();
let databaseType: string = getConfigValue(config, ".database.type");

if (databaseType == "postgresql") {
  import "drivers/postgresql.shx";
}
else if (databaseType == "mysql") {
  import "drivers/mysql.shx";
}
else {
  console.log("Unsupported database type");
  exit(1);
}
```

## Module Best Practices

### Function Organization

Keep related functions together in logical modules:

```typescript
// math/calculations.shx
function add(a: number, b: number): number {
  return a + b;
}

function multiply(a: number, b: number): number {
  return a * b;
}

function calculatePercentage(value: number, total: number): number {
  return (value / total) * 100;
}
```

### Constants and Configuration

Define module-level constants:

```typescript
// constants/app.shx
const APP_NAME: string = "MyApplication";
const VERSION: string = "1.0.0";
const DEFAULT_TIMEOUT: number = 30;

function getAppInfo(): string {
  return "${APP_NAME} v${VERSION}";
}
```

### Error Handling in Modules

Consistent error handling across modules:

```typescript
// lib/errors.shx
function handleError(module: string, operation: string, error: string): void {
  let timestamp: string = timer.current();
  let message: string = "[${timestamp}] ${module}.${operation}: ${error}";

  console.log(message);
  fs.appendFile("/var/log/app-errors.log", "${message}\n");
}

// Usage in other modules
function riskyOperation(): string {
  try {
    return `$(some-command)`;
  }
  catch (error) {
    handleError("database", "connect", error);
    throw "Database connection failed";
  }
}
```

## Testing Modules

### Unit Testing Approach

```typescript
// tests/test-utils.shx
import "../lib/utils.shx";

function testValidateEmail(): void {
  // Test valid email
  if (!validateEmail("user@example.com")) {
    console.log("FAIL: Valid email rejected");
    exit(1);
  }

  // Test invalid email
  if (validateEmail("invalid-email")) {
    console.log("FAIL: Invalid email accepted");
    exit(1);
  }

  console.log("PASS: Email validation tests");
}

function runTests(): void {
  console.log("Running utility tests...");
  testValidateEmail();
  console.log("All tests passed!");
}

runTests();
```

## Dependency Management

### Explicit Dependencies

Document module dependencies clearly:

```typescript
// modules/reporting.shx
// Dependencies:
// - lib/utils.shx (for logging and formatting)
// - lib/config.shx (for report configuration)
// - modules/database.shx (for data access)

import "../lib/utils.shx";
import "../lib/config.shx";
import "database.shx";

function generateReport(): void {
  logMessage("INFO", "Starting report generation");

  let config: object = loadConfig();
  let db: object = connectToDatabase();

  // Report generation logic
}
```

### Circular Dependency Prevention

Avoid circular imports by organizing dependencies hierarchically:

```text
lib/          # Low-level utilities (no imports)
├── utils.shx
├── config.shx
└── logging.shx

modules/      # Business logic (imports from lib/)
├── database.shx
├── api.shx
└── reporting.shx

main.shx     # Entry point (imports from modules/ and lib/)
```

## Performance Considerations

### Import Optimization

- Import only what you need
- Place imports at the top of files
- Avoid deep import chains when possible

```typescript
// Good: Focused imports
import "lib/logging.shx";  // Only need logging

// Less optimal: Importing everything
import "lib/all-utilities.shx";  // Imports many unused functions
```

### Module Caching

Utah processes each imported file once per script execution, so modules are naturally cached.

## Common Import Patterns

### Library Structure

```typescript
// lib/index.shx - Main library entry point
import "utils.shx";
import "config.shx";
import "logging.shx";

// Applications can then import the entire library
import "lib/index.shx";
```

### Feature Modules

```typescript
// features/user-management.shx
import "../lib/database.shx";
import "../lib/validation.shx";

function createUser(username: string, email: string): boolean {
  if (!validateEmail(email)) {
    return false;
  }

  // User creation logic
  return true;
}
```

### Service Layers

```typescript
// services/notification.shx
import "../lib/config.shx";

function sendEmail(to: string, subject: string, body: string): boolean {
  let config: object = loadConfig();
  let smtpHost: string = getConfigValue(config, ".smtp.host");

  // Email sending logic
  return true;
}
```

The import system in Utah promotes code reusability, maintainability, and clear separation of concerns, making it easier to build complex shell automation tools.
