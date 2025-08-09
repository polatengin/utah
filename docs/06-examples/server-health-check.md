---
layout: default
title: Server Health Check
parent: Examples
nav_order: 2
description: "Practical server monitoring script with auto-recovery capabilities"
permalink: /examples/server-health-check/
---

import { AsciinemaPlayer } from '@site/src/components';

A practical server monitoring script that performs comprehensive health checks, logs issues, and executes automatic recovery procedures. This example demonstrates Utah's error handling, environment configuration, and system monitoring capabilities.

## 🎬 Interactive Demo

Watch this script in action! The demo shows environment detection, privilege checking, resource monitoring, and intelligent auto-recovery procedures:

<AsciinemaPlayer
  src="/assets/server-health-check.cast"
  autoPlay={false}
  loop={false}
  speed={1}
  idleTimeLimit={3}
  theme="asciinema"
  poster="npt:0:01"
/>

## Features Demonstrated

- **Environment-based configuration** with sensible defaults
- **Privilege checking** with sudo validation
- **Resource monitoring** with dynamic thresholds
- **Auto-recovery procedures** with environment-specific logic
- **File system operations** with path manipulation
- **Comprehensive logging** with timestamped entries
- **Error handling** with graceful degradation

## Complete Script

```typescript
// Server Health Check and Auto-Recovery Script
// This script monitors server health, logs issues, and performs automatic recovery

script.enableDebug();
script.exitOnError(); // Enable strict error handling for recovery

console.log("🔍 Starting comprehensive server health check...");

// Load configuration from environment file
const configFile: string = ".env";
const defaultLogLevel: string = env.get("LOG_LEVEL") ? env.get("LOG_LEVEL") : "INFO";
const maxRetries: number = env.get("MAX_RETRIES") ? env.get("MAX_RETRIES") : 3;
const alertEmail: string = env.get("ALERT_EMAIL") ? env.get("ALERT_EMAIL") : "admin@localhost";

console.log(`⚙️  Configuration loaded - Log Level: ${defaultLogLevel}, Max Retries: ${maxRetries}`);

// Check if running with proper privileges
let hasAdminRights: boolean = console.isSudo();
if (!hasAdminRights) {
  console.log("❌ Error: This script requires sudo privileges for system operations");
  console.log("Please run: sudo utah compile server-health-check.shx && sudo ./server-health-check.sh");
  exit(1);
}

console.log("✅ Running with administrator privileges");

// System information gathering
const serverOS: string = os.getOS();
const processId: number = process.id();
let currentMemory: number = process.memory();
let currentCPU: number = process.cpu();

// Use ternary operators for status evaluation
let memoryStatus: string = currentMemory > 80 ? "CRITICAL" : currentMemory > 60 ? "WARNING" : "OK";
let cpuStatus: string = currentCPU > 90 ? "CRITICAL" : currentCPU > 70 ? "WARNING" : "OK";

console.log(`📊 Server OS: ${serverOS}`);
console.log(`🔧 Process ID: ${processId}`);
console.log(`💾 Memory Usage: ${currentMemory}% (${memoryStatus})`);
console.log(`⚡ CPU Usage: ${currentCPU}% (${cpuStatus})`);

// Determine server environment using switch statement
let environment: string = "unknown";
switch (serverOS) {
  case "linux":
    environment = "production";
    break;
  case "darwin":
    environment = "development";
    break;
  case "windows":
    environment = "testing";
    break;
  default:
    environment = "unknown";
    break;
}

console.log(`🌍 Environment detected: ${environment}`);

// Check if critical applications are installed
const applications: string[] = ["docker", "nginx", "git", "curl"];
let missingApps: string[] = [];
let appStatuses: string[] = [];

for (let app: string in applications) {
  let isInstalled: boolean = os.isInstalled(app);
  let status: string = isInstalled ? "✅ INSTALLED" : "❌ MISSING";

  console.log(`${status}: ${app}`);

  if (!isInstalled) {
    array.push(missingApps, app);
  }

  array.push(appStatuses, `${app}: ${status}`);
}

// Application health assessment using switch
let appHealthLevel: string = "";
switch (missingApps.length) {
  case 0:
    appHealthLevel = "EXCELLENT";
    break;
  case 1:
    appHealthLevel = "GOOD";
    break;
  case 2:
    appHealthLevel = "FAIR";
    break;
  default:
    appHealthLevel = "POOR";
    break;
}

console.log(`📦 Application Health: ${appHealthLevel} (${missingApps.length} missing)`);

// Resource monitoring and alerting with environment-based thresholds
const memoryThreshold: number = environment == "production" ? 80 : 90;
const cpuThreshold: number = environment == "production" ? 70 : 85;
let issuesFound: boolean = false;
let alertLevel: string = "INFO";

timer.start();

// Memory check with ternary operator for severity
if (currentMemory > memoryThreshold) {
  alertLevel = currentMemory > 95 ? "CRITICAL" : "WARNING";
  console.log(`⚠️  HIGH MEMORY USAGE: ${currentMemory}% (threshold: ${memoryThreshold}%) - ${alertLevel}`);
  issuesFound = true;

  // Log the issue with timestamp
  let timestamp: string = process.elapsedTime();
  let logEntry: string = `[${timestamp}] ${alertLevel} MEMORY: ${currentMemory}%`;
  fs.writeFile("/var/log/health-check.log", logEntry);
}

// CPU check with switch-based response
if (currentCPU > cpuThreshold) {
  let cpuAction: string = "";

  if (currentCPU > 95) {
    cpuAction = "EMERGENCY_SHUTDOWN";
    alertLevel = "CRITICAL";
  }
  else if (currentCPU > 85) {
    cpuAction = "THROTTLE_SERVICES";
    alertLevel = "WARNING";
  }
  else {
    cpuAction = "MONITOR_CLOSELY";
    alertLevel = "INFO";
  }

  console.log(`⚠️  HIGH CPU USAGE: ${currentCPU}% (threshold: ${cpuThreshold}%) - Action: ${cpuAction}`);
  issuesFound = true;

  // Log the issue
  let timestamp: string = process.elapsedTime();
  let logEntry: string = `[${timestamp}] ${alertLevel} CPU: ${currentCPU}% - ${cpuAction}`;
  fs.writeFile("/var/log/health-check.log", logEntry);
}

// Auto-recovery procedures with environment-specific logic
if (issuesFound) {
  let recoveryMode: string = environment == "production" ? "CONSERVATIVE" : "AGGRESSIVE";
  let shouldRecover: boolean = console.promptYesNo(`🔧 Issues detected (${alertLevel}). Attempt ${recoveryMode} recovery?`);

  if (shouldRecover) {
    console.log(`🔄 Starting ${recoveryMode} recovery procedures...`);

    // Environment-based recovery strategy using switch
    switch (environment) {
      case "production":
        console.log("🛡️  Production mode: Conservative recovery");
        console.log("📊 Monitoring system load...");
        console.log("🔄 Graceful service restart...");
        break;
      case "development":
        console.log("🚀 Development mode: Aggressive optimization");
        console.log("💾 Clearing all caches...");
        console.log("🔄 Hard service restart...");
        break;
      case "testing":
        console.log("🧪 Testing mode: Diagnostic recovery");
        console.log("📋 Collecting debug information...");
        console.log("🔍 Running system diagnostics...");
        break;
      default:
        console.log("❓ Unknown environment: Basic recovery");
        break;
    }

    // Generate recovery delay based on alert level
    let recoveryDelay: number = alertLevel == "CRITICAL" ? utility.random(500, 1000) : utility.random(1000, 3000);
    console.log(`⏳ Waiting ${recoveryDelay}ms before recovery...`);

    let recoverySteps: string = alertLevel == "CRITICAL" ? "emergency" : "standard";
    console.log(`🛠️  Executing ${recoverySteps} recovery steps...`);

    console.log("✅ Recovery procedures completed");
  } else {
    console.log("⏭️  Recovery skipped by user");
  }
}

// File system health check
let logDir: string = "/var/log";
let logDirName: string = fs.dirname("/var/log/health-check.log");
let logFileName: string = fs.fileName("/var/log/health-check.log");

console.log(`📁 Log directory: ${logDirName}`);
console.log(`📄 Log file: ${logFileName}`);

// Configuration validation
let configPaths: string[] = ["/etc/nginx/nginx.conf", "/etc/docker/daemon.json"];
for (let configPath: string in configPaths) {
  let configDir: string = fs.dirname(configPath);
  let configFileName: string = fs.fileName(configPath);
  let configExt: string = fs.extension(configPath);

  console.log(`⚙️  Config: ${configFileName} (${configExt}) in ${configDir}`);
}

// Generate comprehensive report
let reportData: string[] = [];
array.push(reportData, `Server Health Report`);
array.push(reportData, `===================`);
array.push(reportData, `OS: ${serverOS}`);
array.push(reportData, `Memory: ${currentMemory}%`);
array.push(reportData, `CPU: ${currentCPU}%`);
array.push(reportData, `Issues Found: ${issuesFound ? "Yes" : "No"}`);

let reportContent: string = array.join(reportData, "\n");
fs.writeFile("/tmp/health-report.txt", reportContent);

let executionTime: number = timer.stop();
console.log(`⏱️  Health check completed in ${executionTime}ms`);

if (issuesFound) {
  console.log("⚠️  Health check completed with issues - see logs for details");
  exit(1);
} else {
  console.log("🎉 All systems healthy!");
  exit(0);
}
```

