---
layout: default
title: DevOps Deployment Manager
parent: Examples
nav_order: 6
description: "Comprehensive switch statement example: a DevOps deployment pipeline manager demonstrating conditional logic in real-world scenarios"
permalink: /examples/devops-deployment-manager/
---

import { AsciinemaPlayer } from '@site/src/components';

## DevOps Deployment Manager - Comprehensive Switch Statements

A real-world example demonstrating Utah's powerful switch statement capabilities through a DevOps deployment pipeline manager. This script handles different environments, deployment strategies, service operations, and error scenarios using clean conditional logic.

## 🎬 Interactive Demo

Watch this script in action! The demo shows comprehensive switch statement usage for environment configuration, deployment strategies, service management, and resource allocation:

<AsciinemaPlayer
  src="/assets/devops-deployment-manager.cast"
  autoPlay={false}
  loop={false}
  speed={1}
  idleTimeLimit={3}
  theme="asciinema"
  poster="npt:0:01"
/>

## Features Demonstrated

- **Environment-specific deployments** with switch statements
- **Service management** using conditional logic
- **Error handling** with switch-based responses
- **Multi-level decision trees** with nested switches
- **Integration** with system commands and file operations
- **Configuration management** based on conditions
- **Pipeline orchestration** with step-by-step logic
- **Real-world DevOps workflows**

## Complete Script: `devops_manager.shx`

