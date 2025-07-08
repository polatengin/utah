#!/bin/sh

port=$([ -n "${PORT}" ] && echo "${PORT}" || echo "8080")
