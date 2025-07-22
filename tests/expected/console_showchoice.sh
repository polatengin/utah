#!/bin/bash

choice=$(if command -v dialog >/dev/null 2>&1; then _utah_choice_result=$(echo "Apple,Orange,Banana" | tr ',' '\n' | nl -n ln | dialog --title "Choose Option" --default-item 0 --menu "Pick your favorite:" 15 60 10 --file - 2>&1 >/dev/tty); echo "Apple,Orange,Banana" | cut -d',' -f$_utah_choice_result; elif command -v whiptail >/dev/null 2>&1; then _utah_choice_result=$(echo "Apple,Orange,Banana" | tr ',' '\n' | nl -n ln | whiptail --title "Choose Option" --default-item 0 --menu "Pick your favorite:" 15 60 10 --file - 2>&1 >/dev/tty); echo "Apple,Orange,Banana" | cut -d',' -f$_utah_choice_result; else echo "Choose from: " "Apple,Orange,Banana"; read -p "Enter your choice: " _utah_fallback_choice; echo $_utah_fallback_choice; fi)
echo "You chose: ${choice}"
