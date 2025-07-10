public partial class Compiler
{
  private bool _hasArgsUsage = false;

  public string Compile(ProgramNode program)
  {
    var lines = new List<string>
    {
      "#!/bin/sh",
      "" // Empty line after shebang
    };

    // Check if args are used in the program
    _hasArgsUsage = ProgramUsesArgs(program);

    // Add argument parsing infrastructure if needed
    if (_hasArgsUsage)
    {
      lines.AddRange(GenerateArgumentParsingInfrastructure());
    }

    foreach (var stmt in program.Statements)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    // Add argument parsing at the end if args are used
    if (_hasArgsUsage)
    {
      lines.AddRange(GenerateArgumentParsing());
    }

    return string.Join('\n', lines) + '\n';
  }

  private bool ProgramUsesArgs(ProgramNode program)
  {
    // Check if any statement is an args statement
    return program.Statements.Any(stmt =>
      stmt is ArgsDefineStatement ||
      stmt is ArgsShowHelpStatement ||
      ContainsArgsExpression(stmt));
  }

  private bool ContainsArgsExpression(Statement stmt)
  {
    // This is a simplified check - in a real implementation, 
    // you'd want to recursively check all expressions in the statement
    return stmt.ToString().Contains("ArgsHas") ||
           stmt.ToString().Contains("ArgsGet") ||
           stmt.ToString().Contains("ArgsAll");
  }

  private List<string> GenerateArgumentParsingInfrastructure()
  {
    return new List<string>
    {
      "# Utah argument parsing infrastructure",
      "# Initialize argument parsing arrays",
      "__UTAH_ARG_NAMES=()",
      "__UTAH_ARG_SHORT_NAMES=()",
      "__UTAH_ARG_DESCRIPTIONS=()",
      "__UTAH_ARG_TYPES=()",
      "__UTAH_ARG_REQUIRED=()",
      "__UTAH_ARG_DEFAULTS=()",
      "",
      "__utah_has_arg() {",
      "  local flag=\"$1\"",
      "  shift",
      "  for arg in \"$@\"; do",
      "    case \"$arg\" in",
      "      \"$flag\"|\"$flag\"=*)",
      "        return 0",
      "        ;;",
      "    esac",
      "  done",
      "  # Check short flags",
      "  for i in \"${!__UTAH_ARG_NAMES[@]}\"; do",
      "    if [ \"${__UTAH_ARG_NAMES[$i]}\" = \"$flag\" ]; then",
      "      local short=\"${__UTAH_ARG_SHORT_NAMES[$i]}\"",
      "      if [ -n \"$short\" ]; then",
      "        for arg in \"$@\"; do",
      "          case \"$arg\" in",
      "            \"$short\"|\"$short\"=*)",
      "              return 0",
      "              ;;",
      "          esac",
      "        done",
      "      fi",
      "    fi",
      "  done",
      "  return 1",
      "}",
      "",
      "__utah_get_arg() {",
      "  local flag=\"$1\"",
      "  shift",
      "  local next_is_value=false",
      "  local short_flag=\"\"",
      "  ",
      "  # Find the short flag for this long flag",
      "  for i in \"${!__UTAH_ARG_NAMES[@]}\"; do",
      "    if [ \"${__UTAH_ARG_NAMES[$i]}\" = \"$flag\" ]; then",
      "      short_flag=\"${__UTAH_ARG_SHORT_NAMES[$i]}\"",
      "      break",
      "    fi",
      "  done",
      "  ",
      "  for arg in \"$@\"; do",
      "    if [ \"$next_is_value\" = true ]; then",
      "      echo \"$arg\"",
      "      return 0",
      "    fi",
      "    case \"$arg\" in",
      "      \"$flag\"=*)",
      "        echo \"${arg#*=}\"",
      "        return 0",
      "        ;;",
      "      \"$flag\")",
      "        next_is_value=true",
      "        ;;",
      "      \"$short_flag\"=*)",
      "        if [ -n \"$short_flag\" ]; then",
      "          echo \"${arg#*=}\"",
      "          return 0",
      "        fi",
      "        ;;",
      "      \"$short_flag\")",
      "        if [ -n \"$short_flag\" ]; then",
      "          next_is_value=true",
      "        fi",
      "        ;;",
      "    esac",
      "  done",
      "  ",
      "  # Return default value if not found",
      "  for i in \"${!__UTAH_ARG_NAMES[@]}\"; do",
      "    if [ \"${__UTAH_ARG_NAMES[$i]}\" = \"$flag\" ]; then",
      "      echo \"${__UTAH_ARG_DEFAULTS[$i]}\"",
      "      return 0",
      "    fi",
      "  done",
      "  ",
      "  return 1",
      "}",
      "",
      "__utah_all_args() {",
      "  echo \"$*\"",
      "}",
      "",
      "__utah_show_help() {",
      "  echo \"${__UTAH_SCRIPT_DESCRIPTION:-Script}\"",
      "  echo \"\"",
      "  echo \"Usage: $0 [OPTIONS]\"",
      "  echo \"\"",
      "  echo \"Options:\"",
      "  for i in \"${!__UTAH_ARG_NAMES[@]}\"; do",
      "    local flag=\"${__UTAH_ARG_NAMES[$i]}\"",
      "    local short=\"${__UTAH_ARG_SHORT_NAMES[$i]}\"",
      "    local desc=\"${__UTAH_ARG_DESCRIPTIONS[$i]}\"",
      "    local type=\"${__UTAH_ARG_TYPES[$i]}\"",
      "    local default=\"${__UTAH_ARG_DEFAULTS[$i]}\"",
      "    ",
      "    if [ -n \"$short\" ]; then",
      "      printf \"  %s, %-20s %s\" \"$short\" \"$flag\" \"$desc\"",
      "    else",
      "      printf \"  %-24s %s\" \"$flag\" \"$desc\"",
      "    fi",
      "    ",
      "    if [ \"$type\" != \"boolean\" ] && [ -n \"$default\" ]; then",
      "      printf \" (default: %s)\" \"$default\"",
      "    fi",
      "    echo",
      "  done",
      "  exit 0",
      "}",
      ""
    };
  }

  private List<string> GenerateArgumentParsing()
  {
    return new List<string>
    {
      "",
      "# Parse command line arguments",
      "for i in \"${!__UTAH_ARG_NAMES[@]}\"; do",
      "  flag=\"${__UTAH_ARG_NAMES[$i]}\"",
      "  short=\"${__UTAH_ARG_SHORT_NAMES[$i]}\"",
      "  desc=\"${__UTAH_ARG_DESCRIPTIONS[$i]}\"",
      "  type=\"${__UTAH_ARG_TYPES[$i]}\"",
      "  required=\"${__UTAH_ARG_REQUIRED[$i]}\"",
      "  default=\"${__UTAH_ARG_DEFAULTS[$i]}\"",
      "  # Add to help display logic here",
      "done"
    };
  }
}
