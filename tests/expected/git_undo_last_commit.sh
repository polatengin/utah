#!/bin/bash

mkdir -p /tmp/utah-git-test
cd /tmp/utah-git-test
git init
echo "test" > file.txt
git add file.txt
git commit -m "Initial commit"
echo "change" > file.txt
git add file.txt
git commit -m "Second commit"
echo "Before undo:"
git log --oneline
git reset --soft HEAD~1
echo "After undo:"
git log --oneline
