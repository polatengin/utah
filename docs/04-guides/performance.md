---
layout: default
title: Performance Optimization
parent: Guides
nav_order: 6
---

Making Utah scripts faster and more efficient. Learn optimization techniques, profiling methods, and best practices for high-performance automation scripts.

## Prerequisites

- Understanding of system performance concepts
- Knowledge of Utah language features
- Experience with shell scripting optimization
- Familiarity with profiling tools

## Profiling and Measurement

### Script Performance Profiling

```typescript
script.description("Profile Utah script performance");

// Enable debug mode for detailed timing information
script.enableDebug(true);

// Function to measure execution time
function measureExecutionTime<T>(func: () => T, description: string): T {
  let startTime: number = parseInt(`$(date +%s%3N)`); // milliseconds
  
  console.log(`⏱️  Starting: ${description}`);
  let result: T = func();
  
  let endTime: number = parseInt(`$(date +%s%3N)`);
  let duration: number = endTime - startTime;
  
  console.log(`✅ Completed: ${description} (${duration}ms)`);
  
  return result;
}

// Function to profile memory usage
function profileMemoryUsage(description: string): object {
  let beforeMem: string = `$(free -m | grep Mem | awk '{print $3}')`;
  let beforeProcMem: string = `$(ps -o pid,vsz,rss -p $$ | tail -1 | awk '{print $2 "," $3}')`;
  
  console.log(`📊 Memory before ${description}: System=${beforeMem}MB, Process=${beforeProcMem}`);
  
  return {
    "description": description,
    "system_memory_mb": parseInt(beforeMem),
    "process_virtual_kb": parseInt(beforeProcMem.split(",")[0]),
    "process_resident_kb": parseInt(beforeProcMem.split(",")[1]),
    "timestamp": `$(date -Iseconds)`
  };
}

// Benchmark different approaches to the same task
function benchmarkApproaches(): void {
  console.log("Benchmarking different approaches...");
  
  let testData: string[] = [];
  for (let i: number = 0; i < 1000; i++) {
    testData.push(`test-item-${i}`);
  }
  
  // Approach 1: Traditional loop
  let result1: string[] = measureExecutionTime(() => {
    let filtered: string[] = [];
    for (let item: string in testData) {
      if (item.contains("5")) {
        filtered.push(item);
      }
    }
    return filtered;
  }, "Traditional loop filtering");
  
  // Approach 2: Using built-in string functions
  let result2: string[] = measureExecutionTime(() => {
    return testData.filter(item => item.contains("5"));
  }, "Built-in filter method");
  
  // Approach 3: Shell command
  let result3: string[] = measureExecutionTime(() => {
    let tempFile: string = "/tmp/test_data.txt";
    fs.writeFile(tempFile, testData.join("\n"));
    let filtered: string = `$(grep '5' ${tempFile})`;
    `$(rm ${tempFile})`;
    return filtered.split("\n");
  }, "Shell grep command");
  
  console.log(`Results: Approach1=${result1.length}, Approach2=${result2.length}, Approach3=${result3.length}`);
}

// Example profiling session
profileMemoryUsage("startup");
benchmarkApproaches();
profileMemoryUsage("after benchmarks");
```

### Resource Usage Monitoring