```typescript
// DevOps Deployment Manager - Comprehensive Switch Statement Demo
// Handles deployment pipelines, service management, and environment configuration

console.log("🚀 Utah DevOps Deployment Manager");
console.log("==================================");

// Get deployment configuration from environment
let environment: string = env.get("DEPLOY_ENV") ? env.get("DEPLOY_ENV") : "development";
let operation: string = env.get("OPERATION") ? env.get("OPERATION") : "deploy";
let serviceType: string = env.get("SERVICE_TYPE") ? env.get("SERVICE_TYPE") : "web";

console.log("📋 Configuration:");
console.log("Environment:");
console.log(environment);
console.log("Operation:");
console.log(operation);
console.log("Service Type:");
console.log(serviceType);

// Initialize deployment variables
let deploymentStrategy: string = "";
let configFile: string = "";
let databaseUrl: string = "";
let replicas: number = 1;
let resources: string = "";
let monitoring: boolean = false;

// Environment-specific configuration using switch statements
console.log("🔧 Configuring environment settings...");

switch (environment) {
  case "development":
    deploymentStrategy = "rolling";
    configFile = "config.dev.yaml";
    databaseUrl = "localhost:5432/app_dev";
    replicas = 1;
    resources = "cpu=0.5,memory=512Mi";
    monitoring = false;
    console.log("✅ Development environment configured");
    console.log("- Single replica for fast iteration");
    console.log("- Local database connection");
    console.log("- Minimal resource allocation");
    break;

  case "staging":
    deploymentStrategy = "blue-green";
    configFile = "config.staging.yaml";
    databaseUrl = "staging-db.internal:5432/app_staging";
    replicas = 2;
    resources = "cpu=1,memory=1Gi";
    monitoring = true;
    console.log("✅ Staging environment configured");
    console.log("- Blue-green deployment for testing");
    console.log("- Staging database with production-like data");
    console.log("- Moderate resource allocation");
    break;

  case "production":
    deploymentStrategy = "canary";
    configFile = "config.prod.yaml";
    databaseUrl = "prod-db-cluster.internal:5432/app_prod";
    replicas = 5;
    resources = "cpu=2,memory=4Gi";
    monitoring = true;
    console.log("✅ Production environment configured");
    console.log("- Canary deployment for safety");
    console.log("- High-availability database cluster");
    console.log("- High resource allocation");
    break;

  case "test":
    deploymentStrategy = "recreate";
    configFile = "config.test.yaml";
    databaseUrl = "test-db.internal:5432/app_test";
    replicas = 1;
    resources = "cpu=0.5,memory=256Mi";
    monitoring = false;
    console.log("✅ Test environment configured");
    console.log("- Recreate deployment for clean testing");
    console.log("- Test database with sample data");
    console.log("- Minimal resources for cost efficiency");
    break;

  default:
    console.log("❌ Unknown environment:");
    console.log(environment);
    console.log("Supported environments: development, staging, production, test");
    deploymentStrategy = "manual";
    configFile = "config.default.yaml";
    databaseUrl = "localhost:5432/app_default";
    replicas = 1;
    resources = "cpu=0.5,memory=512Mi";
    monitoring = false;
    break;
}

// Operation-specific logic using switch statements
console.log("🎯 Executing operation...");

switch (operation) {
  case "deploy":
    console.log("🚀 Starting deployment process");
    break;

  case "rollback":
    console.log("🔄 Starting rollback process");
    break;

  case "scale":
    console.log("📈 Starting scaling operation");
    break;

  case "health-check":
    console.log("🏥 Performing health check");
    break;

  case "logs":
    console.log("📜 Retrieving application logs");
    break;

  case "status":
    console.log("📊 Checking deployment status");
    break;

  default:
    console.log("❌ Unknown operation:");
    console.log(operation);
    console.log("Supported operations: deploy, rollback, scale, health-check, logs, status");
    break;
}

// Service type configuration using switch statements
console.log("⚙️  Configuring service settings...");

let servicePort: number = 8080;
let healthEndpoint: string = "/health";
let serviceProtocol: string = "HTTP";
let loadBalancer: string = "nginx";

switch (serviceType) {
  case "web":
    servicePort = 80;
    healthEndpoint = "/health";
    serviceProtocol = "HTTP";
    loadBalancer = "nginx";
    console.log("🌐 Web service configuration applied");
    break;

  case "api":
    servicePort = 8080;
    healthEndpoint = "/api/health";
    serviceProtocol = "HTTP";
    loadBalancer = "traefik";
    console.log("🔌 API service configuration applied");
    break;

  case "database":
    servicePort = 5432;
    healthEndpoint = "/health";
    serviceProtocol = "TCP";
    loadBalancer = "haproxy";
    console.log("🗄️  Database service configuration applied");
    break;

  case "cache":
    servicePort = 6379;
    healthEndpoint = "/ping";
    serviceProtocol = "TCP";
    loadBalancer = "sentinel";
    console.log("⚡ Cache service configuration applied");
    break;

  case "worker":
    servicePort = 0;
    healthEndpoint = "/metrics";
    serviceProtocol = "NONE";
    loadBalancer = "none";
    console.log("⚙️  Worker service configuration applied");
    break;

  default:
    console.log("❌ Unknown service type:");
    console.log(serviceType);
    console.log("Supported types: web, api, database, cache, worker");
    servicePort = 8080;
    healthEndpoint = "/health";
    serviceProtocol = "HTTP";
    loadBalancer = "nginx";
    break;
}

// Deployment strategy implementation
console.log("🏗️  Deployment Strategy Implementation");

switch (deploymentStrategy) {
  case "rolling":
    console.log("🔄 Implementing rolling deployment");
    console.log("- Gradually replacing instances");
    console.log("- Zero downtime deployment");
    console.log("- Safe for development and staging");
    break;

  case "blue-green":
    console.log("🔵🟢 Implementing blue-green deployment");
    console.log("- Creating parallel environment");
    console.log("- Instant traffic switch");
    console.log("- Easy rollback capability");
    break;

  case "canary":
    console.log("🐤 Implementing canary deployment");
    console.log("- Gradual traffic shifting");
    console.log("- Risk mitigation strategy");
    console.log("- Production-grade safety");
    break;

  case "recreate":
    console.log("🔄 Implementing recreate deployment");
    console.log("- Stopping all instances");
    console.log("- Clean slate deployment");
    console.log("- Suitable for testing");
    break;

  case "manual":
    console.log("👨‍💻 Manual deployment required");
    console.log("- Please review configuration");
    console.log("- Execute deployment manually");
    console.log("- Verify all settings");
    break;

  default:
    console.log("❌ Invalid deployment strategy:");
    console.log(deploymentStrategy);
    break;
}

// Resource allocation based on environment and service type
console.log("💾 Resource Allocation");

let cpuLimit: string = "";
let memoryLimit: string = "";
let storageSize: string = "";

// Combined switch logic for resource allocation
switch (environment) {
  case "production":
    switch (serviceType) {
      case "web":
        cpuLimit = "2000m";
        memoryLimit = "4Gi";
        storageSize = "20Gi";
        break;
      case "api":
        cpuLimit = "1500m";
        memoryLimit = "3Gi";
        storageSize = "10Gi";
        break;
      case "database":
        cpuLimit = "4000m";
        memoryLimit = "8Gi";
        storageSize = "100Gi";
        break;
      case "cache":
        cpuLimit = "1000m";
        memoryLimit = "2Gi";
        storageSize = "5Gi";
        break;
      default:
        cpuLimit = "1000m";
        memoryLimit = "2Gi";
        storageSize = "10Gi";
        break;
    }
    break;

  case "staging":
    switch (serviceType) {
      case "web":
      case "api":
        cpuLimit = "1000m";
        memoryLimit = "2Gi";
        storageSize = "10Gi";
        break;
      case "database":
        cpuLimit = "2000m";
        memoryLimit = "4Gi";
        storageSize = "50Gi";
        break;
      default:
        cpuLimit = "500m";
        memoryLimit = "1Gi";
        storageSize = "5Gi";
        break;
    }
    break;

  case "development":
  case "test":
    cpuLimit = "500m";
    memoryLimit = "1Gi";
    storageSize = "5Gi";
    break;

  default:
    cpuLimit = "250m";
    memoryLimit = "512Mi";
    storageSize = "1Gi";
    break;
}

console.log("Resource limits configured:");
console.log("CPU:");
console.log(cpuLimit);
console.log("Memory:");
console.log(memoryLimit);
console.log("Storage:");
console.log(storageSize);

// Monitoring and alerting configuration
if (monitoring) {
  console.log("📊 Monitoring Configuration");

  let alertSeverity: string = "info";
  let notificationChannel: string = "";
  let alertThreshold: string = "";

  switch (environment) {
    case "production":
      alertSeverity = "critical";
      notificationChannel = "pagerduty,slack,email";
      alertThreshold = "cpu>80%,memory>85%,errors>5%";
      console.log("🚨 Production monitoring configured");
      console.log("- Critical alerts enabled");
      console.log("- Multiple notification channels");
      console.log("- Strict alert thresholds");
      break;

    case "staging":
      alertSeverity = "warning";
      notificationChannel = "slack,email";
      alertThreshold = "cpu>90%,memory>90%,errors>10%";
      console.log("⚠️  Staging monitoring configured");
      console.log("- Warning level alerts");
      console.log("- Team notification channels");
      console.log("- Relaxed alert thresholds");
      break;

    default:
      alertSeverity = "info";
      notificationChannel = "email";
      alertThreshold = "cpu>95%,memory>95%,errors>20%";
      console.log("ℹ️  Basic monitoring configured");
      console.log("- Informational alerts only");
      console.log("- Email notifications");
      console.log("- High alert thresholds");
      break;
  }

  console.log("Alert configuration:");
  console.log("Severity:");
  console.log(alertSeverity);
  console.log("Channels:");
  console.log(notificationChannel);
  console.log("Thresholds:");
  console.log(alertThreshold);
}

// Security configuration based on environment
console.log("🔒 Security Configuration");

let encryptionLevel: string = "";
let authRequired: boolean = false;
let auditLogging: boolean = false;
let accessControl: string = "";

switch (environment) {
  case "production":
    encryptionLevel = "AES-256";
    authRequired = true;
    auditLogging = true;
    accessControl = "RBAC";
    console.log("🛡️  Maximum security configuration");
    console.log("- AES-256 encryption");
    console.log("- Authentication required");
    console.log("- Full audit logging");
    console.log("- Role-based access control");
    break;

  case "staging":
    encryptionLevel = "AES-128";
    authRequired = true;
    auditLogging = true;
    accessControl = "Basic";
    console.log("🔐 Standard security configuration");
    console.log("- AES-128 encryption");
    console.log("- Authentication required");
    console.log("- Audit logging enabled");
    console.log("- Basic access control");
    break;

  case "development":
  case "test":
    encryptionLevel = "None";
    authRequired = false;
    auditLogging = false;
    accessControl = "None";
    console.log("🔓 Minimal security configuration");
    console.log("- No encryption for development");
    console.log("- No authentication required");
    console.log("- No audit logging");
    console.log("- Open access for testing");
    break;

  default:
    encryptionLevel = "Basic";
    authRequired = true;
    auditLogging = false;
    accessControl = "Basic";
    console.log("🔒 Default security configuration");
    console.log("- Basic encryption");
    console.log("- Authentication required");
    console.log("- No audit logging");
    console.log("- Basic access control");
    break;
}

// Deployment summary and next steps
console.log("📋 DEPLOYMENT SUMMARY");
console.log("=====================");

console.log("Environment Details:");
console.log("Target Environment:");
console.log(environment);
console.log("Deployment Strategy:");
console.log(deploymentStrategy);
console.log("Service Type:");
console.log(serviceType);
console.log("Replicas:");
console.log(replicas);

console.log("Configuration:");
console.log("Config File:");
console.log(configFile);
console.log("Database URL:");
console.log(databaseUrl);
console.log("Service Port:");
console.log(servicePort);
console.log("Health Endpoint:");
console.log(healthEndpoint);

console.log("Resources:");
console.log("CPU Limit:");
console.log(cpuLimit);
console.log("Memory Limit:");
console.log(memoryLimit);
console.log("Storage Size:");
console.log(storageSize);

// Next steps based on operation and environment
console.log("🚀 NEXT STEPS");
console.log("=============");

switch (operation) {
  case "deploy":
    switch (environment) {
      case "production":
        console.log("1. Verify all pre-deployment checks");
        console.log("2. Execute canary deployment");
        console.log("3. Monitor metrics and alerts");
        console.log("4. Gradually increase traffic");
        console.log("5. Complete deployment or rollback");
        break;
      case "staging":
        console.log("1. Execute blue-green deployment");
        console.log("2. Run automated tests");
        console.log("3. Verify functionality");
        console.log("4. Prepare for production deployment");
        break;
      default:
        console.log("1. Execute deployment");
        console.log("2. Verify service health");
        console.log("3. Run basic tests");
        console.log("4. Monitor for issues");
        break;
    }
    break;

  case "rollback":
    console.log("1. Stop current deployment");
    console.log("2. Restore previous version");
    console.log("3. Verify service functionality");
    console.log("4. Update monitoring dashboards");
    break;

  case "scale":
    console.log("1. Calculate resource requirements");
    console.log("2. Update replica configuration");
    console.log("3. Monitor scaling progress");
    console.log("4. Verify load distribution");
    break;

  default:
    console.log("1. Review operation status");
    console.log("2. Check logs for details");
    console.log("3. Take appropriate action");
    console.log("4. Update documentation");
    break;
}

console.log("✅ DevOps Deployment Manager execution complete!");
```

