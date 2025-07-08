#!/bin/sh

echo "Before clear - this should be visible initially"
echo "Another line before clear"
clear
echo "After clear - this should be the first visible line"
echo "This line should also be visible after clear"
shouldClear=true
if [ "${shouldClear}" = "true" ]; then
  echo "Clearing console from within if statement"
  clear
  echo "Console cleared from if statement"
fi
clearScreen() {
  echo "About to clear from function"
  clear
  echo "Console cleared from function"
}
clearScreen
echo "Test completed"
