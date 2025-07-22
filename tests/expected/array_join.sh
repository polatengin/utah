#!/bin/bash

fruits=("apple" "banana" "cherry")
result0=$(IFS=','; printf '%s' "${fruits[*]}")
echo "Default separator: ${result0}"
result1=$(IFS=','; printf '%s' "${fruits[*]}")
echo "Comma separated: ${result1}"
result2=$(IFS=' | '; printf '%s' "${fruits[*]}")
echo "Pipe separated: ${result2}"
result3=$(printf '%s' "${fruits[@]}")
echo "No separator: ${result3}"
result4=$(IFS='-'; printf '%s' "${fruits[*]}")
echo "Dash separated: ${result4}"
numbers=(1 2 3 4 5)
numberResult=$(IFS='-'; printf '%s' "${numbers[*]}")
echo "Numbers joined: ${numberResult}"
flags=(true false true)
flagResult=$(IFS=' & '; printf '%s' "${flags[*]}")
echo "Booleans joined: ${flagResult}"
empty=()
emptyResult=$(IFS=','; printf '%s' "${empty[*]}")
echo "Empty array result: '${emptyResult}'"
single=("only")
singleResult=$(IFS=','; printf '%s' "${single[*]}")
echo "Single element: ${singleResult}"
