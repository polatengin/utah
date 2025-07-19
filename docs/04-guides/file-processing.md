---
layout: default
title: File Processing
parent: Guides
nav_order: 2
---

Batch file operations and data transformation with Utah. Learn how to efficiently process files, directories, and data using Utah's built-in file system functions.

## Prerequisites

- Basic Utah syntax knowledge
- Understanding of file system operations
- Familiarity with data formats (JSON, YAML, CSV)

## Core File Operations

### File System Navigation

```typescript
// Check if files exist
if (fs.exists("/path/to/file.txt")) {
  console.log("File exists");
}

// Get file information
let fileName: string = fs.filename("/path/to/document.pdf");  // "document.pdf"
let extension: string = fs.extension("/path/to/document.pdf"); // ".pdf"
let dirName: string = fs.dirname("/path/to/document.pdf");    // "/path/to"
let parentDir: string = fs.parentDirname("/path/to/document.pdf"); // "/path"
```

### Reading and Writing Files

```typescript
// Read entire file
let content: string = fs.readFile("data.txt");

// Write to file
fs.writeFile("output.txt", "Hello, Utah!");

// Append to file
fs.appendFile("log.txt", "New log entry\n");

// Copy files
fs.copy("source.txt", "destination.txt");

// Move/rename files
fs.move("old-name.txt", "new-name.txt");
```

## Batch File Processing

### Processing Multiple Files

```typescript
script.description("Process all text files in a directory");

// Get all .txt files in current directory
let files: string[] = `$(find . -name "*.txt" -type f)`.split("\n");

for (let file: string in files) {
  if (file.trim() != "") {
    console.log(`Processing: ${file}`);
    
    // Read file content
    let content: string = fs.readFile(file);
    
    // Transform content (example: uppercase)
    let transformed: string = content.toUpperCase();
    
    // Write to new file
    let outputFile: string = file.replace(".txt", "-processed.txt");
    fs.writeFile(outputFile, transformed);
    
    console.log(`✅ Created: ${outputFile}`);
  }
}
```

### Directory Operations

```typescript
// Create directories
fs.createDirectory("output/processed");
fs.createDirectory("logs/$(date +%Y-%m-%d)");

// List directory contents
let entries: string[] = `$(ls -1 /path/to/directory)`.split("\n");

// Process directory tree
function processDirectory(dir: string): void {
  console.log(`Processing directory: ${dir}`);
  
  let items: string[] = `$(find ${dir} -type f -name "*.log")`.split("\n");
  
  for (let item: string in items) {
    if (item.trim() != "") {
      processLogFile(item);
    }
  }
}

function processLogFile(logFile: string): void {
  let content: string = fs.readFile(logFile);
  let lines: string[] = content.split("\n");
  let errorCount: number = 0;
  
  for (let line: string in lines) {
    if (line.contains("ERROR")) {
      errorCount++;
    }
  }
  
  console.log(`${logFile}: ${errorCount} errors found`);
}
```

## Data Format Processing

### JSON Processing

```typescript
script.description("Process and transform JSON data files");

// Read and parse JSON
let jsonFile: string = "data.json";
if (fs.exists(jsonFile)) {
  let content: string = fs.readFile(jsonFile);
  let data: object = json.parse(content);
  
  // Transform data
  let users: object = json.get(data, ".users");
  let activeUsers: object = json.filter(users, ".active", true);
  
  // Add metadata
  let result: object = json.set(data, ".metadata.processed_at", `$(date -Iseconds)`);
  result = json.set(result, ".metadata.active_count", json.length(activeUsers));
  
  // Save processed data
  fs.writeFile("processed-data.json", json.stringify(result, true));
  console.log("✅ JSON processing completed");
}
```

### YAML Processing

```typescript
// Process YAML configuration files
function processYamlConfig(configFile: string): void {
  if (fs.exists(configFile)) {
    let content: string = fs.readFile(configFile);
    let config: object = yaml.parse(content);
    
    // Update configuration
    config = yaml.set(config, ".updated_at", `$(date -Iseconds)`);
    config = yaml.set(config, ".version", "2.0");
    
    // Validate required fields
    let requiredFields: string[] = [".database.host", ".database.port", ".api.key"];
    for (let field: string in requiredFields) {
      if (!yaml.has(config, field)) {
        console.log(`❌ Missing required field: ${field}`);
        exit(1);
      }
    }
    
    // Save updated config
    let outputFile: string = configFile.replace(".yaml", "-updated.yaml");
    fs.writeFile(outputFile, yaml.stringify(config));
    console.log(`✅ Updated config saved to ${outputFile}`);
  }
}
```

### CSV Processing

