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
console.log("We have ${count} fruits");
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
console.log("Fruits: ${array.join(fruits, ", ")}");       // "Fruits: apple, banana, cherry"
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
console.log("Original: ${array.join(numbers, ", ")}");     // Still [3, 1, 4, 1, 5, 9, 2, 6]
console.log("Sorted: ${array.join(numbersAsc, ", ")}");    // [1, 1, 2, 3, 4, 5, 6, 9]

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

### Array Merging

#### Basic Merging

Combine two arrays of the same type:

```typescript
// String array merging
let names: string[] = ["alice", "bob"];
let moreNames: string[] = ["charlie", "diana"];
let allNames: string[] = array.merge(names, moreNames);  // ["alice", "bob", "charlie", "diana"]

// Number array merging
let numbers1: number[] = [1, 2, 3];
let numbers2: number[] = [4, 5, 6];
let allNumbers: number[] = array.merge(numbers1, numbers2);  // [1, 2, 3, 4, 5, 6]

// Boolean array merging
let flags1: boolean[] = [true, false];
let flags2: boolean[] = [false, true];
let allFlags: boolean[] = array.merge(flags1, flags2);  // [true, false, false, true]
```

#### Edge Cases

```typescript
// Merging with empty arrays
let letters: string[] = ["a", "b"];
let empty: string[] = [];
let result1: string[] = array.merge(letters, empty);    // ["a", "b"]
let result2: string[] = array.merge(empty, letters);    // ["a", "b"]
let result3: string[] = array.merge(empty, empty);      // []

// Single element arrays
let single1: number[] = [100];
let single2: number[] = [200];
let merged: number[] = array.merge(single1, single2);   // [100, 200]
```

#### Chaining with Other Array Methods

```typescript
// Merge then sort
let list1: string[] = ["zebra", "apple"];
let list2: string[] = ["banana", "cherry"];
let sortedMerged: string[] = array.sort(array.merge(list1, list2));  // ["apple", "banana", "cherry", "zebra"]

// Sort then merge (preserves sort order)
let sorted1: number[] = array.sort([3, 1, 4]);         // [1, 3, 4]
let sorted2: number[] = array.sort([2, 6, 5]);         // [2, 5, 6]
let finalMerged: number[] = array.merge(sorted1, sorted2);  // [1, 3, 4, 2, 5, 6]

// Complex chaining
let words1: string[] = ["hello", "world"];
let words2: string[] = ["utah", "lang"];
let sentence: string = array.join(array.merge(words1, words2), " ");  // "hello world utah lang"
```

### Array Shuffling

#### Basic Shuffling

Randomly shuffle array elements using the `array.shuffle()` function:

```typescript
// String array shuffling
let names: string[] = ["alice", "bob", "charlie", "diana"];
let shuffledNames: string[] = array.shuffle(names);  // Random order like ["charlie", "alice", "diana", "bob"]

// Number array shuffling
let numbers: number[] = [1, 2, 3, 4, 5];
let shuffledNumbers: number[] = array.shuffle(numbers);  // Random order like [3, 1, 5, 2, 4]

// Boolean array shuffling
let flags: boolean[] = [true, false, true, false];
let shuffledFlags: boolean[] = array.shuffle(flags);  // Random order like [false, true, false, true]
```

#### Shuffle Edge Cases

```typescript
// Shuffling empty arrays
let empty: string[] = [];
let shuffledEmpty: string[] = array.shuffle(empty);  // []

// Single element arrays
let single: number[] = [42];
let shuffledSingle: number[] = array.shuffle(single);  // [42]

// Two element arrays
let pair: string[] = ["first", "second"];
let shuffledPair: string[] = array.shuffle(pair);  // Either ["first", "second"] or ["second", "first"]
```

#### Practical Use Cases

```typescript
// Card shuffling for games
let deck: string[] = ["A♠", "K♠", "Q♠", "J♠", "10♠", "9♠", "8♠", "7♠"];
let shuffledDeck: string[] = array.shuffle(deck);
console.log("Shuffled deck: " + array.join(shuffledDeck, ", "));

// Random question order for quizzes
let questions: string[] = ["What is 2+2?", "What is the capital of France?", "Who wrote Romeo and Juliet?"];
let randomizedQuestions: string[] = array.shuffle(questions);

// Random player order for games
let players: string[] = ["Alice", "Bob", "Charlie", "Diana"];
let gameOrder: string[] = array.shuffle(players);
console.log("Player order: " + array.join(gameOrder, " -> "));

// Random selection from pool
let colors: string[] = ["red", "blue", "green", "yellow", "purple"];
let shuffledColors: string[] = array.shuffle(colors);
let randomColor: string = shuffledColors[0];  // First element of shuffled array
console.log("Random color selected: " + randomColor);
```

