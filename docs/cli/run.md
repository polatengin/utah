---
layout: default
title: Run Command
parent: CLI Reference
nav_order: 2
---

The `utah run` command compiles and immediately executes Utah (.shx) scripts or individual commands without creating persistent .sh files. This is ideal for development, testing, one-off script execution, and quick command testing.

## Basic Usage

```bash
# Run a .shx file
utah run <file.shx>

# Run a single command directly
utah run -c <command>
utah run --command <command>
```

## Examples

### File Execution

```bash
utah run hello.shx
```

### Direct Command Execution

```bash
# Simple console output
utah run -c "console.log('Hello, World!')"

# Check if a package is installed
utah run --command "os.isInstalled('git')"

# File operations
utah run -c "console.log(fs.exists('/etc/passwd'))"

# JSON operations
utah run --command "json.installDependencies()"

# Multiple statements (use quotes to wrap the entire command)
utah run -c "let name: string = 'Utah'; console.log(\`Hello from \${name}!\`);"
```

### With Arguments

Utah scripts can access command-line arguments through the `args` namespace:

**script.shx:**

```typescript
script.description("Example script with arguments");

args.define("--name", "-n", "Your name", "string", true);
args.define("--count", "-c", "Number of greetings", "number", false, 1);

let name: string = args.getString("--name");
let count: number = args.getNumber("--count");

for (let i: number = 0; i < count; i++) {
  console.log(`Hello, ${name}!`);
}
```

```bash
utah run script.shx --name "Alice" --count 3
```

Output:

```text
Hello, Alice!
Hello, Alice!
Hello, Alice!
```

### Direct Command Benefits

The `-c`/`--command` option is particularly useful for:

**Quick Testing:**

```bash
# Test a function before adding to a script
utah run -c "console.log(os.platform())"

# Verify file operations
utah run --command "console.log(fs.dirname('/path/to/file.txt'))"
```

**One-liners:**

```bash
# Quick system checks
utah run -c "console.log(os.isInstalled('docker') ? 'Docker available' : 'Docker not found')"

# Inline calculations
utah run --command "console.log(utility.random(1, 100))"
```

**Scripting Integration:**

```bash
# Use in shell scripts or CI/CD
if utah run -c "os.isInstalled('node')" | grep -q "true"; then
  echo "Node.js is installed"
fi
```

**Development Workflow:**

```bash
# Quick prototyping without creating files
utah run -c "
let data: string[] = ['apple', 'banana', 'cherry'];
for (let item of data) {
  console.log(item.toUpperCase());
}
"
```

## How It Works

### 1. Temporary Compilation

When you run `utah run script.shx`, Utah:

1. **Compiles** the .shx file to bash in memory
2. **Creates** a temporary file in the system temp directory
3. **Executes** the temporary bash script
4. **Streams** output in real-time
5. **Cleans up** the temporary file automatically

### 2. Real-time Output

Unlike `utah compile`, the run command streams output as it happens:

```typescript
// slow-script.shx
for (let i: number = 1; i <= 5; i++) {
  console.log(`Step ${i} of 5`);
  `$(sleep 1)`;  // Wait 1 second
}
console.log("Completed!");
```

```bash
utah run slow-script.shx
```

```text
Step 1 of 5
Step 2 of 5
Step 3 of 5
Step 4 of 5
Step 5 of 5
Completed!
```

### 3. Error Propagation

Exit codes and errors are properly propagated:

```typescript
// error-script.shx
console.log("Starting process...");

if (!fs.exists("required-file.txt")) {
  console.log("Error: required-file.txt not found");
  exit(1);
}

console.log("Process completed successfully");
```

```bash
utah run error-script.shx
echo "Exit code: $?"
```

```text
Starting process...
Error: required-file.txt not found
Exit code: 1
```

## Development Workflow

### Quick Testing

Use `utah run` for rapid iteration during development:

```bash
# Edit script.shx
vim script.shx

# Test immediately
utah run script.shx

# Edit and test again
vim script.shx
utah run script.shx
```

### Debug Output

Enable debug mode for detailed execution information:

```bash
export UTAH_DEBUG=1
utah run script.shx
```

This shows:

- Compilation progress
- Temporary file location
- Execution timing
- Cleanup status

### Interactive Development

Combine with file watching for automatic execution:

```bash
# Install inotify-tools (Linux)
sudo apt install inotify-tools

# Watch and auto-run
while inotifywait -e modify script.shx; do
  clear
  echo "File changed, running..."
  utah run script.shx
done
```

## Advanced Features

### Environment Variables

Scripts can access and modify environment variables:

```typescript
// env-script.shx
console.log(`Current PATH: ${PATH}`);
console.log(`Home directory: ${HOME}`);

// Set environment variable for child processes
ENV["CUSTOM_VAR"] = "Hello from Utah";
`$(echo $CUSTOM_VAR)`;
```

### Standard Input/Output

Scripts can read from stdin and write to stdout/stderr:

```typescript
// input-script.shx
console.log("Enter your name:");
let name: string = console.prompt("Name: ");
console.log(`Hello, ${name}!`);
```

```bash
echo "Alice" | utah run input-script.shx
```

### Process Management

Long-running scripts can be managed like normal processes:

```bash
# Run in background
utah run long-running-script.shx &
SCRIPT_PID=$!

# Check if still running
kill -0 $SCRIPT_PID 2>/dev/null && echo "Still running"

# Terminate if needed
kill $SCRIPT_PID
```

## Performance Considerations

### Compilation Overhead

Each `utah run` invocation includes compilation time:

