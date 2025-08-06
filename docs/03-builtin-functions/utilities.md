---
layout: default
title: Utility Functions
parent: Functions
nav_order: 9
---

Utah provides a collection of utility functions for common programming tasks, calculations, and data processing.

## Random Number Generation

### Basic Random Operations

```typescript
// Random number generation with no parameters (0-32767)
let random: number = utility.random();

// Random number with maximum value (0-max)
let randomMax: number = utility.random(100);

// Random number within range (min-max)
let randomRange: number = utility.random(1, 100);

// Use in control flow
for (let i: number = 0; i < 5; i++) {
  let dice: number = utility.random(1, 6);
  console.log("Roll ${i + 1}: ${dice}");
}
```

### Random Number Bash Code

```bash
# utility.random() - no parameters (0 to 32767)
random=$((RANDOM % 32768))

# utility.random(100) - max only (0 to 100)
random=$((RANDOM % 101))

# utility.random(50, 150) - min and max (50 to 150)
random=$(((RANDOM % 101) + 50))
```

## UUID Generation

### UUID Creation

```typescript
// Generate universally unique identifiers
let sessionId: string = utility.uuid();
let requestId: string = utility.uuid();
let transactionId: string = utility.uuid();

console.log("Session: ${sessionId}");
console.log("Request: ${requestId}");
console.log("Transaction: ${transactionId}");
```

### UUID Bash Code

```bash
# utility.uuid() - generates UUID with multiple fallbacks
uuid=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
```

## Text Hashing

### Hash Generation

```typescript
// Hash with default algorithm (SHA256)
let defaultHash: string = utility.hash("Hello, World!");

// Hash with specific algorithms
let md5Hash: string = utility.hash("Hello, World!", "md5");
let sha1Hash: string = utility.hash("Hello, World!", "sha1");
let sha256Hash: string = utility.hash("Hello, World!", "sha256");
let sha512Hash: string = utility.hash("Hello, World!", "sha512");

// Hash variables
let message: string = "Secret message";
let messageHash: string = utility.hash(message, "sha256");
```

### Hash Bash Code

```bash
# utility.hash(text, algorithm) - generates hash using specified algorithm
hash=$(echo -n ${text} | case "sha256" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha256"" >&2; exit 1 ;; esac)
```

## Base64 Encoding/Decoding

### Base64 Operations

```typescript
// Basic encoding and decoding
let original: string = "Hello, Utah!";
let encoded: string = utility.base64Encode(original);
let decoded: string = utility.base64Decode(encoded);

console.log("Original: ${original}");
console.log("Encoded: ${encoded}");
console.log("Decoded: ${decoded}");

// Working with special characters
let complex: string = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";
let complexEncoded: string = utility.base64Encode(complex);
let complexDecoded: string = utility.base64Decode(complexEncoded);

// Empty string handling
let empty: string = "";
let emptyEncoded: string = utility.base64Encode(empty);
let emptyDecoded: string = utility.base64Decode(emptyEncoded);
```

### Base64 Bash Code

```bash
# utility.base64Encode(text) - encodes text to Base64
encoded=$(echo -n ${text} | base64 -w 0)

# utility.base64Decode(text) - decodes Base64 text
decoded=$(echo -n ${encoded} | base64 -d)
```

## Use Cases

### Random Number Use Cases

- **Game Development**: Dice rolls, random events, procedural generation
- **Load Testing**: Random delays and intervals
- **Sampling**: Random selection from ranges
- **Security**: Random timeouts and delays (not cryptographically secure)
- **User Experience**: Random tips, messages, or content selection

### UUID Use Cases

- **Session Management**: Unique session identifiers
- **Request Tracking**: Unique request IDs for logging and monitoring
- **File Naming**: Unique file names to prevent conflicts
- **Database Keys**: Primary keys for database records
- **API Keys**: Temporary API keys and tokens

### Text Hashing Use Cases

- **Data Integrity**: Verify file integrity with checksums
- **Password Hashing**: Hash passwords before storage (use with salt)
- **Content Fingerprinting**: Generate unique fingerprints for content
- **Cache Keys**: Generate cache keys from content
- **Data Deduplication**: Identify duplicate content

### Base64 Encoding Use Cases

- **Data Transmission**: Encode binary data for text-based protocols
- **Configuration Files**: Store binary data in text configuration files
- **API Communication**: Encode payloads for API requests
- **Email Attachments**: Encode files for email transmission
- **Web Development**: Data URIs for embedded resources

## Implementation Details

All utility functions are implemented with proper error handling and fallback mechanisms:

- **utility.random()**: Uses bash `$RANDOM` with range validation
- **utility.uuid()**: Falls back from `uuidgen` → `python3` → timestamp-based UUID
- **utility.hash()**: Supports md5, sha1, sha256, sha512 algorithms with validation
- **utility.base64Encode/Decode()**: Uses standard `base64` command with proper line wrapping

The functions compile to efficient bash code that works across different Unix-like systems.
