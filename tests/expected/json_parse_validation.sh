#!/bin/bash

echo "Testing JSON parse and validation functions"
validJson='{"name": "John", "age": 30, "active": true}'
parsedData=$(echo ${validJson} | jq .)
echo "Parsed JSON successfully"
isValidData=$(echo ${validJson} | jq empty >/dev/null 2>&1 && echo "true" || echo "false")
echo "Valid JSON check:"
echo "${isValidData}"
invalidJson='{"name": "John", "age":}'
isInvalidData=$(echo ${invalidJson} | jq empty >/dev/null 2>&1 && echo "true" || echo "false")
echo "Invalid JSON check:"
echo "${isInvalidData}"
stringifiedData=$(echo "${parsedData}" | jq -c .)
echo "Stringified JSON:"
echo "${stringifiedData}"
echo "JSON parse and validation tests completed!"
