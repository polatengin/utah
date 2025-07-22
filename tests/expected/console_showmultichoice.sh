#!/bin/bash

selections=$(if command -v dialog >/dev/null 2>&1; then echo "Red,Green,Blue" | tr ',' '\n' | nl -n ln | dialog --title "Multi Select" --checklist "Choose multiple options:" 15 60 10 --file - 2>&1 >/dev/tty | tr '\n' ','; elif command -v whiptail >/dev/null 2>&1; then echo "Red,Green,Blue" | tr ',' '\n' | nl -n ln | whiptail --title "Multi Select" --checklist "Choose multiple options:" 15 60 10 --file - 2>&1 >/dev/tty | tr '\n' ','; else echo "Select from: " "Red,Green,Blue"; read -p "Enter your selections (comma-separated): " _utah_multi_choice; echo $_utah_multi_choice; fi)
echo "You selected: ${selections}"
