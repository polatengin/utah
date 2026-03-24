#!/bin/bash

result=$(kubectl exec "my-pod" -- ls /app)
output=$(kubectl exec "my-pod" --container="nginx" -- cat /etc/config)
kubectl exec "my-pod" -- nginx -s reload
pod="web-server"
cmd="whoami"
kubectl exec ${pod} -- ${cmd}
