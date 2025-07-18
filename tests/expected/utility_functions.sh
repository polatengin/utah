#!/bin/bash

uuid1=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
uuid2=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
echo "UUID 1: ${uuid1}"
echo "UUID 2: ${uuid2}"
echo "UUIDs are different: ([ "uuid1 !" = "${uuid2}" ])"
text="Hello, World!"
hash1=$(echo -n ${text} | case "sha256" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha256"" >&2; exit 1 ;; esac)
echo "Text: ${text}"
echo "SHA256 hash: ${hash1}"
md5Hash=$(echo -n ${text} | case "md5" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "md5"" >&2; exit 1 ;; esac)
sha1Hash=$(echo -n ${text} | case "sha1" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha1"" >&2; exit 1 ;; esac)
sha256Hash=$(echo -n ${text} | case "sha256" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha256"" >&2; exit 1 ;; esac)
sha512Hash=$(echo -n ${text} | case "sha512" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha512"" >&2; exit 1 ;; esac)
echo "MD5 hash: ${md5Hash}"
echo "SHA1 hash: ${sha1Hash}"
echo "SHA256 hash: ${sha256Hash}"
echo "SHA512 hash: ${sha512Hash}"
originalText="Hello, Utah!"
encoded=$(echo -n ${originalText} | base64 -w 0)
echo "Original text: ${originalText}"
echo "Base64 encoded: ${encoded}"
decoded=$(echo -n ${encoded} | base64 -d)
echo "Base64 decoded: ${decoded}"
echo "Round-trip successful: ([ "${originalText}" = "${decoded}" ])"
complexText="This is a test with special characters: !@#$%^&*()_+-=[]{}|;':\",./<>?"
complexEncoded=$(echo -n ${complexText} | base64 -w 0)
complexDecoded=$(echo -n ${complexEncoded} | base64 -d)
echo "Complex text: ${complexText}"
echo "Complex encoded: ${complexEncoded}"
echo "Complex decoded: ${complexDecoded}"
echo "Complex round-trip: ([ "${complexText}" = "${complexDecoded}" ])"
secretMessage="This is a secret message"
secretHash=$(echo -n ${secretMessage} | case "sha256" in "md5") md5sum | cut -d' ' -f1 ;; "sha1") sha1sum | cut -d' ' -f1 ;; "sha256") sha256sum | cut -d' ' -f1 ;; "sha512") sha512sum | cut -d' ' -f1 ;; *) echo "Error: Unsupported hash algorithm: "sha256"" >&2; exit 1 ;; esac)
secretEncoded=$(echo -n ${secretMessage} | base64 -w 0)
echo "Secret message hash: ${secretHash}"
echo "Secret message encoded: ${secretEncoded}"
emptyText=""
emptyEncoded=$(echo -n ${emptyText} | base64 -w 0)
emptyDecoded=$(echo -n ${emptyEncoded} | base64 -d)
echo "Empty text encoded: ${emptyEncoded}"
echo "Empty text decoded: ${emptyDecoded}"
echo "Empty round-trip: ([ "${emptyText}" = "${emptyDecoded}" ])"
uuid3=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
uuid4=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
uuid5=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
echo "UUID 3: ${uuid3}"
echo "UUID 4: ${uuid4}"
echo "UUID 5: ${uuid5}"
echo "All UUIDs unique: ([ "uuid3 !" = "${uuid4}" ] && [ "uuid4 !" = "${uuid5}" ] && [ "uuid3 !" = "${uuid5}" ])"
