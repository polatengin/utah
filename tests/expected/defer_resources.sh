#!/bin/bash

processData() {
  echo "temporary data" > "temp.txt"
  echo "processing data"

  # Defer cleanup function
  _utah_defer_cleanup_processData_1() {
    echo "processing complete" || true
    rm -rf "temp.txt" || true
  }

  # Set up trap to execute defer statements on function exit
  trap '_utah_defer_cleanup_processData_1' RETURN
}
processData
