# Utah Language Development Makefile

.PHONY: help build test compile clean
.DEFAULT_GOAL := help

CLI_DIR := src/cli
TESTS_DIR := tests

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
	@echo "$(BLUE)🧪 Running tests...$(NC)"
	@./$(TESTS_DIR)/run_tests.sh

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
	@rm -f $(TESTS_DIR)/fixtures/*.sh
	@echo "$(GREEN)✅ Clean complete$(NC)"

# Development workflow targets
dev: build test ## Full development cycle: build + test

watch: ## Watch for changes and run tests (requires inotify-tools)
	@echo "$(BLUE)👀 Watching for changes...$(NC)"
	@while inotifywait -r -e modify,create,delete $(CLI_DIR) 2>/dev/null; do \
		echo "$(YELLOW)🔄 Changes detected, running tests...$(NC)"; \
		make test || true; \
		echo "$(BLUE)👀 Watching for changes...$(NC)"; \
	done

# Release targets
check: build test ## Full check: build + all tests (for CI/CD)
	@echo "$(GREEN)🎉 All checks passed!$(NC)"

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
	@echo "Test Fixtures: $$(ls $(TESTS_DIR)/fixtures/*.shx 2>/dev/null | wc -l)"
	@echo "Expected Outputs: $$(ls $(TESTS_DIR)/expected/*.sh 2>/dev/null | wc -l)"
	@echo
	@echo ".NET Version:"
	@dotnet --version 2>/dev/null || echo "  .NET not found"
	@echo
	@echo "Recent Changes:"
	@git log --oneline -5 2>/dev/null || echo "  Not a git repository"
