#!/bin/bash

if command -v dialog >/dev/null 2>&1; then echo 75 | dialog --title "Loading" --gauge "Please wait..." 10 60 0 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then echo 75 | whiptail --title "Loading" --gauge "Please wait..." 10 60 0 2>&1 >/dev/tty; else echo "[" 75 "%] " "Loading" ": " "Please wait..."; f
