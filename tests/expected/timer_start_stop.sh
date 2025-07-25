#!/bin/bash

_utah_timer_start=$(date +%s%3N)
echo "Timer started"
i=0
j=0
while [ ${j} -lt 1000000 ]; do
  i=$((i + 1))
  j=$((j + 1))
done
_utah_timer_end=$(date +%s%3N)
readonly elapsed=$((_utah_timer_end - _utah_timer_start))
echo "Timer elapsed: ${elapsed} ms"
