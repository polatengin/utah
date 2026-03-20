#!/bin/bash

formatted=$(date +"%Y-%m-%d %H:%M:%S")
echo "Current: ${formatted}"
ts=$(date +%s)
formatted2=$(date -d @${ts} +"%Y-%m-%d %H:%M:%S")
echo "Formatted: ${formatted2}"
custom=$(date -d @${ts} +"%Y-%m-%d")
echo "Custom: ${custom}"
