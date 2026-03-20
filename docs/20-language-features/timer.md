---
layout: default
title: Timer
parent: Language Features
nav_order: 15
---

Utah provides timer functions for measuring execution time in milliseconds, useful for benchmarking and performance monitoring.

## Functions

### timer.start()

Starts the timer by recording the current timestamp in milliseconds.

```typescript
timer.start();
```

### timer.stop()

Stops the timer and returns the elapsed time in milliseconds since `timer.start()` was called.

```typescript
timer.start();

// ... do some work ...

let elapsed: number = timer.stop();
console.log("Elapsed: ${elapsed} ms");
```

### timer.current()

Returns the elapsed time in milliseconds since `timer.start()` was called, without stopping the timer. Useful for intermediate checkpoints.

```typescript
timer.start();

// ... do some work ...
let soFar: number = timer.current();
console.log("So far: ${soFar} ms");

// ... do more work ...
let total: number = timer.stop();
console.log("Total: ${total} ms");
```

## Examples

### Benchmarking a Loop

```typescript
timer.start();
console.log("Timer started");

let i: number = 0;
for (let j: number = 0; j < 1000000; j++) {
  i++;
}

const elapsed: number = timer.stop();
console.log("Timer elapsed: ${elapsed} ms");
```

### Measuring Multiple Phases

```typescript
timer.start();

// Phase 1
for (let i: number = 0; i < 100000; i++) {
  // work
}
let phase1: number = timer.current();
console.log("Phase 1: ${phase1} ms");

// Phase 2
for (let i: number = 0; i < 200000; i++) {
  // more work
}
let phase2: number = timer.current();
console.log("Phase 1+2: ${phase2} ms");

let total: number = timer.stop();
console.log("Total: ${total} ms");
```

### Timing a Function Call

```typescript
function processData(): void {
  // Expensive operation
  let result: string = "$(find / -name '*.log' 2>/dev/null | head -10)";
  console.log(result);
}

timer.start();
processData();
let elapsed: number = timer.stop();
console.log("processData took ${elapsed} ms");
```

## Generated Bash

```bash
# timer.start() — records epoch in milliseconds
_utah_timer_start=$(date +%s%3N)

# timer.stop() — computes and returns elapsed ms
_utah_timer_end=$(date +%s%3N)
elapsed=$((_utah_timer_end - _utah_timer_start))

# timer.current() — returns elapsed ms without stopping
soFar=$(( $(date +%s%3N) - _utah_timer_start ))
```

## Notes

- Timer resolution is milliseconds (`date +%s%3N`).
- Only one timer is active at a time; calling `timer.start()` again resets the counter.
- `timer.current()` can be called multiple times between `start()` and `stop()`.
- `timer.stop()` returns the value but also marks the end of the timing window.
