---
layout: default
title: Testing with Utah Scripts
parent: Guides
nav_order: 4

---

Learn how to do testing with Utah scripts effectively with unit tests, integration tests, and best practices.

## Test Structure

### Basic Test Layout

```typescript
// test/math.test.shx
import "../src/math.shx";

// Test addition function
function testAddition(): void {
  let result: number = add(2, 3);
  assert(result == 5, "Addition should return 5");
}

// Test subtraction function
function testSubtraction(): void {
  let result: number = subtract(5, 3);
  assert(result == 2, "Subtraction should return 2");
}

// Run tests
testAddition();
testSubtraction();
console.log("All tests passed!");
```

### Test Organization

```typescript
// test/utils.test.shx
import "../src/utils.shx";

// Test utilities
function assert(condition: boolean, message: string): void {
  if (!condition) {
    console.log("FAIL: ${message}");
    script.exit(1);
  } else {
    console.log("PASS: ${message}");
  }
}

function setUp(): void {
  // Setup test environment
  filesystem.mkdir("/tmp/test-data");
}

function tearDown(): void {
  // Clean up test environment
  filesystem.removeDirectory("/tmp/test-data");
}
```

## Unit Testing

### Testing Functions

```typescript
// src/calculator.shx
function multiply(a: number, b: number): number {
  return a * b;
}

function divide(a: number, b: number): number {
  if (b == 0) {
    throw new Error("Division by zero");
  }
  return a / b;
}

// test/calculator.test.shx
import "../src/calculator.shx";

function testMultiply(): void {
  assert(multiply(3, 4) == 12, "3 * 4 should equal 12");
  assert(multiply(0, 5) == 0, "0 * 5 should equal 0");
  assert(multiply(-2, 3) == -6, "-2 * 3 should equal -6");
}

function testDivide(): void {
  assert(divide(10, 2) == 5, "10 / 2 should equal 5");
  assert(divide(7, 3) == 2, "7 / 3 should equal 2 (integer division)");

  // Test error handling
  try {
    divide(5, 0);
    assert(false, "Division by zero should throw error");
  } catch (error) {
    assert(true, "Division by zero correctly throws error");
  }
}
```

### Testing File Operations

```typescript
// test/file-operations.test.shx
function testFileOperations(): void {
  let testFile: string = "/tmp/test.txt";

  // Test file creation
  filesystem.writeFile(testFile, "Hello, Utah!");
  assert(filesystem.exists(testFile), "File should be created");

  // Test file reading
  let content: string = filesystem.readFile(testFile);
  assert(content == "Hello, Utah!", "File content should match");

  // Test file deletion
  filesystem.remove(testFile);
  assert(!filesystem.exists(testFile), "File should be deleted");
}
```

## Integration Testing

### Testing API Interactions

```typescript
// test/api-integration.test.shx
function testApiIntegration(): void {
  // Test API endpoint
  let response: string = web.get("https://api.github.com/repos/polatengin/utah");
  let data: object = json.parse(response);

  assert(data.name == "utah", "Repository name should be 'utah'");
  assert(data.full_name == "polatengin/utah", "Full name should match");
}

function testWebhookHandler(): void {
  // Test webhook processing
  let payload: string = "{\"action\": \"push\", \"repository\": {\"name\": \"utah\"}}";
  let result: boolean = processWebhook(payload);

  assert(result == true, "Webhook should be processed successfully");
}
```

### Testing System Integration

```typescript
// test/system-integration.test.shx
function testSystemIntegration(): void {
  // Test system commands
  let result: string = system.execute("echo 'test'");
  assert(result.trim() == "test", "System command should return 'test'");

  // Test environment variables
  system.setEnv("TEST_VAR", "test_value");
  let value: string = system.env("TEST_VAR");
  assert(value == "test_value", "Environment variable should be set");
}
```

## Test Automation

### Test Runner Script

```typescript
// test/run-tests.shx
let testFiles: string[] = [
  "test/math.test.shx",
  "test/calculator.test.shx",
  "test/file-operations.test.shx",
  "test/api-integration.test.shx"
];

let passed: number = 0;
let failed: number = 0;

for (let testFile of testFiles) {
  console.log("Running ${testFile}...");

  try {
    system.execute("utah ${testFile}");
    console.log("✅ ${testFile} passed");
    passed++;
  } catch (error) {
    console.log("❌ ${testFile} failed");
    failed++;
  }
}

console.log("\nTest Results: ${passed} passed, ${failed} failed");

if (failed > 0) {
  script.exit(1);
}
```

### Continuous Integration

```yaml
# .github/workflows/test.yml
name: Test Utah Scripts

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Install Utah
      run: |
        curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

    - name: Run Tests
      run: |
        utah test/run-tests.shx
```

## Mocking and Stubbing

### Mocking External Dependencies

```typescript
// test/mocks.shx
let mockApiResponse: string = "{\"status\": \"success\", \"data\": {\"id\": 1}}";

function mockWebGet(url: string): string {
  console.log("Mock: GET ${url}");
  return mockApiResponse;
}

// Override web.get for testing
web.get = mockWebGet;
```

### Stubbing System Commands

```typescript
// test/stubs.shx
function stubSystemExecute(command: string): string {
  if (command == "git status") {
    return "On branch main\nnothing to commit, working tree clean";
  }
  return "Command not found";
}

// Override system.execute for testing
system.execute = stubSystemExecute;
```

## Performance Testing

### Benchmarking Functions

```typescript
// test/performance.test.shx
function benchmarkFunction(func: Function, iterations: number): number {
  let startTime: number = utility.timestamp();

  for (let i: number = 0; i < iterations; i++) {
    func();
  }

  let endTime: number = utility.timestamp();
  return endTime - startTime;
}

function testPerformance(): void {
  let executionTime: number = benchmarkFunction(() => {
    // Function to benchmark
    utility.hash("test string");
  }, 1000);

  console.log("Execution time: ${executionTime}ms");
  assert(executionTime < 5000, "Function should execute in under 5 seconds");
}
```

## Best Practices

### Test File Organization

- Keep tests in a separate `test/` directory
- Use descriptive test function names
- Group related tests together
- Use setup and teardown functions for common operations

### Test Coverage

- Test normal cases and edge cases
- Test error conditions and exception handling
- Test boundary conditions
- Test integration points

### Test Data Management

- Use temporary files for file system tests
- Clean up test data after each test
- Use consistent test data across tests
- Avoid dependencies on external services in unit tests

### Assertions and Reporting

- Use clear assertion messages
- Provide detailed error information
- Log test progress and results
- Exit with appropriate codes for CI/CD integration
