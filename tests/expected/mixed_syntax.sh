#!/bin/sh

name="Utah"
count=5
isActive=true
echo "Starting mixed syntax test"
echo "Name: ${name}"
echo "This is raw bash echo"
if [ "${isActive}" = "true" ]; then
  echo "System is active"
fi
if [ "$count" -lt 10 ]; then
echo "Count is less than 10 (bash syntax)"
fi
i=1
while [ ${i} -le 3 ]; do
  echo "Utah loop iteration: ${i}"
  i=$((i + 1))
done
counter=1
while [ $counter -le 2 ]; do
echo "Bash loop iteration: $counter"
counter=$((counter + 1))
done
greet() {
  local person="$1"
  echo "Hello ${person}"
}
bash_greet() {
echo "Bash says hello to $1"
}
greeting=$(greet "${name}")
echo "${greeting}"
bash_greet "World"
result="Test completed"
echo "${result}"
