#!/bin/bash

export NAME="Utah"
export VERSION="1.0.0"
envsubst < "template.txt" > "output.txt"
result=$(_utah_template_result_1=$(envsubst < "template.txt" > "output2.txt" && echo "true" || echo "false"); echo ${_utah_template_result_1})
echo "Template update result: ${result}"
