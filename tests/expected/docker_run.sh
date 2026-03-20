#!/bin/bash

docker run -d nginx
docker run -d --name my-nginx nginx
docker run -d --name web-server -p 8080:80 nginx
docker run -d --name full-server -p 8080:80 -v /data:/usr/share/nginx/html nginx
containerId=$(docker run -d --name my-redis redis)
echo "Started container: ${containerId}"
