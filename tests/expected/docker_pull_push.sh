#!/bin/bash

docker pull "nginx:latest"
docker push "myregistry/myapp:latest"
image="redis:alpine"
docker pull ${image}
