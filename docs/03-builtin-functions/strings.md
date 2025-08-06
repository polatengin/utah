---
layout: default
title: String Functions
parent: Functions
nav_order: 7
---

Utah provides a comprehensive set of string manipulation functions through the `string.*` namespace for text processing, formatting, and transformation.

## String.* Namespace Functions

### Core Utilities

```typescript
// String length
let len: number = string.length("hello");

// String trimming
let trimmed: string = string.trim("  hello  ");

// Check if string is empty
let isEmpty: boolean = string.isEmpty("");
```

### Case Conversion

```typescript
// Case conversion
let upper: string = string.toUpperCase("hello");
let lower: string = string.toLowerCase("HELLO");

// Capitalize first letter
let capitalized: string = string.capitalize("hello");
```

### Search and Testing

```typescript
// String testing
let startsWithH: boolean = string.startsWith("hello", "h");
let endsWithO: boolean = string.endsWith("hello", "o");
let containsEll: boolean = string.includes("hello", "ell");

// Find substring position
let position: number = string.indexOf("hello world", "world");
```

### Extraction and Manipulation

```typescript
// Substring extraction
let sub1: string = string.substring("hello world", 0, 5);
let sub2: string = string.slice("hello world", 6, 11);

// String replacement
let replaced: string = string.replace("hello world", "world", "utah");
let allReplaced: string = string.replaceAll("hello hello", "hello", "hi");

// String splitting
let parts: string[] = string.split("one,two,three", ",");
```

### Advanced Operations

```typescript
// String padding
let padded: string = string.padStart("42", 5, "0"); // "00042"
let padded2: string = string.padEnd("42", 5, "-"); // "42---"

// String repetition
let repeated: string = string.repeat("abc", 3); // "abcabcabc"
```

### String Interpolation and Concatenation

```typescript
// Template literals (legacy syntax still supported)
let name: string = "Utah";
let message: string = "Hello, ${name}!";

// String concatenation
let combined: string = "hello" + " " + "world";
```

## Generated Bash

Utah string functions compile to efficient bash operations:

```bash
# String length
len=${#hello}

# String concatenation
combined="hello world"

# Substring extraction
sub="${hello_world:0:5}"

# String replacement
replaced="${hello_world/world/utah}"
```

## Use Cases

- Text processing and parsing
- Configuration file manipulation
- Log processing
- Data extraction and transformation
- Template generation
