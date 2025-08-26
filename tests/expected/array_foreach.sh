#!/bin/bash

__UTAH_SCRIPT_DESCRIPTION="Test script for array.forEach implementation"
fruits=("apple" "banana" "cherry")
echo "=== Basic forEach Test ==="
_utah_forEach_array_1=("${fruits[@]}")
for fruit in "${_utah_forEach_array_1[@]}"; do
  echo "Fruit: ${fruit}"
done
echo "=== forEach with Index Test ==="
_utah_forEach_array_2=("${numbers[@]}")
index=0
for num in "${_utah_forEach_array_2[@]}"; do
  echo "Index ${index}: ${num}"
  ((index++))
done
echo "=== Multi-line forEach Test ==="
_utah_forEach_array_3=("${users[@]}")
for user in "${_utah_forEach_array_3[@]}"; do
  echo "Processing user: ${user}"
  upperUser="${user^^}"
  echo "Uppercase: ${upperUser}"
done
_utah_forEach_array_4=($(IFS=','; read -ra SPLIT_ARRAY <<< "red,green,blue"; echo "${SPLIT_ARRAY[@]}"))
idx=0
for color in "${_utah_forEach_array_4[@]}"; do
  echo "Color ${idx}: ${color}"
  ((idx++))
done
echo "=== Nested Operations Test ==="
_utah_forEach_array_5=("${servers[@]}")
for server in "${_utah_forEach_array_5[@]}"; do
  echo "Checking server: ${server}"
  status="online"
  echo "Status of ${server}: ${status}"
done
