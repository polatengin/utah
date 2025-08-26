# Validation Functions

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

## Future Validation Functions

The validation framework is designed for extensibility. Planned additions include:

- `validate.isURL()` - URL format validation
- `validate.isUUID()` - UUID format validation
- `validate.isPhoneNumber()` - Phone number validation
- `validate.isNumeric()` - Numeric value validation
- `validate.isAlphaNumeric()` - Alphanumeric validation
- `validate.isNull()` - Null/empty validation
- `validate.isInRange()` - Range validation for numbers
- `validate.matches()` - Custom regex pattern validation

Each function will follow the same patterns established by `validate.isEmail()` for consistency and reliability.
