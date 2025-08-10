---
layout: default
title: Strings
parent: Language Features
nav_order: 5
---

Utah provides powerful built-in functions for working with strings, making text manipulation intuitive and type-safe.

## Strings

### String Declaration and Initialization

```typescript
// Basic string declaration
let message: string = "Hello, Utah!";

// Empty string
let empty: string = "";

// String with special characters
let path: string = "/usr/local/bin";
```

### String Methods

#### `string.length()`

Get the length of a string:

```typescript
let message: string = "Hello, Utah!";
let len: number = string.length(message);
console.log("Message length: ${len}");
```

#### `string.includes()`

Check if a string contains a substring:

```typescript
let filepath: string = "/home/user/script.shx";

if (string.includes(filepath, ".shx")) {
  console.log("This is a Utah script file");
}
```

#### `string.startsWith()`

Check if a string starts with a prefix:

```typescript
let url: string = "https://api.example.com";

if (string.startsWith(url, "https://")) {
  console.log("Secure connection");
}
```

#### `string.endsWith()`

Check if a string ends with a suffix:

```typescript
let filename: string = "backup.tar.gz";

if (string.endsWith(filename, ".tar.gz")) {
  console.log("This is a compressed archive");
}
```

#### `string.split()`

Split a string into an array:

```typescript
let path: string = "/usr/local/bin";
let parts: string[] = string.split(path, "/");
// parts is ["", "usr", "local", "bin"]
```

#### `string.replace()`

Replace first occurrence of a substring:

```typescript
let message: string = "Hello, World!";
let updated: string = string.replace(message, "World", "Utah");
// updated is "Hello, Utah!"
```

#### `string.replaceAll()`

Replace all occurrences of a substring:

```typescript
let text: string = "hello hello world";
let updated: string = string.replaceAll(text, "hello", "hi");
// updated is "hi hi world"
```

#### `string.trim()`

Remove whitespace from both ends:

```typescript
let input: string = "  hello world  ";
let clean: string = string.trim(input);
// clean is "hello world"
```

#### `string.toUpperCase()` and `string.toLowerCase()`

Change case:

```typescript
let name: string = "Utah";
let upper: string = string.toUpperCase(name);  // "UTAH"
let lower: string = string.toLowerCase(name);  // "utah"
```

#### `string.capitalize()`

Capitalize the first letter of a string:

```typescript
let name: string = "john";
let capitalized: string = string.capitalize(name);  // "John"
```

#### `string.substring()` and `string.slice()`

Extract parts of a string:

```typescript
let text: string = "JavaScript";

// substring(start, end) - extracts from start index to end index
let sub: string = string.substring(text, 0, 4);  // "Java"

// slice(start, end) - similar to substring but supports negative indices
let sliced: string = string.slice(text, 4, 10);  // "Script"
```

#### `string.indexOf()`

Find the position of a substring:

```typescript
let text: string = "The quick brown fox";
let position: number = string.indexOf(text, "brown");  // 10
let notFound: number = string.indexOf(text, "cat");    // -1
```

#### `string.padStart()` and `string.padEnd()`

Add padding to strings:

```typescript
let num: string = "42";

// Pad start with zeros to make it 5 characters
let padded: string = string.padStart(num, 5, "0");  // "00042"

// Pad end with dashes to make it 5 characters
let paddedEnd: string = string.padEnd(num, 5, "-");  // "42---"
```

#### `string.repeat()`

Repeat a string multiple times:

```typescript
let pattern: string = "abc";
let repeated: string = string.repeat(pattern, 3);  // "abcabcabc"
```

#### `string.isEmpty()`

Check if a string is empty:

```typescript
let text: string = "";
let whitespace: string = "   ";

let empty: boolean = string.isEmpty(text);        // true
let notEmpty: boolean = string.isEmpty(whitespace); // false
```

### String Interpolation

Use template literals for dynamic strings:

```typescript
let name: string = "Alice";
let age: number = 30;
let message: string = "Hello, ${name}! You are ${age} years old.";
```

## Common String Patterns

### File Path Processing

```typescript
let filePath: string = "/home/user/documents/report.pdf";

if (string.includes(filePath, "/home/")) {
  console.log("File is in user directory");
}

if (string.endsWith(filePath, ".pdf")) {
  console.log("This is a PDF file");
}

let pathParts: string[] = string.split(filePath, "/");
let fileName: string = pathParts[array.length(pathParts) - 1];
console.log("File name: ${fileName}");
```

### URL Processing

```typescript
let url: string = "https://api.example.com/v1/users";

if (string.startsWith(url, "https://")) {
  console.log("Secure URL");
}

let urlParts: string[] = string.split(url, "/");
let domain: string = urlParts[2];
console.log("Domain: ${domain}");
```

### Text Processing

```typescript
let input: string = "  Hello, World!  ";
let cleaned: string = string.trim(input);
let upperCase: string = string.toUpperCase(cleaned);
let replaced: string = string.replace(upperCase, "WORLD", "UTAH");

console.log("Result: ${replaced}"); // "HELLO, UTAH!"
```

### Advanced String Manipulation

```typescript
// Working with user input
let userName: string = "john_doe";
let displayName: string = string.capitalize(string.replace(userName, "_", " "));
console.log("Welcome ${displayName}!"); // "Welcome John doe!"

// Formatting numbers
let number: string = "42";
let padded: string = string.padStart(number, 6, "0");
console.log("Order #${padded}"); // "Order #000042"

// Creating patterns
let separator: string = string.repeat("-", 20);
console.log(separator); // "--------------------"

// Text analysis
let sentence: string = "The quick brown fox jumps over the lazy dog";
let wordPosition: number = string.indexOf(sentence, "fox");
let excerpt: string = string.substring(sentence, wordPosition, wordPosition + 3);
console.log("Found '${excerpt}' at position ${wordPosition}");
```

