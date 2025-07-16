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
