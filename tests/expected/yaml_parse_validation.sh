#!/bin/bash

echo "Testing YAML parse and validation functions"
validYaml='name: John\nage: 30\nactive: true'
parsedData=$(echo ${validYaml} | yq -o=json .)
echo "Parsed YAML successfully"
isValidData=$(echo ${validYaml} | yq empty >/dev/null 2>&1 && echo "true" || echo "false")
echo "Valid YAML check:"
echo "${isValidData}"
invalidYaml='name: John\nage: [unclosed array'
isInvalidData=$(echo ${invalidYaml} | yq empty >/dev/null 2>&1 && echo "true" || echo "false")
echo "Invalid YAML check:"
echo "${isInvalidData}"
stringifiedData=$(echo "${parsedData}" | yq -o=yaml .)
echo "Stringified YAML:"
echo "${stringifiedData}"
echo "YAML parse and validation tests completed!"
