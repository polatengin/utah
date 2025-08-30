---
layout: default
title: System Administration
parent: Guides
nav_order: 3
---

Server management and maintenance scripts with Utah. Automate system administration tasks, monitoring, and infrastructure management using Utah's system and OS functions.

## Prerequisites

- Basic system administration knowledge
- Understanding of Linux/Unix commands
- Knowledge of system services and processes
- Familiarity with networking concepts

## System Information and Monitoring

### System Health Checks

```typescript
script.description("Comprehensive system health monitoring");

// Check system information
let osInfo: string = os.getOS();
let linuxVersion: string = os.getLinuxVersion();

console.log(`System Information:`);
console.log("OS: ${osInfo}");
console.log("Linux Version: ${linuxVersion}");
console.log(`Hostname: $(hostname)`);
console.log(`Uptime: $(uptime)`);

// Check critical services
let criticalServices: string[] = ["ssh", "nginx", "mysql", "redis"];
let healthyServices: number = 0;

console.log(`\nService Status:`);
for (let service: string in criticalServices) {
  if (os.isInstalled("systemctl")) {
    let status: string = "$(systemctl is-active ${service} 2>/dev/null || echo "inactive")";
    if (status == "active") {
      console.log("✅ ${service}: Running");
      healthyServices++;
    } else {
      console.log("❌ ${service}: ${status}");
    }
  }
}

// System resource usage
console.log(`\nResource Usage:`);
let memInfo: string = `$(free -h | grep Mem)`;
let diskInfo: string = `$(df -h / | tail -1)`;
let loadAvg: string = `$(uptime | awk -F'load average:' '{print $2}')`;

console.log("Memory: ${memInfo}");
console.log("Disk: ${diskInfo}");
console.log("Load Average:${loadAvg}");

// Overall health score
let healthPercentage: number = Math.round((healthyServices / criticalServices.length) * 100);
console.log("\nOverall Health: ${healthPercentage}% (${healthyServices}/${criticalServices.length} services)");

if (healthPercentage < 100) {
  console.log(`⚠️  System requires attention`);
  exit(1);
} else {
  console.log(`✅ System is healthy`);
}
```

### Process Management

```typescript
script.description("Monitor and manage system processes");

// Find resource-intensive processes
function findResourceIntensiveProcesses(): void {
  console.log("Top CPU consumers:");
  `$(ps aux --sort=-%cpu | head -10)`;

  console.log("\nTop Memory consumers:");
  `$(ps aux --sort=-%mem | head -10)`;
}

// Kill processes by pattern
function killProcessesByPattern(pattern: string): void {
  let pids: string[] = "$(pgrep -f "${pattern}")".split("\n");

  for (let pid: string in pids) {
    if (pid.trim() != "") {
      console.log("Killing process ${pid} (${pattern})");
      "$(kill -TERM ${pid})";

      // Wait a moment, then force kill if still running
      `$(sleep 5)`;
      if ("$(kill -0 ${pid} 2>/dev/null; echo $?)" == "0") {
        console.log("Force killing process ${pid}");
        "$(kill -KILL ${pid})";
      }
    }
  }
}

// Monitor process count
function checkProcessLimits(): void {
  let totalProcesses: number = parseInt(`$(ps aux | wc -l)`);
  let maxProcesses: number = parseInt(`$(ulimit -u)`);
  let usage: number = Math.round((totalProcesses / maxProcesses) * 100);

  console.log("Process usage: ${totalProcesses}/${maxProcesses} (${usage}%)");

  if (usage > 80) {
    console.log(`⚠️  High process usage detected`);
    findResourceIntensiveProcesses();
  }
}
```

### Disk Space Management

