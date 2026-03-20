#!/bin/bash

docker build -t "myapp:latest" .
docker build -t "myapp:v2" "./app"
result=$(docker build -t "myapp:v3" .)
