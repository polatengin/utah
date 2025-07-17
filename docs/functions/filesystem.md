---
layout: default
title: Filesystem Functions
parent: Functions
nav_order: 3
---

The `fs` namespace provides comprehensive file and directory operations. These functions handle file I/O, path manipulation, and filesystem queries with proper error handling.

## File Operations

### fs.exists()

Check if a file or directory exists:

```typescript
if (fs.exists("config.json")) {
  console.log("Configuration file found");
} else {
  console.log("Configuration file missing");
}

if (fs.exists("/etc/passwd")) {
  console.log("System password file exists");
}
```

**Generated Bash:**

```bash
if [ -e "config.json" ]; then
  echo "Configuration file found"
else
  echo "Configuration file missing"
fi

if [ -e "/etc/passwd" ]; then
  echo "System password file exists"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_exists.shx`
- Tests file existence checking with `-e` test

### fs.readFile()

Read the contents of a file:

```typescript
let config: string = fs.readFile("app.json");
console.log(`Config content: ${config}`);

try {
  let data: string = fs.readFile("important.txt");
  console.log("File loaded successfully");
}
catch {
  console.log("Failed to read file");
}
```

**Generated Bash:**

```bash
config=$(cat "app.json")
echo "Config content: ${config}"

if data=$(cat "important.txt" 2>/dev/null); then
  echo "File loaded successfully"
else
  echo "Failed to read file"
fi
```

### fs.writeFile()

Write content to a file (overwrites existing content):

```typescript
let content: string = "Hello, World!";
fs.writeFile("output.txt", content);

let jsonData: string = '{"name": "test", "value": 123}';
fs.writeFile("data.json", jsonData);
```

**Generated Bash:**

```bash
content="Hello, World!"
echo "${content}" > "output.txt"

jsonData='{"name": "test", "value": 123}'
echo "${jsonData}" > "data.json"
```

### fs.appendFile()

Append content to a file:

```typescript
let logEntry: string = `[${timer.current()}] Application started`;
fs.appendFile("app.log", logEntry + "\n");

fs.appendFile("notes.txt", "Additional note\n");
```

**Generated Bash:**

```bash
logEntry="[$(date '+%Y-%m-%d %H:%M:%S')] Application started"
echo "${logEntry}" >> "app.log"

echo "Additional note" >> "notes.txt"
```

## Path Manipulation

### fs.filename()

Extract the filename from a path:

```typescript
let fullPath: string = "/home/user/documents/report.pdf";
let filename: string = fs.filename(fullPath);
console.log(`Filename: ${filename}`); // Output: report.pdf
```

**Generated Bash:**

```bash
fullPath="/home/user/documents/report.pdf"
filename=$(basename "${fullPath}")
echo "Filename: ${filename}"
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_filename.shx`
- Tests filename extraction using `basename`

### fs.dirname()

Extract the directory path from a full path:

```typescript
let fullPath: string = "/home/user/documents/report.pdf";
let directory: string = fs.dirname(fullPath);
console.log(`Directory: ${directory}`); // Output: /home/user/documents
```

**Generated Bash:**

```bash
fullPath="/home/user/documents/report.pdf"
directory=$(dirname "${fullPath}")
echo "Directory: ${directory}"
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_dirname.shx`
- Tests directory extraction using `dirname`

### fs.extension()

Get the file extension:

```typescript
let filename: string = "document.pdf";
let ext: string = fs.extension(filename);
console.log(`Extension: ${ext}`); // Output: pdf

let noExt: string = fs.extension("README");
console.log(`Extension: ${noExt}`); // Output: (empty)
```

**Generated Bash:**

```bash
filename="document.pdf"
ext="${filename##*.}"
if [ "${ext}" = "${filename}" ]; then
  ext=""
fi
echo "Extension: ${ext}"

noExt="${filename##*.}"
if [ "${noExt}" = "${filename}" ]; then
  noExt=""
fi
echo "Extension: ${noExt}"
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_extension.shx`
- Tests file extension extraction with fallback for files without extensions