## Quick Reference: Switch Statement Patterns

This comprehensive example demonstrates various switch statement patterns in Utah:

| Pattern | Use Case | Example |
|---------|----------|---------|
| **Environment Switch** | Different deployment configs | `switch (environment)` |
| **Operation Switch** | Different actions | `switch (operation)` |
| **Service Type Switch** | Service-specific settings | `switch (serviceType)` |
| **Nested Switches** | Complex decision trees | Environment → Service Type |
| **Error Handling** | Different error responses | `switch (errorCode)` |
| **Fall-through Cases** | Multiple values, same action | `case "dev": case "test":` |
| **Default Cases** | Unknown value handling | `default: // fallback` |

## Usage Examples

### Running the DevOps Manager

```bash
# Compile the script
utah compile devops_manager.shx

# Run with different configurations
DEPLOY_ENV=production OPERATION=deploy SERVICE_TYPE=api ./devops_manager.sh

# Or run directly with utah
DEPLOY_ENV=staging OPERATION=health-check utah run devops_manager.shx
```

### Environment Variables

The script accepts these environment variables:

- **`DEPLOY_ENV`**: `development`, `staging`, `production`, `test`
- **`OPERATION`**: `deploy`, `rollback`, `scale`, `health-check`, `logs`, `status`
- **`SERVICE_TYPE`**: `web`, `api`, `database`, `cache`, `worker`

