#!/bin/bash

response=$(curl -s -X DELETE "https://httpbin.org/delete" 2>/dev/null || echo "")
echo "Response length: ${#response}"
apiUrl="https://httpbin.org/delete"
deleteResponse=$(curl -s -X DELETE ${apiUrl} 2>/dev/null || echo "")
echo "Delete Response: ${deleteResponse}"
responseWithHeaders=$(curl -s -X DELETE "-H 'Authorization: Bearer token123'" "https://httpbin.org/delete" 2>/dev/null || echo "")
echo "Response with headers: ${#responseWithHeaders}"
