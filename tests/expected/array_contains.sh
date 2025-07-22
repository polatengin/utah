#!/bin/bash

fruits=("apple" "banana" "cherry")
numbers=(1 2 3 4 5)
flags=(true false true)
hasApple=$(case " ${fruits[@]} " in *" apple "*) echo "true" ;; *) echo "false" ;; esac)
hasBanana=$(case " ${fruits[@]} " in *" banana "*) echo "true" ;; *) echo "false" ;; esac)
hasGrape=$(case " ${fruits[@]} " in *" grape "*) echo "true" ;; *) echo "false" ;; esac)
echo "Has apple: ${hasApple}"
echo "Has banana: ${hasBanana}"
echo "Has grape: ${hasGrape}"
hasThree=$(case " ${numbers[@]} " in *" 3 "*) echo "true" ;; *) echo "false" ;; esac)
hasTen=$(case " ${numbers[@]} " in *" 10 "*) echo "true" ;; *) echo "false" ;; esac)
echo "Has 3: ${hasThree}"
echo "Has 10: ${hasTen}"
hasTrue=$(case " ${flags[@]} " in *" true "*) echo "true" ;; *) echo "false" ;; esac)
hasFalse=$(case " ${flags[@]} " in *" false "*) echo "true" ;; *) echo "false" ;; esac)
echo "Has true: ${hasTrue}"
echo "Has false: ${hasFalse}"
if [ $(case " ${fruits[@]} " in *" apple "*) echo "true" ;; *) echo "false" ;; esac) ]; then
  echo "Found apple in fruits array"
fi
if [ ! $(case " ${numbers[@]} " in *" 99 "*) echo "true" ;; *) echo "false" ;; esac) ]; then
  echo "99 is not in the numbers array"
fi
searchFruit="cherry"
searchNumber=2
if [ $(case " ${fruits[@]} " in *" ${searchFruit} "*) echo "true" ;; *) echo "false" ;; esac) ]; then
  echo "Found ${searchFruit} in fruits"
fi
if [ $(case " ${numbers[@]} " in *" ${searchNumber} "*) echo "true" ;; *) echo "false" ;; esac) ]; then
  echo "Found ${searchNumber} in numbers"
fi
