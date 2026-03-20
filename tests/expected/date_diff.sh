#!/bin/bash

ts1=$(date -d "2024-01-15" +%s)
ts2=$(date -d "2024-01-10" +%s)
diffSec=$((${ts1} - ${ts2}))
echo "Diff seconds: ${diffSec}"
diffDays=$(((${ts1} - ${ts2}) / 86400))
echo "Diff days: ${diffDays}"
