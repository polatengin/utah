#!/bin/bash

__UTAH_SCRIPT_DESCRIPTION="Test YAML dependency installation"
echo "Testing YAML dependency installation..."
installResult=$(
if ! command -v yq &> /dev/null; then
  echo "Installing yq for YAML processing..."
  if command -v snap &> /dev/null; then
    sudo snap install yq
  elif command -v brew &> /dev/null; then
    brew install yq
  elif command -v wget &> /dev/null; then
    sudo wget -qO /usr/local/bin/yq https://github.com/mikefarah/yq/releases/latest/download/yq_linux_amd64
    sudo chmod +x /usr/local/bin/yq
  else
    echo "Error: Unable to install yq. Please install it manually."
    exit 1
  fi
  if command -v yq &> /dev/null; then
    echo "yq installed successfully"
  else
    echo "Error: yq installation failed"
    exit 1
  fi
else
  echo "yq is already installed"
fi
if ! command -v jq &> /dev/null; then
  echo "Installing jq for JSON processing (required by YAML functions)..."
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
echo "YAML dependencies check result: ${installResult}"
yqAvailable=$(command -v "yq" &> /dev/null && echo "true" || echo "false")
jqAvailable=$(command -v "jq" &> /dev/null && echo "true" || echo "false")
echo "yq available: ${yqAvailable}"
echo "jq available: ${jqAvailable}"
if [ ${yqAvailable} && ${jqAvailable} ]; then
  echo "Testing YAML functions after dependency installation..."
  testYaml='name: test\nversion: 1.0.0\nfeatures:\n  - parsing\n  - validation'
  isValidYaml=$(echo ${testYaml} | yq empty >/dev/null 2>&1 && echo "true" || echo "false")
  echo "YAML validation works: ${isValidYaml}"
  if [ "${isValidYaml}" = "true" ]; then
    parsedYaml=$(echo ${testYaml} | yq -o=json .)
    appName=$(echo "${parsedYaml}" | yq -o=json . | jq -r '.name')
    version=$(echo "${parsedYaml}" | yq -o=json . | jq -r '.version')
    echo "Extracted name: ${appName}"
    echo "Extracted version: ${version}"
  fi
else
  echo "Warning: yq or jq not available, YAML functions may not work"
fi
echo "YAML dependency installation test completed"
