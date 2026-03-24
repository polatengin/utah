---
layout: default
title: Debug Command
parent: CLI Reference
nav_order: 2
---

The `utah debug` command compiles Utah (.shx) source files into bash (.sh) scripts with embedded source-map comments. Each statement in the generated bash is preceded by a comment showing which `.shx` line produced it, making it easy to trace runtime errors back to the original source.

## Basic Usage

```bash
utah debug <file.shx> [-o, --output <output.sh>]
```

## Examples

### Simple Debug Compilation

```bash
utah debug script.shx
```

This creates `script.sh` with source-map comments in the same directory.

### Custom Output File

```bash
utah debug script.shx -o /path/to/debug-output.sh
utah debug script.shx --output /path/to/debug-output.sh
```

## How It Works

The `debug` command performs the same transpilation as `compile`, but injects a `# [shx:<line>]` comment before every compiled statement. The comment includes the 1-based line number and the original `.shx` source text.

### Example

**Input (script.shx):**

```typescript
let name: string = "Utah";
console.log("Hello");
if (name === "Utah") {
  console.log("found it");
}
```

**Output with `utah debug` (script.sh):**

```bash
#!/bin/bash

# [shx:1] let name: string = "Utah";
name="Utah"
# [shx:2] console.log("Hello");
echo "Hello"
# [shx:3] if (name === "Utah") {
if [ "${name}" = "Utah" ]; then
  # [shx:4] console.log("found it");
  echo "found it"
fi
```

**Compare with regular `utah compile` (script.sh):**

```bash
#!/bin/bash

name="Utah"
echo "Hello"
if [ "${name}" = "Utah" ]; then
  echo "found it"
fi
```

### Source-Map Comment Format

Each source-map comment follows this format:

```text
# [shx:<line_number>] <original_source_line>
```

- **`<line_number>`**: 1-based line number from the original `.shx` file
- **`<original_source_line>`**: The trimmed source text of that line

Comments appear for all compiled statements, including those nested inside control structures, functions, and loops.

## When to Use Debug vs Compile

| Scenario | Command |
|----------|---------|
| Production deployment | `utah compile` |
| Troubleshooting runtime errors | `utah debug` |
| Reviewing generated bash logic | `utah debug` |
| CI/CD pipelines | `utah compile` |
| Learning how Utah translates constructs | `utah debug` |

## Troubleshooting with Debug Output

When a bash script fails at runtime, the source-map comments help you locate the problem:

```text
$ bash script.sh
script.sh: line 14: some_command: command not found
```

Open `script.sh` and look at line 14. The nearest `# [shx:N]` comment above it tells you which `.shx` line to fix:

```bash
# [shx:7] let result: string = someCommand();
result=$(some_command)    # ← line 14, the error is here
```

Now you know the issue is on line 7 of your `.shx` source.

### Tips

1. **Use `debug` during development**, switch to `compile` for production
2. **Source-map comments are valid bash** — debug-compiled scripts run identically to regular compiled scripts
3. **Combine with `bash -x`** for even deeper tracing:

```bash
utah debug script.shx -o debug-script.sh
bash -x debug-script.sh
```

This gives you both Utah source mapping and bash execution tracing.

## Integration Examples

### Debug Build Script

```bash
#!/bin/bash
# debug-build.sh - Compile with source maps for development

for file in src/*.shx; do
  basename=$(basename "$file" .shx)
  utah debug "$file" -o "debug/$basename.sh"
  echo "✅ Debug compiled: $basename.sh"
done
```

### Makefile Integration

```makefile
SOURCES := $(wildcard src/*.shx)
DEBUG_TARGETS := $(SOURCES:src/%.shx=debug/%.sh)

debug: $(DEBUG_TARGETS)

debug/%.sh: src/%.shx
	utah debug $< -o $@

.PHONY: debug
```

## Related Documentation

- **[Compile Command](compile.md)** — Standard compilation without source maps
- **[Run Command](run.md)** — Compile and execute scripts
- **[Script Directives](../20-language-features/script-directives.md)** — `script.enableDebug()` for bash-level `set -x` tracing