```typescript
script.description("Process CSV data files");

function processCsvFile(csvFile: string): void {
  if (fs.exists(csvFile)) {
    let content: string = fs.readFile(csvFile);
    let lines: string[] = content.split("\n");
    
    // Skip header row
    let header: string = lines[0];
    console.log(`Processing CSV with headers: ${header}`);
    
    let processedRows: string[] = [header]; // Keep header
    
    for (let i: number = 1; i < lines.length; i++) {
      let line: string = lines[i];
      if (line.trim() != "") {
        // Split CSV row (simple comma split)
        let columns: string[] = line.split(",");
        
        // Process columns (example: trim whitespace)
        let processedColumns: string[] = [];
        for (let column: string in columns) {
          processedColumns.push(column.trim());
        }
        
        // Rejoin row
        processedRows.push(processedColumns.join(","));
      }
    }
    
    // Save processed CSV
    let outputFile: string = csvFile.replace(".csv", "-processed.csv");
    fs.writeFile(outputFile, processedRows.join("\n"));
    console.log(`✅ Processed CSV saved to ${outputFile}`);
  }
}
```

## Advanced File Processing

### File Filtering and Sorting

```typescript
script.description("Filter and sort files by various criteria");

// Filter files by size
function filterFilesBySize(directory: string, minSizeMB: number): string[] {
  let minSizeBytes: number = minSizeMB * 1024 * 1024;
  let largeFiles: string[] = [];
  
  let allFiles: string[] = `$(find ${directory} -type f)`.split("\n");
  
  for (let file: string in allFiles) {
    if (file.trim() != "") {
      let sizeBytes: string = `$(stat -f%z "${file}" 2>/dev/null || stat -c%s "${file}" 2>/dev/null)`;
      if (sizeBytes != "" && parseInt(sizeBytes) > minSizeBytes) {
        largeFiles.push(file);
      }
    }
  }
  
  return largeFiles;
}

// Filter files by date
function filterFilesByDate(directory: string, daysSince: number): string[] {
  let recentFiles: string[] = [];
  
  // Find files modified in the last N days
  let findCmd: string = `find ${directory} -type f -mtime -${daysSince}`;
  let files: string[] = `$(${findCmd})`.split("\n");
  
  for (let file: string in files) {
    if (file.trim() != "") {
      recentFiles.push(file);
    }
  }
  
  return recentFiles;
}

// Example usage
let largeFiles: string[] = filterFilesBySize("/var/log", 10); // Files > 10MB
let recentFiles: string[] = filterFilesByDate("/tmp", 7);     // Files from last 7 days

console.log(`Found ${largeFiles.length} large files`);
console.log(`Found ${recentFiles.length} recent files`);
```

### Parallel File Processing

```typescript
script.description("Process multiple files in parallel");

// Process files concurrently
function processFilesParallel(files: string[]): void {
  let maxParallel: number = 4; // Process 4 files at once
  let currentBatch: string[] = [];
  
  for (let file: string in files) {
    currentBatch.push(file);
    
    if (currentBatch.length >= maxParallel) {
      processBatch(currentBatch);
      currentBatch = [];
    }
  }
  
  // Process remaining files
  if (currentBatch.length > 0) {
    processBatch(currentBatch);
  }
}

function processBatch(batch: string[]): void {
  console.log(`Processing batch of ${batch.length} files...`);
  
  // Start all processes in background
  let pids: string[] = [];
  for (let file: string in batch) {
    let pid: string = `$(processFileBackground "${file}" & echo $!)`;
    pids.push(pid);
  }
  
  // Wait for all to complete
  for (let pid: string in pids) {
    `$(wait ${pid})`;
  }
  
  console.log("✅ Batch processing completed");
}

function processFileBackground(file: string): void {
  // Your file processing logic here
  console.log(`Processing ${file} in background...`);
  
  // Simulate processing time
  `$(sleep 2)`;
  
  console.log(`✅ Completed ${file}`);
}
```

## Data Transformation Patterns

### Log File Analysis

