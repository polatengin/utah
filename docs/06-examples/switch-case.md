---
layout: default
title: Switch Statements
parent: Examples
nav_order: 6
description: "Conditional logic examples using switch/case statements with various data types"
permalink: /examples/switch-case/
---

Conditional logic examples using switch/case statements with various data types. This tutorial demonstrates Utah's clean and powerful conditional syntax.

## Features Demonstrated

- **String-based switch statements** for OS detection
- **Number-based switch statements** for version handling
- **Multiple case handling** with fall-through behavior
- **Default case handling** for unknown values
- **Environment-specific logic** using switch statements
- **Error handling** with switch-based responses

## Complete Script

```typescript
// Comprehensive switch/case/default test
const OS: string = "linux";
const VERSION: number = 20;
let platform: string = "";
let support: string = "";

// Test with const string
switch (OS) {
  case "windows":
    platform = "Windows Platform";
    break;
  case "macos":
    platform = "macOS Platform";
    break;
  case "linux":
    platform = "Linux Platform";
    break;
  default:
    platform = "Unknown Platform";
    break;
}

// Test with const number
switch (VERSION) {
  case 18:
  case 20:
  case 22:
    support = "LTS Version";
    break;
  case 19:
  case 21:
    support = "Standard Version";
    break;
  default:
    support = "Unsupported Version";
    break;
}

console.log(`Platform: ${platform}`);
console.log(`Support Level: ${support}`);

// Runtime variable testing
let userRole: string = env.get("USER_ROLE") ? env.get("USER_ROLE") : "guest";

switch (userRole) {
  case "admin":
    console.log("🔑 Full system access granted");
    console.log("📊 Admin dashboard available");
    break;
  case "user":
    console.log("👤 Standard user access");
    console.log("📝 Limited operations available");
    break;
  case "guest":
    console.log("👁️  Read-only access");
    console.log("ℹ️  Registration required for full access");
    break;
  default:
    console.log("❌ Invalid user role");
    console.log("🔒 Access denied");
    break;
}
```

## Switch Statement Patterns

### OS-Specific Configuration

Switch statements excel at handling different operating systems:

```typescript
let currentOS: string = os.getOS();
let packageManager: string = "";
let installCommand: string = "";

switch (currentOS) {
  case "linux":
    // Check for specific Linux distribution
    let distro: string = os.getLinuxVersion();

    switch (distro) {
      case "ubuntu":
      case "debian":
        packageManager = "apt";
        installCommand = "sudo apt install";
        break;
      case "centos":
      case "rhel":
        packageManager = "yum";
        installCommand = "sudo yum install";
        break;
      case "fedora":
        packageManager = "dnf";
        installCommand = "sudo dnf install";
        break;
      default:
        packageManager = "unknown";
        installCommand = "# Please check your distribution";
        break;
    }
    break;

  case "darwin":
    packageManager = "brew";
    installCommand = "brew install";
    break;

  case "windows":
    packageManager = "choco";
    installCommand = "choco install";
    break;

  default:
    packageManager = "unknown";
    installCommand = "# Unsupported operating system";
    break;
}

console.log(`OS: ${currentOS}`);
console.log(`Package Manager: ${packageManager}`);
console.log(`Install Command: ${installCommand}`);
```

### Service Status Management

Handle different service states with clear logic:

```typescript
let serviceStatus: string = "running"; // Could be: running, stopped, failed, unknown

switch (serviceStatus) {
  case "running":
    console.log("✅ Service is operational");
    console.log("📊 Monitoring active");
    console.log("🔄 Auto-restart enabled");
    break;

  case "stopped":
    console.log("⏹️  Service is stopped");
    console.log("🔧 Manual intervention required");
    let shouldStart: boolean = console.promptYesNo("Start service now?");

    if (shouldStart) {
      console.log("🚀 Starting service...");
      console.log("✅ Service started successfully");
    }
    break;

  case "failed":
    console.log("❌ Service has failed");
    console.log("📋 Collecting diagnostic information...");
    console.log("🔍 Check logs for error details");
    console.log("🔄 Attempting automatic recovery...");
    break;

  case "unknown":
    console.log("❓ Service status unknown");
    console.log("🔍 Performing status check...");
    console.log("📊 Gathering system information...");
    break;

  default:
    console.log(`⚠️  Unrecognized status: ${serviceStatus}`);
    console.log("🛠️  Manual diagnosis required");
    break;
}
```

### Version-Based Feature Support

Handle different software versions elegantly:

```typescript
let nodeVersion: number = 18; // Could be from: node --version parsing

switch (nodeVersion) {
  case 16:
    console.log("📦 Node.js 16 LTS (Gallium)");
    console.log("✅ Supported until April 2024");
    console.log("⚠️  Consider upgrading soon");
    break;

  case 18:
    console.log("📦 Node.js 18 LTS (Hydrogen)");
    console.log("✅ Supported until April 2025");
    console.log("🎯 Recommended for production");
    break;

  case 20:
    console.log("📦 Node.js 20 LTS (Iron)");
    console.log("✅ Supported until April 2026");
    console.log("🚀 Latest LTS version");
    break;

  case 19:
  case 21:
    console.log(`📦 Node.js ${nodeVersion} (Current)`);
    console.log("⚠️  Non-LTS version");
    console.log("🔄 Consider using LTS for stability");
    break;

  default:
    if (nodeVersion < 16) {
      console.log(`📦 Node.js ${nodeVersion} (Legacy)`);
      console.log("❌ No longer supported");
      console.log("🚨 Upgrade required immediately");
    } else {
      console.log(`📦 Node.js ${nodeVersion} (Future)`);
      console.log("🆕 Newer than current release");
      console.log("🧪 Experimental - use with caution");
    }
    break;
}
```

### Error Code Handling

Process different error conditions systematically:

```typescript
let exitCode: number = process.exitCode(); // Simulated error code

switch (exitCode) {
  case 0:
    console.log("✅ Operation completed successfully");
    console.log("📊 All systems operational");
    break;

  case 1:
    console.log("❌ General error occurred");
    console.log("🔍 Check application logs");
    console.log("🔄 Retry operation if needed");
    break;

  case 2:
    console.log("⚠️  Permission denied");
    console.log("🔑 Check user privileges");
    console.log("💡 Try running with sudo");
    break;

  case 126:
    console.log("🚫 Command cannot execute");
    console.log("📄 Check file permissions");
    console.log("🔧 Verify executable bit is set");
    break;

  case 127:
    console.log("❓ Command not found");
    console.log("📦 Check if package is installed");
    console.log("🛠️  Verify PATH environment");
    break;

  case 130:
    console.log("⏹️  Process terminated by user");
    console.log("🛑 CTRL+C signal received");
    console.log("✅ Clean shutdown performed");
    break;

  default:
    console.log(`❌ Unknown error code: ${exitCode}`);
    console.log("📚 Consult documentation");
    console.log("🆘 Contact support if needed");
    break;
}
```

## Advanced Switch Patterns

### Environment Configuration

```typescript
let environment: string = env.get("NODE_ENV") ? env.get("NODE_ENV") : "development";
let dbHost: string = "";
let dbPort: number = 0;
let logLevel: string = "";
let cacheEnabled: boolean = false;

switch (environment) {
  case "development":
    dbHost = "localhost";
    dbPort = 3306;
    logLevel = "DEBUG";
    cacheEnabled = false;

    console.log("🛠️  Development Environment");
    console.log("🔧 Debug mode enabled");
    console.log("💾 Local database");
    break;

  case "staging":
    dbHost = "staging-db.company.com";
    dbPort = 3306;
    logLevel = "INFO";
    cacheEnabled = true;

    console.log("🧪 Staging Environment");
    console.log("⚖️  Production-like testing");
    console.log("🔍 Performance monitoring");
    break;

  case "production":
    dbHost = "prod-db.company.com";
    dbPort = 3306;
    logLevel = "WARN";
    cacheEnabled = true;

    console.log("🚀 Production Environment");
    console.log("🛡️  High availability mode");
    console.log("📊 Full monitoring enabled");
    break;

  default:
    console.log(`⚠️  Unknown environment: ${environment}`);
    console.log("🔧 Falling back to development settings");

    dbHost = "localhost";
    dbPort = 3306;
    logLevel = "DEBUG";
    cacheEnabled = false;
    break;
}

console.log(`Database: ${dbHost}:${dbPort}`);
console.log(`Log Level: ${logLevel}`);
console.log(`Cache: ${cacheEnabled ? "Enabled" : "Disabled"}`);
```

### HTTP Status Code Processing

```typescript
let httpStatus: number = 404; // Example HTTP response code

switch (httpStatus) {
  case 200:
    console.log("✅ OK - Request successful");
    break;

  case 201:
    console.log("✅ Created - Resource created successfully");
    break;

  case 400:
    console.log("❌ Bad Request - Invalid syntax");
    break;

  case 401:
    console.log("🔐 Unauthorized - Authentication required");
    break;

  case 403:
    console.log("🚫 Forbidden - Access denied");
    break;

  case 404:
    console.log("❓ Not Found - Resource doesn't exist");
    break;

  case 500:
    console.log("💥 Internal Server Error - Server malfunction");
    break;

  case 502:
    console.log("🔗 Bad Gateway - Upstream server error");
    break;

  case 503:
    console.log("⏳ Service Unavailable - Temporary overload");
    break;

  default:
    if (httpStatus >= 200 && httpStatus < 300) {
      console.log(`✅ Success (${httpStatus})`);
    } else if (httpStatus >= 300 && httpStatus < 400) {
      console.log(`🔄 Redirection (${httpStatus})`);
    } else if (httpStatus >= 400 && httpStatus < 500) {
      console.log(`❌ Client Error (${httpStatus})`);
    } else if (httpStatus >= 500 && httpStatus < 600) {
      console.log(`💥 Server Error (${httpStatus})`);
    } else {
      console.log(`❓ Unknown Status (${httpStatus})`);
    }
    break;
}
```