#### Shuffle Chaining with Other Array Methods

```typescript
// Shuffle then take first N elements
let items: string[] = ["item1", "item2", "item3", "item4", "item5"];
let shuffledItems: string[] = array.shuffle(items);
let randomThree: string = array.join([shuffledItems[0], shuffledItems[1], shuffledItems[2]], ", ");

// Sort then shuffle (useful for testing)
let sorted: number[] = array.sort([5, 1, 3, 2, 4]);  // [1, 2, 3, 4, 5]
let shuffledSorted: number[] = array.shuffle(sorted);  // Random order of sorted elements

// Multiple shuffles produce different results
let originalDeck: string[] = ["A", "K", "Q", "J"];
let shuffle1: string[] = array.shuffle(originalDeck);
let shuffle2: string[] = array.shuffle(originalDeck);
// shuffle1 and shuffle2 will likely have different orders

// Use in conditionals
if (!array.isEmpty(array.shuffle(numbers))) {
  console.log("Shuffled array contains elements");
}
```

### Array Deduplication

#### Basic Unique Operations

Remove duplicate elements from arrays using the `array.unique()` function:

```typescript
// String array deduplication
let names: string[] = ["alice", "bob", "alice", "charlie", "bob"];
let uniqueNames: string[] = array.unique(names);  // ["alice", "bob", "charlie"]

// Number array deduplication
let numbers: number[] = [1, 2, 2, 3, 1, 4, 3, 5];
let uniqueNumbers: number[] = array.unique(numbers);  // [1, 2, 3, 4, 5]

// Boolean array deduplication
let flags: boolean[] = [true, false, true, false, true];
let uniqueFlags: boolean[] = array.unique(flags);  // [true, false]
```

#### Order Preservation

The `array.unique()` function preserves the order of first occurrence:

```typescript
// First occurrence order is maintained
let items: string[] = ["c", "a", "b", "a", "c", "d", "b"];
let uniqueItems: string[] = array.unique(items);  // ["c", "a", "b", "d"]

// Even with numbers, first occurrence is preserved
let data: number[] = [5, 2, 8, 2, 1, 5, 3];
let uniqueData: number[] = array.unique(data);  // [5, 2, 8, 1, 3]
```

#### Unique Edge Cases

```typescript
// Empty arrays
let empty: string[] = [];
let uniqueEmpty: string[] = array.unique(empty);  // []

// Single element arrays
let single: number[] = [42];
let uniqueSingle: number[] = array.unique(single);  // [42]

// No duplicates
let noDuplicates: string[] = ["a", "b", "c"];
let stillUnique: string[] = array.unique(noDuplicates);  // ["a", "b", "c"]

// All duplicates
let allSame: number[] = [7, 7, 7, 7];
let onlyOne: number[] = array.unique(allSame);  // [7]
```

#### Deduplication Use Cases

```typescript
// Remove duplicates from user input
let userTags: string[] = ["javascript", "typescript", "javascript", "react", "typescript"];
let cleanTags: string[] = array.unique(userTags);
console.log("Clean tags: " + array.join(cleanTags, ", "));

// Deduplicate merged data
let list1: string[] = ["apple", "banana"];
let list2: string[] = ["banana", "cherry", "apple"];
let combined: string[] = array.merge(list1, list2);      // ["apple", "banana", "banana", "cherry", "apple"]
let uniqueCombined: string[] = array.unique(combined);   // ["apple", "banana", "cherry"]

// Data cleaning pipeline
let rawData: number[] = [10, 20, 10, 30, 20, 40];
let cleanData: number[] = array.unique(rawData);
console.log("Removed " + (array.length(rawData) - array.length(cleanData)) + " duplicates");

// Configuration options deduplication
let defaultSettings: string[] = ["debug", "verbose"];
let userSettings: string[] = ["verbose", "production", "debug"];
let allSettings: string[] = array.unique(array.merge(defaultSettings, userSettings));
console.log("Final settings: " + array.join(allSettings, ", "));
```

#### Unique Chaining with Other Array Methods

