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
