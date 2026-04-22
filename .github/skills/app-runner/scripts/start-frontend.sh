#!/usr/bin/env bash
# Start the React + Vite frontend dev server in the background.
# Installs dependencies if node_modules is missing.
#
# The server runs as a background process. Logs go to logs/frontend.log.
# PID is written to logs/frontend.pid. Use stop-servers.sh to stop.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../../.." && pwd)"
FRONTEND_DIR="$REPO_ROOT/frontend"
LOG_DIR="$SCRIPT_DIR/../logs"
LOG_FILE="$LOG_DIR/frontend.log"
PID_FILE="$LOG_DIR/frontend.pid"

mkdir -p "$LOG_DIR"

cd "$FRONTEND_DIR"

if [ ! -d "node_modules" ]; then
  echo "Installing frontend dependencies..."
  npm install
fi

# Kill any previous frontend on port 5173
if [ -f "$PID_FILE" ]; then
  OLD_PID=$(cat "$PID_FILE")
  if kill -0 "$OLD_PID" 2>/dev/null; then
    echo "Stopping previous frontend (PID $OLD_PID)..."
    kill "$OLD_PID" 2>/dev/null || true
    sleep 1
  fi
  rm -f "$PID_FILE"
fi

echo "Starting frontend dev server..."
nohup npm run dev > "$LOG_FILE" 2>&1 &
echo $! > "$PID_FILE"

# Poll until Vite is serving (max 20s)
for i in $(seq 1 20); do
  if curl -sf http://localhost:5173 > /dev/null 2>&1; then
    echo "Frontend is ready (PID $(cat "$PID_FILE"), logs: $LOG_FILE)"
    exit 0
  fi
  sleep 1
done

echo "ERROR: Frontend did not become ready within 20 seconds."
echo "Last 20 lines of log:"
tail -20 "$LOG_FILE" 2>/dev/null
exit 1
