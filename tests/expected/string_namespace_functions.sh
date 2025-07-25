#!/bin/bash

text="  Hello World  "
name="john"
emptyText=""
trimmed="$(echo "${text}" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')"
length="${#text}"
isEmptyText=$([[ -z "$(echo "${emptyText}" | xargs)" ]] && echo "true" || echo "false")
isEmptyTrimmed=$([[ -z "$(echo "${trimmed}" | xargs)" ]] && echo "true" || echo "false")
echo "=== Core Utilities ==="
echo "Original: '${text}'"
echo "Trimmed: '${trimmed}'"
echo "Length: ${length}"
echo "Empty text is empty: ${isEmptyText}"
echo "Trimmed is empty: ${isEmptyTrimmed}"
testString="hello WORLD"
upperCase="${testString^^}"
lowerCase="${testString,,}"
capitalized="$(echo "${testString}" | sed 's/^./\U&/')"
nameCapitalized="$(echo "${name}" | sed 's/^./\U&/')"
echo "=== Case Operations ==="
echo "Original: ${testString}"
echo "Upper: ${upperCase}"
echo "Lower: ${lowerCase}"
echo "Capitalized: ${capitalized}"
echo "Capitalized name: ${nameCapitalized}"
searchText="The quick brown fox"
startsWithThe=$([[ "${searchText}" == The* ]] && echo "true" || echo "false")
endsWithFox=$([[ "${searchText}" == *fox ]] && echo "true" || echo "false")
containsQuick=$(case "${searchText}" in *quick*) echo "true";; *) echo "false";; esac)
indexOfBrown=$(temp="${searchText}"; pos=${temp%%brown*}; [[ "$pos" == "${searchText}" ]] && echo "-1" || echo "${#pos}")
starts=$([[ "${text}" ==   Hello* ]] && echo "true" || echo "false")
ends=$([[ "${text}" == *   ]] && echo "true" || echo "false")
includes=$(case "${text}" in *World*) echo "true";; *) echo "false";; esac)
index=$(temp="${text}"; pos=${temp%%World*}; [[ "$pos" == "${text}" ]] && echo "-1" || echo "${#pos}")
echo "=== Search and Test Operations ==="
echo "Text: ${searchText}"
echo "Starts with 'The': ${startsWithThe}"
echo "Ends with 'fox': ${endsWithFox}"
echo "Contains 'quick': ${containsQuick}"
echo "Index of 'brown': ${indexOfBrown}"
echo "Text starts with '  Hello': ${starts}"
echo "Text ends with '  ': ${ends}"
echo "Text contains 'World': ${includes}"
echo "Index of 'World' in text: ${index}"
extractText="JavaScript"
substring="${extractText:0:4}"
substr="${text:2:5}"
slice="$(echo "${extractText}" | cut -c4-10)"
replaced="${extractText/Script/Code}"
replaced2="${text/World/Utah}"
replaceAll="$(echo "hello hello hello" | sed "s/hello/hi/g")"
multiword="hello hello world"
allReplaced="${multiword//hello/hi}"
sliced="$(echo "Hello World" | cut -c6-11)"
echo "=== Extraction and Manipulation ==="
echo "Original: ${extractText}"
echo "Substring(0,4): ${substring}"
echo "Substring from text(2,5): '${substr}'"
echo "Slice(4,10): ${slice}"
echo "Replace 'Script' with 'Code': ${replaced}"
echo "Replace 'World' with 'Utah': ${replaced2}"
echo "Replace all 'hello' with 'hi': ${replaceAll}"
echo "Advanced replace all: ${allReplaced}"
echo "Sliced: ${sliced}"
csvText="apple,banana,cherry"
IFS=',' read -ra fruits <<< "${csvText}"
IFS=' ' read -ra words <<< "${text}"
echo "=== Split Operations ==="
echo "CSV: ${csvText}"
echo "Split result: ${fruits}"
padText="42"
padStartResult="$(printf "%*s" 5 ${padText} | sed "s/ /0/g")"
padEndResult="$(printf "%-*s" 5 ${padText} | sed "s/ /-/g")"
repeatResult="$(for ((i=0; i<3; i++)); do printf "%s" "abc"; done)"
paddedStart="$(printf "%*s" 5 "42" | sed "s/ /0/g")"
paddedEnd="$(printf "%-*s" 5 "42" | sed "s/ /-/g")"
repeated="$(for ((i=0; i<3; i++)); do printf "%s" "abc"; done)"
echo "=== Advanced Operations ==="
echo "Pad text: ${padText}"
echo "Pad start (5, '0'): ${padStartResult}"
echo "Pad end (5, '-'): ${padEndResult}"
echo "Repeat 'abc' 3 times: ${repeatResult}"
echo "Additional padded start: ${paddedStart}"
echo "Additional padded end: ${paddedEnd}"
echo "Additional repeated: ${repeated}"
