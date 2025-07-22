#!/bin/bash

testMultipleDefers() {
  echo "function body"

  # Defer cleanup function
  _utah_defer_cleanup_testMultipleDefers_1() {
    echo "first" || true
    echo "second" || true
    echo "third" || true
  }

  # Set up trap to execute defer statements on function exit
  trap '_utah_defer_cleanup_testMultipleDefers_1' RETURN
}
testMultipleDefers
