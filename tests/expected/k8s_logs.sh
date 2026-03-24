#!/bin/bash

output=$(kubectl logs "my-pod")
containerLogs=$(kubectl logs "my-pod" --container="nginx")
tailedLogs=$(kubectl logs "my-pod" --container="nginx" --tail=100)
prevLogs=$(kubectl logs "my-pod" --container="sidecar" --tail=50 --previous)
podName="api-server"
containerName="app"
logs=$(kubectl logs ${podName} --container=${containerName})
