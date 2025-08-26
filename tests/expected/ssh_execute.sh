#!/bin/bash

echo "Testing SSH execute functionality"
declare -A _utah_ssh_conn_1
_utah_ssh_conn_1[host]="test.server.com"
_utah_ssh_conn_1[socket]="/tmp/utah_ssh_1"
_utah_ssh_conn_1[port]="22"
_utah_ssh_conn_1[username]="$(whoami)"
_utah_ssh_conn_1[authMethod]="config"
_utah_ssh_conn_1[async]="false"
_utah_ssh_conn_1[port]=22
_utah_ssh_conn_1[username]="testuser"
_utah_ssh_conn_1[keyPath]="/home/user/.ssh/test_key"
_utah_ssh_conn_1[authMethod]="key"
_utah_ssh_conn_1[async]=true
if [ "${_utah_ssh_conn_1[async]}" = "true" ]; then
  ssh_cmd="ssh -M -S ${_utah_ssh_conn_1[socket]} -o ControlPersist=600 -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_1[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_1[keyPath]}"
  elif [ "${_utah_ssh_conn_1[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_1[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_1[username]}@${_utah_ssh_conn_1[host]}" -p "${_utah_ssh_conn_1[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_1[connected]="true"
  else
    _utah_ssh_conn_1[connected]="false"
  fi
else
  ssh_cmd="ssh -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_1[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_1[keyPath]}"
  elif [ "${_utah_ssh_conn_1[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_1[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_1[username]}@${_utah_ssh_conn_1[host]}" -p "${_utah_ssh_conn_1[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_1[connected]="true"
  else
    _utah_ssh_conn_1[connected]="false"
  fi
fi
connection="_utah_ssh_conn_1"
if [ $(eval "echo \${${connection}[connected]}") ]; then
  echo "Connection established successfully"
  hostnameResult=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "hostname"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "hostname"; fi })
  echo "Hostname: ${hostnameResult}"
  processCount=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ps aux | wc -l"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ps aux | wc -l"; fi })
  echo "Process count: ${processCount}"
  currentDir=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "pwd"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "pwd"; fi })
  echo "Current directory: ${currentDir}"
  fileList=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ls -la /tmp"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ls -la /tmp"; fi })
  echo "Temp directory listing completed"
  uptimeResult=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "uptime"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "uptime"; fi })
  echo "System uptime: ${uptimeResult}"
  diskUsage=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "df -h /"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "df -h /"; fi })
  echo "Disk usage retrieved"
  createResult=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "echo 'Test content' > /tmp/execute_test.txt"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "echo 'Test content' > /tmp/execute_test.txt"; fi })
  readResult=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "cat /tmp/execute_test.txt"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "cat /tmp/execute_test.txt"; fi })
  echo "File content: ${readResult}"
  failResult=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ls /nonexistent/directory 2>/dev/null || echo 'Directory not found'"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "ls /nonexistent/directory 2>/dev/null || echo 'Directory not found'"; fi })
  echo "Fail test result: ${failResult}"
else
  echo "Failed to connect to server"
  exit 1
fi
declare -A _utah_ssh_conn_2
_utah_ssh_conn_2[host]="backup.server.com"
_utah_ssh_conn_2[socket]="/tmp/utah_ssh_2"
_utah_ssh_conn_2[port]="22"
_utah_ssh_conn_2[username]="$(whoami)"
_utah_ssh_conn_2[authMethod]="config"
_utah_ssh_conn_2[async]="false"
_utah_ssh_conn_2[username]="backup"
_utah_ssh_conn_2[keyPath]="/secure/backup_key"
_utah_ssh_conn_2[authMethod]="key"
if [ "${_utah_ssh_conn_2[async]}" = "true" ]; then
  ssh_cmd="ssh -M -S ${_utah_ssh_conn_2[socket]} -o ControlPersist=600 -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_2[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_2[keyPath]}"
  elif [ "${_utah_ssh_conn_2[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_2[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_2[username]}@${_utah_ssh_conn_2[host]}" -p "${_utah_ssh_conn_2[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_2[connected]="true"
  else
    _utah_ssh_conn_2[connected]="false"
  fi
else
  ssh_cmd="ssh -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_2[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_2[keyPath]}"
  elif [ "${_utah_ssh_conn_2[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_2[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_2[username]}@${_utah_ssh_conn_2[host]}" -p "${_utah_ssh_conn_2[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_2[connected]="true"
  else
    _utah_ssh_conn_2[connected]="false"
  fi
fi
syncConnection="_utah_ssh_conn_2"
if [ $(eval "echo \${${syncConnection}[connected]}") ]; then
  echo "Sync connection established"
  whoami=$({ conn_var=${syncConnection}; if [ "$(eval "echo \${${syncConnection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${syncConnection}[socket]}")";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")" -p "$(eval "echo \${${syncConnection}[port]}")" "whoami"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")" -p "$(eval "echo \${${syncConnection}[port]}")" "whoami"; fi })
  echo "Current user on sync connection: ${whoami}"
  dateResult=$({ conn_var=${syncConnection}; if [ "$(eval "echo \${${syncConnection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${syncConnection}[socket]}")";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")" -p "$(eval "echo \${${syncConnection}[port]}")" "date"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")" -p "$(eval "echo \${${syncConnection}[port]}")" "date"; fi })
  echo "Server date: ${dateResult}"
fi
echo "SSH execute test completed"
