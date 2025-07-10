#!/bin/sh

slowEcho() {
  local msg="$1"
  echo "Start: ${msg}"
  _="$(sleep 1)"
  echo "End: ${msg}"
}
echo "Main start"
slowEcho "A" &
slowEcho "B" &
echo "Main end"
_="$(wait)"
echo "All done"
