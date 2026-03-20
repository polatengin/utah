#!/bin/bash

docker restart "my-nginx"
container="web-server"
docker restart ${container}
