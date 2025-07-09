#!/bin/sh

readonly x=10
readonly result=$([ ${x} -gt 5 ] && echo $((x * 2)) || echo $((x / 2)))
echo "Result: ${result}"
status=$([ ${x} -ge 10 ] && echo "high" || echo "low")
echo "Status: ${status}"
readonly value=75
test=$([ ${value} -gt 80 ] && echo "crit" || [ ${value} -gt 60 ] && echo "warn" || echo "ok")
