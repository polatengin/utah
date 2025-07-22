#!/bin/bash

name=$(if command -v dialog >/dev/null 2>&1; then dialog --title "Input" --inputbox "Enter your name" 10 60 "John Doe" 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Input" --inputbox "Enter your name" 10 60 "John Doe" 2>&1 >/dev/tty; else if [[ -n "John Doe" ]]; then read -p "Enter your name" " [" "John Doe" "]: " _utah_input; echo ${_utah_input:-"John Doe"}; else read -p "Enter your name" ": " _utah_input; echo $_utah_input; fi; fi)
echo "Hello, ${name}!"