### Expected Output

```text
🚀 Utah DevOps Deployment Manager
==================================

📋 Configuration:
Environment:
production
Operation:
deploy
Service Type:
api

🔧 Configuring environment settings...
✅ Production environment configured
- Canary deployment for safety
- High-availability database cluster
- High resource allocation

🎯 Executing operation...
🚀 Starting deployment process

⚙️  Configuring service settings...
🔌 API service configuration applied

🏗️  Deployment Strategy Implementation
🐤 Implementing canary deployment
- Gradual traffic shifting
- Risk mitigation strategy
- Production-grade safety

💾 Resource Allocation
Resource limits configured:
CPU:
1500m
Memory:
3Gi
Storage:
10Gi

📊 Monitoring Configuration
🚨 Production monitoring configured
- Critical alerts enabled
- Multiple notification channels
- Strict alert thresholds

🔒 Security Configuration
🛡️  Maximum security configuration
- AES-256 encryption
- Authentication required
- Full audit logging
- Role-based access control

📋 DEPLOYMENT SUMMARY
=====================
[Complete deployment summary with all configurations]

🚀 NEXT STEPS
=============
1. Verify all pre-deployment checks
2. Execute canary deployment
3. Monitor metrics and alerts
4. Gradually increase traffic
5. Complete deployment or rollback

✅ DevOps Deployment Manager execution complete!
```

