#!/bin/bash

echo "Testing fs.exists() function"
fileExists=$([ -e "/bin/bash" ] && echo "true" || echo "false")
echo "Test 1 - /bin/bash exists:"
echo "${fileExists}"
noFileExists=$([ -e "/nonexistent/path/file.txt" ] && echo "true" || echo "false")
echo "Test 2 - Nonexistent file:"
echo "${noFileExists}"
testPath="/etc"
dirExists=$([ -e ${testPath} ] && echo "true" || echo "false")
echo "Test 3 - /etc directory exists:"
echo "${dirExists}"
if [ $([ -e "/usr/bin" ] && echo "true" || echo "false") ]; then
  echo "Test 4 - /usr/bin directory found"
else
  echo "Test 4 - /usr/bin directory not found"
fi
echo "All fs.exists() tests completed!"
