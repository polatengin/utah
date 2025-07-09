#!/bin/sh

text="Hello World"
length=${#text}
upper="${text^^}"
lower="${text,,}"
echo "Text: ${text}"
echo "Length: ${length}"
echo "Upper: ${upper}"
echo "Lower: ${lower}"
