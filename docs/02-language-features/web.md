---
layout: default
title: Web Functions
parent: Language Features
nav_order: 10
---

Utah provides web-related functions for HTTP requests and web service integration.

## HTTP Operations

### Available Functions

Currently implemented web functions:

#### web.get(url)

Performs an HTTP GET request to the specified URL.

```typescript
// Basic GET request
let response: string = web.get("https://api.example.com/data");
console.log(`Response: ${response}`);

// Use with variables
let apiUrl: string = "https://api.github.com/users/octocat";
let userInfo: string = web.get(apiUrl);
console.log(`User Info: ${userInfo}`);
```

#### web.delete(url, options?)

Performs an HTTP DELETE request to the specified URL with optional headers or curl options.

```typescript
// Basic DELETE request
let response: string = web.delete("https://api.example.com/users/123");
console.log(`Delete Response: ${response}`);

// DELETE with authorization headers
let authResponse: string = web.delete("https://api.example.com/users/123", "-H 'Authorization: Bearer token123'");
console.log(`Authenticated Delete: ${authResponse}`);

// DELETE with multiple headers
let fullResponse: string = web.delete("https://api.example.com/users/123", "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'");
console.log(`Full Response: ${fullResponse}`);
```

#### web.post(url, data, options?)

Performs an HTTP POST request to the specified URL with data and optional headers or curl options.

```typescript
// Basic POST request with JSON data
let response: string = web.post("https://api.example.com/users", '{"name": "John", "email": "john@example.com"}');
console.log(`Post Response: ${response}`);

// POST with form data
let formResponse: string = web.post("https://api.example.com/submit", "name=John&email=john@example.com");
console.log(`Form Response: ${formResponse}`);

// POST with authorization headers
let authResponse: string = web.post("https://api.example.com/users", '{"name": "Alice"}', "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'");
console.log(`Authenticated Post: ${authResponse}`);

// POST with variables
let apiUrl: string = "https://api.example.com/create";
let userData: string = '{"name": "Bob", "role": "admin"}';
let createResponse: string = web.post(apiUrl, userData);
console.log(`Create Response: ${createResponse}`);
console.log(`Full Response: ${fullResponse}`);
```

## Generated Bash

Web functions compile to appropriate curl commands:

```bash
# GET request
response=$(curl -s "https://api.example.com/data" 2>/dev/null || echo "")

# DELETE request
response=$(curl -s -X DELETE "https://api.example.com/users/123" 2>/dev/null || echo "")

# DELETE with headers
authResponse=$(curl -s -X DELETE "-H 'Authorization: Bearer token123'" "https://api.example.com/users/123" 2>/dev/null || echo "")

# POST request
response=$(curl -s -X POST -d '{"name": "John", "email": "john@example.com"}' "https://api.example.com/users" 2>/dev/null || echo "")

# POST with headers
authResponse=$(curl -s -X POST "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'" -d '{"name": "Alice"}' "https://api.example.com/users" 2>/dev/null || echo "")

# POST with variables
userData='{"name": "Bob", "role": "admin"}'
createResponse=$(curl -s -X POST -d ${userData} ${apiUrl} 2>/dev/null || echo "")
```

## Error Handling

All web functions include automatic error handling:

- Errors are suppressed with `2>/dev/null`
- Empty string is returned on failure using `|| echo ""`
- This prevents scripts from failing on network issues

## Use Cases

- API integration and testing
- RESTful resource management (GET, POST, DELETE operations)
- Data creation and submission (forms, JSON APIs)
- Authentication and authorization testing
- Webhook and API endpoint testing
- Content management system integration
- Web service monitoring
- API endpoint testing
- Simple HTTP operations

## Future Functions

The following web functions are planned for future releases:

- `web.put()` - HTTP PUT requests
- `web.patch()` - HTTP PATCH requests
- URL manipulation functions
- File download utilities
