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
console.log("Current user: ${USER}");

let count: number = 42;
console.log("Processing ${count} items");
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
console.log("Hello, ${name}!");

let age: string = console.prompt("Enter your age: ");
console.log("You are ${age} years old");
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

### console.isInteractive()

Check if the script is running in an interactive terminal session:

```typescript
if (console.isInteractive()) {
  console.log("Running in interactive mode - can show prompts");
  let name: string = console.promptText("Enter your name");
  console.log(`Hello, ${name}!`);
} else {
  console.log("Running in non-interactive mode");
  console.log("Using default configuration");
}
```

**Generated Bash:**

```bash
if [ -t 0 ]; then
  echo "Running in interactive mode - can show prompts"
  name=$(while true; do read -p "Enter your name: " input; if [ -n "${input}" ]; then echo "${input}"; break; else echo "Please enter a valid name."; fi; done)
  echo "Hello, ${name}!"
else
  echo "Running in non-interactive mode"
  echo "Using default configuration"
fi
```

**Use Cases:**

- **Adaptive Scripts**: Show prompts only when running interactively
- **Automation Support**: Provide non-interactive fallbacks for CI/CD pipelines
- **Background Processing**: Detect when scripts run in background or as cron jobs
- **Error Prevention**: Avoid script hangs when prompts can't be displayed

**Test Coverage:**

- File: `tests/positive_fixtures/console_isinteractive.shx`
- Tests TTY detection using `[ -t 0 ]` bash test

## Dialog Functions

Utah provides comprehensive dialog functions for creating interactive terminal user interfaces. These functions automatically detect and use the best available dialog system (`dialog`, `whiptail`, or fallback to basic prompts).

### Message Display Functions

#### console.showMessage()

Show a general message dialog:

```typescript
console.showMessage("Welcome", "Welcome to Utah Dialog Demo!");
console.showMessage("Update", "System update completed successfully");
```

**Generated Bash:**

```bash
if command -v dialog >/dev/null 2>&1; then
  dialog --title "Welcome" --msgbox "Welcome to Utah Dialog Demo!" 8 50
elif command -v whiptail >/dev/null 2>&1; then
  whiptail --title "Welcome" --msgbox "Welcome to Utah Dialog Demo!" 8 50
else
  echo "Welcome: Welcome to Utah Dialog Demo!"
  read -p "Press Enter to continue..."
fi
```

#### console.showInfo()

Show an informational message:

```typescript
console.showInfo("Information", "Process completed successfully");
```

#### console.showWarning()

Show a warning message:

```typescript
console.showWarning("Warning", "This action cannot be undone");
```

#### console.showError()

Show an error message:

```typescript
console.showError("Error", "Failed to connect to server");
```

#### console.showSuccess()

Show a success message:

```typescript
console.showSuccess("Success", "File uploaded successfully");
```

### User Input Functions

#### console.promptText()

Prompt for text input with optional default value:

```typescript
let name: string = console.promptText("Enter your name", "John Doe");
let email: string = console.promptText("Enter your email");
```

**Generated Bash:**

```bash
if command -v dialog >/dev/null 2>&1; then
  name=$(dialog --title "Input" --inputbox "Enter your name" 8 50 "John Doe" 2>&1 >/dev/tty)
elif command -v whiptail >/dev/null 2>&1; then
  name=$(whiptail --title "Input" --inputbox "Enter your name" 8 50 "John Doe" 2>&1 >/dev/tty)
else
  read -p "Enter your name [John Doe]: " name
  name=${name:-"John Doe"}
fi
```

#### console.promptPassword()

Prompt for password input (hidden):

```typescript
let password: string = console.promptPassword("Enter your password");
```

**Generated Bash:**

```bash
if command -v dialog >/dev/null 2>&1; then
  password=$(dialog --title "Password" --passwordbox "Enter your password" 8 50 2>&1 >/dev/tty)
elif command -v whiptail >/dev/null 2>&1; then
  password=$(whiptail --title "Password" --passwordbox "Enter your password" 8 50 2>&1 >/dev/tty)
else
  read -s -p "Enter your password: " password
  echo
fi
```

#### console.promptNumber()

Prompt for numeric input with validation:

```typescript
let age: number = console.promptNumber("Enter your age", 18, 120, 25);
let port: number = console.promptNumber("Enter port number", 1, 65535);
```

#### console.promptFile()

Prompt for file selection:

```typescript
let configFile: string = console.promptFile("Select config file", "*.json");
let logFile: string = console.promptFile("Select log file");
```

#### console.promptDirectory()

Prompt for directory selection:

```typescript
let workDir: string = console.promptDirectory("Choose working directory", "/tmp");
let backupDir: string = console.promptDirectory("Select backup directory");
```

### Choice and Confirmation Functions

#### console.showChoice()

Show a single choice menu:

