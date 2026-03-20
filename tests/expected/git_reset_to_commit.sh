#!/bin/bash

git reset --hard "abc123"
commitHash="def456"
git reset --hard ${commitHash}
