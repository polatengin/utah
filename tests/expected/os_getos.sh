#!/bin/sh

currentOS=$(_uname_os_get_os=$(uname | tr '[:upper:]' '[:lower:]'); case $_uname_os_get_os in linux*) echo "linux" ;; darwin*) echo "mac" ;; msys*|cygwin*|mingw*|nt|win*) echo "windows" ;; *) echo "unknown" ;; esac)
echo "Current OS: ${currentOS}"
