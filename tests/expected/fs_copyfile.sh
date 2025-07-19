#!/bin/bash

mkdir -p $(dirname "target.txt")
cp "source.txt" "target.txt"
sourcePath="data/input.log"
targetPath="backup/input.log"
mkdir -p $(dirname ${targetPath})
cp ${sourcePath} ${targetPath}
success=$(mkdir -p $(dirname "settings/config.json") && cp "config.json" "settings/config.json" && echo "true" || echo "false")
echo "${success}"
if [ $(mkdir -p $(dirname "public/index.html") && cp "template.html" "public/index.html" && echo "true" || echo "false") ]; then
  echo "File copied successfully"
else
  echo "File copy failed"
fi
mkdir -p $(dirname "docs/project/README.md")
cp "README.md" "docs/project/README.md"
filename="important.txt"
backupDir="backups"
mkdir -p $(dirname $(({backupDir}/ + filename)))
cp ${filename} $(({backupDir}/ + filename))
