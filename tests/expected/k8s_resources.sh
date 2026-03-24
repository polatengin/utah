#!/bin/bash

allPods=$(kubectl get "pods" -o json)
pod=$(kubectl get "pods" "my-pod" -o json)
podInNs=$(kubectl get "deployments" "web-app" -n "production" -o json)
resourceType="services"
resourceName="api-gateway"
result=$(kubectl get ${resourceType} ${resourceName} -o json)
desc=$(kubectl describe "pod" "my-pod")
descNs=$(kubectl describe "deployment" "web-app" -n "staging")
exists=$(kubectl get "pod" "my-pod" > /dev/null 2>&1 && echo "true" || echo "false")
if [ "${exists}" = "true" ]; then
  echo "Pod exists"
fi
svcExists=$(kubectl get "service" "api-svc" -n "default" > /dev/null 2>&1 && echo "true" || echo "false")
if [ "${svcExists}" = "false" ]; then
  echo "Service not found"
fi
