---
layout: default
title: System Functions
parent: Language Features
nav_order: 8
---

Utah provides system-level functions for process management, environment interaction, and system administration.

## System Information

### Platform Detection

```typescript
// Get operating system
let os: string = system.platform();

// Get architecture
let arch: string = system.architecture();

// Get hostname
let hostname: string = system.hostname();

// Get current user
let user: string = system.user();
```

### Environment Variables

```typescript
// Get environment variable
let path: string = system.env("PATH");

// Set environment variable
system.setEnv("MY_VAR", "value");

// Check if variable exists
let exists: boolean = system.hasEnv("HOME");
```

### Process Management

```typescript
// Get process ID
let pid: number = system.pid();

// Execute command
let result: string = system.execute("ls -la");

// Start a process in the background and get its PID
let backgroundPid: number = process.start("echo 'Hello World'");

// Check if a process is running
let isRunning: boolean = process.isRunning(backgroundPid);
console.log("Process running: " + isRunning);

// Check if system init process is running (always true)
let initRunning: boolean = process.isRunning(1);

// Wait for a process to complete and get its exit code
let buildPid: number = process.start("make build");
let exitCode: number = process.waitForExit(buildPid);
if (exitCode == 0) {
  console.log("Build succeeded");
} else {
  console.log("Build failed with exit code: " + exitCode);
}

// Wait with timeout (milliseconds)
let deployPid: number = process.start("deploy.sh");
let result: number = process.waitForExit(deployPid, 30000); // 30 second timeout
if (result == -1) {
  console.log("Deployment timed out");
} else if (result == 0) {
  console.log("Deployment completed successfully");
} else {
  console.log("Deployment failed");
}

// Process monitoring workflow
let longRunningPid: number = process.start("sleep 30");
while (process.isRunning(longRunningPid)) {
  console.log("Process still running...");
  // Do other work or sleep for a bit
}
console.log("Process finished");

// Start process with working directory
let pid1: number = process.start("pwd", { cwd: "/tmp" });

// Start process with input redirection
let pid2: number = process.start("cat", { input: "/etc/hostname" });

// Start process with output redirection
let pid3: number = process.start("date", { output: "/tmp/timestamp.txt" });

// Start process with error redirection
let pid4: number = process.start("ls /nonexistent", { error: "/tmp/errors.log" });

// Start process with all options
let pid5: number = process.start("sort", {
  cwd: "/tmp",
  input: "/etc/passwd",
  output: "/tmp/sorted.txt",
  error: "/tmp/sort_errors.log"
});

// Execute with error handling
try {
  let output: string = system.execute("some-command");
} catch (error) {
  console.log("Command failed");
}
```

## Advanced Process Management

### Waiting for Process Completion

The `process.waitForExit()` function allows you to wait for a background process to complete and retrieve its exit code:

```typescript
// Basic usage - wait indefinitely
let buildPid: number = process.start("npm run build");
let exitCode: number = process.waitForExit(buildPid);

if (exitCode == 0) {
  console.log("Build succeeded");
} else {
  console.log("Build failed with exit code: " + exitCode);
}

// Wait with timeout (in milliseconds)
let testPid: number = process.start("npm test");
let result: number = process.waitForExit(testPid, 120000); // 2 minute timeout

if (result == -1) {
  console.log("Tests timed out after 2 minutes");
} else if (result == 0) {
  console.log("Tests passed");
} else {
  console.log("Tests failed with exit code: " + result);
}

// Sequential workflow example
function deployApplication(): void {
  // Step 1: Build
  console.log("Building application...");
  let buildPid: number = process.start("npm run build");
  let buildResult: number = process.waitForExit(buildPid, 300000); // 5 minute timeout
  
  if (buildResult == -1) {
    console.log("Build timed out");
    exit(1);
  } else if (buildResult != 0) {
    console.log("Build failed");
    exit(1);
  }
  
  // Step 2: Test
  console.log("Running tests...");
  let testPid: number = process.start("npm test");
  let testResult: number = process.waitForExit(testPid, 600000); // 10 minute timeout
  
  if (testResult == -1) {
    console.log("Tests timed out");
    exit(1);
  } else if (testResult != 0) {
    console.log("Tests failed");
    exit(1);
  }
  
  // Step 3: Deploy
  console.log("Deploying application...");
  let deployPid: number = process.start("kubectl apply -f deployment.yaml");
  let deployResult: number = process.waitForExit(deployPid, 180000); // 3 minute timeout
  
  if (deployResult == 0) {
    console.log("Deployment completed successfully");
  } else {
    console.log("Deployment failed");
    exit(1);
  }
}

// Parallel execution with synchronization
let task1Pid: number = process.start("process-large-file-1.sh");
let task2Pid: number = process.start("process-large-file-2.sh");
let task3Pid: number = process.start("process-large-file-3.sh");

// Wait for all tasks to complete
let result1: number = process.waitForExit(task1Pid, 1800000); // 30 minute timeout
let result2: number = process.waitForExit(task2Pid, 1800000);
let result3: number = process.waitForExit(task3Pid, 1800000);

if (result1 == 0 && result2 == 0 && result3 == 0) {
  console.log("All parallel tasks completed successfully");
} else {
  console.log("Some tasks failed or timed out");
  if (result1 != 0) console.log("Task 1 failed/timed out");
  if (result2 != 0) console.log("Task 2 failed/timed out");
  if (result3 != 0) console.log("Task 3 failed/timed out");
}
```

### Process Management Features

- **Exit Code Retrieval**: Get the actual exit code from completed processes
- **Timeout Support**: Prevent indefinite waiting with configurable timeouts
- **Non-blocking**: Continue script execution while waiting for processes
- **Cross-platform**: Works on all Unix-like systems (Linux, macOS, BSD)
- **Resource Efficient**: Uses 100ms polling interval for responsive yet efficient monitoring

### Return Values

- **0-255**: Normal process exit codes (0 typically means success)
- **-1**: Timeout occurred (process was still running when timeout expired)

## File System Operations

### Path Operations

```typescript
// Get current directory
let cwd: string = system.cwd();

// Change directory
system.cd("/path/to/directory");

// Get home directory
let home: string = system.home();

// Get temp directory
let temp: string = system.temp();
```

### File Operations

```typescript
// Check if file exists
let exists: boolean = system.fileExists("/path/to/file");

// Check if directory exists
let dirExists: boolean = system.dirExists("/path/to/dir");

// Create directory
system.mkdir("/path/to/new/dir");

// Remove file
system.remove("/path/to/file");
```

## Generated Bash

System functions compile to appropriate bash commands:

```bash
# Platform detection
os=$(uname -s)

# Environment variables
path="$PATH"
export MY_VAR="value"

# Process management
pid=$$
result=$(ls -la)

# Background process execution with process.start()
backgroundPid=$(echo 'Hello World' &; echo $!)

# Process with working directory
pid1=$((cd "/tmp" && pwd &); echo $!)

# Process with input redirection
pid2=$(cat < "/etc/hostname" &; echo $!)

# Process with output redirection
pid3=$(date > "/tmp/timestamp.txt" &; echo $!)

# Process with error redirection
pid4=$(ls /nonexistent 2> "/tmp/errors.log" &; echo $!)

# Process with all options
pid5=$((cd "/tmp" && sort < "/etc/passwd" > "/tmp/sorted.txt" 2> "/tmp/sort_errors.log" &); echo $!)

# File system operations
cwd=$(pwd)
cd "/path/to/directory"
home="$HOME"
```

## Use Cases

- System administration scripts
- Environment setup and configuration
- Process monitoring and management
- File system operations
- Cross-platform compatibility