```typescript
script.description("Monitor script resource usage");

// Class to track resource usage over time
class ResourceMonitor {
  private startTime: number = parseInt(`$(date +%s%3N)`);
  private samples: object[] = [];
  private isMonitoring: boolean = false;
  private monitoringPid: string = "";
  
  start(): void {
    if (this.isMonitoring) {
      console.log("⚠️  Resource monitoring already started");
      return;
    }
    
    console.log("🔍 Starting resource monitoring...");
    this.isMonitoring = true;
    
    // Start monitoring in background
    let monitorScript: string = `
      while true; do
        TIMESTAMP=$(date +%s%3N)
        CPU_PERCENT=$(ps -p $$ -o %cpu --no-headers 2>/dev/null | tr -d ' ' || echo "0")
        MEM_VSZ=$(ps -p $$ -o vsz --no-headers 2>/dev/null | tr -d ' ' || echo "0")
        MEM_RSS=$(ps -p $$ -o rss --no-headers 2>/dev/null | tr -d ' ' || echo "0")
        
        echo "$TIMESTAMP,$CPU_PERCENT,$MEM_VSZ,$MEM_RSS" >> /tmp/resource_monitor_$$.log
        sleep 1
      done
    `;
    
    this.monitoringPid = `$(${monitorScript} & echo $!)`;
    console.log(`✅ Resource monitoring started (PID: ${this.monitoringPid})`);
  }
  
  stop(): object[] {
    if (!this.isMonitoring) {
      console.log("⚠️  Resource monitoring not started");
      return [];
    }
    
    console.log("🔍 Stopping resource monitoring...");
    
    // Stop monitoring process
    if (this.monitoringPid != "") {
      `$(kill ${this.monitoringPid} 2>/dev/null || true)`;
    }
    
    // Read monitoring data
    let logFile: string = `/tmp/resource_monitor_$$.log`;
    if (fs.exists(logFile)) {
      let logContent: string = fs.readFile(logFile);
      let lines: string[] = logContent.split("\n");
      
      for (let line: string in lines) {
        if (line.trim() != "") {
          let parts: string[] = line.split(",");
          if (parts.length >= 4) {
            this.samples.push({
              "timestamp": parseInt(parts[0]),
              "cpu_percent": parseFloat(parts[1]),
              "memory_virtual_kb": parseInt(parts[2]),
              "memory_resident_kb": parseInt(parts[3])
            });
          }
        }
      }
      
      `$(rm -f ${logFile})`;
    }
    
    this.isMonitoring = false;
    console.log(`✅ Resource monitoring stopped (${this.samples.length} samples)`);
    
    return this.samples;
  }
  
  getSummary(): object {
    if (this.samples.length == 0) {
      return { "error": "No monitoring data available" };
    }
    
    let totalDuration: number = parseInt(`$(date +%s%3N)`) - this.startTime;
    let cpuValues: number[] = this.samples.map(s => json.getNumber(s, ".cpu_percent"));
    let memVszValues: number[] = this.samples.map(s => json.getNumber(s, ".memory_virtual_kb"));
    let memRssValues: number[] = this.samples.map(s => json.getNumber(s, ".memory_resident_kb"));
    
    return {
      "duration_ms": totalDuration,
      "samples_count": this.samples.length,
      "cpu": {
        "max_percent": Math.max(...cpuValues),
        "avg_percent": cpuValues.reduce((a, b) => a + b, 0) / cpuValues.length,
        "min_percent": Math.min(...cpuValues)
      },
      "memory": {
        "max_virtual_kb": Math.max(...memVszValues),
        "max_resident_kb": Math.max(...memRssValues),
        "avg_virtual_kb": Math.round(memVszValues.reduce((a, b) => a + b, 0) / memVszValues.length),
        "avg_resident_kb": Math.round(memRssValues.reduce((a, b) => a + b, 0) / memRssValues.length)
      }
    };
  }
}

// Example usage
let monitor: ResourceMonitor = new ResourceMonitor();
monitor.start();

// Simulate workload
for (let i: number = 0; i < 100; i++) {
  let data: string = `$(seq 1 1000 | head -100)`;
  `$(sleep 0.1)`;
}

let samples: object[] = monitor.stop();
let summary: object = monitor.getSummary();

console.log("Resource Usage Summary:");
console.log(json.stringify(summary, true));
```

## Code Optimization Techniques

### Efficient Data Structures and Algorithms

