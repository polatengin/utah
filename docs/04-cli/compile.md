---
layout: default
title: Compile Command
parent: CLI Reference
nav_order: 1
---

The `utah compile` command transpiles Utah (.shx) source files into bash (.sh) scripts. This is the core functionality of Utah, converting TypeScript-like syntax into clean, executable bash code.

## Basic Usage

```bash
utah compile <file.shx> [-o, --output <output.sh>]
```

## Examples

### Simple Compilation

```bash
utah compile script.shx
```

This creates `script.sh` in the same directory.

### Custom Output File

```bash
utah compile script.shx -o /path/to/custom.sh
utah compile script.shx --output /path/to/custom.sh
```

Both commands create the specified output file. You can use either `-o` or `--output`.

### Batch Compilation

```bash
# Compile all .shx files in current directory
for file in *.shx; do
  utah compile "$file"
done
```

## How It Works

### 1. Import Resolution

Utah automatically resolves and includes imported files:

**main.shx:**

```typescript
import "utils/helpers.shx";
import "config/database.shx";

let connectionString: string = getDatabaseUrl();
```

**utils/helpers.shx:**

```typescript
function getDatabaseUrl(): string {
  return "localhost:5432";
}
```

When compiling `main.shx`, Utah:

1. Reads the main file
2. Finds import statements
3. Recursively loads imported files
4. Combines everything into a single AST
5. Compiles to bash

### 2. AST Generation

Utah parses the source into an Abstract Syntax Tree:

- Variable declarations become bash variable assignments
- Function declarations become bash functions
- Control structures become bash equivalents
- Type information is used for validation but not output

### 3. Bash Code Generation

The compiler generates clean, readable bash:

**Input (.shx):**

```typescript
let name: string = "Utah";
let count: number = 42;

function greet(user: string): void {
  console.log("Hello, ${user}!");
}

greet(name);
```

**Output (.sh):**

```bash
#!/bin/bash
name="Utah"
count=42

greet() {
  local user="$1"
  echo "Hello, ${user}!"
}

greet "${name}"
```

## Error Handling

### Syntax Errors

Utah provides detailed error messages with line numbers:

```bash
utah compile broken.shx
```

```text
❌ Compilation failed: Syntax error at line 5: Expected ';' after variable declaration
```

### Import Errors

```bash
utah compile main.shx
```

```text
❌ Compilation failed: Import file not found: utils/missing.shx
```

### Type Errors

```bash
utah compile typed.shx
```

```text
❌ Compilation failed: Type mismatch at line 8: Cannot assign number to string variable
```

## Advanced Features

### Circular Import Detection

Utah detects and prevents circular imports:

**file1.shx:**

```typescript
import "file2.shx";
```

**file2.shx:**

```typescript
import "file1.shx";  // Circular import
```

```bash
utah compile file1.shx
```

```text
❌ Compilation failed: Circular import detected: file1.shx -> file2.shx -> file1.shx
```

### Relative Import Paths

Import paths are resolved relative to the importing file:

```text
project/
├── main.shx
├── utils/
│   ├── helpers.shx
│   └── math/
│       └── calculator.shx
```

**main.shx:**

```typescript
import "utils/helpers.shx";
```

**utils/helpers.shx:**

```typescript
import "math/calculator.shx";  // Relative to utils/
```

### Generated Code Structure

The compiled bash always includes:

1. **Shebang:** `#!/bin/bash`
2. **Error handling:** `set -e` when try/catch is used
3. **Variable declarations:** With proper scoping
4. **Function definitions:** Before their usage
5. **Main execution:** Script logic

## Performance

### Compilation Speed

| File Size | Typical Time | Imports | Memory Usage |
|-----------|--------------|---------|--------------|
| < 100 lines | < 100ms | 0-5 | 10MB |
| 100-500 lines | < 300ms | 5-20 | 25MB |
| 500-1000 lines | < 500ms | 20-50 | 50MB |
| 1000+ lines | < 2s | 50+ | 100MB |

### Optimization Tips

1. **Minimize imports:** Only import what you need
2. **Avoid deep nesting:** Keep import chains shallow
3. **Use specific paths:** Absolute paths are faster than relative
4. **Cache results:** Only recompile when source changes

## Integration Examples

### Makefile Integration

```makefile
# Makefile
SOURCES := $(wildcard src/*.shx)
TARGETS := $(SOURCES:src/%.shx=dist/%.sh)

all: $(TARGETS)

dist/%.sh: src/%.shx
  utah compile $< -o $@

clean:
  rm -f dist/*.sh

.PHONY: all clean
```

### Build Script

```bash
#!/bin/bash
# build.sh - Build all Utah scripts

set -e

echo "Building Utah scripts..."

# Create output directory
mkdir -p dist

# Compile all source files
for file in src/*.shx; do
  if [ -f "$file" ]; then
    basename=$(basename "$file" .shx)
    echo "Compiling $file..."

    if utah compile "$file" -o "dist/$basename.sh"; then
      echo "✅ $basename.sh"
      chmod +x "dist/$basename.sh"
    else
      echo "❌ Failed to compile $file"
      exit 1
    fi
  fi
done

echo "Build completed successfully!"
echo "Compiled scripts are in the dist/ directory"
```

### Watch Script

```bash
#!/bin/bash
# watch.sh - Auto-compile on file changes

if ! command -v inotifywait >/dev/null; then
  echo "Please install inotify-tools: sudo apt install inotify-tools"
  exit 1
fi

echo "Watching for changes in src/*.shx..."

while inotifywait -e modify,create src/*.shx; do
  echo "Changes detected, recompiling..."
  ./build.sh
done
```

## Troubleshooting

### Common Issues

**Issue:** Compilation hangs
**Cause:** Circular imports or very large files
**Solution:** Check import structure, break cycles

**Issue:** Generated bash has syntax errors
**Cause:** Utah compiler bug or unsupported syntax
**Solution:** Report issue with minimal reproduction case

**Issue:** Import not found
**Cause:** Incorrect path or missing file
**Solution:** Verify paths are relative to importing file

### Debug Output

Enable verbose compilation output:

```bash
export UTAH_DEBUG=1
utah compile script.shx
```

This shows:

- File resolution steps
- Import processing
- AST generation details
- Compilation phases

### Validation

Verify compiled output:

```bash
# Compile and check syntax
utah compile script.shx
bash -n script.sh  # Check syntax without running

# Compile and test
utah compile script.shx
bash script.sh     # Run the compiled script
```

## Best Practices

### 1. Organize Imports

```typescript
// Good: Group imports logically
import "config/database.shx";
import "config/logging.shx";
import "utils/string-helpers.shx";
import "utils/file-helpers.shx";

// Avoid: Mixed imports
import "database.shx";
import "string-helpers.shx";
import "logging.shx";
```

### 2. Use Descriptive Output Names

```bash
# Good: Descriptive names
utah compile backup-script.shx -o bin/backup-database.sh
utah compile deploy.shx --output scripts/deploy-production.sh

# Avoid: Generic names
utah compile backup-script.shx -o script.sh
utah compile deploy.shx --output output.sh
```

### 3. Validate After Compilation

```bash
utah compile script.shx
bash -n script.sh  # Syntax check
shellcheck script.sh  # Static analysis (if available)
```

### 4. Handle Errors Gracefully

```bash
if utah compile script.shx; then
  echo "Compilation successful"
  chmod +x script.sh
else
  echo "Compilation failed"
  exit 1
fi
```

The compile command is the foundation of Utah's functionality, providing robust transpilation from TypeScript-like syntax to efficient bash scripts.
