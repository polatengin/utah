#!/bin/bash

echo "Testing YAML property access functions"
userData='user:\n  name: Jane\n  age: 25\n  settings:\n    theme: dark\n    notifications: true'
userObj=$(echo ${userData} | yq -o=json .)
userName=$(echo "${userObj}" | yq -o=json . | jq -r '.user.name')
echo "User name:"
echo "${userName}"
userTheme=$(echo "${userObj}" | yq -o=json . | jq -r '.user.settings.theme')
echo "User theme:"
echo "${userTheme}"
notifications=$(echo "${userObj}" | yq -o=json . | jq -r '.user.settings.notifications')
echo "Notifications enabled:"
echo "${notifications}"
hasUserName=$(echo "${userObj}" | yq -o=json . | jq 'try .user.name catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has user name:"
echo "${hasUserName}"
hasUserEmail=$(echo "${userObj}" | yq -o=json . | jq 'try .user.email catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has user email:"
echo "${hasUserEmail}"
echo "YAML property access tests completed!"
