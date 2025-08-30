#!/bin/bash

echo "Testing basic process.kill() with default SIGTERM"
shortPid=$(sleep 30 &; echo $!)
echo "Started sleep process with PID:"
echo "${shortPid}"
killed1=$(kill -SIGTERM ${shortPid} 2>/dev/null && echo "true" || echo "false")
echo "Process killed with default SIGTERM:"
echo "${killed1}"
stillRunning1=$(ps -p ${shortPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Process still running after kill:"
echo "${stillRunning1}"
echo "Testing process.kill() with specific signal name"
longPid=$(sleep 60 &; echo $!)
echo "Started long sleep process with PID:"
echo "${longPid}"
killed2=$(kill -SIGKILL ${longPid} 2>/dev/null && echo "true" || echo "false")
echo "Process killed with SIGKILL:"
echo "${killed2}"
stillRunning2=$(ps -p ${longPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false")
echo "Process still running after SIGKILL:"
echo "${stillRunning2}"
echo "Testing process.kill() with numeric signal"
numPid=$(sleep 45 &; echo $!)
echo "Started process for numeric signal test with PID:"
echo "${numPid}"
killed3=$(kill -9 ${numPid} 2>/dev/null && echo "true" || echo "false")
echo "Process killed with signal 9:"
echo "${killed3}"
echo "Testing process.kill() with invalid PID"
invalidKill=$(kill -SIGTERM 99999 2>/dev/null && echo "true" || echo "false")
echo "Kill result for invalid PID:"
echo "${invalidKill}"
echo "Testing process.kill() with variable PID"
varPid=$(sleep 20 &; echo $!)
savedPid=${varPid}
killed4=$(kill -SIGTERM ${savedPid} 2>/dev/null && echo "true" || echo "false")
echo "Process killed using variable PID:"
echo "${killed4}"
echo "Testing process.kill() with variable signal"
signalPid=$(sleep 25 &; echo $!)
signal="SIGTERM"
killed5=$(kill -${signal} ${signalPid} 2>/dev/null && echo "true" || echo "false")
echo "Process killed using variable signal:"
echo "${killed5}"
echo "Testing process.kill() in conditional statements"
condPid=$(sleep 15 &; echo $!)
if [ $(ps -p ${condPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
  echo "Process is running, attempting to kill"
  killResult=$(kill -SIGTERM ${condPid} 2>/dev/null && echo "true" || echo "false")
  if [ "${killResult}" = "true" ]; then
    echo "Process successfully terminated"
  else
    echo "Failed to terminate process"
  fi
fi
echo "Testing graceful kill with fallback pattern"
gracefulPid=$(sleep 40 &; echo $!)
gracefulKill=$(kill -SIGTERM ${gracefulPid} 2>/dev/null && echo "true" || echo "false")
echo "Graceful kill result:"
echo "${gracefulKill}"
if [ $(ps -p ${gracefulPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
  echo "Process still running, using force kill"
  forceKill=$(kill -SIGKILL ${gracefulPid} 2>/dev/null && echo "true" || echo "false")
  echo "Force kill result:"
  echo "${forceKill}"
fi
echo "Testing complete process workflow"
manageProcess() {
  workPid=$(sleep 35 &; echo $!)
  echo "Workflow process started with PID:"
  echo "${workPid}"
  if [ $(ps -p ${workPid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
    echo "Process confirmed running"
    terminated=$(kill -SIGTERM ${workPid} 2>/dev/null && echo "true" || echo "false")
    if [ "${terminated}" = "true" ]; then
      echo "Workflow process terminated successfully"
    fi
  fi
}
manageProcess
echo "Testing multiple process management"
proc1=$(sleep 50 &; echo $!)
proc2=$(sleep 55 &; echo $!)
proc3=$(sleep 60 &; echo $!)
echo "Started three processes for batch termination"
kill1=$(kill -SIGTERM ${proc1} 2>/dev/null && echo "true" || echo "false")
kill2=$(kill -SIGKILL ${proc2} 2>/dev/null && echo "true" || echo "false")
kill3=$(kill -15 ${proc3} 2>/dev/null && echo "true" || echo "false")
echo "Batch kill results:"
echo "${kill1}"
echo "${kill2}"
echo "${kill3}"
echo "Testing kill on already terminated process"
quickPid=$(echo 'Quick task' &; echo $!)
deadKill=$(kill -SIGTERM ${quickPid} 2>/dev/null && echo "true" || echo "false")
echo "Kill result on already terminated process:"
echo "${deadKill}"
echo "All process.kill() tests completed"