```typescript
let color: string = console.showChoice(
  "Color Selection",
  "Choose your favorite color:",
  "Red,Green,Blue,Yellow",
  0  // default selection index
);
```

**Generated Bash:**

```bash
if command -v dialog >/dev/null 2>&1; then
  color=$(dialog --title "Color Selection" --menu "Choose your favorite color:" 15 50 4 \
    "Red" "" "Green" "" "Blue" "" "Yellow" "" 2>&1 >/dev/tty)
elif command -v whiptail >/dev/null 2>&1; then
  color=$(whiptail --title "Color Selection" --menu "Choose your favorite color:" 15 50 4 \
    "Red" "" "Green" "" "Blue" "" "Yellow" "" 2>&1 >/dev/tty)
else
  echo "Choose your favorite color:"
  echo "1) Red"
  echo "2) Green"
  echo "3) Blue"
  echo "4) Yellow"
  read -p "Enter choice [1]: " choice
  choice=${choice:-1}
  case $choice in
    1) color="Red" ;;
    2) color="Green" ;;
    3) color="Blue" ;;
    4) color="Yellow" ;;
    *) color="Red" ;;
  esac
fi
```

#### console.showMultiChoice()

Show a multi-choice checklist:

```typescript
let features: string = console.showMultiChoice(
  "Features",
  "Select features to enable:",
  "Logging,Monitoring,Analytics,Backup",
  "Logging,Monitoring"  // default selections
);
```

#### console.showConfirm()

Show a yes/no confirmation dialog:

```typescript
let confirmed: boolean = console.showConfirm(
  "Confirm Action",
  "Are you sure you want to continue?",
  "yes"  // default button
);
```

### Progress Display

#### console.showProgress()

Show a progress indicator:

```typescript
console.showProgress("Processing", "Loading data...", 75, false);
console.showProgress("Upload", "Uploading files...", 50, true);  // cancellable
```

### Dialog System Compatibility

The dialog functions use a three-tier fallback system:

1. **Primary**: `dialog` command (full-featured TUI with rich widgets)
2. **Secondary**: `whiptail` command (lightweight TUI for basic environments)
3. **Fallback**: Basic terminal prompts using `read` and `echo`

This ensures compatibility across different environments while providing the best possible user experience.

### Test Coverage

- Files: `tests/positive_fixtures/console_show*.shx` and `tests/positive_fixtures/console_prompt*.shx`
- Tests all dialog functions with proper fallback handling
- Covers both expression and statement-level usage

## Practical Examples

### Interactive Setup Script with Dialogs

```typescript
script.description("Application setup wizard with rich dialogs");

// Welcome message
console.showMessage("Setup Wizard", "Welcome to the application setup!");

// Get user information with rich input dialogs
let userName: string = console.promptText("Your name", "User");
let userEmail: string = console.promptText("Your email address");
let userAge: number = console.promptNumber("Your age", 18, 120, 25);

// Choose installation type
let installType: string = console.showChoice(
  "Installation Type",
  "Choose installation type:",
  "Quick,Custom,Advanced",
  0
);

// Multi-select features
let features: string = console.showMultiChoice(
  "Features",
  "Select features to enable:",
  "Logging,Monitoring,Analytics,Backup",
  "Logging,Monitoring"
);

// Show configuration summary
let summary: string = `
Name: ${userName}
Email: ${userEmail}
Age: ${userAge}
Type: ${installType}
Features: ${features}
`;

console.showInfo("Configuration Summary", summary);

// Confirm installation
let confirmed: boolean = console.showConfirm(
  "Confirm Setup",
  "Proceed with installation?",
  "yes"
);

if (confirmed) {
  // Show progress
  console.showProgress("Installing", "Setting up application...", 25, false);
  `$(sleep 1)`;
  console.showProgress("Installing", "Configuring features...", 50, false);
  `$(sleep 1)`;
  console.showProgress("Installing", "Finalizing setup...", 100, false);

  console.showSuccess("Complete", "Installation completed successfully!");
} else {
  console.showWarning("Cancelled", "Installation was cancelled");
}
```

### User Confirmation Flow

```typescript
script.description("File deletion utility with confirmation");

args.define("--file", "-f", "File to delete", "string", true);

let filename: string = args.getString("--file");

if (!fs.exists(filename)) {
  console.log("Error: File not found: ${filename}");
  exit(1);
}

console.log("File: ${filename}");
console.log("Size: $(stat -c%s "${filename}") bytes");

let confirmed: boolean = console.promptYesNo("Are you sure you want to delete ${filename}? (y/n): ");

if (confirmed) {
  console.log("Deleting ${filename}...");
  "$(rm "${filename}")";
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
console.log("Application Name: ${appName}");
console.log("Port: ${port}");
console.log("SSL Enabled: ${enableSSL}");
console.log("Debug Mode: ${enableDebug}");

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
  console.log("✓ Kernel version: ${kernelVersion}");
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

console.log("Starting process with ${total} tasks...\n");

for (let i: number = 0; i < total; i++) {
  let current: number = i + 1;
  let task: string = tasks[i];
  let percent: number = (current * 100) / total;

  console.log("[${current}/${total}] (${percent}%) ${task}...");

  // Simulate work (in real script, this would be actual work)
  `$(sleep 1)`;

  console.log("✓ ${task} completed");
}

console.log("\n✓ All tasks completed successfully!");
```