```typescript
script.description("Analyze web server access logs");

args.define("--log-file", "-f", "Path to log file", "string", true);
args.define("--output-dir", "-o", "Output directory", "string", false, "reports");

let logFile: string = args.getString("--log-file");
let outputDir: string = args.getString("--output-dir");

// Create output directory
fs.createDirectory(outputDir);

if (fs.exists(logFile)) {
  let content: string = fs.readFile(logFile);
  let lines: string[] = content.split("\n");
  
  // Initialize counters
  let stats: object = json.parse(`{
    "total_requests": 0,
    "status_codes": {},
    "ip_addresses": {},
    "user_agents": {},
    "top_pages": {}
  }`);
  
  for (let line: string in lines) {
    if (line.trim() != "") {
      // Parse Apache/Nginx common log format
      // Example: 127.0.0.1 - - [10/Oct/2000:13:55:36 -0700] "GET /index.html HTTP/1.0" 200 2326
      
      // Extract IP address (first field)
      let fields: string[] = line.split(" ");
      if (fields.length >= 6) {
        let ip: string = fields[0];
        let statusCode: string = fields[8];
        
        // Count total requests
        let currentTotal: number = json.getNumber(stats, ".total_requests");
        stats = json.set(stats, ".total_requests", currentTotal + 1);
        
        // Count status codes
        let statusPath: string = `.status_codes.${statusCode}`;
        let statusCount: number = json.getNumber(stats, statusPath) || 0;
        stats = json.set(stats, statusPath, statusCount + 1);
        
        // Count IP addresses
        let ipPath: string = `.ip_addresses.${ip}`;
        let ipCount: number = json.getNumber(stats, ipPath) || 0;
        stats = json.set(stats, ipPath, ipCount + 1);
      }
    }
  }
  
  // Save analysis report
  let reportFile: string = `${outputDir}/access-log-analysis.json`;
  fs.writeFile(reportFile, json.stringify(stats, true));
  
  // Generate summary
  let totalRequests: number = json.getNumber(stats, ".total_requests");
  let successfulRequests: number = json.getNumber(stats, ".status_codes.200") || 0;
  let errorRequests: number = json.getNumber(stats, ".status_codes.404") || 0;
  
  let summaryFile: string = `${outputDir}/summary.txt`;
  let summary: string = `Log Analysis Summary
===================
Total Requests: ${totalRequests}
Successful (200): ${successfulRequests}
Not Found (404): ${errorRequests}
Success Rate: ${Math.round((successfulRequests / totalRequests) * 100)}%

Generated: $(date)
`;
  
  fs.writeFile(summaryFile, summary);
  
  console.log(`✅ Analysis completed. Reports saved to ${outputDir}/`);
  console.log(`   - Detailed report: ${reportFile}`);
  console.log(`   - Summary: ${summaryFile}`);
}
```

### Data Migration

```typescript
script.description("Migrate data between formats");

args.define("--input", "-i", "Input file", "string", true);
args.define("--output", "-o", "Output file", "string", true);
args.define("--format", "-f", "Output format (json|yaml|csv)", "string", true);

let inputFile: string = args.getString("--input");
let outputFile: string = args.getString("--output");
let outputFormat: string = args.getString("--format");

if (!fs.exists(inputFile)) {
  console.log(`❌ Input file not found: ${inputFile}`);
  exit(1);
}

// Detect input format
let inputFormat: string = "";
let inputExt: string = fs.extension(inputFile).toLowerCase();

if (inputExt == ".json") {
  inputFormat = "json";
} else if (inputExt == ".yaml" || inputExt == ".yml") {
  inputFormat = "yaml";
} else if (inputExt == ".csv") {
  inputFormat = "csv";
} else {
  console.log(`❌ Unsupported input format: ${inputExt}`);
  exit(1);
}

console.log(`Converting ${inputFormat} to ${outputFormat}...`);

let content: string = fs.readFile(inputFile);
let data: object = {};

// Parse input data
if (inputFormat == "json") {
  data = json.parse(content);
} else if (inputFormat == "yaml") {
  data = yaml.parse(content);
} else if (inputFormat == "csv") {
  // Simple CSV to JSON conversion
  let lines: string[] = content.split("\n");
  let headers: string[] = lines[0].split(",");
  let records: object[] = [];
  
  for (let i: number = 1; i < lines.length; i++) {
    if (lines[i].trim() != "") {
      let values: string[] = lines[i].split(",");
      let record: object = {};
      
      for (let j: number = 0; j < headers.length; j++) {
        record = json.set(record, `.${headers[j].trim()}`, values[j]?.trim() || "");
      }
      records.push(record);
    }
  }
  
  data = json.set({}, ".records", records);
}

// Generate output
let outputContent: string = "";

if (outputFormat == "json") {
  outputContent = json.stringify(data, true);
} else if (outputFormat == "yaml") {
  outputContent = yaml.stringify(data);
} else if (outputFormat == "csv") {
  // Simple JSON to CSV conversion
  let records: object[] = json.get(data, ".records") || [data];
  if (records.length > 0) {
    // Get headers from first record
    let firstRecord: object = records[0];
    let headers: string[] = json.keys(firstRecord);
    
    outputContent = headers.join(",") + "\n";
    
    for (let record: object in records) {
      let values: string[] = [];
      for (let header: string in headers) {
        let value: string = json.getString(record, `.${header}`) || "";
        values.push(value);
      }
      outputContent += values.join(",") + "\n";
    }
  }
} else {
  console.log(`❌ Unsupported output format: ${outputFormat}`);
  exit(1);
}

// Save output
fs.writeFile(outputFile, outputContent);
console.log(`✅ Conversion completed: ${inputFile} → ${outputFile}`);
```

## Best Practices

### Error Handling

