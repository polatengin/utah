---
layout: default
title: JSON and YAML Processing
parent: Functions
nav_order: 1
---

Utah provides comprehensive JSON and YAML processing capabilities that allow you to work with structured data directly iParse YAML content into an object:

```typescript
// Read YAML from a file
let yamlConfig: string = fs.readFile("app-config.yaml");

let config: object = yaml.parse(yamlConfig);
console.log("YAML configuration loaded");
``` These functions use industry-standard tools like `jq` and `yq` under the hood.

## JSON Functions

### Installation and Setup

Before using JSON functions, ensure dependencies are available:

```typescript
// Automatically install jq if not available
json.installDependencies();

// Check if jq is installed manually
if (!os.isInstalled("jq")) {
  console.log("jq is required for JSON processing");
  exit(1);
}
```

### JSON Parsing and Validation

#### `json.isValid(jsonString): boolean`

Check if a string contains valid JSON syntax:

```typescript
let data: string = "{\"name\": \"Utah\", \"version\": 1.0}";
let valid: boolean = json.isValid(data);

if (valid) {
  console.log("JSON is valid");
} else {
  console.log("Invalid JSON format");
  exit(1);
}
```

#### `json.parse(jsonString): object`

Parse a JSON string into an object:

```typescript
let jsonData: string = "{\"app\": {\"name\": \"MyApp\", \"port\": 8080}}";
let config: object = json.parse(jsonData);
console.log("Configuration loaded");
```

#### `json.stringify(jsonObject): string`

Convert a JSON object back to a string:

```typescript
let configString: string = json.stringify(config);
console.log("Config as string: " + configString);
```

### JSON Property Access

#### `json.get(jsonObject, path): string`

Get a value using jq-style path syntax:

```typescript
let userData: string = "{\"user\": {\"name\": \"Alice\", \"settings\": {\"theme\": \"dark\"}}}";
let user: object = json.parse(userData);

// Access nested properties
let userName: string = json.get(user, ".user.name");           // "Alice"
let theme: string = json.get(user, ".user.settings.theme");   // "dark"

// Access array elements
let tagsJson: string = "{\"tags\": [\"work\", \"personal\", \"urgent\"]}";
let tags: object = json.parse(tagsJson);
let firstTag: string = json.get(tags, ".tags[0]");            // "work"
let lastTag: string = json.get(tags, ".tags[-1]");           // "urgent"
```

#### `json.set(jsonObject, path, value): object`

Set a value at the specified path:

```typescript
let config: object = json.parse('{"database": {"host": "localhost"}}');

// Set simple values
config = json.set(config, ".database.port", 5432);
config = json.set(config, ".database.ssl", true);

// Set nested objects
config = json.set(config, ".auth.enabled", true);
config = json.set(config, ".auth.method", "oauth");

// Set array elements
config = json.set(config, ".features[0]", "logging");
config = json.set(config, ".features[1]", "monitoring");
```

#### `json.has(jsonObject, path): boolean`

Check if a property exists at the given path:

```typescript
let data: object = json.parse('{"user": {"name": "Alice", "email": null}}');

let hasName: boolean = json.has(data, ".user.name");     // true
let hasEmail: boolean = json.has(data, ".user.email");   // true (exists but null)
let hasPhone: boolean = json.has(data, ".user.phone");   // false
```

#### `json.delete(jsonObject, path): object`

Remove a property from the JSON object:

```typescript
let user: object = json.parse('{"name": "Alice", "temp": "remove", "age": 30}');

// Remove unwanted property
user = json.delete(user, ".temp");

// Remove nested property
user = json.delete(user, ".profile.avatar");
```

### JSON Utility Functions

#### `json.keys(jsonObject): string[]`

Get all keys from a JSON object:

```typescript
let config: object = json.parse('{"host": "localhost", "port": 8080, "ssl": true}');
let keys: string[] = json.keys(config);

for (let key: string in keys) {
  console.log("Config key: " + key);
}
// Output: host, port, ssl
```

#### `json.values(jsonObject): any[]`

Get all values from a JSON object:

```typescript
let settings: object = json.parse('{"theme": "dark", "lang": "en", "debug": true}');
let values: any[] = json.values(settings);

for (let value: any in values) {
  console.log("Setting value: " + value);
}
// Output: dark, en, true
```

#### `json.merge(jsonObject1, jsonObject2): object`

Merge two JSON objects (second object takes precedence):

```typescript
let defaults: string = "{\"timeout\": 30, \"retries\": 3, \"debug\": false}";
let userConfig: string = "{\"timeout\": 60, \"debug\": true, \"verbose\": true}";

let defaultObj: object = json.parse(defaults);
let userObj: object = json.parse(userConfig);

let finalConfig: object = json.merge(defaultObj, userObj);
// Result: {"timeout": 60, "retries": 3, "debug": true, "verbose": true}
```

