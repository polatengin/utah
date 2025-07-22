#!/bin/bash

confirmed=$(if command -v dialog >/dev/null 2>&1; then if dialog --title "Confirm Action" --yesno "Are you sure you want to continue?" 10 60 2>&1 >/dev/tty; then echo "true"; else echo "false"; fi; elif command -v whiptail >/dev/null 2>&1; then if whiptail --title "Confirm Action" --yesno "Are you sure you want to continue?" 10 60 2>&1 >/dev/tty; then echo "true"; else echo "false"; fi; else while true; do if [[ "yes" == "yes" ]]; then read -p "Are you sure you want to continue?" " (Y/n): " _utah_confirm; else read -p "Are you sure you want to continue?" " (y/N): " _utah_confirm; fi; case $_utah_confirm in [Yy]*|'') echo "true"; break;; [Nn]*) echo "false"; break;; *) echo "Please answer yes or no.";; esac; done; fi)
echo "Confirmed: ${confirmed}"
