---
layout: default
title: Log File Analyzer
parent: Examples
nav_order: 4
description: "Comprehensive array operations example: a log file analyzer that demonstrates Utah's powerful array handling capabilities"
permalink: /examples/log-file-analyzer/
---

## Log File Analyzer - Comprehensive Array Operations

A real-world example demonstrating Utah's powerful array handling capabilities through a log file analyzer. This script processes server logs, analyzes patterns, generates reports, and demonstrates most of Utah's array functions in practical scenarios.

## Features Demonstrated

- **Dynamic array building** from file processing
- **Array filtering and searching** with `array.contains()`
- **Data aggregation** using `array.merge()` and `array.unique()`
- **Sorting and organizing** with `array.sort()`
- **Statistical analysis** using array manipulation
- **Report generation** with `array.join()`
- **Error handling** with array validation
- **Interactive menus** using arrays
- **File system integration** with arrays

## Complete Script: `log_analyzer.shx`

```typescript
#!/usr/bin/env utah

// Log File Analyzer - Comprehensive Array Operations Demo
// Processes server logs and generates detailed reports

function main(): void {
  console.log("🔍 Utah Log File Analyzer");
  console.log("=========================");
  
  // Initialize data structures
  let logFiles: string[] = [
    "/var/log/apache2/access.log",
    "/var/log/nginx/access.log", 
    "/var/log/app/application.log"
  ];
  
  let allLogEntries: string[] = [];
  let errorEntries: string[] = [];
  let warningEntries: string[] = [];
  let ipAddresses: string[] = [];
  let userAgents: string[] = [];
  
  // For demo purposes, use sample data
  console.log("Using sample data for demonstration...");
  createSampleData(allLogEntries, errorEntries, warningEntries, ipAddresses);
  
  // Generate comprehensive report
  generateReport(allLogEntries, errorEntries, warningEntries, ipAddresses, userAgents);
  
  // Interactive analysis menu
  showAnalysisMenu(allLogEntries, errorEntries, warningEntries, ipAddresses);
}

function createSampleData(
  allEntries: string[],
  errors: string[],
  warnings: string[],
  ips: string[]
): void {
  // Sample log entries
  let sampleLogs: string[] = [
    "2024-01-15 10:30:45 [INFO] User login successful - IP: 192.168.1.100",
    "2024-01-15 10:31:12 [ERROR] Database connection failed - timeout after 30s",
    "2024-01-15 10:31:45 [WARN] High memory usage detected - 85% utilization",
    "2024-01-15 10:32:01 [INFO] File upload completed - size: 2.5MB",
    "2024-01-15 10:32:30 [ERROR] 404 Not Found - /api/missing-endpoint",
    "2024-01-15 10:33:15 [INFO] Cache refresh completed - 1500 entries updated",
    "2024-01-15 10:33:45 [WARN] Slow query detected - execution time: 2.3s",
    "2024-01-15 10:34:12 [ERROR] Connection timeout - host unreachable"
  ];
  
  // Process sample data
  for (let log: string in sampleLogs) {
    array.push(allEntries, log);
    
    // Categorize by log level
    if (string.includes(log, "ERROR")) {
      array.push(errors, log);
    } else if (string.includes(log, "WARN")) {
      array.push(warnings, log);
    }
    
    // Extract sample IP addresses
    if (string.includes(log, "192.168.1.100")) {
      if (!array.contains(ips, "192.168.1.100")) {
        array.push(ips, "192.168.1.100");
      }
    }
  }
  
  // Add more sample IPs for demonstration
  array.push(ips, "203.0.113.45");
  array.push(ips, "198.51.100.78");
  array.push(ips, "10.0.0.15");
}

function generateReport(
  allEntries: string[],
  errors: string[],
  warnings: string[],
  ips: string[],
  agents: string[]
): void {
  console.log("📋 COMPREHENSIVE LOG ANALYSIS REPORT");
  console.log("=====================================");
  
  // Basic statistics using array.length()
  let totalEntries: number = array.length(allEntries);
  let errorCount: number = array.length(errors);
  let warningCount: number = array.length(warnings);
  let ipCount: number = array.length(ips);
  
  console.log("📊 Log Entry Statistics:");
  console.log("Total entries processed:");
  console.log(totalEntries);
  console.log("Error entries found:");
  console.log(errorCount);
  console.log("Warning entries found:");
  console.log(warningCount);
  console.log("Unique IP addresses:");
  console.log(ipCount);
  
  // Error analysis
  let errorsEmpty: boolean = array.isEmpty(errors);
  if (!errorsEmpty) {
    console.log("🚨 Error Analysis:");
    analyzeErrors(errors);
  }
  
  // Warning analysis  
  let warningsEmpty: boolean = array.isEmpty(warnings);
  if (!warningsEmpty) {
    console.log("⚠️  Warning Analysis:");
    analyzeWarnings(warnings);
  }
  
  // IP address analysis
  let ipsEmpty: boolean = array.isEmpty(ips);
  if (!ipsEmpty) {
    console.log("🌐 IP Address Analysis:");
    analyzeIPAddresses(ips);
  }
}

function analyzeErrors(errors: string[]): void {
  // Count error types
  let errorTypes: string[] = [];
  let errorCounts: number[] = [];
  
  for (let error: string in errors) {
    let errorType: string = extractErrorType(error);
    let index: number = findOrAddErrorType(errorTypes, errorCounts, errorType);
    errorCounts[index] = errorCounts[index] + 1;
  }
  
  // Display error types
  console.log("Error type breakdown:");
  let typeCount: number = array.length(errorTypes);
  for (let i: number = 0; i < typeCount; i++) {
    let type: string = errorTypes[i];
    let count: number = errorCounts[i];
    console.log(type);
    console.log("occurrences:");
    console.log(count);
  }
}

function extractErrorType(errorLine: string): string {
  // Extract error type from log line (simplified)
  if (string.includes(errorLine, "404")) return "404 Not Found";
  if (string.includes(errorLine, "500")) return "500 Internal Server Error";
  if (string.includes(errorLine, "timeout")) return "Timeout Error";
  if (string.includes(errorLine, "connection")) return "Connection Error";
  return "Other Error";
}

function findOrAddErrorType(types: string[], counts: number[], errorType: string): number {
  let typeCount: number = array.length(types);
  for (let i: number = 0; i < typeCount; i++) {
    if (types[i] == errorType) {
      return i;
    }
  }
  
  // Not found, add new type
  array.push(types, errorType);
  array.push(counts, 0);
  let newLength: number = array.length(types);
  return newLength - 1;
}

function analyzeWarnings(warnings: string[]): void {
  let warningCount: number = array.length(warnings);
  let displayCount: number = warningCount > 3 ? 3 : warningCount;
  
  console.log("Recent warnings:");
  for (let i: number = warningCount - displayCount; i < warningCount; i++) {
    let warning: string = warnings[i];
    console.log(warning);
  }
}

function analyzeIPAddresses(ips: string[]): void {
  // Sort IPs for better readability using array.sort()
  let sortedIPs: string[] = array.sort(ips);
  
  let ipCount: number = array.length(sortedIPs);
  console.log("Found unique IP addresses:");
  console.log(ipCount);
  
  // Categorize by type
  let internalIPs: string[] = [];
  let externalIPs: string[] = [];
  
  for (let ip: string in sortedIPs) {
    if (string.startsWith(ip, "192.168.") || 
        string.startsWith(ip, "10.") || 
        string.startsWith(ip, "172.")) {
      array.push(internalIPs, ip);
    } else {
      array.push(externalIPs, ip);
    }
  }
  
  let internalCount: number = array.length(internalIPs);
  let externalCount: number = array.length(externalIPs);
  console.log("Internal IPs:");
  console.log(internalCount);
  console.log("External IPs:");
  console.log(externalCount);
  
  // Show sample external IPs using array.join()
  let extEmpty: boolean = array.isEmpty(externalIPs);
  if (!extEmpty) {
    let sampleCount: number = externalCount > 3 ? 3 : externalCount;
    console.log("Sample external IPs:");
    for (let i: number = 0; i < sampleCount; i++) {
      console.log(externalIPs[i]);
    }
  }
}

function showAnalysisMenu(
  allEntries: string[],
  errors: string[],
  warnings: string[],
  ips: string[]
): void {
  console.log("🎯 INTERACTIVE ANALYSIS MENU");
  console.log("=============================");
  
  let menuOptions: string[] = [
    "Show detailed error analysis",
    "Export IP addresses to file",
    "Search log entries by keyword",
    "Generate summary report",
    "Show array statistics",
    "Exit"
  ];
  
  console.log("Available options:");
  let optionCount: number = array.length(menuOptions);
  for (let i: number = 0; i < optionCount; i++) {
    let option: string = menuOptions[i];
    let menuNumber: number = i + 1;
    console.log(menuNumber);
    console.log(". ");
    console.log(option);
  }
  
  // For demo purposes, we'll simulate selecting option 5 (array statistics)
  let choice: number = 5;
  let selectedOption: string = menuOptions[choice - 1];
  console.log("Selected option:");
  console.log(choice);
  console.log(selectedOption);
  
  if (choice == 5) {
    showArrayStatistics(allEntries, errors, warnings, ips);
  }
}

function showArrayStatistics(
  allEntries: string[],
  errors: string[],
  warnings: string[],
  ips: string[]
): void {
  console.log("📈 ARRAY OPERATIONS STATISTICS");
  console.log("===============================");
  
  // Demonstrate array functions
  console.log("Array function demonstrations:");
  
  // array.length() demonstrations
  let totalCount: number = array.length(allEntries);
  let errorCount: number = array.length(errors);
  let warningCount: number = array.length(warnings);
  let ipCount: number = array.length(ips);
  
  console.log("🔢 array.length() results:");
  console.log("All entries:");
  console.log(totalCount);
  console.log("Errors:");
  console.log(errorCount);
  console.log("Warnings:");
  console.log(warningCount);
  console.log("IPs:");
  console.log(ipCount);
  
  // array.isEmpty() demonstrations
  let errorsEmpty: boolean = array.isEmpty(errors);
  let warningsEmpty: boolean = array.isEmpty(warnings);
  let ipsEmpty: boolean = array.isEmpty(ips);
  
  console.log("❓ array.isEmpty() results:");
  console.log("Errors empty:");
  console.log(errorsEmpty);
  console.log("Warnings empty:");
  console.log(warningsEmpty);
  console.log("IPs empty:");
  console.log(ipsEmpty);
  
  // array.contains() demonstrations
  if (!ipsEmpty) {
    let firstIP: string = ips[0];
    let hasFirstIP: boolean = array.contains(ips, firstIP);
    let hasFakeIP: boolean = array.contains(ips, "999.999.999.999");
    
    console.log("🔍 array.contains() examples:");
    console.log("IPs contains first IP:");
    console.log(hasFirstIP);
    console.log("IPs contains fake IP:");
    console.log(hasFakeIP);
  }
  
  // array.merge() demonstration
  let sampleErrors: string[] = ["Error 1", "Error 2"];
  let sampleWarnings: string[] = ["Warning 1", "Warning 2"];
  let combined: string[] = array.merge(sampleErrors, sampleWarnings);
  
  let sampleErrorCount: number = array.length(sampleErrors);
  let sampleWarningCount: number = array.length(sampleWarnings);
  let combinedCount: number = array.length(combined);
  
  console.log("🔗 array.merge() example:");
  console.log("Merged");
  console.log(sampleErrorCount);
  console.log("errors +");
  console.log(sampleWarningCount);
  console.log("warnings =");
  console.log(combinedCount);
  console.log("items");
  
  // array.unique() demonstration
  let duplicatedList: string[] = ["item1", "item2", "item1", "item3", "item2"];
  let uniqueList: string[] = array.unique(duplicatedList);
  
  let originalCount: number = array.length(duplicatedList);
  let uniqueCount: number = array.length(uniqueList);
  
  console.log("⭐ array.unique() example:");
  console.log("Original:");
  console.log(originalCount);
  console.log("items, Unique:");
  console.log(uniqueCount);
  console.log("items");
  
  // array.sort() demonstration
  let unsortedNumbers: string[] = ["3", "1", "4", "1", "5"];
  let sortedNumbers: string[] = array.sort(unsortedNumbers);
  
  let unsortedList: string = array.join(unsortedNumbers, ", ");
  let sortedList: string = array.join(sortedNumbers, ", ");
  
  console.log("📊 array.sort() example:");
  console.log("Unsorted:");
  console.log(unsortedList);
  console.log("Sorted:");
  console.log(sortedList);
  
  // array.shuffle() demonstration  
  let orderedList: string[] = ["A", "B", "C", "D", "E"];
  let shuffledList: string[] = array.shuffle(orderedList);
  
  let orderedJoined: string = array.join(orderedList, ", ");
  let shuffledJoined: string = array.join(shuffledList, ", ");
  
  console.log("🎲 array.shuffle() example:");
  console.log("Original:");
  console.log(orderedJoined);
  console.log("Shuffled:");
  console.log(shuffledJoined);
  
  // array.reverse() demonstration
  let normalOrder: string[] = ["first", "second", "third"];
  let reversedOrder: string[] = array.reverse(normalOrder);
  
  let normalJoined: string = array.join(normalOrder, " → ");
  let reversedJoined: string = array.join(reversedOrder, " ← ");
  
  console.log("🔄 array.reverse() example:");
  console.log("Normal:");
  console.log(normalJoined);
  console.log("Reversed:");
  console.log(reversedJoined);
  
  // array.join() demonstrations
  console.log("🔗 array.join() examples:");
  let fruits: string[] = ["apple", "banana", "cherry"];
  
  let commaSeparated: string = array.join(fruits, ", ");
  let pipeSeparated: string = array.join(fruits, " | ");
  let spaceSeparated: string = array.join(fruits, " ");
  
  console.log("Comma-separated:");
  console.log(commaSeparated);
  console.log("Pipe-separated:");
  console.log(pipeSeparated);
  console.log("Space-separated:");
  console.log(spaceSeparated);
}

// Run the main function
main();
```

