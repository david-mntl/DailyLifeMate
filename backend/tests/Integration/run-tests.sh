#!/bin/bash

# -e: Exit immediately if a command fails
# -o pipefail: Ensure pipeline fails if any command inside it fails
set -eo pipefail

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
TEST_LOG_DIR="$SCRIPT_DIR/test-logs"
TEST_LOG_FILE="$TEST_LOG_DIR/test-runner-$(date +%Y%m%d_%H%M%S).log"
DOCKER_COMPOSE_FILE="$SCRIPT_DIR/docker-compose.integration.yml"

mkdir -p "$TEST_LOG_DIR"

cleanup() {
    # Capture the exit code of the script just before cleanup
    local exit_status=$?
    
    echo ""
    echo "🧹 Executing safe teardown (Trap triggered)..."
    
    docker compose -f "$DOCKER_COMPOSE_FILE" down -v
    
    # Return the original exit status so the pipeline knows if tests passed or failed
    exit $exit_status
}

# EXIT: Standard script end (whether success or error due to 'set -e')
# INT: User presses Ctrl+C
# TERM: Process killed by system (e.g., CI/CD timeout)
trap cleanup EXIT INT TERM

# ==========================================
# EXECUTION
# ==========================================
echo "🚀 Starting Anime Controller Integration Tests..."
echo "📝 Test logs will be saved to: $TEST_LOG_FILE"
echo ""

if docker compose -f "$DOCKER_COMPOSE_FILE" up --build --abort-on-container-exit --exit-code-from test-runner 2>&1 | tee "$TEST_LOG_FILE"; then
    EXIT_CODE=0
    echo ""
    echo "✅ All tests passed!"
else
    EXIT_CODE=$? 
    echo ""
    echo "❌ Tests failed with exit code: $EXIT_CODE"
    echo "===================================="
fi

echo "📊 Full log: $TEST_LOG_FILE"

# Calling exit will trigger the 'EXIT' trap.
exit $EXIT_CODE