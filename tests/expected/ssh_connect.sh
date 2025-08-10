#!/bin/bash

echo "Testing SSH connection functionality"
basicConn=$({ declare -A _utah_ssh_conn_1; _utah_ssh_conn_1[host]="localhost"; _utah_ssh_conn_1[authMethod]="config"; _utah_ssh_conn_1[port]="22"; _utah_ssh_conn_1[username]="$(whoami)"; if ssh -o ConnectTimeout=5 -o BatchMode=yes -q "${_utah_ssh_conn_1[username]}@${_utah_ssh_conn_1[host]}" -p "${_utah_ssh_conn_1[port]}" exit 2>/dev/null; then;   _utah_ssh_conn_1[connected]="true"; else;   _utah_ssh_conn_1[connected]="false"; fi ; echo "_utah_ssh_conn_1"; })
echo "Basic connection created"
configConn=$({ declare -A _utah_ssh_conn_2; _utah_ssh_conn_2[host]="localhost"; _utah_ssh_conn_2[port]="22"; _utah_ssh_conn_2[username]="$(whoami)"; _utah_ssh_conn_2[authMethod]="config"; if ssh -o ConnectTimeout=5 -o BatchMode=yes -q "${_utah_ssh_conn_2[username]}@${_utah_ssh_conn_2[host]}" -p "${_utah_ssh_conn_2[port]}" exit 2>/dev/null; then;   _utah_ssh_conn_2[connected]="true"; else;   _utah_ssh_conn_2[connected]="false"; fi ; echo "_utah_ssh_conn_2"; })
echo "Connection with options created"
echo "SSH connection tests completed"
