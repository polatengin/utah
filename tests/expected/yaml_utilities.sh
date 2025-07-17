#!/bin/bash

echo "Testing YAML utility functions"
sampleData='firstName: John\nlastName: Doe\nage: 30\ncity: "New York"'
dataObj=$(echo ${sampleData} | yq -o=json .)
echo "YAML keys:"
keys=$(echo "${dataObj}" | yq -o=json . | jq -r 'keys[]')
for key in "${keys[@]}"; do
  echo "Key: ${key}"
done
echo "YAML values:"
values=$(echo "${dataObj}" | yq -o=json . | jq -r '.[]')
for value in "${values[@]}"; do
  echo "Value: ${value}"
done
defaultSettings='timeout: 30\nretries: 3\ndebug: false'
userSettings='timeout: 60\ndebug: true\nlogLevel: info'
defaultObj=$(echo ${defaultSettings} | yq -o=json .)
userObj=$(echo ${userSettings} | yq -o=json .)
mergedSettings=$(echo "${defaultObj}" | yq -o=json . | jq --argjson obj2 "$(echo \"${userObj}\" | yq -o=json .)" '. * $obj2' | yq -o=yaml .)
echo "Merged settings:"
mergedString=$(echo "${mergedSettings}" | yq -o=yaml .)
echo "${mergedString}"
finalTimeout=$(echo "${mergedSettings}" | yq -o=json . | jq -r '.timeout')
finalRetries=$(echo "${mergedSettings}" | yq -o=json . | jq -r '.retries')
finalDebug=$(echo "${mergedSettings}" | yq -o=json . | jq -r '.debug')
finalLogLevel=$(echo "${mergedSettings}" | yq -o=json . | jq -r '.logLevel')
echo "Final timeout: ${finalTimeout}"
echo "Final retries: ${finalRetries}"
echo "Final debug: ${finalDebug}"
echo "Final log level: ${finalLogLevel}"
echo "YAML utility tests completed!"