## Quick Reference: Array Functions Used

This comprehensive example demonstrates all of Utah's array functions:

| Function | Purpose | Example Usage |
|----------|---------|---------------|
| `array.length(arr)` | Get array size | `array.length(logEntries)` |
| `array.isEmpty(arr)` | Check if empty | `array.isEmpty(errors)` |
| `array.contains(arr, item)` | Search for item | `array.contains(ips, "192.168.1.1")` |
| `array.push(arr, item)` | Add to end | `array.push(errors, errorMsg)` |
| `array.merge(arr1, arr2)` | Combine arrays | `array.merge(errors, warnings)` |
| `array.unique(arr)` | Remove duplicates | `array.unique(ipAddresses)` |
| `array.sort(arr)` | Sort alphabetically | `array.sort(usernames)` |
| `array.shuffle(arr)` | Randomize order | `array.shuffle(sampleData)` |
| `array.reverse(arr)` | Reverse order | `array.reverse(chronological)` |
| `array.join(arr, sep)` | Convert to string | `array.join(items, ", ")` |

## Usage Examples

### Running the Log Analyzer

```bash
# Compile the script
utah compile log_analyzer.shx

# Run with your log files
./log_analyzer.sh

# Or run directly
utah run log_analyzer.shx
```

### Expected Output

