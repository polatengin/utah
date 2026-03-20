#!/bin/bash

today=$(date +%A)
echo "Today is: ${today}"
ts=$(date +%s)
day=$(date -d @${ts} +%A)
echo "Day: ${day}"
