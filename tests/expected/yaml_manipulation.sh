#!/bin/bash

echo "Testing YAML property manipulation functions"
configData='app:\n  name: MyApp\n  version: "1.0"\n  debug: false'
config=$(echo ${configData} | yq -o=json .)
updatedConfig=$(echo "${config}" | yq -o=json . | jq '.app.name = "UpdatedApp"' | yq -o=yaml .)
echo "Updated app name:"
newName=$(echo "${updatedConfig}" | yq -o=json . | jq -r '.app.name')
echo "${newName}"
updatedConfig=$(echo "${updatedConfig}" | yq -o=json . | jq '.app.debug = true' | yq -o=yaml .)
echo "Updated debug setting:"
debugMode=$(echo "${updatedConfig}" | yq -o=json . | jq -r '.app.debug')
echo "${debugMode}"
updatedConfig=$(echo "${updatedConfig}" | yq -o=json . | jq '.app.port = 8080' | yq -o=yaml .)
echo "Added port setting:"
port=$(echo "${updatedConfig}" | yq -o=json . | jq -r '.app.port')
echo "${port}"
configWithoutDebug=$(echo "${updatedConfig}" | yq -o=json . | jq 'del(.app.debug)' | yq -o=yaml .)
echo "Removed debug property:"
hasDebug=$(echo "${configWithoutDebug}" | yq -o=json . | jq 'try .app.debug catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has debug after deletion:"
echo "${hasDebug}"
echo "YAML property manipulation tests completed!"
