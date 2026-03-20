#!/bin/bash

cores=$(nproc 2>/dev/null || sysctl -n hw.ncpu 2>/dev/null || echo "1")
echo "CPU cores: ${cores}"
