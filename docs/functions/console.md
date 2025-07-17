---
layout: default
title: Console Functions
parent: Functions
nav_order: 2
---

The `console` namespace provides functions for user interaction, output formatting, and terminal control. These functions handle input/output operations and enhance script usability.

## Output Functions

### console.log()

Print messages to standard output with automatic newline:

```typescript
console.log("Hello, World!");
console.log(`Current user: ${USER}`);

let count: number = 42;
console.log(`Processing ${count} items`);
```

**Generated Bash:**

```bash
echo "Hello, World!"
echo "Current user: ${USER}"
count=42
echo "Processing ${count} items"
```

### console.clear()

Clear the terminal screen:

```typescript
console.clear();
console.log("Screen cleared - starting fresh");
```

**Generated Bash:**

```bash
clear
echo "Screen cleared - starting fresh"
```

**Test Coverage:**

- File: `tests/positive_fixtures/console_clear.shx`
- Tests terminal clearing functionality

## Interactive Functions

### console.prompt()

Prompt user for input with a message:

```typescript
let name: string = console.prompt("Enter your name: ");
console.log(`Hello, ${name}!`);

let age: string = console.prompt("Enter your age: ");
console.log(`You are ${age} years old`);
```

**Generated Bash:**

```bash
read -p "Enter your name: " name
echo "Hello, ${name}!"
read -p "Enter your age: " age
echo "You are ${age} years old"
```

### console.promptYesNo()

Prompt user for yes/no confirmation:

```typescript
let shouldContinue: boolean = console.promptYesNo("Do you want to continue? (y/n): ");

if (shouldContinue) {
  console.log("Continuing...");
} else {
  console.log("Operation cancelled");
  exit(0);
}
```

**Generated Bash:**

```bash
while true; do
  read -p "Do you want to continue? (y/n): " yn
  case $yn in
    [Yy]* ) shouldContinue=true; break;;
    [Nn]* ) shouldContinue=false; break;;
    * ) echo "Please answer yes or no.";;
  esac
done

if [ "${shouldContinue}" = "true" ]; then
  echo "Continuing..."
else
  echo "Operation cancelled"
  exit 0
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/console_prompt_yesno.shx`
- Tests yes/no prompt functionality with proper bash case handling

## System Information

### console.isSudo()

Check if the script is running with sudo privileges:

```typescript
if (console.isSudo()) {
  console.log("Running with administrator privileges");
} else {
  console.log("Running as regular user");
  console.log("Some operations may require sudo");
}
```

**Generated Bash:**

```bash
if [ "$EUID" -eq 0 ]; then
  echo "Running with administrator privileges"
else
  echo "Running as regular user"
  echo "Some operations may require sudo"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/console_issudo.shx`
- Tests privilege detection using `$EUID` variable

## Practical Examples

### User Confirmation Flow

```typescript
script.description("File deletion utility with confirmation");

args.define("--file", "-f", "File to delete", "string", true);

let filename: string = args.getString("--file");

if (!fs.exists(filename)) {
  console.log(`Error: File not found: ${filename}`);
  exit(1);
}

console.log(`File: ${filename}`);
console.log(`Size: $(stat -c%s "${filename}") bytes`);

let confirmed: boolean = console.promptYesNo(`Are you sure you want to delete ${filename}? (y/n): `);

if (confirmed) {
  console.log(`Deleting ${filename}...`);
  `$(rm "${filename}")`;
  console.log("File deleted successfully");
} else {
  console.log("Deletion cancelled");
}
```

### Interactive Setup Script

```typescript
script.description("Application setup wizard");

console.clear();
console.log("=== Application Setup Wizard ===");

let appName: string = console.prompt("Enter application name: ");
let port: string = console.prompt("Enter port number (default 3000): ");
if (port == "") {
  port = "3000";
}

let enableSSL: boolean = console.promptYesNo("Enable SSL? (y/n): ");
let enableDebug: boolean = console.promptYesNo("Enable debug mode? (y/n): ");

console.log("\n=== Configuration Summary ===");
console.log(`Application Name: ${appName}`);
console.log(`Port: ${port}`);
console.log(`SSL Enabled: ${enableSSL}`);
console.log(`Debug Mode: ${enableDebug}`);

let saveConfig: boolean = console.promptYesNo("\nSave this configuration? (y/n): ");

if (saveConfig) {
  let config: object = json.parse('{}');
  config = json.set(config, ".name", appName);
  config = json.set(config, ".port", port);
  config = json.set(config, ".ssl", enableSSL ? "true" : "false");
  config = json.set(config, ".debug", enableDebug ? "true" : "false");

  let configJson: string = json.stringify(config, true);
  fs.writeFile("config.json", configJson);

  console.log("Configuration saved to config.json");
} else {
  console.log("Configuration not saved");
}
```

### Permission Checker

