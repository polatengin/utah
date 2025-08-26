---
layout: default
title: Validation Functions
parent: Language Features
nav_order: 15
---

Utah provides a comprehensive set of validation functions for common data types and formats. These functions help ensure data integrity and user input validation in your scripts.

## Overview

Validation functions in Utah:

- Return boolean values (`true` or `false`)
- Work with variables, literals, and expressions
- Use native bash capabilities for maximum performance
- Provide consistent error handling patterns
- Integrate seamlessly with Utah's conditional logic

## Available Validation Functions

### Email Validation

#### validate.isEmail()

Validates email address format using RFC-compliant pattern matching.

**Syntax:**

```typescript
validate.isEmail(email: string) -> boolean
```

**Parameters:**

- `email` - The email address string to validate

**Returns:**

- `true` if the email format is valid
- `false` if the email format is invalid

**Examples:**

```typescript
// Basic validation
let userEmail: string = "user@example.com";
let isValid: boolean = validate.isEmail(userEmail);

console.log(`Email ${userEmail} is ${isValid ? "valid" : "invalid"}`);

// Conditional validation
if (validate.isEmail("admin@company.com")) {
  console.log("Admin email is properly formatted");
} else {
  console.log("Admin email needs correction");
  exit(1);
}

// Interactive validation
let email: string = console.promptText("Enter your email:");
while (!validate.isEmail(email)) {
  console.log("Invalid email format. Please try again.");
  email = console.promptText("Enter your email:");
}
console.log("Email validated successfully!");
```

## Email Format Support

The `validate.isEmail()` function supports standard email formats including:

### Valid Formats

- **Basic**: `user@domain.com`
- **Subdomains**: `user@mail.domain.com`
- **International TLDs**: `user@domain.co.uk`
- **Plus addressing**: `user+tag@domain.com`
- **Dots in local part**: `first.last@domain.com`
- **Underscores**: `user_name@domain.com`
- **Hyphens in domain**: `user@sub-domain.com`
- **Numeric domains**: `user@123domain.com`

### Validation Rules

- **Local part**: Must contain alphanumeric characters, dots, underscores, percent signs, plus signs, or hyphens
- **@ symbol**: Required separator between local and domain parts
- **Domain part**: Must contain alphanumeric characters, dots, or hyphens
- **TLD**: Must be at least 2 alphabetic characters

### Invalid Formats

- Missing @ symbol: `invalid.email`
- Missing local part: `@domain.com`
- Missing domain: `user@`
- Invalid domain: `user@.com`
- Missing TLD: `user@domain`

## Generated Bash Code

Utah compiles validation functions to efficient bash regex commands:

```bash
# validate.isEmail() compilation
email="user@example.com"
isValid=$(echo ${email} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")

# In conditionals
if [ $(echo "admin@company.com" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false") = "true" ]; then
  echo "Admin email is properly formatted"
fi

# With user input validation loop
email=$(read -p "Enter your email: " && echo $REPLY)
while [ $(echo "${email}" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false") != "true" ]; do
  echo "Invalid email format. Please try again."
  email=$(read -p "Enter your email: " && echo $REPLY)
done
```

## URL Validation

### validate.isURL()

Validates URL format supporting multiple protocols including HTTP, HTTPS, FTP, and FILE schemes.

**Syntax:**

```typescript
validate.isURL(url: string) -> boolean
```

**Parameters:**

- `url` - The URL string to validate

**Returns:**

- `true` if the URL format is valid
- `false` if the URL format is invalid

**Examples:**

```typescript
// Basic URL validation
let apiUrl: string = "https://api.example.com";
let isValid: boolean = validate.isURL(apiUrl);

console.log(`URL ${apiUrl} is ${isValid ? "valid" : "invalid"}`);

// Validate user input
let userUrl: string = console.promptText("Enter website URL:");
if (validate.isURL(userUrl)) {
  console.log("Valid URL provided");
} else {
  console.log("Please enter a valid URL");
  exit(1);
}

// Use in conditional logic
if (validate.isURL("https://github.com/polatengin/utah")) {
  console.log("GitHub repository URL is valid");
}

// Validate different protocols
let httpUrl: string = "http://example.com";
let httpsUrl: string = "https://secure.example.com:8080";
let ftpUrl: string = "ftp://files.example.com";
let fileUrl: string = "file:///home/user/document.txt";

console.log(`HTTP URL: ${validate.isURL(httpUrl)}`);
console.log(`HTTPS URL: ${validate.isURL(httpsUrl)}`);
console.log(`FTP URL: ${validate.isURL(ftpUrl)}`);
console.log(`File URL: ${validate.isURL(fileUrl)}`);

// Validate complex URLs
let complexUrl: string = "https://api.example.com:8080/v1/users?filter=active&sort=name#results";
if (validate.isURL(complexUrl)) {
  console.log("Complex URL with port, path, query, and fragment is valid");
}
```

