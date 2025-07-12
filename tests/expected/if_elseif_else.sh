#!/bin/bash

x=50
if [ ${x} -gt 80 ]; then
  echo "High"
elif [ ${x} -gt 60 ]; then
  echo "Medium"
else
  echo "Low"
fi