```typescript
script.description("System permission and capability checker");

console.log("=== System Permission Check ===");

// Check if running as root/sudo
if (console.isSudo()) {
  console.log("✓ Running with administrative privileges");

  // Check system directories
  if (fs.exists("/etc")) {
    console.log("✓ Can access system configuration directory");
  }

  if (fs.exists("/var/log")) {
    console.log("✓ Can access system log directory");
  }

} else {
  console.log("⚠ Running as regular user");

  let needsRoot: boolean = console.promptYesNo("This script may need root privileges. Continue anyway? (y/n): ");
  if (!needsRoot) {
    console.log("Please run with sudo for full functionality:");
    console.log("sudo utah run " + args.getScriptName());
    exit(1);
  }
}

// Check specific capabilities
console.log("\n=== Capability Check ===");

if (os.isInstalled("systemctl")) {
  console.log("✓ systemctl available - can manage services");
} else {
  console.log("✗ systemctl not available");
}

if (os.isInstalled("docker")) {
  console.log("✓ Docker available");
} else {
  console.log("✗ Docker not available");
}

if (fs.exists("/proc/version")) {
  let kernelVersion: string = `$(cat /proc/version | awk '{print $3}')`;
  console.log(`✓ Kernel version: ${kernelVersion}`);
}
```

### Progress Indicator

```typescript
script.description("Long-running process with progress updates");

let tasks: string[] = [
  "Initializing system",
  "Loading configuration",
  "Connecting to database",
  "Processing data",
  "Generating reports",
  "Cleaning up"
];

let total: number = tasks.length;

console.log(`Starting process with ${total} tasks...\n`);

for (let i: number = 0; i < total; i++) {
  let current: number = i + 1;
  let task: string = tasks[i];
  let percent: number = (current * 100) / total;

  console.log(`[${current}/${total}] (${percent}%) ${task}...`);

  // Simulate work (in real script, this would be actual work)
  `$(sleep 1)`;

  console.log(`✓ ${task} completed`);
}

console.log("\n✓ All tasks completed successfully!");
```

### Error Recovery Dialog

```typescript
function handleError(operation: string, error: string): boolean {
  console.log(`\n❌ Error during ${operation}:`);
  console.log(`   ${error}`);

  console.log("\nOptions:");
  console.log("1. Retry the operation");
  console.log("2. Skip and continue");
  console.log("3. Abort the script");

  let choice: string = console.prompt("Choose an option (1-3): ");

  if (choice == "1") {
    console.log("Retrying...");
    return true; // retry
  } else if (choice == "2") {
    console.log("Skipping this operation...");
    return false; // skip
  } else {
    console.log("Aborting script...");
    exit(1);
  }
}

// Usage example
let retryCount: number = 0;
let maxRetries: number = 3;

while (retryCount < maxRetries) {
  try {
    // Attempt some operation
    let result: string = `$(wget -q --spider http://example.com && echo "success" || echo "failed")`;

    if (result == "success") {
      console.log("✓ Connection test successful");
      break;
    } else {
      if (handleError("connection test", "Unable to reach example.com")) {
        retryCount++;
        continue; // retry
      } else {
        break; // skip
      }
    }
  }
  catch {
    if (handleError("connection test", "Network error occurred")) {
      retryCount++;
      continue; // retry
    } else {
      break; // skip
    }
  }
}

if (retryCount >= maxRetries) {
  console.log("Maximum retry attempts reached");
}
```

## Best Practices

### 1. Provide Clear Prompts

```typescript
// Good - clear and specific
let dbPassword: string = console.prompt("Enter database password (will not echo): ");
let confirmDelete: boolean = console.promptYesNo("Delete all log files? This cannot be undone (y/n): ");

// Avoid - unclear prompts
let input: string = console.prompt("Enter value: ");
let confirm: boolean = console.promptYesNo("Continue? ");
```

### 2. Validate User Input

```typescript
function getValidPort(): number {
  while (true) {
    let portStr: string = console.prompt("Enter port number (1-65535): ");
    let port: number = portStr;

    if (port >= 1 && port <= 65535) {
      return port;
    } else {
      console.log("Invalid port number. Please enter a value between 1 and 65535.");
    }
  }
}

let serverPort: number = getValidPort();
```

### 3. Provide Feedback

```typescript
// Good - immediate feedback
console.log("Connecting to server...");
let connected: boolean = connectToServer();
if (connected) {
  console.log("✓ Connected successfully");
} else {
  console.log("✗ Connection failed");
}

// Avoid - silent operations
let result: boolean = connectToServer();
```

### 4. Use Appropriate Privilege Checks

```typescript
function requireRoot(): void {
  if (!console.isSudo()) {
    console.log("This operation requires root privileges.");
    console.log("Please run with sudo:");
    console.log(`sudo utah run ${args.getScriptName()}`);
    exit(1);
  }
}

// Only require root when actually needed
if (args.has("--system-install")) {
  requireRoot();
}
```

### 5. Format Output Consistently

```typescript
// Use consistent symbols and formatting
console.log("=== System Health Check ===");
console.log("✓ Service A is running");
console.log("✓ Service B is running");
console.log("⚠ Service C has warnings");
console.log("✗ Service D is stopped");
console.log("================================");
```

## Function Reference Summary

| Function | Purpose | Return Type | Example |
|----------|---------|-------------|---------|
| `console.log(message)` | Print message | void | `console.log("Hello")` |
| `console.clear()` | Clear screen | void | `console.clear()` |
| `console.prompt(message)` | Get user input | string | `let name = console.prompt("Name: ")` |
| `console.promptYesNo(message)` | Get yes/no input | boolean | `let ok = console.promptYesNo("Continue? ")` |
| `console.isSudo()` | Check root privileges | boolean | `if (console.isSudo()) { ... }` |

The console functions provide essential user interaction capabilities for Utah scripts, making them more interactive and user-friendly while maintaining the robustness expected from system automation tools.
