#!/bin/sh

_uname_os_get_os=$(uname | tr '[:upper:]' '[:lower:]')
case $_uname_os_get_os in
  linux*)
    currentOS="linux"
    ;;
  darwin*)
    currentOS="mac"
    ;;
  msys* | cygwin* | mingw* | nt | win*)
    currentOS="windows"
    ;;
  *)
    currentOS="unknown"
    ;;
esac
echo "Current OS: $currentOS"