### fs.parentDirName()

Get the name of the parent directory:

```typescript
let path: string = "/home/user/projects/myapp/src";
let parentName: string = fs.parentDirName(path);
console.log(`Parent directory: ${parentName}`); // Output: myapp
```

**Generated Bash:**

```bash
path="/home/user/projects/myapp/src"
parentName=$(basename "$(dirname "${path}")")
echo "Parent directory: ${parentName}"
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_parentdirname.shx`
- Tests parent directory name extraction

### fs.path()

Construct a file path by joining components:

```typescript
let dir: string = "/home/user";
let subdir: string = "documents";
let filename: string = "report.txt";

let fullPath: string = fs.path(dir, subdir, filename);
console.log(`Full path: ${fullPath}`); // Output: /home/user/documents/report.txt
```

**Generated Bash:**

```bash
dir="/home/user"
subdir="documents"
filename="report.txt"

fullPath="${dir}/${subdir}/${filename}"
echo "Full path: ${fullPath}"
```

**Test Coverage:**

- File: `tests/positive_fixtures/fs_path.shx`
- Tests path construction with proper separator handling

## Directory Operations

### Creating Directories

```typescript
// Create a single directory
`$(mkdir -p "new_directory")`;

// Create nested directories
`$(mkdir -p "project/src/components")`;

// Check if directory was created
if (fs.exists("project/src")) {
  console.log("Directory structure created successfully");
}
```

### Listing Directory Contents

```typescript
// List files in current directory
let files: string = `$(ls -la)`;
console.log(`Directory contents:\n${files}`);

// List only files (not directories)
let filesOnly: string = `$(find . -maxdepth 1 -type f)`;
console.log(`Files only:\n${filesOnly}`);

// List only directories
let dirsOnly: string = `$(find . -maxdepth 1 -type d)`;
console.log(`Directories only:\n${dirsOnly}`);
```

## Practical Examples

### File Backup System

```typescript
script.description("File backup utility");

function backupFile(source: string, backupDir: string): boolean {
  if (!fs.exists(source)) {
    console.log(`Source file not found: ${source}`);
    return false;
  }

  // Create backup directory if it doesn't exist
  if (!fs.exists(backupDir)) {
    `$(mkdir -p "${backupDir}")`;
  }

  // Generate backup filename with timestamp
  let filename: string = fs.filename(source);
  let extension: string = fs.extension(filename);
  let baseName: string = filename.replace(`.${extension}`, "");
  let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;

  let backupName: string;
  if (extension == "") {
    backupName = `${baseName}_${timestamp}`;
  } else {
    backupName = `${baseName}_${timestamp}.${extension}`;
  }

  let backupPath: string = fs.path(backupDir, backupName);

  // Copy file to backup location
  let result: number = `$(cp "${source}" "${backupPath}")`;
  if (result == 0) {
    console.log(`✓ Backed up: ${source} → ${backupPath}`);
    return true;
  } else {
    console.log(`✗ Failed to backup: ${source}`);
    return false;
  }
}

// Usage
let sourceFiles: string[] = ["config.json", "database.db", "app.log"];
let backupDirectory: string = "backups";

for (let file: string in sourceFiles) {
  backupFile(file, backupDirectory);
}
```

### Log File Manager

