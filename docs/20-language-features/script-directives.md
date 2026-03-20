---
layout: default
title: Script Directives
parent: Language Features
nav_order: 16
---

Utah provides `script.*` directives that control script-level metadata and bash shell options. These are typically placed at the top of a `.shx` file.

## Metadata

### script.description()

Sets a description for the script. This description is displayed in auto-generated help output when used with `args.showHelp()`.

```typescript
script.description("File processing utility");
```

**Generated Bash:**

```bash
__UTAH_SCRIPT_DESCRIPTION="File processing utility"
```

### Example with Argument Parsing

```typescript
script.description("User Management Tool - Creates and manages user accounts");

args.define("--help", "-h", "Show this help message");
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");

if (args.has("--help")) {
  args.showHelp();
  exit(0);
}
```

Running `./script.sh --help` displays:

```
User Management Tool - Creates and manages user accounts

Usage: ./script.sh [OPTIONS]

Options:
  -h, --help                 Show this help message
  -n, --name                 Specify the user's name (default: Anonymous)
```

## Error Behavior

### script.exitOnError()

Causes the script to exit immediately if any command returns a non-zero exit code. Equivalent to `set -e` in bash.

```typescript
script.exitOnError();
console.log("Exit on error enabled");
```

**Generated Bash:**

```bash
set -e
echo "Exit on error enabled"
```

### script.continueOnError()

Allows the script to continue executing even when a command fails. This is the default bash behavior. Equivalent to `set +e`. Useful for reverting a previous `script.exitOnError()`.

```typescript
script.exitOnError();
// ... critical section ...
script.continueOnError();
// ... non-critical section ...
```

**Generated Bash:**

```bash
set -e
# ... critical section ...
set +e
# ... non-critical section ...
```

## Debugging

### script.enableDebug()

Enables debug mode, which prints each command to stderr before executing it. Equivalent to `set -x`.

```typescript
script.enableDebug();
console.log("Debug mode enabled");
let test: string = "hello";
console.log("Test variable: " + test);
```

**Generated Bash:**

```bash
set -x
echo "Debug mode enabled"
test="hello"
echo "Test variable: ${test}"
```

### script.disableDebug()

Disables debug mode. Equivalent to `set +x`.

```typescript
script.enableDebug();
// ... debug section ...
script.disableDebug();
// ... normal section ...
```

**Generated Bash:**

```bash
set -x
# ... debug section ...
set +x
# ... normal section ...
```

## Globbing

### script.enableGlobbing()

Enables filename globbing (pattern expansion). Equivalent to `set +f`. This is the default bash behavior, so this is typically used to re-enable globbing after disabling it.

```typescript
script.enableGlobbing();
console.log("Globbing enabled");
let pattern: string = "*.txt";
console.log("Pattern: " + pattern);
```

**Generated Bash:**

```bash
set +f
echo "Globbing enabled"
pattern="*.txt"
echo "Pattern: ${pattern}"
```

### script.disableGlobbing()

Disables filename globbing so that wildcard characters like `*` and `?` are treated as literal characters. Equivalent to `set -f`.

```typescript
script.disableGlobbing();
console.log("Globbing disabled");
let pattern: string = "*.txt";
console.log("Pattern: " + pattern);
```

**Generated Bash:**

```bash
set -f
echo "Globbing disabled"
pattern="*.txt"
echo "Pattern: ${pattern}"
```

## Summary

| Directive | Bash Equivalent | Description |
|---|---|---|
| `script.description(text)` | `__UTAH_SCRIPT_DESCRIPTION="text"` | Set script description for help output |
| `script.exitOnError()` | `set -e` | Exit on first error |
| `script.continueOnError()` | `set +e` | Continue past errors |
| `script.enableDebug()` | `set -x` | Print commands before execution |
| `script.disableDebug()` | `set +x` | Stop printing commands |
| `script.enableGlobbing()` | `set +f` | Enable filename pattern expansion |
| `script.disableGlobbing()` | `set -f` | Disable filename pattern expansion |

## Best Practices

- Place `script.description()` at the top of the file, before `args.define()` calls.
- Use `script.exitOnError()` in scripts where any failure should halt execution (e.g., deployment scripts).
- Wrap debug sections with `script.enableDebug()` / `script.disableDebug()` to limit verbose output.
- Use `script.disableGlobbing()` when working with strings that contain wildcard characters to prevent unintended expansion.
