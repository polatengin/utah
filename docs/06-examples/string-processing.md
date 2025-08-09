---
layout: default
title: String Processing
parent: Examples
nav_order: 3
description: "String manipulation tutorial using Utah's powerful string functions"
permalink: /examples/string-processing/
---

A comprehensive tutorial on string manipulation using Utah's powerful built-in string functions. This example demonstrates cleaning, formatting, and validating text data with clean, readable syntax.

## Features Demonstrated

- **String cleaning** with trim and normalization
- **Case conversion** with toLowerCase and capitalize
- **String searching** with indexOf and includes
- **String extraction** with substring operations
- **String replacement** for data transformation
- **Email validation** using string operations
- **Format validation** with boolean logic

## Complete Script

```typescript
// String manipulation example
// Demonstrates the power of string.* functions

let userInput: string = "  john.doe@COMPANY.COM  ";

// Clean and normalize the email
let cleanedEmail: string = string.trim(userInput);
let normalizedEmail: string = string.toLowerCase(cleanedEmail);

// Find the @ position to split manually
let atPosition: number = string.indexOf(normalizedEmail, "@");
let username: string = string.substring(normalizedEmail, 0, atPosition);
let domain: string = string.substring(normalizedEmail, atPosition + 1);

// Format username nicely
let formattedUsername: string = string.replace(username, ".", " ");
let capitalizedUsername: string = string.capitalize(formattedUsername);

console.log("Original input: '" + userInput + "'");
console.log("Clean email: " + normalizedEmail);
console.log("Username: " + username);
console.log("Domain: " + domain);
console.log("Display name: " + capitalizedUsername);

// Validate email format
let hasAt: boolean = string.includes(normalizedEmail, "@");
let hasValidDomain: boolean = string.includes(domain, ".");

if (hasAt == true && hasValidDomain == true) {
    console.log("✅ Email format appears valid");
} else {
    console.log("❌ Invalid email format");
}
```

## Key String Functions Explained

### String Cleaning

Utah provides built-in functions for cleaning strings:

```typescript
let userInput: string = "  john.doe@COMPANY.COM  ";
let cleanedEmail: string = string.trim(userInput);           // Removes whitespace
let normalizedEmail: string = string.toLowerCase(cleanedEmail); // Converts to lowercase
```

**Equivalent Bash (complex):**

```bash
userInput="  john.doe@COMPANY.COM  "
cleanedEmail=$(echo "$userInput" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
normalizedEmail=$(echo "$cleanedEmail" | tr '[:upper:]' '[:lower:]')
```

### String Searching and Extraction

Find positions and extract substrings easily:

```typescript
let atPosition: number = string.indexOf(normalizedEmail, "@");
let username: string = string.substring(normalizedEmail, 0, atPosition);
let domain: string = string.substring(normalizedEmail, atPosition + 1);
```

**Equivalent Bash (complex):**

```bash
atPosition=$(expr index "$normalizedEmail" "@")
username=${normalizedEmail%@*}
domain=${normalizedEmail#*@}
```

### String Transformation

Format and transform strings with simple functions:

```typescript
let formattedUsername: string = string.replace(username, ".", " ");
let capitalizedUsername: string = string.capitalize(formattedUsername);
```

**Equivalent Bash (complex):**

```bash
formattedUsername=${username//\./ }
capitalizedUsername=$(echo "$formattedUsername" | sed 's/\b\w/\U&/g')
```

### String Validation

Clean boolean logic for validation:

```typescript
let hasAt: boolean = string.includes(normalizedEmail, "@");
let hasValidDomain: boolean = string.includes(domain, ".");

if (hasAt == true && hasValidDomain == true) {
    console.log("✅ Email format appears valid");
} else {
    console.log("❌ Invalid email format");
}
```

**Equivalent Bash (verbose):**

```bash
if [[ "$normalizedEmail" == *"@"* ]] && [[ "$domain" == *"."* ]]; then
    echo "✅ Email format appears valid"
else
    echo "❌ Invalid email format"
fi
```

## More String Processing Examples

### URL Processing

```typescript
let url: string = "https://api.github.com/users/john-doe/repos";

// Extract components
let protocol: string = string.substring(url, 0, string.indexOf(url, "://"));
let withoutProtocol: string = string.substring(url, string.indexOf(url, "://") + 3);
let domain: string = string.substring(withoutProtocol, 0, string.indexOf(withoutProtocol, "/"));
let path: string = string.substring(withoutProtocol, string.indexOf(withoutProtocol, "/"));

console.log(`Protocol: ${protocol}`);
console.log(`Domain: ${domain}`);
console.log(`Path: ${path}`);

// Validate HTTPS
if (string.startsWith(url, "https://")) {
    console.log("✅ Secure connection");
} else {
    console.log("⚠️  Insecure connection");
}
```

### File Path Processing

```typescript
let filePath: string = "/home/user/documents/report.pdf";

// Extract path components
let directory: string = string.substring(filePath, 0, string.lastIndexOf(filePath, "/"));
let filename: string = string.substring(filePath, string.lastIndexOf(filePath, "/") + 1);
let extension: string = string.substring(filename, string.lastIndexOf(filename, ".") + 1);
let nameWithoutExt: string = string.substring(filename, 0, string.lastIndexOf(filename, "."));

console.log(`Directory: ${directory}`);
console.log(`Filename: ${filename}`);
console.log(`Name: ${nameWithoutExt}`);
console.log(`Extension: ${extension}`);

// File type validation
if (string.endsWith(filePath, ".pdf")) {
    console.log("📄 PDF document detected");
} else if (string.endsWith(filePath, ".txt")) {
    console.log("📝 Text file detected");
} else {
    console.log("📁 Unknown file type");
}
```

