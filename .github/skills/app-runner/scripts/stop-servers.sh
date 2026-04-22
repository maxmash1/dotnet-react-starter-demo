#!/usr/bin/env bash
# Stop the backend and/or frontend servers started by the app-runner scripts.
# Usage: ./stop-servers.sh [backend|frontend|all]
# Default: all

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
LOG_DIR="$SCRIPT_DIR/../logs"
TARGET="${1:-all}"

stop_service() {
  local name="$1"
  local pid_file="$LOG_DIR/${name}.pid"

  if [ ! -f "$pid_file" ]; then
    echo "$name: no PID file found (not running or started manually)"
    return
  fi

  local pid
  pid=$(cat "$pid_file")

  if kill -0 "$pid" 2>/dev/null; then
    echo "$name: stopping PID $pid..."
    kill "$pid" 2>/dev/null || true
    sleep 1
    # Force kill if still alive
    if kill -0 "$pid" 2>/dev/null; then
      kill -9 "$pid" 2>/dev/null || true
    fi
    echo "$name: stopped"
  else
    echo "$name: process $pid already exited"
  fi

  rm -f "$pid_file"
}

case "$TARGET" in
  backend)  stop_service "backend" ;;
  frontend) stop_service "frontend" ;;
  all)
    stop_service "backend"
    stop_service "frontend"
    ;;
  *)
    echo "Usage: $0 [backend|frontend|all]"
    exit 1
    ;;
esac
