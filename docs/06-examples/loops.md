---
layout: default
title: Loops and Iteration
parent: Examples
nav_order: 5
description: "Examples of for loops, for-in loops, and array iteration patterns"
permalink: /examples/loops/
---

Examples of for loops, for-in loops, and array iteration patterns in Utah. This tutorial demonstrates Utah's clean and powerful iteration syntax.

## Features Demonstrated

- **Traditional for loops** with increment/decrement
- **Custom step for loops** with flexible increments
- **For-in loops** for array iteration
- **String splitting** and array processing
- **Nested iteration** patterns
- **Break and continue** control flow

## Complete Script

```typescript
// Utah For Loops - Complete Demo with Arrays

console.log("=== Traditional For Loops ===");

// Basic increment
for (let i: number = 1; i <= 3; i++) {
  console.log(`Count: ${i}`);
}

// Custom increment
for (let j: number = 0; j < 15; j += 5) {
  console.log(`Step by 5: ${j}`);
}

// Decrement
for (let k: number = 10; k >= 0; k -= 2) {
  console.log(`Countdown: ${k}`);
}

console.log("=== For-In Loops with Arrays ===");

// Split comma-separated values
let groceries: string = "milk,bread,eggs,cheese,butter";
let groceryList: string[] = string.split(groceries, ",");

for (let item: string in groceryList) {
  console.log(`Need to buy: ${item}`);
}

// Split space-separated values
let tools: string = "git docker nginx mysql";
let toolList: string[] = string.split(tools, " ");

console.log("=== Development Tools Check ===");
for (let tool: string in toolList) {
  let isInstalled: boolean = os.isInstalled(tool);
  let status: string = isInstalled ? "✅ INSTALLED" : "❌ MISSING";
  console.log(`${tool}: ${status}`);
}

// Number array processing
let numbers: number[] = [2, 4, 6, 8, 10, 12];

console.log("=== Even Number Processing ===");
for (let num: number in numbers) {
  let square: number = num * num;
  let cube: number = num * num * num;
  console.log(`${num}: square=${square}, cube=${cube}`);
}

console.log("=== Nested Loop Example ===");
let environments: string[] = ["dev", "staging", "prod"];
let services: string[] = ["web", "api", "db"];

for (let env: string in environments) {
  console.log(`Environment: ${env}`);
  for (let service: string in services) {
    console.log(`  - ${service}.${env}.company.com`);
  }
}
```

## Loop Types Explained

### Traditional For Loops

Utah supports C-style for loops with initialization, condition, and increment:

```typescript
// Basic increment loop
for (let i: number = 1; i <= 5; i++) {
  console.log(`Iteration: ${i}`);
}

// Custom increment
for (let i: number = 0; i < 20; i += 3) {
  console.log(`Step by 3: ${i}`);
}

// Decrement loop
for (let i: number = 10; i >= 0; i -= 2) {
  console.log(`Countdown: ${i}`);
}
```

### For-In Loops

Elegant iteration over arrays without index management:

```typescript
let fruits: string[] = ["apple", "banana", "orange"];

for (let fruit: string in fruits) {
  console.log(`Processing: ${fruit}`);
}
```

This is much cleaner than traditional bash loops:

```bash
# Bash equivalent (verbose)
fruits=("apple" "banana" "orange")
for fruit in "${fruits[@]}"; do
  echo "Processing: $fruit"
done
```

## Advanced Loop Patterns

### System Administration Loop

```typescript
// Check multiple log files
let logPaths: string[] = [
  "/var/log/nginx/access.log",
  "/var/log/nginx/error.log",
  "/var/log/mysql/error.log",
  "/var/log/syslog"
];

console.log("=== Log File Analysis ===");
for (let logPath: string in logPaths) {
  let fileName: string = fs.fileName(logPath);
  let directory: string = fs.dirname(logPath);

  if (fs.exists(logPath)) {
    let size: number = fs.size(logPath);
    console.log(`✅ ${fileName}: ${size} bytes in ${directory}`);
  } else {
    console.log(`❌ ${fileName}: Not found in ${directory}`);
  }
}
```

