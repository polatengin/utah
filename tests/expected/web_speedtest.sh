#!/bin/bash

speedData=$(curl -w '{"download_speed":"%{speed_download}","upload_speed":"0","time_total":"%{time_total}","time_connect":"%{time_connect}","time_pretransfer":"%{time_pretransfer}","size_download":"%{size_download}","response_code":"%{response_code}"}' --silent --output /dev/null "https://httpbin.org/get" 2>/dev/null || echo '{"error":"failed"}')
echo "Speed test result: ${speedData}"
testUrl="https://httpbin.org/delay/1"
speedResult=$(curl -w '{"download_speed":"%{speed_download}","upload_speed":"0","time_total":"%{time_total}","time_connect":"%{time_connect}","time_pretransfer":"%{time_pretransfer}","size_download":"%{size_download}","response_code":"%{response_code}"}' --silent --output /dev/null ${testUrl} 2>/dev/null || echo '{"error":"failed"}')
echo "Variable URL speed test: ${speedResult}"
timedTest=$(curl "--max-time 5" -w '{"download_speed":"%{speed_download}","upload_speed":"0","time_total":"%{time_total}","time_connect":"%{time_connect}","time_pretransfer":"%{time_pretransfer}","size_download":"%{size_download}","response_code":"%{response_code}"}' --silent --output /dev/null "https://httpbin.org/get" 2>/dev/null || echo '{"error":"failed"}')
echo "Timed speed test: ${timedTest}"
results=$(echo ${speedData} | jq .)
downloadSpeed=$(echo "${results}" | jq -r '.download_speed')
totalTime=$(echo "${results}" | jq -r '.time_total')
echo "Download speed: ${downloadSpeed} bytes/sec in ${totalTime} seconds"