```typescript
script.description("Monitor and manage disk space");

args.define("--threshold", "-t", "Disk usage threshold (%)", "number", false, 80);
args.define("--cleanup", "-c", "Perform automatic cleanup", "boolean", false, false);

let threshold: number = args.get("--threshold");
let autoCleanup: boolean = args.has("--cleanup");

// Check disk usage for all mounted filesystems
function checkDiskUsage(): string[] {
  let problematicMounts: string[] = [];
  let diskInfo: string[] = `$(df -h | grep -v Filesystem)`.split("\n");

  console.log("Disk Usage Report:");
  console.log("Filesystem      Size  Used Avail Use% Mounted on");

  for (let line: string in diskInfo) {
    if (line.trim() != "") {
      console.log(line);

      // Extract usage percentage
      let fields: string[] = line.split(/\s+/);
      if (fields.length >= 5) {
        let usageStr: string = fields[4].replace("%", "");
        let usage: number = parseInt(usageStr);
        let mountPoint: string = fields[5];

        if (usage > threshold) {
          problematicMounts.push(mountPoint);
          console.log("⚠️  ${mountPoint} is ${usage}% full (threshold: ${threshold}%)");
        }
      }
    }
  }

  return problematicMounts;
}

// Clean up common temporary files
function performCleanup(mountPoint: string): void {
  console.log("Performing cleanup on ${mountPoint}...");

  // Clean temporary files
  if (mountPoint == "/" || mountPoint == "/tmp") {
    console.log("Cleaning /tmp directory...");
    `$(find /tmp -type f -atime +7 -delete 2>/dev/null || true)`;

    console.log("Cleaning old log files...");
    `$(find /var/log -name "*.log.*" -mtime +30 -delete 2>/dev/null || true)`;

    console.log("Cleaning package manager cache...");
    if (os.isInstalled("apt")) {
      `$(apt-get clean 2>/dev/null || true)`;
    }
    if (os.isInstalled("yum")) {
      `$(yum clean all 2>/dev/null || true)`;
    }
  }

  // Find and report large files
  console.log("Finding large files (>100MB)...");
  let largeFiles: string[] = "$(find ${mountPoint} -type f -size +100M 2>/dev/null | head -10)".split("\n");

  for (let file: string in largeFiles) {
    if (file.trim() != "") {
      let size: string = "$(du -h "${file}" | cut -f1)";
      console.log("Large file: ${file} (${size})");
    }
  }
}

// Main execution
let problematicMounts: string[] = checkDiskUsage();

if (problematicMounts.length > 0) {
  if (autoCleanup) {
    for (let mount: string in problematicMounts) {
      performCleanup(mount);
    }

    // Re-check after cleanup
    console.log("\nDisk usage after cleanup:");
    checkDiskUsage();
  } else {
    console.log(`\nRun with --cleanup to perform automatic cleanup`);
    exit(1);
  }
} else {
  console.log("✅ All filesystems within acceptable usage limits");
}
```

## Service Management

### Service Monitoring and Control

```typescript
script.description("Manage system services");

args.define("--service", "-s", "Service name", "string", true);
args.define("--action", "-a", "Action (start|stop|restart|status)", "string", true);

let serviceName: string = args.get("--service");
let action: string = args.get("--action");

function manageService(service: string, action: string): void {
  if (!os.isInstalled("systemctl")) {
    console.log("❌ systemctl not available");
    exit(1);
  }

  console.log("Performing ${action} on service ${service}...");

  if (action == "status") {
    "$(systemctl status ${service})";
  } else if (action == "start") {
    "$(systemctl start ${service})";

    // Verify service started
    let status: string = "$(systemctl is-active ${service})";
    if (status == "active") {
      console.log("✅ Service ${service} started successfully");
    } else {
      console.log("❌ Failed to start service ${service}");
      exit(1);
    }
  } else if (action == "stop") {
    "$(systemctl stop ${service})";

    // Verify service stopped
    let status: string = "$(systemctl is-active ${service})";
    if (status == "inactive") {
      console.log("✅ Service ${service} stopped successfully");
    } else {
      console.log("❌ Failed to stop service ${service}");
      exit(1);
    }
  } else if (action == "restart") {
    "$(systemctl restart ${service})";

    // Wait a moment and check status
    `$(sleep 3)`;
    let status: string = "$(systemctl is-active ${service})";
    if (status == "active") {
      console.log("✅ Service ${service} restarted successfully");
    } else {
      console.log("❌ Failed to restart service ${service}");
      "$(systemctl status ${service})";
      exit(1);
    }
  } else {
    console.log("❌ Unknown action: ${action}");
    console.log("Valid actions: start, stop, restart, status");
    exit(1);
  }
}

// Validate action
let validActions: string[] = ["start", "stop", "restart", "status"];
if (!validActions.contains(action)) {
  console.log("❌ Invalid action: ${action}");
  console.log("Valid actions: ${validActions.join(", ")}");
  exit(1);
}

manageService(serviceName, action);
```

