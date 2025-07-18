#!/bin/bash

export APP_NAME="MyApp"
export APP_VERSION="2.1.0"
export AUTHOR="Utah Team"
echo "App: ${APP_NAME}\nVersion: ${APP_VERSION}\nAuthor: ${AUTHOR}" > "config.template"
envsubst < "config.template" > "config.txt"
configContent=$(cat "config.txt")
echo ""Generated config:", configContent"
