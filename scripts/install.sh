#!/usr/bin/env sh

# Utah Installation Script
# Usage:
#   ./install-utah.sh
# Requirements:
#   - curl or wget (for downloading)
#   - sudo access (for installing to /usr/local/bin/)

RELEASES_URI="https://api.github.com/repos/polatengin/utah/releases/latest"

# these values will be populated below
release_info=""
binary_arch=""
version=""

main() {
  # Check if utah already exists in /usr/local/bin/
  if [ -f "/usr/local/bin/utah" ]; then
    echo "⚠️  Utah binary already exists: /usr/local/bin/utah"
    echo "❌ Installation cancelled. Please remove the existing binary first! (sudo rm /usr/local/bin/utah)"
    return 1
  fi

  detect_system
  map_architecture
  map_os
  fetch_latest_version
  download_latest_utah
  move_to_usr_local_bin
}

detect_system(){
    echo "🔍 Detecting system architecture..."
    arch=$(uname -m)
    os=$(uname -s | tr '[:upper:]' '[:lower:]')
    echo "📋 System info: OS=$os, Architecture=$arch"
}

map_os(){
  # Map OS to binary naming convention
  case "$os" in
      linux)
          binary_os="linux"
          ;;
      darwin)
          binary_os="osx"
          ;;
      *)
          echo "❌ Error: Unsupported operating system: $os"
          return 1
          ;;
  esac
}

map_architecture(){
  # Map system architecture to binary naming convention
  case "$arch" in
      x86_64|amd64)
          binary_arch="x64"
          ;;
      arm64|aarch64)
          binary_arch="arm64"
          ;;
      *)
          echo "❌ Error: Unsupported architecture: $arch"
          return 1
          ;;
  esac
}

fetch_latest_version(){
  echo "🌐 Fetching latest release information..."

  if command -v curl >/dev/null 2>&1; then
      release_info=$(curl -s "$RELEASES_URI")
  elif command -v wget >/dev/null 2>&1; then
      release_info=$(wget -qO- "$RELEASES_URI")
  else
      echo "❌ Error: Neither curl nor wget is available"
      return 1
  fi
  version=$(echo "$release_info" | grep '"tag_name"' | head -n1 | cut -d'"' -f4)
}

download_latest_utah() {
    if [ -z "$version" ]; then
        echo "❌ Error: Could not fetch latest version information"
        return 1
    fi

    echo "📦 Latest version: $version"

    binary_name="utah-${binary_os}-${binary_arch}"
    download_url="https://github.com/polatengin/utah/releases/download/${version}/${binary_name}"

    echo "⬇️  Downloading: $binary_name"
    echo "🔗 URL: $download_url"

    if command -v curl >/dev/null 2>&1; then
        if curl -L -o "utah" "$download_url"; then
            echo "✅ Download completed successfully"
        else
            echo "❌ Error: Download failed"
            return 1
        fi
    elif command -v wget >/dev/null 2>&1; then
        if wget -O "utah" "$download_url"; then
            echo "✅ Download completed successfully"
        else
            echo "❌ Error: Download failed"
            return 1
        fi
    fi

    # Make the binary executable
    chmod +x "utah"
    echo "🔧 Made binary executable"

    # Verify the download
    if [ -f "utah" ]; then
        file_size=$(ls -lh "utah" | awk '{print $5}')
        echo "📁 Downloaded file size: $file_size"
        echo "🎉 Utah binary successfully downloaded as 'utah'"
        echo "💡 You can now run: ./utah"
    else
        echo "❌ Error: Binary file not found after download"
        return 1
    fi
}

move_to_usr_local_bin() {
    echo "📦 Moving binary to /usr/local/bin..."

    if sudo mv "utah" "/usr/local/bin/"; then
        echo "✅ Successfully moved to /usr/local/bin/"
    else
        echo "❌ Error: Failed to move binary to /usr/local/bin/"
        return 1
    fi
}

# entry point
main "$@"
