---
layout: default
title: Docker Integration
parent: Guides
nav_order: 5
---

Docker containerization for Utah applications.

## Overview

Docker provides a way to package Utah applications with their dependencies for consistent deployment across environments.

## Creating a Dockerfile

A basic Dockerfile for Utah applications:

```dockerfile
FROM alpine:latest

# Install bash and other dependencies
RUN apk add --no-cache bash curl git

# Copy Utah scripts
COPY scripts/ /app/scripts/
COPY utah /usr/local/bin/utah

WORKDIR /app

# Run the Utah application
CMD ["utah", "run", "main.shx"]
```

## Docker Compose

For complex applications with multiple services:

```yaml
version: '3.8'
services:
  utah-app:
    build: .
    volumes:
      - ./scripts:/app/scripts
    environment:
      - ENV=production

  database:
    image: postgres:13
    environment:
      - POSTGRES_DB=myapp
      - POSTGRES_PASSWORD=secret
```

## Best Practices

### Security

- Use minimal base images
- Run as non-root user
- Scan for vulnerabilities

### Performance

- Use multi-stage builds
- Optimize layer caching
- Minimize image size

### CI/CD Integration

- Build images in CI pipeline
- Tag with version numbers
- Push to registry

## Examples

### Health Check Script

```typescript
#!/usr/bin/env utah

script.description("Health check for containerized service");

// Check if service is healthy
function healthCheck(): void {
  try {
    let response: string = web.get("http://localhost:8080/health");
    console.log("Service is healthy");
    exit(0);
  }
  catch {
    console.log("Service is unhealthy");
    exit(1);
  }
}

healthCheck();
```

### Deployment Script

```typescript
#!/usr/bin/env utah

script.description("Deploy application using Docker");

// Deploy application using Docker
function deploy(): void {
  console.log("Building Docker image...");
  
  let buildResult: string = `$(docker build -t myapp:latest . 2>&1; echo "EXIT_CODE:$?")`;
  let resultParts: string[] = string.split(buildResult, "EXIT_CODE:");
  let exitCode: string = "1";
  if (array.length(resultParts) > 1) {
    exitCode = string.trim(resultParts[1]);
  }

  if (exitCode != "0") {
    console.log("Build failed!");
    exit(1);
  }

  console.log("Deploying container...");
  `$(docker run -d --name myapp -p 8080:8080 myapp:latest)`;

  console.log("Deployment complete!");
}

deploy();
```

## See Also

- [CI/CD Guide](cicd.md)
- [Security Best Practices](security.md)
- [Cloud Deployment](cloud.md)