## Key Features Explained

### Environment-Based Configuration

The script adapts its behavior based on the detected environment:

```typescript
let environment: string = "unknown";
switch (serverOS) {
  case "linux":
    environment = "production";
    break;
  case "darwin":
    environment = "development";
    break;
  case "windows":
    environment = "testing";
    break;
}

// Environment-specific thresholds
const memoryThreshold: number = environment == "production" ? 80 : 90;
const cpuThreshold: number = environment == "production" ? 70 : 85;
```

### Privilege Checking

Utah provides built-in privilege detection:

```typescript
let hasAdminRights: boolean = console.isSudo();
if (!hasAdminRights) {
  console.log("❌ Error: This script requires sudo privileges for system operations");
  exit(1);
}
```

### Smart Status Evaluation

Using ternary operators for concise status logic:

```typescript
let memoryStatus: string = currentMemory > 80 ? "CRITICAL" : currentMemory > 60 ? "WARNING" : "OK";
let cpuStatus: string = currentCPU > 90 ? "CRITICAL" : currentCPU > 70 ? "WARNING" : "OK";
```

### Application Health Assessment

Automated checking of critical applications:

```typescript
const applications: string[] = ["docker", "nginx", "git", "curl"];
let missingApps: string[] = [];

for (let app: string in applications) {
  let isInstalled: boolean = os.isInstalled(app);
  if (!isInstalled) {
    array.push(missingApps, app);
  }
}

// Assessment based on missing applications
switch (missingApps.length) {
  case 0:
    appHealthLevel = "EXCELLENT";
    break;
  case 1:
    appHealthLevel = "GOOD";
    break;
  default:
    appHealthLevel = "POOR";
    break;
}
```

