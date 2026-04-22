#!/usr/bin/env bash
# Start the .NET backend API server in the background.
# Usage: ./start-backend.sh [dev|prod]
# - dev:  ASPNETCORE_ENVIRONMENT=Development (enables Swagger UI at /swagger)
# - prod: ASPNETCORE_ENVIRONMENT=Production  (Swagger disabled)
#
# The server runs as a background process. Logs go to logs/backend.log.
# PID is written to logs/backend.pid. Use stop-servers.sh to stop.

set -euo pipefail

MODE="${1:-prod}"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../../.." && pwd)"
BACKEND_DIR="$REPO_ROOT/backend"
LOG_DIR="$SCRIPT_DIR/../logs"
LOG_FILE="$LOG_DIR/backend.log"
PID_FILE="$LOG_DIR/backend.pid"

mkdir -p "$LOG_DIR"

case "$MODE" in
  dev)
    export ASPNETCORE_ENVIRONMENT=Development
    ;;
  prod)
    export ASPNETCORE_ENVIRONMENT=Production
    ;;
  *)
    echo "Unknown mode: $MODE  (expected 'dev' or 'prod')"
    exit 1
    ;;
esac

# Kill any previous backend on port 5000
if [ -f "$PID_FILE" ]; then
  OLD_PID=$(cat "$PID_FILE")
  if kill -0 "$OLD_PID" 2>/dev/null; then
    echo "Stopping previous backend (PID $OLD_PID)..."
    kill "$OLD_PID" 2>/dev/null || true
    sleep 1
  fi
  rm -f "$PID_FILE"
fi

echo "Starting backend in $ASPNETCORE_ENVIRONMENT mode..."
cd "$BACKEND_DIR"
nohup dotnet run --project src/Api > "$LOG_FILE" 2>&1 &
echo $! > "$PID_FILE"

# Poll until the health endpoint responds (max 30s)
for i in $(seq 1 30); do
  if curl -sf http://localhost:5000/v1/health > /dev/null 2>&1; then
    echo "Backend is ready (PID $(cat "$PID_FILE"), logs: $LOG_FILE)"
    exit 0
  fi
  sleep 1
done

echo "ERROR: Backend did not become ready within 30 seconds."
echo "Last 20 lines of log:"
tail -20 "$LOG_FILE" 2>/dev/null
exit 1