### Configuration Processing

```typescript
// Process environment-specific configurations
let environments: string[] = ["development", "staging", "production"];
let configTemplates: string[] = ["database.yml", "redis.conf", "nginx.conf"];

console.log("=== Configuration Deployment ===");
for (let env: string in environments) {
  console.log(`Deploying to ${env}:`);

  for (let template: string in configTemplates) {
    let configFile: string = `${template}.${env}`;
    console.log(`  📄 Generating ${configFile}`);

    // Simulate config generation
    let content: string = `# ${template} for ${env}\nenvironment=${env}\n`;
    console.log(`  ✅ ${configFile} ready`);
  }

  console.log(`  🚀 ${env} deployment complete\n`);
}
```

### Data Processing Pipeline

```typescript
// Process CSV data with nested loops
let csvData: string = "john,25,engineer;mary,30,manager;bob,22,developer";
let records: string[] = string.split(csvData, ";");

console.log("=== Employee Data Processing ===");
for (let record: string in records) {
  let fields: string[] = string.split(record, ",");

  if (array.length(fields) >= 3) {
    let name: string = fields[0];
    let age: string = fields[1];
    let role: string = fields[2];

    console.log(`Employee: ${name}`);
    console.log(`  Age: ${age}`);
    console.log(`  Role: ${role}`);

    // Age-based categorization
    let ageNum: number = age; // Convert string to number
    if (ageNum < 25) {
      console.log(`  Category: Junior`);
    } else if (ageNum < 30) {
      console.log(`  Category: Mid-level`);
    } else {
      console.log(`  Category: Senior`);
    }
    console.log("");
  }
}
```

### Network Service Monitoring

```typescript
// Monitor multiple servers and ports
let servers: string[] = ["web01", "web02", "db01", "cache01"];
let services: string[] = ["ssh", "http", "https"];
let ports: number[] = [22, 80, 443];

console.log("=== Service Monitoring ===");
for (let server: string in servers) {
  console.log(`Checking ${server}:`);

  for (let i: number = 0; i < array.length(services); i++) {
    let service: string = services[i];
    let port: number = ports[i];

    // Simulate service check
    let isRunning: boolean = utility.random(0, 1) == 1;
    let status: string = isRunning ? "🟢 UP" : "🔴 DOWN";

    console.log(`  ${service} (${port}): ${status}`);
  }
  console.log("");
}
```

## Loop Control Flow

### Break and Continue Patterns

```typescript
// Find first missing dependency
let dependencies: string[] = ["git", "docker", "node", "python3", "go"];

console.log("=== Dependency Check ===");
for (let dep: string in dependencies) {
  if (!os.isInstalled(dep)) {
    console.log(`❌ Missing critical dependency: ${dep}`);
    console.log("⚠️  Please install before continuing");
    break; // Stop checking after first missing dependency
  }
  console.log(`✅ ${dep} is installed`);
}

// Skip processing invalid entries
let userInputs: string[] = ["valid@email.com", "", "invalid-email", "another@valid.com"];

console.log("=== Email Validation ===");
for (let email: string in userInputs) {
  if (string.length(email) == 0) {
    console.log("⏭️  Skipping empty entry");
    continue; // Skip empty strings
  }

  if (!string.includes(email, "@")) {
    console.log(`❌ Invalid email format: ${email}`);
    continue; // Skip invalid emails
  }

  console.log(`✅ Valid email: ${email}`);
}
```

## Complex Iteration Examples

### File Processing Pipeline

```typescript
// Process multiple file types with different handlers
let fileExtensions: string[] = [".txt", ".log", ".conf", ".json"];
let processingMethods: string[] = ["text_analysis", "log_parsing", "config_validation", "json_processing"];

