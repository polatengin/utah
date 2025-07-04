#!/bin/sh

proceed=$(while true; do read -p "Do you want to continue? (y/n): " yn; case $yn in [Yy]* ) echo "true"; break;; [Nn]* ) echo "false"; break;; * ) echo "Please answer yes or no.";; esac; done)
echo "User response: ${proceed}"
if [ "${proceed}" = "true" ]; then
  echo "User chose to continue"
else
  echo "User chose to cancel"
fi
