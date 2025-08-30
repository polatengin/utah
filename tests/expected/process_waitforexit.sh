#!/bin/bash

echo "Testing basic process.waitForExit() usage"
quickPid=$(echo 'Quick task completed' &; echo $!)
exitCode1=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${quickPid} 0
)
echo "Quick process exit code: ${exitCode1}"
echo "Testing process.waitForExit() with timeout - success case"
normalPid=$(sleep 1 &; echo $!)
exitCode2=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${normalPid} 3000
)
if [ ${exitCode2} -eq 0 ]; then
  echo "Normal process completed successfully"
elif [ "${exitCode2}" = -1 ]; then
  echo "Normal process timed out"
else
  echo "Normal process failed with exit code: ${exitCode2}"
fi
echo "Testing process.waitForExit() with timeout - timeout case"
longPid=$(sleep 5 &; echo $!)
exitCode3=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${longPid} 1000
)
if [ "${exitCode3}" = -1 ]; then
  echo "Long process timed out as expected"
else
  echo "Long process unexpectedly completed with exit code: ${exitCode3}"
fi
echo "Testing process.waitForExit() with failing process"
failPid=$(false &; echo $!)
exitCode4=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${failPid} 0
)
echo "Failing process exit code: ${exitCode4}"
echo "Testing multiple processes workflow"
pid1=$(echo 'Task 1' &; echo $!)
pid2=$(echo 'Task 2' &; echo $!)
pid3=$(echo 'Task 3' &; echo $!)
result1=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${pid1} 2000
)
result2=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${pid2} 2000
)
result3=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${pid3} 2000
)
if [ ${result1} -eq 0 ] && [ ${result2} -eq 0 ] && [ ${result3} -eq 0 ]; then
  echo "All tasks completed successfully"
else
  echo "Some tasks failed or timed out"
fi
echo "Testing process.waitForExit() with variable PID"
variablePid=$(echo 'Variable PID test' &; echo $!)
savedPid=${variablePid}
exitCode5=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${savedPid} 0
)
echo "Variable PID process exit code: ${exitCode5}"
echo "Testing process.waitForExit() in function context"
testProcessInFunction() {
  funcPid=$(echo 'Function process' &; echo $!)
  echo "$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${funcPid} 5000
)"
}
funcResult=$(testProcessInFunction)
echo "Function process exit code: ${funcResult}"
echo "Testing sequential workflow simulation"
step1Pid=$(echo 'Step 1 complete' &; echo $!)
step1Result=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${step1Pid} 3000
)
if [ ${step1Result} -eq 0 ]; then
  echo "Step 1 succeeded, starting step 2"
  step2Pid=$(echo 'Step 2 complete' &; echo $!)
  step2Result=$(
_utah_wait_for_exit() {
  local pid=$1
  local timeout_ms=$2
  
  # Fixed 100ms polling interval
  local poll_interval="0.1"
  
  # Convert milliseconds to seconds for timeout calculation
  local timeout_seconds=$((timeout_ms / 1000))
  local start_time=$(date +%s)
  local elapsed=0
  
  while true; do
    # Check if process is still running
    if ! ps -p $pid > /dev/null 2>&1; then
      # Process finished, get exit code from wait if possible
      # Note: wait only works for child processes, so we'll return 0 for external processes
      wait $pid 2>/dev/null || echo 0
      return 0
    fi
    
    # Check timeout (only if timeout > 0)
    if [ $timeout_ms -gt 0 ]; then
      elapsed=$(($(date +%s) - start_time))
      if [ $elapsed -ge $timeout_seconds ]; then
        echo -1  # Timeout indicator
        return 0
      fi
    fi
    
    # Sleep for fixed 100ms interval
    sleep $poll_interval
  done
}
_utah_wait_for_exit ${step2Pid} 3000
)
  if [ ${step2Result} -eq 0 ]; then
    echo "Step 2 succeeded, workflow complete"
  else
    echo "Step 2 failed"
  fi
else
  echo "Step 1 failed, aborting workflow"
fi
echo "process.waitForExit() tests completed"
