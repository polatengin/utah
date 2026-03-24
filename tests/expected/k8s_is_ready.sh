#!/bin/bash

ready=$(kubectl get pod "my-pod" -o jsonpath='{.status.conditions[?(@.type=="Ready")].status}' 2>/dev/null | grep -qi "true" && echo "true" || echo "false")
if [ "${ready}" = "true" ]; then
  echo "Pod is ready"
fi
readyInNs=$(kubectl get pod "api-pod" -n "production" -o jsonpath='{.status.conditions[?(@.type=="Ready")].status}' 2>/dev/null | grep -qi "true" && echo "true" || echo "false")
if [ "${readyInNs}" = "false" ]; then
  echo "Pod is not ready yet"
fi
