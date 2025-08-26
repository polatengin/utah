#!/bin/bash

echo "Testing SSH connection functionality"
declare -A _utah_ssh_conn_1
_utah_ssh_conn_1[host]="localhost"
_utah_ssh_conn_1[socket]="/tmp/utah_ssh_1"
_utah_ssh_conn_1[port]="22"
_utah_ssh_conn_1[username]="$(whoami)"
_utah_ssh_conn_1[authMethod]="config"
_utah_ssh_conn_1[async]="false"
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
basicConn="_utah_ssh_conn_1"
echo "Basic connection created"
echo "Basic connection status: $(eval "echo \${${basicConn}[connected]}")"
declare -A _utah_ssh_conn_2
_utah_ssh_conn_2[host]="localhost"
_utah_ssh_conn_2[socket]="/tmp/utah_ssh_2"
_utah_ssh_conn_2[port]="22"
_utah_ssh_conn_2[username]="$(whoami)"
_utah_ssh_conn_2[authMethod]="config"
_utah_ssh_conn_2[async]="false"
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
configConn="_utah_ssh_conn_2"
echo "Connection with options created"
echo "Config connection status: $(eval "echo \${${configConn}[connected]}")"
declare -A _utah_ssh_conn_3
_utah_ssh_conn_3[host]="localhost"
_utah_ssh_conn_3[socket]="/tmp/utah_ssh_3"
_utah_ssh_conn_3[port]="22"
_utah_ssh_conn_3[username]="$(whoami)"
_utah_ssh_conn_3[authMethod]="config"
_utah_ssh_conn_3[async]="false"
_utah_ssh_conn_3[username]="testuser"
_utah_ssh_conn_3[async]=true
if [ "${_utah_ssh_conn_3[async]}" = "true" ]; then
  ssh_cmd="ssh -M -S ${_utah_ssh_conn_3[socket]} -o ControlPersist=600 -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_3[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_3[keyPath]}"
  elif [ "${_utah_ssh_conn_3[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_3[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_3[username]}@${_utah_ssh_conn_3[host]}" -p "${_utah_ssh_conn_3[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_3[connected]="true"
  else
    _utah_ssh_conn_3[connected]="false"
  fi
else
  ssh_cmd="ssh -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_3[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_3[keyPath]}"
  elif [ "${_utah_ssh_conn_3[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_3[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_3[username]}@${_utah_ssh_conn_3[host]}" -p "${_utah_ssh_conn_3[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_3[connected]="true"
  else
    _utah_ssh_conn_3[connected]="false"
  fi
fi
asyncConn="_utah_ssh_conn_3"
echo "Async connection created"
echo "Async connection status: $(eval "echo \${${asyncConn}[connected]}")"
echo "Async connection host: $(eval "echo \${${asyncConn}[host]}")"
echo "Async connection username: $(eval "echo \${${asyncConn}[username]}")"
echo "Async connection auth method: $(eval "echo \${${asyncConn}[authMethod]}")"
declare -A _utah_ssh_conn_4
_utah_ssh_conn_4[host]="localhost"
_utah_ssh_conn_4[socket]="/tmp/utah_ssh_4"
_utah_ssh_conn_4[port]="22"
_utah_ssh_conn_4[username]="$(whoami)"
_utah_ssh_conn_4[authMethod]="config"
_utah_ssh_conn_4[async]="false"
_utah_ssh_conn_4[username]="testuser"
_utah_ssh_conn_4[keyPath]="/home/user/.ssh/id_rsa"
_utah_ssh_conn_4[authMethod]="key"
_utah_ssh_conn_4[async]=true
if [ "${_utah_ssh_conn_4[async]}" = "true" ]; then
  ssh_cmd="ssh -M -S ${_utah_ssh_conn_4[socket]} -o ControlPersist=600 -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_4[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_4[keyPath]}"
  elif [ "${_utah_ssh_conn_4[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_4[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_4[username]}@${_utah_ssh_conn_4[host]}" -p "${_utah_ssh_conn_4[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_4[connected]="true"
  else
    _utah_ssh_conn_4[connected]="false"
  fi
else
  ssh_cmd="ssh -o ConnectTimeout=5"
  if [ "${_utah_ssh_conn_4[authMethod]}" = "key" ]; then
    ssh_cmd="$ssh_cmd -i ${_utah_ssh_conn_4[keyPath]}"
  elif [ "${_utah_ssh_conn_4[authMethod]}" = "password" ]; then
    ssh_cmd="sshpass -p ${_utah_ssh_conn_4[password]} $ssh_cmd"
  fi
  $ssh_cmd -o BatchMode=yes -q "${_utah_ssh_conn_4[username]}@${_utah_ssh_conn_4[host]}" -p "${_utah_ssh_conn_4[port]}" exit 2>/dev/null
  if [ $? -eq 0 ]; then
    _utah_ssh_conn_4[connected]="true"
  else
    _utah_ssh_conn_4[connected]="false"
  fi
fi
keyConn="_utah_ssh_conn_4"
echo "Key-based connection created"
echo "Key connection auth method: $(eval "echo \${${keyConn}[authMethod]}")"
if [ $(eval "echo \${${asyncConn}[connected]}") ]; then
  result=$({ conn_var=${asyncConn}; if [ "$(eval "echo \${${asyncConn}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${asyncConn}[socket]}")";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "echo 'Hello from remote'"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "echo 'Hello from remote'"; fi })
  echo "Execute result: ${result}"
  dirList=$({ conn_var=${asyncConn}; if [ "$(eval "echo \${${asyncConn}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${asyncConn}[socket]}")";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "ls -la /tmp"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "ls -la /tmp"; fi })
  echo "Directory listing completed"
fi
if [ $(eval "echo \${${asyncConn}[connected]}") ]; then
  testContent="Hello World from Utah!"
  echo ${testContent} > "/tmp/test_upload.txt"
  uploadSuccess=$({ conn_var=${asyncConn}; if [ "$(eval "echo \${${asyncConn}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${asyncConn}[socket]}")";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${asyncConn}[port]}")" "/tmp/test_upload.txt" "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")":"/tmp/remote_test.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${asyncConn}[port]}")" "/tmp/test_upload.txt" "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")":"/tmp/remote_test.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  echo "Upload result: ${uploadSuccess}"
  if [ "${uploadSuccess}" = "true" ]; then
    remoteContent=$({ conn_var=${asyncConn}; if [ "$(eval "echo \${${asyncConn}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${asyncConn}[socket]}")";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "cat /tmp/remote_test.txt"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${asyncConn}[keyPath]}")";   elif [ "$(eval "echo \${${asyncConn}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${asyncConn}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${asyncConn}[username]}")@$(eval "echo \${${asyncConn}[host]}")" -p "$(eval "echo \${${asyncConn}[port]}")" "cat /tmp/remote_test.txt"; fi })
    echo "Remote file content: ${remoteContent}"
  fi
fi
echo "SSH connection tests completed"