```text
🔍 Utah Log File Analyzer
=========================
Using sample data for demonstration...

📋 COMPREHENSIVE LOG ANALYSIS REPORT
=====================================

📊 Log Entry Statistics:
Total entries processed:
8
Error entries found:
3
Warning entries found:
2
Unique IP addresses:
4

🚨 Error Analysis:
Error type breakdown:
Timeout Error
occurrences:
2
404 Not Found
occurrences:
1

⚠️  Warning Analysis:
Recent warnings:
2024-01-15 10:31:45 [WARN] High memory usage detected - 85% utilization
2024-01-15 10:33:45 [WARN] Slow query detected - execution time: 2.3s

🌐 IP Address Analysis:
Found unique IP addresses:
4
Internal IPs:
2
External IPs:
2
Sample external IPs:
198.51.100.78
203.0.113.45

🎯 INTERACTIVE ANALYSIS MENU
=============================
Available options:
1
. 
Show detailed error analysis
2
. 
Export IP addresses to file
[...continues...]
```

## Key Array Concepts in Utah

### Strongly Typed Arrays

Utah enforces type safety for arrays, preventing runtime errors:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];      // Numbers only
let names: string[] = ["Alice", "Bob"];        // Strings only  
let flags: boolean[] = [true, false, true];    // Booleans only
```

### Dynamic Array Building

Arrays can be built dynamically during script execution:

```typescript
let logEntries: string[] = [];                 // Start empty
array.push(logEntries, "First entry");         // Add entries
array.push(logEntries, "Second entry");        // Build gradually
```

### Array Iteration Patterns

Utah provides clean iteration syntax:

```typescript
// For-in loop (recommended)
for (let entry: string in logEntries) {
  console.log("Processing:");
  console.log(entry);
}

