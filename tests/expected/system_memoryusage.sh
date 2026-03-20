#!/bin/bash

memUsage=$(free 2>/dev/null | awk 'NR==2 {printf("%d", ($3/$2)*100)}' || echo "0")
echo "Memory usage: ${memUsage}%"
