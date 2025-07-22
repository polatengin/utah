---
layout: default
title: Defer Statements
parent: Language Features
nav_order: 8
---

The `defer` keyword provides automatic cleanup and resource management in Utah functions. Deferred statements execute automatically when a function exits, regardless of how it exits (normal return, early return, or error).

## Basic Syntax

### Simple Defer Statement

```typescript
function processFile(filename: string): void {
  let file = fs.openFile(filename);
  defer file.close();  // Always executes when function exits

  // Process the file...
  if (someCondition) {
    return;  // file.close() still executes
  }

  // More processing...
}  // file.close() executes here too
```

**Generated Bash:**

```bash
processFile() {
  local filename="$1"
  local _utah_defer_commands_processFile=()

  # Set up cleanup trap
  trap '_utah_cleanup_processFile' RETURN ERR EXIT

  local file
  file=$(fs_openFile "${filename}")

  # Register defer command
  _utah_defer_commands_processFile+=("file_close \"${file}\"")

  # Function body...
  if [ "${someCondition}" = "true" ]; then
    return
  fi
}

_utah_cleanup_processFile() {
  local i
  for ((i=${#_utah_defer_commands_processFile[@]}-1; i>=0; i--)); do
    eval "${_utah_defer_commands_processFile[i]}"
  done
}
```

## Execution Order

### LIFO (Last In, First Out)

Deferred statements execute in reverse order of their declaration:

```typescript
function setupResources(): void {
  defer console.log("Cleanup step 1");
  defer console.log("Cleanup step 2");
  defer console.log("Cleanup step 3");

  console.log("Function body");
}
```

**Output:**

```text
Function body
Cleanup step 3
Cleanup step 2
Cleanup step 1
```

### Multiple Defer Statements

```typescript
function complexCleanup(): void {
  let connection = db.connect();
  defer connection.close();

  let tempDir = fs.createTempDir();
  defer fs.removeDir(tempDir);

  let logFile = fs.openFile("process.log");
  defer logFile.close();

  // Work with resources...
  if (errorCondition) {
    return;  // All three cleanup actions execute in reverse order
  }
}
```

## Common Patterns

### Resource Management

```typescript
function downloadAndProcess(url: string, outputFile: string): void {
  // Create temporary directory
  let tempDir = fs.createTempDir();
  defer fs.removeDir(tempDir);

  // Download file
  let downloadPath = `${tempDir}/download.tmp`;
  web.download(url, downloadPath);
  defer fs.removeFile(downloadPath);

  // Open output file
  let output = fs.openFile(outputFile, "write");
  defer output.close();

  // Process and write...
}
```

### Lock Management

```typescript
function criticalSection(): void {
  let lockFile = "/tmp/process.lock";

  // Acquire lock
  fs.createFile(lockFile);
  defer fs.removeFile(lockFile);

  // Critical work that must be protected...
}
```

### Cleanup Notifications

```typescript
function longRunningTask(): void {
  console.log("Starting long task...");
  defer console.log("Task completed!");

  // Set up progress tracking
  defer console.log("Cleaning up progress tracking");

  // Actual work...
  for (let i = 0; i < 100; i++) {
    // Process item i
  }
}
```

## Error Handling

### Defer with Try/Catch

```typescript
function robustProcessing(): void {
  let resource = acquireResource();
  defer resource.release();  // Always executes, even if exception occurs

  try {
    // Risky operation
    processData(resource);
  } catch (error) {
    console.log(`Error occurred: ${error}`);
    // defer still executes after catch block
  }
}
```

### Conditional Cleanup

```typescript
function conditionalSetup(useBackup: boolean): void {
  let primaryResource = setupPrimary();
  defer primaryResource.cleanup();

  if (useBackup) {
    let backupResource = setupBackup();
    defer backupResource.cleanup();  // Only executes if backup was created
  }

  // Work with resources...
}
```

## Advanced Usage

### Variable Capture

Variables referenced in defer statements are captured by value at the time the defer is declared:

```typescript
function variableCapture(): void {
  let message = "Initial";
  defer console.log(`Deferred: ${message}`);  // Captures "Initial"

  message = "Modified";
  console.log(`Current: ${message}`);         // Prints "Modified"
}
```

**Output:**

```text
Current: Modified
Deferred: Initial
```

### Function Calls in Defer

```typescript
function complexCleanup(): void {
  let config = loadConfig();
  defer saveConfig(config);  // Function call with argument

  let server = startServer(config.port);
  defer server.stop();       // Method call

  // Server operations...
}
```

## Best Practices

### 1. Pair Resource Acquisition with Defer

```typescript
// Good: Immediate defer after acquisition
function goodPattern(): void {
  let file = fs.openFile("data.txt");
  defer file.close();  // Paired immediately

  // Use file...
}

// Avoid: Defer far from acquisition
function avoidPattern(): void {
  let file = fs.openFile("data.txt");

  // Lots of code...

  defer file.close();  // Easy to miss or forget
}
```

### 2. Use Defer for All Cleanup

```typescript
function comprehensiveCleanup(): void {
  // File system cleanup
  let tempFile = fs.createTempFile();
  defer fs.removeFile(tempFile);

  // Network cleanup
  let connection = net.connect("api.example.com");
  defer connection.close();

  // Process cleanup
  let process = system.startBackground("worker");
  defer process.kill();
}
```

### 3. Avoid Complex Logic in Defer

```typescript
// Good: Simple cleanup calls
defer file.close();
defer connection.disconnect();

// Avoid: Complex logic in defer
defer {
  if (someCondition) {
    // Complex cleanup logic
  }
}  // Not supported - defer only accepts simple statements
```

## Limitations

### 1. Function Scope Only

Defer statements can only be used inside functions:

```typescript
// Error: defer outside function
defer console.log("Invalid");  // Compilation error

function validUsage(): void {
  defer console.log("Valid");  // OK
}
```

### 2. Simple Statements Only

Defer only accepts simple statements, not complex control structures:

```typescript
function limitations(): void {
  // Valid defer statements
  defer file.close();
  defer console.log("Done");
  defer cleanup();

  // Invalid defer statements
  defer {  // Error: blocks not supported
    if (condition) {
      doCleanup();
    }
  }

  defer for (let i = 0; i < 10; i++) {  // Error: loops not supported
    cleanup(i);
  }
}
```

### 3. No Defer Modification

Once declared, defer statements cannot be cancelled or modified:

```typescript
function noModification(): void {
  defer cleanup();

  // Cannot cancel or modify the defer
  // The cleanup() call will always execute
}
```

## Implementation Details

Utah implements defer using bash trap handlers that execute when functions exit. The implementation ensures:

- **Automatic execution** on all exit paths (return, error, normal completion)
- **LIFO ordering** through array-based command storage
- **Variable isolation** to prevent interference between functions
- **Error resilience** so defer execution continues even if individual commands fail

The generated bash code uses function-specific arrays to store defer commands and trap handlers to ensure cleanup occurs reliably.
