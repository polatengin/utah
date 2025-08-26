#!/bin/bash

emptyString=""
emptyCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${emptyString}
)
echo "Empty string: ${emptyCheck}"
nonEmptyString="hello"
nonEmptyCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${nonEmptyString}
)
echo "Non-empty string: ${nonEmptyCheck}"
whitespaceString="   "
whitespaceCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${whitespaceString}
)
echo "Whitespace string: ${whitespaceCheck}"
emptyArray=()
emptyArrayCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${emptyArray}
)
echo "Empty array: ${emptyArrayCheck}"
filledArray=("item")
filledArrayCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${filledArray}
)
echo "Filled array: ${filledArrayCheck}"
arrayWithEmptyString=("")
arrayWithEmptyStringCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${arrayWithEmptyString}
)
echo "Array with empty string: ${arrayWithEmptyStringCheck}"
emptyNumbers=()
emptyNumbersCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${emptyNumbers}
)
echo "Empty number array: ${emptyNumbersCheck}"
numbersWithZero=(0)
numbersWithZeroCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${numbersWithZero}
)
echo "Number array with zero: ${numbersWithZeroCheck}"
emptyBooleans=()
emptyBooleansCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${emptyBooleans}
)
echo "Empty boolean array: ${emptyBooleansCheck}"
booleansWithFalse=(false)
booleansWithFalseCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${booleansWithFalse}
)
echo "Boolean array with false: ${booleansWithFalseCheck}"
if [ $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ""
) ]; then
  echo "Empty string detected in conditional"
fi
if [ ! $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ("a" "b")
) ]; then
  echo "Array has contents"
fi
userInput=""
if [ $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${userInput}
) ]; then
  echo "User provided no input"
else
  echo "User provided input: ${userInput}"
fi
dataList=()
if [ $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${dataList}
) ]; then
  echo "Data list is empty, initializing defaults"
else
  echo "Data list has items"
fi
stringIsEmpty=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ""
)
arrayIsEmpty=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ()
)
stringIsNotEmpty=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty "test"
)
arrayIsNotEmpty=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ("test")
)
echo "String isEmpty result: ${stringIsEmpty}"
echo "Array isEmpty result: ${arrayIsEmpty}"
echo "String isNotEmpty result: ${stringIsNotEmpty}"
echo "Array isNotEmpty result: ${arrayIsNotEmpty}"
singleChar="a"
singleCharCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${singleChar}
)
echo "Single character: ${singleCharCheck}"
numberString="0"
numberStringCheck=$(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${numberString}
)
echo "Number as string: ${numberStringCheck}"
config=()
hasDefaults=true
if [ $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ${config}
) && ${hasDefaults} ]; then
  echo "Using default configuration"
else
  echo "Using custom configuration"
fi
echo "Direct call result: $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty "test"
)"
echo "Direct array call result: $(
_utah_validate_empty() {
  local val="$1"
  # Check if it's an empty string
  [ -z "$val" ] && echo "true" && return
  # Check if it's an empty array (empty parentheses with optional whitespace)
  if [[ "$val" =~ ^[[:space:]]*\(\)[[:space:]]*$ ]]; then
    echo "true" && return
  fi
  # Check if it's the literal string '()'
  [ "$val" = "()" ] && echo "true" && return
  echo "false"
}
_utah_validate_empty ()
)"
