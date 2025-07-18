#!/bin/bash

fruits=("apple" "banana" "cherry")
result0=$(_utah_sep_1=","; _utah_join_1=""; for item in "${fruits[@]}"; do if [ -n "${_utah_join_1}" ]; then _utah_join_1+="${_utah_sep_1}"; fi; _utah_join_1+="$item"; done; echo "${_utah_join_1}")
echo "Default separator: ${result0}"
result1=$(_utah_sep_2=","; _utah_join_2=""; for item in "${fruits[@]}"; do if [ -n "${_utah_join_2}" ]; then _utah_join_2+="${_utah_sep_2}"; fi; _utah_join_2+="$item"; done; echo "${_utah_join_2}")
echo "Comma separated: ${result1}"
result2=$(_utah_sep_3=" | "; _utah_join_3=""; for item in "${fruits[@]}"; do if [ -n "${_utah_join_3}" ]; then _utah_join_3+="${_utah_sep_3}"; fi; _utah_join_3+="$item"; done; echo "${_utah_join_3}")
echo "Pipe separated: ${result2}"
result3=$(_utah_sep_4=""; _utah_join_4=""; for item in "${fruits[@]}"; do if [ -n "${_utah_join_4}" ]; then _utah_join_4+="${_utah_sep_4}"; fi; _utah_join_4+="$item"; done; echo "${_utah_join_4}")
echo "No separator: ${result3}"
result4=$(_utah_sep_5="-"; _utah_join_5=""; for item in "${fruits[@]}"; do if [ -n "${_utah_join_5}" ]; then _utah_join_5+="${_utah_sep_5}"; fi; _utah_join_5+="$item"; done; echo "${_utah_join_5}")
echo "Dash separated: ${result4}"
numbers=(1 2 3 4 5)
numberResult=$(_utah_sep_6="-"; _utah_join_6=""; for item in "${numbers[@]}"; do if [ -n "${_utah_join_6}" ]; then _utah_join_6+="${_utah_sep_6}"; fi; _utah_join_6+="$item"; done; echo "${_utah_join_6}")
echo "Numbers joined: ${numberResult}"
flags=(true false true)
flagResult=$(_utah_sep_7=" & "; _utah_join_7=""; for item in "${flags[@]}"; do if [ -n "${_utah_join_7}" ]; then _utah_join_7+="${_utah_sep_7}"; fi; _utah_join_7+="$item"; done; echo "${_utah_join_7}")
echo "Booleans joined: ${flagResult}"
empty=()
emptyResult=$(_utah_sep_8=","; _utah_join_8=""; for item in "${empty[@]}"; do if [ -n "${_utah_join_8}" ]; then _utah_join_8+="${_utah_sep_8}"; fi; _utah_join_8+="$item"; done; echo "${_utah_join_8}")
echo "Empty array result: '${emptyResult}'"
single=("only")
singleResult=$(_utah_sep_9=","; _utah_join_9=""; for item in "${single[@]}"; do if [ -n "${_utah_join_9}" ]; then _utah_join_9+="${_utah_sep_9}"; fi; _utah_join_9+="$item"; done; echo "${_utah_join_9}")
echo "Single element: ${singleResult}"
