---
layout: default
title: Parallel Execution
parent: Guides
nav_order: 5
---

Multi-threading and concurrent processing with Utah. Learn how to execute tasks in parallel, manage background processes, and coordinate concurrent operations for improved performance.

## Prerequisites

- Understanding of concurrent programming concepts
- Knowledge of process management
- Familiarity with shell job control
- Basic understanding of synchronization

## Background Process Management

### Basic Background Execution

```typescript
script.description("Execute tasks in parallel using background processes");

// Function to run a task in background
function runInBackground(taskName: string, command: string): string {
  console.log(`Starting background task: ${taskName}`);
  
  // Start process in background and capture PID
  let pid: string = `$(${command} & echo $!)`;
  
  if (pid.trim() != "") {
    console.log(`✅ Task ${taskName} started with PID: ${pid}`);
    return pid;
  } else {
    console.log(`❌ Failed to start task: ${taskName}`);
    return "";
  }
}

// Function to wait for background process
function waitForProcess(taskName: string, pid: string): boolean {
  console.log(`Waiting for task ${taskName} (PID: ${pid}) to complete...`);
  
  // Wait for specific PID
  let exitCode: string = `$(wait ${pid} 2>/dev/null; echo $?)`;
  
  if (exitCode == "0") {
    console.log(`✅ Task ${taskName} completed successfully`);
    return true;
  } else {
    console.log(`❌ Task ${taskName} failed with exit code: ${exitCode}`);
    return false;
  }
}

// Example: Parallel file processing
let files: string[] = ["file1.txt", "file2.txt", "file3.txt", "file4.txt"];
let backgroundPids: object = {};

// Start all processes in background
for (let file: string in files) {
  let taskName: string = `process-${file}`;
  let command: string = `sleep 5 && echo "Processed ${file}" > ${file}.result`;
  
  let pid: string = runInBackground(taskName, command);
  if (pid != "") {
    backgroundPids = json.set(backgroundPids, `.${taskName}`, pid);
  }
}

// Wait for all processes to complete
let taskNames: string[] = json.keys(backgroundPids);
let successCount: number = 0;

for (let taskName: string in taskNames) {
  let pid: string = json.getString(backgroundPids, `.${taskName}`);
  if (waitForProcess(taskName, pid)) {
    successCount++;
  }
}

console.log(`Parallel processing completed: ${successCount}/${taskNames.length} tasks successful`);
```

### Advanced Process Pool

