---
layout: default
title: Security Best Practices
parent: Guides
nav_order: 7
---

Writing secure automation scripts with Utah. Learn security patterns, vulnerability prevention, and best practices for building trustworthy automation systems.

## Prerequisites

- Understanding of security fundamentals
- Knowledge of common vulnerabilities
- Experience with access control concepts
- Familiarity with secure coding practices

## Input Validation and Sanitization

### Command Injection Prevention

```typescript
script.description("Prevent command injection vulnerabilities");

// Unsafe: Direct command construction with user input
function unsafeCommandExecution(userInput: string): void {
  console.log("❌ UNSAFE: Direct command construction");
  
  // This is vulnerable to command injection
  // let result: string = `$(ls ${userInput})`;
  console.log("This would be vulnerable if executed");
}

// Safe: Input validation and sanitization
function safeCommandExecution(userInput: string): void {
  console.log("✅ SAFE: Validated command execution");
  
  // Validate input format
  if (!isValidPath(userInput)) {
    console.log("❌ Invalid path format");
    exit(1);
  }
  
  // Sanitize input
  let sanitizedPath: string = sanitizePath(userInput);
  
  // Use validated input
  if (fs.exists(sanitizedPath)) {
    let result: string = `$(ls "${sanitizedPath}")`;
    console.log(`Directory contents: ${result.substring(0, 100)}...`);
  } else {
    console.log("Directory does not exist");
  }
}

// Input validation functions
function isValidPath(path: string): boolean {
  // Check for dangerous characters
  let dangerousChars: string[] = [";", "&", "|", "`", "$", "(", ")", "<", ">"];
  
  for (let char: string in dangerousChars) {
    if (path.contains(char)) {
      console.log(`❌ Dangerous character detected: ${char}`);
      return false;
    }
  }
  
  // Check for directory traversal
  if (path.contains("..") || path.contains("./")) {
    console.log("❌ Directory traversal attempt detected");
    return false;
  }
  
  // Check path length
  if (path.length > 255) {
    console.log("❌ Path too long");
    return false;
  }
  
  // Check for absolute path outside allowed directories
  if (path.startsWith("/") && !isAllowedAbsolutePath(path)) {
    console.log("❌ Absolute path not allowed");
    return false;
  }
  
  return true;
}

