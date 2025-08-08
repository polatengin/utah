#!/bin/bash

numbers=(3 1 4 1 5 9 2 6 5 3 5)
uniqueNumbers=($(declare -A _utah_seen; for item in "${numbers[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Original numbers: $(IFS=', '; printf '%s' "${numbers[*]}")"
echo "Unique numbers: $(IFS=', '; printf '%s' "${uniqueNumbers[*]}")"
fruits=("apple" "banana" "apple" "cherry" "banana" "date" "apple")
uniqueFruits=($(declare -A _utah_seen; for item in "${fruits[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Original fruits: $(IFS=', '; printf '%s' "${fruits[*]}")"
echo "Unique fruits: $(IFS=', '; printf '%s' "${uniqueFruits[*]}")"
flags=(true false true false true true false)
uniqueFlags=($(declare -A _utah_seen; for item in "${flags[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Original flags: $(IFS=', '; printf '%s' "${flags[*]}")"
echo "Unique flags: $(IFS=', '; printf '%s' "${uniqueFlags[*]}")"
empty=()
uniqueEmpty=($(declare -A _utah_seen; for item in "${empty[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Empty array unique: $(IFS=', '; printf '%s' "${uniqueEmpty[*]}")"
single=(42)
uniqueSingle=($(declare -A _utah_seen; for item in "${single[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Single element unique: $(IFS=', '; printf '%s' "${uniqueSingle[*]}")"
noDuplicates=("a" "b" "c" "d")
stillUnique=($(declare -A _utah_seen; for item in "${noDuplicates[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "No duplicates array: $(IFS=', '; printf '%s' "${stillUnique[*]}")"
allSame=(7 7 7 7)
onlyOne=($(declare -A _utah_seen; for item in "${allSame[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "All same elements: $(IFS=', '; printf '%s' "${onlyOne[*]}")"
orderTest=("c" "a" "b" "a" "c" "d" "b")
uniqueOrder=($(declare -A _utah_seen; for item in "${orderTest[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Order preservation test: $(IFS=', '; printf '%s' "${uniqueOrder[*]}")"
list1=(1 2 3)
list2=(2 3 4)
merged=($(printf '%s\n' "${list1[@]}" "${list2[@]}"))
uniqueMerged=($(declare -A _utah_seen; for item in "${merged[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Merged array: $(IFS=', '; printf '%s' "${merged[*]}")"
echo "Unique merged: $(IFS=', '; printf '%s' "${uniqueMerged[*]}")"
unsorted=(5 2 8 2 1 5 3)
uniqueUnsorted=($(declare -A _utah_seen; for item in "${unsorted[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
sortedUnique=$(_utah_sort_1=(); while IFS= read -r line; do _utah_sort_1+=("$line"); done < <(printf '%s\n' "${uniqueUnsorted[@]}" | sort); echo "${_utah_sort_1[@]}")
echo "Original unsorted: $(IFS=', '; printf '%s' "${unsorted[*]}")"
echo "Unique then sorted: $(IFS=', '; printf '%s' "${sortedUnique[*]}")"
sortedFirst=$(_utah_sort_2=(); while IFS= read -r line; do _utah_sort_2+=("$line"); done < <(printf '%s\n' "${unsorted[@]}" | sort); echo "${_utah_sort_2[@]}")
uniqueAfterSort=($(declare -A _utah_seen; for item in "${sortedFirst[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Sorted then unique: $(IFS=', '; printf '%s' "${uniqueAfterSort[*]}")"
testData=("test" "data" "test" "more" "data")
uniqueData=($(declare -A _utah_seen; for item in "${testData[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
if [ ${#uniqueData[@]} -lt ${#testData[@]} ]; then
  echo "Duplicates were removed successfully"
fi
echo "Original numbers unchanged: $(IFS=', '; printf '%s' "${numbers[*]}")"
echo "Original fruits unchanged: $(IFS=', '; printf '%s' "${fruits[*]}")"
moreNumbers=(10 20 10 30 20 40 30 50 40 60 50)
uniqueMore=($(declare -A _utah_seen; for item in "${moreNumbers[@]}"; do if [[ -z "${_utah_seen[$item]}" ]]; then _utah_seen["$item"]=1; echo "$item"; fi; done))
echo "Larger array unique: $(IFS=', '; printf '%s' "${uniqueMore[*]}")"
