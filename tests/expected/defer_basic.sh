#!/bin/bash

testBasicDefer() {
  echo "function body"

  # Defer cleanup function
  _utah_defer_cleanup_testBasicDefer_1() {
    echo "cleanup executed" || true
  }

  # Set up trap to execute defer statements on function exit
  trap '_utah_defer_cleanup_testBasicDefer_1' RETURN
}
testBasicDefer
