# SSH Functions

Utah provides comprehensive SSH functions for secure remote connections and command execution. These functions enable you to establish SSH connections with various authentication methods and manage remote server interactions directly within your scripts.

## Available SSH Functions

### Connection Management

- **`ssh.connect(host, options?)`** - Establish SSH connection with various authentication methods

## SSH Connection Methods

Utah supports multiple authentication methods to accommodate different security requirements and environments.

### 1. SSH Config Based Connection

The simplest method uses your existing SSH configuration:

```typescript
// Connect using SSH config entry
let connection: object = ssh.connect("myserver");

// Connect using explicit config name
let connection: object = ssh.connect("production-server", {
  configName: "production-server"
});
```

This method relies on entries in your `~/.ssh/config` file:

```
Host myserver
    HostName myserver.company.com
    User admin
    Port 22
    IdentityFile ~/.ssh/company_key

Host production-server
    HostName prod.company.com
    User deploy
    Port 2222
    IdentityFile ~/.ssh/prod_key
```

### 2. Key-Based Authentication

Use SSH private keys for secure authentication:

```typescript
// Connect with SSH key
let connection: object = ssh.connect("192.168.1.100", {
  username: "ubuntu",
  keyPath: "/home/user/.ssh/id_rsa",
  port: 22
});

// Connect with custom key and port
let connection: object = ssh.connect("myserver.com", {
  username: "admin",
  keyPath: "/secure/admin_key",
  port: 2222
});
```

### 3. Password Authentication

Use username and password for authentication (requires `sshpass`):

```typescript
// Connect with password
let connection: object = ssh.connect("192.168.1.100", {
  username: "user",
  password: "mypassword",
  port: 22
});

// Password with custom port
let connection: object = ssh.connect("legacy-server.com", {
  username: "admin",
  password: "admin123",
  port: 2222
});
```

**Note**: Password authentication requires the `sshpass` utility to be installed on your system.

## Connection Object Properties

Each SSH connection returns an object with the following properties:

| Property | Type | Description |
|----------|------|-------------|
| `host` | string | The target hostname or IP address |
| `port` | string | The SSH port (default: "22") |
| `username` | string | The SSH username for authentication |
| `authMethod` | string | Authentication method: "config", "key", or "password" |
| `connected` | string | Connection status: "true" or "false" |
| `keyPath` | string | Path to SSH private key (key authentication only) |
| `password` | string | SSH password (password authentication only) |
| `configName` | string | SSH config entry name (config authentication only) |

## Basic Usage Examples

### Simple Server Connection

```typescript
#!/usr/bin/env utah run

console.log("Connecting to web server...");

let webServer: object = ssh.connect("web.company.com", {
  username: "webadmin",
  keyPath: "/home/user/.ssh/web_key"
});

if (webServer.connected) {
  console.log("✅ Successfully connected to web server");
  console.log(`Connected to: ${webServer.host}:${webServer.port}`);
  console.log(`Username: ${webServer.username}`);
  console.log(`Auth method: ${webServer.authMethod}`);
} else {
  console.log("❌ Failed to connect to web server");
  exit(1);
}
```

### Multi-Server Management

```typescript
#!/usr/bin/env utah run

console.log("Connecting to server farm...");

let servers: string[] = [
  "web1.company.com",
  "web2.company.com", 
  "web3.company.com"
];

let connections: object[] = [];
let successCount: number = 0;

for (let server: string in servers) {
  console.log(`Connecting to ${server}...`);
  
  let connection: object = ssh.connect(server, {
    username: "admin",
    keyPath: "/home/admin/.ssh/server_key",
    port: 22
  });
  
  connections.push(connection);
  
  if (connection.connected) {
    console.log(`✅ Connected to ${server}`);
    successCount = successCount + 1;
  } else {
    console.log(`❌ Failed to connect to ${server}`);
  }
}

console.log(`Connected to ${successCount} out of ${servers.length} servers`);

if (successCount < servers.length) {
  console.log("Some servers are unreachable - check network connectivity");
}
```

### Environment-Specific Connections

```typescript
#!/usr/bin/env utah run

// Determine environment
let environment: string = console.promptText("Enter environment (dev/staging/prod)", "dev");

let connection: object;

if (environment == "dev") {
  connection = ssh.connect("dev.company.com", {
    username: "developer",
    password: "devpass123"
  });
} else if (environment == "staging") {
  connection = ssh.connect("staging.company.com", {
    username: "staging",
    keyPath: "/keys/staging_key"
  });
} else if (environment == "prod") {
  // Production uses SSH config for security
  connection = ssh.connect("production");
} else {
  console.log("Invalid environment specified");
  exit(1);
}

if (connection.connected) {
  console.log(`Connected to ${environment} environment`);
  console.log(`Host: ${connection.host}`);
  console.log(`Port: ${connection.port}`);
} else {
  console.log(`Failed to connect to ${environment} environment`);
  exit(1);
}
```

