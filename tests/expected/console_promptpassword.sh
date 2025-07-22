#!/bin/bash

password=$(if command -v dialog >/dev/null 2>&1; then dialog --title "Password" --passwordbox "Enter your password" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Password" --passwordbox "Enter your password" 10 60 2>&1 >/dev/tty; else read -s -p "Enter your password" ": " _utah_password; echo $_utah_password; fi)
echo "Password length: ${#password}"
