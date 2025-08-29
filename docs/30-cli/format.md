---
layout: default
title: Format Command
parent: CLI Reference
nav_order: 3
---

The `utah format` command formats Utah (.shx) source code according to configurable style rules. It helps maintain consistent code style across teams and projects.

## Basic Usage

```bash
utah format [file.shx] [options]
```

## Options

| Option | Description | Example |
|--------|-------------|---------|
| (no file) | Format all .shx files recursively | `utah format` |
| `-o, --output <file>` | Output to specific file (single file only) | `utah format script.shx -o clean.shx` |
| `--in-place` | Overwrite original file(s) | `utah format script.shx --in-place` |
| `--check` | Check formatting only | `utah format script.shx --check` |

## Examples

### Recursive Formatting

```bash
utah format
```

Finds and formats all `.shx` files recursively from the current directory, creating `.formatted.shx` files for those that need formatting.

```bash
utah format --in-place
```

Formats all `.shx` files recursively in place (overwrites originals).

```bash
utah format --check
```

Checks if all `.shx` files recursively are properly formatted. Exits with code 1 if any files need formatting.

### Basic Formatting

```bash
utah format script.shx
```

Creates `script.formatted.shx` with proper formatting.

### In-Place Formatting

```bash
utah format script.shx --in-place
```

Overwrites `script.shx` with formatted version.

### Format Check

```bash
utah format script.shx --check
```

Exits with code 1 if file needs formatting, 0 if already formatted.

### Custom Output

```bash
utah format messy-script.shx -o clean-script.shx
```

Creates formatted version with custom name using short option.

```bash
utah format messy-script.shx --output clean-script.shx
```

Creates formatted version with custom name using long option.

## Formatting Rules

### Indentation

Utah uses 2-space indentation by default:

**Before:**

```typescript
function process(data:string):void{
if(data.length>0){
for(let i:number=0;i<data.length;i++){
console.log(data[i]);
}
}
}
```

**After:**

```typescript
function process(data: string): void {
  if (data.length > 0) {
    for (let i: number = 0; i < data.length; i++) {
      console.log(data[i]);
    }
  }
}
```

### Spacing

Proper spacing around operators and keywords:

**Before:**

```typescript
let name:string="Alice";
let count:number=42+8;
if(count>50){
console.log("High count");
}
```

**After:**

```typescript
let name: string = "Alice";
let count: number = 42 + 8;
if (count > 50) {
  console.log("High count");
}
```

### Line Breaking

Consistent line breaks and empty lines:

**Before:**

```typescript
import "utils.shx";import "config.shx";
function init():void{
console.log("Starting");
}
function cleanup():void{
console.log("Cleaning up");
}
```

**After:**

```typescript
import "utils.shx";
import "config.shx";

function init(): void {
  console.log("Starting");
}

function cleanup(): void {
  console.log("Cleaning up");
}
```

### Bracket Placement

Consistent bracket placement:

**Before:**

```typescript
if (condition)
{
  doSomething();
}
else
{
  doSomethingElse();
}
```

**After:**

```typescript
if (condition) {
  doSomething();
} else {
  doSomethingElse();
}
```

## Configuration

### EditorConfig Support

Utah respects `.editorconfig` files in your project:

```ini
# .editorconfig
root = true

[*.shx]
indent_style = space
indent_size = 2
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
max_line_length = 100
```

### Default Settings

When no `.editorconfig` is found, Utah uses these defaults:

- **Indent:** 2 spaces
- **Line endings:** LF (Unix)
- **Charset:** UTF-8
- **Max line length:** 100 characters
- **Trim whitespace:** Yes
- **Final newline:** Yes

## Integration Examples

### Pre-commit Hook

Add formatting check to Git pre-commit hooks:

```bash
#!/bin/sh
# .git/hooks/pre-commit

echo "Checking Utah file formatting..."

# Get list of staged .shx files
staged_files=$(git diff --cached --name-only --diff-filter=ACM | grep '\.shx$')

if [ -z "$staged_files" ]; then
  exit 0
fi

# Check formatting
needs_formatting=false
for file in $staged_files; do
  if ! utah format "$file" --check; then
    echo "❌ $file needs formatting"
    needs_formatting=true
  fi
done

if [ "$needs_formatting" = true ]; then
  echo ""
  echo "Some files need formatting. Run:"
  echo "  utah format <file> --in-place"
  echo "or:"
  echo "  make format"
  exit 1
fi

echo "✅ All Utah files are properly formatted"
```

### Makefile Integration

```makefile
# Format all .shx files recursively (recommended)
format:
  utah format --in-place

# Check formatting without changing files
format-check:
  @echo "Checking formatting..."
  @utah format --check

# Format and show summary
format-summary:
  @echo "Formatting all Utah files..."
  @utah format --in-place

# Format specific directory
format-src:
  cd src && utah format --in-place

.PHONY: format format-check format-summary format-src
```

**Benefits of the recursive approach:**

- Simpler commands (no `find` needed)
- Better progress reporting
- Consistent error handling
- Works from any directory level

### VS Code Integration

Utah's VS Code extension automatically formats on save:

