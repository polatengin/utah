# Utah Language Development Makefile

.PHONY: help build build-extension build-debian build-debian-changelog test compile clean install info dev markdownlint format
.DEFAULT_GOAL := help

CLI_DIR := src/cli
VSCODE_DIR := src/vscode-extension
TESTS_DIR := tests
FIXTURES_DIR := $(TESTS_DIR)/positive_fixtures
NEGATIVE_TESTS_DIR := $(TESTS_DIR)/negative_fixtures
MALFORMED_DIR := $(TESTS_DIR)/malformed
EXPECTED_DIR := $(TESTS_DIR)/expected
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
	@echo "  make test FILE=env_get_ternary"
	@echo "  make compile FILE=examples/hello.shx"
	@echo "  make format"

clean: ## Clean build artifacts and test files
	@echo "$(BLUE)🧹 Cleaning build artifacts...$(NC)"
	@cd $(CLI_DIR) && dotnet clean
	@rm -rf $(TESTS_DIR)/temp
	@rm -f $(TESTS_DIR)/positive_fixtures/*.sh
	@rm -f $(TESTS_DIR)/negative_fixtures/*.sh
	@rm -rf $(VSCODE_DIR)/server
	@rm -rf $(VSCODE_DIR)/dist
	@echo "$(GREEN)✅ Clean complete$(NC)"

build: clean ## Build the CLI
	@echo "$(BLUE)🔨 Building Utah CLI...$(NC)"
	@cd $(CLI_DIR) && dotnet build
	@echo "$(GREEN)✅ Build complete$(NC)"

test: build ## Run all regression tests (or specific test with FILE=testname)
	@echo "$(BLUE)🧪 Running Utah CLI Regression Tests$(NC)"
	@echo "====================================="
	@mkdir -p $(TEMP_DIR)
	@total=0; passed=0; failed=0; \
	if [ -n "$(FILE)" ]; then \
		echo "$(BLUE)Running specific test: $(FILE)$(NC)"; \
		fixture="$(FIXTURES_DIR)/$(FILE).shx"; \
		if [ ! -f "$$fixture" ]; then \
			fixture="$(NEGATIVE_TESTS_DIR)/$(FILE).shx"; \
			if [ ! -f "$$fixture" ]; then \
				echo "$(RED)❌ Test file $(FILE).shx not found in positive_fixtures or negative_fixtures$(NC)"; \
				exit 1; \
			fi; \
			echo "$(BLUE)Running negative test (should fail compilation)...$(NC)"; \
			test_name="$(FILE)"; \
			total=1; \
			echo -n "🔍 Testing $$test_name (expect failure)... "; \
			if dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$fixture" > /dev/null 2>&1; then \
				printf "$(RED)❌ FAIL (should have failed compilation)$(NC)\n"; \
				failed=1; \
			else \
				printf "$(GREEN)✅ PASS (failed as expected)$(NC)\n"; \
				passed=1; \
			fi; \
		else \
			echo "$(BLUE)Running positive test...$(NC)"; \
			test_name="$(FILE)"; \
			expected_file="$(EXPECTED_DIR)/$$test_name.sh"; \
			actual_file="$(TEMP_DIR)/$$test_name.sh"; \
			total=1; \
			echo -n "🔍 Testing $$test_name... "; \
			if [ ! -f "$$expected_file" ]; then \
				printf "$(RED)❌ Expected file not found$(NC)\n"; \
				failed=1; \
			elif ! dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$fixture" > /dev/null 2>&1; then \
				printf "$(RED)❌ Compilation failed$(NC)\n"; \
				failed=1; \
			else \
				generated_file="$(FIXTURES_DIR)/$$test_name.sh"; \
				mv "$$generated_file" "$$actual_file"; \
				if diff -u "$$expected_file" "$$actual_file" > /dev/null; then \
					printf "$(GREEN)✅ PASS$(NC)\n"; \
					passed=1; \
				else \
					printf "$(RED)❌ FAIL$(NC)\n"; \
					printf "$(YELLOW)Expected vs Actual differences:$(NC)\n"; \
					diff -u "$$expected_file" "$$actual_file"; \
					echo; \
					failed=1; \
				fi; \
			fi; \
		fi; \
	else \
		MAX_JOBS=$$(nproc 2>/dev/null || echo "4"); \
		MAX_JOBS=$$([ $$MAX_JOBS -gt 16 ] && echo 16 || echo $$MAX_JOBS); \
		echo "$(BLUE)Running tests with up to $$MAX_JOBS concurrent jobs$(NC)"; \
		echo; \
		total=0; passed=0; failed=0; \
		run_test() { \
			local file="$$1" type="$$2"; \
			name=$$(basename "$$file" .shx); \
			case "$$type" in \
			positive) \
				expected="$(EXPECTED_DIR)/$$name.sh"; actual="$(TEMP_DIR)/$$name.sh"; \
				if [ -f "$$expected" ] && dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$file" >/dev/null 2>&1; then \
					mv "$(FIXTURES_DIR)/$$name.sh" "$$actual" 2>/dev/null || true; \
					if diff -u "$$expected" "$$actual" >/dev/null 2>&1; then \
						echo "PASS" > "$$actual.result"; \
					else \
						echo "FAIL" > "$$actual.result"; \
					fi; \
				else \
					echo "FAIL" > "$$actual.result"; \
				fi ;; \
			negative) \
				result="$(TEMP_DIR)/$$name.result"; \
				if dotnet run --project $(CLI_DIR) --verbosity quiet -- compile "$$file" >/dev/null 2>&1; then \
					echo "FAIL" > "$$result"; \
				else \
					echo "PASS" > "$$result"; \
				fi ;; \
			format) \
				expected="$(EXPECTED_DIR)/$$name.formatted.shx"; actual="$(TEMP_DIR)/$$name.formatted.shx"; \
				if [ -f "$$expected" ] && dotnet run --project $(CLI_DIR) --verbosity quiet -- format "$$file" -o "$$actual" >/dev/null 2>&1; then \
					if diff -u "$$expected" "$$actual" >/dev/null 2>&1; then \
						echo "PASS" > "$$actual.result"; \
					else \
						echo "FAIL" > "$$actual.result"; \
					fi; \
				else \
					echo "FAIL" > "$$actual.result"; \
				fi ;; \
			esac; \
		}; \
		run_test_group() { \
			local test_dir="$$1" test_type="$$2" group_name="$$3"; \
			local group_pids="" group_count=0 group_total=0 group_passed=0 group_failed=0; \
			echo "$(BLUE)Running $$group_name...$(NC)"; \
			for file in "$$test_dir"/*.shx; do \
				[ -f "$$file" ] || continue; \
				run_test "$$file" "$$test_type" & \
				pid=$$!; \
				group_pids="$$group_pids $$pid"; \
				group_count=$$((group_count + 1)); \
				if [ $$group_count -ge $$MAX_JOBS ]; then \
					set -- $$group_pids; wait "$$1"; \
					shift; group_pids="$$*"; group_count=$$((group_count - 1)); \
				fi; \
			done; \
			for pid in $$group_pids; do wait "$$pid" 2>/dev/null || true; done; \
			for file in "$$test_dir"/*.shx; do \
				[ -f "$$file" ] || continue; \
				name=$$(basename "$$file" .shx); \
				case "$$test_type" in \
					positive) result_file="$(TEMP_DIR)/$$name.sh.result"; label="$$name";; \
					negative) result_file="$(TEMP_DIR)/$$name.result"; label="$$name (expect failure)";; \
					format) result_file="$(TEMP_DIR)/$$name.formatted.shx.result"; label="$$name format";; \
				esac; \
				[ -f "$$result_file" ] || continue; \
				printf "🔍 Testing $$label... "; \
				if [ "$$(cat "$$result_file" 2>/dev/null)" = "PASS" ]; then \
					case "$$label" in \
						*"(expect failure)"*) echo "$(GREEN)✅ PASS (failed as expected)$(NC)"; group_passed=$$((group_passed + 1));; \
						*) echo "$(GREEN)✅ PASS$(NC)"; group_passed=$$((group_passed + 1));; \
					esac; \
				else \
					case "$$label" in \
						*"(expect failure)"*) echo "$(RED)❌ FAIL (should have failed)$(NC)"; group_failed=$$((group_failed + 1));; \
						*) echo "$(RED)❌ FAIL$(NC)"; group_failed=$$((group_failed + 1));; \
					esac; \
				fi; \
				group_total=$$((group_total + 1)); \
			done; \
			echo; \
			total=$$((total + group_total)); \
			passed=$$((passed + group_passed)); \
			failed=$$((failed + group_failed)); \
		}; \
		run_test_group "$(FIXTURES_DIR)" "positive" "positive tests"; \
		run_test_group "$(NEGATIVE_TESTS_DIR)" "negative" "negative tests (should fail compilation)"; \
		run_test_group "$(MALFORMED_DIR)" "format" "format tests"; \
		echo "$(BLUE)Testing command-based executions...$(NC)"; \
		test_command() { \
			local cmd="$$1" expected_exit="$$2" desc="$$3"; \
			total=$$((total + 1)); \
			echo -n "🔍 Testing $$desc... "; \
			if dotnet run --project $(CLI_DIR) --verbosity quiet -- --command "$$cmd" > /dev/null 2>&1; then \
				if [ "$$expected_exit" = "0" ]; then \
					printf "$(GREEN)✅ PASS$(NC)\n"; \
					passed=$$((passed + 1)); \
				else \
					printf "$(RED)❌ FAIL (should have failed)$(NC)\n"; \
					failed=$$((failed + 1)); \
				fi; \
			else \
				if [ "$$expected_exit" = "1" ]; then \
					printf "$(GREEN)✅ PASS (failed as expected)$(NC)\n"; \
					passed=$$((passed + 1)); \
				else \
					printf "$(RED)❌ FAIL (unexpected failure)$(NC)\n"; \
					failed=$$((failed + 1)); \
				fi; \
			fi; \
		}; \
		test_command '' 0 "empty command"; \
		test_command 'console.log("Hello World")' 0 "simple console.log"; \
		test_command 'let x = 5; console.log(x)' 0 "variable assignment and output"; \
		test_command 'fs.exists("/etc/passwd")' 0 "file system check"; \
		test_command 'git.status()' 0 "git status command"; \
		test_command 'os.isInstalled("bash")' 0 "OS utility check"; \
		test_command 'utility.random(1, 10)' 0 "utility function"; \
		test_command 'console.log("Multi"); console.log("Command")' 0 "multiple commands"; \
		test_command 'for (let i = 1; i <= 3; i++) { console.log(i) }' 0 "for loop"; \
		test_command 'if (true) { console.log("condition met") }' 0 "if statement"; \
		test_command 'function greet(name) { console.log("Hello " + name) } greet("World")' 0 "function definition and call"; \
		test_command 'let arr = [1, 2, 3]; console.log(arr.length)' 0 "array operations"; \
		test_command 'try { console.log("success") } catch { console.log("error") }' 0 "try-catch block"; \
		test_command 'defer console.log("cleanup"); console.log("main")' 0 "defer statement"; \
		test_command 'let x = args.get("test", "default"); console.log(x)' 0 "args functionality"; \
		test_command 'template.substitute("Hello {{name}}")' 0 "template functionality"; \
		test_command 'function processFiles() { let files = fs.find("/tmp", "*.txt"); for (let file in files) { console.log("Processing: " + file) } } processFiles()' 0 "complex file processing function"; \
		test_command 'let config = { name: "test", version: "1.0" }; if (config.name === "test") { console.log("Config valid") } else { console.log("Config invalid") }' 0 "object and conditional logic"; \
		test_command 'try { let result = git.status(); console.log("Git status: " + result) } catch { console.log("Git error: " + e) }' 0 "git operations with error handling"; \
		test_command 'let numbers = [1, 2, 3, 4, 5]; let sum = 0; for (let num in numbers) { sum += num } console.log("Sum: " + sum)' 0 "array iteration and calculation"; \
		test_command 'function validateInput(input) { if (string.length(input) === 0) { exit(1) } return true } try { validateInput("test"); console.log("Valid") } catch { console.log("Invalid") }' 0 "function with validation and error handling"; \
		test_command 'let = }' 1 "invalid syntax (should fail)"; \
		test_command 'console.nonexistentFunction()' 1 "nonexistent function (should fail)"; \
		echo; \
	fi; \
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

format: ## Format .NET source code and markdown files in the project
	@echo "$(BLUE)📝 Formatting project files...$(NC)"
	@echo "$(BLUE)Formatting .NET source code...$(NC)"
	@cd $(CLI_DIR) && dotnet format
	@echo "$(BLUE)Formatting markdown files...$(NC)"
	@find . -name "*.md" -exec sed -i -E 's/[[:space:]]+$$//' {} +
	@if command -v markdownlint >/dev/null 2>&1; then \
		markdownlint --fix --ignore-path .gitignore --ignore node_modules --ignore src/website/node_modules --ignore src/vscode-extension/node_modules "**/*.md" 2>/dev/null || true; \
	else \
		echo "$(YELLOW)⚠️  markdownlint not found, skipping markdown formatting$(NC)"; \
	fi
	@echo "$(GREEN)✅ Project formatting complete$(NC)"

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

