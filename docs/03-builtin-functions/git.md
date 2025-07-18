---
layout: default
title: Git Functions
parent: Functions
nav_order: 6
---

Utah provides comprehensive Git integration functions for version control operations and repository management.

## Repository Operations

### Basic Commands

```typescript
// Initialize repository
git.init();

// Clone repository
git.clone("https://github.com/user/repo.git");

// Add files
git.add("file.txt");
git.addAll();

// Commit changes
git.commit("Initial commit");

// Push changes
git.push();

// Pull changes
git.pull();
```

### Branch Management

```typescript
// Create branch
git.createBranch("feature/new-feature");

// Switch branch
git.checkout("feature/new-feature");

// Merge branch
git.merge("feature/new-feature");

// Delete branch
git.deleteBranch("feature/old-feature");

// List branches
let branches: string[] = git.branches();
```

### Repository Information

```typescript
// Get current branch
let branch: string = git.currentBranch();

// Get repository status
let status: string = git.status();

// Get commit history
let log: string = git.log();

// Get remote URL
let remote: string = git.remote();
```

## Advanced Operations

### Stashing

```typescript
// Stash changes
git.stash();

// Stash with message
git.stash("Work in progress");

// Apply stash
git.stashPop();

// List stashes
let stashes: string[] = git.stashList();
```

### Tagging

```typescript
// Create tag
git.tag("v1.0.0");

// Create annotated tag
git.tag("v1.0.0", "Release version 1.0.0");

// Push tags
git.pushTags();

// List tags
let tags: string[] = git.tags();
```

### Diff Operations

```typescript
// Show differences
let diff: string = git.diff();

// Show staged differences
let staged: string = git.diffStaged();

// Show differences between branches
let branchDiff: string = git.diffBranches("main", "develop");
```

## Utility Functions

### Repository Validation

```typescript
// Check if in git repository
let isRepo: boolean = git.isRepository();

// Check if working directory is clean
let isClean: boolean = git.isClean();

// Check if branch exists
let exists: boolean = git.branchExists("feature/test");
```

### Commit Operations

```typescript
// Undo last commit
git.undoLastCommit();

// Reset to specific commit
git.resetToCommit("abc123");

// Cherry-pick commit
git.cherryPick("def456");
```

## Generated Bash

Git functions compile to standard git commands:

```bash
# Initialize repository
git init

# Clone repository
git clone "https://github.com/user/repo.git"

# Add and commit
git add "file.txt"
git commit -m "Initial commit"

# Branch operations
git checkout -b "feature/new-feature"
git merge "feature/new-feature"

# Repository information
branch=$(git rev-parse --abbrev-ref HEAD)
status=$(git status --porcelain)
```

## Use Cases

- Version control automation
- CI/CD pipeline integration
- Release management
- Branch management workflows
- Repository maintenance
- Automated deployments
