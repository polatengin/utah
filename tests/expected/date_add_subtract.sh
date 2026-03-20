#!/bin/bash

ts=$(date +%s)
future=$((${ts} + 7 * 86400))
echo "Future: ${future}"
past=$((${ts} - 2 * 3600))
echo "Past: ${past}"