| File Size | Compilation | Execution | Total |
|-----------|-------------|-----------|-------|
| < 100 lines | 50ms | Variable | 50ms + execution |
| 100-500 lines | 150ms | Variable | 150ms + execution |
| 500+ lines | 300ms+ | Variable | 300ms+ + execution |

### When to Use Run vs Compile

**Use `utah run` for:**

- Development and testing
- One-off script execution
- Interactive scripting
- Scripts that change frequently

**Use `utah compile` for:**

- Production deployment
- Scripts run multiple times
- Performance-critical applications
- When you need to inspect generated bash

### Optimization Tips

1. **Minimize imports:** Faster compilation
2. **Use caching:** For scripts with heavy imports
3. **Profile execution:** Identify bottlenecks

```bash
# Time the execution
time utah run script.shx

# Profile with detailed timing
UTAH_DEBUG=1 UTAH_PROFILE=1 utah run script.shx
```

## Error Handling

### Compilation Errors

If the script fails to compile, `utah run` stops with an error:

```bash
utah run broken.shx
```

```text
❌ Compilation failed: Syntax error at line 5: Expected ';'
```

### Runtime Errors

Runtime errors are shown with context:

```typescript
// runtime-error.shx
let result: number = `$(date +%Y)` / 0;  // Division by zero
```

```bash
utah run runtime-error.shx
```

```text
/tmp/utah_temp_12345.sh: line 3: division by 0 (error token is "0")
```

### Signal Handling

Scripts can be interrupted with Ctrl+C:

```bash
utah run infinite-loop.shx
# Press Ctrl+C to stop
^C
```

The temporary file is still cleaned up even after interruption.

## Integration Examples

### Testing Framework

```bash
#!/bin/bash
# test-runner.sh

echo "Running Utah script tests..."

test_files=(
  "tests/unit/test-math.shx"
  "tests/unit/test-strings.shx"
  "tests/integration/test-api.shx"
)

failed=0

for test_file in "${test_files[@]}"; do
  echo "Running $test_file..."

  if utah run "$test_file"; then
    echo "✅ $test_file passed"
  else
    echo "❌ $test_file failed"
    ((failed++))
  fi
done

if [ $failed -eq 0 ]; then
  echo "All tests passed!"
else
  echo "$failed test(s) failed"
  exit 1
fi
```

### Build Automation

```bash
#!/bin/bash
# dev-workflow.sh

set -e

echo "Development workflow starting..."

# Run linting
echo "Checking code formatting..."
utah format src/*.shx --check

# Run tests
echo "Running tests..."
for test in tests/*.shx; do
  utah run "$test"
done

# Run main script
echo "Running main application..."
utah run src/main.shx --env development

echo "Development workflow completed!"
```

### Continuous Integration

```yaml
# .github/workflows/utah-ci.yml
name: Utah CI

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Install Utah
      run: |
        curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

    - name: Run unit tests
      run: |
        for test in tests/unit/*.shx; do
          echo "Running $test"
          utah run "$test"
        done

    - name: Run integration tests
      run: |
        for test in tests/integration/*.shx; do
          echo "Running $test"
          utah run "$test"
        done

    - name: Run smoke tests
      run: |
        utah run scripts/health-check.shx
```

## Troubleshooting

### Common Issues

**Issue:** Script runs but produces no output

```bash
utah run silent-script.shx
# No output
```

**Diagnosis:** Check if script has console.log statements
**Solution:** Add debug output to verify execution

**Issue:** Permission denied errors

```bash
utah run script.shx
# Permission denied accessing files
```

**Diagnosis:** Script may need elevated privileges
**Solution:** Check file permissions or run with sudo if needed

**Issue:** Script hangs indefinitely

```bash
utah run hanging-script.shx
# Never terminates
```

**Diagnosis:** Infinite loop or waiting for input
**Solution:** Use Ctrl+C to interrupt, check script logic

### Debug Techniques

1. **Add logging:**

```typescript
console.log("Debug: Reached checkpoint 1");
console.log(`Debug: Variable value is ${someVar}`);
```

1. **Use temporary files:**

```typescript
fs.writeFile("/tmp/debug.log", `Current state: ${JSON.stringify(data)}`);
```

1. **Check environment:**

```typescript
console.log(`PWD: ${PWD}`);
console.log(`USER: ${USER}`);
console.log(`PATH: ${PATH}`);
```

### Performance Debugging

```bash
# Time compilation vs execution
time utah compile script.shx  # Compilation time
time ./script.sh             # Execution time
time utah run script.shx     # Total time

# Memory usage
/usr/bin/time -v utah run script.shx
```

## Best Practices

### 1. Use for Development

```bash
# Good: Quick testing during development
utah run dev-test.shx

# Good: Interactive debugging
utah run debug-helper.shx --verbose

# Avoid: Production deployment
# utah run production-script.shx  # Use compile instead
```

### 2. Handle Arguments Properly

```typescript
// Good: Validate arguments
if (!args.has("--required-param")) {
  console.log("Error: --required-param is required");
  args.showHelp();
  exit(1);
}

// Good: Provide defaults
let timeout: number = args.has("--timeout") ? args.getNumber("--timeout") : 30;
```

### 3. Provide Clear Output

```typescript
// Good: Clear progress indication
console.log("Starting data processing...");
console.log("Processing file 1 of 5...");
console.log("✅ Processing completed successfully");

// Avoid: Silent operation
// processData(); // No feedback
```

### 4. Exit Gracefully

```typescript
// Good: Clean exit with status
if (errorOccurred) {
  console.log("❌ Operation failed: " + errorMessage);
  exit(1);
}

console.log("✅ Operation completed successfully");
exit(0);
```

The run command provides an excellent development experience for Utah scripts, enabling rapid iteration and testing without the overhead of managing compiled files.
