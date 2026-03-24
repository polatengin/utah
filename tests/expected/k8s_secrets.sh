#!/bin/bash

secretData=$(kubectl get secret "my-secret" -o json)
password=$(kubectl get secret "db-credentials" -o jsonpath='{.data.password}' | base64 -d)
token=$(kubectl get secret "api-key" -n "production" -o jsonpath='{.data.token}' | base64 -d)
kubectl create secret generic "my-secret" --from-literal=api-key=abc123
secretName="db-creds"
key="password"
value="s3cret"
kubectl create secret generic ${secretName} --from-literal=${key}=${value}
