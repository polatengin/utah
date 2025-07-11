#!/bin/sh

numbers=(1 2 3 4 5)
names=("Alice" "Bob" "Charlie")
first="${numbers[0]}"
second="${names[1]}"
count=${#numbers[@]}
echo "First number:"
echo "${first}"
echo "Second name:"
echo "${second}"
echo "Array length:"
echo "${count}"
echo "Numbers:"
for num in "${numbers[@]}"; do
  echo "${num}"
done
echo "Names:"
for name in "${names[@]}"; do
  echo "${name}"
done
data="red,green,blue"
IFS=',' read -ra colors <<< "${data}"
echo "Colors:"
for color in "${colors[@]}"; do
  echo "${color}"
done
messages=("System monitoring complete! 🎉" "All systems checked! 🚀" "Health check finished! ✨" "Monitoring mission accomplished! 🏆")
randomIndex=$(_utah_random_min_1=0; _utah_random_max_1=$((${#messages[@]} - 1)); if [ $_utah_random_min_1 -gt $_utah_random_max_1 ]; then echo "Error: min value ($_utah_random_min_1) cannot be greater than max value ($_utah_random_max_1) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_1 - _utah_random_min_1 + 1) / 32768 + _utah_random_min_1)))
finalMessage="${messages[randomIndex]}"
