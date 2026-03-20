#!/bin/bash

clean=$([ -z "$(git status --porcelain)" ] && echo "true" || echo "false")
if [ "${clean}" = "true" ]; then
  echo "Working directory is clean"
else
  echo "Working directory has uncommitted changes"
fi
