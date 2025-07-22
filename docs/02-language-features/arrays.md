---
layout: default
title: Arrays
parent: Language Features
nav_order: 4
---

Utah provides powerful built-in functions for working with arrays, making data manipulation intuitive and type-safe.

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
let reversed: number[] = array.reverse(numbers);
// reversed is [5, 4, 3, 2, 1]
```

#### `array.join()`

Join array elements into a string with a separator:

```typescript
let fruits: string[] = ["apple", "banana", "cherry"];

// Join with default separator (comma)
let defaultJoin: string = array.join(fruits);           // "apple,banana,cherry"

// Join with custom separators
let pipeJoin: string = array.join(fruits, " | ");         // "apple | banana | cherry"
let dashJoin: string = array.join(fruits, "-");           // "apple-banana-cherry"
let spaceJoin: string = array.join(fruits, " ");          // "apple banana cherry"

// Join different array types
let numbers: number[] = [1, 2, 3, 4, 5];
let numberString: string = array.join(numbers, "-");      // "1-2-3-4-5"

// Use in string interpolation
console.log(`Fruits: ${array.join(fruits, ", ")}`);       // "Fruits: apple, banana, cherry"
```

#### `array.sort()`

Sort array elements in ascending or descending order:

```typescript
let numbers: number[] = [3, 1, 4, 1, 5, 9, 2, 6];
let fruits: string[] = ["banana", "apple", "cherry", "date"];
let flags: boolean[] = [true, false, true, false];

// Sort with default order (ascending)
let numbersAsc: number[] = array.sort(numbers);         // [1, 1, 2, 3, 4, 5, 6, 9]
let fruitsAsc: string[] = array.sort(fruits);           // ["apple", "banana", "cherry", "date"]
let flagsAsc: boolean[] = array.sort(flags);            // [false, false, true, true]

// Sort with explicit ascending order
let numbersAscExplicit: number[] = array.sort(numbers, "asc");

// Sort with descending order
let numbersDesc: number[] = array.sort(numbers, "desc");  // [9, 6, 5, 4, 3, 2, 1, 1]
let fruitsDesc: string[] = array.sort(fruits, "desc");    // ["date", "cherry", "banana", "apple"]

// Original arrays remain unchanged - sort() returns a new array
console.log(`Original: ${array.join(numbers, ", ")}`);     // Still [3, 1, 4, 1, 5, 9, 2, 6]
console.log(`Sorted: ${array.join(numbersAsc, ", ")}`);    // [1, 1, 2, 3, 4, 5, 6, 9]

// Use with different data types
// - string[]: Lexicographic (alphabetical) sorting
// - number[]: Numeric sorting
// - boolean[]: false before true (false = 0, true = 1)

// Edge cases
let empty: string[] = [];
let emptySorted: string[] = array.sort(empty);          // []

let single: number[] = [42];
let singleSorted: number[] = array.sort(single);        // [42]

// Chain with other array methods
let topThreeNumbers: string = array.join(array.sort(numbers, "desc"), ", ");
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

## Common Array Patterns

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

## Array Type Safety

Arrays in Utah are strongly typed:

```typescript
// Type-safe array operations
let numbers: number[] = [1, 2, 3];
let strings: string[] = ["a", "b", "c"];

// This would cause a compile error:
// numbers[0] = "invalid";  // Error: Cannot assign string to number
```

## Array Performance Tips

1. **Pre-allocate arrays** when possible to avoid frequent resizing
2. **Use appropriate methods** for your use case (contains vs iteration)
3. **Cache array lengths** in loops to avoid repeated calculations
4. **Use array.join()** instead of repeated string concatenation

## Array Error Handling

Always validate inputs when working with arrays:

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

Arrays provide the foundation for data collection and manipulation in Utah, offering type-safe operations with familiar syntax and powerful built-in functions.
