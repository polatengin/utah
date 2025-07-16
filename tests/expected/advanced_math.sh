#!/bin/bash

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
