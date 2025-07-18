---
layout: default
title: Installation
parent: Getting Started
nav_order: 1
---

This guide will help you install the Utah CLI tool on your system.

## Quick Installation

The easiest way to install Utah is using our one-liner installation script:

```bash
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
```

This script will:

- Download the latest release
- Install the Utah CLI to `/usr/local/bin/`
- Make it available system-wide
- Verify the installation

## Manual Installation

### Prerequisites

Utah requires:

- **Operating System**: Linux, macOS, or Windows with WSL
- **Architecture**: x64 (AMD64)
- **.NET Runtime**: Not required (Utah is self-contained)

### Download and Install

1. **Download the latest release:**

   Go to the [releases page](https://github.com/polatengin/utah/releases) and download the appropriate binary for your system.

2. **Extract and install:**

   ```bash
   # Extract the downloaded archive
   tar -xzf utah-linux-x64.tar.gz

   # Move to system path
   sudo mv utah /usr/local/bin/

   # Make executable
   sudo chmod +x /usr/local/bin/utah
   ```

3. **Verify installation:**

   ```bash
   utah --version
   ```

## Platform-Specific Instructions

### Linux (Ubuntu/Debian)

```bash
# Using the install script (recommended)
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

# Or manual installation
wget https://github.com/polatengin/utah/releases/latest/download/utah-linux-x64.tar.gz
tar -xzf utah-linux-x64.tar.gz
sudo mv utah /usr/local/bin/
sudo chmod +x /usr/local/bin/utah
```

### macOS

```bash
# Using the install script
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

# Or using Homebrew (if available)
# brew install utah  # Coming soon

# Or manual installation
curl -LO https://github.com/polatengin/utah/releases/latest/download/utah-osx-x64.tar.gz
tar -xzf utah-osx-x64.tar.gz
sudo mv utah /usr/local/bin/
sudo chmod +x /usr/local/bin/utah
```

### Windows (WSL)

Utah works best in Windows Subsystem for Linux (WSL):

```bash
# In WSL terminal
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
```

## Installing Dependencies

Utah relies on external tools for some functionality. You can install these manually or use Utah's built-in dependency management:

### Core Dependencies

- **jq**: For JSON processing
- **yq**: For YAML processing
- **curl**: For web requests
- **git**: For git utilities

### Automatic Dependency Installation

Utah provides built-in functions to install dependencies:

```typescript
// Install JSON processing dependencies
json.installDependencies();

// Install YAML processing dependencies
yaml.installDependencies();
```

### Manual Dependency Installation

**Ubuntu/Debian:**

```bash
sudo apt update
sudo apt install -y jq curl git
# For yq
sudo snap install yq
```

**macOS:**

```bash
brew install jq yq curl git
```

**CentOS/RHEL/Fedora:**

```bash
sudo dnf install -y jq curl git
# For yq
sudo snap install yq
```

## Verifying Installation

After installation, verify everything is working:

```bash
# Check Utah version
utah --version

# Check available commands
utah --help

# Test with a simple script
echo 'console.log("Hello, Utah!");' > test.shx
utah compile test.shx
./test.sh
```

Expected output:

```text
Hello, Utah!
```

## Updating Utah

To update to the latest version:

```bash
# Re-run the installation script
curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
```

The script will automatically replace the existing installation with the latest version.

## Uninstalling Utah

To remove Utah from your system:

```bash
# Remove the binary
sudo rm /usr/local/bin/utah

# Remove any generated scripts (optional)
rm -rf ~/.utah/
```

## Troubleshooting

### Permission Denied

If you get permission errors:

```bash
# Make sure Utah is executable
sudo chmod +x /usr/local/bin/utah

# Or install to user directory
mkdir -p ~/.local/bin
mv utah ~/.local/bin/
echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

### Command Not Found

If `utah` command is not found:

1. Check if it's in your PATH:

   ```bash
   echo $PATH
   ls -la /usr/local/bin/utah
   ```

2. Add to PATH if needed:

   ```bash
   echo 'export PATH="/usr/local/bin:$PATH"' >> ~/.bashrc
   source ~/.bashrc
   ```

### Dependencies Missing

If Utah functions fail due to missing dependencies:

```bash
# Install missing tools
sudo apt install -y jq  # For JSON functions
sudo snap install yq    # For YAML functions

# Or use Utah's built-in installers
utah run -c "json.installDependencies()"
utah run -c "yaml.installDependencies()"
```

## Next Steps

Once Utah is installed, you're ready to:

1. [Write your first script](first-script.md)
2. [Learn the basic syntax](syntax.md)
3. [Explore language features](../02-language-features/variables.md)

## Getting Help

- **Documentation**: [Utah Language Docs](https://polatengin.github.io/utah)
- **GitHub Issues**: [Report problems](https://github.com/polatengin/utah/issues)
- **Discussions**: [Community support](https://github.com/polatengin/utah/discussions)
