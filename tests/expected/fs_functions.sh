#!/bin/sh

echo "Testing fs.readFile and fs.writeFile"
echo "Hello World" > "test1.txt"
content1=$(cat "test1.txt")
echo "Test 1 - String literal:"
echo "${content1}"
message="This is from a variable"
echo "${message}" > "test2.txt"
content2=$(cat "test2.txt")
echo "Test 2 - Variable content:"
echo "${content2}"
echo "All tests completed!"
