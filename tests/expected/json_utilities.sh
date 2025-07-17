#!/bin/bash

echo "Testing JSON utility functions"
sampleData='{"firstName": "John", "lastName": "Doe", "age": 30, "city": "New York"}'
dataObj=$(echo ${sampleData} | jq .)
echo "JSON keys:"
keys=$(echo "${dataObj}" | jq -r 'keys[]')
for key in "${keys[@]}"; do
  echo "Key: ${key}"
done
echo "JSON values:"
values=$(echo "${dataObj}" | jq -r '.[]')
for value in "${values[@]}"; do
  echo "Value: ${value}"
done
defaultSettings='{"timeout": 30, "retries": 3, "debug": false}'
userSettings='{"timeout": 60, "debug": true, "logLevel": "info"}'
defaultObj=$(echo ${defaultSettings} | jq .)
userObj=$(echo ${userSettings} | jq .)
mergedSettings=$(echo "${defaultObj}" | jq --argjson obj2 "${userObj}" '. * $obj2')
echo "Merged settings:"
mergedString=$(echo "${mergedSettings}" | jq -c .)
echo "${mergedString}"
finalTimeout=$(echo "${mergedSettings}" | jq -r '.timeout')
finalRetries=$(echo "${mergedSettings}" | jq -r '.retries')
finalDebug=$(echo "${mergedSettings}" | jq -r '.debug')
finalLogLevel=$(echo "${mergedSettings}" | jq -r '.logLevel')
echo "Final timeout: ${finalTimeout}"
echo "Final retries: ${finalRetries}"
echo "Final debug: ${finalDebug}"
echo "Final log level: ${finalLogLevel}"
echo "JSON utility tests completed!"
