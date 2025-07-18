---
layout: default
title: Built-in Functions
nav_order: 4
has_children: true
---

Utah provides an extensive library of built-in functions organized into logical categories. These functions compile to efficient bash code and provide a powerful foundation for script development.

## Function Categories

### Core Functions

- **[Console Functions](console.md)** - Output, logging, and user interaction
- **[File System Functions](filesystem.md)** - File and directory operations
- **[Template Functions](template.md)** - File templating and variable substitution
- **[Operating System Functions](operating-system.md)** - System information and process management
- **[JSON/YAML Functions](json-yaml.md)** - Data serialization and parsing

### Data Manipulation

- **[String Functions](strings.md)** - String processing and manipulation
- **[Array Functions](arrays.md)** - Collection operations and iteration
- **[Utility Functions](utilities.md)** - Math, date/time, and helper functions

### Integration Functions

- **[System Functions](system.md)** - System administration and process management
- **[Web Functions](web.md)** - HTTP requests and web service integration
- **[Git Functions](git.md)** - Version control operations

## Quick Reference

### Most Common Functions

```typescript
// Console output
console.log("Hello, World!");

// File operations
let content: string = filesystem.readFile("file.txt");
filesystem.writeFile("output.txt", content);

// Template processing
template.update("config.template", "config.yml");

// System information
let platform: string = os.platform();
let isInstalled: boolean = os.isInstalled("git");

// JSON parsing
let data: object = json.parse(jsonString);
let jsonString: string = json.stringify(data);

// String manipulation
let upper: string = "hello".toUpperCase();
let parts: string[] = "a,b,c".split(",");

// Array operations
let fruits: string[] = ["apple", "banana"];
fruits.push("orange");
```

### Function Patterns

All Utah functions follow consistent patterns:

- **Predictable naming**: Functions use clear, descriptive names
- **Type safety**: Functions have well-defined input and output types
- **Error handling**: Functions throw meaningful errors for invalid inputs
- **Bash compilation**: All functions compile to efficient bash code

## Function Documentation

Each function category includes:

- Complete function reference
- Usage examples
- Generated bash code
- Best practices and use cases
- Performance considerations

## Contributing

Function requests and contributions are welcome! Please see the [contribution guidelines](../01-getting-started/contributing.md) for details on:

- Proposing new functions
- Implementation standards
- Testing requirements
- Documentation standards
