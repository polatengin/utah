#!/bin/sh

if command -v git &> /dev/null; then
  gitInstalled="true"
else
  gitInstalled="false"
fi
if [ "${gitInstalled}" = "true" ]; then
  echo "Git is installed"
else
  echo "Git is not installed"
fi
