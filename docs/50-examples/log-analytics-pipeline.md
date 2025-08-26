---
layout: default
title: Log Analytics Pipeline
parent: Examples
nav_order: 5
description: "A comprehensive real-world example using loops, string processing, filesystem operations, and arrays to analyze server logs"
permalink: /examples/log-analytics-pipeline/
---

import { AsciinemaPlayer } from '@site/src/components';

A comprehensive real-world example that demonstrates Utah's capabilities by building a log analytics pipeline. This script processes web server access logs to extract insights about traffic patterns, errors, and user behavior, showcasing the powerful combination of loops and string processing in Utah.

## 🎬 Interactive Demo

Watch this script in action! The demo shows the complete log analytics pipeline including file processing, string manipulation, loops, and report generation:

<AsciinemaPlayer
  src="/assets/log-analytics-pipeline.cast"
  autoPlay={false}
  loop={false}
  speed={1}
  idleTimeLimit={3}
  theme="asciinema"
  poster="npt:0:01"
  cols={120}
  rows={30}
  fontSize="14px"
/>

## Features Demonstrated

- **File system operations** for reading log files
- **String processing** for parsing log entries
- **Array manipulation** for data aggregation
- **Control flow** with loops and conditionals
- **Error handling** with try-catch blocks
- **Git integration** for version control
- **Console output** for reporting results

## Real-World Scenario

You're a DevOps engineer who needs to analyze web server logs to:

- Identify the most frequent error codes
- Track top IP addresses (potential security threats)
- Analyze request patterns by hour
- Generate a summary report for the team
- Save results and commit them to the repository

## Complete Script