```typescript
script.description("Log file rotation and cleanup");

function rotateLogFile(logFile: string, maxSize: number): void {
  if (!fs.exists(logFile)) {
    console.log(`Log file not found: ${logFile}`);
    return;
  }

  // Check file size (in bytes)
  let size: number = `$(stat -c%s "${logFile}")`;

  if (size > maxSize) {
    console.log(`Log file ${logFile} is ${size} bytes, rotating...`);

    // Create rotated filename
    let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;
    let dir: string = fs.dirname(logFile);
    let filename: string = fs.filename(logFile);
    let rotatedName: string = `${filename}.${timestamp}`;
    let rotatedPath: string = fs.path(dir, rotatedName);

    // Move current log to rotated name
    `$(mv "${logFile}" "${rotatedPath}")`;

    // Create new empty log file
    fs.writeFile(logFile, "");

    console.log(`Log rotated: ${logFile} → ${rotatedPath}`);
  } else {
    console.log(`Log file ${logFile} is ${size} bytes, no rotation needed`);
  }
}

function cleanupOldLogs(logDir: string, daysToKeep: number): void {
  if (!fs.exists(logDir)) {
    console.log(`Log directory not found: ${logDir}`);
    return;
  }

  console.log(`Cleaning up logs older than ${daysToKeep} days in ${logDir}`);

  // Find and remove old log files
  `$(find "${logDir}" -name "*.log.*" -type f -mtime +${daysToKeep} -delete)`;

  console.log("Log cleanup completed");
}

// Usage
rotateLogFile("app.log", 10485760); // 10MB
rotateLogFile("error.log", 5242880); // 5MB
cleanupOldLogs("logs", 30); // Keep 30 days
```

### Configuration File Manager

```typescript
script.description("Configuration file manager");

function loadConfig(configFile: string): object {
  if (!fs.exists(configFile)) {
    console.log(`Config file not found: ${configFile}, creating default`);
    let defaultConfig: object = json.parse('{"version": "1.0", "debug": false}');
    saveConfig(configFile, defaultConfig);
    return defaultConfig;
  }

  try {
    let content: string = fs.readFile(configFile);
    if (!json.isValid(content)) {
      console.log(`Invalid JSON in ${configFile}, using defaults`);
      return json.parse('{"version": "1.0", "debug": false}');
    }
    return json.parse(content);
  }
  catch {
    console.log(`Error reading ${configFile}, using defaults`);
    return json.parse('{"version": "1.0", "debug": false}');
  }
}

function saveConfig(configFile: string, config: object): void {
  try {
    let configJson: string = json.stringify(config, true);
    fs.writeFile(configFile, configJson);
    console.log(`Configuration saved to ${configFile}`);
  }
  catch {
    console.log(`Error saving configuration to ${configFile}`);
  }
}

function backupConfig(configFile: string): string {
  if (!fs.exists(configFile)) {
    return "";
  }

  let dir: string = fs.dirname(configFile);
  let filename: string = fs.filename(configFile);
  let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;
  let backupName: string = `${filename}.backup.${timestamp}`;
  let backupPath: string = fs.path(dir, backupName);

  `$(cp "${configFile}" "${backupPath}")`;
  console.log(`Config backed up to: ${backupPath}`);
  return backupPath;
}

// Usage
let configPath: string = "app.json";

// Backup existing config
let backupPath: string = backupConfig(configPath);

// Load current config
let config: object = loadConfig(configPath);

// Modify config
config = json.set(config, ".lastUpdate", timer.current());
config = json.set(config, ".version", "1.1");

// Save updated config
saveConfig(configPath, config);
```

### Project File Organizer

