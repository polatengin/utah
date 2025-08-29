#!/bin/bash

longPid=$(sleep 10 &; echo $!)
echo "Started sleep process with PID:"
echo "${longPid}"
isRunning1=$(ps -p ${longPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Sleep process running:"
echo "${isRunning1}"
quickPid=$(echo 'Quick task done' &; echo $!)
echo "Started quick process with PID:"
echo "${quickPid}"
isRunning2=$(ps -p ${quickPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Quick process running:"
echo "${isRunning2}"
runningCheck1=$(ps -p 1 -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Init process (PID 1) running:"
echo "${runningCheck1}"
runningCheck2=$(ps -p 99999 -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Invalid PID (99999) running:"
echo "${runningCheck2}"
testPid=1
variableCheck=$(ps -p ${testPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Variable PID check:"
echo "${variableCheck}"
if [ $(ps -p 1 -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
  echo "System init process is running (as expected)"
fi
if [ ! $(ps -p 99999 -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
  echo "Invalid PID correctly returns false"
fi
monitorPid=$(sleep 2 &; echo $!)
echo "Started 2-second process for monitoring"
monitoring=true
checkCount=0
while [ ${monitoring} && [ ${checkCount} -lt 5 ] ]; do
  stillRunning=$(ps -p ${monitorPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
  echo "Monitor check:"
  echo "${checkCount}"
  echo "Result:"
  echo "${stillRunning}"
  if [ "${stillRunning}" = "false" ]; then
    monitoring=false
    echo "Process finished, stopping monitor"
  fi
  checkCount=$((checkCount + 1))
done
echo "Process monitoring test completed"
currentProcessId=$(ps -o pid -p $$ --no-headers | tr -d ' ')
currentRunning=$(ps -p ${currentProcessId} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Current process running:"
echo "${currentRunning}"
testCmd=$(echo 'Integration test' &; echo $!)
if [ $(ps -p ${testCmd} -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
  echo "Process integration test: process is running"
else
  echo "Process integration test: process finished quickly"
fi
echo "All process.isRunning() tests completed"