```typescript
// Unique then sort
let unsorted: number[] = [5, 2, 8, 2, 1, 5, 3];
let sortedUnique: number[] = array.sort(array.unique(unsorted));  // [1, 2, 3, 5, 8]

// Sort then unique (same result for this case)
let uniqueSorted: number[] = array.unique(array.sort(unsorted));  // [1, 2, 3, 5, 8]

// Complex chaining
let words1: string[] = ["hello", "world", "hello"];
let words2: string[] = ["world", "utah", "hello"];
let processedWords: string[] = array.sort(array.unique(array.merge(words1, words2)));
console.log("Final words: " + array.join(processedWords, ", "));  // "hello, utah, world"

// Use in data analysis
let responses: string[] = ["yes", "no", "yes", "maybe", "no", "yes"];
let uniqueResponses: string[] = array.unique(responses);
console.log("Response types: " + array.length(uniqueResponses));  // 3

// Filter duplicates before processing
let ids: number[] = [1, 2, 3, 2, 4, 1, 5];
let uniqueIds: number[] = array.unique(ids);
for (let id: number in uniqueIds) {
  console.log("Processing ID: " + id);
}
```

### Array Iteration

#### For-In Loops

Iterate over array elements:

```typescript
let servers: string[] = ["web1", "web2", "db1"];

for (let server: string in servers) {
  console.log("Checking server: ${server}");
}
```

#### Traditional For Loops

Use index-based iteration:

```typescript
let files: string[] = ["file1.txt", "file2.txt", "file3.txt"];

for (let i: number = 0; i < array.length(files); i++) {
  console.log("Processing file ${i + 1}: ${files[i]}");
}
```

#### forEach Function

Iterate over arrays with advanced control using the `array.forEach()` function:

```typescript
// Basic forEach with single parameter
let fruits: string[] = ["apple", "banana", "cherry"];

array.forEach(fruits, (fruit) => {
  console.log("Processing: ${fruit}");
});

// forEach with index parameter
let servers: string[] = ["web1", "web2", "web3"];

array.forEach(servers, (server, index) => {
  console.log("Server ${index}: ${server}");
});

// Multi-line callback for complex operations
let users: string[] = ["alice", "bob", "charlie"];

array.forEach(users, (user) => {
  console.log("Processing user: ${user}");
  let upperUser: string = string.toUpperCase(user);
  console.log("Uppercase: ${upperUser}");
  
  // Use any Utah language features within forEach
  if (string.includes(user, "a")) {
    console.log("User ${user} contains 'a'");
  }
});

// forEach with complex array expressions
array.forEach(string.split("red,green,blue", ","), (color, idx) => {
  console.log("Color ${idx}: ${color}");
});

// Data processing with forEach
let logFiles: string[] = ["app.log", "error.log", "access.log"];

array.forEach(logFiles, (logFile) => {
  if (fs.exists(logFile)) {
    console.log("Processing log file: ${logFile}");
    // Process the file content here
  } else {
    console.log("Log file not found: ${logFile}");
  }
});

// Combine forEach with other array functions
let numbers: number[] = [5, 2, 8, 1, 9];
let sortedNumbers: number[] = array.sort(numbers);

array.forEach(sortedNumbers, (value, position) => {
  console.log("Position ${position}: ${value}");
});
```

#### When to Use Each Iteration Method

- **For-in loops**: Simple iteration when you only need the element value
- **Traditional for loops**: When you need precise index control or complex loop logic
- **forEach function**: When you need both element and index, or when performing complex operations on each element

## Common Array Patterns

### Processing File Lists

```typescript
let files: string[] = ["config.json", "data.csv", "script.sh"];

for (let file: string in files) {
  if (string.endsWith(file, ".json")) {
    console.log("Processing JSON file: ${file}");
  }
  else if (string.endsWith(file, ".csv")) {
    console.log("Processing CSV file: ${file}");
  }
  else {
    console.log("Skipping unknown file type: ${file}");
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
  command = "${command} ${option}";
}
command = "${command} ${image}";

console.log("Running: ${command}");
```

### Path Manipulation

```typescript
let basePath: string = "/var/www";
let projects: string[] = ["project1", "project2", "project3"];

for (let project: string in projects) {
  let fullPath: string = "${basePath}/${project}";

  if (fs.exists(fullPath)) {
    console.log("Found project: ${fullPath}");
  }
}
```

### Data Validation

```typescript
let emails: string[] = ["user@example.com", "invalid-email", "admin@site.org"];

for (let email: string in emails) {
  if (string.contains(email, "@") && string.contains(email, ".")) {
    console.log("Valid email: ${email}");
  }
  else {
    console.log("Invalid email: ${email}");
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
    console.log("Processing: ${item}");
  }
}
```

Arrays provide the foundation for data collection and manipulation in Utah, offering type-safe operations with familiar syntax and powerful built-in functions.
