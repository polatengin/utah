---
layout: default
title: Operating System Functions
parent: Language Features
nav_order: 4
---

The `os` namespace provides system information, package management, and operating system interaction functions. These functions help scripts adapt to different environments and manage system dependencies.

## System Information

### os.getOS()

Get the current operating system name:

```typescript
let currentOS: string = os.getOS();
console.log("Running on: ${currentOS}");

if (currentOS == "Linux") {
  console.log("Linux-specific operations can be performed");
} else if (currentOS == "Darwin") {
  console.log("macOS-specific operations can be performed");
}
```

**Generated Bash:**

```bash
currentOS=$(uname -s)
echo "Running on: ${currentOS}"

if [ "${currentOS}" = "Linux" ]; then
  echo "Linux-specific operations can be performed"
elif [ "${currentOS}" = "Darwin" ]; then
  echo "macOS-specific operations can be performed"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/os_getos.shx`
- Tests OS detection using `uname -s`

### os.getLinuxVersion()

Get Linux distribution information (Linux only):

```typescript
if (os.getOS() == "Linux") {
  let linuxVersion: string = os.getLinuxVersion();
  console.log("Linux distribution: ${linuxVersion}");

  if (string.contains(linuxVersion, "Ubuntu")) {
    console.log("Running on Ubuntu");
  } else if (string.contains(linuxVersion, "CentOS")) {
    console.log("Running on CentOS");
  }
}
```

**Generated Bash:**

```bash
if [ "$(uname -s)" = "Linux" ]; then
  if [ -f /etc/os-release ]; then
    linuxVersion=$(grep '^PRETTY_NAME=' /etc/os-release | cut -d '"' -f 2)
  elif [ -f /etc/redhat-release ]; then
    linuxVersion=$(cat /etc/redhat-release)
  else
    linuxVersion="Unknown Linux"
  fi
  echo "Linux distribution: ${linuxVersion}"

  if [[ "${linuxVersion}" == *"Ubuntu"* ]]; then
    echo "Running on Ubuntu"
  elif [[ "${linuxVersion}" == *"CentOS"* ]]; then
    echo "Running on CentOS"
  fi
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/os_getlinuxversion.shx`
- Tests Linux distribution detection from `/etc/os-release`

## Package Management

### os.isInstalled()

Check if a command or package is available:

```typescript
if (os.isInstalled("git")) {
  console.log("Git is available");
  let version: string = `$(git --version)`;
  console.log("Version: ${version}");
} else {
  console.log("Git is not installed");
}

// Check multiple tools
let requiredTools: string[] = ["curl", "wget", "jq"];
for (let tool: string in requiredTools) {
  if (os.isInstalled(tool)) {
    console.log("✓ ${tool} is available");
  } else {
    console.log("✗ ${tool} is missing");
  }
}
```

**Generated Bash:**

```bash
if command -v git >/dev/null 2>&1; then
  echo "Git is available"
  version=$(git --version)
  echo "Version: ${version}"
else
  echo "Git is not installed"
fi

requiredTools=("curl" "wget" "jq")
for tool in "${requiredTools[@]}"; do
  if command -v "${tool}" >/dev/null 2>&1; then
    echo "✓ ${tool} is available"
  else
    echo "✗ ${tool} is missing"
  fi
done
```

**Test Coverage:**

- File: `tests/positive_fixtures/os_isinstalled.shx`
- Tests command availability using `command -v`

## Process Information

### Process and Environment Queries

Get system process and environment information:

```typescript
// Get current user
let currentUser: string = `$(whoami)`;
console.log("Current user: ${currentUser}");

// Get system uptime
if (os.isInstalled("uptime")) {
  let uptime: string = `$(uptime)`;
  console.log("System uptime: ${uptime}");
}

// Get memory information (Linux)
if (os.getOS() == "Linux" && fs.exists("/proc/meminfo")) {
  let memTotal: string = `$(grep MemTotal /proc/meminfo | awk '{print $2}')`;
  let memFree: string = `$(grep MemFree /proc/meminfo | awk '{print $2}')`;
  console.log("Memory: ${memFree}KB free of ${memTotal}KB total");
}

// Get CPU information (Linux)
if (os.getOS() == "Linux" && fs.exists("/proc/cpuinfo")) {
  let cpuCount: string = `$(grep -c ^processor /proc/cpuinfo)`;
  let cpuModel: string = `$(grep "model name" /proc/cpuinfo | head -1 | cut -d: -f2 | sed 's/^ *//')`;
  console.log("CPU: ${cpuCount} cores, ${cpuModel}");
}
```

