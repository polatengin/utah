#!/bin/bash

numbers=(3 1 4 1 5 9 2 6)
numbersAsc=$(_utah_sort_1=(); while IFS= read -r line; do _utah_sort_1+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_1[@]}")
echo "Numbers ascending: $(IFS=', '; printf '%s' "${numbersAsc[*]}")"
numbersAscExplicit=$(_utah_sort_2=(); while IFS= read -r line; do _utah_sort_2+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_2[@]}")
echo "Numbers ascending explicit: $(IFS=', '; printf '%s' "${numbersAscExplicit[*]}")"
numbersDesc=$(_utah_sort_3=(); while IFS= read -r line; do _utah_sort_3+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -nr); echo "${_utah_sort_3[@]}")
echo "Numbers descending: $(IFS=', '; printf '%s' "${numbersDesc[*]}")"
fruits=("banana" "apple" "cherry" "date")
fruitsAsc=$(_utah_sort_4=(); while IFS= read -r line; do _utah_sort_4+=("$line"); done < <(printf '%s\n' "${fruits[@]}" | sort); echo "${_utah_sort_4[@]}")
echo "Fruits ascending: $(IFS=', '; printf '%s' "${fruitsAsc[*]}")"
fruitsDesc=$(_utah_sort_5=(); while IFS= read -r line; do _utah_sort_5+=("$line"); done < <(printf '%s\n' "${fruits[@]}" | sort -r); echo "${_utah_sort_5[@]}")
echo "Fruits descending: $(IFS=', '; printf '%s' "${fruitsDesc[*]}")"
flags=(true false true false true)
flagsAsc=$(_utah_sort_6=(); while IFS= read -r line; do _utah_sort_6+=("$line"); done < <(printf '%s\n' "${flags[@]}" | sed 's/true/1/g; s/false/0/g' | sort -n | sed 's/1/true/g; s/0/false/g'); echo "${_utah_sort_6[@]}")
echo "Flags ascending: $(IFS=', '; printf '%s' "${flagsAsc[*]}")"
flagsDesc=$(_utah_sort_7=(); while IFS= read -r line; do _utah_sort_7+=("$line"); done < <(printf '%s\n' "${flags[@]}" | sed 's/true/1/g; s/false/0/g' | sort -nr | sed 's/1/true/g; s/0/false/g'); echo "${_utah_sort_7[@]}")
echo "Flags descending: $(IFS=', '; printf '%s' "${flagsDesc[*]}")"
empty=()
emptySorted=$(_utah_sort_8=(); while IFS= read -r line; do _utah_sort_8+=("$line"); done < <(printf '%s\n' "${empty[@]}" | sort); echo "${_utah_sort_8[@]}")
echo "Empty array sorted: $(IFS=', '; printf '%s' "${emptySorted[*]}")"
single=(42)
singleSorted=$(_utah_sort_9=(); while IFS= read -r line; do _utah_sort_9+=("$line"); done < <(printf '%s\n' "${single[@]}" | sort -n); echo "${_utah_sort_9[@]}")
echo "Single element sorted: $(IFS=', '; printf '%s' "${singleSorted[*]}")"
numbersDescSorted=$(_utah_sort_10=(); while IFS= read -r line; do _utah_sort_10+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -nr); echo "${_utah_sort_10[@]}")
maxNumber="${numbersDescSorted[0]}"
echo "Maximum number: ${maxNumber}"
numbersAscSorted=$(_utah_sort_11=(); while IFS= read -r line; do _utah_sort_11+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_11[@]}")
minNumber="${numbersAscSorted[0]}"
echo "Minimum number: ${minNumber}"
echo "Original numbers: $(IFS=', '; printf '%s' "${numbers[*]}")"