## Error Handling and Validation

### Connection Validation

```typescript
#!/usr/bin/env utah run

// Check SSH client availability
let sshAvailable: boolean = os.isInstalled("ssh");
if (!sshAvailable) {
  console.log("Error: SSH client is not installed");
  exit(1);
}

// For password authentication, check sshpass
let usePassword: boolean = console.promptYesNo("Use password authentication?");
if (usePassword) {
  let sshpassAvailable: boolean = os.isInstalled("sshpass");
  if (!sshpassAvailable) {
    console.log("Error: sshpass is required for password authentication");
    console.log("Install with: sudo apt-get install sshpass");
    exit(1);
  }
}

// Attempt connection with error handling
try {
  let connection: object;
  
  if (usePassword) {
    let password: string = console.promptPassword("Enter SSH password");
    connection = ssh.connect("myserver.com", {
      username: "user",
      password: password
    });
  } else {
    connection = ssh.connect("myserver.com", {
      username: "user",
      keyPath: "/home/user/.ssh/id_rsa"
    });
  }
  
  if (connection.connected) {
    console.log("SSH connection established successfully");
  } else {
    console.log("SSH connection failed - check credentials and network");
    exit(1);
  }
} catch {
  console.log("SSH connection error occurred");
  console.log("Check SSH configuration and network connectivity");
  exit(1);
}
```

### Key File Validation

```typescript
#!/usr/bin/env utah run

let keyPath: string = "/home/user/.ssh/id_rsa";

// Check if key file exists
let keyExists: boolean = fs.exists(keyPath);
if (!keyExists) {
  console.log(`Error: SSH key file not found at ${keyPath}`);
  console.log("Generate a key with: ssh-keygen -t rsa -b 4096");
  exit(1);
}

console.log("SSH key file found");

// Attempt connection
let connection: object = ssh.connect("myserver.com", {
  username: "user",
  keyPath: keyPath
});

if (connection.connected) {
  console.log("Connected successfully using SSH key");
} else {
  console.log("Connection failed - check key permissions and server configuration");
  console.log("Ensure key permissions are 600: chmod 600 ~/.ssh/id_rsa");
}
```

## Integration with Other Utah Functions

### JSON Configuration Management

```typescript
#!/usr/bin/env utah run

// Load server configuration from JSON
let configFile: string = "servers.json";
let configExists: boolean = fs.exists(configFile);

if (!configExists) {
  console.log("Server configuration file not found");
  exit(1);
}

let configJson: string = fs.readFile(configFile);
let config: object = json.parse(configJson);

// Extract server details
let serverHost: string = json.get(config, ".production.host");
let serverPort: string = json.get(config, ".production.port");
let serverUser: string = json.get(config, ".production.username");
let serverKey: string = json.get(config, ".production.keyPath");

console.log(`Connecting to ${serverHost}:${serverPort} as ${serverUser}`);

let connection: object = ssh.connect(serverHost, {
  username: serverUser,
  keyPath: serverKey,
  port: serverPort
});

if (connection.connected) {
  console.log("Connected to production server");
  
  // Update connection status in config
  config = json.set(config, ".production.lastConnected", "$(date)");
  let updatedConfig: string = json.stringify(config);
  fs.writeFile(configFile, updatedConfig);
} else {
  console.log("Failed to connect to production server");
  exit(1);
}
```

### Parallel Server Connections

```typescript
#!/usr/bin/env utah run

console.log("Starting parallel server health checks...");

// Connect to multiple servers in parallel
parallel ssh.connect("web1.company.com", { username: "monitor", keyPath: "/keys/monitor_key" });
parallel ssh.connect("web2.company.com", { username: "monitor", keyPath: "/keys/monitor_key" });
parallel ssh.connect("web3.company.com", { username: "monitor", keyPath: "/keys/monitor_key" });
parallel ssh.connect("db.company.com", { username: "dbmonitor", keyPath: "/keys/db_key" });

// Wait for all connections to complete
let _: string = `$(wait)`;

console.log("All connection attempts completed");
console.log("Server health check finished");
```

### Template-Based SSH Configuration

```typescript
#!/usr/bin/env utah run

// Create SSH config template
let sshConfigTemplate: string = 'Host ${SERVER_NAME}
    HostName ${SERVER_HOST}
    User ${SERVER_USER}
    Port ${SERVER_PORT}
    IdentityFile ${SERVER_KEY}
    StrictHostKeyChecking no
';

// Update template with server details
template.update("ssh-config.template", "/tmp/ssh_config");

console.log("SSH configuration updated");

// Use the generated config (future enhancement)
let connection: object = ssh.connect("myserver");

if (connection.connected) {
  console.log("Connected using generated SSH config");
} else {
  console.log("Connection failed with generated config");
}
```

