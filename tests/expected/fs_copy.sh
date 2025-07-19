#!/bin/bash

mkdir -p $(dirname "target.txt")
cp -r "source.txt" "target.txt"
sourcePath="data/input.log"
targetPath="backup/input.log"
mkdir -p $(dirname ${targetPath})
cp -r ${sourcePath} ${targetPath}
success=$(mkdir -p $(dirname "settings/config.json") && cp -r "config.json" "settings/config.json" && echo "true" || echo "false")
echo "${success}"
if [ $(mkdir -p $(dirname "public/index.html") && cp -r "template.html" "public/index.html" && echo "true" || echo "false") ]; then
  echo "File copied successfully"
else
  echo "File copy failed"
fi
mkdir -p $(dirname "docs/project/README.md")
cp -r "README.md" "docs/project/README.md"
filename="important.txt"
backupDir="backups"
mkdir -p $(dirname $(({backupDir}/ + filename)))
cp -r ${filename} $(({backupDir}/ + filename))
