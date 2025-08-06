---
layout: default
title: Web Functions
parent: Language Features
nav_order: 10
---

Utah provides web-related functions for HTTP requests, URL manipulation, and web service integration.

## HTTP Operations

### Basic Requests

```typescript
// GET request
let response: string = web.get("https://api.example.com/data");

// POST request
let result: string = web.post("https://api.example.com/create", {
  name: "Utah",
  version: "1.0"
});

// PUT request
let updated: string = web.put("https://api.example.com/update/1", {
  name: "Updated Utah"
});

// DELETE request
let deleted: string = web.delete("https://api.example.com/delete/1");
```

### Advanced Requests

```typescript
// Request with headers
let response: string = web.get("https://api.example.com/protected", {
  headers: {
    "Authorization": "Bearer token123",
    "Content-Type": "application/json"
  }
});

// Request with timeout
let result: string = web.get("https://slow-api.example.com", {
  timeout: 30
});

// Request with retry
let data: string = web.get("https://unreliable-api.example.com", {
  retry: 3
});
```

## URL Operations

### URL Manipulation

```typescript
// Parse URL components
let protocol: string = web.urlProtocol("https://example.com/path");
let domain: string = web.urlDomain("https://example.com/path");
let path: string = web.urlPath("https://example.com/path");

// Build URL
let url: string = web.buildUrl("https://example.com", "/api/v1", {
  param1: "value1",
  param2: "value2"
});
```

### URL Encoding

```typescript
// Encode URL component
let encoded: string = web.urlEncode("hello world");

// Decode URL component
let decoded: string = web.urlDecode("hello%20world");
```

## Web Utilities

### Download Operations

```typescript
// Download file
web.download("https://example.com/file.zip", "/local/path/file.zip");

// Download with progress
web.downloadWithProgress("https://example.com/large-file.zip", "/local/path/");

// Check if URL is accessible
let accessible: boolean = web.isAccessible("https://example.com");
```

## Generated Bash

Web functions compile to appropriate curl commands:

```bash
# GET request
response=$(curl -s "https://api.example.com/data")

# POST request
result=$(curl -s -X POST "https://api.example.com/create" \
  -H "Content-Type: application/json" \
  -d '{"name":"Utah","version":"1.0"}')

# Request with headers
response=$(curl -s "https://api.example.com/protected" \
  -H "Authorization: Bearer token123" \
  -H "Content-Type: application/json")

# Download file
curl -o "/local/path/file.zip" "https://example.com/file.zip"
```

## Use Cases

- API integration and testing
- Web scraping and data extraction
- File downloads and uploads
- Web service monitoring
- REST API consumption
- Webhook processing
