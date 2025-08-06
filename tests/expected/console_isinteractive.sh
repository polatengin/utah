#!/bin/bash

isInteractive=$([ -t 0 ] && echo "true" || echo "false")
echo "${isInteractive}"
if [ "${isInteractive}" = "true" ]; then
  echo "Running in interactive terminal"
else
  echo "Running in non-interactive environment"
fi