## Generated Bash Code

SSH functions transpile to efficient bash commands using standard SSH tools:

### Basic Connection

```bash
#!/bin/bash

# Basic SSH connection with config
connection=$({ 
  declare -A _utah_ssh_conn_1
  _utah_ssh_conn_1[host]="myserver.com"
  _utah_ssh_conn_1[authMethod]="config"
  _utah_ssh_conn_1[port]="22"
  _utah_ssh_conn_1[username]="$(whoami)"
  
  if ssh -o ConnectTimeout=5 -o BatchMode=yes -q "${_utah_ssh_conn_1[username]}@${_utah_ssh_conn_1[host]}" -p "${_utah_ssh_conn_1[port]}" exit 2>/dev/null; then
    _utah_ssh_conn_1[connected]="true"
  else
    _utah_ssh_conn_1[connected]="false"
  fi
  
  echo "_utah_ssh_conn_1"
})
```

### Key-Based Authentication (Future Enhancement)

```bash
#!/bin/bash

# Key-based SSH connection
keyConnection=$({ 
  declare -A _utah_ssh_conn_2
  _utah_ssh_conn_2[host]="192.168.1.100"
  _utah_ssh_conn_2[username]="ubuntu"
  _utah_ssh_conn_2[keyPath]="/home/user/.ssh/id_rsa"
  _utah_ssh_conn_2[port]="22"
  _utah_ssh_conn_2[authMethod]="key"
  
  if ssh -i "${_utah_ssh_conn_2[keyPath]}" -o ConnectTimeout=5 -o BatchMode=yes -q "${_utah_ssh_conn_2[username]}@${_utah_ssh_conn_2[host]}" -p "${_utah_ssh_conn_2[port]}" exit 2>/dev/null; then
    _utah_ssh_conn_2[connected]="true"
  else
    _utah_ssh_conn_2[connected]="false"
  fi
  
  echo "_utah_ssh_conn_2"
})
```

### Password Authentication (Future Enhancement)

```bash
#!/bin/bash

# Check for sshpass availability
if ! command -v sshpass &> /dev/null; then
  echo "Error: sshpass is required for password authentication but not installed"
  exit 1
fi

# Password-based SSH connection
passwordConnection=$({ 
  declare -A _utah_ssh_conn_3
  _utah_ssh_conn_3[host]="192.168.1.100"
  _utah_ssh_conn_3[username]="ubuntu"
  _utah_ssh_conn_3[password]="mypassword"
  _utah_ssh_conn_3[port]="2222"
  _utah_ssh_conn_3[authMethod]="password"
  
  if sshpass -p "${_utah_ssh_conn_3[password]}" ssh -o ConnectTimeout=5 -o StrictHostKeyChecking=no -q "${_utah_ssh_conn_3[username]}@${_utah_ssh_conn_3[host]}" -p "${_utah_ssh_conn_3[port]}" exit 2>/dev/null; then
    _utah_ssh_conn_3[connected]="true"
  else
    _utah_ssh_conn_3[connected]="false"
  fi
  
  echo "_utah_ssh_conn_3"
})
```

## SSH Use Cases

### Server Administration

```typescript
#!/usr/bin/env utah run

let servers: string[] = ["web1", "web2", "db1", "cache1"];

console.log("Starting server maintenance check...");

for (let server: string in servers) {
  let connection: object = ssh.connect(server);
  
  if (connection.connected) {
    console.log(`✅ ${server} is accessible`);
    // Future: Check disk space, memory, etc.
  } else {
    console.log(`❌ ${server} is unreachable - requires attention`);
  }
}

console.log("Server maintenance check completed");
```

### Deployment Automation

```typescript
#!/usr/bin/env utah run

console.log("Starting application deployment...");

// Connect to deployment server
let deployServer: object = ssh.connect("deploy.company.com", {
  username: "deploy",
  keyPath: "/secure/deploy_key"
});

if (!deployServer.connected) {
  console.log("Failed to connect to deployment server");
  exit(1);
}

console.log("Connected to deployment server");

// Future enhancements:
// ssh.execute(deployServer, "cd /app && git pull");
// ssh.execute(deployServer, "docker build -t myapp .");
// ssh.execute(deployServer, "docker-compose up -d");

console.log("Deployment completed successfully");
```

### Environment Setup

