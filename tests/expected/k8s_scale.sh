#!/bin/bash

kubectl scale deployment/web-app --replicas=3
resource="statefulset"
name="database"
replicas=5
kubectl scale ${resource}/${name} --replicas=${replicas}
result=$(kubectl scale deployment/api-server --replicas=2)
