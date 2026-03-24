#!/bin/bash

ctx=$(kubectl config current-context)
echo "Current context: ${ctx}"
kubectl config use-context "production"
ns=$(kubectl config view --minify --output 'jsonpath={..namespace}' 2>/dev/null || echo "default")
echo "Current namespace: ${ns}"
kubectl config set-context --current --namespace="kube-system"
targetCtx="staging"
kubectl config use-context ${targetCtx}
targetNs="monitoring"
kubectl config set-context --current --namespace=${targetNs}
