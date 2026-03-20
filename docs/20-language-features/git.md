---
layout: default
title: Git Functions
parent: Language Features
nav_order: 6
---

Utah provides Git integration functions for version control operations and repository management.

## Available Functions

| Function | Returns | Description |
|---|---|---|
| `git.undoLastCommit()` | void | Undo the last commit, preserving changes in staging area |
| `git.currentBranch()` | string | Get the name of the current git branch |
| `git.isClean()` | boolean | Check if the working directory has no uncommitted changes |
| `git.resetToCommit(hash)` | void | Hard reset to a specific commit hash |
| `git.status()` | string | Get the short status of the working directory |

## git.undoLastCommit()

Performs a soft reset to undo the last commit while preserving all changes in the staging area. This is the safest undo operation — no work is lost.

```typescript
// Undo the last commit
git.undoLastCommit();
console.log("Last commit undone, changes preserved in staging area");

// Use in error recovery
let commitNeedsFixing: boolean = true;
if (commitNeedsFixing) {
  git.undoLastCommit();
  console.log("Ready to make corrected commit");
}

// Use in a function for reusable rollback
function emergencyRollback(): void {
  console.log("Rolling back last commit...");
  git.undoLastCommit();
  console.log("Done. Review changes before recommitting.");
}
```

**Generated Bash:**

```bash
# git.undoLastCommit() becomes:
git reset --soft HEAD~1
```

**Reset type comparison:**

| Reset Type | Commit History | Staging Area | Working Directory |
|---|---|---|---|
| **Soft** (Utah) | ✅ Undone | ✅ Preserved | ✅ Preserved |
| Mixed | ✅ Undone | ❌ Cleared | ✅ Preserved |
| Hard | ✅ Undone | ❌ Cleared | ❌ Cleared |

**Test Coverage:**

- File: `tests/positive_fixtures/git_undo_last_commit.shx`
- Tests standalone statement usage in a git workflow

## git.currentBranch()

Returns the name of the currently checked-out git branch as a string.

```typescript
// Get and display the current branch
let branch: string = git.currentBranch();
console.log("Currently on branch: ${branch}");

// Use in conditional logic
let branch: string = git.currentBranch();
if (branch == "main") {
  console.log("On production branch - be careful!");
}

// Use in deployment scripts
let currentBranch: string = git.currentBranch();
console.log("Deploying from branch: ${currentBranch}");
```

**Generated Bash:**

```bash
# git.currentBranch() becomes:
branch=$(git rev-parse --abbrev-ref HEAD)
```

**Test Coverage:**

- File: `tests/positive_fixtures/git_current_branch.shx`
- Tests variable assignment with branch name

## git.isClean()

Returns a boolean indicating whether the working directory has no uncommitted changes. Returns `true` if clean (no modifications, no untracked files), `false` otherwise.

```typescript
// Check if working directory is clean
let clean: boolean = git.isClean();
if (clean) {
  console.log("Working directory is clean");
} else {
  console.log("Working directory has uncommitted changes");
}

// Guard a deployment
let isClean: boolean = git.isClean();
if (!isClean) {
  console.log("Error: commit or stash your changes before deploying");
  exit(1);
}
console.log("Working directory clean - proceeding with deploy");

// Combine with branch check
let branch: string = git.currentBranch();
let clean: boolean = git.isClean();
if (branch == "main" && clean) {
  console.log("Ready to release from main");
}
```

**Generated Bash:**

```bash
# git.isClean() becomes:
clean=$([ -z "$(git status --porcelain)" ] && echo "true" || echo "false")

# In an if condition:
if [ "$([ -z "$(git status --porcelain)" ] && echo "true" || echo "false")" = "true" ]; then
  echo "Working directory is clean"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/git_is_clean.shx`
- Tests boolean result in variable assignment and if-condition usage

## git.resetToCommit(hash)

Performs a hard reset to a specific commit hash. **This is a destructive operation** — all uncommitted changes and commits after the target will be lost.

```typescript
// Reset to a specific commit hash
git.resetToCommit("abc123def");

// Reset using a variable
let targetCommit: string = "a1b2c3d";
git.resetToCommit(targetCommit);

// Reset with confirmation
let shouldReset: boolean = console.promptYesNo("Reset to previous release?");
if (shouldReset) {
  git.resetToCommit("v2.0.0");
  console.log("Reset complete");
}
```

**Generated Bash:**

```bash
# git.resetToCommit("abc123") with a string literal becomes:
git reset --hard "abc123"

# git.resetToCommit(commitHash) with a variable becomes:
git reset --hard ${commitHash}
```

:::warning
`git.resetToCommit()` uses `git reset --hard`, which **permanently discards** all uncommitted changes and all commits after the target. Use `git.undoLastCommit()` for a safer alternative when you only need to undo the most recent commit.
:::

**Test Coverage:**

- File: `tests/positive_fixtures/git_reset_to_commit.shx`
- Tests both string literal and variable argument usage
- Negative test: `tests/negative_fixtures/git_wrong_args.shx` — verifies that calling without arguments fails

## git.status()

Returns the short-format status of the working directory as a string.

```typescript
let status: string = git.status();
console.log("Repository status: ${status}");
```

**Generated Bash:**

```bash
# git.status() becomes:
status=$(git status --short)
```

## Practical Examples

### Pre-deploy Safety Check

```typescript
let branch: string = git.currentBranch();
let clean: boolean = git.isClean();

if (branch != "main") {
  console.log("Error: deployments must be from main branch");
  exit(1);
}

if (!clean) {
  console.log("Error: working directory is not clean");
  exit(1);
}

console.log("All checks passed - deploying...");
```

### Undo, Fix, and Recommit Workflow

```typescript
console.log("Starting commit correction workflow...");
git.undoLastCommit();
console.log("Last commit undone - changes are staged");
console.log("After making corrections, commit again");
```

### Safe Reset with Confirmation

```typescript
let shouldReset: boolean = console.promptYesNo("Hard reset to this commit?");
if (shouldReset) {
  git.resetToCommit("abc123");
  console.log("Reset complete");
} else {
  console.log("Reset cancelled");
}
```

## Use Cases

- **Version control automation** — scripted git workflows
- **CI/CD pipeline integration** — branch checks, clean-state validation
- **Release management** — tag-based resets, branch verification
- **Repository maintenance** — automated commit corrections
- **Deployment guards** — ensuring clean state before deploy
