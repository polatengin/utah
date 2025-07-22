#!/bin/bash

directory=$(if command -v dialog >/dev/null 2>&1; then dialog --title "Directory Selection" --dselect "/tmp" 15 80 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Directory Selection" --inputbox "Select a directory" " [" "/tmp" "]:" 10 60 "/tmp" 2>&1 >/dev/tty; else if [[ -n "/tmp" ]]; then read -p "Select a directory" " [" "/tmp" "]: " _utah_dir; echo ${_utah_dir:-"/tmp"}; else read -p "Select a directory" ": " _utah_dir; echo $_utah_dir; fi; fi)
echo "Selected directory: ${directory}"
