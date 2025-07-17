#!/bin/bash

__UTAH_SCRIPT_DESCRIPTION="Test JSON dependency installation"
echo "Testing JSON dependency installation..."
installResult=$(
if ! command -v jq &> /dev/null; then
  echo "Installing jq for JSON processing..."
  if command -v apt-get &> /dev/null; then
    sudo apt-get update && sudo apt-get install -y jq
  elif command -v yum &> /dev/null; then
    sudo yum install -y jq
  elif command -v dnf &> /dev/null; then
    sudo dnf install -y jq
  elif command -v brew &> /dev/null; then
    brew install jq
  elif command -v pacman &> /dev/null; then
    sudo pacman -S --noconfirm jq
  else
    echo "Error: Unable to install jq. Please install it manually."
    exit 1
  fi
  if command -v jq &> /dev/null; then
    echo "jq installed successfully"
  else
    echo "Error: jq installation failed"
    exit 1
  fi
else
  echo "jq is already installed"
fi
)
echo "JSON dependencies check result: ${installResult}"
jqAvailable=$(command -v "jq" &> /dev/null && echo "true" || echo "false")
echo "jq available: ${jqAvailable}"
if [ "${jqAvailable}" = "true" ]; then
  echo "Testing JSON functions after dependency installation..."
  testJson='{"name": "test", "version": "1.0.0"}'
  isValidJson=$(echo ${testJson} | jq empty >/dev/null 2>&1 && echo "true" || echo "false")
  echo "JSON validation works: ${isValidJson}"
  if [ "${isValidJson}" = "true" ]; then
    parsedJson=$(echo ${testJson} | jq .)
    appName=$(echo "${parsedJson}" | jq -r '.name')
    echo "Extracted name: ${appName}"
  fi
else
  echo "Warning: jq not available, JSON functions may not work"
fi
echo "JSON dependency installation test completed"
