#!/bin/bash

randomDefault=$(_utah_random_min_1=0; _utah_random_max_1=32767; if [ $_utah_random_min_1 -gt $_utah_random_max_1 ]; then echo "Error: min value ($_utah_random_min_1) cannot be greater than max value ($_utah_random_max_1) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_1 - _utah_random_min_1 + 1) / 32768 + _utah_random_min_1)))
echo "Random default: ${randomDefault}"
randomMax=$(_utah_random_min_2=0; _utah_random_max_2=100; if [ $_utah_random_min_2 -gt $_utah_random_max_2 ]; then echo "Error: min value ($_utah_random_min_2) cannot be greater than max value ($_utah_random_max_2) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_2 - _utah_random_min_2 + 1) / 32768 + _utah_random_min_2)))
echo "Random 0-100: ${randomMax}"
randomRange=$(_utah_random_min_3=50; _utah_random_max_3=150; if [ $_utah_random_min_3 -gt $_utah_random_max_3 ]; then echo "Error: min value ($_utah_random_min_3) cannot be greater than max value ($_utah_random_max_3) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_3 - _utah_random_min_3 + 1) / 32768 + _utah_random_min_3)))
echo "Random 50-150: ${randomRange}"
minVal=10
maxVal=20
randomVar=$(_utah_random_min_4=${minVal}; _utah_random_max_4=${maxVal}; if [ $_utah_random_min_4 -gt $_utah_random_max_4 ]; then echo "Error: min value ($_utah_random_min_4) cannot be greater than max value ($_utah_random_max_4) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_4 - _utah_random_min_4 + 1) / 32768 + _utah_random_min_4)))
echo "Random with vars: ${randomVar}"
invalid=$(_utah_random_min_5=150; _utah_random_max_5=50; if [ $_utah_random_min_5 -gt $_utah_random_max_5 ]; then echo "Error: min value ($_utah_random_min_5) cannot be greater than max value ($_utah_random_max_5) in utility.random()" >&2; exit 100; fi; echo $((RANDOM * (_utah_random_max_5 - _utah_random_min_5 + 1) / 32768 + _utah_random_min_5)))