### Error Recovery Dialog

```typescript
function handleError(operation: string, error: string): boolean {
  console.log("\n❌ Error during ${operation}:");
  console.log("   ${error}");

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
    console.log("sudo utah run ${args.getScriptName()}");
    exit(1);
  }
}

// Only require root when actually needed
if (args.has("--system-install")) {
  requireRoot();
}
```

### 5. Adapt to Execution Environment

```typescript
function setupConfiguration(): void {
  let configFile: string;

  if (console.isInteractive()) {
    // Interactive mode - can prompt user
    configFile = console.promptFile("Select config file", "*.json");
  } else {
    // Non-interactive mode - use environment or defaults
    configFile = env.get("CONFIG_FILE") || "/etc/app/default.json";
    if (!fs.exists(configFile)) {
      console.log("Error: Config file not found and not in interactive mode");
      console.log("Set CONFIG_FILE environment variable or run interactively");
      exit(1);
    }
  }

  console.log(`Using config: ${configFile}`);
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

| Function | Purpose | Return Type | Parameters | Example |
|----------|---------|-------------|------------|---------|
| `console.log(message)` | Print message | void | message: string | `console.log("Hello")` |
| `console.clear()` | Clear screen | void | none | `console.clear()` |
| `console.prompt(message)` | Get user input | string | message: string | `let name = console.prompt("Name: ")` |
| `console.promptYesNo(message)` | Get yes/no input | boolean | message: string | `let ok = console.promptYesNo("Continue? ")` |
| `console.isSudo()` | Check root privileges | boolean | none | `if (console.isSudo()) { ... }` |
| `console.isInteractive()` | Check if interactive | boolean | none | `if (console.isInteractive()) { ... }` |

### Dialog Functions Reference

| Function | Purpose | Return Type | Parameters | Example |
|----------|---------|-------------|------------|---------|
| `console.showMessage(title, message)` | General message dialog | void | title: string, message: string | `console.showMessage("Info", "Done")` |
| `console.showInfo(title, message)` | Information message | void | title: string, message: string | `console.showInfo("Info", "Success")` |
| `console.showWarning(title, message)` | Warning message | void | title: string, message: string | `console.showWarning("Warning", "Check logs")` |
| `console.showError(title, message)` | Error message | void | title: string, message: string | `console.showError("Error", "Failed")` |
| `console.showSuccess(title, message)` | Success message | void | title: string, message: string | `console.showSuccess("Success", "Complete")` |
| `console.showChoice(title, message, options, defaultIndex?)` | Single choice menu | string | title: string, message: string, options: string, defaultIndex?: number | `let color = console.showChoice("Pick", "Color:", "Red,Blue", 0)` |
| `console.showMultiChoice(title, message, options, defaultSelected?)` | Multi-choice checklist | string | title: string, message: string, options: string, defaultSelected?: string | `let features = console.showMultiChoice("Features", "Select:", "A,B,C", "A,B")` |
| `console.showConfirm(title, message, defaultButton?)` | Yes/no confirmation | boolean | title: string, message: string, defaultButton?: string | `let ok = console.showConfirm("Confirm", "Continue?", "yes")` |
| `console.showProgress(title, message, percent, canCancel?)` | Progress display | void | title: string, message: string, percent: number, canCancel?: boolean | `console.showProgress("Loading", "Please wait...", 75, false)` |
| `console.promptText(prompt, defaultValue?, validation?)` | Text input prompt | string | prompt: string, defaultValue?: string, validation?: string | `let name = console.promptText("Name:", "John")` |
| `console.promptPassword(prompt)` | Password input (hidden) | string | prompt: string | `let pwd = console.promptPassword("Password:")` |
| `console.promptNumber(prompt, min?, max?, defaultValue?)` | Numeric input with validation | number | prompt: string, min?: number, max?: number, defaultValue?: number | `let age = console.promptNumber("Age:", 1, 150, 25)` |
| `console.promptFile(prompt, filter?)` | File selection dialog | string | prompt: string, filter?: string | `let file = console.promptFile("Config:", "*.json")` |
| `console.promptDirectory(prompt, defaultPath?)` | Directory selection dialog | string | prompt: string, defaultPath?: string | `let dir = console.promptDirectory("Folder:", "/tmp")` |

The console functions provide essential user interaction capabilities for Utah scripts, making them more interactive and user-friendly while maintaining the robustness expected from system automation tools.
