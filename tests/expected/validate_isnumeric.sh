#!/bin/bash

positiveInt=42
isValidPosInt=$(echo "${positiveInt}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Positive integer validation:"
echo "${isValidPosInt}"
negativeInt=-42
isValidNegInt=$(echo "${negativeInt}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Negative integer validation:"
echo "${isValidNegInt}"
zero=0
isValidZero=$(echo "${zero}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Zero validation:"
echo "${isValidZero}"
positiveFloat=123.456
isValidPosFloat=$(echo "${positiveFloat}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Positive float validation:"
echo "${isValidPosFloat}"
negativeFloat=-123.456
isValidNegFloat=$(echo "${negativeFloat}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Negative float validation:"
echo "${isValidNegFloat}"
zeroFloat=0.0
isValidZeroFloat=$(echo "${zeroFloat}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Zero float validation:"
echo "${isValidZeroFloat}"
decimalOnly=0.5
isValidDecimal=$(echo "${decimalOnly}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Decimal validation:"
echo "${isValidDecimal}"
negativeDecimal=-0.5
isValidNegDecimal=$(echo "${negativeDecimal}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Negative decimal validation:"
echo "${isValidNegDecimal}"
stringInt="789"
isValidStringInt=$(echo "${stringInt}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "String integer validation:"
echo "${isValidStringInt}"
stringFloat="123.789"
isValidStringFloat=$(echo "${stringFloat}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "String float validation:"
echo "${isValidStringFloat}"
stringNegative="-456.789"
isValidStringNeg=$(echo "${stringNegative}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "String negative validation:"
echo "${isValidStringNeg}"
largeNumber="999999999999999"
isValidLarge=$(echo "${largeNumber}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Large number validation:"
echo "${isValidLarge}"
smallDecimal="0.000001"
isValidSmall=$(echo "${smallDecimal}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Small decimal validation:"
echo "${isValidSmall}"
singleDigit="5"
isValidSingle=$(echo "${singleDigit}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Single digit validation:"
echo "${isValidSingle}"
emptyString=""
isValidEmpty=$(echo "${emptyString}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Empty string validation:"
echo "${isValidEmpty}"
alphabetic="abc"
isValidAlpha=$(echo "${alphabetic}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Alphabetic validation:"
echo "${isValidAlpha}"
alphanumeric="123abc"
isValidAlphaNum=$(echo "${alphanumeric}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Alphanumeric validation:"
echo "${isValidAlphaNum}"
withSpaces=" 123 "
isValidSpaces=$(echo "${withSpaces}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "With spaces validation:"
echo "${isValidSpaces}"
multipleDecimals="123.45.67"
isValidMultiDec=$(echo "${multipleDecimals}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Multiple decimals validation:"
echo "${isValidMultiDec}"
multipleNegatives="--123"
isValidMultiNeg=$(echo "${multipleNegatives}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Multiple negatives validation:"
echo "${isValidMultiNeg}"
trailingDecimal="123."
isValidTrailingDec=$(echo "${trailingDecimal}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Trailing decimal validation:"
echo "${isValidTrailingDec}"
leadingDecimal=".123"
isValidLeadingDec=$(echo "${leadingDecimal}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Leading decimal validation:"
echo "${isValidLeadingDec}"
scientificNotation="1.23e10"
isValidScientific=$(echo "${scientificNotation}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Scientific notation validation:"
echo "${isValidScientific}"
hexadecimal="0x1A"
isValidHex=$(echo "${hexadecimal}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Hexadecimal validation:"
echo "${isValidHex}"
plusSign="+123"
isValidPlus=$(echo "${plusSign}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Plus sign validation:"
echo "${isValidPlus}"
userInput="42.5"
if [ $(echo "${userInput}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false") ]; then
  echo "Input is valid number"
else
  echo "Input is not valid number"
fi
testValue="invalid123"
if [ ! $(echo "${testValue}" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false") ]; then
  echo "Value failed numeric validation"
fi
result1=$(echo ""100"" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
result2=$(echo ""-50.25"" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
result3=$(echo ""not_a_number"" | grep -qE '^-?[0-9]+(\.[0-9]+)?$' && echo "true" || echo "false")
echo "Direct assignment results:"
echo "${result1}"
echo "${result2}"
echo "${result3}"
echo "validate.isNumeric() testing complete"
