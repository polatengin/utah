#!/bin/bash

pid1=$(echo 'Hello from background process' &; echo $!)
echo "Started basic process with PID: ${pid1}"
pid2=$((cd "/tmp" && pwd &); echo $!)
echo "Started process in /tmp with PID: ${pid2}"
pid3=$(cat < "/etc/hostname" &; echo $!)
echo "Started cat with input redirection, PID: ${pid3}"
pid4=$(date > "/tmp/date_output.txt" &; echo $!)
echo "Started date with output redirection, PID: ${pid4}"
pid5=$(ls /nonexistent 2> "/tmp/error_output.txt" &; echo $!)
echo "Started command with error redirection, PID: ${pid5}"
pid6=$((cd "/var/log" && ls -la > "/tmp/log_listing.txt" &); echo $!)
echo "Started ls in /var/log with output redirection, PID: ${pid6}"
pid7=$((cd "/tmp" && sort < "/etc/passwd" > "/tmp/sorted_passwd.txt" 2> "/tmp/sort_errors.txt" &); echo $!)
echo "Started sort with all options, PID: ${pid7}"
workDir="/home"
outputFile="/tmp/home_files.txt"
pid8=$((cd ${workDir} && find . -name '*.txt' > ${outputFile} &); echo $!)
echo "$((Started find command in ${workDir}, PID:  + pid8))"
echo "All processes started, PIDs should be valid numbers"
