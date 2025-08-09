#!/bin/bash

response=$(curl -s -X POST -d ${'{"name": "Utah", "version": "1.0"}'} "https://httpbin.org/post" 2>/dev/null || echo "")
echo "Response length: ${#response}"
apiUrl="https://httpbin.org/post"
postData='{"user": "test", "action": "create"}'
createResponse=$(curl -s -X POST -d ${postData} ${apiUrl} 2>/dev/null || echo "")
echo "Create Response: ${createResponse}"
responseWithHeaders=$(curl -s -X POST "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'" -d ${'{"authenticated": true}'} "https://httpbin.org/post" 2>/dev/null || echo "")
echo "Response with headers: ${#responseWithHeaders}"