### File Type Processing

```typescript
let fileName: string = "document.pdf";
let fileExtension: string = fs.extension(fileName);

switch (fileExtension) {
  case ".txt":
  case ".md":
  case ".log":
    console.log("📝 Text document detected");
    console.log("🔍 Content analysis available");
    console.log("✂️  Text processing enabled");
    break;

  case ".jpg":
  case ".jpeg":
  case ".png":
  case ".gif":
    console.log("🖼️  Image file detected");
    console.log("🎨 Image processing available");
    console.log("📏 Metadata extraction enabled");
    break;

  case ".pdf":
    console.log("📄 PDF document detected");
    console.log("📖 Text extraction available");
    console.log("🔒 Security check enabled");
    break;

  case ".json":
  case ".xml":
  case ".yaml":
  case ".yml":
    console.log("📊 Structured data detected");
    console.log("🔍 Schema validation available");
    console.log("🔄 Format conversion enabled");
    break;

  case ".sh":
  case ".bash":
  case ".zsh":
    console.log("⚙️  Shell script detected");
    console.log("🔍 Syntax validation available");
    console.log("⚠️  Security scan recommended");
    break;

  default:
    console.log(`❓ Unknown file type: ${fileExtension}`);
    console.log("📁 Treating as binary file");
    console.log("🔍 Basic metadata only");
    break;
}
```

## Usage Examples

### Basic Switch Demo

```bash
utah compile switch-case.shx
./switch-case.sh
```

### Expected Output

```text
Platform: Linux Platform
Support Level: LTS Version
👁️  Read-only access
ℹ️  Registration required for full access
```

### With Environment Variables

```bash
export USER_ROLE="admin"
export NODE_ENV="production"
./switch-case.sh
```

Output:

```text
Platform: Linux Platform
Support Level: LTS Version
🔑 Full system access granted
📊 Admin dashboard available
```

## Benefits Over Traditional Bash

### Utah Switch (Clean and Type-Safe)

```typescript
switch (os.getOS()) {
  case "linux":
    console.log("Linux system");
    break;
  case "darwin":
    console.log("macOS system");
    break;
  default:
    console.log("Unknown system");
    break;
}
```

### Bash Case (Verbose and Error-Prone)

```bash
case "$(uname -s)" in
  Linux*)
    echo "Linux system"
    ;;
  Darwin*)
    echo "macOS system"
    ;;
  *)
    echo "Unknown system"
    ;;
esac
```

### Key Advantages

- **Type Safety**: Switch values are strongly typed
- **Clean Syntax**: No complex case patterns or `;;` terminators
- **Better Readability**: Clear structure and intent
- **Error Prevention**: Type checking prevents mismatches
- **Consistent Behavior**: Same syntax for all data types

## Common Switch Patterns

### Multi-Value Cases

```typescript
let dayOfWeek: number = 1; // 1 = Monday, 7 = Sunday

switch (dayOfWeek) {
  case 1:
  case 2:
  case 3:
  case 4:
  case 5:
    console.log("📅 Weekday - Office hours");
    console.log("💼 Business operations active");
    break;

  case 6:
  case 7:
    console.log("📅 Weekend - Reduced hours");
    console.log("🏠 Maintenance window available");
    break;

  default:
    console.log("❌ Invalid day of week");
    break;
}
```

### Nested Switch Statements

```typescript
let serverType: string = "web";
let environment: string = "production";

switch (serverType) {
  case "web":
    console.log("🌐 Web server configuration");

    switch (environment) {
      case "production":
        console.log("  🚀 Production web server");
        console.log("  ⚡ High performance mode");
        break;
      case "staging":
        console.log("  🧪 Staging web server");
        console.log("  🔍 Testing configuration");
        break;
      default:
        console.log("  🛠️  Development web server");
        break;
    }
    break;

  case "database":
    console.log("🗄️  Database server configuration");

    switch (environment) {
      case "production":
        console.log("  🛡️  Production database");
        console.log("  💾 Backup enabled");
        break;
      default:
        console.log("  🧪 Test database");
        break;
    }
    break;
}
```

## Related Examples

- [Health Check](health-check) - Using switch for environment detection
- [Loops](loops) - Combining switches with iteration
- [String Processing](string-processing) - Switch-based text processing
