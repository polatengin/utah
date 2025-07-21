---
layout: default
title: CI/CD Integration
parent: Guides
nav_order: 6
---

Deploy Utah applications to cloud platforms.

## Overview

Utah applications can be deployed to various cloud platforms including AWS, Google Cloud, Azure, and others.

## AWS Deployment

### Lambda Functions

Deploy Utah scripts as AWS Lambda functions:

```shx
#!/usr/bin/env utah

// AWS Lambda handler
const lambdaHandler = (event, context) => {
    const result = processEvent(event)
    return {
        statusCode: 200,
        body: JSON.stringify(result)
    }
}

const processEvent = (event) => {
    // Process the Lambda event
    return { message: "Hello from Utah!" }
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

```shx
#!/usr/bin/env utah

// Cloud Function entry point
const cloudFunction = (req, res) => {
    const result = processRequest(req)
    res.json(result)
}

const processRequest = (req) => {
    return { message: "Hello from Utah on GCP!" }
}
```

### Compute Engine

Deploy on Compute Engine instances:

```shx
#!/usr/bin/env utah

// GCE deployment script
const deployToGCE = () => {
    console.log("Deploying to Google Compute Engine...")

    const instanceName = "utah-app-instance"
    const zone = "us-central1-a"

    // Create instance
    const createCmd = `gcloud compute instances create ${instanceName}
        --zone=${zone}
        --machine-type=e2-micro
        --image-family=ubuntu-2004-lts
        --image-project=ubuntu-os-cloud`

    os.exec(createCmd)

    console.log("Instance created successfully!")
}

deployToGCE()
```

## Azure Deployment

### Azure Functions

Deploy as Azure Functions:

```shx
#!/usr/bin/env utah

// Azure Function handler
const azureFunction = (context, req) => {
    const result = processAzureRequest(req)
    context.res = {
        status: 200,
        body: result
    }
}

const processAzureRequest = (req) => {
    return { message: "Hello from Utah on Azure!" }
}
```

### Azure Container Instances

Deploy using ACI:

```shx
#!/usr/bin/env utah

// Deploy to Azure Container Instances
const deployToACI = () => {
    console.log("Deploying to Azure Container Instances...")

    const resourceGroup = "utah-app-rg"
    const containerName = "utah-app"
    const image = "myapp:latest"

    const deployCmd = `az container create
        --resource-group ${resourceGroup}
        --name ${containerName}
        --image ${image}
        --dns-name-label utah-app-unique
        --ports 8080`

    os.exec(deployCmd)

    console.log("Container deployed successfully!")
}

deployToACI()
```

## Best Practices

### Environment Configuration

Use environment variables for configuration:

```shx
#!/usr/bin/env utah

const config = {
    port: env.get("PORT", "8080"),
    dbUrl: env.get("DATABASE_URL", "localhost:5432"),
    apiKey: env.get("API_KEY", "")
}

if (!config.apiKey) {
    console.log("API_KEY environment variable is required")
    exit(1)
}
```

### Health Checks

Implement health check endpoints:

```shx
#!/usr/bin/env utah

const healthCheck = () => {
    const checks = [
        checkDatabase(),
        checkExternalService(),
        checkFileSystem()
    ]

    const allHealthy = checks.every(check => check.healthy)

    if (allHealthy) {
        return { status: "healthy", checks }
    } else {
        return { status: "unhealthy", checks }
    }
}

const checkDatabase = () => {
    // Database connectivity check
    return { name: "database", healthy: true }
}

const checkExternalService = () => {
    // External service check
    return { name: "external-api", healthy: true }
}

const checkFileSystem = () => {
    // File system check
    return { name: "filesystem", healthy: true }
}
```

### Monitoring and Logging

Set up monitoring:

```shx
#!/usr/bin/env utah

const setupMonitoring = () => {
    // Configure logging
    const logLevel = env.get("LOG_LEVEL", "info")
    console.log(`Setting log level to: ${logLevel}`)

    // Set up metrics collection
    const metricsEndpoint = env.get("METRICS_ENDPOINT", "")
    if (metricsEndpoint) {
        console.log(`Metrics will be sent to: ${metricsEndpoint}`)
    }

    // Configure alerts
    const alertsEnabled = env.get("ALERTS_ENABLED", "true") === "true"
    console.log(`Alerts enabled: ${alertsEnabled}`)
}

setupMonitoring()
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
