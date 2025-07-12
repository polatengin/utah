#!/bin/bash

__UTAH_ARG_NAMES=()
__UTAH_ARG_SHORT_NAMES=()
__UTAH_ARG_DESCRIPTIONS=()
__UTAH_ARG_TYPES=()
__UTAH_ARG_REQUIRED=()
__UTAH_ARG_DEFAULTS=()

__utah_has_arg() {
  local flag="$1"
  shift
  for arg in "$@"; do
    case "$arg" in
      "$flag"|"$flag"=*)
        return 0
        ;;
    esac
  done
  for i in "${!__UTAH_ARG_NAMES[@]}"; do
    if [ "${__UTAH_ARG_NAMES[$i]}" = "$flag" ]; then
      local short="${__UTAH_ARG_SHORT_NAMES[$i]}"
      if [ -n "$short" ]; then
        for arg in "$@"; do
          case "$arg" in
            "$short"|"$short"=*)
              return 0
              ;;
          esac
        done
      fi
    fi
  done
  return 1
}

__utah_get_arg() {
  local flag="$1"
  shift
  local next_is_value=false
  local short_flag=""

  for i in "${!__UTAH_ARG_NAMES[@]}"; do
    if [ "${__UTAH_ARG_NAMES[$i]}" = "$flag" ]; then
      short_flag="${__UTAH_ARG_SHORT_NAMES[$i]}"
      break
    fi
  done

  for arg in "$@"; do
    if [ "$next_is_value" = true ]; then
      echo "$arg"
      return 0
    fi
    case "$arg" in
      "$flag"=*)
        echo "${arg#*=}"
        return 0
        ;;
      "$flag")
        next_is_value=true
        ;;
      "$short_flag"=*)
        if [ -n "$short_flag" ]; then
          echo "${arg#*=}"
          return 0
        fi
        ;;
      "$short_flag")
        if [ -n "$short_flag" ]; then
          next_is_value=true
        fi
        ;;
    esac
  done

  for i in "${!__UTAH_ARG_NAMES[@]}"; do
    if [ "${__UTAH_ARG_NAMES[$i]}" = "$flag" ]; then
      echo "${__UTAH_ARG_DEFAULTS[$i]}"
      return 0
    fi
  done

  return 1
}

__utah_all_args() {
  echo "$*"
}

__utah_show_help() {
  echo "${__UTAH_SCRIPT_DESCRIPTION:-Script}"
  echo ""
  echo "Usage: $0 [OPTIONS]"
  echo ""
  echo "Options:"
  for i in "${!__UTAH_ARG_NAMES[@]}"; do
    local flag="${__UTAH_ARG_NAMES[$i]}"
    local short="${__UTAH_ARG_SHORT_NAMES[$i]}"
    local desc="${__UTAH_ARG_DESCRIPTIONS[$i]}"
    local type="${__UTAH_ARG_TYPES[$i]}"
    local default="${__UTAH_ARG_DEFAULTS[$i]}"

    if [ -n "$short" ]; then
      printf "  %s, %-20s %s" "$short" "$flag" "$desc"
    else
      printf "  %-24s %s" "$flag" "$desc"
    fi

    if [ "$type" != "boolean" ] && [ -n "$default" ]; then
      printf " (default: %s)" "$default"
    fi
    echo
  done
  exit 0
}

__UTAH_SCRIPT_DESCRIPTION="User Management Tool - Creates and manages user accounts"
__UTAH_ARG_NAMES+=("--version")
__UTAH_ARG_SHORT_NAMES+=("-v")
__UTAH_ARG_DESCRIPTIONS+=("Show the application version")
__UTAH_ARG_TYPES+=("string")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("")

__UTAH_ARG_NAMES+=("--help")
__UTAH_ARG_SHORT_NAMES+=("-h")
__UTAH_ARG_DESCRIPTIONS+=("Show this help message")
__UTAH_ARG_TYPES+=("string")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("")

__UTAH_ARG_NAMES+=("--name")
__UTAH_ARG_SHORT_NAMES+=("-n")
__UTAH_ARG_DESCRIPTIONS+=("Specify the user's name")
__UTAH_ARG_TYPES+=("string")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("Anonymous")

__UTAH_ARG_NAMES+=("--age")
__UTAH_ARG_SHORT_NAMES+=("")
__UTAH_ARG_DESCRIPTIONS+=("Specify the user's age")
__UTAH_ARG_TYPES+=("number")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("25")

__UTAH_ARG_NAMES+=("--admin")
__UTAH_ARG_SHORT_NAMES+=("")
__UTAH_ARG_DESCRIPTIONS+=("Create user with admin privileges")
__UTAH_ARG_TYPES+=("boolean")
__UTAH_ARG_REQUIRED+=("false")
__UTAH_ARG_DEFAULTS+=("false")

if [ "$(__utah_has_arg "--help" "$@" && echo "true" || echo "false")" = "true" ]; then
  __utah_show_help "$@"
  exit 0
fi
if [ "$(__utah_has_arg "--version" "$@" && echo "true" || echo "false")" = "true" ]; then
  echo "User Management Tool v1.0.0"
  exit 0
fi
userName=$(__utah_get_arg "--name" "$@")
userAge=$(__utah_get_arg "--age" "$@")
isAdmin=$(__utah_has_arg "--admin" "$@" && echo "true" || echo "false")
echo "Creating user: ${userName}, age: ${userAge}, admin: ${isAdmin}"
echo "All arguments: $(__utah_all_args "$@")"

for i in "${!__UTAH_ARG_NAMES[@]}"; do
  flag="${__UTAH_ARG_NAMES[$i]}"
  short="${__UTAH_ARG_SHORT_NAMES[$i]}"
  desc="${__UTAH_ARG_DESCRIPTIONS[$i]}"
  type="${__UTAH_ARG_TYPES[$i]}"
  required="${__UTAH_ARG_REQUIRED[$i]}"
  default="${__UTAH_ARG_DEFAULTS[$i]}"
done
