#!/bin/bash

IFS=',' read -ra items <<< "apple,banana,cherry"
for item in "${items[@]}"; do
  echo "Item: ${item}"
done