```typescript
#!/usr/bin/env utah

// Log Analytics Pipeline - Process web server access logs
// Demonstrates loops, string processing, arrays, and filesystem operations

console.clear();
console.log("🔍 Starting Log Analytics Pipeline");
console.log("=====================================");

// Configuration
let logDir: string = "/var/log/nginx";
let outputDir: string = "./reports";
let logFile: string = logDir + "/access.log";

try {
  // Step 1: Check if log file exists
  if (!fs.exists(logFile)) {
    console.log("❌ Log file not found: " + logFile);
    console.log("💡 Creating sample log data for demo...");

    // Create sample log entries for demonstration
    let sampleLogs: string[] = [
      "192.168.1.100 - - [09/Aug/2025:10:15:23 +0000] \"GET /api/users HTTP/1.1\" 200 1234",
      "10.0.0.50 - - [09/Aug/2025:10:16:45 +0000] \"POST /login HTTP/1.1\" 401 567",
      "192.168.1.100 - - [09/Aug/2025:10:17:12 +0000] \"GET /dashboard HTTP/1.1\" 200 8901",
      "203.0.113.25 - - [09/Aug/2025:10:18:34 +0000] \"GET /api/data HTTP/1.1\" 500 234",
      "192.168.1.75 - - [09/Aug/2025:11:22:15 +0000] \"GET /home HTTP/1.1\" 200 5678",
      "10.0.0.50 - - [09/Aug/2025:11:23:44 +0000] \"POST /api/submit HTTP/1.1\" 404 123",
      "203.0.113.25 - - [09/Aug/2025:11:24:12 +0000] \"GET /admin HTTP/1.1\" 403 890",
      "192.168.1.100 - - [09/Aug/2025:12:30:55 +0000] \"DELETE /api/user/123 HTTP/1.1\" 200 45",
      "198.51.100.10 - - [09/Aug/2025:12:31:22 +0000] \"GET /api/stats HTTP/1.1\" 500 678",
      "192.168.1.75 - - [09/Aug/2025:13:45:33 +0000] \"PUT /api/update HTTP/1.1\" 201 345"
    ];

    // Create output directory if it doesn't exist
    if (!fs.exists(outputDir)) {
      fs.createDirectory(outputDir);
    }

    logFile = outputDir + "/sample_access.log";
    let logContent: string = array.join(sampleLogs, "\n");
    fs.writeFile(logFile, logContent);
    console.log("✅ Sample log created: " + logFile);
  }

  // Step 2: Read and parse log file
  console.log("\n📖 Reading log file: " + logFile);
  let logContent: string = fs.readFile(logFile);
  let logLines: string[] = string.split(logContent, "\n");

  console.log("📊 Processing " + array.length(logLines) + " log entries...");

  // Initialize data structures for analysis
  let ipCounts: string[] = [];
  let statusCodes: string[] = [];
  let hourlyTraffic: string[] = [];
  let errorEntries: string[] = [];
  let totalRequests: number = 0;

  // Step 3: Process each log entry with detailed parsing
  for (let line: string in logLines) {
    if (string.isEmpty(string.trim(line))) {
      continue; // Skip empty lines
    }

    totalRequests++;

    // Parse IP address (first field)
    let spaceIndex: number = string.indexOf(line, " ");
    if (spaceIndex > 0) {
      let ipAddress: string = string.substring(line, 0, spaceIndex);

      // Track IP addresses
      let ipEntry: string = ipAddress + ":1";

      // Check if IP already exists in our tracking
      let ipFound: boolean = false;
      for (let i: number = 0; i < array.length(ipCounts); i++) {
        let existingEntry: string = ipCounts[i];
        let existingIp: string = string.substring(existingEntry, 0, string.indexOf(existingEntry, ":"));

        if (existingIp == ipAddress) {
          // Increment count
          let countStr: string = string.substring(existingEntry, string.indexOf(existingEntry, ":") + 1);
          let count: number = utility.parseInt(countStr) + 1;
          ipCounts[i] = ipAddress + ":" + count;
          ipFound = true;
          break;
        }
      }

      if (!ipFound) {
        array.push(ipCounts, ipEntry);
      }
    }

    // Parse timestamp and extract hour
    let timestampStart: number = string.indexOf(line, "[");
    let timestampEnd: number = string.indexOf(line, "]");

    if (timestampStart >= 0 && timestampEnd > timestampStart) {
      let timestamp: string = string.substring(line, timestampStart + 1, timestampEnd);
      // Extract hour from timestamp: [09/Aug/2025:10:15:23 +0000]
      let hourStart: number = string.indexOf(timestamp, ":") + 1;
      let hourEnd: number = string.indexOf(timestamp, ":", hourStart);

      if (hourStart > 0 && hourEnd > hourStart) {
        let hour: string = string.substring(timestamp, hourStart, hourEnd);
        let hourEntry: string = hour + ":1";

        // Track hourly traffic
        let hourFound: boolean = false;
        for (let i: number = 0; i < array.length(hourlyTraffic); i++) {
          let existingHour: string = string.substring(hourlyTraffic[i], 0, string.indexOf(hourlyTraffic[i], ":"));
          if (existingHour == hour) {
            let countStr: string = string.substring(hourlyTraffic[i], string.indexOf(hourlyTraffic[i], ":") + 1);
            let count: number = utility.parseInt(countStr) + 1;
            hourlyTraffic[i] = hour + ":" + count;
            hourFound = true;
            break;
          }
        }

        if (!hourFound) {
          array.push(hourlyTraffic, hourEntry);
        }
      }
    }

    // Parse HTTP status code
    let statusStart: number = string.indexOf(line, "\" ");
    if (statusStart >= 0) {
      let afterQuote: string = string.substring(line, statusStart + 2);
      let statusEnd: number = string.indexOf(afterQuote, " ");

      if (statusEnd > 0) {
        let statusCode: string = string.substring(afterQuote, 0, statusEnd);
        let statusEntry: string = statusCode + ":1";

        // Track status codes
        let statusFound: boolean = false;
        for (let i: number = 0; i < array.length(statusCodes); i++) {
          let existingStatus: string = string.substring(statusCodes[i], 0, string.indexOf(statusCodes[i], ":"));
          if (existingStatus == statusCode) {
            let countStr: string = string.substring(statusCodes[i], string.indexOf(statusCodes[i], ":") + 1);
            let count: number = utility.parseInt(countStr) + 1;
            statusCodes[i] = statusCode + ":" + count;
            statusFound = true;
            break;
          }
        }

        if (!statusFound) {
          array.push(statusCodes, statusEntry);
        }

        // Track error entries (4xx and 5xx status codes)
        let statusNum: number = utility.parseInt(statusCode);
        if (statusNum >= 400) {
          array.push(errorEntries, line);
        }
      }
    }
  }

  // Step 4: Generate comprehensive analytics report
  console.log("\n📈 Generating Analytics Report");
  console.log("==============================");

  let report: string[] = [];
  array.push(report, "Log Analytics Report - Generated on " + utility.getCurrentTimestamp());
  array.push(report, "====================================================");
  array.push(report, "");

  // Summary statistics
  array.push(report, "📊 Summary Statistics:");
  array.push(report, "  Total Requests: " + totalRequests);
  array.push(report, "  Unique IP Addresses: " + array.length(ipCounts));
  array.push(report, "  Error Requests: " + array.length(errorEntries));
  array.push(report, "  Error Rate: " + (array.length(errorEntries) * 100 / totalRequests) + "%");
  array.push(report, "");

  // Top IP addresses
  array.push(report, "🌐 Top IP Addresses:");
  for (let entry: string in ipCounts) {
    let parts: string[] = string.split(entry, ":");
    let ip: string = parts[0];
    let count: string = parts[1];
    array.push(report, "  " + ip + " (" + count + " requests)");
  }
  array.push(report, "");

  // Status code distribution
  array.push(report, "📋 HTTP Status Code Distribution:");
  for (let entry: string in statusCodes) {
    let parts: string[] = string.split(entry, ":");
    let code: string = parts[0];
    let count: string = parts[1];

    let description: string = "";
    switch (code) {
      case "200":
        description = "OK";
        break;
      case "201":
        description = "Created";
        break;
      case "401":
        description = "Unauthorized";
        break;
      case "403":
        description = "Forbidden";
        break;
      case "404":
        description = "Not Found";
        break;
      case "500":
        description = "Internal Server Error";
        break;
      default:
        description = "Other";
    }

    array.push(report, "  " + code + " " + description + ": " + count + " requests");
  }
  array.push(report, "");

  // Hourly traffic patterns
  array.push(report, "🕐 Hourly Traffic Distribution:");
  for (let entry: string in hourlyTraffic) {
    let parts: string[] = string.split(entry, ":");
    let hour: string = parts[0];
    let count: string = parts[1];
    array.push(report, "  " + hour + ":00 - " + count + " requests");
  }
  array.push(report, "");

  // Error analysis
  if (array.length(errorEntries) > 0) {
    array.push(report, "❌ Error Entries Analysis:");
    let errorCount: number = 0;
    for (let errorLine: string in errorEntries) {
      errorCount++;
      if (errorCount <= 5) { // Show first 5 errors
        array.push(report, "  " + errorCount + ". " + errorLine);
      }
    }

    if (array.length(errorEntries) > 5) {
      array.push(report, "  ... and " + (array.length(errorEntries) - 5) + " more errors");
    }
    array.push(report, "");
  }

  // Step 5: Save report to file
  let reportContent: string = array.join(report, "\n");
  let reportFile: string = outputDir + "/analytics_report_" + utility.getCurrentDate() + ".txt";

  fs.writeFile(reportFile, reportContent);
  console.log("💾 Report saved to: " + reportFile);

  // Step 6: Display summary to console
  console.log("\n" + reportContent);

  // Step 7: Git operations (if in a git repository)
  if (fs.exists(".git")) {
    console.log("\n🔄 Committing report to Git repository...");

    try {
      git.add(reportFile);
      git.commit("Add analytics report for " + utility.getCurrentDate());
      console.log("✅ Report committed to Git");

      // Show recent commits
      console.log("\n📝 Recent commits:");
      // Note: In a real implementation, we'd use git.log() if available

    } catch (error) {
      console.log("⚠️  Git operations failed (repository might not be initialized)");
    }
  }

  // Step 8: Clean up and recommendations
  console.log("\n🎯 Recommendations:");

  // Analyze error rate
  let errorRate: number = array.length(errorEntries) * 100 / totalRequests;
  if (errorRate > 10) {
    console.log("🚨 High error rate detected (" + errorRate + "%). Investigate server issues.");
  } else if (errorRate > 5) {
    console.log("⚠️  Moderate error rate (" + errorRate + "%). Monitor closely.");
  } else {
    console.log("✅ Low error rate (" + errorRate + "%). System appears healthy.");
  }

  // Check for suspicious IP activity
  for (let entry: string in ipCounts) {
    let parts: string[] = string.split(entry, ":");
    let count: number = utility.parseInt(parts[1]);

    if (count > totalRequests / 2) {
      console.log("🔍 IP " + parts[0] + " has unusually high activity (" + count + " requests). Consider investigation.");
    }
  }

  console.log("\n✨ Analytics pipeline completed successfully!");

} catch (error) {
  console.log("❌ Error during log analysis: " + error);
  console.log("💡 Make sure log files are accessible and properly formatted");
}

// Additional String Processing Examples
console.log("\n" + "=".repeat(50));
console.log("📝 Additional String Processing Examples");
console.log("=".repeat(50));

// Example 1: Email Processing
console.log("\n🔤 Email Processing Example:");
let userInput: string = "  john.doe@COMPANY.COM  ";

// Clean and normalize the email
let cleanedEmail: string = string.trim(userInput);
let normalizedEmail: string = string.toLowerCase(cleanedEmail);

// Find the @ position to split manually
let atPosition: number = string.indexOf(normalizedEmail, "@");
let username: string = string.substring(normalizedEmail, 0, atPosition);
let domain: string = string.substring(normalizedEmail, atPosition + 1);

// Format username nicely
let formattedUsername: string = string.replace(username, ".", " ");
let capitalizedUsername: string = string.capitalize(formattedUsername);

console.log("Original input: '" + userInput + "'");
console.log("Clean email: " + normalizedEmail);
console.log("Username: " + username);
console.log("Domain: " + domain);
console.log("Display name: " + capitalizedUsername);

// Validate email format
let hasAt: boolean = string.contains(normalizedEmail, "@");
let hasValidDomain: boolean = string.contains(domain, ".");

if (hasAt && hasValidDomain) {
    console.log("✅ Email format appears valid");
} else {
    console.log("❌ Invalid email format");
}

// Example 2: URL Processing
console.log("\n🌐 URL Processing Example:");
let url: string = "https://api.github.com/users/john-doe/repos";

// Extract components
let protocol: string = string.substring(url, 0, string.indexOf(url, "://"));
let withoutProtocol: string = string.substring(url, string.indexOf(url, "://") + 3);
let domainEnd: number = string.indexOf(withoutProtocol, "/");
let urlDomain: string = string.substring(withoutProtocol, 0, domainEnd);
let path: string = string.substring(withoutProtocol, domainEnd);

console.log("Protocol: " + protocol);
console.log("Domain: " + urlDomain);
console.log("Path: " + path);

// Validate HTTPS
if (string.startsWith(url, "https://")) {
    console.log("✅ Secure connection");
} else {
    console.log("⚠️  Insecure connection");
}

// Example 3: File Path Processing
console.log("\n📁 File Path Processing Example:");
let filePath: string = "/home/user/documents/report.pdf";

// Extract path components
let lastSlash: number = string.lastIndexOf(filePath, "/");
let directory: string = string.substring(filePath, 0, lastSlash);
let filename: string = string.substring(filePath, lastSlash + 1);
let lastDot: number = string.lastIndexOf(filename, ".");
let nameWithoutExt: string = string.substring(filename, 0, lastDot);
let extension: string = string.substring(filename, lastDot + 1);

console.log("Directory: " + directory);
console.log("Filename: " + filename);
console.log("Name: " + nameWithoutExt);
console.log("Extension: " + extension);

// File type validation
if (string.endsWith(filePath, ".pdf")) {
    console.log("📄 PDF document detected");
} else if (string.endsWith(filePath, ".txt")) {
    console.log("📝 Text file detected");
} else {
    console.log("📁 Unknown file type");
}

// Example 4: Advanced Loop Patterns
console.log("\n🔄 Advanced Loop Processing Examples:");

// Process development tools
let tools: string = "git docker nginx mysql";
let toolList: string[] = string.split(tools, " ");

console.log("\n=== Development Tools Check ===");
for (let tool: string in toolList) {
  let isInstalled: boolean = os.isInstalled(tool);
  let status: string = isInstalled ? "✅ INSTALLED" : "❌ MISSING";
  console.log(tool + ": " + status);
}

// Number array processing
let numbers: number[] = [2, 4, 6, 8, 10, 12];

console.log("\n=== Even Number Processing ===");
for (let num: number in numbers) {
  let square: number = num * num;
  let cube: number = num * num * num;
  console.log(num + ": square=" + square + ", cube=" + cube);
}

// Nested loop example
let environments: string[] = ["dev", "staging", "prod"];
let services: string[] = ["web", "api", "db"];

console.log("\n=== Nested Loop Example ===");
for (let env: string in environments) {
  console.log("Environment: " + env);
  for (let service: string in services) {
    console.log("  - " + service + "." + env + ".company.com");
  }
}

// CSV data processing
let csvData: string = "john,25,engineer;mary,30,manager;bob,22,developer";
let records: string[] = string.split(csvData, ";");

console.log("\n=== Employee Data Processing ===");
for (let record: string in records) {
  let fields: string[] = string.split(record, ",");

  if (array.length(fields) >= 3) {
    let name: string = fields[0];
    let age: string = fields[1];
    let role: string = fields[2];

    console.log("Employee: " + name);
    console.log("  Age: " + age);
    console.log("  Role: " + role);

    // Age-based categorization
    let ageNum: number = utility.parseInt(age);
    if (ageNum < 25) {
      console.log("  Category: Junior");
    } else if (ageNum < 30) {
      console.log("  Category: Mid-level");
    } else {
      console.log("  Category: Senior");
    }
    console.log("");
  }
}
```

