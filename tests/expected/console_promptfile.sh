#!/bin/bash

file=$(if command -v dialog >/dev/null 2>&1; then dialog --title "File Selection" --fselect "$(pwd)/" 15 80 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "File Selection" --inputbox "Select a file" " (current dir: $(pwd))" 10 60 2>&1 >/dev/tty; else read -p "Select a file" ": " _utah_file; echo $_utah_file; fi)
echo "Selected file: ${file}"
