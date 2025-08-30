#!/bin/bash

echo "Testing fs.watch() functionality"
simpleWatchPid=$(_utah_watch_pid_1=$(inotifywait -m -e modify,create,delete,move "/tmp/test.log" --format '%w%f %e' | while read file event; do
  echo 'File changed: $1, Event: $2' "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_1}")
echo "Simple watch started with PID:"
echo "${simpleWatchPid}"
handleFileChange() {
  local filePath="$1"
  local eventType="$2"
  echo "Change detected!"
  echo "File: ${filePath}"
  echo "Event: ${eventType}"
  if [ "${eventType}" = "modify" ]; then
    echo "File was modified"
  elif [ "${eventType}" = "create" ]; then
    echo "File was created"
  elif [ "${eventType}" = "delete" ]; then
    echo "File was deleted"
  fi
}
directoryWatchPid=$(_utah_watch_pid_2=$(inotifywait -m -e modify,create,delete,move "/tmp" --format '%w%f %e' | while read file event; do
  handleFileChange "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_2}")
echo "Directory watch started with PID:"
echo "${directoryWatchPid}"
analyzeFileEvent() {
  local filePath="$1"
  local eventType="$2"
  if [ "${filePath##*.}" = ".log" ]; then
    echo "Log file event detected: ${eventType}"
    if [ [ "${eventType}" = "modify" ] && $([ -e ${filePath} ] && echo "true" || echo "false") ]; then
      fileSize=`$(stat -f%z "${filePath}" 2>/dev/null || stat -c%s "${filePath}" 2>/dev/null)`
      echo "File size: ${fileSize} bytes"
    fi
  fi
}
complexWatchPid=$(_utah_watch_pid_3=$(inotifywait -m -e modify,create,delete,move "/var/log/application.log" --format '%w%f %e' | while read file event; do
  analyzeFileEvent "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_3}")
echo "Complex watch started with PID:"
echo "${complexWatchPid}"
watchPath="/tmp/monitor"
callbackCommand="echo 'Variable watch triggered: $1 $2'"
variableWatchPid=$(_utah_watch_pid_4=$(inotifywait -m -e modify,create,delete,move "${watchPath}" --format '%w%f %e' | while read file event; do
  ${callbackCommand} "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_4}")
echo "Variable watch started with PID:"
echo "${variableWatchPid}"
configPath="/etc/myapp/config.yaml"
reloadConfiguration() {
  local filePath="$1"
  local eventType="$2"
  echo "Configuration changed, reloading..."
  echo "Config file: ${filePath}"
  if [ $([ -e ${filePath} ] && echo "true" || echo "false") ]; then
    echo "Configuration file exists, processing reload"
  else
    echo "Configuration file was deleted!"
  fi
}
configWatchPid=$(_utah_watch_pid_5=$(inotifywait -m -e modify,create,delete,move "${configPath}" --format '%w%f %e' | while read file event; do
  reloadConfiguration "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_5}")
echo "Config watch started with PID:"
echo "${configWatchPid}"
processDocumentChange() {
  local filePath="$1"
  local eventType="$2"
  extension="${filePath##*.}"
  if [ "${extension}" = ".pdf" ] || [ "${extension}" = ".doc" ] || [ "${extension}" = ".docx" ]; then
    echo "Document file changed: $(fs.filename "${filePath}")"
    echo "Event type: ${eventType}"
    echo "Directory: $(dirname ${filePath})"
  fi
}
documentWatchPid=$(_utah_watch_pid_6=$(inotifywait -m -e modify,create,delete,move "/home/user/Documents" --format '%w%f %e' | while read file event; do
  processDocumentChange "${file}" "${event}"
done & echo $!)
echo "${_utah_watch_pid_6}")
echo "Document watch started with PID:"
echo "${documentWatchPid}"
activePids=()
activePids+=(${simpleWatchPid})
activePids+=(${directoryWatchPid})
activePids+=(${complexWatchPid})
echo "Active watch processes:"
for pid in "${activePids[@]}"; do
  echo "PID: ${pid}"
  if [ $(ps -p ${pid} -o pid= > /dev/null 2>&1 && echo "true" || echo "false") ]; then
    echo "  Status: Running"
  else
    echo "  Status: Not running"
  fi
done
echo "fs.watch() testing completed"
