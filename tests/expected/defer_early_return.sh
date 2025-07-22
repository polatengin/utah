#!/bin/bash

testEarlyReturn() {
  local shouldReturn="$1"
  echo "starting function"
  if [ "${shouldReturn}" = "true" ]; then
    echo "early return"
    return
  fi
  echo "normal execution"

  # Defer cleanup function
  _utah_defer_cleanup_testEarlyReturn_1() {
    echo "cleanup always runs" || true
  }

  # Set up trap to execute defer statements on function exit
  trap '_utah_defer_cleanup_testEarlyReturn_1' RETURN
}
testEarlyReturn true
testEarlyReturn false
