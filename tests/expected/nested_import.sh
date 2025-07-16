#!/bin/bash

add() {
  local a="$1"
  local b="$2"
  echo $((a + b))
}
greet() {
  local name="$1"
  echo "Hello, ${name}!"
}
sharedMessage="This message comes from utils.shx"
multiply() {
  local x="$1"
  local y="$2"
  echo $((x * y))
}
square() {
  local n="$1"
  echo "$(multiply "${n}" "${n}")"
}
mathConstant=42
cube() {
  local n="$1"
  echo "$(multiply "${n}" $(square "${n}"))"
}
factorial() {
  local n="$1"
  if [ ${n} -le 1 ]; then
    echo "1"
  else
    echo "$(multiply "${n}" $(factorial $((n - 1))))"
  fi
}
advancedConstant=100
echo "Testing nested imports:"
number=3
cubed=$(cube "${number}")
fact=$(factorial 4)
echo "Cube of ${number}: ${cubed}"
echo "Factorial of 4: ${fact}"
echo "Advanced constant: ${advancedConstant}"
echo "$(greet "Nested Imports")"
echo "${sharedMessage}"
