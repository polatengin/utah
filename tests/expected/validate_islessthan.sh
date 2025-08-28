#!/bin/bash

score1=65
passing1=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${score1} 70
)
echo "Score 65 < 70: ${passing1}"
score2=85
passing2=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${score2} 70
)
echo "Score 85 < 70: ${passing2}"
score3=70
passing3=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${score3} 70
)
echo "Score 70 < 70: ${passing3}"
temp1=98.5
fever1=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${temp1} 98.6
)
echo "Temperature 98.5 < 98.6: ${fever1}"
temp2=98.7
fever2=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${temp2} 98.6
)
echo "Temperature 98.7 < 98.6: ${fever2}"
price1=4.9
affordable1=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${price1} 5
)
echo "Price 4.9 < 5: ${affordable1}"
userAge="17"
isMinor=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${userAge} "18"
)
echo "Age 17 < 18: ${isMinor}"
stringFloat1="2.8"
stringFloat2="3.0"
piLess=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${stringFloat1} ${stringFloat2}
)
echo "String 2.8 < 3.0: ${piLess}"
neg1=$(( - 20))
neg2=$(( - 10))
negComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${neg1} ${neg2}
)
echo "Negative -20 < -10: ${negComparison}"
neg3=$(( - 10))
neg4=$(( - 20))
negComparison2=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${neg3} ${neg4}
)
echo "Negative -10 < -20: ${negComparison2}"
mixedInt=5
mixedFloat=5.1
mixedComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${mixedInt} ${mixedFloat}
)
echo "Mixed 5 < 5.1: ${mixedComparison}"
equal1=100
equal2=100
equalComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${equal1} ${equal2}
)
echo "Equal 100 < 100: ${equalComparison}"
invalidValue="abc"
numericThreshold=5
invalidComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${invalidValue} ${numericThreshold}
)
echo "Invalid 'abc' < 5: ${invalidComparison}"
validValue=10
invalidThreshold="xyz"
invalidThresholdComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${validValue} ${invalidThreshold}
)
echo "Invalid 10 < 'xyz': ${invalidThresholdComparison}"
emptyString1=""
emptyString2=""
emptyComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${emptyString1} ${emptyString2}
)
echo "Empty strings comparison: ${emptyComparison}"
zero1=$(( - 1))
zero2=0
zeroComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${zero1} ${zero2}
)
echo "Zero -1 < 0: ${zeroComparison}"
zero3=0
zero4=$(( - 1))
zeroComparison2=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${zero3} ${zero4}
)
echo "Zero 0 < -1: ${zeroComparison2}"
if [ $(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${userAge} "18"
) ]; then
  echo "User is a minor"
else
  echo "User is an adult"
fi
maxScore=80
actualScore=65
if [ $(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${actualScore} ${maxScore}
) ]; then
  echo "Student scored below maximum"
else
  echo "Student achieved maximum or higher score"
fi
threshold=50
result1=25
result2=75
bothBelow=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${result1} ${threshold}
) && $(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${result2} ${threshold}
)
echo "Both results below threshold: ${bothBelow}"
largeNumber1=999998
largeNumber2=999999
largeComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${largeNumber1} ${largeNumber2}
)
echo "Large numbers 999998 < 999999: ${largeComparison}"
precise1=1.0000000
precise2=1.0000001
preciseComparison=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${precise1} ${precise2}
)
echo "Precision 1.0000000 < 1.0000001: ${preciseComparison}"
studentAge=16
minimumAge=18
if [ $(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${studentAge} ${minimumAge}
) ]; then
  echo "Student needs parental consent"
fi
responseTime=250
performanceLimit=500
isPerformant=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${responseTime} ${performanceLimit}
)
echo "Response time is within limits: ${isPerformant}"
currentTemp=75
maxTemp=85
if [ $(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${currentTemp} ${maxTemp}
) ]; then
  echo "Temperature is safe"
else
  echo "Temperature warning!"
fi
memoryUsage=78
memoryLimit=90
memoryOk=$(
_utah_validate_less_than() {
  local value="$1"
  local threshold="$2"

  # Check if both values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$threshold" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    result=$(echo "$value < $threshold" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { print ($value < $threshold) ? 1 : 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_less_than ${memoryUsage} ${memoryLimit}
)
echo "Memory usage is acceptable: ${memoryOk}"