```typescript
script.description("Organize project files by type");

function organizeFiles(sourceDir: string): void {
  if (!fs.exists(sourceDir)) {
    console.log(`Source directory not found: ${sourceDir}`);
    return;
  }

  // Define file type mappings
  let typeMap: object = json.parse(`{
    "images": ["jpg", "jpeg", "png", "gif", "bmp", "svg"],
    "documents": ["pdf", "doc", "docx", "txt", "md"],
    "code": ["js", "ts", "py", "java", "cpp", "c", "h"],
    "data": ["json", "xml", "csv", "yaml", "yml"],
    "archives": ["zip", "tar", "gz", "rar", "7z"]
  }`);

  // Get all files in source directory
  let allFiles: string = `$(find "${sourceDir}" -maxdepth 1 -type f)`;
  let fileList: string[] = allFiles.split("\n");

  for (let file: string in fileList) {
    if (file == "") continue;

    let filename: string = fs.filename(file);
    let extension: string = fs.extension(filename);

    if (extension == "") {
      console.log(`Skipping file without extension: ${filename}`);
      continue;
    }

    // Find appropriate category
    let category: string = "misc";

    if (json.getArray(typeMap, ".images").contains(extension)) {
      category = "images";
    } else if (json.getArray(typeMap, ".documents").contains(extension)) {
      category = "documents";
    } else if (json.getArray(typeMap, ".code").contains(extension)) {
      category = "code";
    } else if (json.getArray(typeMap, ".data").contains(extension)) {
      category = "data";
    } else if (json.getArray(typeMap, ".archives").contains(extension)) {
      category = "archives";
    }

    // Create category directory
    let categoryDir: string = fs.path(sourceDir, category);
    if (!fs.exists(categoryDir)) {
      `$(mkdir -p "${categoryDir}")`;
      console.log(`Created directory: ${categoryDir}`);
    }

    // Move file to category directory
    let destination: string = fs.path(categoryDir, filename);
    `$(mv "${file}" "${destination}")`;
    console.log(`Moved ${filename} to ${category}/`);
  }

  console.log("File organization completed");
}

// Usage
args.define("--dir", "-d", "Directory to organize", "string", false, ".");
let targetDir: string = args.getString("--dir");

organizeFiles(targetDir);
```

## Best Practices

### 1. Always Check File Existence

```typescript
// Good - check before operations
if (fs.exists("config.json")) {
  let config: string = fs.readFile("config.json");
  // Process config...
} else {
  console.log("Config file not found, using defaults");
}

// Avoid - assuming files exist
let config: string = fs.readFile("config.json"); // May fail
```

### 2. Handle Path Separators Properly

```typescript
// Good - use fs.path() for cross-platform compatibility
let fullPath: string = fs.path(baseDir, subDir, filename);

// Avoid - hardcoding separators
let fullPath: string = baseDir + "/" + subDir + "/" + filename; // Unix-specific
```

### 3. Use Try-Catch for File Operations

```typescript
function safeReadFile(filename: string): string {
  try {
    return fs.readFile(filename);
  }
  catch {
    console.log(`Failed to read file: ${filename}`);
    return "";
  }
}
```

### 4. Validate File Extensions

```typescript
function isImageFile(filename: string): boolean {
  let ext: string = fs.extension(filename).toLowerCase();
  let imageExts: string[] = ["jpg", "jpeg", "png", "gif", "bmp"];
  return imageExts.contains(ext);
}
```

### 5. Create Directories When Needed

```typescript
function ensureDirectory(dirPath: string): void {
  if (!fs.exists(dirPath)) {
    `$(mkdir -p "${dirPath}")`;
    console.log(`Created directory: ${dirPath}`);
  }
}
```

## Function Reference Summary

| Function | Purpose | Return Type | Example |
|----------|---------|-------------|---------|
| `fs.exists(path)` | Check existence | boolean | `fs.exists("file.txt")` |
| `fs.readFile(path)` | Read file content | string | `fs.readFile("config.json")` |
| `fs.writeFile(path, content)` | Write to file | void | `fs.writeFile("out.txt", data)` |
| `fs.appendFile(path, content)` | Append to file | void | `fs.appendFile("log.txt", entry)` |
| `fs.filename(path)` | Extract filename | string | `fs.filename("/path/file.txt")` |
| `fs.dirname(path)` | Extract directory | string | `fs.dirname("/path/file.txt")` |
| `fs.extension(path)` | Get file extension | string | `fs.extension("file.txt")` |
| `fs.parentDirName(path)` | Parent dir name | string | `fs.parentDirName("/a/b/c")` |
| `fs.path(...)` | Join path components | string | `fs.path(dir, subdir, file)` |

Filesystem functions are essential for any script that works with files and directories. They provide a safe, cross-platform way to manipulate the filesystem while maintaining Utah's type safety guarantees.
