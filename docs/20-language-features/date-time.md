---
layout: default
title: Date & Time
parent: Language Features
nav_order: 18
---

Utah provides a `date.*` namespace for working with dates, times, and Unix timestamps using the system `date` command.

## Getting the Current Time

### Unix Timestamp (seconds)

```typescript
let ts: number = date.now()
console.log("Current timestamp: ${ts}")
```

Compiles to:

```bash
ts=$(date +%s)
echo "Current timestamp: ${ts}"
```

### Unix Timestamp (milliseconds)

```typescript
let ms: number = date.nowMillis()
console.log("Current time in ms: ${ms}")
```

Compiles to:

```bash
ms=$(date +%s%3N)
echo "Current time in ms: ${ms}"
```

## Formatting Dates

Use `date.format()` to convert timestamps into human-readable strings.

```typescript
// Format current time with default format
let now: string = date.format()

// Format a specific timestamp with default format
let ts: number = date.now()
let formatted: string = date.format(ts)

// Format with a custom format string
let custom: string = date.format(ts, "%Y-%m-%d")
```

### Common Format Specifiers

| Specifier | Description       | Example    |
|-----------|-------------------|------------|
| `%Y`      | 4-digit year      | 2024       |
| `%m`      | Month (01-12)     | 03         |
| `%d`      | Day (01-31)       | 15         |
| `%H`      | Hour (00-23)      | 14         |
| `%M`      | Minute (00-59)    | 30         |
| `%S`      | Second (00-59)    | 45         |
| `%A`      | Day name          | Monday     |
| `%B`      | Month name        | March      |
| `%s`      | Unix timestamp    | 1710000000 |

## Parsing Date Strings

Convert a date string into a Unix timestamp with `date.parse()`.

```typescript
let ts: number = date.parse("2024-01-15")
console.log("Timestamp: ${ts}")

// With a custom output format
let formatted: string = date.parse("2024-01-15", "%Y-%m-%d %H:%M:%S")
```

## Calculating Differences

Use `date.diff()` to find the difference between two timestamps.

```typescript
let start: number = date.parse("2024-01-01")
let end: number = date.parse("2024-01-31")

// Difference in seconds (default)
let diffSec: number = date.diff(end, start)

// Difference in specific units
let diffDays: number = date.diff(end, start, "days")
let diffHours: number = date.diff(end, start, "hours")
let diffMinutes: number = date.diff(end, start, "minutes")
```

### Supported Units

| Unit        | Description              |
|-------------|--------------------------|
| `"seconds"` | Difference in seconds    |
| `"minutes"` | Difference in minutes    |
| `"hours"`   | Difference in hours      |
| `"days"`    | Difference in days       |

## Timestamp Arithmetic

### Adding Time

```typescript
let now: number = date.now()

// Add 7 days
let nextWeek: number = date.add(now, 7, "days")

// Add 2 hours
let later: number = date.add(now, 2, "hours")

// Add 30 minutes
let soon: number = date.add(now, 30, "minutes")
```

### Subtracting Time

```typescript
let now: number = date.now()

// Subtract 1 day
let yesterday: number = date.subtract(now, 1, "days")

// Subtract 3 hours
let earlier: number = date.subtract(now, 3, "hours")
```

## Day of the Week

Get the name of the day for the current time or a specific timestamp.

```typescript
// Current day
let today: string = date.dayOfWeek()
console.log("Today is: ${today}")

// Day for a specific timestamp
let ts: number = date.parse("2024-01-15")
let day: string = date.dayOfWeek(ts)
console.log("That day was: ${day}")
```

## Complete Example

```typescript
// Measure script execution time
let startTime: number = date.now()
let startFormatted: string = date.format(startTime)
console.log("Script started at: ${startFormatted}")

// Do some work...
console.log("Processing...")

let endTime: number = date.now()
let elapsed: number = date.diff(endTime, startTime)
console.log("Elapsed: ${elapsed} seconds")

// Schedule a future event
let deadline: number = date.add(startTime, 24, "hours")
let deadlineStr: string = date.format(deadline, "%Y-%m-%d %H:%M")
console.log("Deadline: ${deadlineStr}")

// Check what day it is
let day: string = date.dayOfWeek()
console.log("Today is ${day}")
```

## API Reference

| Function | Parameters | Returns | Description |
|----------|-----------|---------|-------------|
| `date.now()` | — | `number` | Current Unix timestamp (seconds) |
| `date.nowMillis()` | — | `number` | Current Unix timestamp (milliseconds) |
| `date.format(ts?, fmt?)` | `ts`: timestamp, `fmt`: format string | `string` | Format timestamp as string |
| `date.parse(str, fmt?)` | `str`: date string, `fmt`: output format | `number\|string` | Parse date string |
| `date.diff(ts1, ts2, unit?)` | `ts1`, `ts2`: timestamps, `unit`: time unit | `number` | Difference between timestamps |
| `date.add(ts, amt, unit)` | `ts`: timestamp, `amt`: amount, `unit`: time unit | `number` | Add time to timestamp |
| `date.subtract(ts, amt, unit)` | `ts`: timestamp, `amt`: amount, `unit`: time unit | `number` | Subtract time from timestamp |
| `date.dayOfWeek(ts?)` | `ts`: timestamp | `string` | Day of the week name |