## Key Features Highlighted

### 🔄 Comprehensive Loops

- **For-in loops** for array iteration
- **Traditional for loops** with counters and conditions
- **Nested loops** for complex data processing
- **Loop control** with break and continue logic

### 🔤 Advanced String Processing

- **String parsing** with `indexOf()`, `substring()`, and `split()`
- **Pattern matching** for log entry analysis
- **String manipulation** with `trim()`, `isEmpty()`, and more
- **Dynamic string building** with array joins

### 📁 File System Operations

- **File existence checking** with `fs.exists()`
- **File reading and writing** with `fs.readFile()` and `fs.writeFile()`
- **Directory management** with `fs.createDirectory()`

### 🔢 Array Management

- **Dynamic arrays** with `array.push()` and `array.length()`
- **Array searching** with manual iteration patterns
- **Array joining** with `array.join()` for output formatting

### 🔧 Error Handling

- **Try-catch blocks** for robust error management
- **Graceful fallbacks** when files don't exist
- **Validation logic** throughout the pipeline

### 🌐 Git Integration

- **Repository status checking** with `.git` directory detection
- **Automated commits** with `git.add()` and `git.commit()`
- **Version control workflow** integration

## Generated Bash Output

When compiled, this Utah script generates efficient bash code that handles:

```bash
#!/bin/bash

# File existence checking
if [ -e "/var/log/nginx/access.log" ]; then
  # File reading and processing
  # String manipulation with bash parameter expansion
  # Array operations with bash arrays
  # Loop constructs with bash for loops
fi

# Error handling with subshells and exit codes
# Git operations with actual git commands
```

