#!/bin/bash

kubectl port-forward "my-pod" 8080:80 > /dev/null 2>&1 &
pid=$(kubectl port-forward "api-pod" 3000:8080 > /dev/null 2>&1 & echo $!)
echo "Port forward PID: ${pid}"
podName="web-pod"
kubectl port-forward ${podName} 9090:443 > /dev/null 2>&1 &