```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "utah-lang.utah-language-support",
  "[shx]": {
    "editor.formatOnSave": true,
    "editor.insertSpaces": true,
    "editor.tabSize": 2
  }
}
```

### GitHub Actions

```yaml
name: Format Check

on: [push, pull_request]

jobs:
  format:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Install Utah
      run: |
        curl -sL https://utahshx.com/install.sh | sudo bash

    - name: Check formatting
      run: |
        echo "Checking Utah file formatting..."
        utah format --check
```

**Alternative GitHub Actions with Auto-formatting:**

```yaml
name: Auto Format

on: [push]

jobs:
  format:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Install Utah
      run: |
        curl -sL https://utahshx.com/install.sh | sudo bash

    - name: Format files
      run: utah format --in-place

    - name: Commit changes
      run: |
        git config --local user.email "action@github.com"
        git config --local user.name "GitHub Action"
        git add -A
        git diff --staged --quiet || git commit -m "Auto-format Utah files"
        git push
```

## Advanced Usage

### Recursive Formatting (Recommended)

Utah's built-in recursive formatting is the simplest way to format entire projects:

```bash
# Format all .shx files recursively from current directory
utah format

# Format all files in place (recommended for development)
utah format --in-place

# Check all files are formatted (great for CI/CD)
utah format --check
```

**Output Examples:**

```bash
$ utah format
Found 5 .shx file(s) to format:
  src/main.shx... ✅ formatted -> src/main.formatted.shx
  src/utils.shx... ✅ already formatted
  tests/test.shx... ✅ formatted -> tests/test.formatted.shx
  lib/helper.shx... ✅ already formatted
  scripts/build.shx... ✅ formatted -> scripts/build.formatted.shx

Summary: 3 formatted, 2 already formatted, 0 errors
```

```bash
$ utah format --check
Found 5 .shx file(s) to format:
  src/main.shx... ❌ not formatted
  src/utils.shx... ✅ already formatted
  tests/test.shx... ✅ already formatted
  lib/helper.shx... ✅ already formatted
  scripts/build.shx... ❌ not formatted

Summary: 3 properly formatted, 2 need formatting, 0 errors
Run 'utah format --in-place' to format all files.
```

### Conditional Formatting

Only format files that need it:

```bash
#!/bin/bash
# smart-format.sh

for file in src/*.shx; do
  if ! utah format "$file" --check 2>/dev/null; then
    echo "Formatting $file..."
    utah format "$file" --in-place
  else
    echo "✅ $file already formatted"
  fi
done
```

### Diff Preview

See what would change before formatting:

```bash
#!/bin/bash
# format-diff.sh

file="$1"
temp_file=$(mktemp)

utah format "$file" -o "$temp_file"
diff -u "$file" "$temp_file"
rm "$temp_file"
```

## Performance

### Formatting Speed

| File Size | Formatting Time | Memory Usage |
|-----------|----------------|--------------|
| < 100 lines | < 50ms | 5MB |
| 100-500 lines | < 100ms | 10MB |
| 500-1000 lines | < 200ms | 15MB |
| 1000+ lines | < 500ms | 25MB |

### Optimization Tips

1. **Batch operations:** Format multiple files together
2. **Use check mode:** Avoid unnecessary formatting
3. **Cache results:** Only format changed files

```bash
# Only format files newer than their formatted versions
for file in src/*.shx; do
  formatted="${file%.shx}.formatted.shx"
  if [ "$file" -nt "$formatted" ]; then
    utah format "$file" -o "$formatted"
  fi
done
```

## Error Handling

### Common Errors

**Syntax Error:**

```bash
utah format broken.shx
```

```text
❌ Formatting failed: Syntax error at line 5: Expected ';'
```

**File Not Found:**

```bash
utah format missing.shx
```

```text
File not found: missing.shx
```

**Permission Denied:**

```bash
utah format readonly.shx --in-place
```

```text
❌ Formatting failed: Permission denied writing to readonly.shx
```

### Recovery

If formatting fails, the original file remains unchanged:

```bash
# Safe formatting with backup
cp important.shx important.shx.backup
if utah format important.shx --in-place; then
  echo "✅ Formatted successfully"
  rm important.shx.backup
else
  echo "❌ Formatting failed, restoring backup"
  mv important.shx.backup important.shx
fi
```

## Best Practices

### 1. Use in CI/CD

```bash
# Good: Enforce formatting in CI
utah format src/*.shx --check

# Good: Auto-format in development
utah format src/*.shx --in-place
```

### 2. Configure Team Standards

```ini
# .editorconfig - Team formatting standards
[*.shx]
indent_size = 2
max_line_length = 100
insert_final_newline = true
```

### 3. Format Before Committing

```bash
# Good: Format before committing
utah format changed-file.shx --in-place
git add changed-file.shx
git commit -m "Update script"

# Consider: Auto-format on save in editor
```

### 4. Document Formatting Rules

```markdown
# Team Coding Standards

## Utah (.shx) Files

- Use `utah format` before committing
- Follow 2-space indentation
- Maximum line length: 100 characters
- Always include final newline

## Setup

Add to your editor:
- Format on save: enabled
- Default formatter: Utah Language Support
```

The format command ensures consistent code style across Utah projects, making code more readable and maintainable for teams and individual developers.