### URL Validation Patterns

The `validate.isURL()` function validates URLs based on these components:

- **Protocol**: Must be one of `http`, `https`, `ftp`, or `file`
- **Domain**: Must contain valid domain name with optional subdomains
- **Port**: Optional port number (e.g., `:8080`)
- **Path**: Optional path component (e.g., `/api/v1/users`)
- **Query**: Optional query parameters (e.g., `?param=value&other=data`)
- **Fragment**: Optional fragment identifier (e.g., `#section`)

### Valid URL Examples

```typescript
validate.isURL("https://www.example.com");                    // true
validate.isURL("http://localhost:3000");                      // true
validate.isURL("https://api.example.com/v1/users");           // true
validate.isURL("ftp://files.company.com");                    // true
validate.isURL("file:///home/user/document.txt");             // true
validate.isURL("https://search.com?q=utah+language");         // true
validate.isURL("https://docs.com/guide#section1");            // true
```

### Invalid URL Examples

```typescript
validate.isURL("not-a-url");                                  // false
validate.isURL("httpexample.com");                            // false (missing ://)
validate.isURL("https://");                                   // false (no domain)
validate.isURL("smtp://mail.example.com");                    // false (unsupported protocol)
validate.isURL("https://.example.com");                       // false (invalid domain)
```

### Generated Bash Code for URL Validation

```bash
# validate.isURL() compilation
url="https://api.example.com"
isValid=$(echo ${url} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")

# In conditionals
if [ $(echo "https://github.com/polatengin/utah" | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false") = "true" ]; then
  echo "GitHub URL is properly formatted"
fi

# URL validation loop
url=$(read -p "Enter website URL: " && echo $REPLY)
while [ $(echo "${url}" | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false") != "true" ]; do
  echo "Invalid URL format. Please try again."
  url=$(read -p "Enter website URL: " && echo $REPLY)
done
```

## Use Cases

### User Registration Systems

```typescript
function validateUserRegistration(email: string, username: string): boolean {
  if (!validate.isEmail(email)) {
    console.log("Error: Invalid email address format");
    return false;
  }

  console.log("User registration data validated successfully");
  return true;
}

let userEmail: string = console.promptText("Email:");
let userName: string = console.promptText("Username:");

if (validateUserRegistration(userEmail, userName)) {
  console.log("Creating user account...");
} else {
  console.log("Registration failed - please check your input");
  exit(1);
}
```

### Configuration File Validation

```typescript
// Validate email addresses in configuration files
let configFile: string = fs.readFile("config.json");
let config: object = json.parse(configFile);

let adminEmail: string = json.get(config, ".admin.email");
let supportEmail: string = json.get(config, ".support.email");

if (!validate.isEmail(adminEmail)) {
  console.log("Error: Invalid admin email in configuration");
  exit(1);
}

if (!validate.isEmail(supportEmail)) {
  console.log("Error: Invalid support email in configuration");
  exit(1);
}

console.log("Configuration email addresses validated");
```

### Batch Email Processing

```typescript
// Process and validate a list of email addresses
let emails: string[] = ["user1@example.com", "user2@test.org", "invalid.email"];

for (let email: string in emails) {
  if (validate.isEmail(email)) {
    console.log(`✅ ${email} - Valid`);
  } else {
    console.log(`❌ ${email} - Invalid format`);
  }
}
```

### API Input Validation

```typescript
// Validate API parameters
function processContactForm(name: string, email: string, message: string): void {
  if (!validate.isEmail(email)) {
    console.log("Error: Invalid email address provided");
    web.post("https://api.example.com/error", '{"error": "Invalid email"}');
    exit(1);
  }

  // Process valid form data
  let formData: string = json.stringify({
    "name": name,
    "email": email,
    "message": message
  });

  web.post("https://api.example.com/contact", formData);
  console.log("Contact form submitted successfully");
}
```

## Best Practices

### 1. Validate Early and Often

