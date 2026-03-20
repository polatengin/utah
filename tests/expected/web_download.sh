#!/bin/bash

status=$(curl -sL -o "/tmp/file.tar.gz" -w "%{http_code}" "https://example.com/file.tar.gz" 2>/dev/null || echo "000")
echo "Download status: ${status}"
fileUrl="https://example.com/data.json"
outputPath="/tmp/data.json"
result=$(curl -sL -o ${outputPath} -w "%{http_code}" ${fileUrl} 2>/dev/null || echo "000")
echo "Download result: ${result}"
