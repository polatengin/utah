---
layout: default
title: Array Functions
parent: Functions
nav_order: 5
---

Utah provides powerful array manipulation functions for data processing and collection handling.

## Array Operations

### Basic Functions

```typescript
// Array declaration
let fruits: string[] = ["apple", "banana", "cherry"];

// Array length
let count: number = fruits.length;

// Array access
let first: string = fruits[0];

// Array push
fruits.push("orange");

// Array pop
let last: string = fruits.pop();
```

### Array Manipulation

```typescript
// Array slicing
let slice: string[] = fruits.slice(1, 3);

// Array joining
let joined: string = fruits.join(", ");

// Array reversing
let reversed: string[] = fruits.reverse();

// Array contains
let hasApple: boolean = fruits.includes("apple");
```

### Array Iteration

```typescript
// For-each loop
for (let fruit of fruits) {
  console.log(fruit);
}

// For-in loop
for (let i in fruits) {
  console.log(`${i}: ${fruits[i]}`);
}

// Traditional for loop
for (let i: number = 0; i < fruits.length; i++) {
  console.log(fruits[i]);
}
```

## Generated Bash

Utah array functions compile to bash array operations:

```bash
# Array declaration
fruits=("apple" "banana" "cherry")

# Array length
count=${#fruits[@]}

# Array access
first="${fruits[0]}"

# Array push
fruits+=("orange")

# Array iteration
for fruit in "${fruits[@]}"; do
  echo "$fruit"
done
```

## Use Cases

- Data collection and processing
- Configuration management
- File list processing
- Command result handling
- Batch operations
