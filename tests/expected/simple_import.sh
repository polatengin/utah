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
result=$(add 5 3)
squared=$(square 4)
product=$(multiply 6 7)
echo "Addition result: ${result}"
echo "Square of 4: ${squared}"
echo "Product of 6 and 7: ${product}"
echo "${sharedMessage}"
echo "Math constant: ${mathConstant}"
greeting=$(greet "Utah Language")
echo "${greeting}"
