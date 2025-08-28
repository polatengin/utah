#!/bin/bash

score1=75
passing1=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${score1} 60 100
)
echo "Score 75 in range [60,100]: ${passing1}"
score2=50
passing2=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${score2} 60 100
)
echo "Score 50 in range [60,100]: ${passing2}"
score3=110
passing3=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${score3} 60 100
)
echo "Score 110 in range [60,100]: ${passing3}"
minBound=60
maxBound=100
atMin=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range 60 ${minBound} ${maxBound}
)
atMax=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range 100 ${minBound} ${maxBound}
)
echo "Value 60 at min bound [60,100]: ${atMin}"
echo "Value 100 at max bound [60,100]: ${atMax}"
temp1=98.6
normalTemp=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${temp1} 97.0 99.0
)
echo "Temperature 98.6 in range [97.0,99.0]: ${normalTemp}"
temp2=96.5
lowTemp=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${temp2} 97.0 99.0
)
echo "Temperature 96.5 in range [97.0,99.0]: ${lowTemp}"
temp3=99.8
highTemp=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${temp3} 97.0 99.0
)
echo "Temperature 99.8 in range [97.0,99.0]: ${highTemp}"
precise1=3.14159
piInRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${precise1} 3.0 4.0
)
echo "Pi 3.14159 in range [3.0,4.0]: ${piInRange}"
precise2=2.99999
almostThree=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${precise2} 3.0 4.0
)
echo "Value 2.99999 in range [3.0,4.0]: ${almostThree}"
age1="25"
workingAge=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${age1} "18" "65"
)
echo "Age 25 in working range [18,65]: ${workingAge}"
age2="16"
underAge=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${age2} "18" "65"
)
echo "Age 16 in working range [18,65]: ${underAge}"
age3="70"
retirement=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${age3} "18" "65"
)
echo "Age 70 in working range [18,65]: ${retirement}"
mixedValue=5
mixedRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${mixedValue} 4.5 5.5
)
echo "Integer 5 in float range [4.5,5.5]: ${mixedRange}"
floatValue=4.75
intRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${floatValue} 4 6
)
echo "Float 4.75 in integer range [4,6]: ${intRange}"
negValue1=-5
negRange1=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${negValue1} -10 0
)
echo "Negative -5 in range [-10,0]: ${negRange1}"
negValue2=-15
negRange2=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${negValue2} -10 0
)
echo "Negative -15 in range [-10,0]: ${negRange2}"
negValue3=5
negRange3=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${negValue3} -10 0
)
echo "Positive 5 in negative range [-10,0]: ${negRange3}"
crossValue1=0
crossRange1=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${crossValue1} -5 5
)
echo "Zero in range [-5,5]: ${crossRange1}"
crossValue2=-3
crossRange2=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${crossValue2} -5 5
)
echo "Value -3 in range [-5,5]: ${crossRange2}"
crossValue3=3
crossRange3=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${crossValue3} -5 5
)
echo "Value 3 in range [-5,5]: ${crossRange3}"
singleValue1=50
singlePoint=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${singleValue1} 50 50
)
echo "Value 50 in single point range [50,50]: ${singlePoint}"
singleValue2=49
outsideSingle=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${singleValue2} 50 50
)
echo "Value 49 in single point range [50,50]: ${outsideSingle}"
invalidRange1=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range 75 100 50
)
echo "Value 75 in invalid range [100,50]: ${invalidRange1}"
invalidRange2=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range 75 80 70
)
echo "Value 75 in invalid range [80,70]: ${invalidRange2}"
largeValue=999999
largeRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${largeValue} 999998 1000000
)
echo "Large number 999999 in range [999998,1000000]: ${largeRange}"
invalidValue="abc"
nonNumeric1=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${invalidValue} 1 10
)
echo "Invalid value 'abc' in range [1,10]: ${nonNumeric1}"
validValue=5
invalidMin="xyz"
nonNumeric2=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${validValue} ${invalidMin} 10
)
echo "Valid value 5 with invalid min 'xyz': ${nonNumeric2}"
validValue2=5
invalidMax="abc"
nonNumeric3=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${validValue2} 1 ${invalidMax}
)
echo "Valid value 5 with invalid max 'abc': ${nonNumeric3}"
emptyValue=""
emptyRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${emptyValue} "" ""
)
echo "Empty strings in range: ${emptyRange}"
studentAge=20
if [ $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${studentAge} 18 25
) ]; then
  echo "Student eligible for college discount"
else
  echo "Student not eligible for college discount"
fi
cpuTemp=72
if [ $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${cpuTemp} 0 85
) ]; then
  echo "CPU temperature is safe"
else
  echo "CPU temperature is dangerous"
fi
examScore=88
if [ $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${examScore} 90 100
) ]; then
  echo "Grade: A"
elif [ $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${examScore} 80 89
) ]; then
  echo "Grade: B"
elif [ $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${examScore} 70 79
) ]; then
  echo "Grade: C"
else
  echo "Grade: Below C"
fi
completion=75
validPercent=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${completion} 0 100
)
echo "Completion percentage is valid: ${validPercent}"
latitude=40.7128
longitude=-74.0060
validLat=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${latitude} -90 90
)
validLon=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${longitude} -180 180
)
echo "Latitude is valid: ${validLat}"
echo "Longitude is valid: ${validLon}"
productPrice=49.99
affordableRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${productPrice} 25.0 75.0
)
echo "Product price in affordable range: ${affordableRange}"
currentHour=14
businessHours=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${currentHour} 9 17
)
echo "Current time is during business hours: ${businessHours}"
memoryUsage=78.5
memoryOk=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${memoryUsage} 0 90
)
echo "Memory usage is within limits: ${memoryOk}"
threshold1=50
threshold2=90
value1=60
value2=80
bothInRange=$(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${value1} ${threshold1} ${threshold2}
) && $(
_utah_validate_in_range() {
  local value="$1"
  local min="$2"
  local max="$3"

  # Check if all values are numeric (integer or float)
  if ! [[ "$value" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$min" =~ ^-?[0-9]+(\.[0-9]+)?$ ]] || ! [[ "$max" =~ ^-?[0-9]+(\.[0-9]+)?$ ]]; then
    echo "false" && return
  fi

  # Use bc for floating-point comparison, awk as fallback
  if command -v bc >/dev/null 2>&1; then
    # Check valid range (min <= max)
    valid_range=$(echo "$min <= $max" | bc)
    [ "$valid_range" != "1" ] && echo "false" && return

    # Check if value is in range (inclusive: min <= value <= max)
    result=$(echo "$min <= $value && $value <= $max" | bc)
    [ "$result" = "1" ] && echo "true" || echo "false"
  else
    # Fallback using awk for float comparison
    result=$(awk "BEGIN { if ($min <= $max && $min <= $value && $value <= $max) print 1; else print 0 }")
    [ "$result" = "1" ] && echo "true" || echo "false"
  fi
}
_utah_validate_in_range ${value2} ${threshold1} ${threshold2}
)
echo "Both values in range [50,90]: ${bothInRange}"
