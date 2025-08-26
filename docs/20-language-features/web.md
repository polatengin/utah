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

- `web.get(url)` - HTTP GET requests
- `web.delete(url, options?)` - HTTP DELETE requests
- `web.post(url, data, options?)` - HTTP POST requests
- `web.put(url, data, options?)` - HTTP PUT requests
- `web.speedtest(url, options?)` - Network speed testing

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
```

#### web.put(url, data, options?)

Performs an HTTP PUT request to the specified URL with data and optional headers or curl options. PUT requests are typically used to update or replace entire resources.

```typescript
// Basic PUT request to update a resource
let response: string = web.put("https://api.example.com/users/123", '{"name": "Updated Name", "email": "updated@example.com"}');
console.log(`Put Response: ${response}`);

// PUT with form data
let formResponse: string = web.put("https://api.example.com/config", "setting=value&enabled=true");
console.log(`Form Response: ${formResponse}`);

// PUT with authorization headers
let authResponse: string = web.put("https://api.example.com/users/456", '{"status": "active"}', "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'");
console.log(`Authenticated Put: ${authResponse}`);

// PUT with variables
let resourceUrl: string = "https://api.example.com/documents/789";
let documentData: string = '{"title": "Updated Document", "content": "New content here"}';
let updateResponse: string = web.put(resourceUrl, documentData);
console.log(`Update Response: ${updateResponse}`);

// PUT for configuration updates
let configUrl: string = "https://api.example.com/settings";
let configData: string = '{"theme": "dark", "notifications": true, "autoSave": false}';
let configResponse: string = web.put(configUrl, configData, "-H 'Content-Type: application/json'");
console.log(`Config Response: ${configResponse}`);
```

#### web.speedtest(url, options?)

Performs a network speed test to the specified URL and returns detailed performance metrics as JSON.

```typescript
// Basic speed test
let speedData: string = web.speedtest("https://httpbin.org/get");
console.log(`Speed test result: ${speedData}`);

// Parse results with JSON functions
let results: object = json.parse(speedData);
let downloadSpeed: string = json.get(results, ".download_speed");
let totalTime: string = json.get(results, ".time_total");
let responseCode: string = json.get(results, ".response_code");
console.log(`Download speed: ${downloadSpeed} bytes/sec in ${totalTime} seconds (HTTP ${responseCode})`);

// Speed test with timeout option
let timedTest: string = web.speedtest("https://httpbin.org/delay/1", "--max-time 5");
console.log(`Timed speed test: ${timedTest}`);

// Speed test with custom headers
let authTest: string = web.speedtest("https://httpbin.org/bearer", "-H 'Authorization: Bearer token123'");
console.log(`Authenticated speed test: ${authTest}`);

// Test multiple endpoints for performance comparison
let endpoints: string[] = ["https://httpbin.org/get", "https://www.google.com"];
for (let endpoint: string in endpoints) {
  let result: string = web.speedtest(endpoint);
  let data: object = json.parse(result);
  let speed: string = json.get(data, ".download_speed");
  let time: string = json.get(data, ".time_total");
  console.log(`${endpoint}: ${speed} bytes/sec in ${time} seconds`);
}
```

The web.speedtest() function returns a JSON object with the following metrics:

- `download_speed`: Download speed in bytes per second
- `upload_speed`: Upload speed (always "0" for GET requests)
- `time_total`: Total time for the request in seconds
- `time_connect`: Time to establish connection in seconds
- `time_pretransfer`: Time before transfer started in seconds
- `size_download`: Number of bytes downloaded
- `response_code`: HTTP response code

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

# Speed test
speedData=$(curl -w '{"download_speed":"%{speed_download}","upload_speed":"0","time_total":"%{time_total}","time_connect":"%{time_connect}","time_pretransfer":"%{time_pretransfer}","size_download":"%{size_download}","response_code":"%{response_code}"}' --silent --output /dev/null "https://httpbin.org/get" 2>/dev/null || echo '{"error":"failed"}')

# Speed test with options
timedTest=$(curl "--max-time 5" -w '{"download_speed":"%{speed_download}","upload_speed":"0","time_total":"%{time_total}","time_connect":"%{time_connect}","time_pretransfer":"%{time_pretransfer}","size_download":"%{size_download}","response_code":"%{response_code}"}' --silent --output /dev/null "https://httpbin.org/get" 2>/dev/null || echo '{"error":"failed"}')
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
- Network performance monitoring and diagnostics
- Speed testing for multiple geographic endpoints
- CI/CD pipeline performance gates
- Load testing preparation and baseline measurements
- Simple HTTP operations

## Future Functions

The following web functions are planned for future releases:

- `web.put()` - HTTP PUT requests
- `web.patch()` - HTTP PATCH requests
- URL manipulation functions
- File download utilities