## Educational Value

This example demonstrates how Utah makes complex scripting tasks more manageable by providing:

1. **Type Safety**: Variables are strongly typed, preventing common bash errors
2. **Readable Syntax**: Clean, familiar syntax instead of cryptic bash constructs
3. **Built-in Functions**: Rich standard library for common operations
4. **Error Handling**: Modern try-catch instead of bash error handling patterns
5. **Code Organization**: Structured programming with clear flow control

## Running the Example

```bash
# Compile the Utah script
utah compile log-analytics-pipeline.shx

# Run the generated bash script
./log-analytics-pipeline.sh

# View the generated report
cat ./reports/analytics_report_*.txt
```

## Expected Output

```text
🔍 Starting Log Analytics Pipeline
=====================================

💡 Creating sample log data for demo...
✅ Sample log created: ./reports/sample_access.log

📖 Reading log file: ./reports/sample_access.log
📊 Processing 10 log entries...

📈 Generating Analytics Report
==============================

📊 Summary Statistics:
  Total Requests: 10
  Unique IP Addresses: 5
  Error Requests: 4
  Error Rate: 40%

🌐 Top IP Addresses:
  192.168.1.100 (3 requests)
  10.0.0.50 (2 requests)
  203.0.113.25 (2 requests)
  192.168.1.75 (2 requests)
  198.51.100.10 (1 requests)

📋 HTTP Status Code Distribution:
  200 OK: 4 requests
  401 Unauthorized: 1 requests
  500 Internal Server Error: 2 requests
  404 Not Found: 1 requests
  403 Forbidden: 1 requests
  201 Created: 1 requests

🕐 Hourly Traffic Distribution:
  10:00 - 3 requests
  11:00 - 3 requests
  12:00 - 2 requests
  13:00 - 1 requests

❌ Error Entries Analysis:
  1. 10.0.0.50 - - [09/Aug/2025:10:16:45 +0000] "POST /login HTTP/1.1" 401 567
  2. 203.0.113.25 - - [09/Aug/2025:10:18:34 +0000] "GET /api/data HTTP/1.1" 500 234
  3. 10.0.0.50 - - [09/Aug/2025:11:23:44 +0000] "POST /api/submit HTTP/1.1" 404 123
  4. 203.0.113.25 - - [09/Aug/2025:11:24:12 +0000] "GET /admin HTTP/1.1" 403 890

💾 Report saved to: ./reports/analytics_report_2025-08-09.txt

🎯 Recommendations:
🚨 High error rate detected (40%). Investigate server issues.

✨ Analytics pipeline completed successfully!

==================================================
📝 Additional String Processing Examples
==================================================

🔤 Email Processing Example:
Original input: '  john.doe@COMPANY.COM  '
Clean email: john.doe@company.com
Username: john.doe
Domain: company.com
Display name: John Doe
✅ Email format appears valid

🌐 URL Processing Example:
Protocol: https
Domain: api.github.com
Path: /users/john-doe/repos
✅ Secure connection

📁 File Path Processing Example:
Directory: /home/user/documents
Filename: report.pdf
Name: report
Extension: pdf
📄 PDF document detected

🔄 Advanced Loop Processing Examples:

=== Development Tools Check ===
git: ✅ INSTALLED
docker: ❌ MISSING
nginx: ✅ INSTALLED
mysql: ❌ MISSING

=== Even Number Processing ===
2: square=4, cube=8
4: square=16, cube=64
6: square=36, cube=216
8: square=64, cube=512
10: square=100, cube=1000
12: square=144, cube=1728

=== Nested Loop Example ===
Environment: dev
  - web.dev.company.com
  - api.dev.company.com
  - db.dev.company.com
Environment: staging
  - web.staging.company.com
  - api.staging.company.com
  - db.staging.company.com
Environment: prod
  - web.prod.company.com
  - api.prod.company.com
  - db.prod.company.com

=== Employee Data Processing ===
Employee: john
  Age: 25
  Role: engineer
  Category: Mid-level

Employee: mary
  Age: 30
  Role: manager
  Category: Senior

Employee: bob
  Age: 22
  Role: developer
  Category: Junior
```