```typescript
// Always check if files exist before processing
function safeFileOperation(file: string): boolean {
  if (!fs.exists(file)) {
    console.log(`❌ File not found: ${file}`);
    return false;
  }
  
  // Check if file is readable
  if (`$(test -r "${file}"; echo $?)` != "0") {
    console.log(`❌ File not readable: ${file}`);
    return false;
  }
  
  return true;
}

// Use try-catch pattern for data parsing
function parseJsonSafely(content: string): object | null {
  try {
    return json.parse(content);
  } catch {
    console.log("❌ Invalid JSON format");
    return null;
  }
}
```

### Performance Optimization

```typescript
// Process large files in chunks
function processLargeFile(file: string, chunkSize: number = 1000): void {
  let lineNumber: number = 0;
  let chunk: string[] = [];
  
  // Read file line by line (using external tool for large files)
  let lines: string[] = `$(cat "${file}")`.split("\n");
  
  for (let line: string in lines) {
    chunk.push(line);
    lineNumber++;
    
    if (chunk.length >= chunkSize) {
      processChunk(chunk, lineNumber - chunkSize + 1);
      chunk = [];
    }
  }
  
  // Process remaining lines
  if (chunk.length > 0) {
    processChunk(chunk, lineNumber - chunk.length + 1);
  }
}

function processChunk(lines: string[], startLine: number): void {
  console.log(`Processing lines ${startLine} to ${startLine + lines.length - 1}`);
  
  for (let line: string in lines) {
    // Process individual line
    processLine(line);
  }
}
```

### Backup and Recovery

```typescript
// Always backup before modifying files
function safeFileModification(file: string, newContent: string): void {
  // Create backup
  let backupFile: string = `${file}.backup.$(date +%Y%m%d_%H%M%S)`;
  fs.copy(file, backupFile);
  console.log(`📁 Backup created: ${backupFile}`);
  
  try {
    // Modify file
    fs.writeFile(file, newContent);
    console.log(`✅ File updated: ${file}`);
  } catch {
    // Restore from backup on error
    console.log("❌ Error occurred, restoring from backup...");
    fs.copy(backupFile, file);
    console.log(`🔄 File restored from backup`);
  }
}
```

## Common Use Cases

### 1. Log Rotation and Cleanup

```typescript
script.description("Rotate and compress old log files");

let logDir: string = "/var/log/myapp";
let maxDays: number = 30;

// Find old log files
let oldLogs: string[] = `$(find ${logDir} -name "*.log" -mtime +${maxDays})`.split("\n");

for (let logFile: string in oldLogs) {
  if (logFile.trim() != "") {
    console.log(`Processing old log: ${logFile}`);
    
    // Compress the log file
    `$(gzip "${logFile}")`;
    
    // Move to archive directory
    let archiveDir: string = `${logDir}/archive`;
    fs.createDirectory(archiveDir);
    
    let compressedFile: string = `${logFile}.gz`;
    let archiveFile: string = `${archiveDir}/${fs.filename(compressedFile)}`;
    
    fs.move(compressedFile, archiveFile);
    console.log(`✅ Archived: ${archiveFile}`);
  }
}
```

### 2. Configuration Management

```typescript
script.description("Update configuration files across environments");

args.define("--env", "-e", "Environment (dev|staging|prod)", "string", true);
args.define("--config-dir", "-c", "Configuration directory", "string", false, "./config");

let environment: string = args.getString("--env");
let configDir: string = args.getString("--config-dir");

let configFiles: string[] = ["database.yaml", "api.yaml", "cache.yaml"];

for (let configFile: string in configFiles) {
  let sourcePath: string = `${configDir}/templates/${configFile}`;
  let targetPath: string = `${configDir}/${environment}/${configFile}`;
  
  if (fs.exists(sourcePath)) {
    let content: string = fs.readFile(sourcePath);
    let config: object = yaml.parse(content);
    
    // Apply environment-specific settings
    if (environment == "prod") {
      config = yaml.set(config, ".debug", false);
      config = yaml.set(config, ".log_level", "INFO");
    } else {
      config = yaml.set(config, ".debug", true);
      config = yaml.set(config, ".log_level", "DEBUG");
    }
    
    // Update timestamp
    config = yaml.set(config, ".updated_at", `$(date -Iseconds)`);
    
    // Ensure target directory exists
    fs.createDirectory(fs.dirname(targetPath));
    
    // Save environment-specific config
    fs.writeFile(targetPath, yaml.stringify(config));
    console.log(`✅ Updated ${targetPath}`);
  }
}
```

## Next Steps

- **[System Administration](system-admin.md)** - Server management scripts
- **[CI/CD Integration](cicd.md)** - Automation in build pipelines
- **[Performance Optimization](performance.md)** - Optimize file processing scripts

File processing is a fundamental skill for automation. Master these patterns to handle any file-based workflow efficiently.
