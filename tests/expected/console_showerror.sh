#!/bin/bash

if command -v dialog >/dev/null 2>&1; then dialog --title "Error" --msgbox "This is an error message" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Error" --msgbox "This is an error message" 10 60 2>&1 >/dev/tty; else echo "[ERROR] " "Error" ": " "This is an error message" >&2; read -p "Press Enter to continue..."; f