```typescript
script.description("Implement a process pool for controlled parallel execution");

args.define("--max-workers", "-w", "Maximum number of worker processes", "number", false, 4);
args.define("--task-file", "-f", "File containing tasks to execute", "string", true);

let maxWorkers: number = args.getNumber("--max-workers");
let taskFile: string = args.getString("--task-file");

if (!fs.exists(taskFile)) {
  console.log(`❌ Task file not found: ${taskFile}`);
  exit(1);
}

// Read tasks from file
let taskContent: string = fs.readFile(taskFile);
let tasks: string[] = taskContent.split("\n");
let validTasks: string[] = [];

for (let task: string in tasks) {
  if (task.trim() != "") {
    validTasks.push(task.trim());
  }
}

console.log(`Processing ${validTasks.length} tasks with ${maxWorkers} workers`);

// Process pool state
let runningWorkers: object = {}; // { pid: { task: string, startTime: number } }
let completedTasks: number = 0;
let failedTasks: number = 0;
let taskQueue: string[] = validTasks;

// Function to start a worker
function startWorker(task: string): string {
  let startTime: number = parseInt(`$(date +%s)`);
  
  // Create a unique task ID
  let taskId: string = `task-${completedTasks + failedTasks + 1}`;
  
  // Execute task in background
  let command: string = `(${task}) 2>&1 | tee logs/${taskId}.log`;
  let pid: string = `$(${command} & echo $!)`;
  
  if (pid.trim() != "") {
    runningWorkers = json.set(runningWorkers, `.${pid}`, {
      "task": task,
      "task_id": taskId,
      "start_time": startTime
    });
    
    console.log(`🚀 Started worker ${taskId} (PID: ${pid}): ${task.substring(0, 50)}...`);
    return pid;
  }
  
  return "";
}

// Function to check for completed workers
function checkCompletedWorkers(): void {
  let workerPids: string[] = json.keys(runningWorkers);
  
  for (let pid: string in workerPids) {
    // Check if process is still running
    let isRunning: string = `$(kill -0 ${pid} 2>/dev/null && echo "running" || echo "done")`;
    
    if (isRunning.trim() == "done") {
      // Get exit code
      let exitCode: string = `$(wait ${pid} 2>/dev/null; echo $?)`;
      let worker: object = json.get(runningWorkers, `.${pid}`);
      let task: string = json.getString(worker, ".task");
      let taskId: string = json.getString(worker, ".task_id");
      let startTime: number = json.getNumber(worker, ".start_time");
      let duration: number = parseInt(`$(date +%s)`) - startTime;
      
      if (exitCode == "0") {
        completedTasks++;
        console.log(`✅ Worker ${taskId} completed in ${duration}s`);
      } else {
        failedTasks++;
        console.log(`❌ Worker ${taskId} failed in ${duration}s (exit code: ${exitCode})`);
      }
      
      // Remove from running workers
      runningWorkers = json.delete(runningWorkers, `.${pid}`);
    }
  }
}

// Create logs directory
fs.createDirectory("logs");

// Main processing loop
console.log("Starting process pool...");

while (taskQueue.length > 0 || json.keys(runningWorkers).length > 0) {
  // Start new workers if slots available and tasks remaining
  let currentWorkers: number = json.keys(runningWorkers).length;
  
  while (currentWorkers < maxWorkers && taskQueue.length > 0) {
    let nextTask: string = taskQueue.shift();
    let pid: string = startWorker(nextTask);
    
    if (pid != "") {
      currentWorkers++;
    } else {
      console.log(`❌ Failed to start worker for task: ${nextTask}`);
      failedTasks++;
    }
  }
  
  // Check for completed workers
  checkCompletedWorkers();
  
  // Brief pause before next iteration
  `$(sleep 1)`;
  
  // Progress update
  let totalTasks: number = validTasks.length;
  let processedTasks: number = completedTasks + failedTasks;
  let progress: number = Math.round((processedTasks / totalTasks) * 100);
  
  console.log(`Progress: ${processedTasks}/${totalTasks} (${progress}%) - Running: ${json.keys(runningWorkers).length}, Completed: ${completedTasks}, Failed: ${failedTasks}`);
}

console.log(`🎉 Process pool completed: ${completedTasks} successful, ${failedTasks} failed`);
```

## Concurrent Data Processing

### Parallel File Processing

```typescript
script.description("Process multiple files concurrently");

args.define("--input-dir", "-i", "Input directory", "string", true);
args.define("--output-dir", "-o", "Output directory", "string", true);
args.define("--pattern", "-p", "File pattern to process", "string", false, "*.txt");
args.define("--workers", "-w", "Number of worker processes", "number", false, 4);

let inputDir: string = args.getString("--input-dir");
let outputDir: string = args.getString("--output-dir");
let pattern: string = args.getString("--pattern");
let workers: number = args.getNumber("--workers");

if (!fs.exists(inputDir)) {
  console.log(`❌ Input directory not found: ${inputDir}`);
  exit(1);
}

fs.createDirectory(outputDir);

// Find files to process
let files: string[] = `$(find ${inputDir} -name "${pattern}" -type f)`.split("\n");
let validFiles: string[] = [];

for (let file: string in files) {
  if (file.trim() != "") {
    validFiles.push(file.trim());
  }
}

if (validFiles.length == 0) {
  console.log(`❌ No files found matching pattern: ${pattern}`);
  exit(1);
}

console.log(`Found ${validFiles.length} files to process`);

// Create worker function for file processing
function createWorkerScript(): string {
  let workerScript: string = `#!/bin/bash
# Worker script for file processing

process_file() {
  local input_file="$1"
  local output_dir="$2"
  local worker_id="$3"
  
  echo "[Worker $worker_id] Processing: $input_file"
  
  # Get filename without path
  local filename=$(basename "$input_file")
  local output_file="$output_dir/processed_$filename"
  
  # Example processing: convert to uppercase and add metadata
  {
    echo "# Processed by Worker $worker_id at $(date)"
    echo "# Original file: $input_file"
    echo "# ================================"
    cat "$input_file" | tr '[:lower:]' '[:upper:]'
  } > "$output_file"
  
  echo "[Worker $worker_id] ✅ Completed: $output_file"
}