```typescript
// Good - validate immediately after receiving input
let email: string = console.promptText("Enter email:");
if (!validate.isEmail(email)) {
  console.log("Invalid email format");
  exit(1);
}

// Continue with valid email
console.log("Processing email: " + email);
```

### 2. Provide Clear Error Messages

```typescript
// Good - specific error messages
if (!validate.isEmail(userInput)) {
  console.log("Please enter a valid email address (e.g., user@example.com)");
} else {
  console.log("Email format validated successfully");
}
```

### 3. Combine with Business Logic

```typescript
// Combine format validation with business rules
let email: string = "user@competitor.com";

if (!validate.isEmail(email)) {
  console.log("Invalid email format");
} else if (email.includes("competitor.com")) {
  console.log("Corporate policy: External domains not allowed");
} else {
  console.log("Email approved for processing");
}
```

### 4. Handle Edge Cases

```typescript
// Handle empty or null inputs gracefully
function safeEmailValidation(input: string): boolean {
  if (input.length() === 0) {
    console.log("Email address is required");
    return false;
  }

  return validate.isEmail(input);
}
```

### 5. Use in Loops for Retry Logic

```typescript
// Retry until valid input is provided
let email: string = "";
let attempts: number = 0;
let maxAttempts: number = 3;

while (attempts < maxAttempts) {
  email = console.promptText("Enter your email:");

  if (validate.isEmail(email)) {
    console.log("Email validated successfully");
    break;
  }

  attempts = attempts + 1;
  console.log(`Invalid email format. Attempts remaining: ${maxAttempts - attempts}`);
}

if (attempts >= maxAttempts) {
  console.log("Maximum validation attempts exceeded");
  exit(1);
}
```

## Performance Considerations

### Regex Efficiency

- Utah's email validation uses optimized regex patterns
- Validation is performed using native bash `grep` for maximum speed
- No external dependencies or network calls required

### Memory Usage

- Validation functions have minimal memory overhead
- Regex patterns are compiled once per validation call
- No persistent state maintained between validations

### Scalability

```typescript
// Efficient for batch processing
let emails: string[] = loadEmailsFromFile("contacts.txt");
let validEmails: string[] = [];

for (let email: string in emails) {
  if (validate.isEmail(email)) {
    validEmails.push(email);
  }
}

console.log(`Validated ${validEmails.length()} out of ${emails.length()} emails`);
```

## Error Handling

### Integration with Try-Catch

```typescript
try {
  let email: string = getEmailFromExternalSource();

  if (!validate.isEmail(email)) {
    throw new Error("Invalid email format from external source");
  }

  console.log("External email validated successfully");
} catch {
  console.log("Email validation failed - using default");
  email = "default@company.com";
}
```

### Graceful Degradation

```typescript
// Fallback validation for critical systems
function validateEmailWithFallback(email: string): boolean {
  // Primary validation
  if (validate.isEmail(email)) {
    return true;
  }

  // Fallback: basic @ symbol check
  if (email.includes("@") && email.includes(".")) {
    console.log("Warning: Using fallback email validation");
    return true;
  }

  return false;
}
```

## Technical Implementation

### Regex Pattern Details

The email validation uses the following regex pattern:

```text
^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$
```

**Pattern Breakdown:**

- `^` - Start of string
- `[A-Za-z0-9._%+-]+` - Local part (one or more allowed characters)
- `@` - Required @ symbol
- `[A-Za-z0-9.-]+` - Domain part (one or more allowed characters)
- `\.` - Required dot before TLD
- `[A-Za-z]{2,}` - TLD (two or more letters)
- `$` - End of string

### Bash Implementation

```bash
# Generated bash code structure
email_to_validate="user@example.com"
validation_result=$(echo "${email_to_validate}" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
```

## UUID Validation

### validate.isUUID()

Validates UUID (Universally Unique Identifier) format according to RFC 4122 standards.

**Syntax:**

```typescript
validate.isUUID(uuid: string) -> boolean
```

**Parameters:**

- `uuid` - The UUID string to validate

**Returns:**

- `true` if the UUID format is valid (RFC 4122 compliant)
- `false` if the UUID format is invalid

**Supported UUID Versions:** 1, 2, 3, 4, 5

**Examples:**