### Service Health Monitoring

```typescript
script.description("Continuous service health monitoring");

let monitoredServices: string[] = ["nginx", "mysql", "redis", "docker"];
let checkInterval: number = 60; // seconds
let maxFailures: number = 3;
let failureCounts: object = {};

// Initialize failure counts
for (let service: string in monitoredServices) {
  failureCounts = json.set(failureCounts, ".${service}", 0);
}

function checkServiceHealth(service: string): boolean {
  if (!os.isInstalled("systemctl")) {
    return false;
  }

  let status: string = "$(systemctl is-active ${service} 2>/dev/null || echo "inactive")";
  return status == "active";
}

function handleServiceFailure(service: string): void {
  let currentFailures: number = json.getNumber(failureCounts, ".${service}") + 1;
  failureCounts = json.set(failureCounts, ".${service}", currentFailures);

  console.log("❌ Service ${service} is down (failure count: ${currentFailures})");

  if (currentFailures >= maxFailures) {
    console.log("🚨 Service ${service} has failed ${maxFailures} times, attempting restart...");

    "$(systemctl restart ${service})";
    `$(sleep 5)`;

    if (checkServiceHealth(service)) {
      console.log("✅ Service ${service} restarted successfully");
      failureCounts = json.set(failureCounts, ".${service}", 0); // Reset failure count
    } else {
      console.log("❌ Failed to restart service ${service}");
      // Could send alert here
    }
  }
}

// Main monitoring loop
console.log("Starting service monitoring (${monitoredServices.length} services)...");
console.log("Check interval: ${checkInterval}s, Max failures: ${maxFailures}");

while (true) {
  let timestamp: string = `$(date '+%Y-%m-%d %H:%M:%S')`;
  console.log("[${timestamp}] Checking services...");

  let healthyCount: number = 0;

  for (let service: string in monitoredServices) {
    if (checkServiceHealth(service)) {
      healthyCount++;
      // Reset failure count on successful check
      failureCounts = json.set(failureCounts, ".${service}", 0);
    } else {
      handleServiceFailure(service);
    }
  }

  console.log("Status: ${healthyCount}/${monitoredServices.length} services healthy");

  // Sleep before next check
  "$(sleep ${checkInterval})";
}
```

## Network and Security

### Network Monitoring

```typescript
script.description("Monitor network connectivity and ports");

// Check network connectivity
function checkNetworkConnectivity(): void {
  let testHosts: string[] = ["8.8.8.8", "1.1.1.1", "google.com"];

  console.log("Network Connectivity Check:");

  for (let host: string in testHosts) {
    let result: string = "$(ping -c 1 -W 5 ${host} >/dev/null 2>&1 && echo "OK" || echo "FAIL")";
    if (result.trim() == "OK") {
      console.log("✅ ${host}: Reachable");
    } else {
      console.log("❌ ${host}: Unreachable");
    }
  }
}

// Check open ports
function checkOpenPorts(): void {
  let criticalPorts: number[] = [22, 80, 443, 3306];

  console.log("\nPort Status Check:");

  for (let port: number in criticalPorts) {
    let result: string = "$(netstat -tuln | grep ":${port} " >/dev/null && echo "OPEN" || echo "CLOSED")";
    if (result.trim() == "OPEN") {
      console.log("✅ Port ${port}: Open");
    } else {
      console.log("⚠️  Port ${port}: Closed");
    }
  }
}

// Monitor network interfaces
function checkNetworkInterfaces(): void {
  console.log("\nNetwork Interfaces:");

  let interfaces: string[] = `$(ip link show | grep -E '^[0-9]+:' | awk -F': ' '{print $2}' | awk '{print $1}')`.split("\n");

  for (let interface: string in interfaces) {
    if (interface.trim() != "" && interface != "lo") {
      let status: string = "$(ip link show ${interface} | grep -q "state UP" && echo "UP" || echo "DOWN")";
      let ip: string = "$(ip addr show ${interface} | grep 'inet ' | awk '{print $2}' | cut -d'/' -f1)";

      if (status.trim() == "UP") {
        console.log("✅ ${interface}: ${status} (${ip})");
      } else {
        console.log("❌ ${interface}: ${status}");
      }
    }
  }
}

checkNetworkConnectivity();
checkOpenPorts();
checkNetworkInterfaces();
```