## Platform-Specific Operations

### Cross-Platform Package Installation

```typescript
function installPackage(packageName: string): boolean {
  let osType: string = os.getOS();
  let installed: boolean = false;

  if (osType == "Linux") {
    let linuxVersion: string = os.getLinuxVersion();

    if (string.contains(linuxVersion, "Ubuntu") || string.contains(linuxVersion, "Debian")) {
      // Ubuntu/Debian using apt
      console.log("Installing ${packageName} using apt...");
      let result: number = "$(sudo apt-get update && sudo apt-get install -y ${packageName})";
      installed = (result == 0);

    } else if (string.contains(linuxVersion, "CentOS") || string.contains(linuxVersion, "Red Hat")) {
      // CentOS/RHEL using yum or dnf
      if (os.isInstalled("dnf")) {
        console.log("Installing ${packageName} using dnf...");
        let result: number = "$(sudo dnf install -y ${packageName})";
        installed = (result == 0);
      } else if (os.isInstalled("yum")) {
        console.log("Installing ${packageName} using yum...");
        let result: number = "$(sudo yum install -y ${packageName})";
        installed = (result == 0);
      }

    } else if (string.contains(linuxVersion, "Arch")) {
      // Arch Linux using pacman
      console.log("Installing ${packageName} using pacman...");
      let result: number = "$(sudo pacman -S --noconfirm ${packageName})";
      installed = (result == 0);
    }

  } else if (osType == "Darwin") {
    // macOS using Homebrew
    if (os.isInstalled("brew")) {
      console.log("Installing ${packageName} using Homebrew...");
      let result: number = "$(brew install ${packageName})";
      installed = (result == 0);
    } else {
      console.log("Homebrew not found. Please install Homebrew first.");
    }
  }

  if (installed) {
    console.log("✓ ${packageName} installed successfully");
  } else {
    console.log("✗ Failed to install ${packageName}");
  }

  return installed;
}
```

## Service Management

### System Service Operations

```typescript
function checkService(serviceName: string): string {
  if (!os.isInstalled("systemctl")) {
    return "systemctl not available";
  }

  let status: string = "$(systemctl is-active ${serviceName} 2>/dev/null || echo "inactive")";
  return status;
}

function isServiceRunning(serviceName: string): boolean {
  let status: string = checkService(serviceName);
  return status == "active";
}

function startService(serviceName: string): boolean {
  if (!console.isSudo()) {
    console.log("Root privileges required to start services");
    return false;
  }

  console.log("Starting service: ${serviceName}");
  let result: number = "$(systemctl start ${serviceName})";

  if (result == 0) {
    console.log("✓ Service ${serviceName} started");
    return true;
  } else {
    console.log("✗ Failed to start service ${serviceName}");
    return false;
  }
}

// Usage example
let services: string[] = ["nginx", "mysql", "redis"];

for (let service: string in services) {
  if (isServiceRunning(service)) {
    console.log("✓ ${service} is running");
  } else {
    console.log("✗ ${service} is not running");

    let shouldStart: boolean = console.promptYesNo("Start ${service}?");
    if (shouldStart) {
      startService(service);
    }
  }
}
```

## Practical Examples

### System Health Check

