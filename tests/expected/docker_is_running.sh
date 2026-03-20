#!/bin/bash

running=$(docker inspect -f '{{.State.Running}}' "my-nginx" 2>/dev/null || echo "false")
if [ "${running}" = "true" ]; then
  echo "Container is running"
else
  echo "Container is not running"
fi
