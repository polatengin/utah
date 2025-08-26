#!/bin/bash

score1=85
passing1=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${score1} 70
)
echo "Score 85 > 70: ${passing1}"
score2=65
passing2=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${score2} 70
)
echo "Score 65 > 70: ${passing2}"
score3=70
passing3=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${score3} 70
)
echo "Score 70 > 70: ${passing3}"
temp1=98.7
fever1=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${temp1} 98.6
)
echo "Temperature 98.7 > 98.6: ${fever1}"
temp2=98.5
fever2=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${temp2} 98.6
)
echo "Temperature 98.5 > 98.6: ${fever2}"
price1=5.1
expensive1=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${price1} 5
)
echo "Price 5.1 > 5: ${expensive1}"
userAge="25"
isAdult=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${userAge} "18"
)
echo "Age 25 > 18: ${isAdult}"
stringFloat1="3.14"
stringFloat2="3.0"
piGreater=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${stringFloat1} ${stringFloat2}
)
echo "String 3.14 > 3.0: ${piGreater}"
neg1=$(( - 10))
neg2=$(( - 20))
negComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${neg1} ${neg2}
)
echo "Negative -10 > -20: ${negComparison}"
neg3=$(( - 20))
neg4=$(( - 10))
negComparison2=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${neg3} ${neg4}
)
echo "Negative -20 > -10: ${negComparison2}"
mixedInt=6
mixedFloat=5.9
mixedComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${mixedInt} ${mixedFloat}
)
echo "Mixed 6 > 5.9: ${mixedComparison}"
equal1=100
equal2=100
equalComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${equal1} ${equal2}
)
echo "Equal 100 > 100: ${equalComparison}"
invalidValue="abc"
numericThreshold=5
invalidComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${invalidValue} ${numericThreshold}
)
echo "Invalid 'abc' > 5: ${invalidComparison}"
validValue=10
invalidThreshold="xyz"
invalidThresholdComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${validValue} ${invalidThreshold}
)
echo "Invalid 10 > 'xyz': ${invalidThresholdComparison}"
emptyString1=""
emptyString2=""
emptyComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${emptyString1} ${emptyString2}
)
echo "Empty strings comparison: ${emptyComparison}"
zero1=0
zero2=$(( - 1))
zeroComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${zero1} ${zero2}
)
echo "Zero 0 > -1: ${zeroComparison}"
zero3=$(( - 1))
zero4=0
zeroComparison2=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${zero3} ${zero4}
)
echo "Zero -1 > 0: ${zeroComparison2}"
if [ $(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${userAge} "21"
) ]; then
  echo "User can drink alcohol"
else
  echo "User cannot drink alcohol"
fi
minScore=80
actualScore=92
if [ $(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${actualScore} ${minScore}
) ]; then
  echo "Student passed with high score"
else
  echo "Student did not meet minimum score"
fi
threshold=50
result1=75
result2=25
bothPassed=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${result1} ${threshold}
) && $(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${result2} ${threshold}
)
echo "Both results passed threshold: ${bothPassed}"
largeNumber1=999999
largeNumber2=999998
largeComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${largeNumber1} ${largeNumber2}
)
echo "Large numbers 999999 > 999998: ${largeComparison}"
precise1=1.0000001
precise2=1.0000000
preciseComparison=$(
_utah_validate_greater_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value > $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value > $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_greater_than ${precise1} ${precise2}
)
echo "Precision 1.0000001 > 1.0000000: ${preciseComparison}"
