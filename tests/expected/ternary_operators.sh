#!/bin/sh

readonly x=10
readonly result=$([ $x -gt 5 ] && echo $((x * 2)) || echo $((x / 2)))
echo "Result: $result"
status=$([ $x -ge 10 ] && echo "high" || echo "low")
echo "Status: $status"
