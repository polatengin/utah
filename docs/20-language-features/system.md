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
