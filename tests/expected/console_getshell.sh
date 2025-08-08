#!/bin/bash

shell=$(basename "${SHELL:-$0}")
echo "Detected shell: ${shell}"
if [ "${shell}" = "true" ]; then
  echo "Shell detection successful"
else
  echo "Shell detection failed"
fi
