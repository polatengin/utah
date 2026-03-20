#!/bin/bash

result=$(docker exec "my-nginx" ls /usr/share/nginx/html)
echo "Files: ${result}"
container="web-server"
docker exec ${container} nginx -s reload
