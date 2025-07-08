#!/bin/sh

numbers=(1 2 3 4 5)
names=("Alice" "Bob" "Charlie")
reversedNumbers=($(for ((i=${#numbers[@]}-1; i>=0; i--)); do echo "${numbers[i]}"; done))
reversedNames=($(for ((i=${#names[@]}-1; i>=0; i--)); do echo "${names[i]}"; done))
echo "Original numbers:"
for num in "${numbers[@]}"; do
  echo "${num}"
done
echo "Reversed numbers:"
for num in "${reversedNumbers[@]}"; do
  echo "${num}"
done
echo "Original names:"
for name in "${names[@]}"; do
  echo "${name}"
done
echo "Reversed names:"
for name in "${reversedNames[@]}"; do
  echo "${name}"
done
empty=()
reversedEmpty=($(for ((i=${#empty[@]}-1; i>=0; i--)); do echo "${empty[i]}"; done))
echo "Empty array reversed length:"
echo "${#reversedEmpty[@]}"
first="${reversedNumbers[0]}"
echo "First element of reversed numbers:"
echo "${first}"
