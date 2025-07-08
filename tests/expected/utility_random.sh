#!/bin/sh

randomDefault=$(_utah_random_min_1=0; _utah_random_max_1=32767; ((_utah_random_max_1>_utah_random_min_1)) && echo $((RANDOM % (_utah_random_max_1 - _utah_random_min_1 + 1) + _utah_random_min_1)))
echo "Random default: ${randomDefault}"
randomMax=$(_utah_random_min_2=0; _utah_random_max_2=100; ((_utah_random_max_2>_utah_random_min_2)) && echo $((RANDOM % (_utah_random_max_2 - _utah_random_min_2 + 1) + _utah_random_min_2)))
echo "Random 0-100: ${randomMax}"
randomRange=$(_utah_random_min_3=50; _utah_random_max_3=150; ((_utah_random_max_3>_utah_random_min_3)) && echo $((RANDOM % (_utah_random_max_3 - _utah_random_min_3 + 1) + _utah_random_min_3)))
echo "Random 50-150: ${randomRange}"
minVal=10
maxVal=20
randomVar=$(_utah_random_min_4=${minVal}; _utah_random_max_4=${maxVal}; ((_utah_random_max_4>_utah_random_min_4)) && echo $((RANDOM % (_utah_random_max_4 - _utah_random_min_4 + 1) + _utah_random_min_4)))
echo "Random with vars: ${randomVar}"
