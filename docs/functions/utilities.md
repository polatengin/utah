---
sidebar_position: 7
---

# Utility Functions

Utah provides a collection of utility functions for common programming tasks, calculations, and data processing.

## Math Operations

### Basic Math

```typescript
// Random number generation
let random: number = utility.random(1, 100);

// Absolute value
let abs: number = utility.abs(-42);

// Power calculation
let power: number = utility.pow(2, 8);

// Square root
let sqrt: number = utility.sqrt(16);

// Rounding
let rounded: number = utility.round(3.14159);
let floor: number = utility.floor(3.9);
let ceil: number = utility.ceil(3.1);
```

### Advanced Math

```typescript
// Trigonometric functions
let sin: number = utility.sin(0.5);
let cos: number = utility.cos(0.5);
let tan: number = utility.tan(0.5);

// Logarithmic functions
let log: number = utility.log(10);
let log10: number = utility.log10(100);

// Min/Max operations
let min: number = utility.min(5, 3, 8, 1);
let max: number = utility.max(5, 3, 8, 1);
```

## String Utilities

### String Processing

```typescript
// Generate UUID
let uuid: string = utility.uuid();

// Generate random string
let randomStr: string = utility.randomString(10);

// Hash string
let hash: string = utility.hash("input string");

// Base64 encoding/decoding
let encoded: string = utility.base64Encode("hello world");
let decoded: string = utility.base64Decode(encoded);
```

### String Validation

```typescript
// Check if string is numeric
let isNum: boolean = utility.isNumeric("123");

// Check if string is email
let isEmail: boolean = utility.isEmail("user@example.com");

// Check if string is URL
let isUrl: boolean = utility.isUrl("https://example.com");

// Check if string is empty
let isEmpty: boolean = utility.isEmpty(" ");
```

## Date and Time

### Date Operations

```typescript
// Current timestamp
let now: number = utility.timestamp();

// Current date string
let dateStr: string = utility.dateString();

// Format date
let formatted: string = utility.formatDate(timestamp, "YYYY-MM-DD");

// Parse date
let parsed: number = utility.parseDate("2024-01-15");
```

### Time Calculations

```typescript
// Add days to date
let future: number = utility.addDays(timestamp, 7);

// Subtract days from date
let past: number = utility.subtractDays(timestamp, 30);

// Difference between dates
let diff: number = utility.dateDiff(date1, date2);
```

## Array Utilities

### Array Operations

```typescript
// Shuffle array
let shuffled: string[] = utility.shuffle(["a", "b", "c", "d"]);

// Unique values
let unique: string[] = utility.unique(["a", "b", "a", "c", "b"]);

// Array intersection
let common: string[] = utility.intersect(array1, array2);

// Array difference
let diff: string[] = utility.difference(array1, array2);
```

### Array Statistics

```typescript
// Sum of numbers
let sum: number = utility.sum([1, 2, 3, 4, 5]);

// Average of numbers
let avg: number = utility.average([1, 2, 3, 4, 5]);

// Median of numbers
let median: number = utility.median([1, 2, 3, 4, 5]);

// Mode of numbers
let mode: number = utility.mode([1, 2, 2, 3, 3, 3]);
```

## File and Path Utilities

### Path Operations

```typescript
// Join paths
let fullPath: string = utility.joinPath("/home", "user", "documents");

// Get file extension
let ext: string = utility.getExtension("file.txt");

// Get filename without extension
let name: string = utility.getBasename("file.txt");

// Normalize path
let normalized: string = utility.normalizePath("/home/user/../user/docs");
```

### File Operations

```typescript
// Get file size
let size: number = utility.fileSize("/path/to/file.txt");

// Get file modification time
let mtime: number = utility.fileModTime("/path/to/file.txt");

// Check file permissions
let readable: boolean = utility.isReadable("/path/to/file.txt");
let writable: boolean = utility.isWritable("/path/to/file.txt");
```

## Generated Bash

Utility functions compile to appropriate bash operations:

```bash
# Random number generation
random=$(shuf -i 1-100 -n 1)

# UUID generation
uuid=$(uuidgen)

# Base64 encoding
encoded=$(echo -n "hello world" | base64)

# Current timestamp
now=$(date +%s)

# File operations
size=$(stat -c %s "/path/to/file.txt")
mtime=$(stat -c %Y "/path/to/file.txt")
```

## Use Cases

- Data processing and analysis
- Mathematical calculations
- String manipulation and validation
- Date and time operations
- File system utilities
- Random data generation
- Cryptographic operations