let targetFiles: string[] = [
  "readme.txt",
  "error.log",
  "nginx.conf",
  "package.json",
  "unknown.xyz"
];

console.log("=== File Processing Pipeline ===");
for (let file: string in targetFiles) {
  console.log(`Processing: ${file}`);

  let processed: boolean = false;

  for (let i: number = 0; i < array.length(fileExtensions); i++) {
    let ext: string = fileExtensions[i];

    if (string.endsWith(file, ext)) {
      let method: string = processingMethods[i];
      console.log(`  📄 Using ${method} for ${ext} file`);
      processed = true;
      break;
    }
  }

  if (!processed) {
    console.log(`  ⚠️  No handler for file type`);
  }

  console.log("");
}
```

### System Resource Monitoring

```typescript
// Monitor system resources over time intervals
let checkIntervals: number[] = [1, 5, 10, 30]; // seconds
let resourceTypes: string[] = ["CPU", "Memory", "Disk", "Network"];

console.log("=== Resource Monitoring Simulation ===");
for (let interval: number in checkIntervals) {
  console.log(`Monitoring every ${interval} seconds:`);

  for (let resource: string in resourceTypes) {
    // Simulate resource usage
    let usage: number = utility.random(10, 95);
    let status: string = "🟢 Normal";

    if (usage > 80) {
      status = "🔴 Critical";
    } else if (usage > 60) {
      status = "🟡 Warning";
    }

    console.log(`  ${resource}: ${usage}% ${status}`);
  }

  console.log(`  ⏰ Next check in ${interval}s\n`);
}
```

## Usage Examples

### Basic Loop Demo

```bash
utah compile loops.shx
./loops.sh
```

### Expected Output

```text
=== Traditional For Loops ===
Count: 1
Count: 2
Count: 3
Step by 5: 0
Step by 5: 5
Step by 5: 10
Countdown: 10
Countdown: 8
Countdown: 6
Countdown: 4
Countdown: 2
Countdown: 0

=== For-In Loops with Arrays ===
Need to buy: milk
Need to buy: bread
Need to buy: eggs
Need to buy: cheese
Need to buy: butter

=== Development Tools Check ===
git: ✅ INSTALLED
docker: ❌ MISSING
nginx: ✅ INSTALLED
mysql: ❌ MISSING
```

## Benefits Over Traditional Bash

### Utah Loops (Clean and Type-Safe)

```typescript
let servers: string[] = ["web01", "web02", "db01"];

for (let server: string in servers) {
  console.log(`Checking ${server}`);
}

for (let i: number = 1; i <= 5; i++) {
  console.log(`Iteration ${i}`);
}
```

### Bash Loops (Verbose and Error-Prone)

```bash
servers=("web01" "web02" "db01")

for server in "${servers[@]}"; do
  echo "Checking $server"
done

for ((i=1; i<=5; i++)); do
  echo "Iteration $i"
done
```

### Key Advantages

- **Type Safety**: Loop variables are strongly typed
- **Clean Syntax**: No complex array notation like `"${array[@]}"`
- **Better Readability**: Clear iteration intent
- **Error Prevention**: Type checking prevents common mistakes
- **Consistent Behavior**: Same syntax across different data types

## Performance Considerations

### Efficient Array Processing

```typescript
// Efficient: Process array once
let items: string[] = string.split("a,b,c,d,e", ",");
let count: number = array.length(items);

console.log(`Processing ${count} items:`);
for (let item: string in items) {
  console.log(`  - ${item}`);
}

// Less efficient: Multiple array.length() calls
for (let i: number = 0; i < array.length(items); i++) {
  // array.length() called every iteration
  console.log(items[i]);
}
```

## Related Examples

- [Log File Analyzer](log-file-analyzer) - Working with array data structures
- [String Processing](string-processing) - Processing string arrays
- [System Health Monitor](system-health-monitor) - Loops in real-world monitoring
