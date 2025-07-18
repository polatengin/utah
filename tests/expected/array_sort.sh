#!/bin/bash

numbers=(3 1 4 1 5 9 2 6)
numbersAsc=$(_utah_sort_1=(); while IFS= read -r line; do _utah_sort_1+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_1[@]}")
echo "Numbers ascending: $(_utah_sep_1=", "; _utah_join_1=""; for item in "${numbersAsc[@]}"; do if [ -n "${_utah_join_1}" ]; then _utah_join_1+="${_utah_sep_1}"; fi; _utah_join_1+="$item"; done; echo "${_utah_join_1}")"
numbersAscExplicit=$(_utah_sort_2=(); while IFS= read -r line; do _utah_sort_2+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_2[@]}")
echo "Numbers ascending explicit: $(_utah_sep_2=", "; _utah_join_2=""; for item in "${numbersAscExplicit[@]}"; do if [ -n "${_utah_join_2}" ]; then _utah_join_2+="${_utah_sep_2}"; fi; _utah_join_2+="$item"; done; echo "${_utah_join_2}")"
numbersDesc=$(_utah_sort_3=(); while IFS= read -r line; do _utah_sort_3+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -nr); echo "${_utah_sort_3[@]}")
echo "Numbers descending: $(_utah_sep_3=", "; _utah_join_3=""; for item in "${numbersDesc[@]}"; do if [ -n "${_utah_join_3}" ]; then _utah_join_3+="${_utah_sep_3}"; fi; _utah_join_3+="$item"; done; echo "${_utah_join_3}")"
fruits=("banana" "apple" "cherry" "date")
fruitsAsc=$(_utah_sort_4=(); while IFS= read -r line; do _utah_sort_4+=("$line"); done < <(printf '%s\n' "${fruits[@]}" | sort); echo "${_utah_sort_4[@]}")
echo "Fruits ascending: $(_utah_sep_4=", "; _utah_join_4=""; for item in "${fruitsAsc[@]}"; do if [ -n "${_utah_join_4}" ]; then _utah_join_4+="${_utah_sep_4}"; fi; _utah_join_4+="$item"; done; echo "${_utah_join_4}")"
fruitsDesc=$(_utah_sort_5=(); while IFS= read -r line; do _utah_sort_5+=("$line"); done < <(printf '%s\n' "${fruits[@]}" | sort -r); echo "${_utah_sort_5[@]}")
echo "Fruits descending: $(_utah_sep_5=", "; _utah_join_5=""; for item in "${fruitsDesc[@]}"; do if [ -n "${_utah_join_5}" ]; then _utah_join_5+="${_utah_sep_5}"; fi; _utah_join_5+="$item"; done; echo "${_utah_join_5}")"
flags=(true false true false true)
flagsAsc=$(_utah_sort_6=(); while IFS= read -r line; do _utah_sort_6+=("$line"); done < <(printf '%s\n' "${flags[@]}" | sed 's/true/1/g; s/false/0/g' | sort -n | sed 's/1/true/g; s/0/false/g'); echo "${_utah_sort_6[@]}")
echo "Flags ascending: $(_utah_sep_6=", "; _utah_join_6=""; for item in "${flagsAsc[@]}"; do if [ -n "${_utah_join_6}" ]; then _utah_join_6+="${_utah_sep_6}"; fi; _utah_join_6+="$item"; done; echo "${_utah_join_6}")"
flagsDesc=$(_utah_sort_7=(); while IFS= read -r line; do _utah_sort_7+=("$line"); done < <(printf '%s\n' "${flags[@]}" | sed 's/true/1/g; s/false/0/g' | sort -nr | sed 's/1/true/g; s/0/false/g'); echo "${_utah_sort_7[@]}")
echo "Flags descending: $(_utah_sep_7=", "; _utah_join_7=""; for item in "${flagsDesc[@]}"; do if [ -n "${_utah_join_7}" ]; then _utah_join_7+="${_utah_sep_7}"; fi; _utah_join_7+="$item"; done; echo "${_utah_join_7}")"
empty=()
emptySorted=$(_utah_sort_8=(); while IFS= read -r line; do _utah_sort_8+=("$line"); done < <(printf '%s\n' "${empty[@]}" | sort); echo "${_utah_sort_8[@]}")
echo "Empty array sorted: $(_utah_sep_8=", "; _utah_join_8=""; for item in "${emptySorted[@]}"; do if [ -n "${_utah_join_8}" ]; then _utah_join_8+="${_utah_sep_8}"; fi; _utah_join_8+="$item"; done; echo "${_utah_join_8}")"
single=(42)
singleSorted=$(_utah_sort_9=(); while IFS= read -r line; do _utah_sort_9+=("$line"); done < <(printf '%s\n' "${single[@]}" | sort -n); echo "${_utah_sort_9[@]}")
echo "Single element sorted: $(_utah_sep_9=", "; _utah_join_9=""; for item in "${singleSorted[@]}"; do if [ -n "${_utah_join_9}" ]; then _utah_join_9+="${_utah_sep_9}"; fi; _utah_join_9+="$item"; done; echo "${_utah_join_9}")"
numbersDescSorted=$(_utah_sort_10=(); while IFS= read -r line; do _utah_sort_10+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -nr); echo "${_utah_sort_10[@]}")
maxNumber="${numbersDescSorted[0]}"
echo "Maximum number: ${maxNumber}"
numbersAscSorted=$(_utah_sort_11=(); while IFS= read -r line; do _utah_sort_11+=("$line"); done < <(printf '%s\n' "${numbers[@]}" | sort -n); echo "${_utah_sort_11[@]}")
minNumber="${numbersAscSorted[0]}"
echo "Minimum number: ${minNumber}"
echo "Original numbers: $(_utah_sep_10=", "; _utah_join_10=""; for item in "${numbers[@]}"; do if [ -n "${_utah_join_10}" ]; then _utah_join_10+="${_utah_sep_10}"; fi; _utah_join_10+="$item"; done; echo "${_utah_join_10}")"
