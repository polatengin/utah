#!/bin/bash

numbers=(3 1 4 1 5 9 2 6)
numbersAsc=($(printf '%s\n' "${numbers[@]}" | sort))
echo "Numbers ascending: $(IFS=', '; printf '%s' "${numbersAsc[*]}")"
numbersAscExplicit=($(printf '%s\n' "${numbers[@]}" | sort))
echo "Numbers ascending explicit: $(IFS=', '; printf '%s' "${numbersAscExplicit[*]}")"
numbersDesc=($(printf '%s\n' "${numbers[@]}" | sort -r))
echo "Numbers descending: $(IFS=', '; printf '%s' "${numbersDesc[*]}")"
fruits=("banana" "apple" "cherry" "date")
fruitsAsc=($(printf '%s\n' "${fruits[@]}" | sort))
echo "Fruits ascending: $(IFS=', '; printf '%s' "${fruitsAsc[*]}")"
fruitsDesc=($(printf '%s\n' "${fruits[@]}" | sort -r))
echo "Fruits descending: $(IFS=', '; printf '%s' "${fruitsDesc[*]}")"
flags=(true false true false true)
flagsAsc=($(printf '%s\n' "${flags[@]}" | sort))
echo "Flags ascending: $(IFS=', '; printf '%s' "${flagsAsc[*]}")"
flagsDesc=($(printf '%s\n' "${flags[@]}" | sort -r))
echo "Flags descending: $(IFS=', '; printf '%s' "${flagsDesc[*]}")"
empty=()
emptySorted=($(printf '%s\n' "${empty[@]}" | sort))
echo "Empty array sorted: $(IFS=', '; printf '%s' "${emptySorted[*]}")"
single=(42)
singleSorted=($(printf '%s\n' "${single[@]}" | sort))
echo "Single element sorted: $(IFS=', '; printf '%s' "${singleSorted[*]}")"
numbersDescSorted=($(printf '%s\n' "${numbers[@]}" | sort -r))
maxNumber="${numbersDescSorted[0]}"
echo "Maximum number: ${maxNumber}"
numbersAscSorted=($(printf '%s\n' "${numbers[@]}" | sort))
minNumber="${numbersAscSorted[0]}"
echo "Minimum number: ${minNumber}"
echo "Original numbers: $(IFS=', '; printf '%s' "${numbers[*]}")"
