#!/bin/bash

if command -v dialog >/dev/null 2>&1; then dialog --title "Success" --msgbox "This is a success message" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Success" --msgbox "This is a success message" 10 60 2>&1 >/dev/tty; else echo "[SUCCESS] " "Success" ": " "This is a success message"; read -p "Press Enter to continue..."; f