```typescript
#!/usr/bin/env utah run

// Development environment setup
let devServers: string[] = ["dev-web", "dev-db", "dev-cache"];

console.log("Setting up development environment...");

for (let server: string in devServers) {
  let connection: object = ssh.connect(server, {
    username: "developer",
    keyPath: "/home/dev/.ssh/dev_key"
  });
  
  if (connection.connected) {
    console.log(`Setting up ${server}...`);
    // Future: Execute setup commands
  } else {
    console.log(`Failed to connect to ${server}`);
  }
}

console.log("Development environment setup completed");
```

## Security Best Practices

### 1. Use Key-Based Authentication

```typescript
// Prefer SSH keys over passwords
let connection: object = ssh.connect("myserver.com", {
  username: "admin",
  keyPath: "/secure/admin_key"  // More secure than passwords
});
```

### 2. Validate SSH Configuration

```typescript
// Check SSH client and dependencies
let sshInstalled: boolean = os.isInstalled("ssh");
if (!sshInstalled) {
  console.log("SSH client not found - install openssh-client");
  exit(1);
}

// Verify key file permissions
let keyPath: string = "/home/user/.ssh/id_rsa";
if (fs.exists(keyPath)) {
  console.log("SSH key found - ensure permissions are 600");
} else {
  console.log("SSH key not found - generate with ssh-keygen");
}
```

### 3. Use SSH Config Files

```typescript
// Centralize SSH configuration
let connection: object = ssh.connect("production");  // Uses ~/.ssh/config

// SSH config example:
// Host production
//     HostName prod.company.com
//     User admin
//     Port 2222
//     IdentityFile ~/.ssh/prod_key
//     StrictHostKeyChecking yes
```

### 4. Connection Monitoring

```typescript
// Monitor connection status
let connection: object = ssh.connect("myserver.com");

if (connection.connected) {
  console.log(`✅ Connected to ${connection.host}:${connection.port}`);
  console.log(`Authentication: ${connection.authMethod}`);
  console.log(`User: ${connection.username}`);
} else {
  console.log("❌ Connection failed - check logs and configuration");
}
```

## Technical Implementation

### Connection Object Structure

SSH connections are implemented as bash associative arrays:

```bash
declare -A _utah_ssh_conn_1
_utah_ssh_conn_1[host]="myserver.com"
_utah_ssh_conn_1[port]="22"
_utah_ssh_conn_1[username]="admin"
_utah_ssh_conn_1[authMethod]="config"
_utah_ssh_conn_1[connected]="true"
```

### Connection Testing

Each connection is tested before being marked as connected:

```bash
if ssh -o ConnectTimeout=5 -o BatchMode=yes -q "${username}@${host}" -p "${port}" exit 2>/dev/null; then
  connection[connected]="true"
else
  connection[connected]="false"
fi
```

### Dependencies

- **ssh**: OpenSSH client (standard on most Unix systems)
- **sshpass**: Required for password authentication (optional)

### Future Enhancements

The SSH connection system is designed for expansion with additional functions:

- `ssh.execute(connection, command)` - Execute commands on remote servers
- `ssh.upload(connection, localPath, remotePath)` - Upload files to remote servers
- `ssh.download(connection, remotePath, localPath)` - Download files from remote servers
- `ssh.disconnect(connection)` - Explicitly close SSH connections
- `ssh.tunnel(connection, localPort, remotePort)` - Create SSH tunnels

## Troubleshooting

### Common Issues

1. **Connection Timeouts**
   - Check network connectivity
   - Verify server is running and accessible
   - Confirm SSH service is running on target port

2. **Authentication Failures**
   - Verify SSH key permissions (should be 600)
   - Check SSH key is added to server's authorized_keys
   - Confirm username is correct

3. **sshpass Not Found**
   - Install sshpass: `sudo apt-get install sshpass`
   - Consider using key-based authentication instead

4. **Host Key Verification**
   - Add host to known_hosts: `ssh-keyscan hostname >> ~/.ssh/known_hosts`
   - Configure SSH to accept new host keys (less secure)

### Debug SSH Connections

```typescript
#!/usr/bin/env utah run

console.log("Testing SSH connectivity...");

// Enable debug mode for SSH troubleshooting
script.enableDebug();

let connection: object = ssh.connect("problematic-server.com", {
  username: "admin",
  keyPath: "/home/admin/.ssh/id_rsa"
});

script.disableDebug();

if (connection.connected) {
  console.log("Connection successful");
} else {
  console.log("Connection failed - check debug output above");
  console.log("Try manual connection: ssh admin@problematic-server.com");
}
```

## Examples Repository

For more SSH examples and use cases, see the Utah examples repository and the comprehensive test suite in `tests/positive_fixtures/ssh_connect.shx`.
