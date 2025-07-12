#!/bin/bash

grade="B"
case ${grade} in
  A)
    echo "Excellent"
    ;;
  B)
    echo "Good"
    ;;
  *)
    echo "Try harder"
    ;;
esac