### Configuration Parsing

```typescript
let config: string = "debug=true,port=8080,host=localhost";
let pairs: string[] = string.split(config, ",");

for (let pair: string in pairs) {
  let keyValue: string[] = string.split(pair, "=");
  if (array.length(keyValue) == 2) {
    let key: string = string.trim(keyValue[0]);
    let value: string = string.trim(keyValue[1]);
    console.log("${key}: ${value}");
  }
}
```

### String Validation

```typescript
function validateInput(input: string): boolean {
  // Check if string is empty
  if (string.isEmpty(input)) {
    console.log("Input cannot be empty");
    return false;
  }

  // Check if string is only whitespace
  if (string.isEmpty(string.trim(input))) {
    console.log("Input cannot be only whitespace");
    return false;
  }

  return true;
}

function isValidIdentifier(name: string): boolean {
  // Check for valid identifier pattern
  if (string.includes(name, " ")) {
    return false;
  }

  if (string.startsWith(name, "_") || string.startsWith(name, "-")) {
    return false;
  }

  return true;
}

let userInput: string = "my_variable";
if (validateInput(userInput) && isValidIdentifier(userInput)) {
  console.log("Valid identifier: ${userInput}");
}
```

### Text Formatting and Cleanup

```typescript
// Clean and format text data
let rawData: string = "  JavaScript, Python, TypeScript  ";
let languages: string[] = string.split(rawData, ",");

for (let lang: string in languages) {
  let cleaned: string = string.trim(lang);
  let formatted: string = string.capitalize(string.toLowerCase(cleaned));
  console.log("Language: ${formatted}");
}

// Replace multiple patterns
let messyText: string = "Hello... world!!! How are you???";
let step1: string = string.replaceAll(messyText, "...", ".");
let step2: string = string.replaceAll(step1, "!!!", "!");
let step3: string = string.replaceAll(step2, "???", "?");
console.log("Cleaned: ${step3}");
```

### Log Processing

```typescript
let logLine: string = "2024-01-15 14:30:22 INFO User logged in successfully";
let parts: string[] = string.split(logLine, " ");

if (array.length(parts) >= 4) {
  let date: string = parts[0];
  let time: string = parts[1];
  let level: string = parts[2];
  let message: string = string.join(array.slice(parts, 3), " ");

  console.log("Date: ${date}, Time: ${time}, Level: ${level}");
  console.log("Message: ${message}");
}
```

## String Type Safety

Strings in Utah are strongly typed:

```typescript
// Type-safe string operations
let message: string = "Hello";
let length: number = string.length(message);  // Always returns number

// This would cause a compile error:
// let invalid: string = 123;  // Error: Cannot assign number to string
```

## String Performance Tips

1. **Use string interpolation** instead of repeated concatenation
2. **Cache string lengths** in loops to avoid repeated calculations
3. **Use specific methods** for your use case (contains vs startsWith/endsWith)
4. **Avoid unnecessary string operations** in performance-critical code

## String Error Handling

Always validate inputs when working with strings:

```typescript
function processString(input: string): void {
  if (string.length(input) == 0) {
    console.log("Warning: Empty string provided");
    return;
  }

  let trimmed: string = string.trim(input);
  if (string.length(trimmed) == 0) {
    console.log("Warning: String contains only whitespace");
    return;
  }

  // Process string
  console.log("Processing: ${trimmed}");
}
```

## String Formatting

### Template Literals

Use template literals for complex string formatting:

```typescript
let user: string = "Alice";
let action: string = "login";
let timestamp: string = "2024-01-15 14:30:22";

let logMessage: string = "[${timestamp}] User '${user}' performed action: ${action}";
console.log(logMessage);
```

### Conditional String Building

```typescript
let environment: string = "production";
let debugMode: boolean = false;

let config: string = "Environment: ${environment}";
if (debugMode) {
  config = "${config}, Debug: enabled";
} else {
  config = "${config}, Debug: disabled";
}

console.log(config);
```

### Email Validation

```typescript
function isValidEmail(email: string): boolean {
  // Check if empty or only whitespace
  if (string.isEmpty(email) || string.isEmpty(string.trim(email))) {
    return false;
  }

  // Basic email validation
  if (!string.includes(email, "@")) {
    return false;
  }

  if (!string.includes(email, ".")) {
    return false;
  }

  let emailParts: string[] = string.split(email, "@");
  if (array.length(emailParts) != 2) {
    return false;
  }

  let localPart: string = emailParts[0];
  let domainPart: string = emailParts[1];

  if (string.isEmpty(localPart) || string.isEmpty(domainPart)) {
    return false;
  }

  return true;
}

let email: string = "user@example.com";
if (isValidEmail(email)) {
  console.log("Valid email: ${email}");
} else {
  console.log("Invalid email: ${email}");
}
```

## String Escaping

Handle special characters in strings:

```typescript
// Strings with quotes
let message: string = "She said, \"Hello, world!\"";

// Strings with backslashes
let path: string = "C:\\Users\\Alice\\Documents";
let regexPattern: string = "\\d+\\.\\d+";
```

Strings form the foundation of text manipulation in Utah, providing type-safe operations with familiar syntax and powerful built-in functions for common text processing tasks.