```typescript
script.description("Optimize data processing with efficient algorithms");

// Optimized array operations
function optimizedArrayOperations(): void {
  console.log("Testing array operation optimizations...");
  
  let largeArray: string[] = [];
  for (let i: number = 0; i < 10000; i++) {
    largeArray.push(`item-${i}`);
  }
  
  // Inefficient: Multiple array iterations
  let inefficientResult: string[] = measureExecutionTime(() => {
    let filtered: string[] = [];
    let mapped: string[] = [];
    let result: string[] = [];
    
    // Three separate iterations
    for (let item: string in largeArray) {
      if (item.contains("5")) {
        filtered.push(item);
      }
    }
    
    for (let item: string in filtered) {
      mapped.push(item.toUpperCase());
    }
    
    for (let item: string in mapped) {
      if (item.length > 8) {
        result.push(item);
      }
    }
    
    return result;
  }, "Inefficient multiple iterations");
  
  // Efficient: Single iteration with combined logic
  let efficientResult: string[] = measureExecutionTime(() => {
    let result: string[] = [];
    
    for (let item: string in largeArray) {
      if (item.contains("5")) {
        let mapped: string = item.toUpperCase();
        if (mapped.length > 8) {
          result.push(mapped);
        }
      }
    }
    
    return result;
  }, "Efficient single iteration");
  
  console.log(`Results match: ${inefficientResult.length == efficientResult.length}`);
}

// Optimized string operations
function optimizedStringOperations(): void {
  console.log("Testing string operation optimizations...");
  
  let testStrings: string[] = [];
  for (let i: number = 0; i < 1000; i++) {
    testStrings.push(`This is test string number ${i} with some content`);
  }
  
  // Inefficient: String concatenation in loop
  let inefficientConcat: string = measureExecutionTime(() => {
    let result: string = "";
    for (let str: string in testStrings) {
      result += str + "\n";
    }
    return result;
  }, "Inefficient string concatenation");
  
  // Efficient: Array join
  let efficientConcat: string = measureExecutionTime(() => {
    return testStrings.join("\n");
  }, "Efficient array join");
  
  console.log(`Result lengths match: ${inefficientConcat.length == efficientConcat.length}`);
}

// Optimized file I/O
function optimizedFileIO(): void {
  console.log("Testing file I/O optimizations...");
  
  let testData: string[] = [];
  for (let i: number = 0; i < 1000; i++) {
    testData.push(`Line ${i}: ${`$(date +%s%3N)`}\n`);
  }
  
  // Inefficient: Multiple file writes
  measureExecutionTime(() => {
    let outputFile: string = "/tmp/inefficient_output.txt";
    `$(rm -f ${outputFile})`;
    
    for (let line: string in testData) {
      fs.appendFile(outputFile, line);
    }
  }, "Inefficient multiple file writes");
  
  // Efficient: Single write operation
  measureExecutionTime(() => {
    let outputFile: string = "/tmp/efficient_output.txt";
    fs.writeFile(outputFile, testData.join(""));
  }, "Efficient single file write");
  
  // Clean up
  `$(rm -f /tmp/inefficient_output.txt /tmp/efficient_output.txt)`;
}

optimizedArrayOperations();
optimizedStringOperations();
optimizedFileIO();
```

### Memory Management