```typescript
// Basic UUID validation
let sessionId: string = "550e8400-e29b-41d4-a716-446655440000";
let isValid: boolean = validate.isUUID(sessionId);

console.log(`UUID ${sessionId} is ${isValid ? "valid" : "invalid"}`);

// Use with generated UUIDs
let generatedUUID: string = utility.uuid();
if (validate.isUUID(generatedUUID)) {
  console.log("Generated UUID is valid");
  // Process with valid UUID
} else {
  console.log("UUID generation failed");
  exit(1);
}

// Conditional validation
if (validate.isUUID(requestId)) {
  console.log("Processing request with valid UUID");
} else {
  console.log("Invalid UUID format in request");
  exit(1);
}

// Integration with variables
let transactionId: string = utility.uuid();
let isValidTransaction: boolean = validate.isUUID(transactionId);
console.log(`Transaction ${transactionId} valid: ${isValidTransaction}`);
```

### UUID Format Requirements

The `validate.isUUID()` function validates UUIDs based on RFC 4122:

- **Format**: `xxxxxxxx-xxxx-Mxxx-Nxxx-xxxxxxxxxxxx`
- **Length**: Exactly 36 characters including hyphens
- **Characters**: Hexadecimal digits (0-9, a-f, A-F) only
- **Version (M)**: Must be 1, 2, 3, 4, or 5 (third group, first digit)
- **Variant (N)**: Must be 8, 9, a, b, A, or B (fourth group, first digit)
- **Case**: Supports both uppercase and lowercase hexadecimal

### Valid UUID Examples

```typescript
validate.isUUID("550e8400-e29b-41d4-a716-446655440000");     // true (version 1)
validate.isUUID("6ba7b810-9dad-41d1-80b4-00c04fd430c8");     // true (version 4)
validate.isUUID("6ba7b815-9dad-51d1-80b4-00c04fd430c8");     // true (version 5)
validate.isUUID("550E8400-E29B-41D4-A716-446655440000");     // true (uppercase)
validate.isUUID("550e8400-E29B-41d4-A716-446655440000");     // true (mixed case)
validate.isUUID("00000000-0000-4000-8000-000000000000");     // true (zeros)
validate.isUUID("ffffffff-ffff-4fff-bfff-ffffffffffff");     // true (all F's)
```

### Invalid UUID Examples

```typescript
validate.isUUID("550e8400-e29b-41d4-a716");                  // false (too short)
validate.isUUID("550e8400-e29b-41d4-a716-44665544000g");     // false (invalid char 'g')
validate.isUUID("550e8400-e29b-61d4-a716-446655440000");     // false (version 6 unsupported)
validate.isUUID("550e8400-e29b-41d4-2716-446655440000");     // false (invalid variant 2)
validate.isUUID("550e8400e29b41d4a716446655440000");         // false (no hyphens)
validate.isUUID("550e840-0e29b-41d4-a716-446655440000");     // false (wrong hyphen position)
validate.isUUID("");                                         // false (empty string)
validate.isUUID("not-a-uuid-at-all");                        // false (completely invalid)
```

### UUID Validation Integration

```typescript
// Session management
function createSession(): string {
  let sessionId: string = utility.uuid();
  if (validate.isUUID(sessionId)) {
    return sessionId;
  } else {
    console.log("Failed to generate valid session ID");
    exit(1);
  }
}

// Request tracking
let requestId: string = utility.uuid();
let logEntry: string = validate.isUUID(requestId) ? `[${requestId}] Request started` : "[INVALID-ID] Request started";
console.log(logEntry);

// Database operations
if (validate.isUUID(userId) && validate.isUUID(transactionId)) {
  console.log("Proceeding with database operation");
} else {
  console.log("Invalid UUID format in database parameters");
  exit(1);
}
```

### UUID Validation Bash Output

```bash
# UUID validation compiles to efficient regex pattern matching
sessionId="550e8400-e29b-41d4-a716-446655440000"
isValid=$(echo ${sessionId} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")

if [ "${isValid}" = "true" ]; then
  echo "UUID is valid"
else
  echo "Invalid UUID format"
  exit 1
fi
```

## Future Validation Functions

The validation framework is designed for extensibility. Planned additions include:

- `validate.isPhoneNumber()` - Phone number validation
- `validate.isNumeric()` - Numeric value validation
- `validate.isAlphaNumeric()` - Alphanumeric validation
- `validate.isNull()` - Null/empty validation
- `validate.isInRange()` - Range validation for numbers
- `validate.matches()` - Custom regex pattern validation

Each function will follow the same patterns established by `validate.isEmail()` for consistency and reliability.
