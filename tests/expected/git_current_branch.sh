#!/bin/bash

branch=$(git rev-parse --abbrev-ref HEAD)
echo "Current branch: ${branch}"