build-debian: ## Build Debian package
	@echo "$(BLUE)📦 Building Debian package...$(NC)"
	@debuild -us -uc -b --lintian-opts --suppress-tags debian-changelog-line-too-long,extended-description-line-too-long,no-manual-page
	@echo "$(GREEN)✅ Debian package build complete$(NC)"

build-debian-changelog: ## Generate debian/changelog from git tags
	@echo "$(BLUE)📝 Generating Debian changelog...$(NC)"
	@bash -c ' \
	PACKAGE_NAME="utah"; \
	CHANGELOG_FILE="debian/changelog"; \
	DEBFULLNAME="Engin Polat"; \
	DEBEMAIL="polatengin@hotmail.com"; \
	mapfile -t tags < <(git tag --sort=-version:refname | grep "^v"); \
	> "$$CHANGELOG_FILE"; \
	for ((i=0; i<$${#tags[@]}; i++)); do \
		current_tag="$${tags[$$i]}"; \
		if [ $$i -eq $$((${#tags[@]}-1)) ]; then \
			prev_tag="$$(git rev-list --max-parents=0 HEAD | head -1)"; \
		else \
			prev_tag="$${tags[$$((i+1))]}"; \
		fi; \
		version="$${current_tag#v}"; \
		commits=$$(git log "$$prev_tag..$$current_tag" --pretty=format:"  * %s" --no-merges); \
		[[ -z "$$commits" ]] && commits="  * Initial release"; \
		date_rfc=$$(git log -1 --format=%cD "$$current_tag"); \
		{ \
			echo "$$PACKAGE_NAME ($$version) unstable; urgency=medium"; \
			echo; \
			echo "$$commits"; \
			echo; \
			echo " -- $$DEBFULLNAME <$$DEBEMAIL>  $$date_rfc"; \
			echo; \
		} >> "$$CHANGELOG_FILE"; \
	done'
	@echo "$(GREEN)✅ Changelog generated at debian/changelog$(NC)"