### Data Validation and Formatting

```typescript
let phoneNumber: string = "+1-555-123-4567";

// Clean phone number
let cleanPhone: string = string.replace(phoneNumber, "-", "");
cleanPhone = string.replace(cleanPhone, "+", "");
cleanPhone = string.replace(cleanPhone, " ", "");

console.log(`Original: ${phoneNumber}`);
console.log(`Cleaned: ${cleanPhone}`);

// Validate length
if (string.length(cleanPhone) == 11) {
    console.log("✅ Valid US phone number length");
} else {
    console.log("❌ Invalid phone number length");
}

// Format for display
let countryCode: string = string.substring(cleanPhone, 0, 1);
let areaCode: string = string.substring(cleanPhone, 1, 4);
let exchange: string = string.substring(cleanPhone, 4, 7);
let number: string = string.substring(cleanPhone, 7);

let formattedPhone: string = `+${countryCode} (${areaCode}) ${exchange}-${number}`;
console.log(`Formatted: ${formattedPhone}`);
```

## Complete String Function Reference

| Function | Description | Example |
|----------|-------------|---------|
| `string.trim(str)` | Remove leading/trailing whitespace | `"  hello  "` → `"hello"` |
| `string.toLowerCase(str)` | Convert to lowercase | `"HELLO"` → `"hello"` |
| `string.toUpperCase(str)` | Convert to uppercase | `"hello"` → `"HELLO"` |
| `string.capitalize(str)` | Capitalize words | `"hello world"` → `"Hello World"` |
| `string.indexOf(str, search)` | Find first occurrence position | `indexOf("hello", "l")` → `2` |
| `string.lastIndexOf(str, search)` | Find last occurrence position | `lastIndexOf("hello", "l")` → `3` |
| `string.substring(str, start, end)` | Extract substring | `substring("hello", 1, 4)` → `"ell"` |
| `string.includes(str, search)` | Check if string contains text | `includes("hello", "ell")` → `true` |
| `string.startsWith(str, prefix)` | Check if string starts with text | `startsWith("hello", "he")` → `true` |
| `string.endsWith(str, suffix)` | Check if string ends with text | `endsWith("hello", "lo")` → `true` |
| `string.replace(str, old, new)` | Replace text | `replace("hello", "l", "x")` → `"hexxo"` |
| `string.length(str)` | Get string length | `length("hello")` → `5` |
| `string.split(str, delimiter)` | Split into array | `split("a,b,c", ",")` → `["a","b","c"]` |

## Usage Examples

### Basic String Processing

```bash
utah compile string-processing.shx
./string-processing.sh
```

### Output

```text
Original input: '  john.doe@COMPANY.COM  '
Clean email: john.doe@company.com
Username: john.doe
Domain: company.com
Display name: John Doe
✅ Email format appears valid
```

### With Different Input

```typescript
let userInput: string = "invalid-email-format";
// ... rest of script
```

Output:

```text
Original input: 'invalid-email-format'
Clean email: invalid-email-format
Username: invalid-email-format
Domain:
Display name: Invalid-email-format
❌ Invalid email format
```

## Benefits Over Traditional Bash

- **Clean Syntax**: No complex parameter expansion or external tools
- **Type Safety**: All string operations return the correct type
- **Readable Code**: Function names clearly indicate their purpose
- **No External Dependencies**: All functions are built into Utah
- **Consistent Behavior**: Functions work the same way across platforms
- **Error Handling**: Built-in validation and error checking

## Traditional Bash Comparison

To achieve the same functionality in bash would require:

```bash
#!/bin/bash

userInput="  john.doe@COMPANY.COM  "

# Complex trimming
cleanedEmail=$(echo "$userInput" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')

# Case conversion
normalizedEmail=$(echo "$cleanedEmail" | tr '[:upper:]' '[:lower:]')

# Extract username and domain
IFS='@' read -ra PARTS <<< "$normalizedEmail"
username="${PARTS[0]}"
domain="${PARTS[1]}"

# Replace dots with spaces (complex)
formattedUsername=$(echo "$username" | sed 's/\./ /g')

# Capitalize each word (very complex)
capitalizedUsername=$(echo "$formattedUsername" | sed 's/\b\w/\U&/g')

# Check for @ and . (verbose)
if [[ "$normalizedEmail" == *"@"* ]] && [[ "$domain" == *"."* ]]; then
    echo "✅ Email format appears valid"
else
    echo "❌ Invalid email format"
fi
```

The Utah version is:

- **50% shorter** (fewer lines of code)
- **More readable** (self-documenting function names)
- **Type safe** (no string conversion errors)
- **More maintainable** (clear intent and structure)

## Related Examples

- [Log File Analyzer](log-file-analyzer) - Working with string arrays and data structures
- [Health Check](health-check) - Using strings in system monitoring
- [Loops](loops) - Iterating over string collections