```typescript
script.description("Optimize memory usage in Utah scripts");

// Memory-efficient data processing
function processLargeDataset(filePath: string): void {
  console.log(`Processing large dataset: ${filePath}`);
  
  if (!fs.exists(filePath)) {
    console.log(`Creating test dataset: ${filePath}`);
    
    // Create a large test file
    let lines: string[] = [];
    for (let i: number = 0; i < 100000; i++) {
      lines.push(`${i},data-${i},${Math.random()},${`$(date +%s)`}`);
    }
    fs.writeFile(filePath, lines.join("\n"));
  }
  
  // Memory-inefficient approach: Load entire file
  measureExecutionTime(() => {
    profileMemoryUsage("before loading entire file");
    let content: string = fs.readFile(filePath);
    let lines: string[] = content.split("\n");
    
    profileMemoryUsage("after loading entire file");
    
    let processedCount: number = 0;
    for (let line: string in lines) {
      if (line.contains("data")) {
        processedCount++;
      }
    }
    
    console.log(`Inefficient approach processed: ${processedCount} lines`);
    profileMemoryUsage("after processing");
  }, "Memory-inefficient file processing");
  
  // Memory-efficient approach: Stream processing using shell commands
  measureExecutionTime(() => {
    profileMemoryUsage("before streaming");
    
    let processedCount: number = parseInt(`$(grep -c 'data' ${filePath})`);
    
    console.log(`Efficient approach processed: ${processedCount} lines`);
    profileMemoryUsage("after streaming");
  }, "Memory-efficient stream processing");
}

// Chunk-based processing for large datasets
function processInChunks(filePath: string, chunkSize: number = 1000): void {
  console.log(`Processing file in chunks of ${chunkSize} lines`);
  
  let totalLines: number = parseInt(`$(wc -l < ${filePath})`);
  let chunksCount: number = Math.ceil(totalLines / chunkSize);
  
  console.log(`Total lines: ${totalLines}, Chunks: ${chunksCount}`);
  
  let processedTotal: number = 0;
  
  for (let chunk: number = 0; chunk < chunksCount; chunk++) {
    let startLine: number = chunk * chunkSize + 1;
    let endLine: number = Math.min((chunk + 1) * chunkSize, totalLines);
    
    console.log(`Processing chunk ${chunk + 1}/${chunksCount} (lines ${startLine}-${endLine})`);
    
    // Extract chunk using sed
    let chunkData: string = `$(sed -n '${startLine},${endLine}p' ${filePath})`;
    let chunkLines: string[] = chunkData.split("\n");
    
    // Process chunk
    let chunkProcessed: number = 0;
    for (let line: string in chunkLines) {
      if (line.trim() != "" && line.contains("data")) {
        chunkProcessed++;
      }
    }
    
    processedTotal += chunkProcessed;
    console.log(`Chunk ${chunk + 1} processed: ${chunkProcessed} lines`);
    
    // Force garbage collection simulation
    chunkData = "";
    chunkLines = [];
  }
  
  console.log(`Total processed: ${processedTotal} lines`);
}

// Example usage
let testFile: string = "/tmp/large_dataset.csv";
processLargeDataset(testFile);
processInChunks(testFile, 5000);

// Clean up
`$(rm -f ${testFile})`;
```

### Caching and Memoization

