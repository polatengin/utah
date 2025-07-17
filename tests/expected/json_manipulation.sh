#!/bin/bash

echo "Testing JSON property manipulation functions"
configData='{"app": {"name": "MyApp", "version": "1.0", "debug": false}}'
config=$(echo ${configData} | jq .)
updatedConfig=$(echo "${config}" | jq '.app.name = "UpdatedApp"')
echo "Updated app name:"
newName=$(echo "${updatedConfig}" | jq -r '.app.name')
echo "${newName}"
updatedConfig=$(echo "${updatedConfig}" | jq '.app.debug = true')
echo "Updated debug setting:"
debugMode=$(echo "${updatedConfig}" | jq -r '.app.debug')
echo "${debugMode}"
updatedConfig=$(echo "${updatedConfig}" | jq '.app.port = 8080')
echo "Added port setting:"
port=$(echo "${updatedConfig}" | jq -r '.app.port')
echo "${port}"
configWithoutDebug=$(echo "${updatedConfig}" | jq 'del(.app.debug)')
echo "Removed debug property:"
hasDebug=$(echo "${configWithoutDebug}" | jq 'try .app.debug catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has debug after deletion:"
echo "${hasDebug}"
echo "JSON property manipulation tests completed!"
