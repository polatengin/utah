#!/bin/bash

if command -v dialog >/dev/null 2>&1; then dialog --title "Information" --infobox "This is an info message" 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title "Information" --infobox "This is an info message" 10 60 2>&1 >/dev/tty; else echo "[INFO] " "Information" ": " "This is an info message"; f
