#!/bin/bash

exists=$(docker image inspect "nginx:latest" > /dev/null 2>&1 && echo "true" || echo "false")
if [ "${exists}" = "true" ]; then
  echo "Image exists"
  docker rmi "nginx:latest"
else
  echo "Image not found"
fi
