#!/bin/bash

output=$(docker logs "my-nginx")
echo "Logs: ${output}"
container="web-server"
logs=$(docker logs ${container})
