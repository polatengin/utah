#!/bin/bash

validAlphaNum1="abc123"
isValid1=$(echo "${validAlphaNum1}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Mixed letters and numbers:"
echo "${isValid1}"
validAlphaNum2="ABC"
isValid2=$(echo "${validAlphaNum2}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Uppercase letters only:"
echo "${isValid2}"
validAlphaNum3="xyz"
isValid3=$(echo "${validAlphaNum3}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Lowercase letters only:"
echo "${isValid3}"
validAlphaNum4="123"
isValid4=$(echo "${validAlphaNum4}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Numbers only:"
echo "${isValid4}"
validAlphaNum5="a1B2c3"
isValid5=$(echo "${validAlphaNum5}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Mixed case and numbers:"
echo "${isValid5}"
validAlphaNum6="username123"
isValid6=$(echo "${validAlphaNum6}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Typical username format:"
echo "${isValid6}"
validAlphaNum7="A"
isValid7=$(echo "${validAlphaNum7}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Single letter:"
echo "${isValid7}"
validAlphaNum8="9"
isValid8=$(echo "${validAlphaNum8}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Single digit:"
echo "${isValid8}"
invalidEmpty=""
isEmpty=$(echo "${invalidEmpty}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Empty string:"
echo "${isEmpty}"
invalidSpace="abc 123"
hasSpace=$(echo "${invalidSpace}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains space:"
echo "${hasSpace}"
invalidHyphen="user-name"
hasHyphen=$(echo "${invalidHyphen}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains hyphen:"
echo "${hasHyphen}"
invalidUnderscore="user_name"
hasUnderscore=$(echo "${invalidUnderscore}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains underscore:"
echo "${hasUnderscore}"
invalidSpecial="user@name"
hasSpecial=$(echo "${invalidSpecial}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains special character:"
echo "${hasSpecial}"
invalidPeriod="user.name"
hasPeriod=$(echo "${invalidPeriod}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains period:"
echo "${hasPeriod}"
invalidExclamation="123!"
hasExclamation=$(echo "${invalidExclamation}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Contains exclamation:"
echo "${hasExclamation}"
invalidLeadingSpace=" abc123"
hasLeadingSpace=$(echo "${invalidLeadingSpace}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Leading space:"
echo "${hasLeadingSpace}"
invalidTrailingSpace="abc123 "
hasTrailingSpace=$(echo "${invalidTrailingSpace}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Trailing space:"
echo "${hasTrailingSpace}"
invalidSpaces=" abc123 "
hasSpaces=$(echo "${invalidSpaces}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Leading and trailing spaces:"
echo "${hasSpaces}"
validLargeNumber="999999999999999"
isValidLarge=$(echo "${validLargeNumber}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Large number string:"
echo "${isValidLarge}"
validMixedLarge="ABC999DEF888"
isValidMixedLarge=$(echo "${validMixedLarge}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Large mixed alphanumeric:"
echo "${isValidMixedLarge}"
if [ $(echo ""productCode123"" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false") ]; then
  echo "Product code format is valid"
fi
if [ ! $(echo ""invalid-code"" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false") ]; then
  echo "Invalid product code format detected"
fi
usernameValid=$(echo ""user123"" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
passwordValid=$(echo ""pass@word"" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Username validation result:"
echo "${usernameValid}"
echo "Password validation result:"
echo "${passwordValid}"
testInput1="validInput123"
testInput2="invalid input"
result1=$(echo "${testInput1}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
result2=$(echo "${testInput2}" | grep -qE '^[A-Za-z0-9]+$' && echo "true" || echo "false")
echo "Variable test 1:"
echo "${result1}"
echo "Variable test 2:"
echo "${result2}"
echo "All alphanumeric validation tests completed"