```typescript
script.description("Comprehensive system health check");

function performHealthCheck(): void {
  console.log("=== System Health Check ===");

  // Basic system information
  let os: string = os.getOS();
  let user: string = `$(whoami)`;
  console.log("Operating System: ${os}");
  console.log("Current User: ${user}");

  if (os == "Linux") {
    let distro: string = os.getLinuxVersion();
    console.log("Distribution: ${distro}");
  }

  // Check essential tools
  console.log("\n=== Essential Tools ===");
  let essentialTools: string[] = ["bash", "curl", "wget", "git", "tar", "gzip"];
  let missingTools: string[] = [];

  for (let tool: string in essentialTools) {
    if (os.isInstalled(tool)) {
      console.log("✓ ${tool}");
    } else {
      console.log("✗ ${tool}");
      missingTools[missingTools.length] = tool;
    }
  }

  // System resources
  console.log("\n=== System Resources ===");

  if (os.isInstalled("df")) {
    let diskSpace: string = `$(df -h / | tail -1)`;
    console.log("Disk Usage: ${diskSpace}");
  }

  if (os.isInstalled("free")) {
    let memory: string = `$(free -h | grep Mem)`;
    console.log("Memory: ${memory}");
  }

  if (os.isInstalled("uptime")) {
    let uptime: string = `$(uptime)`;
    console.log("Uptime: ${uptime}");
  }

  // Network connectivity
  console.log("\n=== Network Connectivity ===");

  if (os.isInstalled("ping")) {
    let pingResult: number = `$(ping -c 1 google.com >/dev/null 2>&1; echo $?)`;
    if (pingResult == 0) {
      console.log("✓ Internet connectivity");
    } else {
      console.log("✗ No internet connectivity");
    }
  }

  // Summary
  console.log("\n=== Summary ===");
  if (missingTools.length == 0) {
    console.log("✓ All essential tools are available");
  } else {
    console.log("⚠ Missing tools: ${array.join(missingTools, ", ")}");
  }
}

performHealthCheck();
```

### Development Environment Setup

```typescript
script.description("Development environment setup script");

function setupDevelopmentEnvironment(): void {
  console.log("=== Development Environment Setup ===");

  let os: string = os.getOS();
  console.log("Setting up for ${os}...");

  // Essential development tools
  let devTools: string[] = ["git", "curl", "wget", "vim", "htop"];
  let missingTools: string[] = [];

  // Check what's missing
  for (let tool: string in devTools) {
    if (!os.isInstalled(tool)) {
      missingTools[missingTools.length] = tool;
    }
  }

  if (missingTools.length > 0) {
    console.log("Missing development tools: ${array.join(missingTools, ", ")}");

    let shouldInstall: boolean = console.promptYesNo("Install missing tools?");
    if (shouldInstall) {
      for (let tool: string in missingTools) {
        installPackage(tool);
      }
    }
  } else {
    console.log("✓ All development tools are already installed");
  }

  // Programming languages
  console.log("\n=== Programming Language Support ===");

  // Check Node.js
  if (os.isInstalled("node")) {
    let nodeVersion: string = `$(node --version)`;
    console.log("✓ Node.js: ${nodeVersion}");
  } else {
    console.log("✗ Node.js not found");
    let installNode: boolean = console.promptYesNo("Install Node.js?");
    if (installNode) {
      if (os == "Darwin" && os.isInstalled("brew")) {
        `$(brew install node)`;
      } else if (os == "Linux") {
        // Use NodeSource repository for latest Node.js
        `$(curl -fsSL https://deb.nodesource.com/setup_lts.x | sudo -E bash -)`;
        installPackage("nodejs");
      }
    }
  }

  // Check Python
  if (os.isInstalled("python3")) {
    let pythonVersion: string = `$(python3 --version)`;
    console.log("✓ Python: ${pythonVersion}");
  } else {
    console.log("✗ Python3 not found");
    let installPython: boolean = console.promptYesNo("Install Python3?");
    if (installPython) {
      installPackage("python3");
    }
  }

  // Check Docker
  if (os.isInstalled("docker")) {
    let dockerVersion: string = `$(docker --version)`;
    console.log("✓ Docker: ${dockerVersion}");
  } else {
    console.log("✗ Docker not found");
    let installDocker: boolean = console.promptYesNo("Install Docker?");
    if (installDocker) {
      console.log("Please install Docker manually from https://docker.com");
    }
  }

  console.log("\n✓ Development environment setup completed");
}

setupDevelopmentEnvironment();
```

### System Monitoring

```typescript
script.description("System monitoring and alerting");