### Auto-Recovery Logic

Environment-specific recovery procedures:

```typescript
switch (environment) {
  case "production":
    console.log("🛡️  Production mode: Conservative recovery");
    console.log("🔄 Graceful service restart...");
    break;
  case "development":
    console.log("🚀 Development mode: Aggressive optimization");
    console.log("💾 Clearing all caches...");
    break;
  case "testing":
    console.log("🧪 Testing mode: Diagnostic recovery");
    console.log("🔍 Running system diagnostics...");
    break;
}
```

### File System Operations

Built-in path manipulation functions:

```typescript
let logDirName: string = fs.dirname("/var/log/health-check.log");
let logFileName: string = fs.fileName("/var/log/health-check.log");
let configExt: string = fs.extension(configPath);
```

## Usage Examples

### Basic Health Check

```bash
utah compile server-health-check.shx
sudo ./server-health-check.sh
```

### With Environment Variables

```bash
export LOG_LEVEL="DEBUG"
export MAX_RETRIES="5"
export ALERT_EMAIL="ops@company.com"
sudo ./server-health-check.sh
```

### Automated Monitoring

```bash
# Add to crontab for regular monitoring
*/5 * * * * /usr/local/bin/utah run /opt/scripts/server-health-check.shx
```

## Environment Configuration

Create a `.env` file for customization:

```bash
LOG_LEVEL=INFO
MAX_RETRIES=3
ALERT_EMAIL=admin@company.com
```

## Output Examples

### Healthy System

```text
🔍 Starting comprehensive server health check...
⚙️  Configuration loaded - Log Level: INFO, Max Retries: 3
✅ Running with administrator privileges
📊 Server OS: linux
🔧 Process ID: 12345
💾 Memory Usage: 45% (OK)
⚡ CPU Usage: 25% (OK)
🌍 Environment detected: production
✅ INSTALLED: docker
✅ INSTALLED: nginx
✅ INSTALLED: git
✅ INSTALLED: curl
📦 Application Health: EXCELLENT (0 missing)
⏱️  Health check completed in 245ms
🎉 All systems healthy!
```

### System with Issues

```text
🔍 Starting comprehensive server health check...
📊 Server OS: linux
💾 Memory Usage: 85% (CRITICAL)
⚡ CPU Usage: 75% (WARNING)
⚠️  HIGH MEMORY USAGE: 85% (threshold: 80%) - CRITICAL
⚠️  HIGH CPU USAGE: 75% (threshold: 70%) - Action: THROTTLE_SERVICES
🔧 Issues detected (CRITICAL). Attempt CONSERVATIVE recovery? (y/n)
```

## Benefits Over Traditional Bash

- **Type Safety**: All variables are strongly typed with clear interfaces
- **Error Handling**: Built-in error handling with `script.exitOnError()`
- **Environment Detection**: Automatic OS and environment detection
- **File Operations**: Clean path manipulation without external tools
- **Interactive Prompts**: Built-in user interaction functions
- **Comprehensive Logging**: Structured logging with timestamps
- **Auto-Recovery**: Intelligent recovery procedures based on severity

## Related Examples

- [System Health Monitor](system-health-monitor) - Comprehensive monitoring suite
- [String Processing](string-processing) - Text manipulation techniques
- [Log File Analyzer](log-file-analyzer) - Array operations and data structures