// Index-based loop
let entryCount: number = array.length(logEntries);
for (let i: number = 0; i < entryCount; i++) {
  let entry: string = logEntries[i];
  console.log("Entry");
  console.log(i);
  console.log(":");
  console.log(entry);
}
```

### Integration with Other Utah Features

Arrays work seamlessly with Utah's other features:

```typescript
// File system integration
let configFiles: string[] = ["/etc/app.conf", "/etc/db.conf"];
for (let file: string in configFiles) {
  if (fs.exists(file)) {
    let content: string = fs.readFile(file);
    // Process content...
  }
}

// String processing
let csvData: string = "user1,user2,user3";
let users: string[] = string.split(csvData, ",");

// Console interaction  
for (let user: string in users) {
  console.log("Processing user:");
  console.log(user);
}
```

## Advanced Array Patterns Demonstrated

### 1. Data Aggregation Pattern

```typescript
// Collect data from multiple sources
let allData: string[] = [];
let sources: string[] = ["source1.log", "source2.log", "source3.log"];

for (let source: string in sources) {
  let sourceData: string[] = processSource(source);
  allData = array.merge(allData, sourceData);
}
```

### 2. Classification Pattern

```typescript
// Classify items into categories
let errors: string[] = [];
let warnings: string[] = [];
let info: string[] = [];