function monitorSystem(): void {
  console.log("=== System Monitoring ===");

  // CPU usage (if available)
  if (os.isInstalled("top")) {
    let cpuUsage: string = `$(top -bn1 | grep "Cpu(s)" | awk '{print $2}' | cut -d'%' -f1)`;
    let cpuFloat: number = cpuUsage;

    console.log("CPU Usage: ${cpuUsage}%");
    if (cpuFloat > 80) {
      console.log("⚠ High CPU usage detected!");
    }
  }

  // Memory usage
  if (os.getOS() == "Linux" && fs.exists("/proc/meminfo")) {
    let memTotal: number = `$(grep MemTotal /proc/meminfo | awk '{print $2}')`;
    let memFree: number = `$(grep MemFree /proc/meminfo | awk '{print $2}')`;
    let memUsed: number = memTotal - memFree;
    let memPercent: number = (memUsed * 100) / memTotal;

    console.log("Memory Usage: ${memPercent}% (${memUsed}KB / ${memTotal}KB)");
    if (memPercent > 90) {
      console.log("⚠ High memory usage detected!");
    }
  }

  // Disk usage
  if (os.isInstalled("df")) {
    let diskUsage: string = `$(df -h / | tail -1 | awk '{print $5}' | cut -d'%' -f1)`;
    let diskPercent: number = diskUsage;

    console.log("Disk Usage: ${diskUsage}%");
    if (diskPercent > 85) {
      console.log("⚠ High disk usage detected!");
    }
  }

  // Load average (Unix-like systems)
  if (os.isInstalled("uptime")) {
    let loadAvg: string = `$(uptime | awk -F'load average:' '{print $2}')`;
    console.log("Load Average:${loadAvg}");
  }

  // Check for failed services (systemd systems)
  if (os.isInstalled("systemctl")) {
    let failedServices: string = `$(systemctl --failed --no-legend | wc -l)`;
    let failedCount: number = failedServices;

    if (failedCount > 0) {
      console.log("⚠ ${failedCount} failed services detected!");
      let failedList: string = `$(systemctl --failed --no-legend)`;
      console.log("Failed services:\n${failedList}");
    } else {
      console.log("✓ All services are running normally");
    }
  }
}

// Run monitoring
monitorSystem();

// Optionally run in a loop for continuous monitoring
args.define("--watch", "-w", "Continuous monitoring mode", "boolean", false);

if (args.has("--watch")) {
  console.log("Starting continuous monitoring (Ctrl+C to stop)...");
  while (true) {
    console.clear();
    monitorSystem();
    `$(sleep 5)`;
  }
}
```

## Best Practices

### 1. Check Availability Before Use

```typescript
// Good - check before using commands
if (os.isInstalled("systemctl")) {
  let status: string = `$(systemctl is-active nginx)`;
  console.log("Nginx status: ${status}");
} else {
  console.log("systemctl not available - cannot check service status");
}

// Avoid - assuming commands exist
let status: string = `$(systemctl is-active nginx)`; // May fail
```

### 2. Handle Platform Differences

```typescript
function getPackageManager(): string {
  let os: string = os.getOS();

  if (os == "Darwin") {
    return os.isInstalled("brew") ? "brew" : "none";
  } else if (os == "Linux") {
    let distro: string = os.getLinuxVersion();

    if (string.contains(distro, "Ubuntu") || string.contains(distro, "Debian")) {
      return "apt";
    } else if (string.contains(distro, "CentOS") || string.contains(distro, "Red Hat")) {
      return os.isInstalled("dnf") ? "dnf" : "yum";
    } else if (string.contains(distro, "Arch")) {
      return "pacman";
    }
  }

  return "unknown";
}
```

### 3. Graceful Degradation

```typescript
function getSystemInfo(): object {
  let info: object = json.parse('{}');

  // Always available
  info = json.set(info, ".os", os.getOS());

  // Optional information
  if (os.isInstalled("whoami")) {
    info = json.set(info, ".user", `$(whoami)`);
  }

  if (os.isInstalled("hostname")) {
    info = json.set(info, ".hostname", `$(hostname)`);
  }

  return info;
}
```

### 4. Validate Permissions

```typescript
function requireTool(toolName: string): void {
  if (!os.isInstalled(toolName)) {
    console.log("Error: ${toolName} is required but not installed");
    console.log("Please install ${toolName} and try again");
    exit(1);
  }
}

function requireRoot(): void {
  if (!console.isSudo()) {
    console.log("This operation requires root privileges");
    console.log("Please run with sudo");
    exit(1);
  }
}
```

## Function Reference Summary

| Function | Purpose | Return Type | Example |
|----------|---------|-------------|---------|
| `os.getOS()` | Get OS name | string | `os.getOS()` |
| `os.getLinuxVersion()` | Get Linux distro | string | `os.getLinuxVersion()` |
| `os.isInstalled(cmd)` | Check command availability | boolean | `os.isInstalled("git")` |

Operating system functions provide the foundation for writing portable scripts that can adapt to different environments and manage system dependencies effectively.