### Security Auditing

```typescript
script.description("Basic security audit and hardening checks");

// Check for unauthorized users
function checkUnauthorizedUsers(): void {
  console.log("User Account Audit:");

  // Check for users with UID 0 (root privileges)
  let rootUsers: string[] = `$(awk -F: '$3 == 0 {print $1}' /etc/passwd)`.split("\n");

  for (let user: string in rootUsers) {
    if (user.trim() != "") {
      if (user == "root") {
        console.log("✅ Root user: ${user} (expected)");
      } else {
        console.log("⚠️  Root-privileged user: ${user} (review required)");
      }
    }
  }

  // Check for users with empty passwords
  let emptyPasswordUsers: string[] = `$(awk -F: '$2 == "" {print $1}' /etc/shadow 2>/dev/null || echo "")`.split("\n");

  for (let user: string in emptyPasswordUsers) {
    if (user.trim() != "") {
      console.log("❌ User with empty password: ${user}");
    }
  }
}

// Check file permissions
function checkCriticalFilePermissions(): void {
  console.log("\nCritical File Permissions:");

  let criticalFiles: object = {
    "/etc/passwd": "644",
    "/etc/shadow": "640",
    "/etc/ssh/sshd_config": "600",
    "/etc/sudoers": "440"
  };

  let fileList: string[] = json.keys(criticalFiles);

  for (let file: string in fileList) {
    if (fs.exists(file)) {
      let currentPerms: string = "$(stat -c %a ${file})";
      let expectedPerms: string = json.getString(criticalFiles, ".${file}");

      if (currentPerms == expectedPerms) {
        console.log("✅ ${file}: ${currentPerms} (correct)");
      } else {
        console.log("❌ ${file}: ${currentPerms} (should be ${expectedPerms})");
      }
    } else {
      console.log("⚠️  ${file}: File not found");
    }
  }
}

// Check for suspicious processes
function checkSuspiciousProcesses(): void {
  console.log("\nProcess Security Check:");

  // Check for processes running as root
  let rootProcesses: string[] = `$(ps aux | awk '$1 == "root" && $11 !~ /^\[/ {print $11}' | sort | uniq)`.split("\n");

  let suspiciousPatterns: string[] = ["nc", "netcat", "telnet"];

  for (let process: string in rootProcesses) {
    if (process.trim() != "") {
      for (let pattern: string in suspiciousPatterns) {
        if (process.contains(pattern)) {
          console.log("⚠️  Suspicious root process: ${process}");
        }
      }
    }
  }
}

// Check SSH configuration
function checkSSHSecurity(): void {
  console.log("\nSSH Security Check:");

  let sshConfig: string = "/etc/ssh/sshd_config";
  if (fs.exists(sshConfig)) {
    let content: string = fs.readFile(sshConfig);

    // Check if root login is disabled
    if (content.contains("PermitRootLogin no")) {
      console.log("✅ Root SSH login disabled");
    } else {
      console.log("❌ Root SSH login may be enabled");
    }

    // Check if password authentication is disabled
    if (content.contains("PasswordAuthentication no")) {
      console.log("✅ SSH password authentication disabled");
    } else {
      console.log("⚠️  SSH password authentication may be enabled");
    }

    // Check SSH port
    let portLine: string = "$(grep -E '^Port' ${sshConfig} || echo "Port 22")";
    if (portLine.contains("Port 22")) {
      console.log("⚠️  SSH running on default port 22");
    } else {
      console.log("✅ SSH running on custom port: ${portLine}");
    }
  }
}

checkUnauthorizedUsers();
checkCriticalFilePermissions();
checkSuspiciousProcesses();
checkSSHSecurity();
```

