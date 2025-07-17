#!/bin/bash

echo "Testing JSON property access functions"
userData='{"user": {"name": "Jane", "age": 25, "settings": {"theme": "dark", "notifications": true}}}'
userObj=$(echo ${userData} | jq .)
userName=$(echo "${userObj}" | jq -r '.user.name')
echo "User name:"
echo "${userName}"
userTheme=$(echo "${userObj}" | jq -r '.user.settings.theme')
echo "User theme:"
echo "${userTheme}"
notifications=$(echo "${userObj}" | jq -r '.user.settings.notifications')
echo "Notifications enabled:"
echo "${notifications}"
hasUserName=$(echo "${userObj}" | jq 'try .user.name catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has user name:"
echo "${hasUserName}"
hasUserEmail=$(echo "${userObj}" | jq 'try .user.email catch false | type != "null"' | tr '[:upper:]' '[:lower:]')
echo "Has user email:"
echo "${hasUserEmail}"
echo "JSON property access tests completed!"
