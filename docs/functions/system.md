---
sidebar_position: 4
---

# System Functions

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
