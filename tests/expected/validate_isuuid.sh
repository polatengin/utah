#!/bin/bash

uuid1="550e8400-e29b-41d4-a716-446655440000"
isValid1=$(echo ${uuid1} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 1 UUID: ${uuid1} is ${isValid1}"
uuid4v1="6ba7b810-9dad-11d1-80b4-00c04fd430c8"
isValid4v1=$(echo ${uuid4v1} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 4 UUID: ${uuid4v1} is ${isValid4v1}"
uuid4v2="6ba7b814-9dad-41d1-80b4-00c04fd430c8"
isValid4v2=$(echo ${uuid4v2} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Another V4 UUID: ${uuid4v2} is ${isValid4v2}"
uuid5="6ba7b815-9dad-51d1-80b4-00c04fd430c8"
isValid5=$(echo ${uuid5} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 5 UUID: ${uuid5} is ${isValid5}"
uuidUpper="550E8400-E29B-41D4-A716-446655440000"
isValidUpper=$(echo ${uuidUpper} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Uppercase UUID: ${uuidUpper} is ${isValidUpper}"
uuidMixed="550e8400-E29B-41d4-A716-446655440000"
isValidMixed=$(echo ${uuidMixed} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Mixed case UUID: ${uuidMixed} is ${isValidMixed}"
uuidV2="550e8400-e29b-21d4-a716-446655440000"
isValidV2=$(echo ${uuidV2} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 2 UUID: ${uuidV2} is ${isValidV2}"
uuidV3="550e8400-e29b-31d4-a716-446655440000"
isValidV3=$(echo ${uuidV3} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 3 UUID: ${uuidV3} is ${isValidV3}"
uuidVariant8="550e8400-e29b-41d4-8716-446655440000"
isValidVariant8=$(echo ${uuidVariant8} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant 8 UUID: ${uuidVariant8} is ${isValidVariant8}"
uuidVariant9="550e8400-e29b-41d4-9716-446655440000"
isValidVariant9=$(echo ${uuidVariant9} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant 9 UUID: ${uuidVariant9} is ${isValidVariant9}"
uuidVariantA="550e8400-e29b-41d4-a716-446655440000"
isValidVariantA=$(echo ${uuidVariantA} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant A UUID: ${uuidVariantA} is ${isValidVariantA}"
uuidVariantB="550e8400-e29b-41d4-b716-446655440000"
isValidVariantB=$(echo ${uuidVariantB} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant B UUID: ${uuidVariantB} is ${isValidVariantB}"
invalidShort="550e8400-e29b-41d4-a716"
isInvalidShort=$(echo ${invalidShort} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Too short: ${invalidShort} is ${isInvalidShort}"
invalidLong="550e8400-e29b-41d4-a716-446655440000-extra"
isInvalidLong=$(echo ${invalidLong} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Too long: ${invalidLong} is ${isInvalidLong}"
invalidChar="550e8400-e29b-41d4-a716-44665544000g"
isInvalidChar=$(echo ${invalidChar} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Invalid char: ${invalidChar} is ${isInvalidChar}"
invalidSpecial="550e8400-e29b-41d4-a716-446655440#00"
isInvalidSpecial=$(echo ${invalidSpecial} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Special char: ${invalidSpecial} is ${isInvalidSpecial}"
invalidVersion0="550e8400-e29b-01d4-a716-446655440000"
isInvalidVersion0=$(echo ${invalidVersion0} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 0: ${invalidVersion0} is ${isInvalidVersion0}"
invalidVersion6="550e8400-e29b-61d4-a716-446655440000"
isInvalidVersion6=$(echo ${invalidVersion6} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 6: ${invalidVersion6} is ${isInvalidVersion6}"
invalidVersion7="550e8400-e29b-71d4-a716-446655440000"
isInvalidVersion7=$(echo ${invalidVersion7} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Version 7: ${invalidVersion7} is ${isInvalidVersion7}"
invalidVariant0="550e8400-e29b-41d4-0716-446655440000"
isInvalidVariant0=$(echo ${invalidVariant0} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant 0: ${invalidVariant0} is ${isInvalidVariant0}"
invalidVariant2="550e8400-e29b-41d4-2716-446655440000"
isInvalidVariant2=$(echo ${invalidVariant2} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant 2: ${invalidVariant2} is ${isInvalidVariant2}"
invalidVariantC="550e8400-e29b-41d4-c716-446655440000"
isInvalidVariantC=$(echo ${invalidVariantC} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Variant C: ${invalidVariantC} is ${isInvalidVariantC}"
noHyphens="550e8400e29b41d4a716446655440000"
isNoHyphens=$(echo ${noHyphens} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "No hyphens: ${noHyphens} is ${isNoHyphens}"
wrongHyphens1="550e840-0e29b-41d4-a716-446655440000"
isWrongHyphens1=$(echo ${wrongHyphens1} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Wrong hyphens 1: ${wrongHyphens1} is ${isWrongHyphens1}"
wrongHyphens2="550e8400-e29b4-1d4-a716-446655440000"
isWrongHyphens2=$(echo ${wrongHyphens2} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Wrong hyphens 2: ${wrongHyphens2} is ${isWrongHyphens2}"
emptyUuid=""
isEmptyUuid=$(echo ${emptyUuid} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Empty string: '${emptyUuid}' is ${isEmptyUuid}"
notUuidAtAll="not-a-uuid-at-all"
isNotUuidAtAll=$(echo ${notUuidAtAll} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Not UUID: ${notUuidAtAll} is ${isNotUuidAtAll}"
if [ $(echo "550e8400-e29b-41d4-a716-446655440000" | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false") ]; then
  echo "Valid UUID in if statement"
else
  echo "Invalid UUID in if statement"
fi
generatedUUID=$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c "import uuid; print(uuid.uuid4())"; else echo "$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))"; fi)
isGeneratedValid=$(echo ${generatedUUID} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Generated UUID: ${generatedUUID} is ${isGeneratedValid}"
testUUID="6ba7b810-9dad-41d1-80b4-00c04fd430c8"
validationResult=$(echo ${testUUID} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
if [ "${validationResult}" = "true" ]; then
  echo "UUID validation passed"
else
  echo "UUID validation failed"
fi
zeroUUID="00000000-0000-4000-8000-000000000000"
isZeroValid=$(echo ${zeroUUID} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Zero UUID: ${zeroUUID} is ${isZeroValid}"
maxUUID="ffffffff-ffff-4fff-bfff-ffffffffffff"
isMaxValid=$(echo ${maxUUID} | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[1-5][0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$' && echo "true" || echo "false")
echo "Max UUID: ${maxUUID} is ${isMaxValid}"
