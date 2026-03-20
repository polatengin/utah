#!/bin/bash

numbers=(1 2 3 4 5)
doubled=($(_utah_map_source_1=("${numbers[@]}"); _utah_map_result_1=(); for ((_utah_map_index_1=0; _utah_map_index_1<${#_utah_map_source_1[@]}; _utah_map_index_1++)); do num="${_utah_map_source_1[_utah_map_index_1]}"; index=${_utah_map_index_1}; _utah_map_value_1=$(echo $((num * 2))); _utah_map_result_1+=("${_utah_map_value_1}"); done; printf '%s\n' "${_utah_map_result_1[@]}"))
echo "Doubled:"
echo "$(IFS=', '; printf '%s' "${doubled[*]}")"
evenNumbers=($(_utah_filter_source_2=("${numbers[@]}"); _utah_filter_result_2=(); for ((_utah_filter_index_2=0; _utah_filter_index_2<${#_utah_filter_source_2[@]}; _utah_filter_index_2++)); do num="${_utah_filter_source_2[_utah_filter_index_2]}"; _utah_filter_match_2=$([ $((num % 2)) -eq 0 ] && echo "true" || echo "false"); if [ "${_utah_filter_match_2}" = "true" ]; then _utah_filter_result_2+=("${num}"); fi; done; printf '%s\n' "${_utah_filter_result_2[@]}"))
echo "Even numbers:"
echo "$(IFS=', '; printf '%s' "${evenNumbers[*]}")"
total=$(_utah_reduce_source_3=("${numbers[@]}"); _utah_reduce_acc_3=0; for ((_utah_reduce_index_3=0; _utah_reduce_index_3<${#_utah_reduce_source_3[@]}; _utah_reduce_index_3++)); do acc="${_utah_reduce_acc_3}"; item="${_utah_reduce_source_3[_utah_reduce_index_3]}"; index=${_utah_reduce_index_3}; _utah_reduce_acc_3=$(echo $((acc + item))); done; echo "${_utah_reduce_acc_3}")
echo "Total:"
echo "${total}"
firstOverThree=$(_utah_find_source_4=("${numbers[@]}"); for ((_utah_find_index_4=0; _utah_find_index_4<${#_utah_find_source_4[@]}; _utah_find_index_4++)); do num="${_utah_find_source_4[_utah_find_index_4]}"; _utah_find_match_4=$([ ${num} -gt 3 ] && echo "true" || echo "false"); if [ "${_utah_find_match_4}" = "true" ]; then echo "${num}"; break; fi; done)
echo "First over three:"
echo "${firstOverThree}"
hasOdd=$(_utah_some_source_5=("${numbers[@]}"); for ((_utah_some_index_5=0; _utah_some_index_5<${#_utah_some_source_5[@]}; _utah_some_index_5++)); do num="${_utah_some_source_5[_utah_some_index_5]}"; _utah_some_match_5=$([ "$((num % 2))" != 0 ] && echo "true" || echo "false"); if [ "${_utah_some_match_5}" = "true" ]; then echo "true"; exit 0; fi; done; echo "false")
allPositive=$(_utah_every_source_6=("${numbers[@]}"); for ((_utah_every_index_6=0; _utah_every_index_6<${#_utah_every_source_6[@]}; _utah_every_index_6++)); do num="${_utah_every_source_6[_utah_every_index_6]}"; _utah_every_match_6=$([ ${num} -gt 0 ] && echo "true" || echo "false"); if [ "${_utah_every_match_6}" != "true" ]; then echo "false"; exit 0; fi; done; echo "true")
echo "Has odd:"
echo "${hasOdd}"
echo "All positive:"
echo "${allPositive}"
options=$(jq -cn '{"retries": 3, "timeout": 30}')
echo "Retries:"
echo "$(echo "${options}" | jq -cr --arg key "retries" '.[$key]')"
options=$(echo "${options}" | jq -c --arg key "retries" --argjson _utah_map_value_8 "5" '.[$key] = $_utah_map_value_8')
echo "Updated retries:"
echo "$(echo "${options}" | jq -cr --arg key "retries" '.[$key]')"
echo "Has timeout:"
echo "$(echo "${options}" | jq -cr --arg key "timeout" 'has($key)')"
labels=$(jq -cn '{"env": "prod", "region": "eu"}')
echo "Label env:"
echo "$(echo "${labels}" | jq -cr --arg key "env" '.[$key]')"
_utah_set_source_10=("alpha" "beta" "alpha" "release")
tags=($(printf '%s\n' "${_utah_set_source_10[@]}" | awk '!seen[$0]++'))
echo "Tags:"
echo "$(IFS=', '; printf '%s' "${tags[*]}")"
echo "Has beta:"
echo "$(_utah_set_has_source_11=("${tags[@]}"); _utah_set_has_needle_11="beta"; for _utah_set_has_item_11 in "${_utah_set_has_source_11[@]}"; do if [ "${_utah_set_has_item_11}" = "${_utah_set_has_needle_11}" ]; then echo "true"; exit 0; fi; done; echo "false")"
_utah_set_source_12=($(_utah_set_add_source_13=("${tags[@]}"); _utah_set_add_result_13=("${_utah_set_add_source_13[@]}"); _utah_set_add_value_13="stable"; _utah_set_add_exists_13=false; for _utah_set_add_item_13 in "${_utah_set_add_source_13[@]}"; do if [ "${_utah_set_add_item_13}" = "${_utah_set_add_value_13}" ]; then _utah_set_add_exists_13=true; break; fi; done; if [ "${_utah_set_add_exists_13}" != "true" ]; then _utah_set_add_result_13+=("${_utah_set_add_value_13}"); fi; printf '%s\n' "${_utah_set_add_result_13[@]}"))
extendedTags=($(printf '%s\n' "${_utah_set_source_12[@]}" | awk '!seen[$0]++'))
echo "Extended tags:"
echo "$(IFS=', '; printf '%s' "${extendedTags[*]}")"
_utah_set_source_14=($(_utah_set_remove_source_15=("${extendedTags[@]}"); _utah_set_remove_result_15=(); _utah_set_remove_value_15="alpha"; for _utah_set_remove_item_15 in "${_utah_set_remove_source_15[@]}"; do if [ "${_utah_set_remove_item_15}" != "${_utah_set_remove_value_15}" ]; then _utah_set_remove_result_15+=("${_utah_set_remove_item_15}"); fi; done; printf '%s\n' "${_utah_set_remove_result_15[@]}"))
removedTags=($(printf '%s\n' "${_utah_set_source_14[@]}" | awk '!seen[$0]++'))
echo "Removed alpha:"
echo "$(IFS=', '; printf '%s' "${removedTags[*]}")"
_utah_set_source_16=("release" "nightly" "stable")
moreTags=($(printf '%s\n' "${_utah_set_source_16[@]}" | awk '!seen[$0]++'))
_utah_set_source_17=($( _utah_set_union_left_18=("${tags[@]}"); _utah_set_union_right_18=("${moreTags[@]}"); printf '%s\n' "${_utah_set_union_left_18[@]}" "${_utah_set_union_right_18[@]}" | awk '!seen[$0]++' ))
unionTags=($(printf '%s\n' "${_utah_set_source_17[@]}" | awk '!seen[$0]++'))
_utah_set_source_19=($( _utah_set_intersection_left_20=("${tags[@]}"); _utah_set_intersection_right_20=("${moreTags[@]}"); _utah_set_intersection_result_20=(); for _utah_set_intersection_item_20 in "${_utah_set_intersection_left_20[@]}"; do _utah_set_intersection_found_20=false; for _utah_set_intersection_other_20 in "${_utah_set_intersection_right_20[@]}"; do if [ "${_utah_set_intersection_item_20}" = "${_utah_set_intersection_other_20}" ]; then _utah_set_intersection_found_20=true; break; fi; done; if [ "${_utah_set_intersection_found_20}" = "true" ]; then _utah_set_intersection_result_20+=("${_utah_set_intersection_item_20}"); fi; done; printf '%s\n' "${_utah_set_intersection_result_20[@]}" | awk '!seen[$0]++' ))
intersectionTags=($(printf '%s\n' "${_utah_set_source_19[@]}" | awk '!seen[$0]++'))
_utah_set_source_21=($( _utah_set_difference_left_22=("${unionTags[@]}"); _utah_set_difference_right_22=("${moreTags[@]}"); _utah_set_difference_result_22=(); for _utah_set_difference_item_22 in "${_utah_set_difference_left_22[@]}"; do _utah_set_difference_found_22=false; for _utah_set_difference_other_22 in "${_utah_set_difference_right_22[@]}"; do if [ "${_utah_set_difference_item_22}" = "${_utah_set_difference_other_22}" ]; then _utah_set_difference_found_22=true; break; fi; done; if [ "${_utah_set_difference_found_22}" != "true" ]; then _utah_set_difference_result_22+=("${_utah_set_difference_item_22}"); fi; done; printf '%s\n' "${_utah_set_difference_result_22[@]}" | awk '!seen[$0]++' ))
differenceTags=($(printf '%s\n' "${_utah_set_source_21[@]}" | awk '!seen[$0]++'))
echo "Union tags:"
echo "$(IFS=', '; printf '%s' "${unionTags[*]}")"
echo "Intersection tags:"
echo "$(IFS=', '; printf '%s' "${intersectionTags[*]}")"
echo "Difference tags:"
echo "$(IFS=', '; printf '%s' "${differenceTags[*]}")"
