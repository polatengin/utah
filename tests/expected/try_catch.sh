#!/bin/sh

_utah_try_block_1() {
  (
    set -e
    message="Success"
    echo "Try block executed successfully"
  )
}
utah_catch_1() {
  echo "This should not execute"
}
_utah_try_block_1 || utah_catch_1
_utah_try_block_2() {
  (
    set -e
    echo "Hope the following folder exists"
    ls "/non/existing/folder" || exit 1
    echo "this line doesn't executed"
  )
}
utah_catch_2() {
  echo "Error caught successfully"
}
_utah_try_block_2 || utah_catch_2
_utah_try_block_3() {
  (
    set -e
    echo "About to exit with error"
    exit 1
  )
}
utah_catch_3() {
  echo "Error caught successfully"
}
_utah_try_block_3 || utah_catch_3
echo "Script completed"