## Log Management

### Log Analysis and Rotation

```typescript
script.description("Analyze and manage system logs");

args.define("--log-dir", "-d", "Log directory", "string", false, "/var/log");
args.define("--days", "-n", "Number of days to analyze", "number", false, 7);

let logDir: string = args.get("--log-dir");
let analysisDays: number = args.get("--days");

// Analyze system logs for errors
function analyzeSystemLogs(): void {
  console.log("Analyzing system logs for the last ${analysisDays} days...");

  let logFiles: string[] = ["${logDir}/syslog", "${logDir}/auth.log", "${logDir}/kern.log"];

  for (let logFile: string in logFiles) {
    if (fs.exists(logFile)) {
      console.log("\nAnalyzing ${logFile}:");

      // Count error types
      let errors: number = parseInt("$(grep -i error ${logFile} | wc -l)");
      let warnings: number = parseInt("$(grep -i warning ${logFile} | wc -l)");
      let failed: number = parseInt("$(grep -i failed ${logFile} | wc -l)");

      console.log("  Errors: ${errors}");
      console.log("  Warnings: ${warnings}");
      console.log("  Failed: ${failed}");

      // Show recent critical errors
      if (errors > 0) {
        console.log("  Recent errors:");
        "$(grep -i error ${logFile} | tail -5)";
      }
    }
  }
}

// Rotate and compress old logs
function rotateLogFiles(): void {
  console.log("\nRotating log files older than ${analysisDays} days...");

  let oldLogs: string[] = "$(find ${logDir} -name "*.log" -mtime +${analysisDays} -type f)".split("\n");

  for (let logFile: string in oldLogs) {
    if (logFile.trim() != "") {
      let compressedFile: string = "${logFile}.gz";

      console.log("Compressing ${logFile}...");
      "$(gzip "${logFile}")";

      if (fs.exists(compressedFile)) {
        console.log("✅ Compressed: ${compressedFile}");
      } else {
        console.log("❌ Failed to compress: ${logFile}");
      }
    }
  }
}

// Clean up very old compressed logs
function cleanOldLogs(): void {
  let retentionDays: number = analysisDays * 4; // Keep compressed logs 4x longer

  console.log("\nCleaning logs older than ${retentionDays} days...");

  let veryOldLogs: string[] = "$(find ${logDir} -name "*.gz" -mtime +${retentionDays} -type f)".split("\n");

  for (let logFile: string in veryOldLogs) {
    if (logFile.trim() != "") {
      console.log("Removing old log: ${logFile}");
      "$(rm "${logFile}")";
    }
  }
}

analyzeSystemLogs();
rotateLogFiles();
cleanOldLogs();
```

## Backup and Recovery

### System Backup