## Key Switch Statement Concepts in Utah

### Simple Switch Statements

Basic conditional logic with clear case handling:

```typescript
let status: string = "healthy";

switch (status) {
  case "healthy":
    console.log("Service is running normally");
    break;
  case "degraded":
    console.log("Service performance is reduced");
    break;
  case "down":
    console.log("Service is unavailable");
    break;
  default:
    console.log("Unknown service status");
    break;
}
```

### Fall-through Cases

Multiple values that trigger the same logic:

```typescript
let day: string = "Saturday";

switch (day) {
  case "Monday":
  case "Tuesday":
  case "Wednesday":
  case "Thursday":
  case "Friday":
    console.log("Weekday - Deploy during business hours");
    break;
  case "Saturday":
  case "Sunday":
    console.log("Weekend - Deploy during maintenance window");
    break;
  default:
    console.log("Invalid day");
    break;
}
```

### Nested Switch Statements

Complex decision trees for multi-dimensional logic:

```typescript
let environment: string = "production";
let serviceType: string = "database";

switch (environment) {
  case "production":
    switch (serviceType) {
      case "web":
        console.log("Production web service - High availability");
        break;
      case "database":
        console.log("Production database - Maximum security");
        break;
      default:
        console.log("Production default service");
        break;
    }
    break;
  case "staging":
    console.log("Staging environment configuration");
    break;
  default:
    console.log("Default environment configuration");
    break;
}
```

### Integration with Other Utah Features

Switch statements work seamlessly with other Utah features:

```typescript
// Environment variables
let env: string = env.get("NODE_ENV") ? env.get("NODE_ENV") : "development";

// File system operations
let configExists: boolean = fs.exists("/etc/app/config.yaml");

// Arrays for configuration
let environments: string[] = ["dev", "staging", "prod"];

// String operations
let upperEnv: string = string.toUpperCase(env);

switch (upperEnv) {
  case "DEVELOPMENT":
    if (configExists) {
      console.log("Development config found");
    }
    break;
  case "PRODUCTION":
    console.log("Production environment detected");
    break;
  default:
    console.log("Unknown environment");
    break;
}
```

