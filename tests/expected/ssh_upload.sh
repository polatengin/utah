#!/bin/bash

echo "Testing SSH upload functionality"
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
  echo "Hello World from Utah SSH upload test!" > "/tmp/test_file.txt"
  uploadSuccess=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/test_file.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_test_file.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/test_file.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_test_file.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${uploadSuccess}" = "true" ]; then
    echo "Basic file upload successful"
    remoteContent=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   ssh_cmd="ssh -S $(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "cat /tmp/remote_test_file.txt"; else;   # One-time connection;   ssh_cmd="ssh";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     ssh_cmd="$ssh_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     ssh_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $ssh_cmd";   fi;   $ssh_cmd "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")" -p "$(eval "echo \${${connection}[port]}")" "cat /tmp/remote_test_file.txt"; fi })
    echo "Remote file content: ${remoteContent}"
  else
    echo "Basic file upload failed"
  fi
  echo "Binary content with special chars: \n\t!@#$%^&*()" > "/tmp/binary_test.dat"
  binaryUpload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/binary_test.dat" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_binary.dat";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/binary_test.dat" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_binary.dat";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${binaryUpload}" = "true" ]; then
    echo "Binary file upload successful"
  else
    echo "Binary file upload failed"
  fi
  scriptContent="#!/bin/bash\necho 'This is a test script'\ndate\n"
  echo ${scriptContent} > "/tmp/test_script.sh"
  scriptUpload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/test_script.sh" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/scripts/test_script.sh";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/test_script.sh" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/scripts/test_script.sh";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  echo "Script upload result: ${scriptUpload}"
  configContent="# Configuration file\nserver_name=test_server\nport=8080\ndebug=true\n"
  echo ${configContent} > "/tmp/app.conf"
  configUpload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/app.conf" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/etc/app/app.conf";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/app.conf" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/etc/app/app.conf";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  echo "Config upload result: ${configUpload}"
  echo "Content of file 1" > "/tmp/file1.txt"
  echo "Content of file 2" > "/tmp/file2.txt"
  echo "Content of file 3" > "/tmp/file3.txt"
  upload1=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file1.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file1.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file1.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file1.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  upload2=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file2.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file2.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file2.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file2.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  upload3=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file3.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file3.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/file3.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_file3.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ ${upload1} && ${upload2} && ${upload3} ]; then
    echo "All multiple file uploads successful"
  else
    echo "Some multiple file uploads failed"
  fi
  largeContent=""
  i=0
  while [ ${i} -lt 100 ]; do
    largeContent="${largeContent}Line with more content to test larger file uploads.\n"
    i=$((i + 1))
  done
  echo ${largeContent} > "/tmp/large_file.txt"
  largeUpload=$({ conn_var=${connection}; if [ "$(eval "echo \${${connection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${connection}[socket]}")";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/large_file.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_large_file.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${connection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${connection}[keyPath]}")";   elif [ "$(eval "echo \${${connection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${connection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${connection}[port]}")" "/tmp/large_file.txt" "$(eval "echo \${${connection}[username]}")@$(eval "echo \${${connection}[host]}")":"/tmp/remote_large_file.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  echo "Large file upload result: ${largeUpload}"
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
  echo "Sync connection upload test" > "/tmp/sync_test.txt"
  syncUpload=$({ conn_var=${syncConnection}; if [ "$(eval "echo \${${syncConnection}[async]}")" = "true" ]; then;   # Use control master for async connection;   scp_cmd="scp -o ControlPath=$(eval "echo \${${syncConnection}[socket]}")";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${syncConnection}[port]}")" "/tmp/sync_test.txt" "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")":"/tmp/remote_sync_test.txt";   upload_result=$?; else;   # One-time connection;   scp_cmd="scp";   if [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "key" ]; then;     scp_cmd="$scp_cmd -i $(eval "echo \${${syncConnection}[keyPath]}")";   elif [ "$(eval "echo \${${syncConnection}[authMethod]}")" = "password" ]; then;     scp_cmd="sshpass -p $(eval "echo \${${syncConnection}[password]}") $scp_cmd";   fi;   $scp_cmd -P "$(eval "echo \${${syncConnection}[port]}")" "/tmp/sync_test.txt" "$(eval "echo \${${syncConnection}[username]}")@$(eval "echo \${${syncConnection}[host]}")":"/tmp/remote_sync_test.txt";   upload_result=$?; fi; if [ $upload_result -eq 0 ]; then echo "true"; else echo "false"; fi })
  if [ "${syncUpload}" = "true" ]; then
    echo "Sync connection upload successful"
  else
    echo "Sync connection upload failed"
  fi
fi
echo "SSH upload test completed"
