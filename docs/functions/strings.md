---
sidebar_position: 2
---

# String Functions

Utah provides a comprehensive set of string manipulation functions for text processing, formatting, and transformation.

## String Operations

### Basic Functions

```typescript
// String length
let len: number = "hello".length;

// String concatenation
let combined: string = "hello" + " " + "world";

// Substring extraction
let sub: string = "hello world".substring(0, 5);

// String replacement
let replaced: string = "hello world".replace("world", "utah");
```

### String Formatting

```typescript
// Template literals
let name: string = "Utah";
let message: string = `Hello, ${name}!`;

// String interpolation
let count: number = 42;
let result: string = `There are ${count} items`;
```

### String Transformation

```typescript
// Case conversion
let upper: string = "hello".toUpperCase();
let lower: string = "HELLO".toLowerCase();

// String trimming
let trimmed: string = "  hello  ".trim();

// String splitting
let parts: string[] = "one,two,three".split(",");
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
