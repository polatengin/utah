#!/bin/bash

mkdir -p $(dirname "new/location/file.txt")
mv "old/location/file.txt" "new/location/file.txt"
sourcePath="data/input.log"
targetPath="processed/input.log"
mkdir -p $(dirname ${targetPath})
mv ${sourcePath} ${targetPath}
success=$(mkdir -p $(dirname "settings/config.json") && mv "config.json" "settings/config.json" && echo "true" || echo "false")
echo "${success}"
if [ $(mkdir -p $(dirname "public/index.html") && mv "template.html" "public/index.html" && echo "true" || echo "false") ]; then
  echo "File moved successfully"
else
  echo "File move failed"
fi
mkdir -p $(dirname "docs/project/README.md")
mv "README.md" "docs/project/README.md"
mkdir -p $(dirname "new-name.txt")
mv "old-name.txt" "new-name.txt"
filename="important.txt"
archiveDir="archive"
mkdir -p $(dirname $(({archiveDir}/ + filename)))
mv ${filename} $(({archiveDir}/ + filename))
timestamp="2024-01-15"
mkdir -p $(dirname "history/data-${timestamp}.csv")
mv "temp/data.csv" "history/data-${timestamp}.csv"
sourceDir="downloads"
targetDir="organized"
fileExt=".pdf"
mkdir -p $(dirname $(({targetDir}/documents/document + fileExt)))
mv $(({sourceDir}/document + fileExt)) $(({targetDir}/documents/document + fileExt))