## YAML Functions

YAML functions require both `yq` and `jq`:

```typescript
// Automatically install yq and jq if not available
yaml.installDependencies();

// Manual dependency check
if (!os.isInstalled("yq") || !os.isInstalled("jq")) {
  console.log("YAML functions require both yq and jq");
  exit(1);
}
```

### YAML Parsing and Validation

#### `yaml.isValid(yamlString): boolean`

Check if a string contains valid YAML syntax:

```typescript
// Read YAML from a file instead of multiline strings
let configYaml: string = fs.readFile("config.yaml");

let valid: boolean = yaml.isValid(configYaml);
if (valid) {
  console.log("YAML is valid");
}
```

#### `yaml.parse(yamlString): object`

Parse YAML string into an object:

```typescript
let yamlConfig: string = `
app:
  name: MyApp
  version: "1.0"
  settings:
    debug: true
    theme: dark
`;

let config: object = yaml.parse(yamlConfig);
console.log("YAML configuration loaded");
```

#### `yaml.stringify(yamlObject): string`

Convert object back to YAML format:

```typescript
let yamlString: string = yaml.stringify(config);
console.log("YAML output:\n" + yamlString);
```

### YAML Property Access

YAML functions use the same jq-style path syntax as JSON functions:

```typescript
// Read Kubernetes manifest from file
let k8sManifest: string = fs.readFile("deployment.yaml");
`;

let deployment: object = yaml.parse(k8sManifest);

// Access nested properties
let appName: string = yaml.get(deployment, ".metadata.name");
let replicas: number = yaml.get(deployment, ".spec.replicas");
let image: string = yaml.get(deployment, ".spec.template.spec.containers[0].image");

console.log("Deploying ${appName} with ${replicas} replicas using ${image}");

// Update deployment
deployment = yaml.set(deployment, ".spec.replicas", 5);
deployment = yaml.set(deployment, ".spec.template.spec.containers[0].image", "myapp:v2.0");

// Save updated manifest
let updatedYaml: string = yaml.stringify(deployment);
fs.writeFile("deployment-updated.yaml", updatedYaml);
```

## Practical Examples

### Configuration Management

```typescript
script.description("Configuration management script");

// Load and merge configuration files
let defaultConfigPath: string = "config/defaults.json";
let userConfigPath: string = "config/user.json";

if (!fs.exists(defaultConfigPath)) {
  console.log("Default configuration not found");
  exit(1);
}

let defaultConfig: string = fs.readFile(defaultConfigPath);
let finalConfig: object = json.parse(defaultConfig);

// Merge user configuration if exists
if (fs.exists(userConfigPath)) {
  let userConfig: string = fs.readFile(userConfigPath);
  let userObj: object = json.parse(userConfig);
  finalConfig = json.merge(finalConfig, userObj);
  console.log("Merged user configuration");
}

// Extract important settings
let dbHost: string = json.get(finalConfig, ".database.host");
let dbPort: number = json.get(finalConfig, ".database.port");
let debugMode: boolean = json.get(finalConfig, ".debug");

console.log("Database: ${dbHost}:${dbPort}");
console.log("Debug mode: ${debugMode}");

// Save final configuration
let configOutput: string = json.stringify(finalConfig);
fs.writeFile("config/final.json", configOutput);
```

### Kubernetes Deployment Automation

```typescript
script.description("Kubernetes deployment updater");

// Define command line arguments
args.define("--image", "-i", "Container image tag", "string", true);
args.define("--replicas", "-r", "Number of replicas", "number", false, 3);
args.define("--manifest", "-f", "Deployment manifest file", "string", false, "deployment.yaml");

let imageTag: string = args.get("--image");
let replicas: number = args.get("--replicas");
let manifestFile: string = args.get("--manifest");

if (!fs.exists(manifestFile)) {
  console.log("Manifest file not found: ${manifestFile}");
  exit(1);
}

// Load and parse Kubernetes manifest
let manifestContent: string = fs.readFile(manifestFile);
let deployment: object = yaml.parse(manifestContent);

// Update deployment configuration
deployment = yaml.set(deployment, ".spec.replicas", replicas);
deployment = yaml.set(deployment, ".spec.template.spec.containers[0].image", imageTag);

// Add deployment timestamp
let timestamp: string = timer.current().toString();
deployment = yaml.set(deployment, ".metadata.annotations.\"updated-at\"", timestamp);

// Save updated manifest
let updatedManifest: string = yaml.stringify(deployment);
let outputFile: string = manifestFile.replace(".yaml", "-updated.yaml");
fs.writeFile(outputFile, updatedManifest);

console.log("Updated manifest saved to: ${outputFile}");
console.log("Image: ${imageTag}");
console.log("Replicas: ${replicas}");
```

### API Response Processing

```typescript
script.description("Process API responses with JSON");

// Ensure JSON tools are available
json.installDependencies();

