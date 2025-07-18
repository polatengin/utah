---
layout: default
title: Arrays and Strings
parent: Language Features
nav_order: 4
---

Utah provides powerful built-in functions for working with arrays and strings, making data manipulation intuitive and type-safe.

## Arrays

### Declaration and Initialization

```typescript
// Empty array
let users: string[] = [];

// Array with initial values
let numbers: number[] = [1, 2, 3, 4, 5];

// Mixed types (object array)
let items: object[] = [];
```

### Array Methods

#### `array.length()`

Get the number of elements in an array:

```typescript
let fruits: string[] = ["apple", "banana", "orange"];
let count: number = array.length(fruits);
console.log(`We have ${count} fruits`);
```

#### `array.contains()`

Check if an array contains a specific value:

```typescript
let languages: string[] = ["javascript", "python", "go"];

if (array.contains(languages, "python")) {
  console.log("Python is supported");
}
```

#### `array.isEmpty()`

Check if an array is empty:

```typescript
let tasks: string[] = [];

if (array.isEmpty(tasks)) {
  console.log("No tasks pending");
}
```

#### `array.reverse()`

Reverse the order of elements:

```typescript
let numbers: number[] = [1, 2, 3, 4, 5];
let reversed: number[] = numbers.reverse();
// reversed is [5, 4, 3, 2, 1]
```

#### `array.join()`

Join array elements into a string with a separator:

```typescript
let fruits: string[] = ["apple", "banana", "cherry"];

// Join with default separator (comma)
let defaultJoin: string = fruits.join();           // "apple,banana,cherry"

// Join with custom separators
let pipeJoin: string = fruits.join(" | ");         // "apple | banana | cherry"
let dashJoin: string = fruits.join("-");           // "apple-banana-cherry"
let spaceJoin: string = fruits.join(" ");          // "apple banana cherry"

// Join different array types
let numbers: number[] = [1, 2, 3, 4, 5];
let numberString: string = numbers.join("-");      // "1-2-3-4-5"

// Use in string interpolation
console.log(`Fruits: ${fruits.join(", ")}`);       // "Fruits: apple, banana, cherry"
```

### Array Iteration

#### For-In Loops

Iterate over array elements:

```typescript
let servers: string[] = ["web1", "web2", "db1"];

for (let server: string in servers) {
  console.log(`Checking server: ${server}`);
}
```

#### Traditional For Loops

Use index-based iteration:

```typescript
let files: string[] = ["file1.txt", "file2.txt", "file3.txt"];

for (let i: number = 0; i < array.length(files); i++) {
  console.log(`Processing file ${i + 1}: ${files[i]}`);
}
```

## Strings

### String Methods

#### `string.length()`

Get the length of a string:

```typescript
let message: string = "Hello, Utah!";
let len: number = string.length(message);
console.log(`Message length: ${len}`);
```

#### `string.contains()`

Check if a string contains a substring:

```typescript
let filepath: string = "/home/user/script.shx";

if (string.contains(filepath, ".shx")) {
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

Replace occurrences of a substring:

```typescript
let message: string = "Hello, World!";
let updated: string = string.replace(message, "World", "Utah");
// updated is "Hello, Utah!"
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

### String Interpolation

Use template literals for dynamic strings:

```typescript
let name: string = "Alice";
let age: number = 30;
let message: string = `Hello, ${name}! You are ${age} years old.`;
```

### Multi-line Strings

Utah supports multi-line string literals:

```typescript
let script: string = `
  #!/bin/bash
  echo "Starting process..."
  ./run-task.sh
  echo "Process completed"
`;
```

## Common Patterns

### Processing File Lists

```typescript
let files: string[] = ["config.json", "data.csv", "script.sh"];

for (let file: string in files) {
  if (string.endsWith(file, ".json")) {
    console.log(`Processing JSON file: ${file}`);
  }
  else if (string.endsWith(file, ".csv")) {
    console.log(`Processing CSV file: ${file}`);
  }
  else {
    console.log(`Skipping unknown file type: ${file}`);
  }
}
```

### Building Command Arguments

```typescript
let baseCommand: string = "docker run";
let options: string[] = ["-d", "--name myapp", "-p 8080:80"];
let image: string = "nginx:latest";

let command: string = baseCommand;
for (let option: string in options) {
  command = `${command} ${option}`;
}
command = `${command} ${image}`;

console.log(`Running: ${command}`);
```

### Path Manipulation

```typescript
let basePath: string = "/var/www";
let projects: string[] = ["project1", "project2", "project3"];

for (let project: string in projects) {
  let fullPath: string = `${basePath}/${project}`;
  
  if (fs.exists(fullPath)) {
    console.log(`Found project: ${fullPath}`);
  }
}
```

### Data Validation

```typescript
let emails: string[] = ["user@example.com", "invalid-email", "admin@site.org"];

for (let email: string in emails) {
  if (string.contains(email, "@") && string.contains(email, ".")) {
    console.log(`Valid email: ${email}`);
  }
  else {
    console.log(`Invalid email: ${email}`);
  }
}
```

## Type Safety

Arrays and strings in Utah are strongly typed:

```typescript
// Type-safe array operations
let numbers: number[] = [1, 2, 3];
let strings: string[] = ["a", "b", "c"];

// This would cause a compile error:
// numbers[0] = "invalid";  // Error: Cannot assign string to number

// Safe string operations
let message: string = "Hello";
let length: number = string.length(message);  // Always returns number
```

## Performance Tips

1. **Pre-allocate arrays** when possible to avoid frequent resizing
2. **Use appropriate methods** for your use case (contains vs iteration)
3. **Cache array lengths** in loops to avoid repeated calculations
4. **Use string interpolation** instead of repeated concatenation

## Error Handling

Always validate inputs when working with arrays and strings:

```typescript
function processArray(items: string[]): void {
  if (array.isEmpty(items)) {
    console.log("Warning: Empty array provided");
    return;
  }
  
  for (let item: string in items) {
    if (string.length(item) == 0) {
      console.log("Warning: Empty string found, skipping");
      continue;
    }
    
    // Process item
    console.log(`Processing: ${item}`);
  }
}
```

Arrays and strings form the foundation of data manipulation in Utah, providing type-safe operations with familiar syntax and powerful built-in functions.
