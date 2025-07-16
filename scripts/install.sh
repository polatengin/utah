#!/usr/bin/env sh

# Utah Installation Script
# Usage:
#   ./install-utah.sh                    # Install latest version
#   ./install-utah.sh v1.2.3             # Install specific version
#   curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | bash
#   curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | bash -s v1.2.3
# Requirements:
#   - curl or wget (for downloading)
#   - sudo access (for installing to /usr/local/bin/)

RELEASES_URI="https://api.github.com/repos/polatengin/utah/releases/latest"

# these values will be populated below
release_info=""
binary_arch=""
binary_os=""
version=""

main() {
  # Handle version parameter
  target_version="${1:-latest}"
  
  # Check if utah already exists
  if [ -f "/usr/local/bin/utah" ]; then
    echo "⚠️  Utah binary already exists: /usr/local/bin/utah"
    echo "❌ Installation cancelled. Please remove the existing binary first! (sudo rm /usr/local/bin/utah)"
    return 1
  fi

  echo "🎯 Target version: $target_version"

  detect_system
  set_version "$target_version"
  download_utah
  install_utah
}

detect_system(){
  echo "🔍 Detecting system..."
  arch=$(uname -m)
  os=$(uname -s | tr '[:upper:]' '[:lower:]')
  echo "📋 System: $os ($arch)"
  
  # Map OS to binary naming convention
  case "$os" in
    linux) binary_os="linux" ;;
    darwin) binary_os="osx" ;;
    *) echo "❌ Error: Unsupported operating system: $os"; return 1 ;;
  esac
  
  # Map architecture to binary naming convention
  case "$arch" in
    x86_64|amd64) binary_arch="x64" ;;
    arm64|aarch64) binary_arch="arm64" ;;
    *) echo "❌ Error: Unsupported architecture: $arch"; return 1 ;;
  esac
}

set_version(){
  local target_version="$1"
  
  if [ "$target_version" = "latest" ]; then
    echo "🌐 Fetching latest release..."
    release_info=$(download_content "$RELEASES_URI") || return 1
    version=$(echo "$release_info" | grep '"tag_name"' | head -n1 | cut -d'"' -f4)
    
    if [ -z "$version" ]; then
      echo "❌ Error: Could not fetch latest version"
      return 1
    fi
    echo "📦 Latest version: $version"
  else
    version="$target_version"
    echo "📦 Using version: $version"
  fi
}

download_content(){
  local url="$1"
  if command -v curl >/dev/null 2>&1; then
    curl -s "$url"
  elif command -v wget >/dev/null 2>&1; then
    wget -qO- "$url"
  else
    echo "❌ Error: Neither curl nor wget is available" >&2
    return 1
  fi
}

download_utah() {
  binary_name="utah-${binary_os}-${binary_arch}"
  download_url="https://github.com/polatengin/utah/releases/download/${version}/${binary_name}"

  echo "⬇️  Downloading $binary_name ($version)..."

  if command -v curl >/dev/null 2>&1; then
    curl -L -o "utah" "$download_url" || { echo "❌ Download failed"; return 1; }
  elif command -v wget >/dev/null 2>&1; then
    wget -O "utah" "$download_url" || { echo "❌ Download failed"; return 1; }
  else
    echo "❌ Error: Neither curl nor wget is available"
    return 1
  fi

  chmod +x "utah"
  file_size=$(ls -lh "utah" | awk '{print $5}')
  echo "✅ Downloaded $file_size binary"
}

install_utah() {
  echo "📦 Installing to /usr/local/bin..."
  if sudo mv "utah" "/usr/local/bin/"; then
    echo "✅ Utah $version installed successfully!"
    echo "💡 You can now run: utah --help"
  else
    echo "❌ Error: Failed to install binary"
    return 1
  fi
}

# entry point
main "$@"
