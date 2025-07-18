#!/bin/bash

name="${USER:-unknown}"
echo "Hello ${name}"
export GREETING="Utah Language"
greeting="${GREETING:-default}"
echo "${greeting}"
path="${PATH}"
echo "PATH length: ${#path}"
export START_TIME="$(date -Iseconds)"
export PID="$$"
export HOSTNAME="$(hostname)"
serverPort="${PORT:-8080}"
debugMode=$([ "${DEBUG:-false}" = "true" ] && echo "true" || echo "false")
enableMetrics=$([ "${ENABLE_METRICS:-true}" = "true" ] && echo "true" || echo "false")
echo "Server port: ${serverPort}"
echo "Debug mode: ${debugMode}"
echo "Metrics enabled: ${enableMetrics}"
appName="utah-transpiler"
version="1.0.0"
export APP_NAME=${appName}
export APP_VERSION=${version}
homeDir="${HOME:-/tmp}"
logDir="${LOG_DIR:-/var/log}"
dataDir="${DATA_DIR:-/var/lib}"
configPath="${homeDir}/.config/utah"
logFile="${logDir}/utah.log"
dataPath="${dataDir}/utah"
echo "Config path: ${configPath}"
echo "Log file: ${logFile}"
echo "Data directory: ${dataPath}"
[ -f ".env" ] && source ".env"
[ -f "config/.env.local" ] && source "config/.env.local"
export TEMP_VAR="temporary value"
tempValue="${TEMP_VAR:-}"
echo "Temp value before delete: ${tempValue}"
unset TEMP_VAR
deletedValue="${TEMP_VAR:-not found}"
echo "Temp value after delete: ${deletedValue}"
if [ "${NODE_ENV:-development}" = "production" ]; then
  export LOG_LEVEL="warn"
  export DEBUG="false"
else
  export LOG_LEVEL="debug"
  export DEBUG="true"
fi
timeout=$((TIMEOUT:-30 * 1000))
maxRetries=$((MAX_RETRIES:-3 + 1))
echo "Timeout in ms: ${timeout}"
echo "Max retries plus one: ${maxRetries}"
databaseUrl="${DATABASE_URL:-postgresql://user:pass@localhost:5432/db}"
apiEndpoint="${API_ENDPOINT:-https://api.example.com/v1}"
echo "Database URL: ${databaseUrl}"
echo "API Endpoint: ${apiEndpoint}"
getConfigValue() {
  local key="$1"
  local defaultValue="$2"
  echo "${${key}:-${defaultValue}}"
}
customConfig=$(getConfigValue "CUSTOM_CONFIG" "default-config")
echo "Custom config: ${customConfig}"
envVars=("HOME" "USER" "PATH" "SHELL")
i=0
while [ ${i} -lt ${#envVars[@]} ]; do
  envVar="${envVars[i]}"
  value="${${envVar}:-not set}"
  echo "$(({envVar}:  + value))"
  i=$((i + 1))
done
