#!/bin/bash

if command -v dialog >/dev/null 2>&1; then dialog --title "Warning" --msgbox "This is a warning message" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Warning" --msgbox "This is a warning message" 10 60 2>&1 >/dev/tty; else echo "[WARNING] " "Warning" ": " "This is a warning message"; read -p "Press Enter to continue..."; f
