#!/bin/sh

name="${USER:-unknown}"
echo "Hello $name"
export GREETING="Utah Language"
greeting="${GREETING:-default}"
echo "$greeting"