## Advanced Switch Statement Patterns

### 1. Configuration Management Pattern

```typescript
let deploymentType: string = "microservices";
let configStrategy: string = "";

switch (deploymentType) {
  case "monolith":
    configStrategy = "single-config";
    break;
  case "microservices":
    configStrategy = "service-mesh";
    break;
  case "serverless":
    configStrategy = "function-config";
    break;
  default:
    configStrategy = "default-config";
    break;
}
```

### 2. Error Recovery Pattern

```typescript
let errorSeverity: string = "critical";
let recoveryStrategy: string = "";

switch (errorSeverity) {
  case "low":
    recoveryStrategy = "log-and-continue";
    break;
  case "medium":
    recoveryStrategy = "retry-with-backoff";
    break;
  case "high":
    recoveryStrategy = "failover-to-backup";
    break;
  case "critical":
    recoveryStrategy = "immediate-shutdown";
    break;
  default:
    recoveryStrategy = "manual-intervention";
    break;
}
```

### 3. Resource Scaling Pattern

```typescript
let loadLevel: string = "medium";
let scalingAction: string = "";

switch (loadLevel) {
  case "low":
    scalingAction = "scale-down";
    break;
  case "medium":
    scalingAction = "maintain";
    break;
  case "high":
    scalingAction = "scale-up";
    break;
  case "critical":
    scalingAction = "emergency-scale";
    break;
  default:
    scalingAction = "manual-review";
    break;
}
```

## Benefits Over Traditional Bash Conditionals

### Utah Switch Statements (Clean and Readable)

```typescript
switch (environment) {
  case "development":
    console.log("Dev environment");
    break;
  case "production":
    console.log("Prod environment");
    break;
  default:
    console.log("Unknown environment");
    break;
}
```

### Bash Conditionals (Verbose and Error-Prone)

```bash
if [ "$environment" = "development" ]; then
  echo "Dev environment"
elif [ "$environment" = "production" ]; then
  echo "Prod environment"
else
  echo "Unknown environment"
fi
```

### Key Advantages

- **Cleaner Syntax**: More readable than if-elif chains
- **Better Performance**: Optimized for multiple conditions
- **Easier Maintenance**: Clear structure for complex logic
- **Fall-through Support**: Multiple cases with same action
- **Type Safety**: Compile-time checking for valid cases
- **IDE Support**: Better autocomplete and error detection

## Real-World Applications

This DevOps deployment manager pattern can be adapted for:

- **CI/CD Pipelines**: Different stages and environments
- **Service Management**: Start, stop, restart, scale operations
- **Configuration Management**: Environment-specific settings
- **Error Handling**: Different response strategies
- **Resource Management**: Dynamic resource allocation
- **Monitoring**: Alert severity and response actions
- **Security**: Access control and encryption levels
- **Infrastructure**: Cloud provider-specific configurations

## Simple Example - Getting Started

If you're new to Utah switch statements, start with this basic example:

```typescript
// Basic switch statement example
let grade: string = "B";

switch (grade) {
  case "A":
    console.log("Excellent work!");
    break;
  case "B":
    console.log("Good job!");
    break;
  case "C":
    console.log("Average performance");
    break;
  case "D":
    console.log("Needs improvement");
    break;
  case "F":
    console.log("Failed");
    break;
  default:
    console.log("Invalid grade");
    break;
}
```

## Related Examples

- [Log File Analyzer](log-file-analyzer) - Arrays with conditional logic
- [String Processing](string-processing) - Text processing with switches
- [System Health Monitor](system-health-monitor) - Comprehensive system management

## Extension Ideas

You can extend this example to include:

- **Database migrations** with environment-specific logic
- **API version handling** with backward compatibility
- **Multi-cloud deployments** with provider-specific configurations
- **Container orchestration** with different runtime environments
- **Security scanning** with severity-based responses
- **Performance optimization** with load-based scaling
- **Disaster recovery** with failure scenario handling
