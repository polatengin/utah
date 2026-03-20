#!/bin/bash

totalMem=$(free -m 2>/dev/null | awk 'NR==2 {print $2}' || sysctl -n hw.memsize 2>/dev/null | awk '{printf("%d", $1/1024/1024)}' || echo "0")
echo "Total memory: ${totalMem} MB"
