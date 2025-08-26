#!/bin/bash

$(chmod "755" "script.sh" && echo "true" || echo "false")
filePath="config.txt"
permissions="644"
$(chmod ${permissions} ${filePath} && echo "true" || echo "false")
success=$(chmod "600" "data.log" && echo "true" || echo "false")
echo "${success}"
if [ $(chmod "400" "secrets.txt" && echo "true" || echo "false") ]; then
  echo "File permissions updated successfully"
else
  echo "Failed to update file permissions"
fi
$(chmod "755" "executable.sh" && echo "true" || echo "false")
$(chmod "600" "private.txt" && echo "true" || echo "false")
$(chmod "644" "public.txt" && echo "true" || echo "false")
$(chmod "444" "readonly.txt" && echo "true" || echo "false")
$(chmod "755" "directory" && echo "true" || echo "false")
$(chmod "u+x" "file1.txt" && echo "true" || echo "false")
$(chmod "g+w" "file2.txt" && echo "true" || echo "false")
$(chmod "o-r" "file3.txt" && echo "true" || echo "false")
$(chmod "a+r" "file4.txt" && echo "true" || echo "false")
$(chmod "u+rw" "file5.txt" && echo "true" || echo "false")
$(chmod "u+x" "script.py" && echo "true" || echo "false")
$(chmod "go-rwx" "config.conf" && echo "true" || echo "false")
scriptFile="automation.sh"
execPermissions="755"
$(chmod ${execPermissions} ${scriptFile} && echo "true" || echo "false")
configFile="app.conf"
if [ $([ -e ${configFile} ] && echo "true" || echo "false") ]; then
  $(chmod "600" ${configFile} && echo "true" || echo "false")
  echo "Config file secured"
fi
files=("file1.txt" "file2.txt" "file3.txt")
for file in "${files[@]}"; do
  result=$(chmod "644" ${file} && echo "true" || echo "false")
  echo "Permissions set for file: ${result}"
done
