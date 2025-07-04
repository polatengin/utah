#!/bin/sh

echo "Process Information Test"
pid=$(ps -o pid -p $$ --no-headers | tr -d ' ')
echo "Process ID: ${pid}"
cpuUsage=$(ps -o pcpu -p $$ --no-headers | tr -d ' ')
echo "CPU Usage: ${cpuUsage}%"
memUsage=$(ps -o pmem -p $$ --no-headers | tr -d ' ')
echo "Memory Usage: ${memUsage}%"
elapsedTime=$(ps -o etime -p $$ --no-headers | tr -d ' ')
echo "Elapsed Time: ${elapsedTime}"
command=$(ps -o cmd= -p $$)
echo "Command: ${command}"
status=$(ps -o stat= -p $$)
echo "Status: ${status}"
echo "Process info test completed"
