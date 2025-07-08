# Utah Language Development Makefile

.PHONY: help build build-extension test compile clean install info dev watch
.DEFAULT_GOAL := help

CLI_DIR := src/cli
VSCODE_DIR := src/vscode-extension
TESTS_DIR := tests
FIXTURES_DIR := $(TESTS_DIR)/positive_fixtures
EXPECTED_DIR := $(TESTS_DIR)/expected
NEGATIVE_TESTS_DIR := $(TESTS_DIR)/negative_fixtures
TEMP_DIR := $(TESTS_DIR)/temp

RED := \033[0;31m
GREEN := \033[0;32m
YELLOW := \033[1;33m
BLUE := \033[0;34m
NC := \033[0m # No Color

help:
	@echo "Utah Development Helper"
	@echo "======================="
	@echo
	@echo "Usage: make <target>"
	@echo
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "  $(YELLOW)%-15s$(NC) %s\n", $$1, $$2}'
	@echo
	@echo "Examples:"
	@echo "  make build"
	@echo "  make test"
	@echo "  make compile FILE=examples/hello.shx"

build: ## Build the CLI
	@echo "$(BLUE)🔨 Building Utah CLI...$(NC)"
	@cd $(CLI_DIR) && dotnet build
	@echo "$(GREEN)✅ Build complete$(NC)"

test: build ## Run all regression tests
	@echo "$(BLUE)🧪 Running Utah CLI Regression Tests$(NC)"
	@echo "====================================="
	@mkdir -p $(TEMP_DIR)
	@total=0; passed=0; failed=0; \
	echo "$(BLUE)Running positive tests...$(NC)"; \
	for fixture in $(FIXTURES_DIR)/*.shx; do \
		if [ -f "$$fixture" ]; then \
			test_name=$$(basename "$$fixture" .shx); \
			expected_file="$(EXPECTED_DIR)/$$test_name.sh"; \
			actual_file="$(TEMP_DIR)/$$test_name.sh"; \
			total=$$((total + 1)); \
			echo -n "🔍 Testing $$test_name... "; \
			if [ ! -f "$$expected_file" ]; then \
				printf "$(RED)❌ Expected file not found$(NC)\n"; \
				failed=$$((failed + 1)); \
				continue; \
			fi; \
			if ! dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$fixture" > /dev/null 2>&1; then \
				printf "$(RED)❌ Compilation failed$(NC)\n"; \
				failed=$$((failed + 1)); \
				continue; \
			fi; \
			generated_file="$(FIXTURES_DIR)/$$test_name.sh"; \
			mv "$$generated_file" "$$actual_file"; \
			if diff -u "$$expected_file" "$$actual_file" > /dev/null; then \
				printf "$(GREEN)✅ PASS$(NC)\n"; \
				passed=$$((passed + 1)); \
			else \
				printf "$(RED)❌ FAIL$(NC)\n"; \
				printf "$(YELLOW)Expected vs Actual differences:$(NC)\n"; \
				diff -u "$$expected_file" "$$actual_file" | head -20; \
				echo; \
				failed=$$((failed + 1)); \
			fi; \
		fi; \
	done; \
	echo; \
	echo "$(BLUE)Running negative tests (should fail compilation)...$(NC)"; \
	for fixture in $(NEGATIVE_TESTS_DIR)/*.shx; do \
		if [ -f "$$fixture" ]; then \
			test_name=$$(basename "$$fixture" .shx); \
			total=$$((total + 1)); \
			echo -n "🔍 Testing $$test_name (expect failure)... "; \
			if dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$fixture" > /dev/null 2>&1; then \
				printf "$(RED)❌ FAIL (should have failed compilation)$(NC)\n"; \
				failed=$$((failed + 1)); \
			else \
				printf "$(GREEN)✅ PASS (failed as expected)$(NC)\n"; \
				passed=$$((passed + 1)); \
			fi; \
		fi; \
	done; \
	echo; \
	echo "📊 Test Results"; \
	echo "==============="; \
	echo "Total tests: $$total"; \
	printf "$(GREEN)Passed: $$passed$(NC)\n"; \
	if [ $$failed -gt 0 ]; then \
		printf "$(RED)Failed: $$failed$(NC)\n"; \
	else \
		echo "Failed: $$failed"; \
	fi; \
	rm -rf $(TEMP_DIR); \
	if [ $$failed -eq 0 ]; then \
		printf "\n$(GREEN)🎉 All tests passed!$(NC)\n"; \
		exit 0; \
	else \
		printf "\n$(RED)💥 Some tests failed!$(NC)\n"; \
		exit 1; \
	fi

compile: build ## Compile a .shx file (usage: make compile FILE=path/to/file.shx)
ifndef FILE
	@echo "$(YELLOW)Usage: make compile FILE=<file.shx>$(NC)"
	@echo "Example: make compile FILE=examples/hello.shx"
	@exit 1
endif
	@echo "$(BLUE)📝 Compiling $(FILE)...$(NC)"
	@cd $(CLI_DIR) && dotnet run -- compile "../../$(FILE)"

clean: ## Clean build artifacts and test files
	@echo "$(BLUE)🧹 Cleaning build artifacts...$(NC)"
	@cd $(CLI_DIR) && dotnet clean
	@rm -rf $(TESTS_DIR)/temp
	@rm -f $(TESTS_DIR)/positive_fixtures/*.sh
	@rm -f $(TESTS_DIR)/negative_fixtures/*.sh
	@rm -rf $(VSCODE_DIR)/server
	@rm -rf $(VSCODE_DIR)/dist
	@echo "$(GREEN)✅ Clean complete$(NC)"

# Development workflow targets
build-extension: build ## Build both CLI and VS Code extension
	@echo "$(BLUE)🔨 Building VS Code extension...$(NC)"
	@cd $(VSCODE_DIR) && npm run compile
	@echo "$(BLUE)📦 Copying CLI output to VS Code extension dist/server...$(NC)"
	@mkdir -p $(VSCODE_DIR)/dist/server
	@cp $(CLI_DIR)/bin/Debug/net9.0/utah $(VSCODE_DIR)/dist/server/
	@cp $(CLI_DIR)/bin/Debug/net9.0/utah.dll $(VSCODE_DIR)/dist/server/
	@cp $(CLI_DIR)/bin/Debug/net9.0/utah.runtimeconfig.json $(VSCODE_DIR)/dist/server/
	@cp $(CLI_DIR)/bin/Debug/net9.0/utah.deps.json $(VSCODE_DIR)/dist/server/
	@cp $(CLI_DIR)/bin/Debug/net9.0/*.dll $(VSCODE_DIR)/dist/server/ 2>/dev/null || true
	@chmod +x $(VSCODE_DIR)/dist/server/utah
	@echo "$(GREEN)✅ Extension build complete$(NC)"

dev: build test ## Full development cycle: build + test

watch: ## Watch for changes and run tests (requires inotify-tools)
	@echo "$(BLUE)👀 Watching for changes...$(NC)"
	@while inotifywait -r -e modify,create,delete $(CLI_DIR) 2>/dev/null; do \
		echo "$(YELLOW)🔄 Changes detected, running tests...$(NC)"; \
		make test || true; \
		echo "$(BLUE)👀 Watching for changes...$(NC)"; \
	done

install: build ## Install Utah CLI globally (requires sudo)
	@echo "$(BLUE)📦 Installing Utah CLI globally...$(NC)"
	@cd $(CLI_DIR) && dotnet publish -c Release -o /usr/local/bin/utah
	@echo "$(GREEN)✅ Utah CLI installed to /usr/local/bin/utah$(NC)"

# Information targets
info: ## Show project information
	@echo "Utah Language Development Environment"
	@echo "===================================="
	@echo "CLI Directory: $(CLI_DIR)"
	@echo "Tests Directory: $(TESTS_DIR)"
	@echo "Positive Fixtures: $$(ls $(TESTS_DIR)/positive_fixtures/*.shx 2>/dev/null | wc -l)"
	@echo "Negative Fixtures: $$(ls $(TESTS_DIR)/negative_fixtures/*.shx 2>/dev/null | wc -l)"
	@echo "Expected Outputs: $$(ls $(TESTS_DIR)/expected/*.sh 2>/dev/null | wc -l)"
	@echo
	@echo ".NET Version:"
	@dotnet --version 2>/dev/null || echo "  .NET not found"
	@echo
	@echo "Recent Changes:"
	@git log --oneline -5 2>/dev/null || echo "  Not a git repository"
