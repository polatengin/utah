#!/bin/sh

name="Utah"
echo "Language: ${name}"
timestamp=$(ps -o etime -p $$ --no-headers | tr -d ' ')
value=80
logEntry="[${timestamp}] MEMORY: ${value}%"
i=1
result=[ ${i} -le 3 ]
