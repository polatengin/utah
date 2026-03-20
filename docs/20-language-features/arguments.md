---
layout: default
title: Argument Parsing
parent: Language Features
nav_order: 10
---

Utah provides built-in argument parsing for CLI scripts. When any `args.*` function is used, Utah automatically generates the necessary helper infrastructure in the compiled bash output.

## Defining Arguments

### args.define()

Defines a command-line argument with its flags, description, type, and optional default value.

```typescript
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");
args.define("--age", "", "Specify the user's age", "number", false, 25);
args.define("--admin", "", "Create with admin privileges", "boolean", false, false);
args.define("--config", "-c", "Path to config file", "string", true);
```

**Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `longFlag` | string | Long flag name (e.g., `"--name"`) |
| `shortFlag` | string | Short flag alias (e.g., `"-n"`), or `""` for none |
| `description` | string | Help text for the argument |
| `type` | string | Argument type: `"string"`, `"number"`, or `"boolean"` |
| `required` | boolean | Whether the argument is required (default: `false`) |
| `defaultValue` | any | Default value when not provided |

## Accessing Arguments

### args.get()

Returns the value of the specified argument. If the argument was not provided, returns the default value from `args.define()`.

```typescript
let userName: string = args.get("--name");
let port: number = args.get("--port");
```

### args.has()

Returns `true` if the specified argument was passed on the command line.

```typescript
if (args.has("--verbose")) {
  console.log("Verbose mode enabled");
}
```

### args.all()

Returns all command-line arguments as a single string.

```typescript
let allArgs: string = args.all();
console.log("Arguments: " + allArgs);
```

## Help Generation

### args.showHelp()

Displays auto-generated help text based on `args.define()` calls, including the script description set via `script.description()`, and exits.

```typescript
script.description("User Management Tool - Creates and manages user accounts");

args.define("--help", "-h", "Show this help message");
args.define("--version", "-v", "Show the application version");
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");
args.define("--age", "", "Specify the user's age", "number", false, 25);

if (args.has("--help")) {
  args.showHelp();
  exit(0);
}
```

Running `./script.sh --help` produces:

```
User Management Tool - Creates and manages user accounts

Usage: ./script.sh [OPTIONS]

Options:
  -h, --help                 Show this help message
  -v, --version              Show the application version
  -n, --name                 Specify the user's name (default: Anonymous)
      --age                  Specify the user's age (default: 25)
```

## Complete Example

```typescript
script.description("User Management Tool - Creates and manages user accounts");

args.define("--version", "-v", "Show the application version");
args.define("--help", "-h", "Show this help message");
args.define("--name", "-n", "Specify the user's name", "string", false, "Anonymous");
args.define("--age", "", "Specify the user's age", "number", false, 25);
args.define("--admin", "", "Create user with admin privileges", "boolean", false, false);

if (args.has("--help")) {
  args.showHelp();
  exit(0);
}

if (args.has("--version")) {
  console.log("User Management Tool v1.0.0");
  exit(0);
}

let userName: string = args.get("--name");
let userAge: number = args.get("--age");
let isAdmin: boolean = args.has("--admin");

console.log("Creating user: ${userName}, age: ${userAge}, admin: ${isAdmin}");
console.log("All arguments: " + args.all());
```

## Generated Bash

When `args.*` functions are detected, Utah auto-injects argument parsing infrastructure:

```bash
# Auto-generated arrays for argument metadata
__UTAH_ARG_NAMES=()
__UTAH_ARG_SHORT_NAMES=()
__UTAH_ARG_DESCRIPTIONS=()
__UTAH_ARG_TYPES=()
__UTAH_ARG_REQUIRED=()
__UTAH_ARG_DEFAULTS=()

# Each args.define() appends to the arrays
__UTAH_ARG_NAMES+=("--name")
__UTAH_ARG_SHORT_NAMES+=("-n")
__UTAH_ARG_DESCRIPTIONS+=("Specify the user's name")
__UTAH_ARG_TYPES+=("string")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("Anonymous")

# args.has() checks for flag presence
if [ "$(__utah_has_arg "--verbose" "$@" && echo "true" || echo "false")" = "true" ]; then
  echo "Verbose mode enabled"
fi

# args.get() retrieves value or default
userName=$(__utah_get_arg "--name" "$@")

# args.all() returns all arguments
echo "$(__utah_all_args "$@")"

# args.showHelp() prints formatted help
__utah_show_help "$@"
```

## Features

- **Auto-injection**: Parsing infrastructure is only added when `args.*` functions are used
- **Short and long flags**: Support for both `--name` and `-n` style arguments
- **Type support**: String, number, and boolean argument types
- **Default values**: Automatic fallback when arguments are not provided
- **Help generation**: Auto-formatted help text from `args.define()` metadata
- **Value formats**: Supports both `--flag value` and `--flag=value` syntax
