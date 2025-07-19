#!/bin/bash

rm -rf "temp.txt"
filePath="data/old-file.log"
rm -rf ${filePath}
rm -rf "old-folder"
success=$(rm -rf "config.json" && echo "true" || echo "false")
echo "${success}"
if [ $(rm -rf "template.html" && echo "true" || echo "false") ]; then
  echo "File deleted successfully"
else
  echo "File delete failed"
fi
rm -rf "logs/archive/old-data.csv"
filename="temporary.txt"
tempDir="temp"
rm -rf $(({tempDir}/ + filename))
timestamp="2024-01-15"
rm -rf "backups/backup-${timestamp}.tar.gz"
projectDir="old-projects"
projectName="deprecated"
fileExt=".zip"
rm -rf $(((({projectDir}/ + projectName)) + fileExt))