```typescript
script.description("Create system backups");

args.define("--backup-dir", "-d", "Backup destination", "string", true);
args.define("--include", "-i", "Directories to include (comma-separated)", "string", false, "/etc,/home,/var/www");
args.define("--exclude", "-e", "Patterns to exclude", "string", false, "*.tmp,*.log,cache/*");

let backupDir: string = args.get("--backup-dir");
let includeStr: string = args.get("--include");
let excludeStr: string = args.get("--exclude");

let includeDirs: string[] = includeStr.split(",");
let excludePatterns: string[] = excludeStr.split(",");

// Create backup directory
fs.createDirectory(backupDir);

let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;
let backupName: string = "system_backup_${timestamp}";
let backupPath: string = "${backupDir}/${backupName}";

console.log("Creating system backup: ${backupPath}");

// Build tar command with exclusions
let excludeArgs: string = "";
for (let pattern: string in excludePatterns) {
  excludeArgs += " --exclude="${pattern.trim()}"";
}

// Create the backup
let tarCmd: string = "tar -czf "${backupPath}.tar.gz"${excludeArgs}";
for (let dir: string in includeDirs) {
  let dirTrimmed: string = dir.trim();
  if (fs.exists(dirTrimmed)) {
    tarCmd += " "${dirTrimmed}"";
  } else {
    console.log("⚠️  Directory not found: ${dirTrimmed}");
  }
}

console.log(`Running backup command...`);
"$(${tarCmd})";

// Verify backup was created
if (fs.exists("${backupPath}.tar.gz")) {
  let backupSize: string = "$(du -h "${backupPath}.tar.gz" | cut -f1)";
  console.log("✅ Backup created successfully: ${backupPath}.tar.gz (${backupSize})");

  // Create checksum
  let checksumFile: string = "${backupPath}.sha256";
  "$(sha256sum "${backupPath}.tar.gz" > "${checksumFile}")";
  console.log("✅ Checksum created: ${checksumFile}");

  // Log backup info
  let backupInfo: object = {
    "timestamp": timestamp,
    "backup_file": "${backupPath}.tar.gz",
    "checksum_file": checksumFile,
    "included_directories": includeDirs,
    "excluded_patterns": excludePatterns,
    "size": backupSize
  };

  let infoFile: string = "${backupPath}.json";
  fs.writeFile(infoFile, json.stringify(backupInfo, true));
  console.log("📋 Backup info saved: ${infoFile}");
} else {
  console.log(`❌ Backup failed`);
  exit(1);
}
```

## Performance Monitoring

### System Performance Metrics

```typescript
script.description("Collect and analyze system performance metrics");

// Collect CPU metrics
function collectCPUMetrics(): object {
  let loadAvg: string = `$(uptime | awk -F'load average:' '{print $2}' | sed 's/^ *//')`;
  let cpuUsage: string = `$(top -bn1 | grep "Cpu(s)" | awk '{print $2}' | cut -d'%' -f1)`;

  return {
    "load_average": loadAvg.trim(),
    "cpu_usage_percent": parseFloat(cpuUsage) || 0
  };
}

// Collect memory metrics
function collectMemoryMetrics(): object {
  let memInfo: string[] = `$(free -m | grep Mem)`.split(/\s+/);

  if (memInfo.length >= 4) {
    let total: number = parseInt(memInfo[1]);
    let used: number = parseInt(memInfo[2]);
    let available: number = parseInt(memInfo[6] || memInfo[3]);

    return {
      "total_mb": total,
      "used_mb": used,
      "available_mb": available,
      "usage_percent": Math.round((used / total) * 100)
    };
  }

  return {};
}

// Collect disk metrics
function collectDiskMetrics(): object[] {
  let diskInfo: string[] = `$(df -h | grep -v Filesystem | grep -v tmpfs)`.split("\n");
  let disks: object[] = [];

  for (let line: string in diskInfo) {
    if (line.trim() != "") {
      let fields: string[] = line.split(/\s+/);
      if (fields.length >= 6) {
        let disk: object = {
          "filesystem": fields[0],
          "size": fields[1],
          "used": fields[2],
          "available": fields[3],
          "usage_percent": parseInt(fields[4].replace("%", "")),
          "mount_point": fields[5]
        };
        disks.push(disk);
      }
    }
  }

  return disks;
}

// Collect network metrics
function collectNetworkMetrics(): object {
  let interfaces: string[] = `$(ip link show | grep -E '^[0-9]+:' | awk -F': ' '{print $2}' | awk '{print $1}')`.split("\n");
  let interfaceStats: object = {};

  for (let interface: string in interfaces) {
    if (interface.trim() != "" && interface != "lo") {
      let rxBytes: string = "$(cat /sys/class/net/${interface}/statistics/rx_bytes 2>/dev/null || echo "0")";
      let txBytes: string = "$(cat /sys/class/net/${interface}/statistics/tx_bytes 2>/dev/null || echo "0")";

      interfaceStats = json.set(interfaceStats, ".${interface}", {
        "rx_bytes": parseInt(rxBytes),
        "tx_bytes": parseInt(txBytes)
      });
    }
  }

  return interfaceStats;
}

// Generate performance report
let timestamp: string = `$(date -Iseconds)`;
let hostname: string = `$(hostname)`;

let metrics: object = {
  "timestamp": timestamp,
  "hostname": hostname,
  "cpu": collectCPUMetrics(),
  "memory": collectMemoryMetrics(),
  "disk": collectDiskMetrics(),
  "network": collectNetworkMetrics()
};

// Save metrics to file
let metricsDir: string = "/var/log/system-metrics";
fs.createDirectory(metricsDir);

let metricsFile: string = "${metricsDir}/metrics-$(date +%Y%m%d).json";
fs.appendFile(metricsFile, json.stringify(metrics) + "\n");

// Display summary
console.log("System Performance Summary:");
console.log("Timestamp: ${timestamp}");
console.log("Hostname: ${hostname}");

let cpu: object = json.get(metrics, ".cpu");
let memory: object = json.get(metrics, ".memory");

console.log("CPU Usage: ${json.getNumber(cpu, ".cpu_usage_percent")}%");
console.log("Load Average: ${json.getString(cpu, ".load_average")}");
console.log("Memory Usage: ${json.getNumber(memory, ".usage_percent")}% (${json.getNumber(memory, ".used_mb")}/${json.getNumber(memory, ".total_mb")} MB)");

let disks: object[] = json.get(metrics, ".disk");
for (let disk: object in disks) {
  let mountPoint: string = json.getString(disk, ".mount_point");
  let usage: number = json.getNumber(disk, ".usage_percent");
  console.log("Disk ${mountPoint}: ${usage}%");

  if (usage > 85) {
    console.log("⚠️  High disk usage on ${mountPoint}");
  }
}

console.log("Metrics saved to: ${metricsFile}");
```

