# Template Functions

Utah provides template functions for processing files with variable substitution. These functions use `envsubst` to replace "${VARIABLE}" placeholders with environment variable values, making them perfect for configuration file generation and dynamic content creation.

## Available Functions

### `template.update(sourceFilePath, targetFilePath)`

Process a template file and write the result with environment variable substitution.

**Parameters:**

- `sourceFilePath` (string): Path to the template file containing "${VARIABLE}" placeholders
- `targetFilePath` (string): Path where the processed file will be written

**Returns:** `string` - "true" if successful, "false" if failed (when used as expression)

**Example:**

```typescript
// Set environment variables
export APP_NAME="MyApplication"
export VERSION="1.2.3"

// Create template file
fs.writeFile("config.template", "app: ${APP_NAME}\nversion: ${VERSION}");

// Process template
template.update("config.template", "config.txt");

// Use as expression
let success = template.update("config.template", "backup.txt");
if (success === "true") {
  console.log("Template processing succeeded");
}
```

**Generated Bash:**

```bash
# As statement
envsubst < "config.template" > "config.txt"

# As expression
success=$(_utah_template_result_1=$(envsubst < "config.template" > "backup.txt" && echo "true" || echo "false"); echo ${_utah_template_result_1})
```

## Common Use Cases

### Configuration Management

Generate configuration files for different environments:

```typescript
export NODE_ENV="production"
export DATABASE_URL="prod-db.example.com"
export API_KEY="prod-key-xyz"

template.update("app.config.template", "app.config.json");
```

### CI/CD Pipeline Scripts

Generate deployment scripts with environment-specific values:

```typescript
export DEPLOYMENT_TARGET="staging"
export BUILD_VERSION="v2.1.0"
export DOCKER_REGISTRY="registry.example.com"

template.update("deploy.template.sh", "deploy.sh");
```

### Documentation Generation

Generate README files with dynamic content:

```typescript
export PROJECT_NAME="my-project"
export CURRENT_VERSION="1.0.0"
export AUTHOR_NAME="Development Team"

template.update("README.template.md", "README.md");
```

## Template File Format

Template files use standard bash variable substitution syntax:

```text
# config.template
server:
  name: ${APP_NAME}
  version: ${APP_VERSION}
  port: ${PORT:-8080}
  environment: ${NODE_ENV:-production}
```

## Error Handling

Template functions will fail if:

- Source file doesn't exist
- Target directory doesn't exist
- Insufficient permissions
- Invalid file paths

When used as expressions, the function returns "false" on failure, allowing for error handling:

```typescript
let result = template.update("missing.template", "output.txt");
if (result === "false") {
  console.log("Template processing failed");
  exit(1);
}
```

## Integration with Other Functions

Template functions work seamlessly with other Utah functions:

```typescript
// Check if template file exists before processing
if (fs.exists("config.template")) {
  template.update("config.template", "config.yml");

  // Verify the output was created
  if (fs.exists("config.yml")) {
    console.log("Configuration generated successfully");
  }
}
```
