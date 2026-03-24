#!/bin/bash

clientVer=$(kubectl version --client -o json 2>/dev/null | jq -r '.clientVersion.gitVersion')
echo "Client version: ${clientVer}"
serverVer=$(kubectl version -o json 2>/dev/null | jq -r '.serverVersion.gitVersion')
echo "Server version: ${serverVer}"
