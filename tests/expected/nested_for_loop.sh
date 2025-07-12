#!/bin/bash

i=1
while [ ${i} -le 2 ]; do
  echo "Outer: ${i}"
  j=1
  while [ ${j} -le 2 ]; do
    echo "Inner: ${j}"
    j=$((j + 1))
  done
  echo "End outer: ${i}"
  i=$((i + 1))
done
