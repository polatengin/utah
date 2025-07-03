#!/bin/sh

numbers=(1 2 3 4 5)
names=("Alice" "Bob" "Charlie")
first="${numbers[0]}"
second="${names[1]}"
count="${#numbers[@]}"
echo "First number:"
echo $first
echo "Second name:"
echo $second
echo "Array length:"
echo $count
echo "Numbers:"
for num in "${numbers[@]}"; do
  echo "$num"
done
echo "Names:"
for name in "${names[@]}"; do
  echo "$name"
done
data="red,green,blue"
IFS=',' read -ra colors <<< "${data}"
echo "Colors:"
for color in "${colors[@]}"; do
  echo "$color"
done
