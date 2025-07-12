#!/bin/bash

version=$(if [[ -f /etc/os-release ]]; then source /etc/os-release; echo "${VERSION_ID}"; elif type lsb_release >/dev/null 2>&1; then lsb_release -sr; elif [[ -f /etc/lsb-release ]]; then source /etc/lsb-release; echo "${DISTRIB_RELEASE}"; else echo "unknown"; fi)
echo "${version}"