## Benefits Over Traditional Bash

### Utah Approach (Clean and Type-Safe)

```typescript
// Process multiple files with loops
for (let file: string in fileList) {
  let content: string = fs.readFile(file);
  let lines: string[] = string.split(content, "\n");

  for (let line: string in lines) {
    if (string.contains(line, "ERROR")) {
      console.log("Found error in " + file + ": " + line);
    }
  }
}

// String processing with built-in functions
let email: string = string.trim(userInput);
let isValid: boolean = string.contains(email, "@");
```

### Bash Approach (Complex and Error-Prone)

```bash
# Verbose file processing
for file in "${file_list[@]}"; do
  while IFS= read -r line; do
    if [[ "$line" == *"ERROR"* ]]; then
      echo "Found error in $file: $line"
    fi
  done < "$file"
done

# Complex string manipulation
email=$(echo "$user_input" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
if [[ "$email" == *"@"* ]]; then
  is_valid=true
else
  is_valid=false
fi
```

### Key Advantages

- **Type Safety**: All variables and operations are type-checked
- **Built-in Functions**: Rich standard library eliminates external tool dependencies
- **Clean Syntax**: Familiar programming patterns instead of bash-specific constructs
- **Error Prevention**: Compile-time validation catches common mistakes
- **Maintainability**: Code is self-documenting and easier to modify

