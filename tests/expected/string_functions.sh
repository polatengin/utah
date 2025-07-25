#!/bin/bash

text="Hello World"
length="${#text}"
upper="${text^^}"
lower="${text,,}"
echo "Text: ${text}"
echo "Length: ${length}"
echo "Upper: ${upper}"
echo "Lower: ${lower}"
logPath="/var/log/nginx.log"
echo "${logPath}"
if [ $([[ "${logPath}" == /var* ]] && echo "true" || echo "false") ]; then
  echo "Log is in var directory"
fi
filename="test.txt"
if [ $([[ "${filename}" == *.txt ]] && echo "true" || echo "false") ]; then
  echo "Text file detected"
fi