install: build ## Install Utah CLI globally (requires sudo)
	@echo "$(BLUE)📦 Installing Utah CLI globally...$(NC)"
	@if [ -n "$(DESTDIR)" ]; then \
		echo "$(BLUE)Building release version for packaging...$(NC)"; \
		cd $(CLI_DIR) && dotnet publish -c Release; \
		echo "$(GREEN)✅ Utah CLI built for packaging$(NC)"; \
	else \
		cd $(CLI_DIR) && dotnet publish -c Release -o /usr/local/bin/utah; \
		echo "$(GREEN)✅ Utah CLI installed to /usr/local/bin/utah$(NC)"; \
	fi

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

markdownlint: ## Run markdownlint on all markdown files
	@echo "$(BLUE)📝 Running markdownlint on markdown files...$(NC)"
	@echo "$(BLUE)Checking markdown files...$(NC)"
	@find . -name "*.md" -exec sed -i -E 's/[[:space:]]+$$//' {} +
	@if markdownlint --ignore-path .gitignore --ignore node_modules --ignore src/website/node_modules --ignore src/vscode-extension/node_modules "**/*.md"; then \
		echo "$(GREEN)✅ All markdown files passed linting$(NC)"; \
	else \
		echo "$(RED)❌ Markdown linting failed$(NC)"; \
		exit 1; \
	fi
