---
layout: default
title: Cloud Integration
parent: Guides
nav_order: 6
---

Deploy Utah applications to cloud platforms.

## Overview

Utah applications can be deployed to various cloud platforms including Azure, AWS, Google Cloud, and others.

## AWS Deployment

### Lambda Functions

Deploy Utah scripts as AWS Lambda functions:

```typescript
#!/usr/bin/env utah

script.description("AWS Lambda handler for Utah applications");

// AWS Lambda handler
function lambdaHandler(event: object, context: object): object {
  let result: object = processEvent(event);
  return {
    "statusCode": 200,
    "body": json.stringify(result)
  };
}

function processEvent(event: object): object {
  // Process the Lambda event
  return { "message": "Hello from Utah!" };
}
```

### EC2 Instances

Deploy on EC2 with user data script:

```bash
#!/bin/bash
# Install Utah
curl -fsSL https://get.utah.sh | sh

# Deploy application
utah /app/deploy.shx
```

### ECS Containers

Use Docker containers with ECS:

```yaml
version: '3.8'
services:
  utah-app:
    image: myapp:latest
    cpu: 256
    memory: 512
    essential: true
    portMappings:
      - containerPort: 8080
        hostPort: 8080
```

## Google Cloud Platform

### Cloud Functions

Deploy as Google Cloud Functions:

```typescript
#!/usr/bin/env utah

script.description("Google Cloud Function handler");

// Cloud Function entry point
function cloudFunction(req: object, res: object): void {
  let result: object = processRequest(req);
  // In actual implementation, this would use GCP response mechanisms
  console.log("Response: " + json.stringify(result));
}

function processRequest(req: object): object {
  return { "message": "Hello from Utah on GCP!" };
}
```

### Compute Engine

Deploy on Compute Engine instances:

```typescript
#!/usr/bin/env utah

script.description("GCE deployment script");

// GCE deployment script
function deployToGCE(): void {
  console.log("Deploying to Google Compute Engine...");

  let instanceName: string = "utah-app-instance";
  let zone: string = "us-central1-a";

  // Create instance
  let createCmd: string = "gcloud compute instances create ${instanceName} --zone=${zone} --machine-type=e2-micro --image-family=ubuntu-2004-lts --image-project=ubuntu-os-cloud";

  `$(${createCmd})`;

  console.log("Instance created successfully!");
}

deployToGCE();
```

## Azure Deployment

### Azure Functions

Deploy as Azure Functions:

```typescript
#!/usr/bin/env utah

script.description("Azure Function handler");

// Azure Function handler
function azureFunction(context: object, req: object): void {
  let result: object = processAzureRequest(req);
  // In actual implementation, this would use Azure response mechanisms
  console.log("Response: " + json.stringify(result));
}

function processAzureRequest(req: object): object {
  return { "message": "Hello from Utah on Azure!" };
}
```

### Azure Container Instances

Deploy using ACI:

```typescript
#!/usr/bin/env utah

script.description("Deploy to Azure Container Instances");

// Deploy to Azure Container Instances
function deployToACI(): void {
  console.log("Deploying to Azure Container Instances...");

  let resourceGroup: string = "utah-app-rg";
  let containerName: string = "utah-app";
  let image: string = "myapp:latest";

  let deployCmd: string = "az container create --resource-group ${resourceGroup} --name ${containerName} --image ${image} --dns-name-label utah-app-unique --ports 8080";

  `$(${deployCmd})`;

  console.log("Container deployed successfully!");
}

deployToACI();
```

## Best Practices

### Environment Configuration

Use environment variables for configuration:

```typescript
#!/usr/bin/env utah

script.description("Environment configuration for cloud deployment");

let port: string = env.get("PORT") || "8080";
let dbUrl: string = env.get("DATABASE_URL") || "localhost:5432";
let apiKey: string = env.get("API_KEY") || "";

if (apiKey == "") {
  console.log("API_KEY environment variable is required");
  exit(1);
}

console.log("Configuration loaded:");
console.log("Port: ${port}");
console.log("Database URL: ${dbUrl}");
console.log("API Key: [REDACTED]");
```

### Health Checks

Implement health check endpoints:

```typescript
#!/usr/bin/env utah

script.description("Health check implementation");

function healthCheck(): object {
  let checks: object[] = [
    checkDatabase(),
    checkExternalService(),
    checkFileSystem()
  ];

  let allHealthy: boolean = true;
  for (let check: object in checks) {
    let healthStatus: string = json.get(check, ".healthy");
    if (healthStatus != "true") {
      allHealthy = false;
    }
  }

  if (allHealthy) {
    return { "status": "healthy", "checks": checks };
  } else {
    return { "status": "unhealthy", "checks": checks };
  }
}

function checkDatabase(): object {
  // Database connectivity check
  return { "name": "database", "healthy": true };
}

function checkExternalService(): object {
  // External service check
  return { "name": "external-api", "healthy": true };
}

function checkFileSystem(): object {
  // File system check
  return { "name": "filesystem", "healthy": true };
}
```

### Monitoring and Logging

Set up monitoring:

```typescript
#!/usr/bin/env utah

script.description("Setup monitoring and logging");

function setupMonitoring(): void {
  // Configure logging
  let logLevel: string = env.get("LOG_LEVEL") || "info";
  console.log("Setting log level to: ${logLevel}");

  // Set up metrics collection
  let metricsEndpoint: string = env.get("METRICS_ENDPOINT") || "";
  if (metricsEndpoint != "") {
    console.log("Metrics will be sent to: ${metricsEndpoint}");
  }

  // Configure alerts
  let alertsEnabledStr: string = env.get("ALERTS_ENABLED") || "true";
  let alertsEnabled: boolean = alertsEnabledStr == "true";
  console.log("Alerts enabled: ${alertsEnabled}");
}

setupMonitoring();
```

## CI/CD Integration

### GitHub Actions

Example GitHub Actions workflow:

```yaml
name: Deploy to Cloud

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v2

    - name: Setup Utah
      run: curl -fsSL https://get.utah.sh | sh

    - name: Build
      run: utah compile src/main.shx

    - name: Deploy
      run: utah deploy/cloud.shx
      env:
        CLOUD_API_KEY: ${{ secrets.CLOUD_API_KEY }}
```

### GitLab CI

Example GitLab CI configuration:

```yaml
deploy:
  image: alpine:latest

  before_script:
    - apk add --no-cache bash curl
    - curl -fsSL https://get.utah.sh | sh

  script:
    - utah deploy/cloud.shx

  environment:
    name: production
    url: https://myapp.example.com

  only:
    - main
```

## See Also

- [Docker Integration](docker.md)
- [CI/CD Guide](cicd.md)
- [Security Best Practices](security.md)
- [Parallel Processing](parallel.md)
