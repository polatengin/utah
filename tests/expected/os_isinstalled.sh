#!/bin/bash

gitInstalled=$(command -v "git" &> /dev/null && echo "true" || echo "false")
if [ "${gitInstalled}" = "true" ]; then
  echo "Git is installed"
else
  echo "Git is not installed"
fi
readonly applications=("docker" "nginx" "git" "curl")
for app in "${applications[@]}"; do
  isInstalled=$(command -v ${app} &> /dev/null && echo "true" || echo "false")
  status=$([ "${isInstalled}" = "true" ] && echo "✅ INSTALLED" || echo "❌ MISSING")
  echo "${status}: ${app}"
done
