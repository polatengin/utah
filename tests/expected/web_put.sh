#!/bin/bash

response=$(curl -s -X PUT -d ${'{"name": "Utah", "version": "1.0"}'} "https://httpbin.org/put" 2>/dev/null || echo "")
echo "Response length: ${#response}"
apiUrl="https://httpbin.org/put"
putData='{"user": "test", "action": "update"}'
updateResponse=$(curl -s -X PUT -d ${putData} ${apiUrl} 2>/dev/null || echo "")
echo "Update Response: ${updateResponse}"
responseWithHeaders=$(curl -s -X PUT "-H 'Authorization: Bearer token123' -H 'Content-Type: application/json'" -d ${'{"authenticated": true}'} "https://httpbin.org/put" 2>/dev/null || echo "")
echo "Response with headers: ${#responseWithHeaders}"
