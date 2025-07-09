#!/bin/sh

fileExt="pdf"
echo "File extension:"
echo "${fileExt}"
fileName="document.pdf"
pdfExt="${fileName##*.}"
echo "${pdfExt}"
