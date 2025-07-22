#!/bin/bash

{ if command -v dialog >/dev/null 2>&1; then dialog --title "Test Title" --msgbox "Test Message" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Test Title" --msgbox "Test Message" 10 60 2>&1 >/dev/tty; else echo "Test Title": "Test Message"; read -p "Press Enter..."; fi; } 2>/dev/nul