# Execute if called with arguments
if [ $# -eq 3 ]; then
  process_file "$1" "$2" "$3"
fi
`;
  
  fs.writeFile("worker.sh", workerScript);
  `$(chmod +x worker.sh)`;
  
  return "worker.sh";
}

// Distribute files among workers
function distributeFiles(files: string[], numWorkers: number): object[] {
  let workerQueues: object[] = [];
  
  // Initialize worker queues
  for (let i: number = 0; i < numWorkers; i++) {
    workerQueues.push({ "worker_id": i + 1, "files": [] });
  }
  
  // Distribute files round-robin
  for (let i: number = 0; i < files.length; i++) {
    let workerIndex: number = i % numWorkers;
    let workerQueue: object = workerQueues[workerIndex];
    let fileList: string[] = json.get(workerQueue, ".files");
    fileList.push(files[i]);
    workerQueues[workerIndex] = json.set(workerQueue, ".files", fileList);
  }
  
  return workerQueues;
}

// Create worker script
let workerScript: string = createWorkerScript();

// Distribute files among workers
let workerQueues: object[] = distributeFiles(validFiles, workers);

// Start all workers
let workerPids: string[] = [];

for (let queue: object in workerQueues) {
  let workerId: number = json.getNumber(queue, ".worker_id");
  let fileList: string[] = json.get(queue, ".files");
  
  if (fileList.length > 0) {
    console.log(`Starting worker ${workerId} with ${fileList.length} files`);
    
    // Create worker command that processes all assigned files
    let workerCommand: string = "";
    for (let file: string in fileList) {
      workerCommand += `./worker.sh "${file}" "${outputDir}" "${workerId}"; `;
    }
    
    // Start worker in background
    let pid: string = `$(${workerCommand} & echo $!)`;
    
    if (pid.trim() != "") {
      workerPids.push(pid);
      console.log(`✅ Worker ${workerId} started (PID: ${pid})`);
    } else {
      console.log(`❌ Failed to start worker ${workerId}`);
    }
  }
}

// Monitor progress
console.log(`Monitoring ${workerPids.length} workers...`);

let startTime: number = parseInt(`$(date +%s)`);

// Wait for all workers to complete
for (let pid: string in workerPids) {
  console.log(`Waiting for worker PID ${pid}...`);
  `$(wait ${pid})`;
}

let endTime: number = parseInt(`$(date +%s)`);
let duration: number = endTime - startTime;

// Clean up
`$(rm -f worker.sh)`;

// Verify results
let processedFiles: string[] = `$(find ${outputDir} -name "processed_*" -type f)`.split("\n");
let processedCount: number = 0;

for (let file: string in processedFiles) {
  if (file.trim() != "") {
    processedCount++;
  }
}

console.log(`🎉 Parallel processing completed in ${duration} seconds`);
console.log(`Files processed: ${processedCount}/${validFiles.length}`);

if (processedCount == validFiles.length) {
  console.log("✅ All files processed successfully");
} else {
  console.log(`⚠️  ${validFiles.length - processedCount} files failed to process`);
}
```

### Concurrent API Calls

```typescript
script.description("Make concurrent HTTP API calls");

args.define("--urls-file", "-f", "File containing URLs to fetch", "string", true);
args.define("--concurrency", "-c", "Number of concurrent requests", "number", false, 5);
args.define("--timeout", "-t", "Request timeout in seconds", "number", false, 30);

let urlsFile: string = args.getString("--urls-file");
let concurrency: number = args.getNumber("--concurrency");
let timeout: number = args.getNumber("--timeout");

if (!fs.exists(urlsFile)) {
  console.log(`❌ URLs file not found: ${urlsFile}`);
  exit(1);
}

// Read URLs
let urlsContent: string = fs.readFile(urlsFile);
let allUrls: string[] = urlsContent.split("\n");
let urls: string[] = [];

for (let url: string in allUrls) {
  if (url.trim() != "" && !url.trim().startsWith("#")) {
    urls.push(url.trim());
  }
}

if (urls.length == 0) {
  console.log("❌ No valid URLs found");
  exit(1);
}

console.log(`Making concurrent requests to ${urls.length} URLs with concurrency ${concurrency}`);

// Create results directory
let resultsDir: string = "api-results";
fs.createDirectory(resultsDir);

// Function to make a single HTTP request
function makeRequest(url: string, index: number): string {
  let outputFile: string = `${resultsDir}/response-${index}.json`;
  let startTime: number = parseInt(`$(date +%s%3N)`); // milliseconds
  
  // Make HTTP request using curl
  let curlCmd: string = `curl -s -w '{"http_code":"%{http_code}","time_total":"%{time_total}","size_download":"%{size_download}"}' -m ${timeout} "${url}" -o "${outputFile}.body"`;
  let curlResult: string = `$(${curlCmd} 2>/dev/null || echo '{"http_code":"000","time_total":"0","size_download":"0"}')`;
  
  let endTime: number = parseInt(`$(date +%s%3N)`);
  let totalTime: number = endTime - startTime;
  
  // Parse curl metrics
  let metrics: object = {};
  try {
    metrics = json.parse(curlResult);
  } catch {
    metrics = {
      "http_code": "000",
      "time_total": "0",
      "size_download": "0"
    };
  }
  
  // Create response metadata
  let response: object = {
    "url": url,
    "index": index,
    "timestamp": `$(date -Iseconds)`,
    "http_code": json.getString(metrics, ".http_code"),
    "time_total_ms": totalTime,
    "curl_time": parseFloat(json.getString(metrics, ".time_total")) * 1000,
    "size_bytes": parseInt(json.getString(metrics, ".size_download")),
    "success": json.getString(metrics, ".http_code").startsWith("2"),
    "body_file": `${outputFile}.body`
  };
  
  // Save response metadata
  fs.writeFile(outputFile, json.stringify(response, true));
  
  let httpCode: string = json.getString(response, ".http_code");
  let timeMs: number = json.getNumber(response, ".time_total_ms");
  
  if (json.getBoolean(response, ".success")) {
    console.log(`✅ [${index}] ${url} → ${httpCode} (${timeMs}ms)`);
  } else {
    console.log(`❌ [${index}] ${url} → ${httpCode} (${timeMs}ms)`);
  }
  
  return outputFile;
}

// Batch processing with concurrency control
let batchSize: number = concurrency;
let processedCount: number = 0;
let results: string[] = [];

while (processedCount < urls.length) {
  // Create current batch
  let batch: string[] = [];
  let batchPids: object = {};
  
  for (let i: number = 0; i < batchSize && processedCount + i < urls.length; i++) {
    let urlIndex: number = processedCount + i;
    let url: string = urls[urlIndex];
    batch.push(url);
    
    // Start request in background
    let command: string = `utah -c "makeRequest('${url}', ${urlIndex})"`;
    let pid: string = `$(makeRequest "${url}" ${urlIndex} & echo $!)`;
    
    if (pid.trim() != "") {
      batchPids = json.set(batchPids, `.${pid}`, {
        "url": url,
        "index": urlIndex
      });
    }
  }
  
  console.log(`Processing batch: ${batch.length} requests`);
  
  // Wait for batch to complete
  let pids: string[] = json.keys(batchPids);
  for (let pid: string in pids) {
    `$(wait ${pid})`;
  }
  
  processedCount += batch.length;
  console.log(`Completed: ${processedCount}/${urls.length} requests`);
}

// Analyze results
console.log("\nAnalyzing results...");

let responseFiles: string[] = `$(find ${resultsDir} -name "response-*.json" -type f)`.split("\n");
let successCount: number = 0;
let errorCount: number = 0;
let totalTime: number = 0;
let totalSize: number = 0;

let summary: object = {
  "total_requests": urls.length,
  "successful": 0,
  "failed": 0,
  "average_time_ms": 0,
  "total_size_bytes": 0,
  "status_codes": {}
};

for (let responseFile: string in responseFiles) {
  if (responseFile.trim() != "") {
    let responseContent: string = fs.readFile(responseFile);
    let response: object = json.parse(responseContent);
    
    let isSuccess: boolean = json.getBoolean(response, ".success");
    let timeMs: number = json.getNumber(response, ".time_total_ms");
    let sizeBytes: number = json.getNumber(response, ".size_bytes");
    let httpCode: string = json.getString(response, ".http_code");
    
    if (isSuccess) {
      successCount++;
    } else {
      errorCount++;
    }
    
    totalTime += timeMs;
    totalSize += sizeBytes;
    
    // Count status codes
    let statusPath: string = `.status_codes.${httpCode}`;
    let statusCount: number = json.getNumber(summary, statusPath) || 0;
    summary = json.set(summary, statusPath, statusCount + 1);
  }
}

// Update summary
summary = json.set(summary, ".successful", successCount);
summary = json.set(summary, ".failed", errorCount);
summary = json.set(summary, ".average_time_ms", Math.round(totalTime / urls.length));
summary = json.set(summary, ".total_size_bytes", totalSize);

// Save summary
fs.writeFile(`${resultsDir}/summary.json`, json.stringify(summary, true));

console.log(`\nConcurrent API calls completed:`);
console.log(`✅ Successful: ${successCount}`);
console.log(`❌ Failed: ${errorCount}`);
console.log(`⏱️  Average time: ${Math.round(totalTime / urls.length)}ms`);
console.log(`📦 Total data: ${Math.round(totalSize / 1024)}KB`);
console.log(`📊 Results saved to: ${resultsDir}/`);
```

## Synchronization and Coordination

### File-based Coordination

```typescript
script.description("Coordinate parallel processes using file-based synchronization");

args.define("--process-id", "-p", "Process ID for coordination", "string", true);
args.define("--total-processes", "-t", "Total number of processes", "number", true);
args.define("--sync-dir", "-s", "Synchronization directory", "string", false, "./sync");

let processId: string = args.getString("--process-id");
let totalProcesses: number = args.getNumber("--total-processes");
let syncDir: string = args.getString("--sync-dir");

fs.createDirectory(syncDir);

console.log(`Process ${processId} starting coordination with ${totalProcesses} total processes`);

// Function to wait for all processes to reach a barrier
function waitForBarrier(barrierName: string): void {
  let barrierFile: string = `${syncDir}/${barrierName}_${processId}.ready`;
  
  // Signal this process is ready
  fs.writeFile(barrierFile, `$(date -Iseconds)`);
  console.log(`Process ${processId} reached barrier: ${barrierName}`);
  
  // Wait for all processes to be ready
  while (true) {
    let readyFiles: string[] = `$(find ${syncDir} -name "${barrierName}_*.ready" -type f)`.split("\n");
    let readyCount: number = 0;
    
    for (let file: string in readyFiles) {
      if (file.trim() != "") {
        readyCount++;
      }
    }
    
    if (readyCount >= totalProcesses) {
      console.log(`All processes reached barrier: ${barrierName}`);
      break;
    }
    
    console.log(`Waiting at barrier ${barrierName}: ${readyCount}/${totalProcesses} processes ready`);
    `$(sleep 2)`;
  }
}

// Function to acquire a distributed lock
function acquireLock(lockName: string, timeoutSeconds: number = 30): boolean {
  let lockFile: string = `${syncDir}/${lockName}.lock`;
  let startTime: number = parseInt(`$(date +%s)`);
  
  while (true) {
    // Try to create lock file atomically
    let lockResult: string = `$(set -C; echo "${processId}" > "${lockFile}" 2>/dev/null && echo "acquired" || echo "failed")`;
    
    if (lockResult.trim() == "acquired") {
      console.log(`Process ${processId} acquired lock: ${lockName}`);
      return true;
    }
    
    // Check timeout
    let currentTime: number = parseInt(`$(date +%s)`);
    if (currentTime - startTime > timeoutSeconds) {
      console.log(`Process ${processId} failed to acquire lock ${lockName} (timeout)`);
      return false;
    }
    
    // Check if lock holder is still alive
    if (fs.exists(lockFile)) {
      let lockHolder: string = fs.readFile(lockFile).trim();
      let holderAlive: string = `$(ps aux | grep "${lockHolder}" | grep -v grep >/dev/null && echo "alive" || echo "dead")`;
      
      if (holderAlive.trim() == "dead") {
        console.log(`Lock holder ${lockHolder} appears dead, removing stale lock`);
        `$(rm -f "${lockFile}")`;
      }
    }
    
    `$(sleep 1)`;
  }
}

// Function to release a distributed lock
function releaseLock(lockName: string): void {
  let lockFile: string = `${syncDir}/${lockName}.lock`;
  
  if (fs.exists(lockFile)) {
    let lockHolder: string = fs.readFile(lockFile).trim();
    
    if (lockHolder == processId) {
      `$(rm -f "${lockFile}")`;
      console.log(`Process ${processId} released lock: ${lockName}`);
    } else {
      console.log(`Process ${processId} cannot release lock ${lockName} (held by ${lockHolder})`);
    }
  }
}

// Function to update shared state
function updateSharedState(key: string, value: string): void {
  if (acquireLock("shared_state", 10)) {
    let stateFile: string = `${syncDir}/shared_state.json`;
    let state: object = {};
    
    if (fs.exists(stateFile)) {
      let stateContent: string = fs.readFile(stateFile);
      try {
        state = json.parse(stateContent);
      } catch {
        state = {};
      }
    }
    
    state = json.set(state, `.${key}`, value);
    fs.writeFile(stateFile, json.stringify(state, true));
    
    releaseLock("shared_state");
    console.log(`Process ${processId} updated shared state: ${key} = ${value}`);
  } else {
    console.log(`Process ${processId} failed to update shared state: ${key}`);
  }
}

// Function to read shared state
function readSharedState(key: string): string {
  let stateFile: string = `${syncDir}/shared_state.json`;
  
  if (fs.exists(stateFile)) {
    let stateContent: string = fs.readFile(stateFile);
    try {
      let state: object = json.parse(stateContent);
      return json.getString(state, `.${key}`) || "";
    } catch {
      return "";
    }
  }
  
  return "";
}

// Example coordinated workflow
console.log("Starting coordinated workflow...");

// Phase 1: Initialization
updateSharedState(`process_${processId}_status`, "initializing");
waitForBarrier("initialization");

// Phase 2: Work distribution
if (acquireLock("work_distribution", 30)) {
  console.log(`Process ${processId} is distributing work...`);
  
  // Create work items
  for (let i: number = 1; i <= 10; i++) {
    let workFile: string = `${syncDir}/work_item_${i}.json`;
    let workItem: object = {
      "id": i,
      "task": `Process item ${i}`,
      "created_by": processId,
      "created_at": `$(date -Iseconds)`,
      "status": "pending"
    };
    
    fs.writeFile(workFile, json.stringify(workItem, true));
  }
  
  updateSharedState("work_distributed", "true");
  releaseLock("work_distribution");
} else {
  console.log(`Process ${processId} waiting for work distribution...`);
  
  while (readSharedState("work_distributed") != "true") {
    `$(sleep 1)`;
  }
}

waitForBarrier("work_distributed");

// Phase 3: Parallel work processing
let processedCount: number = 0;

while (true) {
  // Try to claim a work item
  let workItems: string[] = `$(find ${syncDir} -name "work_item_*.json" -type f)`.split("\n");
  let claimed: boolean = false;
  
  for (let workFile: string in workItems) {
    if (workFile.trim() != "" && !claimed) {
      if (acquireLock(`work_${fs.filename(workFile)}`, 5)) {
        if (fs.exists(workFile)) {
          let workContent: string = fs.readFile(workFile);
          let workItem: object = json.parse(workContent);
          
          if (json.getString(workItem, ".status") == "pending") {
            // Claim the work item
            workItem = json.set(workItem, ".status", "processing");
            workItem = json.set(workItem, ".assigned_to", processId);
            workItem = json.set(workItem, ".started_at", `$(date -Iseconds)`);
            
            fs.writeFile(workFile, json.stringify(workItem, true));
            
            let taskId: number = json.getNumber(workItem, ".id");
            console.log(`Process ${processId} claimed work item ${taskId}`);
            
            // Simulate work
            `$(sleep $((RANDOM % 5 + 1)))`;
            
            // Mark as completed
            workItem = json.set(workItem, ".status", "completed");
            workItem = json.set(workItem, ".completed_at", `$(date -Iseconds)`);
            
            fs.writeFile(workFile, json.stringify(workItem, true));
            
            console.log(`Process ${processId} completed work item ${taskId}`);
            processedCount++;
            claimed = true;
          }
        }
        
        releaseLock(`work_${fs.filename(workFile)}`);
      }
    }
  }
  
  if (!claimed) {
    // Check if all work is done
    let pendingWork: string[] = `$(grep -l '"status":"pending"' ${syncDir}/work_item_*.json 2>/dev/null || echo "")`.split("\n");
    let hasPending: boolean = false;
    
    for (let file: string in pendingWork) {
      if (file.trim() != "") {
        hasPending = true;
        break;
      }
    }
    
    if (!hasPending) {
      console.log(`Process ${processId} found no more work available`);
      break;
    }
    
    `$(sleep 1)`;
  }
}

updateSharedState(`process_${processId}_completed`, processedCount.toString());
waitForBarrier("work_completed");

// Phase 4: Results aggregation
if (acquireLock("results_aggregation", 30)) {
  console.log(`Process ${processId} aggregating results...`);
  
  let results: object = {
    "total_processes": totalProcesses,
    "aggregated_at": `$(date -Iseconds)`,
    "aggregated_by": processId,
    "process_results": {}
  };
  
  // Collect results from each process
  for (let i: number = 1; i <= totalProcesses; i++) {
    let processKey: string = `process_${i}`;
    let completed: string = readSharedState(`${processKey}_completed`);
    
    results = json.set(results, `.process_results.${processKey}`, {
      "items_processed": parseInt(completed) || 0
    });
  }
  
  fs.writeFile(`${syncDir}/final_results.json`, json.stringify(results, true));
  updateSharedState("results_ready", "true");
  
  releaseLock("results_aggregation");
}

// Wait for results to be ready
while (readSharedState("results_ready") != "true") {
  `$(sleep 1)`;
}

console.log(`Process ${processId} completed coordination. Processed ${processedCount} items.`);

// Clean up process-specific files
`$(rm -f ${syncDir}/*_${processId}.ready)`;
```

## Performance Monitoring

### Resource Usage Tracking

```typescript
script.description("Monitor resource usage of parallel processes");

args.define("--monitor-interval", "-i", "Monitoring interval in seconds", "number", false, 5);
args.define("--output-file", "-o", "Output file for monitoring data", "string", false, "resource-usage.json");

let monitorInterval: number = args.getNumber("--monitor-interval");
let outputFile: string = args.getString("--output-file");

// Function to collect system metrics
function collectSystemMetrics(): object {
  let timestamp: number = parseInt(`$(date +%s)`);
  
  // CPU usage
  let cpuUsage: string = `$(top -bn1 | grep "Cpu(s)" | awk '{print $2}' | cut -d'%' -f1)`;
  
  // Memory usage
  let memInfo: string[] = `$(free -m | grep Mem)`.split(/\s+/);
  let totalMem: number = parseInt(memInfo[1] || "0");
  let usedMem: number = parseInt(memInfo[2] || "0");
  
  // Load average
  let loadAvg: string = `$(uptime | awk -F'load average:' '{print $2}' | awk -F',' '{print $1}' | tr -d ' ')`;
  
  // Process count
  let processCount: number = parseInt(`$(ps aux | wc -l)`) - 1; // Subtract header
  
  return {
    "timestamp": timestamp,
    "cpu_usage_percent": parseFloat(cpuUsage) || 0,
    "memory_total_mb": totalMem,
    "memory_used_mb": usedMem,
    "memory_usage_percent": Math.round((usedMem / totalMem) * 100),
    "load_average": parseFloat(loadAvg) || 0,
    "process_count": processCount
  };
}

// Function to monitor specific processes
function monitorProcesses(pids: string[]): object[] {
  let processMetrics: object[] = [];
  
  for (let pid: string in pids) {
    if (pid.trim() != "") {
      // Check if process exists
      let exists: string = `$(kill -0 ${pid} 2>/dev/null && echo "yes" || echo "no")`;
      
      if (exists.trim() == "yes") {
        // Get process info
        let psInfo: string = `$(ps -p ${pid} -o pid,ppid,pcpu,pmem,vsz,rss,comm --no-headers 2>/dev/null || echo "")`;
        
        if (psInfo.trim() != "") {
          let fields: string[] = psInfo.trim().split(/\s+/);
          
          let processMetric: object = {
            "pid": parseInt(fields[0] || "0"),
            "ppid": parseInt(fields[1] || "0"),
            "cpu_percent": parseFloat(fields[2] || "0"),
            "memory_percent": parseFloat(fields[3] || "0"),
            "virtual_memory_kb": parseInt(fields[4] || "0"),
            "resident_memory_kb": parseInt(fields[5] || "0"),
            "command": fields[6] || "unknown"
          };
          
          processMetrics.push(processMetric);
        }
      }
    }
  }
  
  return processMetrics;
}

// Main monitoring function
function startMonitoring(processesToMonitor: string[]): void {
  console.log(`Starting resource monitoring (interval: ${monitorInterval}s)`);
  console.log(`Monitoring ${processesToMonitor.length} processes`);
  console.log(`Output file: ${outputFile}`);
  
  let monitoringData: object[] = [];
  let startTime: number = parseInt(`$(date +%s)`);
  
  while (true) {
    let systemMetrics: object = collectSystemMetrics();
    let processMetrics: object[] = monitorProcesses(processesToMonitor);
    
    let dataPoint: object = {
      "system": systemMetrics,
      "processes": processMetrics,
      "monitoring_duration": parseInt(`$(date +%s)`) - startTime
    };
    
    monitoringData.push(dataPoint);
    
    // Display current metrics
    let cpuUsage: number = json.getNumber(systemMetrics, ".cpu_usage_percent");
    let memUsage: number = json.getNumber(systemMetrics, ".memory_usage_percent");
    let loadAvg: number = json.getNumber(systemMetrics, ".load_average");
    let activeProcesses: number = processMetrics.length;
    
    console.log(`[${`$(date '+%H:%M:%S')`}] CPU: ${cpuUsage}%, Mem: ${memUsage}%, Load: ${loadAvg}, Active: ${activeProcesses}`);
    
    // Save data periodically
    fs.writeFile(outputFile, json.stringify(monitoringData, true));
    
    // Check if any processes are still running
    if (activeProcesses == 0 && processesToMonitor.length > 0) {
      console.log("All monitored processes have finished");
      break;
    }
    
    `$(sleep ${monitorInterval})`;
  }
  
  console.log(`Monitoring completed. Data saved to: ${outputFile}`);
}

// Example usage with background processes
let backgroundTasks: string[] = [
  "sleep 30",
  "find / -name '*.txt' 2>/dev/null | wc -l",
  "dd if=/dev/zero of=/tmp/test_file bs=1M count=100 2>/dev/null",
  "ping -c 100 8.8.8.8 >/dev/null"
];

let taskPids: string[] = [];

// Start background tasks
for (let i: number = 0; i < backgroundTasks.length; i++) {
  let task: string = backgroundTasks[i];
  console.log(`Starting task ${i + 1}: ${task}`);
  
  let pid: string = `$(${task} & echo $!)`;
  if (pid.trim() != "") {
    taskPids.push(pid);
    console.log(`Task ${i + 1} started with PID: ${pid}`);
  }
}

// Start monitoring
if (taskPids.length > 0) {
  startMonitoring(taskPids);
} else {
  console.log("No tasks started, exiting");
}
```

## Best Practices

### Error Handling in Parallel Execution

```typescript
// Robust error handling for parallel processes
function executeWithErrorHandling(command: string, taskId: string): object {
  let startTime: number = parseInt(`$(date +%s%3N)`);
  
  try {
    let result: string = `$(${command} 2>&1; echo "EXIT_CODE:$?")`;
    let endTime: number = parseInt(`$(date +%s%3N)`);
    
    let exitCode: string = result.split("EXIT_CODE:")[1]?.trim() || "1";
    let output: string = result.split("EXIT_CODE:")[0] || "";
    
    return {
      "task_id": taskId,
      "success": exitCode == "0",
      "exit_code": parseInt(exitCode),
      "output": output.trim(),
      "duration_ms": endTime - startTime,
      "timestamp": `$(date -Iseconds)`
    };
    
  } catch {
    let endTime: number = parseInt(`$(date +%s%3N)`);
    
    return {
      "task_id": taskId,
      "success": false,
      "exit_code": -1,
      "output": "Task execution failed",
      "duration_ms": endTime - startTime,
      "timestamp": `$(date -Iseconds)`,
      "error": "Exception during execution"
    };
  }
}

// Cleanup function for orphaned processes
function cleanupOrphanedProcesses(parentPid: string): void {
  console.log("Cleaning up orphaned processes...");
  
  let children: string[] = `$(pgrep -P ${parentPid} 2>/dev/null || echo "")`.split("\n");
  
  for (let childPid: string in children) {
    if (childPid.trim() != "") {
      console.log(`Terminating child process: ${childPid}`);
      `$(kill -TERM ${childPid} 2>/dev/null || true)`;
      
      `$(sleep 2)`;
      
      // Force kill if still running
      let stillRunning: string = `$(kill -0 ${childPid} 2>/dev/null && echo "yes" || echo "no")`;
      if (stillRunning.trim() == "yes") {
        console.log(`Force killing process: ${childPid}`);
        `$(kill -KILL ${childPid} 2>/dev/null || true)`;
      }
    }
  }
}
```

### Memory Management

```typescript
// Monitor and control memory usage
function monitorMemoryUsage(maxMemoryMB: number): boolean {
  let currentMemory: number = parseInt(`$(free -m | grep Mem | awk '{print $3}')`);
  let memoryPercent: number = Math.round((currentMemory / maxMemoryMB) * 100);
  
  if (memoryPercent > 90) {
    console.log(`⚠️  High memory usage: ${memoryPercent}% (${currentMemory}MB)`);
    return false;
  }
  
  return true;
}

// Throttle process creation based on system resources
function shouldCreateNewProcess(): boolean {
  let loadAvg: number = parseFloat(`$(uptime | awk -F'load average:' '{print $2}' | awk -F',' '{print $1}' | tr -d ' ')`);
  let cpuCount: number = parseInt(`$(nproc)`);
  
  if (loadAvg > cpuCount * 2) {
    console.log(`⚠️  High system load: ${loadAvg} (CPU cores: ${cpuCount})`);
    return false;
  }
  
  return true;
}
```

## Next Steps

- **[Performance Optimization](performance.md)** - Optimize parallel execution performance
- **[Security Best Practices](security.md)** - Secure parallel processing
- **[Cloud Automation](cloud.md)** - Parallel processing in cloud environments

Parallel execution with Utah enables efficient resource utilization and improved performance for CPU-intensive and I/O-bound tasks. Use these patterns to build scalable automation solutions.