// Fetch data from API
let apiUrl: string = "https://api.github.com/repos/polatengin/utah";
let response: string = web.get(apiUrl);

if (!json.isValid(response)) {
  console.log("Invalid JSON response from API");
  exit(1);
}

let repoData: object = json.parse(response);

// Extract repository information
let repoName: string = json.get(repoData, ".name");
let description: string = json.get(repoData, ".description");
let stars: number = json.get(repoData, ".stargazers_count");
let language: string = json.get(repoData, ".language");
let lastUpdate: string = json.get(repoData, ".updated_at");

// Create summary object
let summary: object = json.parse("{}");
summary = json.set(summary, ".repository", repoName);
summary = json.set(summary, ".description", description);
summary = json.set(summary, ".stars", stars);
summary = json.set(summary, ".language", language);
summary = json.set(summary, ".last_updated", lastUpdate);

// Save summary
let summaryJson: string = json.stringify(summary);
fs.writeFile("repo-summary.json", summaryJson);

console.log("Repository: ${repoName}");
console.log("Stars: ${stars}");
console.log("Language: ${language}");
```

## Error Handling

### JSON Error Handling

```typescript
try {
  let data: string = fs.readFile("config.json");

  if (!json.isValid(data)) {
    console.log("Configuration file contains invalid JSON");
    exit(1);
  }

  let config: object = json.parse(data);
  let dbHost: string = json.get(config, ".database.host");

  if (dbHost == "") {
    console.log("Database host not configured");
    exit(1);
  }

  console.log("Using database: ${dbHost}");
}
catch {
  console.log("Failed to load configuration");
  exit(1);
}
```

### YAML Error Handling

```typescript
try {
  yaml.installDependencies();

  let yamlContent: string = fs.readFile("docker-compose.yml");

  if (!yaml.isValid(yamlContent)) {
    console.log("Invalid YAML in docker-compose.yml");
    exit(1);
  }

  let compose: object = yaml.parse(yamlContent);

  // Check if required services exist
  if (!yaml.has(compose, ".services.app")) {
    console.log("App service not found in docker-compose.yml");
    exit(1);
  }

  console.log("Docker Compose configuration is valid");
}
catch {
  console.log("Failed to process YAML file");
  exit(1);
}
```

## Best Practices

### 1. Always Validate Input

```typescript
// Good - validate before processing
if (json.isValid(jsonData)) {
  let obj: object = json.parse(jsonData);
  // Process safely
} else {
  console.log("Invalid JSON input");
  exit(1);
}

// Avoid - parsing without validation
let obj: object = json.parse(jsonData);  // Might fail
```

### 2. Use Dependency Installation

```typescript
// Good - ensure tools are available
json.installDependencies();
yaml.installDependencies();

// Then use functions safely
let data: object = json.parse(jsonString);
```

### 3. Handle Missing Properties Gracefully

```typescript
// Good - check before accessing
if (json.has(config, ".database.host")) {
  let host: string = json.get(config, ".database.host");
} else {
  console.log("Database host not configured");
  let host: string = "localhost";  // Use default
}
```

### 4. Use Meaningful Path Expressions

```typescript
// Good - clear and specific
let userEmail: string = json.get(userData, ".user.profile.email");
let firstTag: string = json.get(tags, ".tags[0]");

// Less clear
let email: string = json.get(userData, ".user.profile.email");
```

### 5. Save Important Data

```typescript
// Process and save results
let processedData: object = json.merge(defaults, userConfig);
let outputJson: string = json.stringify(processedData);
fs.writeFile("output/processed-config.json", outputJson);
```

## Troubleshooting

### Common Issues

1. **Missing Dependencies**:

   ```typescript
   // Solution: Use auto-install functions
   json.installDependencies();
   yaml.installDependencies();
   ```

2. **Invalid JSON/YAML**:

   ```typescript
   // Solution: Always validate first
   if (!json.isValid(data)) {
     console.log("Invalid format detected");
     exit(1);
   }
   ```

3. **Path Syntax Errors**:

   ```typescript
   // Correct jq path syntax
   json.get(obj, ".user.settings.theme");     // Object property
   json.get(obj, ".items[0]");                // Array element
   json.get(obj, ".tags[-1]");                // Last array element
   ```

## Integration with Other Functions

JSON and YAML functions work seamlessly with other Utah features:

```typescript
// With file system
let config: string = fs.readFile("app.json");
let configObj: object = json.parse(config);

// With web requests
let apiData: string = web.get("https://api.example.com/config");
let apiObj: object = json.parse(apiData);

// With environment variables
let dbHost: string = env.get("DB_HOST", "localhost");
configObj = json.set(configObj, ".database.host", dbHost);

// With parallel execution
parallel processConfigFile("config1.json");
parallel processConfigFile("config2.json");
parallel processConfigFile("config3.json");
```
