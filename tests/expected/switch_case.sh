#!/bin/sh

score=85
category=""
case $score in
  100)
    category="Perfect"
    ;;
  90)
    category="Excellent"
    ;;
  80)
    category="Good"
    ;;
  *)
    category="Needs improvement"
    ;;
esac
echo $category
day="Monday"
type=""
case $day in
  Monday|Tuesday|Wednesday|Thursday|Friday)
    type="Weekday"
    ;;
  Saturday|Sunday)
    type="Weekend"
    ;;
  *)
    type="Unknown"
    ;;
esac
echo $type
