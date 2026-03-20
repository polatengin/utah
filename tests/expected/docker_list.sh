#!/bin/bash

containers=$(docker ps --format '{{.Names}}')
echo "Running containers: ${containers}"
