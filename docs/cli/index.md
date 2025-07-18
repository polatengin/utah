---
layout: default
title: CLI Reference
nav_order: 5
has_children: true
---

Utah provides a comprehensive command-line interface for compiling, running, formatting, and managing Utah scripts. The CLI is built with .NET 9 and offers robust error handling and clear feedback.

## Quick Start

```bash
# Install Utah
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

# Compile a script
utah compile script.shx

# Run a script directly
utah run script.shx

# Format a single script
utah format script.shx

# Format all .shx files in project recursively
utah format --in-place
```

## Available Commands

| Command | Description | Usage |
|---------|-------------|-------|
| `compile` | Transpile .shx to .sh | `utah compile <file.shx> [-o <output.sh>]` |
| `run` | Compile and execute | `utah run <file.shx>` or `utah run -c <command>` |
| `format` | Format source code | `utah format [file.shx] [options]` |
| `lsp` | Language server | `utah lsp` |
| `version` | Show version info | `utah version` |

> When no file is specified, `utah format` automatically processes all `.shx` files recursively from the current directory.

## Command Details

### Compile Command

Transpiles Utah (.shx) source files into bash (.sh) scripts:

```bash
utah compile script.shx                # Creates script.sh
utah compile script.shx -o custom.sh   # Creates custom.sh
```

**Features:**

- Resolves import statements automatically
- Generates clean, readable bash code
- Preserves comments and structure
- Error reporting with line numbers

### Run Command

Compiles and immediately executes Utah scripts or commands:

```bash
# Run a .shx file
utah run script.shx

# Run a single command directly
utah run -c "console.log('Hello, World!')"
utah run --command "json.installDependencies()"

# More command examples
utah run -c "os.isInstalled('git')"
utah run --command "fs.exists('/path/to/file')"
```

**Features:**

- Execute .shx files directly
- Run single commands without creating files
- Temporary compilation (no .sh file created for direct commands)
- Real-time output streaming
- Error propagation from bash execution
- Automatic cleanup of temporary files

**Options:**

- `utah run <file.shx>`: Execute a .shx file
- `utah run -c <command>`: Execute a single command directly
- `utah run --command <command>`: Execute a single command directly (long form)

### Format Command

Formats Utah source code according to EditorConfig rules:

```bash
utah format                               # Format all .shx files recursively
utah format --in-place                    # Format all files in place
utah format --check                       # Check all files formatting
utah format script.shx                    # Creates script.formatted.shx
utah format script.shx -o clean.shx       # Creates clean.shx
utah format script.shx --output clean.shx # Creates clean.shx (long form)
utah format script.shx --in-place         # Overwrites script.shx
utah format script.shx --check            # Exit 1 if not formatted
```

**Options:**

- No file specified: Format all `.shx` files recursively
- `-o, --output <file>`: Specify output file (single file only)
- `--in-place`: Overwrite original file(s)
- `--check`: Check formatting without modifying

### Language Server

Provides IDE integration for VS Code and other editors:

```bash
utah lsp
```

**Features:**

- Syntax highlighting
- Error diagnostics
- Code completion
- Symbol navigation

### Version Information

Shows detailed version and runtime information:

```bash
utah version
utah --version
utah -v
```

**Output includes:**

- Utah version number
- .NET runtime version
- Operating system details
- Architecture information

## Error Handling

Utah CLI provides clear error messages and appropriate exit codes:

```bash
# Compilation errors
utah compile broken.shx
# ❌ Compilation failed: Syntax error at line 5: Expected ';'

# File not found
utah run missing.shx
# File not found: missing.shx

# Format check failure
utah format messy.shx --check
# ❌ File is not formatted: messy.shx
# Exit code: 1
```

## Integration Examples

### Build Scripts

```bash
#!/bin/bash
# Build script using Utah CLI

echo "Building Utah scripts..."

for file in src/*.shx; do
  if utah compile "$file" -o "dist/$(basename "$file" .shx).sh"; then
    echo "✅ Compiled: $file"
  else
    echo "❌ Failed: $file"
    exit 1
  fi
done

echo "Build completed successfully"
```

### CI/CD Pipeline

```yaml
# GitHub Actions example
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

      - name: Check formatting
        run: |
          for file in src/*.shx; do
            utah format "$file" --check
          done

      - name: Compile all scripts
        run: |
          for file in src/*.shx; do
            utah compile "$file"
          done

      - name: Run tests
        run: |
          for file in tests/*.shx; do
            utah run "$file"
          done
```

### VS Code Integration

Utah includes a VS Code extension that uses the language server:

```json
{
  "recommendations": [
    "utah-lang.utah-language-support"
  ]
}
```

**Features provided:**

- Syntax highlighting for .shx files
- Real-time error checking
- Code completion for built-in functions
- Format on save support

## Advanced Usage

### Import Resolution

Utah automatically resolves import statements during compilation:

```typescript
// main.shx
import "utils/helpers.shx";
import "config/settings.shx";

// Code using imported functions
```

```bash
utah compile main.shx
# Automatically includes helpers.shx and settings.shx
```

### Batch Operations

```bash
# Compile all .shx files in a directory
find . -name "*.shx" -exec utah compile {} \;

# Format all files recursively (recommended)
utah format --in-place

# Check all files are formatted recursively (recommended)
utah format --check

# Legacy: Format with find (still works)
find . -name "*.shx" -exec utah format {} --in-place \;
```

**Note**: The new `utah format` without a filename automatically processes all `.shx` files recursively, providing better progress reporting and error handling than the legacy `find` approach.

### Output Redirection

```bash
# Capture compilation output
utah compile script.shx 2> errors.log

# Run with output logging
utah run script.shx > output.log 2>&1
```

## Troubleshooting

### Common Issues

- **Problem: Command not found**

```bash
utah: command not found
```

**Solution:** Ensure Utah is installed and in your PATH

```bash
# Reinstall Utah
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
```

- **Problem: Compilation fails with syntax error**

```text
❌ Compilation failed: Syntax error at line 5: Expected ';'
```

**Solution:** Check syntax around the reported line number

- **Problem: Permission denied when running**

```text
❌ Unexpected error: Permission denied
```

**Solution:** Check file permissions and ensure bash is executable

```bash
chmod +x generated_script.sh
```

- **Problem: Import file not found**

```text
❌ Compilation failed: Import file not found: utils/helper.shx
```

**Solution:** Verify import paths are relative to the main file

### Debug Mode

Enable verbose output for troubleshooting:

```bash
# Set environment variable for debug output
export UTAH_DEBUG=1
utah compile script.shx
```

### Getting Help

```bash
# Show usage information
utah

# Show command-specific help
utah compile
utah format
utah run
```

## Performance Considerations

### Compilation Speed

- **Small files (< 100 lines):** < 100ms
- **Medium files (100-1000 lines):** < 500ms
- **Large files (1000+ lines):** < 2s

### Memory Usage

- **Typical usage:** 10-50MB RAM
- **Large projects:** 100-200MB RAM
- **Language server:** 50-100MB RAM

### Optimization Tips

1. **Use specific imports:** Only import what you need
2. **Avoid deep nesting:** Keep import chains shallow
3. **Batch operations:** Compile multiple files together when possible
4. **Cache results:** Compiled .sh files don't need recompilation unless .shx changes

## Related Documentation

- **[Installation Guide](../getting-started/installation.md)** - Setup and installation
- **[Language Features](../language-features/)** - Utah syntax and features
- **[Functions Reference](../functions/)** - Built-in function documentation
- **[VS Code Extension](vscode-extension.md)** - IDE integration details