for (let entry: string in allEntries) {
  if (string.includes(entry, "ERROR")) {
    array.push(errors, entry);
  } else if (string.includes(entry, "WARN")) {
    array.push(warnings, entry);
  } else {
    array.push(info, entry);
  }
}
```

### 3. Deduplication Pattern

```typescript
// Remove duplicates from combined data
let rawIPs: string[] = [];
// ... collect IPs from various sources ...
let uniqueIPs: string[] = array.unique(rawIPs);
```

### 4. Analysis Pattern

```typescript
// Analyze and report on data
let sortedData: string[] = array.sort(uniqueIPs);
let report: string = array.join(sortedData, "\n");
console.log("Sorted unique IPs:");
console.log(report);
```

### 5. Menu/Selection Pattern

```typescript
// Interactive menus using arrays
let options: string[] = [
  "View errors",
  "Export data", 
  "Generate report",
  "Exit"
];

let optionCount: number = array.length(options);
for (let i: number = 0; i < optionCount; i++) {
  let menuNumber: number = i + 1;
  console.log(menuNumber);
  console.log(". ");
  console.log(options[i]);
}
```

## Benefits Over Traditional Bash Arrays

### Utah Arrays (Type-Safe and Intuitive)

```typescript
let servers: string[] = ["web01", "web02", "db01"];
let serverCount: number = array.length(servers);
console.log("Server count:");
console.log(serverCount);

let hasWeb01: boolean = array.contains(servers, "web01");
if (hasWeb01) {
  console.log("Web01 is configured");
}

let allServers: string[] = array.merge(servers, backupServers);
```

### Bash Arrays (Complex and Error-Prone)

```bash
servers=("web01" "web02" "db01")
echo "Server count: ${#servers[@]}"

# Complex contains check
if [[ " ${servers[@]} " =~ " web01 " ]]; then
  echo "Web01 is configured"
fi

# Complex merge
allServers=("${servers[@]}" "${backupServers[@]}")
```

### Key Advantages

- **Type Safety**: Compile-time type checking prevents errors
- **Intuitive Syntax**: Clear, readable array operations
- **Built-in Functions**: Rich set of array manipulation methods
- **Error Prevention**: Bounds checking and validation
- **Better Performance**: Optimized implementations
- **Consistent API**: All array functions follow same patterns

## Real-World Applications

This log analyzer pattern can be adapted for many scenarios:

- **System Monitoring**: Process system logs and metrics
- **Data Processing**: Parse CSV files, API responses
- **Configuration Management**: Handle multiple config files
- **Reporting**: Generate summaries from multiple sources
- **File Processing**: Batch process multiple files
- **User Management**: Handle user lists and permissions

## Related Examples

- [String Processing](string-processing) - Working with text arrays
- [System Health Monitor](system-health-monitor) - Arrays in monitoring
- [Loops](loops) - Advanced iteration with arrays

## Extension Ideas

You can extend this example to include:

- Database integration with array-based queries
- Network monitoring with IP address arrays
- Performance metrics collection
- Automated alerting based on array analysis
- Export functionality to multiple formats
- Interactive filtering and searching
- Real-time log monitoring with arrays
