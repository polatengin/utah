#!/bin/sh

i=0
while [ ${i} -lt 5 ]; do
  echo "${i}"
  if [ ${i} -eq 3 ]; then
    break
  fi
  i=$((i + 1))
done
echo "done"
