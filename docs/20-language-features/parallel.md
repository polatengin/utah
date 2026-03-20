---
layout: default
title: Parallel Execution
parent: Language Features
nav_order: 17
---

Utah supports running function calls concurrently using the `parallel` keyword. This compiles to bash background execution with `&`.

## Basic Syntax

Prefix a function call with `parallel` to run it in the background:

```typescript
parallel functionName(args);
```

Use `wait` (via a raw bash expression) to block until all background jobs complete:

```typescript
let _ = "$(wait)";
```

## Examples

### Running Functions in Parallel

```typescript
function slowEcho(msg: string): void {
  console.log("Start: ${msg}");
  let _ = "$(sleep 1)";
  console.log("End: ${msg}");
}

console.log("Main start");
parallel slowEcho("A");
parallel slowEcho("B");
console.log("Main end");
let _ = "$(wait)";
console.log("All done");
```

**Generated Bash:**

```bash
slowEcho() {
  local msg="$1"
  echo "Start: ${msg}"
  _="$(sleep 1)"
  echo "End: ${msg}"
}
echo "Main start"
slowEcho "A" &
slowEcho "B" &
echo "Main end"
_="$(wait)"
echo "All done"
```

Both `slowEcho("A")` and `slowEcho("B")` run simultaneously. The main script continues to print `"Main end"` immediately, then `wait` blocks until both background jobs finish.

### File Processing Pipeline

```typescript
function processFile(path: string): void {
  console.log("Processing: ${path}");
  let _ = "$(sleep 2)";
  console.log("Done: ${path}");
}

parallel processFile("data1.csv");
parallel processFile("data2.csv");
parallel processFile("data3.csv");
let _ = "$(wait)";
console.log("All files processed");
```

### Parallel with Process Management

For more control over individual background tasks, combine `parallel` with `process.start()` and `process.waitForExit()`:

```typescript
// Simple parallel with the parallel keyword
parallel buildFrontend();
parallel buildBackend();
let _ = "$(wait)";

// Advanced parallel with process management
let pid1: number = process.start("npm run build:frontend");
let pid2: number = process.start("npm run build:backend");

let result1: number = process.waitForExit(pid1, 300000);
let result2: number = process.waitForExit(pid2, 300000);

if (result1 == 0 && result2 == 0) {
  console.log("Both builds succeeded");
}
```

## Generated Bash

The `parallel` keyword appends `&` to the compiled function call, sending it to the background:

```bash
# parallel slowEcho("A");
slowEcho "A" &

# parallel slowEcho("B");
slowEcho "B" &

# let _ = "$(wait)";
_="$(wait)"
```

## Notes

- `parallel` works with any user-defined function.
- Background jobs share the same stdout/stderr as the parent script, so output from parallel calls may interleave.
- Use `wait` to synchronize before continuing with code that depends on the parallel results.
- For finer control (exit codes, timeouts, process monitoring), see [System Functions](system.md) — specifically `process.start()`, `process.waitForExit()`, and `process.kill()`.
