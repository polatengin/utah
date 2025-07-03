#!/bin/bash

# Utah CLI Regression Test Runner
# This script compiles all test fixtures and compares output with expected results

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Paths
TESTS_DIR="$(cd "$(dirname "$0")" && pwd)"
FIXTURES_DIR="$TESTS_DIR/fixtures"
EXPECTED_DIR="$TESTS_DIR/expected"
TEMP_DIR="$TESTS_DIR/temp"
CLI_DIR="$TESTS_DIR/../src/cli"

# Create temp directory for test outputs
mkdir -p "$TEMP_DIR"

# Function to clean up temp files
cleanup() {
    rm -rf "$TEMP_DIR"
}
trap cleanup EXIT

# Initialize counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

echo "🧪 Running Utah CLI Regression Tests"
echo "====================================="

# Build the CLI first
echo "🔨 Building Utah CLI..."
cd "$CLI_DIR"
if ! dotnet build --verbosity quiet > /dev/null 2>&1; then
    echo -e "${RED}❌ Failed to build Utah CLI${NC}"
    exit 1
fi
echo -e "${GREEN}✅ CLI built successfully${NC}"
echo

# Function to run a single test
run_test() {
    local fixture_file="$1"
    local test_name=$(basename "$fixture_file" .shx)
    local expected_file="$EXPECTED_DIR/$test_name.sh"
    local actual_file="$TEMP_DIR/$test_name.sh"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    echo -n "🔍 Testing $test_name... "
    
    # Check if expected file exists
    if [ ! -f "$expected_file" ]; then
        echo -e "${RED}❌ Expected file not found${NC}"
        FAILED_TESTS=$((FAILED_TESTS + 1))
        return 1
    fi
    
    # Compile the fixture
    if ! dotnet run --verbosity quiet -- compile "$fixture_file" > /dev/null 2>&1; then
        echo -e "${RED}❌ Compilation failed${NC}"
        FAILED_TESTS=$((FAILED_TESTS + 1))
        return 1
    fi
    
    # Move the generated file to temp directory
    mv "$FIXTURES_DIR/$test_name.sh" "$actual_file"
    
    # Compare with expected output
    if diff -u "$expected_file" "$actual_file" > /dev/null; then
        echo -e "${GREEN}✅ PASS${NC}"
        PASSED_TESTS=$((PASSED_TESTS + 1))
        return 0
    else
        echo -e "${RED}❌ FAIL${NC}"
        echo -e "${YELLOW}Expected vs Actual differences:${NC}"
        diff -u "$expected_file" "$actual_file" | head -20
        echo
        FAILED_TESTS=$((FAILED_TESTS + 1))
        return 1
    fi
}

# Run all tests
cd "$CLI_DIR"
for fixture_file in "$FIXTURES_DIR"/*.shx; do
    if [ -f "$fixture_file" ]; then
        run_test "$fixture_file"
    fi
done

echo
echo "📊 Test Results"
echo "==============="
echo -e "Total tests: $TOTAL_TESTS"
echo -e "${GREEN}Passed: $PASSED_TESTS${NC}"
if [ $FAILED_TESTS -gt 0 ]; then
    echo -e "${RED}Failed: $FAILED_TESTS${NC}"
else
    echo -e "Failed: $FAILED_TESTS"
fi

# Return appropriate exit code
if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "\n${GREEN}🎉 All tests passed!${NC}"
    exit 0
else
    echo -e "\n${RED}💥 Some tests failed!${NC}"
    exit 1
fi
