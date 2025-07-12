#!/bin/bash

response=$(curl -s "https://httpbin.org/get" 2>/dev/null || echo "")
echo "Response length: ${#response}"
apiUrl="https://httpbin.org/ip"
ipResponse=$(curl -s ${apiUrl} 2>/dev/null || echo "")
echo "IP Response: ${ipResponse}"