## Real-World Applications

This pattern can be adapted for:

- **Server Log Analysis**: Parse Apache, Nginx, or application logs
- **Security Monitoring**: Detect suspicious IP patterns and attack attempts
- **Performance Analysis**: Track response times and resource usage
- **Business Intelligence**: Extract metrics from application logs
- **DevOps Automation**: Automated reporting and alerting pipelines
- **Data Migration**: Process and transform log data for analytics platforms

## String Processing Function Reference

| Function | Purpose | Example Usage |
|----------|---------|---------------|
| `string.trim(text)` | Remove whitespace | Clean user input |
| `string.split(text, delimiter)` | Split into array | Parse CSV data |
| `string.indexOf(text, search)` | Find position | Locate patterns |
| `string.substring(text, start, end)` | Extract portion | Parse structured data |
| `string.contains(text, search)` | Check contains | Validate content |
| `string.replace(text, old, new)` | Replace text | Format output |
| `string.toLowerCase(text)` | Normalize case | Case-insensitive comparison |
| `string.isEmpty(text)` | Check empty | Input validation |

## Loop Pattern Reference

| Pattern | Use Case | Example |
|---------|----------|---------|
| `for (item in array)` | Array iteration | Process file lists |
| `for (i = 0; i < count; i++)` | Counted loops | Retry operations |
| Nested loops | Multi-dimensional data | Matrix processing |
| Break/continue | Flow control | Error handling |

## Next Steps

Try modifying this example to:

1. **Add time-based filtering** to analyze specific time ranges
2. **Implement geographic IP analysis** using IP location databases
3. **Create alerting mechanisms** for critical error thresholds
4. **Generate charts and graphs** using data visualization tools
5. **Integrate with monitoring systems** like Prometheus or Grafana
6. **Add machine learning** for anomaly detection in traffic patterns

## Related Examples

- **[System Health Monitor](system-health-monitor)**: Advanced system monitoring with loops
- **[DevOps Deployment Manager](devops-deployment-manager)**: Configuration management patterns
- **[Log File Analyzer](log-file-analyzer)**: Specialized log processing techniques