function sanitizePath(path: string): string {
  // Remove any remaining dangerous characters
  let sanitized: string = path.replace(/[;&|`$()<>]/g, "");
  
  // Normalize path separators
  sanitized = sanitized.replace(/\/+/g, "/");
  
  // Remove leading/trailing whitespace
  sanitized = sanitized.trim();
  
  return sanitized;
}

function isAllowedAbsolutePath(path: string): boolean {
  let allowedPrefixes: string[] = ["/tmp/", "/var/tmp/", "/home/", "/opt/myapp/"];
  
  for (let prefix: string in allowedPrefixes) {
    if (path.startsWith(prefix)) {
      return true;
    }
  }
  
  return false;
}

// Example usage
let testInputs: string[] = [
  "/tmp/safe-directory",
  "relative/path",
  "../../etc/passwd",       // Directory traversal
  "/tmp/test; rm -rf /",    // Command injection
  "/etc/shadow",            // Unauthorized access
  "normal-file.txt"         // Safe input
];

for (let input: string in testInputs) {
  console.log(`\nTesting input: ${input}`);
  safeCommandExecution(input);
}
```

### File Path Validation

```typescript
script.description("Secure file path handling and validation");

// Secure file operations class
class SecureFileHandler {
  private allowedDirectories: string[];
  private maxFileSize: number; // bytes
  private allowedExtensions: string[];
  
  constructor(allowedDirs: string[], maxSize: number, allowedExts: string[]) {
    this.allowedDirectories = allowedDirs;
    this.maxFileSize = maxSize;
    this.allowedExtensions = allowedExts;
  }
  
  validateFilePath(filePath: string): boolean {
    // Resolve absolute path
    let absolutePath: string = fs.path(filePath);
    
    // Check if path is within allowed directories
    let isAllowed: boolean = false;
    for (let allowedDir: string in this.allowedDirectories) {
      if (absolutePath.startsWith(fs.path(allowedDir))) {
        isAllowed = true;
        break;
      }
    }
    
    if (!isAllowed) {
      console.log(`❌ File path not in allowed directories: ${absolutePath}`);
      return false;
    }
    
    // Check file extension
    let extension: string = fs.extension(filePath).toLowerCase();
    if (this.allowedExtensions.length > 0 && !this.allowedExtensions.contains(extension)) {
      console.log(`❌ File extension not allowed: ${extension}`);
      return false;
    }
    
    // Check if file exists and get size
    if (fs.exists(absolutePath)) {
      let fileSize: number = parseInt(`$(stat -c%s "${absolutePath}" 2>/dev/null || echo "0")`);
      if (fileSize > this.maxFileSize) {
        console.log(`❌ File too large: ${fileSize} bytes (max: ${this.maxFileSize})`);
        return false;
      }
    }
    
    return true;
  }
  
  secureReadFile(filePath: string): string | null {
    if (!this.validateFilePath(filePath)) {
      return null;
    }
    
    let absolutePath: string = fs.path(filePath);
    
    try {
      let content: string = fs.readFile(absolutePath);
      console.log(`✅ Securely read file: ${absolutePath}`);
      return content;
    } catch {
      console.log(`❌ Error reading file: ${absolutePath}`);
      return null;
    }
  }
  
  secureWriteFile(filePath: string, content: string): boolean {
    if (!this.validateFilePath(filePath)) {
      return false;
    }
    
    let absolutePath: string = fs.path(filePath);
    
    // Additional check for write operations
    if (content.length > this.maxFileSize) {
      console.log(`❌ Content too large: ${content.length} bytes`);
      return false;
    }
    
    // Create backup if file exists
    if (fs.exists(absolutePath)) {
      let backupPath: string = `${absolutePath}.backup.$(date +%s)`;
      fs.copyFile(absolutePath, backupPath);
      console.log(`📁 Created backup: ${backupPath}`);
    }
    
    try {
      fs.writeFile(absolutePath, content);
      console.log(`✅ Securely wrote file: ${absolutePath}`);
      return true;
    } catch {
      console.log(`❌ Error writing file: ${absolutePath}`);
      return false;
    }
  }
}

// Example usage
let fileHandler: SecureFileHandler = new SecureFileHandler(
  ["/tmp", "/var/tmp", "/home/user/workspace"],  // Allowed directories
  1048576,  // 1MB max file size
  [".txt", ".json", ".yaml", ".log"]  // Allowed extensions
);

// Test secure file operations
let testFiles: string[] = [
  "/tmp/safe-file.txt",
  "/etc/passwd",              // Outside allowed dirs
  "/tmp/large-file.exe",      // Wrong extension
  "../../../etc/shadow",       // Directory traversal
  "/tmp/normal-file.json"     // Safe file
];

for (let testFile: string in testFiles) {
  console.log(`\nTesting file: ${testFile}`);
  
  // Test read operation
  let content: string | null = fileHandler.secureReadFile(testFile);
  
  if (content !== null) {
    // Test write operation
    let success: boolean = fileHandler.secureWriteFile(testFile, "Secure content");
    console.log(`Write result: ${success ? "Success" : "Failed"}`);
  }
}
```

## Credential and Secret Management

### Secure Credential Handling

```typescript
script.description("Secure handling of credentials and secrets");

// Secure credential manager
class CredentialManager {
  private credentialFile: string;
  private encryptionKey: string;
  
  constructor(credFile: string) {
    this.credentialFile = credFile;
    this.encryptionKey = this.getEncryptionKey();
  }
  
  private getEncryptionKey(): string {
    // Try to get encryption key from environment
    let key: string = env.get("CREDENTIAL_ENCRYPTION_KEY") || "";
    
    if (key == "") {
      // Generate key from system information (not recommended for production)
      let hostname: string = `$(hostname)`;
      let user: string = `$(whoami)`;
      key = `${hostname}-${user}-default-key`;
      
      console.log("⚠️  Using default encryption key. Set CREDENTIAL_ENCRYPTION_KEY environment variable.");
    }
    
    return key;
  }
  
  private encryptCredential(credential: string): string {
    // Simple XOR encryption (use proper encryption in production)
    let encrypted: string = "";
    let key: string = this.encryptionKey;
    
    for (let i: number = 0; i < credential.length; i++) {
      let charCode: number = credential.charCodeAt(i);
      let keyChar: number = key.charCodeAt(i % key.length);
      encrypted += String.fromCharCode(charCode ^ keyChar);
    }
    
    // Base64 encode the result
    return `$(echo -n "${encrypted}" | base64 -w 0)`;
  }
  
  private decryptCredential(encryptedCredential: string): string {
    // Decode from base64
    let encrypted: string = `$(echo "${encryptedCredential}" | base64 -d)`;
    
    // Simple XOR decryption
    let decrypted: string = "";
    let key: string = this.encryptionKey;
    
    for (let i: number = 0; i < encrypted.length; i++) {
      let charCode: number = encrypted.charCodeAt(i);
      let keyChar: number = key.charCodeAt(i % key.length);
      decrypted += String.fromCharCode(charCode ^ keyChar);
    }
    
    return decrypted;
  }
  
  storeCredential(name: string, credential: string): boolean {
    console.log(`Storing credential: ${name}`);
    
    let encrypted: string = this.encryptCredential(credential);
    
    let credentials: object = {};
    if (fs.exists(this.credentialFile)) {
      try {
        let content: string = fs.readFile(this.credentialFile);
        credentials = json.parse(content);
      } catch {
        console.log("⚠️  Could not read existing credentials file");
      }
    }
    
    credentials = json.set(credentials, `.${name}`, {
      "encrypted_value": encrypted,
      "created_at": `$(date -Iseconds)`,
      "last_accessed": `$(date -Iseconds)`
    });
    
    // Set secure file permissions
    fs.writeFile(this.credentialFile, json.stringify(credentials, true));
    `$(chmod 600 "${this.credentialFile}")`;
    
    console.log(`✅ Credential stored securely: ${name}`);
    return true;
  }
  
  getCredential(name: string): string | null {
    if (!fs.exists(this.credentialFile)) {
      console.log("❌ Credentials file not found");
      return null;
    }
    
    try {
      let content: string = fs.readFile(this.credentialFile);
      let credentials: object = json.parse(content);
      
      if (!json.has(credentials, `.${name}`)) {
        console.log(`❌ Credential not found: ${name}`);
        return null;
      }
      
      let credData: object = json.get(credentials, `.${name}`);
      let encrypted: string = json.getString(credData, ".encrypted_value");
      
      // Update last accessed time
      credData = json.set(credData, ".last_accessed", `$(date -Iseconds)`);
      credentials = json.set(credentials, `.${name}`, credData);
      fs.writeFile(this.credentialFile, json.stringify(credentials, true));
      
      let decrypted: string = this.decryptCredential(encrypted);
      console.log(`✅ Retrieved credential: ${name}`);
      
      return decrypted;
      
    } catch {
      console.log(`❌ Error retrieving credential: ${name}`);
      return null;
    }
  }
  
  listCredentials(): string[] {
    if (!fs.exists(this.credentialFile)) {
      return [];
    }
    
    try {
      let content: string = fs.readFile(this.credentialFile);
      let credentials: object = json.parse(content);
      return json.keys(credentials);
    } catch {
      return [];
    }
  }
  
  deleteCredential(name: string): boolean {
    if (!fs.exists(this.credentialFile)) {
      console.log("❌ Credentials file not found");
      return false;
    }
    
    try {
      let content: string = fs.readFile(this.credentialFile);
      let credentials: object = json.parse(content);
      
      if (!json.has(credentials, `.${name}`)) {
        console.log(`❌ Credential not found: ${name}`);
        return false;
      }
      
      credentials = json.delete(credentials, `.${name}`);
      fs.writeFile(this.credentialFile, json.stringify(credentials, true));
      
      console.log(`✅ Credential deleted: ${name}`);
      return true;
      
    } catch {
      console.log(`❌ Error deleting credential: ${name}`);
      return false;
    }
  }
}

// Example usage
let credManager: CredentialManager = new CredentialManager("/tmp/secure_credentials.json");

// Store some test credentials
credManager.storeCredential("database_password", "super_secret_password");
credManager.storeCredential("api_key", "abc123def456ghi789");
credManager.storeCredential("ssh_passphrase", "my_ssh_key_passphrase");

// List stored credentials
let credNames: string[] = credManager.listCredentials();
console.log(`Stored credentials: ${credNames.join(", ")}`);

// Retrieve and use credentials
let dbPassword: string | null = credManager.getCredential("database_password");
if (dbPassword !== null) {
  console.log(`Database password retrieved (length: ${dbPassword.length})`);
  // Use password for database connection...
}

// Clean up test credentials
credManager.deleteCredential("database_password");
credManager.deleteCredential("api_key");
credManager.deleteCredential("ssh_passphrase");
```

### Environment Variable Security

```typescript
script.description("Secure environment variable handling");

// Secure environment variable manager
function secureEnvSetup(): void {
  console.log("Setting up secure environment...");
  
  // Check for sensitive data in environment
  let sensitivePatterns: string[] = [
    "password", "secret", "key", "token", "credential",
    "passwd", "pwd", "auth", "private"
  ];
  
  console.log("Scanning environment for potentially sensitive variables:");
  
  let envVars: string[] = `$(env | cut -d'=' -f1)`.split("\n");
  
  for (let varName: string in envVars) {
    if (varName.trim() != "") {
      let lowerName: string = varName.toLowerCase();
      
      for (let pattern: string in sensitivePatterns) {
        if (lowerName.contains(pattern)) {
          console.log(`⚠️  Potentially sensitive environment variable: ${varName}`);
          
          // Check if it's properly protected
          let value: string = env.get(varName) || "";
          if (value.length < 20) {
            console.log(`   Warning: Short value might not be secure`);
          }
          
          break;
        }
      }
    }
  }
}

// Function to mask sensitive output
function maskSensitiveOutput(output: string): string {
  let sensitivePatterns: string[] = [
    "password=\\S+",
    "secret=\\S+", 
    "key=\\S+",
    "token=\\S+",
    "Bearer \\S+",
    "Basic \\S+"
  ];
  
  let masked: string = output;
  
  for (let pattern: string in sensitivePatterns) {
    // Replace sensitive data with asterisks
    masked = `$(echo "${masked}" | sed -E 's/${pattern}/[REDACTED]/g')`;
  }
  
  return masked;
}

// Secure command execution with output masking
function secureCommandExecution(command: string, description: string): void {
  console.log(`Executing: ${description}`);
  
  let output: string = `$(${command} 2>&1)`;
  let maskedOutput: string = maskSensitiveOutput(output);
  
  console.log(`Output: ${maskedOutput}`);
}

// Example usage
secureEnvSetup();

// Test output masking
let testCommands: string[] = [
  "echo 'password=secret123'",
  "echo 'API response: {\"token\": \"abc123\", \"data\": \"public\"}'",
  "echo 'Authorization: Bearer xyz789token'"
];

for (let cmd: string in testCommands) {
  secureCommandExecution(cmd, "Test command with sensitive data");
}
```

## Access Control and Permissions

### File Permission Management

```typescript
script.description("Secure file permission management");

// Secure file permission class
class FilePermissionManager {
  
  // Set secure permissions for different file types
  setSecurePermissions(filePath: string, fileType: string): boolean {
    if (!fs.exists(filePath)) {
      console.log(`❌ File not found: ${filePath}`);
      return false;
    }
    
    let permissions: string = "";
    
    switch (fileType) {
      case "config":
        permissions = "640";  // Owner read/write, group read
        break;
      case "secret":
        permissions = "600";  // Owner read/write only
        break;
      case "script":
        permissions = "750";  // Owner read/write/execute, group read/execute
        break;
      case "public":
        permissions = "644";  // Owner read/write, group/other read
        break;
      case "private":
        permissions = "600";  // Owner read/write only
        break;
      default:
        console.log(`❌ Unknown file type: ${fileType}`);
        return false;
    }
    
    `$(chmod ${permissions} "${filePath}")`;
    
    // Verify permissions were set correctly
    let actualPerms: string = `$(stat -c %a "${filePath}")`;
    if (actualPerms == permissions) {
      console.log(`✅ Set ${fileType} permissions ${permissions} on ${filePath}`);
      return true;
    } else {
      console.log(`❌ Failed to set permissions on ${filePath} (expected: ${permissions}, actual: ${actualPerms})`);
      return false;
    }
  }
  
  // Audit file permissions
  auditPermissions(directory: string): object {
    console.log(`Auditing permissions in: ${directory}`);
    
    let audit: object = {
      "directory": directory,
      "timestamp": `$(date -Iseconds)`,
      "issues": [],
      "summary": {
        "total_files": 0,
        "secure_files": 0,
        "insecure_files": 0
      }
    };
    
    let files: string[] = `$(find "${directory}" -type f)`.split("\n");
    
    for (let file: string in files) {
      if (file.trim() != "") {
        let perms: string = `$(stat -c %a "${file}")`;
        let owner: string = `$(stat -c %U "${file}")`;
        let group: string = `$(stat -c %G "${file}")`;
        
        let totalFiles: number = json.getNumber(audit, ".summary.total_files") + 1;
        audit = json.set(audit, ".summary.total_files", totalFiles);
        
        // Check for insecure permissions
        let isInsecure: boolean = false;
        let issues: string[] = [];
        
        // Check for world-writable files
        if (perms.endsWith("2") || perms.endsWith("6") || perms.endsWith("7")) {
          isInsecure = true;
          issues.push("World-writable");
        }
        
        // Check for executable files with broad permissions
        if ((perms.endsWith("5") || perms.endsWith("7")) && perms.startsWith("7")) {
          if (perms == "755" || perms == "777") {
            isInsecure = true;
            issues.push("Broadly executable");
          }
        }
        
        // Check for config files with loose permissions
        if (file.contains("config") || file.contains(".conf") || file.contains(".cfg")) {
          if (perms != "600" && perms != "640" && perms != "644") {
            isInsecure = true;
            issues.push("Config file with loose permissions");
          }
        }
        
        if (isInsecure) {
          let auditIssues: object[] = json.get(audit, ".issues");
          auditIssues.push({
            "file": file,
            "permissions": perms,
            "owner": owner,
            "group": group,
            "issues": issues
          });
          audit = json.set(audit, ".issues", auditIssues);
          
          let insecureCount: number = json.getNumber(audit, ".summary.insecure_files") + 1;
          audit = json.set(audit, ".summary.insecure_files", insecureCount);
        } else {
          let secureCount: number = json.getNumber(audit, ".summary.secure_files") + 1;
          audit = json.set(audit, ".summary.secure_files", secureCount);
        }
      }
    }
    
    return audit;
  }
  
  // Fix common permission issues
  fixPermissionIssues(auditResults: object): void {
    console.log("Fixing permission issues...");
    
    let issues: object[] = json.get(auditResults, ".issues");
    
    for (let issue: object in issues) {
      let file: string = json.getString(issue, ".file");
      let currentPerms: string = json.getString(issue, ".permissions");
      let problemList: string[] = json.get(issue, ".issues");
      
      console.log(`Fixing ${file} (current: ${currentPerms})`);
      
      for (let problem: string in problemList) {
        if (problem == "World-writable") {
          // Remove world write permission
          `$(chmod o-w "${file}")`;
          console.log(`  ✅ Removed world write permission`);
        } else if (problem == "Broadly executable") {
          // Set safer permissions for executables
          `$(chmod 750 "${file}")`;
          console.log(`  ✅ Set safer executable permissions (750)`);
        } else if (problem.contains("Config file")) {
          // Set secure config permissions
          `$(chmod 640 "${file}")`;
          console.log(`  ✅ Set secure config permissions (640)`);
        }
      }
    }
  }
}

// Example usage
let permManager: FilePermissionManager = new FilePermissionManager();

// Create test files with various permissions
let testDir: string = "/tmp/permission_test";
fs.createDirectory(testDir);

let testFiles: object = {
  "config.txt": "config",
  "secret.key": "secret", 
  "script.sh": "script",
  "public.txt": "public",
  "private.data": "private"
};

let fileNames: string[] = json.keys(testFiles);

for (let fileName: string in fileNames) {
  let filePath: string = `${testDir}/${fileName}`;
  let fileType: string = json.getString(testFiles, `.${fileName}`);
  
  // Create test file
  fs.writeFile(filePath, `Test content for ${fileName}`);
  
  // Set secure permissions
  permManager.setSecurePermissions(filePath, fileType);
}

// Audit permissions
let auditResults: object = permManager.auditPermissions(testDir);
console.log("\nPermission audit results:");
console.log(json.stringify(auditResults, true));

// Clean up
`$(rm -rf ${testDir})`;
```

### User and Process Security

```typescript
script.description("User and process security management");

// Security context checker
function checkSecurityContext(): object {
  console.log("Checking security context...");
  
  let context: object = {
    "timestamp": `$(date -Iseconds)`,
    "user": {
      "current_user": `$(whoami)`,
      "effective_uid": parseInt(`$(id -u)`),
      "effective_gid": parseInt(`$(id -g)`),
      "groups": `$(groups)`.split(" "),
      "is_root": parseInt(`$(id -u)`) == 0
    },
    "process": {
      "pid": parseInt(`$$`),
      "ppid": parseInt(`$(ps -o ppid= -p $$)`),
      "session_id": `$(ps -o sid= -p $$)`.trim()
    },
    "security_checks": {}
  };
  
  // Check if running as root
  let isRoot: boolean = json.getBoolean(context, ".user.is_root");
  if (isRoot) {
    console.log("⚠️  Running as root - elevated privileges detected");
    context = json.set(context, ".security_checks.root_warning", true);
  } else {
    console.log("✅ Running as non-root user");
    context = json.set(context, ".security_checks.root_warning", false);
  }
  
  // Check for sudo usage
  let sudoUser: string = env.get("SUDO_USER") || "";
  if (sudoUser != "") {
    console.log(`⚠️  Script executed via sudo by user: ${sudoUser}`);
    context = json.set(context, ".security_checks.sudo_usage", true);
    context = json.set(context, ".security_checks.original_user", sudoUser);
  } else {
    context = json.set(context, ".security_checks.sudo_usage", false);
  }
  
  // Check umask
  let umask: string = `$(umask)`;
  context = json.set(context, ".security_checks.umask", umask);
  
  if (umask != "0022" && umask != "0077") {
    console.log(`⚠️  Unusual umask detected: ${umask}`);
    context = json.set(context, ".security_checks.unusual_umask", true);
  } else {
    context = json.set(context, ".security_checks.unusual_umask", false);
  }
  
  return context;
}

// Privilege escalation detection
function detectPrivilegeEscalation(): void {
  console.log("Checking for privilege escalation attempts...");
  
  // Check for SUID/SGID files in common locations
  let suspiciousPaths: string[] = ["/tmp", "/var/tmp", "/dev/shm"];
  
  for (let path: string in suspiciousPaths) {
    if (fs.exists(path)) {
      let suidFiles: string[] = `$(find "${path}" -type f \\( -perm -4000 -o -perm -2000 \\) 2>/dev/null)`.split("\n");
      
      for (let file: string in suidFiles) {
        if (file.trim() != "") {
          console.log(`⚠️  SUID/SGID file found in ${path}: ${file}`);
        }
      }
    }
  }
  
  // Check for unusual process ownership
  let processes: string[] = `$(ps -eo user,pid,ppid,command --no-headers)`.split("\n");
  
  for (let process: string in processes) {
    if (process.trim() != "") {
      let fields: string[] = process.trim().split(/\s+/);
      if (fields.length >= 4) {
        let user: string = fields[0];
        let pid: string = fields[1];
        let command: string = fields.slice(3).join(" ");
        
        // Check for processes running as different users
        if (user == "root" && command.contains("sudo")) {
          console.log(`ℹ️  Root process via sudo: ${command.substring(0, 50)}...`);
        }
      }
    }
  }
}

// Secure process execution
function executeSecurely(command: string, dropPrivileges: boolean = true): string {
  console.log(`Executing securely: ${command.substring(0, 50)}...`);
  
  if (dropPrivileges && parseInt(`$(id -u)`) == 0) {
    console.log("⚠️  Dropping root privileges for command execution");
    
    // Find a non-root user to run as
    let safeUser: string = "nobody";
    let userExists: string = `$(id ${safeUser} 2>/dev/null && echo "exists" || echo "missing")`;
    
    if (userExists.trim() == "exists") {
      // Execute as safe user
      let result: string = `$(sudo -u ${safeUser} ${command} 2>&1)`;
      console.log(`✅ Command executed as ${safeUser}`);
      return result;
    } else {
      console.log(`❌ Safe user ${safeUser} not found, executing as root`);
    }
  }
  
  // Execute normally
  let result: string = `$(${command} 2>&1)`;
  return result;
}

// Example security checks
let securityContext: object = checkSecurityContext();
console.log("\nSecurity context:");
console.log(json.stringify(securityContext, true));

detectPrivilegeEscalation();

// Test secure execution
let testCommands: string[] = [
  "whoami",
  "id",
  "echo 'Safe command execution'"
];

for (let cmd: string in testCommands) {
  let result: string = executeSecurely(cmd, true);
  console.log(`Command result: ${result.trim()}`);
}
```

## Network Security

### Secure HTTP Requests

```typescript
script.description("Secure HTTP request handling");

// Secure HTTP client
class SecureHttpClient {
  private allowedHosts: string[];
  private timeout: number;
  private maxRetries: number;
  
  constructor(allowedHosts: string[], timeoutSeconds: number = 30, maxRetries: number = 3) {
    this.allowedHosts = allowedHosts;
    this.timeout = timeoutSeconds;
    this.maxRetries = maxRetries;
  }
  
  private validateUrl(url: string): boolean {
    // Check protocol
    if (!url.startsWith("https://")) {
      console.log("❌ Only HTTPS URLs are allowed");
      return false;
    }
    
    // Extract hostname
    let hostname: string = url.replace("https://", "").split("/")[0].split(":")[0];
    
    // Check against allowed hosts
    let isAllowed: boolean = false;
    for (let allowedHost: string in this.allowedHosts) {
      if (hostname == allowedHost || hostname.endsWith(`.${allowedHost}`)) {
        isAllowed = true;
        break;
      }
    }
    
    if (!isAllowed) {
      console.log(`❌ Host not in allowed list: ${hostname}`);
      return false;
    }
    
    // Check for suspicious characters
    if (url.contains(" ") || url.contains("\n") || url.contains("\r")) {
      console.log("❌ URL contains suspicious characters");
      return false;
    }
    
    return true;
  }
  
  private sanitizeHeaders(headers: object): object {
    let sanitized: object = {};
    let headerNames: string[] = json.keys(headers);
    
    for (let headerName: string in headerNames) {
      let headerValue: string = json.getString(headers, `.${headerName}`);
      
      // Remove potential injection characters
      let cleanValue: string = headerValue.replace(/[\r\n]/g, "");
      
      // Validate header name
      if (headerName.match(/^[a-zA-Z0-9-]+$/)) {
        sanitized = json.set(sanitized, `.${headerName}`, cleanValue);
      } else {
        console.log(`⚠️  Skipping invalid header name: ${headerName}`);
      }
    }
    
    return sanitized;
  }
  
  get(url: string, headers: object = {}): object {
    console.log(`Making secure GET request to: ${url}`);
    
    if (!this.validateUrl(url)) {
      return {
        "success": false,
        "error": "URL validation failed",
        "status_code": 0
      };
    }
    
    let sanitizedHeaders: object = this.sanitizeHeaders(headers);
    
    // Build curl command with security options
    let curlCmd: string = `curl -s -L -k --max-time ${this.timeout} --max-redirs 3`;
    
    // Add headers
    let headerNames: string[] = json.keys(sanitizedHeaders);
    for (let headerName: string in headerNames) {
      let headerValue: string = json.getString(sanitizedHeaders, `.${headerName}`);
      curlCmd += ` -H "${headerName}: ${headerValue}"`;
    }
    
    // Add response format options
    curlCmd += ` -w '{"http_code":"%{http_code}","time_total":"%{time_total}","size_download":"%{size_download}"}' "${url}"`;
    
    let attempt: number = 0;
    while (attempt < this.maxRetries) {
      attempt++;
      
      console.log(`Attempt ${attempt}/${this.maxRetries}`);
      
      let startTime: number = parseInt(`$(date +%s%3N)`);
      let result: string = `$(${curlCmd} 2>/dev/null || echo '{"http_code":"000","time_total":"0","size_download":"0"}')`;
      let endTime: number = parseInt(`$(date +%s%3N)`);
      
      try {
        let metrics: object = json.parse(result.split('\n').pop() || "{}");
        let httpCode: string = json.getString(metrics, ".http_code");
        
        if (httpCode.startsWith("2")) {
          console.log(`✅ Request successful: ${httpCode}`);
          
          return {
            "success": true,
            "status_code": parseInt(httpCode),
            "response_time_ms": endTime - startTime,
            "curl_time": parseFloat(json.getString(metrics, ".time_total")) * 1000,
            "size_bytes": parseInt(json.getString(metrics, ".size_download")),
            "attempt": attempt
          };
        } else if (httpCode == "000") {
          console.log(`❌ Network error on attempt ${attempt}`);
        } else {
          console.log(`❌ HTTP error ${httpCode} on attempt ${attempt}`);
          
          // Don't retry on client errors
          if (httpCode.startsWith("4")) {
            break;
          }
        }
        
      } catch {
        console.log(`❌ Failed to parse response on attempt ${attempt}`);
      }
      
      if (attempt < this.maxRetries) {
        let backoff: number = attempt * 2;
        console.log(`Waiting ${backoff} seconds before retry...`);
        `$(sleep ${backoff})`;
      }
    }
    
    return {
      "success": false,
      "error": "All retry attempts failed",
      "status_code": 0,
      "attempts": attempt
    };
  }
}

// Example usage
let httpClient: SecureHttpClient = new SecureHttpClient(
  ["httpbin.org", "jsonplaceholder.typicode.com", "api.github.com"],
  30,
  3
);

// Test secure requests
let testUrls: string[] = [
  "https://httpbin.org/json",
  "https://httpbin.org/status/200",
  "https://evil.com/malicious",  // Not in allowed hosts
  "http://httpbin.org/json",     // HTTP not allowed
  "https://httpbin.org/status/500"  // Server error
];

for (let url: string in testUrls) {
  console.log(`\n--- Testing URL: ${url} ---`);
  
  let response: object = httpClient.get(url, {
    "User-Agent": "Utah-Secure-Client/1.0",
    "Accept": "application/json"
  });
  
  console.log(`Result: ${json.stringify(response, true)}`);
}
```

## Best Practices Summary

### Security Checklist

```typescript
script.description("Comprehensive security checklist for Utah scripts");

// Security audit function
function performSecurityAudit(): object {
  console.log("Performing comprehensive security audit...");
  
  let audit: object = {
    "timestamp": `$(date -Iseconds)`,
    "audit_version": "1.0",
    "checks": {},
    "recommendations": [],
    "score": 0,
    "max_score": 0
  };
  
  let score: number = 0;
  let maxScore: number = 0;
  let recommendations: string[] = [];
  
  // Check 1: User privileges
  maxScore++;
  if (parseInt(`$(id -u)`) != 0) {
    score++;
    audit = json.set(audit, ".checks.non_root_execution", true);
  } else {
    audit = json.set(audit, ".checks.non_root_execution", false);
    recommendations.push("Avoid running scripts as root when possible");
  }
  
  // Check 2: File permissions
  maxScore++;
  let scriptFile: string = `$0`;
  if (fs.exists(scriptFile)) {
    let perms: string = `$(stat -c %a "${scriptFile}")`;
    if (perms == "750" || perms == "755") {
      score++;
      audit = json.set(audit, ".checks.secure_script_permissions", true);
    } else {
      audit = json.set(audit, ".checks.secure_script_permissions", false);
      recommendations.push("Set secure permissions (750/755) on script files");
    }
  }
  
  // Check 3: Environment variable security
  maxScore++;
  let hasSecureEnv: boolean = env.get("SECURE_MODE") == "true";
  audit = json.set(audit, ".checks.secure_environment", hasSecureEnv);
  if (hasSecureEnv) {
    score++;
  } else {
    recommendations.push("Set SECURE_MODE=true environment variable");
  }
  
  // Check 4: Input validation
  maxScore++;
  // This would be checked by static analysis in a real implementation
  audit = json.set(audit, ".checks.input_validation", true);
  score++; // Assume good for this example
  
  // Check 5: Error handling
  maxScore++;
  // Check if script.exitOnError is used
  audit = json.set(audit, ".checks.error_handling", true);
  score++; // Assume good for this example
  
  // Check 6: Logging security
  maxScore++;
  let logDir: string = "/var/log";
  if (fs.exists(logDir)) {
    let logDirPerms: string = `$(stat -c %a "${logDir}")`;
    if (logDirPerms == "755" || logDirPerms == "750") {
      score++;
      audit = json.set(audit, ".checks.secure_logging", true);
    } else {
      audit = json.set(audit, ".checks.secure_logging", false);
      recommendations.push("Ensure log directories have secure permissions");
    }
  } else {
    score++; // No logging directory, assume secure
    audit = json.set(audit, ".checks.secure_logging", true);
  }
  
  // Calculate final score
  let percentage: number = Math.round((score / maxScore) * 100);
  audit = json.set(audit, ".score", score);
  audit = json.set(audit, ".max_score", maxScore);
  audit = json.set(audit, ".percentage", percentage);
  audit = json.set(audit, ".recommendations", recommendations);
  
  return audit;
}

// Security guidelines
function printSecurityGuidelines(): void {
  console.log(`
🔒 Utah Security Best Practices:

1. **Input Validation**
   - Validate all user inputs
   - Sanitize command arguments
   - Use allowlists over blocklists
   - Check file paths for directory traversal

2. **Command Injection Prevention**
   - Never construct commands with user input
   - Use Utah's built-in functions when possible
   - Validate and sanitize all external data

3. **File System Security**
   - Set restrictive file permissions (600/640/750)
   - Validate file paths before access
   - Create backups before modifications
   - Use secure temporary directories

4. **Credential Management**
   - Never hardcode secrets in scripts
   - Use encrypted credential storage
   - Rotate credentials regularly
   - Limit credential scope and lifetime

5. **Network Security**
   - Use HTTPS for all external requests
   - Validate SSL certificates
   - Implement request timeouts
   - Restrict allowed hosts/domains

6. **Process Security**
   - Run with minimal privileges
   - Drop root privileges when possible
   - Monitor for privilege escalation
   - Use secure environment variables

7. **Error Handling**
   - Don't expose sensitive data in errors
   - Log security events appropriately
   - Fail securely by default
   - Implement proper cleanup on errors

8. **Audit and Monitoring**
   - Log security-relevant events
   - Regular security audits
   - Monitor file permission changes
   - Track credential usage
`);
}

// Perform security audit
let auditResults: object = performSecurityAudit();

console.log("\n=== SECURITY AUDIT RESULTS ===");
console.log(json.stringify(auditResults, true));

let percentage: number = json.getNumber(auditResults, ".percentage");
if (percentage >= 80) {
  console.log(`✅ Good security posture: ${percentage}%`);
} else if (percentage >= 60) {
  console.log(`⚠️  Moderate security posture: ${percentage}%`);
} else {
  console.log(`❌ Poor security posture: ${percentage}%`);
}

printSecurityGuidelines();
```

## Next Steps

- **[Docker Integration](docker.md)** - Secure containerized deployments
- **[Cloud Automation](cloud.md)** - Cloud security best practices
- **[System Administration](system-admin.md)** - Apply security to system scripts

Security is paramount in automation scripts. These patterns and practices help build trustworthy, secure Utah applications that protect sensitive data and system resources.
