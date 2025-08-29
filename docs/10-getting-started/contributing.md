---
layout: default
title: Contributing to Utah
parent: Getting Started
nav_order: 5
---

Thank you for your interest in contributing to Utah! This guide will help you get started with contributing to the project.

## Getting Started

### Development Environment

1. **Prerequisites**
   - .NET 9 SDK
   - Node.js 18+
   - Git

2. **Clone the Repository**

   ```bash
   git clone https://github.com/polatengin/utah.git
   cd utah
   ```

3. **Build the Project**

   ```bash
   make build
   ```

4. **Run Tests**

   ```bash
   make test
   ```

## Project Structure

Utah is organized into several main components:

- **src/cli/** - Main CLI transpiler written in C#
- **src/vscode-extension/** - VS Code extension for Utah language support
- **src/mcp-server/** - Model Context Protocol server for AI assistant integration
- **tests/** - Test suite with positive and negative test cases
- **docs/** - Documentation website built with Docusaurus

## Development Workflow

### 1. Create a Feature Branch

```bash
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes

Follow the established patterns in the codebase:

- **CLI Changes**: Modify files in `src/cli/`
- **Language Features**: Add AST nodes, parser logic, and compiler output
- **Tests**: Add or update tests in the `tests/` directory
- **VS Code Extension**: Update files in `src/vscode-extension/`
- **MCP Server**: Update Model Context Protocol server in `src/mcp-server/`
- **Documentation**: Update relevant `.md` files

### 3. Add Tests

For new language features:

1. Create a test file in `tests/positive_fixtures/your_feature.shx`
2. Add expected output in `tests/expected/your_feature.sh`
3. Run tests with `make test FILE=your_feature`

### 4. Update Documentation

- Update [README.md](https://github.com/polatengin/utah/blob/main/README.md) if needed
- Update documentation in `docs/` folder

### 5. Submit a Pull Request

1. Push your branch to your fork
2. Create a pull request against the main repository
3. Include a clear description of your changes
4. Reference any related issues

## Code Style Guidelines

### C# Code (CLI)

- Use C# 12 features and patterns
- Follow AST-first design principles
- Use records for AST nodes
- Maintain separation between Parser, AST, and Compiler

Example AST node:

```csharp
public record NewFeatureExpression(string Parameter) : Expression;
```

### TypeScript Code (VS Code Extension)

- Use TypeScript strict mode
- Follow existing code patterns
- Use proper type annotations
- Test in VS Code environment

### Documentation

- Use clear, concise language
- Include code examples
- Follow existing documentation structure
- Test all examples

## Adding New Built-in Functions

Utah built-in functions follow a specific pattern:

1. **Add AST Node**

   ```csharp
   public record NewFunctionExpression(params) : Expression;
   ```

2. **Add Parser Logic**

   ```csharp
   // In Parser.Expressions.cs
   if (line.StartsWith("namespace.functionName("))
   {
       // Parse the function call
       return new NewFunctionExpression(params);
   }
   ```

3. **Add Compiler Output**

   ```csharp
   // In Compiler.Expressions.cs
   public string Visit(NewFunctionExpression expr)
   {
       return "generated_bash_code";
   }
   ```

4. **Add Test Cases**

   ```typescript
   // In tests/positive_fixtures/new_function.shx
   const result = namespace.functionName(param)
   console.log(result)
   ```

## Testing

### Running Tests

```bash
# Run all tests
make test

# Run specific test
make test FILE=your_test_name
```

### Test Types

- **Positive Tests**: Valid Utah code that should compile successfully
- **Negative Tests**: Invalid code that should fail compilation
- **Format Tests**: Malformed code that should be formatted correctly
- **Command Tests**: Tests for CLI commands and their output

### Adding New Tests

1. Create `.shx` file in appropriate test directory
2. Add expected output (for positive tests)
3. Run test to verify it passes
4. Include test in your pull request

## Building and Organizing Documentation

### Building Documentation

```bash
cd ./src/website
npm install
npm run build
npm run start
```

### Documentation Structure

- **Getting Started**: Installation and basic usage
- **Language Features**: Core language concepts
- **CLI Reference**: Command-line interface documentation
- **Guides**: Advanced usage patterns
- **Examples**: Code examples and use cases

## Release Process

Utah follows semantic versioning:

- **Major**: Breaking changes to language syntax or CLI
- **Minor**: New features, new built-in functions
- **Patch**: Bug fixes, documentation improvements

## Community

### Getting Help

- **GitHub Issues**: Report bugs and request features
- **GitHub Discussions**: Ask questions and share ideas

### Code of Conduct

We are committed to providing a welcoming and inclusive environment for all contributors. Please read and follow our Code of Conduct.

## Common Contribution Areas

### High Priority

- New built-in functions
- Language feature improvements
- Performance optimizations
- Documentation improvements

### Good First Issues

- Adding examples
- Fixing typos in documentation
- Adding test cases
- Improving error messages

### Advanced Contributions

- Parser improvements
- Compiler optimizations
- VS Code extension features
- CI/CD improvements

## Questions?

If you have questions about contributing:

1. Check existing GitHub issues and discussions
2. Create a new issue with the "question" label

Thank you for contributing to Utah! 🚀
