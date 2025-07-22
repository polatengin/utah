#!/bin/bash

emptyArray=()
filledArray=("apple" "banana" "cherry")
emptyCheck=$([ ${#emptyArray[@]} -eq 0 ] && echo "true" || echo "false")
filledCheck=$([ ${#filledArray[@]} -eq 0 ] && echo "true" || echo "false")
echo "Empty array is empty: ${emptyCheck}"
echo "Filled array is empty: ${filledCheck}"
if [ $([ ${#emptyArray[@]} -eq 0 ] && echo "true" || echo "false") ]; then
  echo "The empty array is indeed empty"
fi
if [ ! $([ ${#filledArray[@]} -eq 0 ] && echo "true" || echo "false") ]; then
  echo "The filled array is not empty"
fi
