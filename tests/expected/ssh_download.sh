#!/bin/bash

echo "Testing SSH download functionality"
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
  downloadSuccess=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/remote/file.txt" "/local/file.txt";   download_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/remote/file.txt" "/local/file.txt";   download_result=$?; fi; if [ $download_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${downloadSuccess}" = "true" ]; then
    echo "File downloaded successfully"
  else
    echo "Failed to download file"
  fi
  configDownload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/etc/app/config.yml" "/tmp/downloaded_config.yml";   download_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/etc/app/config.yml" "/tmp/downloaded_config.yml";   download_result=$?; fi; if [ $download_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${configDownload}" = "true" ]; then
    echo "Configuration file downloaded"
  fi
  logDownload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/var/log/app.log" "/local/logs/app.log";   download_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/var/log/app.log" "/local/logs/app.log";   download_result=$?; fi; if [ $download_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  scriptDownload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/opt/scripts/deploy.sh" "/local/scripts/deploy.sh";   download_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/opt/scripts/deploy.sh" "/local/scripts/deploy.sh";   download_result=$?; fi; if [ $download_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ ${logDownload} && ${scriptDownload} ]; then
    echo "All files downloaded successfully"
  else
    echo "Some downloads failed"
  fi
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
  backupDownload=$({ conn_var=${syncConnection}; if [ "$(eval "echo \${${syncConnection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${syncConnection}[socket]}")";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${syncConnection}[port]}")" "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")":"/backups/database.sql" "/local/database.sql";   download_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${syncConnection}[port]}")" "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")":"/backups/database.sql" "/local/database.sql";   download_result=$?; fi; if [ $download_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${backupDownload}" = "true" ]; then
    echo "Database backup downloaded"
  fi
fi
echo "SSH download test completed"
