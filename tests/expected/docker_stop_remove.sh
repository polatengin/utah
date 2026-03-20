#!/bin/bash

docker stop "my-nginx"
docker rm "my-nginx"
container="web-server"
docker stop ${container}
docker rm ${container}
