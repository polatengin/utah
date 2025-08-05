#!/bin/bash

fruits=("apple" "banana")
vegetables=("carrot" "broccoli")
food=($(printf '%s\n' "${fruits[@]}" "${vegetables[@]}"))
echo "Merged fruits and vegetables:"
for item in "${food[@]}"; do
  echo "- ${item}"
done
numbers1=(1 2 3)
numbers2=(4 5 6)
allNumbers=($(printf '%s\n' "${numbers1[@]}" "${numbers2[@]}"))
echo "Merged numbers: $(IFS=', '; printf '%s' "${allNumbers[*]}")"
flags1=(true false)
flags2=(true true)
allFlags=($(printf '%s\n' "${flags1[@]}" "${flags2[@]}"))
echo "Merged flags: $(IFS=', '; printf '%s' "${allFlags[*]}")"
empty=()
items=("item1" "item2")
mergedWithEmpty1=($(printf '%s\n' "${empty[@]}" "${items[@]}"))
mergedWithEmpty2=($(printf '%s\n' "${items[@]}" "${empty[@]}"))
mergedBothEmpty=($(printf '%s\n' "${empty[@]}" "${empty[@]}"))
echo "Empty + items: $(IFS=', '; printf '%s' "${mergedWithEmpty1[*]}")"
echo "Items + empty: $(IFS=', '; printf '%s' "${mergedWithEmpty2[*]}")"
echo "Empty + empty: $(IFS=', '; printf '%s' "${mergedBothEmpty[*]}")"
moreNumbers1=(10 20)
moreNumbers2=(30 40 50)
combinedNumbers=($(printf '%s\n' "${moreNumbers1[@]}" "${moreNumbers2[@]}"))
echo "Combined longer arrays: $(IFS=' -> '; printf '%s' "${combinedNumbers[*]}")"
baseItems=("base1" "base2")
additionalItems=("add1" "add2")
if [ "$([ ${#additionalItems[@]} -eq 0 ] && echo "true" || echo "false")" = "false" ]; then
  allItems=($(printf '%s\n' "${baseItems[@]}" "${additionalItems[@]}"))
  echo "All items: $(IFS=', '; printf '%s' "${allItems[*]}")"
fi
