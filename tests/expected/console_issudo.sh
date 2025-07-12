#!/bin/bash

isSudo=$([ "$(id -u)" -eq 0 ] && echo "true" || echo "false")
echo "${isSudo}"