```typescript
script.description("Implement caching for performance optimization");

// Simple cache implementation
class SimpleCache {
  private cache: object = {};
  private hitCount: number = 0;
  private missCount: number = 0;
  
  get(key: string): string | null {
    if (json.has(this.cache, `.${key}`)) {
      this.hitCount++;
      return json.getString(this.cache, `.${key}`);
    } else {
      this.missCount++;
      return null;
    }
  }
  
  set(key: string, value: string): void {
    this.cache = json.set(this.cache, `.${key}`, value);
  }
  
  getStats(): object {
    let total: number = this.hitCount + this.missCount;
    return {
      "hits": this.hitCount,
      "misses": this.missCount,
      "total_requests": total,
      "hit_rate": total > 0 ? Math.round((this.hitCount / total) * 100) : 0
    };
  }
  
  clear(): void {
    this.cache = {};
    this.hitCount = 0;
    this.missCount = 0;
  }
}

// File-based persistent cache
class FileCache {
  private cacheDir: string;
  private maxAge: number; // seconds
  
  constructor(cacheDir: string, maxAgeSeconds: number = 3600) {
    this.cacheDir = cacheDir;
    this.maxAge = maxAgeSeconds;
    fs.createDirectory(this.cacheDir);
  }
  
  private getCacheFilePath(key: string): string {
    // Create safe filename from key
    let safeKey: string = key.replace(/[^a-zA-Z0-9]/g, "_");
    return `${this.cacheDir}/${safeKey}.cache`;
  }
  
  get(key: string): string | null {
    let cacheFile: string = this.getCacheFilePath(key);
    
    if (fs.exists(cacheFile)) {
      // Check if cache is still valid
      let fileAge: number = parseInt(`$(stat -c %Y ${cacheFile})`);
      let currentTime: number = parseInt(`$(date +%s)`);
      
      if (currentTime - fileAge < this.maxAge) {
        return fs.readFile(cacheFile);
      } else {
        // Cache expired, remove file
        `$(rm -f ${cacheFile})`;
      }
    }
    
    return null;
  }
  
  set(key: string, value: string): void {
    let cacheFile: string = this.getCacheFilePath(key);
    fs.writeFile(cacheFile, value);
  }
  
  clear(): void {
    `$(rm -f ${this.cacheDir}/*.cache)`;
  }
}

// Cached expensive operation example
let cache: SimpleCache = new SimpleCache();

function expensiveOperation(input: string): string {
  let cached: string | null = cache.get(input);
  if (cached !== null) {
    return cached;
  }
  
  // Simulate expensive computation
  console.log(`Computing expensive operation for: ${input}`);
  `$(sleep 1)`; // Simulate delay
  
  let result: string = `processed_${input}_${`$(date +%s)`}`;
  cache.set(input, result);
  
  return result;
}

// Test caching performance
console.log("Testing cache performance...");

let testInputs: string[] = ["input1", "input2", "input3", "input1", "input2", "input4", "input1"];

measureExecutionTime(() => {
  for (let input: string in testInputs) {
    let result: string = expensiveOperation(input);
    console.log(`Result for ${input}: ${result.substring(0, 20)}...`);
  }
}, "Cached operations");

let stats: object = cache.getStats();
console.log("Cache statistics:");
console.log(json.stringify(stats, true));

// Test file cache
console.log("\nTesting file cache...");
let fileCache: FileCache = new FileCache("/tmp/utah_cache", 60);

function cachedHttpRequest(url: string): string {
  let cached: string | null = fileCache.get(url);
  if (cached !== null) {
    console.log(`Cache hit for: ${url}`);
    return cached;
  }
  
  console.log(`Making HTTP request to: ${url}`);
  let response: string = `$(curl -s "${url}" || echo "Error fetching ${url}")`;
  
  fileCache.set(url, response);
  return response;
}

let testUrls: string[] = [
  "https://httpbin.org/json",
  "https://httpbin.org/uuid", 
  "https://httpbin.org/json", // Repeat to test cache
  "https://httpbin.org/ip"
];

for (let url: string in testUrls) {
  let response: string = cachedHttpRequest(url);
  console.log(`Response length for ${url}: ${response.length} characters`);
}

// Clean up
fileCache.clear();
```

## Shell Command Optimization

### Efficient Command Usage

```typescript
script.description("Optimize shell command usage for better performance");

// Optimize find operations
function optimizedFindOperations(): void {
  console.log("Testing find operation optimizations...");
  
  // Create test directory structure
  let testDir: string = "/tmp/find_test";
  `$(rm -rf ${testDir})`;
  fs.createDirectory(testDir);
  
  // Create test files
  for (let i: number = 0; i < 100; i++) {
    let subDir: string = `${testDir}/dir${i % 10}`;
    fs.createDirectory(subDir);
    
    for (let j: number = 0; j < 10; j++) {
      let fileName: string = i % 2 == 0 ? `file${j}.txt` : `document${j}.log`;
      fs.writeFile(`${subDir}/${fileName}`, `Test content ${i}-${j}`);
    }
  }
  
  // Inefficient: Multiple find commands
  measureExecutionTime(() => {
    let txtFiles: string[] = `$(find ${testDir} -name "*.txt")`.split("\n");
    let logFiles: string[] = `$(find ${testDir} -name "*.log")`.split("\n");
    
    console.log(`Found ${txtFiles.length} .txt files and ${logFiles.length} .log files`);
  }, "Multiple find commands");
  
  // Efficient: Single find with regex
  measureExecutionTime(() => {
    let allFiles: string[] = `$(find ${testDir} -type f \\( -name "*.txt" -o -name "*.log" \\))`.split("\n");
    
    let txtCount: number = 0;
    let logCount: number = 0;
    
    for (let file: string in allFiles) {
      if (file.endsWith(".txt")) {
        txtCount++;
      } else if (file.endsWith(".log")) {
        logCount++;
      }
    }
    
    console.log(`Found ${txtCount} .txt files and ${logCount} .log files`);
  }, "Single find with multiple patterns");
  
  // Clean up
  `$(rm -rf ${testDir})`;
}

// Optimize grep operations
function optimizedGrepOperations(): void {
  console.log("Testing grep operation optimizations...");
  
  // Create test file
  let testFile: string = "/tmp/grep_test.txt";
  let lines: string[] = [];
  
  for (let i: number = 0; i < 10000; i++) {
    let lineType: string = i % 3 == 0 ? "ERROR" : (i % 3 == 1 ? "WARNING" : "INFO");
    lines.push(`[${`$(date +%H:%M:%S)`}] ${lineType}: Log message ${i}`);
  }
  
  fs.writeFile(testFile, lines.join("\n"));
  
  // Inefficient: Multiple grep commands
  measureExecutionTime(() => {
    let errors: number = parseInt(`$(grep -c "ERROR" ${testFile})`);
    let warnings: number = parseInt(`$(grep -c "WARNING" ${testFile})`);
    let total: number = errors + warnings;
    
    console.log(`Found ${errors} errors, ${warnings} warnings (total: ${total})`);
  }, "Multiple grep commands");
  
  // Efficient: Single grep with extended regex
  measureExecutionTime(() => {
    let matches: string = `$(grep -E "(ERROR|WARNING)" ${testFile} | wc -l)`;
    let total: number = parseInt(matches);
    
    // Get counts separately if needed
    let errors: number = parseInt(`$(grep -o "ERROR" ${testFile} | wc -l)`);
    let warnings: number = total - errors;
    
    console.log(`Found ${errors} errors, ${warnings} warnings (total: ${total})`);
  }, "Single grep with regex");
  
  // Clean up
  `$(rm -f ${testFile})`;
}

// Optimize sort and text processing
function optimizedTextProcessing(): void {
  console.log("Testing text processing optimizations...");
  
  // Create test data
  let testFile: string = "/tmp/sort_test.txt";
  let data: string[] = [];
  
  for (let i: number = 0; i < 5000; i++) {
    data.push(`${Math.floor(Math.random() * 1000)},user${i},action${i % 10}`);
  }
  
  fs.writeFile(testFile, data.join("\n"));
  
  // Inefficient: Multiple processing steps
  measureExecutionTime(() => {
    `$(sort ${testFile} > /tmp/sorted.txt)`;
    `$(uniq /tmp/sorted.txt > /tmp/unique.txt)`;
    let uniqueCount: number = parseInt(`$(wc -l < /tmp/unique.txt)`);
    
    console.log(`Unique lines (inefficient): ${uniqueCount}`);
    `$(rm -f /tmp/sorted.txt /tmp/unique.txt)`;
  }, "Multiple text processing steps");
  
  // Efficient: Pipeline processing
  measureExecutionTime(() => {
    let uniqueCount: number = parseInt(`$(sort ${testFile} | uniq | wc -l)`);
    
    console.log(`Unique lines (efficient): ${uniqueCount}`);
  }, "Pipeline text processing");
  
  // Clean up
  `$(rm -f ${testFile})`;
}

optimizedFindOperations();
optimizedGrepOperations();
optimizedTextProcessing();
```

### Parallel Command Execution

```typescript
script.description("Optimize performance with parallel command execution");

// Parallel file processing
function parallelFileProcessing(): void {
  console.log("Testing parallel file processing...");
  
  // Create test files
  let testDir: string = "/tmp/parallel_test";
  `$(rm -rf ${testDir})`;
  fs.createDirectory(testDir);
  
  let files: string[] = [];
  for (let i: number = 0; i < 20; i++) {
    let fileName: string = `${testDir}/file${i}.txt`;
    let content: string[] = [];
    
    for (let j: number = 0; j < 1000; j++) {
      content.push(`Line ${j} in file ${i} with content ${Math.random()}`);
    }
    
    fs.writeFile(fileName, content.join("\n"));
    files.push(fileName);
  }
  
  // Sequential processing
  measureExecutionTime(() => {
    let totalLines: number = 0;
    
    for (let file: string in files) {
      let lineCount: number = parseInt(`$(wc -l < ${file})`);
      totalLines += lineCount;
    }
    
    console.log(`Sequential processing: ${totalLines} total lines`);
  }, "Sequential file processing");
  
  // Parallel processing using xargs
  measureExecutionTime(() => {
    let totalLines: number = parseInt(`$(printf '%s\\n' ${files.join(' ')} | xargs -P 4 wc -l | tail -1 | awk '{print $1}')`);
    
    console.log(`Parallel processing: ${totalLines} total lines`);
  }, "Parallel file processing with xargs");
  
  // Clean up
  `$(rm -rf ${testDir})`;
}

// Parallel network operations
function parallelNetworkOperations(): void {
  console.log("Testing parallel network operations...");
  
  let urls: string[] = [
    "https://httpbin.org/delay/1",
    "https://httpbin.org/delay/1", 
    "https://httpbin.org/delay/1",
    "https://httpbin.org/delay/1"
  ];
  
  // Sequential requests
  measureExecutionTime(() => {
    let responses: number = 0;
    
    for (let url: string in urls) {
      let response: string = `$(curl -s -w "%{http_code}" -o /dev/null "${url}" || echo "000")`;
      if (response == "200") {
        responses++;
      }
    }
    
    console.log(`Sequential requests: ${responses} successful`);
  }, "Sequential HTTP requests");
  
  // Parallel requests using background processes
  measureExecutionTime(() => {
    let pids: string[] = [];
    let tempDir: string = "/tmp/parallel_requests";
    fs.createDirectory(tempDir);
    
    // Start all requests in background
    for (let i: number = 0; i < urls.length; i++) {
      let url: string = urls[i];
      let outputFile: string = `${tempDir}/response_${i}.txt`;
      let command: string = `curl -s -w "%{http_code}" -o /dev/null "${url}" > ${outputFile}`;
      
      let pid: string = `$(${command} & echo $!)`;
      pids.push(pid);
    }
    
    // Wait for all to complete
    for (let pid: string in pids) {
      `$(wait ${pid})`;
    }
    
    // Count successful responses
    let responses: number = parseInt(`$(grep -c "200" ${tempDir}/response_*.txt 2>/dev/null || echo "0")`);
    
    console.log(`Parallel requests: ${responses} successful`);
    
    // Clean up
    `$(rm -rf ${tempDir})`;
  }, "Parallel HTTP requests");
}

parallelFileProcessing();
parallelNetworkOperations();
```

## Configuration and Environment Optimization

### Environment Variable Management

```typescript
script.description("Optimize environment variable usage");

// Efficient environment variable access
function optimizeEnvAccess(): void {
  console.log("Testing environment variable optimization...");
  
  // Set up test environment variables
  env.set("TEST_VAR_1", "value1");
  env.set("TEST_VAR_2", "value2");
  env.set("TEST_VAR_3", "value3");
  
  // Inefficient: Multiple env.get() calls
  measureExecutionTime(() => {
    for (let i: number = 0; i < 1000; i++) {
      let var1: string = env.get("TEST_VAR_1") || "";
      let var2: string = env.get("TEST_VAR_2") || "";
      let var3: string = env.get("TEST_VAR_3") || "";
      
      // Use variables
      let combined: string = `${var1}-${var2}-${var3}`;
    }
  }, "Multiple env.get() calls");
  
  // Efficient: Cache environment variables
  measureExecutionTime(() => {
    let cachedVar1: string = env.get("TEST_VAR_1") || "";
    let cachedVar2: string = env.get("TEST_VAR_2") || "";
    let cachedVar3: string = env.get("TEST_VAR_3") || "";
    
    for (let i: number = 0; i < 1000; i++) {
      let combined: string = `${cachedVar1}-${cachedVar2}-${cachedVar3}`;
    }
  }, "Cached environment variables");
}

// Configuration loading optimization
function optimizeConfigLoading(): void {
  console.log("Testing configuration loading optimization...");
  
  // Create test configuration
  let configFile: string = "/tmp/test_config.json";
  let config: object = {
    "database": {
      "host": "localhost",
      "port": 5432,
      "name": "testdb"
    },
    "api": {
      "endpoint": "https://api.example.com",
      "timeout": 30,
      "retries": 3
    },
    "features": {
      "caching": true,
      "logging": true,
      "monitoring": false
    }
  };
  
  fs.writeFile(configFile, json.stringify(config, true));
  
  // Inefficient: Load config multiple times
  measureExecutionTime(() => {
    for (let i: number = 0; i < 100; i++) {
      let configContent: string = fs.readFile(configFile);
      let configData: object = json.parse(configContent);
      
      let dbHost: string = json.getString(configData, ".database.host");
      let apiEndpoint: string = json.getString(configData, ".api.endpoint");
    }
  }, "Multiple config file loads");
  
  // Efficient: Load config once
  measureExecutionTime(() => {
    let configContent: string = fs.readFile(configFile);
    let configData: object = json.parse(configContent);
    
    for (let i: number = 0; i < 100; i++) {
      let dbHost: string = json.getString(configData, ".database.host");
      let apiEndpoint: string = json.getString(configData, ".api.endpoint");
    }
  }, "Single config file load");
  
  // Clean up
  `$(rm -f ${configFile})`;
}

optimizeEnvAccess();
optimizeConfigLoading();
```

## Best Practices Summary

### Performance Checklist

```typescript
script.description("Performance optimization checklist and guidelines");

// Performance audit function
function auditScriptPerformance(): object {
  console.log("Auditing script performance...");
  
  let audit: object = {
    "timestamp": `$(date -Iseconds)`,
    "checks": {},
    "recommendations": []
  };
  
  // Check 1: File I/O patterns
  let hasMultipleFileReads: boolean = false; // This would be detected by static analysis
  audit = json.set(audit, ".checks.file_io", hasMultipleFileReads ? "needs_optimization" : "good");
  
  if (hasMultipleFileReads) {
    let recommendations: string[] = json.get(audit, ".recommendations");
    recommendations.push("Consider batching file operations or using streaming");
    audit = json.set(audit, ".recommendations", recommendations);
  }
  
  // Check 2: Memory usage patterns
  let currentMemory: number = parseInt(`$(ps -o rss -p $$ --no-headers | tr -d ' ')`);
  let memoryStatus: string = currentMemory > 100000 ? "high" : "normal"; // KB
  audit = json.set(audit, ".checks.memory_usage", memoryStatus);
  
  if (memoryStatus == "high") {
    let recommendations: string[] = json.get(audit, ".recommendations");
    recommendations.push("Consider processing data in chunks or using streaming");
    audit = json.set(audit, ".recommendations", recommendations);
  }
  
  // Check 3: Command execution patterns
  audit = json.set(audit, ".checks.command_optimization", "review_needed");
  
  let recommendations: string[] = json.get(audit, ".recommendations");
  recommendations.push("Review shell commands for efficiency (use pipelines, avoid multiple greps)");
  audit = json.set(audit, ".recommendations", recommendations);
  
  return audit;
}

// Performance guidelines
function printPerformanceGuidelines(): void {
  console.log(`
🚀 Utah Performance Optimization Guidelines:

1. **File Operations**
   - Batch file reads/writes when possible
   - Use streaming for large files
   - Avoid repeated file system calls

2. **String Operations**
   - Use array.join() instead of string concatenation in loops
   - Leverage built-in string functions
   - Consider regex for complex pattern matching

3. **Array Processing**
   - Combine multiple iterations into single loops
   - Use built-in array methods when available
   - Process data in chunks for large datasets

4. **Shell Commands**
   - Use pipelines instead of multiple commands
   - Leverage parallel execution with xargs -P
   - Avoid repeated expensive commands

5. **Memory Management**
   - Process large datasets in chunks
   - Clear large variables when done
   - Use streaming for file processing

6. **Caching**
   - Cache expensive operations
   - Store computed results
   - Use file-based caching for persistence

7. **Environment Variables**
   - Cache frequently accessed env vars
   - Load configuration once at startup
   - Minimize environment variable lookups

8. **Debugging and Profiling**
   - Use script.enableDebug() for timing info
   - Profile resource usage during development
   - Benchmark different approaches
`);
}

let auditResults: object = auditScriptPerformance();
console.log("Performance audit results:");
console.log(json.stringify(auditResults, true));

printPerformanceGuidelines();
```

## Next Steps

- **[Security Best Practices](security.md)** - Secure your optimized scripts
- **[Parallel Execution](parallel.md)** - Scale with parallel processing
- **[System Administration](system-admin.md)** - Apply optimizations to system scripts

Performance optimization in Utah requires understanding both the language features and the underlying system. Use profiling, measurement, and iterative improvement to build efficient automation solutions.
