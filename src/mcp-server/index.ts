import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";

const server = new McpServer({
  name: "Utah MCP Server",
  version: "1.0.0"
});

server.tool(
  "definition",
  "Provide utah language definitions",
  {},
  async () => {
    const payload = `
> Use the following documentation to build the shx file. Pay close attention to syntax and documentation.
# Utah Language Reference

Utah is a TypeScript-inspired language that transpiles to bash scripts. It provides modern syntax with strong typing for shell scripting.

## Variables & Functions

\`\`\`typescript
// Variables (mutable) and constants (immutable)
let name: string = "Alice";
const appName: string = "MyApp";

// Functions with parameters and return types
function greet(name: string): void {
  console.log(\`Hello, \${name}!\`);
}

function add(a: number, b: number): number {
  return a + b;
}
\`\`\`

## Control Flow

\`\`\`typescript
// Conditionals
if (count > 0) {
  console.log("Positive");
} else {
  console.log("Zero or negative");
}

// Ternary operator
let status: string = isActive ? "online" : "offline";

// Switch statements
switch (grade) {
  case "A": message = "Excellent!"; break;
  case "B": message = "Good!"; break;
  default: message = "Try harder"; break;
}

// For loops
for (let i: number = 0; i < 5; i++) {
  console.log(\`Count: \${i}\`);
}

// For-in loops (with arrays)
let fruits: string[] = ["apple", "banana"];
for (let fruit: string in fruits) {
  console.log(\`Fruit: \${fruit}\`);
}

// While loops
let i: number = 0;
while (i < 5) {
  console.log(\`Value: \${i}\`);
  i++;
  if (condition) break; // Early exit
}
\`\`\`

## Arrays

\`\`\`typescript
// Create and manipulate arrays
let numbers: number[] = [1, 2, 3, 4, 5];
let names: string[] = ["Alice", "Bob"];

// Array methods
let length: number = array.length(numbers);
let isEmpty: boolean = array.isEmpty(numbers);
let contains: boolean = array.contains(numbers, 3);
let joined: string = array.join(names, ", ");
let reversed: number[] = array.reverse(numbers);
let sorted: string[] = array.sort(names, "asc"); // or "desc"

// Create arrays from strings
let parts: string[] = string.split("a,b,c", ",");
\`\`\`

## Error Handling

\`\`\`typescript
// Try/catch for error handling
try {
  let content: string = fs.readFile("file.txt");
  console.log(content);
} catch {
  console.log("File not found - using default");
}

// Defer for cleanup (executes when function exits)
function processFile(): void {
  defer console.log("Cleanup completed");
  defer fs.delete("temp.txt");

  fs.writeFile("temp.txt", "data");
  // Defer statements execute automatically on exit
}
\`\`\`

## Built-in Functions

### File System

\`\`\`typescript
// File operations
fs.writeFile("file.txt", "content");
let content: string = fs.readFile("file.txt");
let exists: boolean = fs.exists("file.txt");
fs.copy("source.txt", "dest.txt");
fs.move("old.txt", "new.txt");
fs.delete("file.txt");

// Path operations
let dir: string = fs.dirname("/path/to/file.txt");
let filename: string = fs.fileName("/path/to/file.txt");
let ext: string = fs.extension("file.txt");
\`\`\`

### String Functions

\`\`\`typescript
let text: string = "Hello World";

// Basic operations
let length: number = string.length(text);
let upper: string = string.toUpperCase(text);
let lower: string = string.toLowerCase(text);
let trimmed: string = string.trim("  text  ");

// Search and test
let starts: boolean = string.startsWith(text, "Hello");
let contains: boolean = string.includes(text, "World");
let index: number = string.indexOf(text, "World");

// Manipulation
let sub: string = string.substring(text, 0, 5);
let replaced: string = string.replace(text, "World", "Universe");
let parts: string[] = string.split(text, " ");
\`\`\`

### System & Utilities

\`\`\`typescript
// Check installed packages
let hasGit: boolean = os.isInstalled("git");

// Git operations
git.undoLastCommit();
let hasChanges: boolean = git.hasChanges();

// Random numbers
let dice: number = utility.random(1, 6);      // 1-6
let percent: number = utility.random(0, 100); // 0-100

// Process info
let pid: number = process.id();
let cpu: number = process.cpu();
let memory: number = process.memory();

// Timing
timer.start();
// ... do work ...
let elapsed: number = timer.stop();

// Templates (environment variable substitution)
template.update("config.template", "config.yml");
\`\`\`

## Import System

\`\`\`typescript
import "utils.shx";
import "config.shx";
// Use functions from imported files
\`\`\`

## Usage Guide

**When to use:**

- \`let\` for mutable variables, \`const\` for immutable
- \`for\` loops when you know iteration count
- \`while\` loops for condition-based iteration
- \`try/catch\` for handling potential failures
- \`defer\` for guaranteed cleanup operations
- Array methods for list processing
- String functions for text manipulation
- File system functions for file operations

## Complete Example

\`\`\`typescript
import "utils.shx";

function processLogs(): void {
  defer console.log("Processing completed");
  defer fs.delete("temp.log");

  timer.start();

  let logFiles: string[] = ["app.log", "error.log", "debug.log"];

  for (let file: string in logFiles) {
    if (fs.exists(file)) {
      try {
        let content: string = fs.readFile(file);
        let processed: string = string.toUpperCase(content);
        fs.writeFile(\`processed_\${file}\`, processed);
        console.log(\`Processed: \${file}\`);
      } catch {
        console.log(\`Failed to process: \${file}\`);
      }
    }
  }

  let duration: number = timer.stop();
  console.log(\`Total time: \${duration}ms\`);
}

if (os.isInstalled("git")) {
  processLogs();
} else {
  console.log("Git required but not installed");
}
\`\`\`

Utah provides TypeScript-like syntax for shell scripting with automatic transpilation to efficient bash code.`;

    return {
      content: [{ type: "text", text: payload }],
    };
  }
);

const transport = new StdioServerTransport();

await server.connect(transport);
