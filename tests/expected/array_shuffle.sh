#!/bin/bash

numbers=(1 2 3 4 5)
shuffledNumbers=($(if command -v shuf &> /dev/null; then printf '%s\n' "${numbers[@]}" | shuf; else arr=("${numbers[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Original numbers: $(IFS=', '; printf '%s' "${numbers[*]}")"
echo "Shuffled numbers: $(IFS=', '; printf '%s' "${shuffledNumbers[*]}")"
fruits=("apple" "banana" "cherry" "date")
shuffledFruits=($(if command -v shuf &> /dev/null; then printf '%s\n' "${fruits[@]}" | shuf; else arr=("${fruits[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Original fruits: $(IFS=', '; printf '%s' "${fruits[*]}")"
echo "Shuffled fruits: $(IFS=', '; printf '%s' "${shuffledFruits[*]}")"
flags=(true false true false)
shuffledFlags=($(if command -v shuf &> /dev/null; then printf '%s\n' "${flags[@]}" | shuf; else arr=("${flags[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Original flags: $(IFS=', '; printf '%s' "${flags[*]}")"
echo "Shuffled flags: $(IFS=', '; printf '%s' "${shuffledFlags[*]}")"
empty=()
shuffledEmpty=($(if command -v shuf &> /dev/null; then printf '%s\n' "${empty[@]}" | shuf; else arr=("${empty[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Empty array shuffled: $(IFS=', '; printf '%s' "${shuffledEmpty[*]}")"
single=(42)
shuffledSingle=($(if command -v shuf &> /dev/null; then printf '%s\n' "${single[@]}" | shuf; else arr=("${single[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Single element shuffled: $(IFS=', '; printf '%s' "${shuffledSingle[*]}")"
echo "Verify original numbers unchanged: $(IFS=', '; printf '%s' "${numbers[*]}")"
echo "Verify original fruits unchanged: $(IFS=', '; printf '%s' "${fruits[*]}")"
echo "Verify original flags unchanged: $(IFS=', '; printf '%s' "${flags[*]}")"
sorted=$(_utah_sort_1=(3 1 4 1 5); tmp=(); while IFS= read -r line; do tmp+=("$line"); done < <(printf '%s\n' "${_utah_sort_1[@]}" | sort); echo "${tmp[@]}")
shuffledSorted=($(if command -v shuf &> /dev/null; then printf '%s\n' "${sorted[@]}" | shuf; else arr=("${sorted[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Sorted then shuffled: $(IFS=', '; printf '%s' "${shuffledSorted[*]}")"
shuffledForCheck=($(if command -v shuf &> /dev/null; then printf '%s\n' "${numbers[@]}" | shuf; else arr=("${numbers[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
if [ ! $([ ${#shuffledForCheck[@]} -eq 0 ] && echo "true" || echo "false") ]; then
  echo "Shuffled array is not empty"
fi
deck=("A" "K" "Q" "J")
shuffle1=($(if command -v shuf &> /dev/null; then printf '%s\n' "${deck[@]}" | shuf; else arr=("${deck[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
shuffle2=($(if command -v shuf &> /dev/null; then printf '%s\n' "${deck[@]}" | shuf; else arr=("${deck[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Shuffle 1: $(IFS=', '; printf '%s' "${shuffle1[*]}")"
echo "Shuffle 2: $(IFS=', '; printf '%s' "${shuffle2[*]}")"
moreNumbers=(10 20 30 40 50 60)
shuffledMore=($(if command -v shuf &> /dev/null; then printf '%s\n' "${moreNumbers[@]}" | shuf; else arr=("${moreNumbers[@]}"); for ((i=${#arr[@]}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp="${arr[i]}"; arr[i]="${arr[j]}"; arr[j]="$temp"; done; printf '%s\n' "${arr[@]}"; fi))
echo "Longer array shuffled: $(IFS=', '; printf '%s' "${shuffledMore[*]}")"