## Best Practices

### Error Handling and Logging

```typescript
// Always log administrative actions
function logAdminAction(action: string, details: string): void {
  let timestamp: string = `$(date -Iseconds)`;
  let user: string = `$(whoami)`;
  let logEntry: string = "[${timestamp}] ${user}: ${action} - ${details}";

  fs.appendFile("/var/log/admin-actions.log", logEntry + "\n");
  console.log("Logged: ${action}");
}

// Validate prerequisites before execution
function validatePrerequisites(): boolean {
  // Check if running as appropriate user
  let currentUser: string = `$(whoami)`;
  if (currentUser != "root" && currentUser != "admin") {
    console.log("❌ This script requires root or admin privileges");
    return false;
  }

  // Check required tools
  let requiredTools: string[] = ["systemctl", "ps", "df", "free"];
  for (let tool: string in requiredTools) {
    if (!os.isInstalled(tool)) {
      console.log("❌ Required tool not found: ${tool}");
      return false;
    }
  }

  return true;
}
```

### Resource Management

```typescript
// Monitor script resource usage
function monitorResourceUsage(): void {
  let startTime: number = parseInt(`$(date +%s)`);
  let scriptPid: string = `$$`;

  // Monitor memory usage periodically
  function checkMemoryUsage(): void {
    let memUsage: string = "$(ps -o pid,vsz,rss,comm -p ${scriptPid} | tail -1)";
    console.log("Memory usage: ${memUsage}");
  }

  // Set up cleanup on exit
  "$(trap 'echo "Script completed in $(($(date +%s) - ${startTime})) seconds"' EXIT)";
}
```

## Next Steps

- **[CI/CD Integration](cicd.md)** - Integrate system admin scripts in pipelines
- **[Security Best Practices](security.md)** - Advanced security hardening
- **[Performance Optimization](performance.md)** - Optimize system administration scripts

System administration with Utah provides powerful automation capabilities while maintaining security and reliability. Use these patterns to build robust infrastructure management solutions.
