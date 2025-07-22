#!/bin/bash

age=$(while true; do if [[ -n 25 ]]; then read -p "Enter your age" " [" 25 "]: " _utah_number; _utah_number=${_utah_number:-25}; else read -p "Enter your age" ": " _utah_number; fi; if [[ $_utah_number =~ ^-?[0-9]+$ ]]; then _utah_valid=true; if [[ -n 18 && $_utah_number -lt 18 ]]; then echo "Value must be >= " 18; _utah_valid=false; fi; if [[ -n 120 && $_utah_number -gt 120 ]]; then echo "Value must be <= " 120; _utah_valid=false; fi; if [[ $_utah_valid == true ]]; then echo $_utah_number; break; fi; else echo "Please enter a valid number."; fi; done)
echo "You are ${age} years old"
