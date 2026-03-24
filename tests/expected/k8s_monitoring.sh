#!/bin/bash

podUsage=$(kubectl top pods)
echo "Pod usage: ${podUsage}"
nsUsage=$(kubectl top pods -n "kube-system")
nodeUsage=$(kubectl top nodes)
echo "Node usage: ${nodeUsage}"
ns="monitoring"
monUsage=$(kubectl top pods -n ${ns})
