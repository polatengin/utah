# Utah Language Development Instructions

## Project Overview
Utah is a CLI transpiler that converts TypeScript-like `.shx` files into bash `.sh` scripts. The project has two main components:
- **CLI Tool** (`src/cli/`): .NET 9 C# transpiler with parser → AST → compiler architecture
- **VS Code Extension** (`src/vscode-extension/`): TypeScript extension providing syntax highlighting and language support

## Architecture Patterns

### AST-First Design
- All language features start as AST nodes in `AST.cs` using C# records
- Expression nodes inherit from `Expression : Node`, statements from `Statement : Node`
- Example: `public record GitUndoLastCommitExpression() : Expression;`

### Three-Layer Transpilation
1. **Parser** (`Parser.cs`, `Parser.Expressions.cs`, `Parser.Functions.cs`): Converts `.shx` → AST
2. **AST** (`AST.cs`): Type-safe intermediate representation
3. **Compiler** (`Compiler.cs`, `Compiler.Expressions.cs`, `Compiler.Statements.cs`): AST → bash code

### Expression vs Statement Handling
- **Expressions** return values: `${variable}`, `$(command)`, function calls
- **Statements** perform actions: assignments, control flow, function declarations
- Special case: Built-in functions like `git.undoLastCommit()` can be both expression and statement

## Key Development Workflows

### Adding New Language Features
1. Add AST node in `AST.cs` (e.g., `public record NewFeatureExpression(params) : Expression;`)
2. Add parsing logic in appropriate `Parser.*.cs` file
3. Add compilation logic in `Compiler.Expressions.cs` or `Compiler.Statements.cs`
4. Create test fixture in `tests/positive_fixtures/` (`.shx` file)
5. Generate expected output in `tests/expected/` (`.sh` file)

### Testing Commands
```bash
make test                    # Run all tests
make test FILE=feature_name  # Run specific test
make build                   # Build CLI only
make build-extension         # Build VS Code extension
```

### Function Pattern for Built-ins
Built-in functions (like `git.undoLastCommit()`, `os.isInstalled()`) follow this pattern:
- Expression class in AST.cs for return value usage
- Parser recognition in `Parser.Expressions.cs` using dot notation parsing
- Compiler logic that generates appropriate bash commands
- Special handling in `Compiler.Statements.cs` for statement-level calls

## Project-Specific Conventions

### Parser Architecture
- Uses line-by-line parsing with `_lines` array
- Comment removal in preprocessing step (preserves strings)
- Recursive descent parser with separate files by domain
- Raw bash commands detected via `IsRawBashStatement()` heuristics

### Compiler Output Style
- Always starts with `#!/bin/bash` shebang
- Uses `${}` variable expansion consistently
- Generates unique variable names to avoid conflicts (e.g., `_utah_random_min_1`)
- Function definitions use bash `function_name() { }` syntax

### Test Architecture
- **Positive tests**: `.shx` files in `positive_fixtures/` with expected `.sh` output in `expected/`
- **Negative tests**: `.shx` files in `negative_fixtures/` that should fail compilation
- Test runner compares generated output with expected files character-by-character

### Error Handling Patterns
- Parser uses early returns with descriptive error messages
- Compiler generates bash error handling (try/catch → subshells with `set -e`)
- Range validation for functions like `utility.random()` exits with code 100

## Critical Integration Points

### VS Code Extension Coordination
- Language server mode: `utah lsp` for VS Code integration
- Syntax files in `src/vscode-extension/syntaxes/` define `.shx` highlighting
- Extension uses `esbuild` + TypeScript compilation pipeline

### CLI Commands
- `utah compile file.shx` → generates `file.sh`
- `utah run file.shx` → compiles and executes
- `utah lsp` → starts language server for VS Code

### Argument Parsing Infrastructure
- Automatically injected when `args.*` functions detected in source
- Generates bash arrays and helper functions for robust argument handling
- Supports typed arguments with defaults and validation

## When Adding Built-in Functions
1. Choose appropriate namespace (`os.*`, `git.*`, `console.*`, etc.)
2. Add expression class to AST.cs
3. Update parser to recognize dot notation calls
4. Add compiler logic that generates efficient bash
5. Handle both expression and statement contexts if applicable
6. Add comprehensive test covering all parameter combinations
7. Document in README.md following established patterns

## Extension Development
Use VS Code tasks: "watch" task runs both TypeScript compilation and esbuild bundling for the extension. The extension provides syntax highlighting and will eventually support language server features.
