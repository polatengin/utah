#!/bin/bash

dir=$(dirname "/home/user/documents/file.txt")
echo "Directory:"
echo "${dir}"
parentDir=$(basename $(dirname "/home/user/documents/file.txt"))
echo "Parent directory name:"
echo "${parentDir}"
ext="txt"
echo "File extension:"
echo "${ext}"
fileName=$(basename "/home/user/documents/file.txt")
echo "File name:"
echo "${fileName}"
