using System.Text.RegularExpressions;

public class Compiler
{
  public string Compile(ProgramNode program)
  {
    var lines = new List<string>
    {
      "#!/bin/sh",
      "" // Empty line after shebang
    };

    foreach (var stmt in program.Statements)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    return string.Join('\n', lines) + '\n';
  }

  private List<string> CompileStatement(Node stmt)
  {
    var lines = new List<string>();

    switch (stmt)
    {
      case RawStatement raw:
        lines.Add(raw.Content);
        break;

      case VariableDeclaration v:
        if (v.IsConst)
        {
          lines.Add($"readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"{v.Name}=\"{v.Value}\"");
        }
        break;

      case VariableDeclarationExpression ve:
        var expressionValue = CompileExpression(ve.Value);
        if (ve.IsConst)
        {
          lines.Add($"readonly {ve.Name}={expressionValue}");
        }
        else
        {
          lines.Add($"{ve.Name}={expressionValue}");
        }
        break;

      case ConsoleIsSudoExpression sudo:
        // This case is for when console.isSudo() is a statement, which is not valid.
        // However, to be robust, we can throw an error or handle it gracefully.
        // For now, we'll assume it's part of an expression and handled by CompileExpression.
        break;

      case FunctionDeclaration f:
        lines.Add($"{f.Name}() {{");
        for (int j = 0; j < f.Parameters.Count; j++)
        {
          var (paramName, _) = f.Parameters[j];
          lines.Add($"  local {paramName}=\"${j + 1}\"");
        }
        foreach (var b in f.Body)
          lines.AddRange(CompileBlock(b));
        lines.Add("}");
        break;

      case FunctionCall c:
        var bashArgs = c.Arguments.Select(a =>
        {
          if (a.StartsWith("\"") && a.EndsWith("\""))
          {
            // String literal - use as-is
            return a;
          }
          else
          {
            // Variable reference - add $ prefix
            return $"\"${a}\"";
          }
        }).ToList();
        if (bashArgs.Count > 0)
        {
          lines.Add($"{c.Name} {string.Join(" ", bashArgs)}");
        }
        else
        {
          lines.Add(c.Name);
        }
        break;

      case ConsoleLog log:
        if (log.IsExpression && log.Expression != null)
        {
          var compiledExpr = CompileExpression(log.Expression);
          // If the expression is already quoted (like string concatenation), don't add extra quotes
          if (compiledExpr.StartsWith("\"") && compiledExpr.EndsWith("\""))
          {
            lines.Add($"echo {compiledExpr}");
          }
          else
          {
            lines.Add($"echo \"{compiledExpr}\"");
          }
        }
        else
        {
          var message = log.Message.Replace("\"", "\\\"").Replace("`", "\\`");
          lines.Add($"echo \"{message}\"");
        }
        break;

      case IfStatement ifs:
        // Check if condition is a simple variable (no parentheses) or a function call
        if (ifs.ConditionCall.Contains("(") && ifs.ConditionCall.Contains(")"))
        {
          // Function call - use command substitution
          lines.Add($"if [ \"$({ifs.ConditionCall})\" = \"true\" ]; then");
        }
        else
        {
          // Simple variable - use direct variable reference
          lines.Add($"if [ \"${{{ifs.ConditionCall}}}\" = \"true\" ]; then");
        }
        foreach (var b in ifs.ThenBody)
          lines.AddRange(CompileBlock(b));

        // Only add else clause if there are statements in the else body
        if (ifs.ElseBody.Count > 0)
        {
          lines.Add("else");
          foreach (var b in ifs.ElseBody)
            lines.AddRange(CompileBlock(b));
        }

        lines.Add("fi");
        break;

      case ForLoop forLoop:
        var initValue = CompileExpression(forLoop.InitValue);
        lines.Add($"{forLoop.InitVariable}={initValue}");

        var condition = CompileExpression(forLoop.Condition);
        if (condition.StartsWith("[ ") && condition.EndsWith(" ]"))
        {
          condition = condition.Substring(2, condition.Length - 4);
        }
        lines.Add($"while [ {condition} ]; do");

        foreach (var bodyStmt in forLoop.Body)
          lines.AddRange(CompileBlock(bodyStmt));

        switch (forLoop.UpdateOperator)
        {
          case "++":
            lines.Add($"  {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} + 1))");
            break;
          case "--":
            lines.Add($"  {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} - 1))");
            break;
          case "+=":
            var addValue = CompileExpression(forLoop.UpdateValue!);
            lines.Add($"  {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} + {addValue}))");
            break;
          case "-=":
            var subValue = CompileExpression(forLoop.UpdateValue!);
            lines.Add($"  {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} - {subValue}))");
            break;
        }

        lines.Add("done");
        break;

      case ForInLoop forInLoop:
        lines.Add($"for {forInLoop.Variable} in \"${{{forInLoop.Iterable}[@]}}\"; do");

        foreach (var bodyStmt in forInLoop.Body)
          lines.AddRange(CompileBlock(bodyStmt));

        lines.Add("done");
        break;

      case StringLength sl:
        lines.Add($"{sl.TargetVariable}=\"${{#{sl.SourceString}}}\"");
        break;

      case StringSlice ss:
        if (ss.EndIndex.HasValue)
        {
          var length = ss.EndIndex.Value - ss.StartIndex;
          lines.Add($"{ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}:{length}}}\"");
        }
        else
        {
          lines.Add($"{ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}}}\"");
        }
        break;

      case StringReplace sr:
        if (sr.ReplaceAll)
        {
          lines.Add($"{sr.TargetVariable}=\"${{{sr.SourceString}//{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        else
        {
          lines.Add($"{sr.TargetVariable}=\"${{{sr.SourceString}/{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        break;

      case StringToUpper su:
        lines.Add($"{su.TargetVariable}=\"${{{su.SourceString}^^}}\"");
        break;

      case StringToLower sl:
        lines.Add($"{sl.TargetVariable}=\"${{{sl.SourceString},,}}\"");
        break;

      case StringTrim st:
        // Custom bash function for trimming
        lines.Add($"{st.TargetVariable}=$(echo \"${{{st.SourceString}}}\" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')");
        break;

      case StringStartsWith ssw:
        lines.Add($"if [[ \"${{{ssw.SourceString}}}\" == \"{ssw.Prefix}\"* ]]; then");
        lines.Add($"  {ssw.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {ssw.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringEndsWith sew:
        lines.Add($"if [[ \"${{{sew.SourceString}}}\" == *\"{sew.Suffix}\" ]]; then");
        lines.Add($"  {sew.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {sew.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringContains sc:
        lines.Add($"if [[ \"${{{sc.SourceString}}}\" == *\"{sc.Substring}\"* ]]; then");
        lines.Add($"  {sc.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {sc.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringSplit sp:
        // Check if SourceString looks like a variable name or a string literal
        var sourceValue = Regex.IsMatch(sp.SourceString, @"^\w+$")
          ? $"\"${{{sp.SourceString}}}\""
          : $"\"{sp.SourceString}\"";
        lines.Add($"IFS='{sp.Delimiter}' read -ra {sp.ResultArrayName} <<< {sourceValue}");
        break;

      case OsIsInstalled os:
        lines.Add($"if command -v {os.AppName} &> /dev/null; then");
        lines.Add($"  {os.AssignTo}=\"true\"");
        lines.Add("else");
        lines.Add($"  {os.AssignTo}=\"false\"");
        lines.Add("fi");
        break;

      case SwitchStatement sw:
        var switchExpr = CompileExpression(sw.SwitchExpression);
        lines.Add($"case {switchExpr} in");

        foreach (var caseClause in sw.Cases)
        {
          // Build case pattern - handle multiple values for fall-through
          var casePatterns = caseClause.Values.Select(v =>
          {
            var compiled = CompileExpression(v);
            // Remove quotes if they exist since case patterns don't need them
            if (compiled.StartsWith("\"") && compiled.EndsWith("\""))
            {
              return compiled[1..^1];
            }
            return compiled;
          }).ToList();

          var pattern = string.Join("|", casePatterns);
          lines.Add($"  {pattern})");

          foreach (var bodyStmt in caseClause.Body)
          {
            var bodyLines = CompileStatement(bodyStmt);
            foreach (var bodyLine in bodyLines)
            {
              lines.Add($"    {bodyLine}");
            }
          }

          lines.Add("    ;;");
        }

        if (sw.DefaultCase != null)
        {
          lines.Add("  *)");
          foreach (var bodyStmt in sw.DefaultCase.Body)
          {
            var bodyLines = CompileStatement(bodyStmt);
            foreach (var bodyLine in bodyLines)
            {
              lines.Add($"    {bodyLine}");
            }
          }
          lines.Add("    ;;");
        }

        lines.Add("esac");
        break;

      case AssignmentStatement assign:
        var assignValue = CompileExpression(assign.Value);
        lines.Add($"{assign.VariableName}={assignValue}");
        break;

      case ExitStatement e:
        lines.Add($"exit {e.ExitCode}");
        break;

      case FsReadFile fr:
        // Generate Bash code to read file contents into a variable
        // Use cat command with command substitution
        lines.Add($"{fr.AssignTo}=$(cat \"{fr.FilePath}\")");
        break;

      case FsWriteFile fw:
        // Generate Bash code to write content to a file
        if (fw.Content.StartsWith("${") && fw.Content.EndsWith("}"))
        {
          // Variable reference - use directly
          lines.Add($"echo \"{fw.Content}\" > \"{fw.FilePath}\"");
        }
        else
        {
          // String literal - wrap in quotes
          lines.Add($"echo \"{fw.Content}\" > \"{fw.FilePath}\"");
        }
        break;

      case FsDirname fd:
        // Generate Bash code to get directory name (dirname command)
        lines.Add($"{fd.AssignTo}=$(dirname \"{fd.FilePath}\")");
        break;

      case FsParentDirName fpd:
        // Generate Bash code to get parent directory name (basename of dirname)
        lines.Add($"{fpd.AssignTo}=$(basename $(dirname \"{fpd.FilePath}\"))");
        break;

      case FsExtension fe:
        // Generate Bash code to get file extension
        // Use a temporary variable to store the path, then extract extension
        lines.Add($"_temp_path=\"{fe.FilePath}\"");
        lines.Add($"{fe.AssignTo}=\"${{_temp_path##*.}}\"");
        break;

      case FsFileName fn:
        // Generate Bash code to get file name (basename command)
        lines.Add($"{fn.AssignTo}=$(basename \"{fn.FilePath}\")");
        break;

      case TimerStart ts:
        // Record the start time in milliseconds
        lines.Add("_utah_timer_start=$(date +%s%3N)");
        break;

      case TimerStop ts:
        // Calculate elapsed time and assign to variable
        lines.Add("_utah_timer_end=$(date +%s%3N)");
        lines.Add($"{ts.AssignTo}=$((_utah_timer_end - _utah_timer_start))");
        break;

      case ProcessId pi:
        // Get process ID using ps command
        lines.Add($"{pi.AssignTo}=$(ps -o pid -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessCpu pc:
        // Get CPU usage percentage using ps command
        lines.Add($"{pc.AssignTo}=$(ps -o pcpu -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessMemory pm:
        // Get memory usage percentage using ps command
        lines.Add($"{pm.AssignTo}=$(ps -o pmem -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessElapsedTime pet:
        // Get elapsed time using ps command
        lines.Add($"{pet.AssignTo}=$(ps -o etime -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessCommand pcmd:
        // Get command line using ps command
        lines.Add($"{pcmd.AssignTo}=$(ps -o cmd= -p $$)");
        break;

      case ProcessStatus pstat:
        // Get process status using ps command
        lines.Add($"{pstat.AssignTo}=$(ps -o stat= -p $$)");
        break;

      case OsGetOS ogo:
        lines.Add($"_uname_os_get_os=$(uname | tr '[:upper:]' '[:lower:]')");
        lines.Add($"case $_uname_os_get_os in");
        lines.Add($"  linux*)");
        lines.Add($"    {ogo.AssignTo}=\"linux\"");
        lines.Add($"    ;;");
        lines.Add($"  darwin*)");
        lines.Add($"    {ogo.AssignTo}=\"mac\"");
        lines.Add($"    ;;");
        lines.Add($"  msys* | cygwin* | mingw* | nt | win*)");
        lines.Add($"    {ogo.AssignTo}=\"windows\"");
        lines.Add($"    ;;");
        lines.Add($"  *)");
        lines.Add($"    {ogo.AssignTo}=\"unknown\"");
        lines.Add($"    ;;");
        lines.Add($"esac");
        break;

      case OsGetLinuxVersion oglv:
        lines.Add($"if [[ -f /etc/os-release ]]; then");
        lines.Add($"  source /etc/os-release");
        lines.Add($"  {oglv.AssignTo}=\"${{VERSION_ID}}\"");
        lines.Add($"elif type lsb_release >/dev/null 2>&1; then");
        lines.Add($"  {oglv.AssignTo}=$(lsb_release -sr)");
        lines.Add($"elif [[ -f /etc/lsb-release ]]; then");
        lines.Add($"  source /etc/lsb-release");
        lines.Add($"  {oglv.AssignTo}=\"${{DISTRIB_RELEASE}}\"");
        lines.Add($"else");
        lines.Add($"  {oglv.AssignTo}=\"unknown\"");
        lines.Add($"fi");
        break;

      case EnvGet eg:
        if (string.IsNullOrEmpty(eg.DefaultValue))
        {
          lines.Add($"{eg.AssignTo}=\"${{{eg.VariableName}}}\"");
        }
        else
        {
          lines.Add($"{eg.AssignTo}=\"${{{eg.VariableName}:-{eg.DefaultValue}}}\"");
        }
        break;

      case EnvSet es:
        lines.Add($"export {es.VariableName}=\"{es.Value}\"");
        break;

      case EnvLoad el:
        lines.Add($"if [ -f \"{el.FilePath}\" ]; then");
        lines.Add($"  set -a");
        lines.Add($"  source \"{el.FilePath}\"");
        lines.Add($"  set +a");
        lines.Add($"fi");
        break;

      case EnvDelete ed:
        lines.Add($"unset {ed.VariableName}");
        break;
    }

    return lines;
  }

  private List<string> CompileBlock(Node stmt)
  {
    var lines = new List<string>();

    switch (stmt)
    {
      case VariableDeclaration v:
        if (v.IsConst)
        {
          lines.Add($"  readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"  local {v.Name}=\"{v.Value}\"");
        }
        break;
      case ConsoleLog log:
        if (log.IsExpression && log.Expression != null)
        {
          var compiledExpr = CompileExpression(log.Expression);
          // If the expression is already quoted (like string concatenation), don't add extra quotes
          if (compiledExpr.StartsWith("\"") && compiledExpr.EndsWith("\""))
          {
            lines.Add($"  echo {compiledExpr}");
          }
          else
          {
            lines.Add($"  echo \"{compiledExpr}\"");
          }
        }
        else
        {
          var message = log.Message.Replace("\"", "\\\"").Replace("`", "\\`");
          lines.Add($"  echo \"{message}\"");
        }
        break;
      case ReturnStatement ret:
        if (ret.Value != null)
        {
          var returnValue = CompileExpression(ret.Value);
          lines.Add($"  echo {returnValue}");
        }
        else
        {
          lines.Add($"  echo");
        }
        break;
      case ExitStatement e:
        lines.Add($"  exit {e.ExitCode}");
        break;

      // String manipulation functions inside blocks
      case StringLength sl:
        lines.Add($"  {sl.TargetVariable}=\"${{#{sl.SourceString}}}\"");
        break;

      case StringSlice ss:
        if (ss.EndIndex.HasValue)
        {
          var length = ss.EndIndex.Value - ss.StartIndex;
          lines.Add($"  {ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}:{length}}}\"");
        }
        else
        {
          lines.Add($"  {ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}}}\"");
        }
        break;

      case StringReplace sr:
        if (sr.ReplaceAll)
        {
          lines.Add($"  {sr.TargetVariable}=\"${{{sr.SourceString}//{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        else
        {
          lines.Add($"  {sr.TargetVariable}=\"${{{sr.SourceString}/{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        break;

      case StringToUpper su:
        lines.Add($"  {su.TargetVariable}=\"${{{su.SourceString}^^}}\"");
        break;

      case StringToLower sl:
        lines.Add($"  {sl.TargetVariable}=\"${{{sl.SourceString},,}}\"");
        break;

      case StringTrim st:
        lines.Add($"  {st.TargetVariable}=$(echo \"${{{st.SourceString}}}\" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')");
        break;

      case StringStartsWith ssw:
        lines.Add($"  if [[ \"${{{ssw.SourceString}}}\" == \"{ssw.Prefix}\"* ]]; then");
        lines.Add($"    {ssw.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {ssw.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringEndsWith sew:
        lines.Add($"  if [[ \"${{{sew.SourceString}}}\" == *\"{sew.Suffix}\" ]]; then");
        lines.Add($"    {sew.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {sew.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringContains sc:
        lines.Add($"  if [[ \"${{{sc.SourceString}}}\" == *\"{sc.Substring}\"* ]]; then");
        lines.Add($"    {sc.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {sc.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringSplit sp:
        lines.Add($"  IFS='{sp.Delimiter}' read -ra {sp.ResultArrayName} <<< \"${{{sp.SourceString}}}\"");
        break;

      case EnvGet eg:
        lines.Add($"  {eg.AssignTo}=\"${{{eg.VariableName}:-{eg.DefaultValue}}}\"");
        break;

      case EnvSet es:
        lines.Add($"  export {es.VariableName}=\"{es.Value}\"");
        break;

      case EnvLoad el:
        lines.Add($"  if [ -f \"{el.FilePath}\" ]; then");
        lines.Add($"    set -a");
        lines.Add($"    source \"{el.FilePath}\"");
        lines.Add($"    set +a");
        lines.Add($"  fi");
        break;

      case EnvDelete ed:
        lines.Add($"  unset {ed.VariableName}");
        break;

      case OsIsInstalled os:
        lines.Add($"  if command -v {os.AppName} &> /dev/null; then");
        lines.Add($"    {os.AssignTo}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {os.AssignTo}=\"false\"");
        lines.Add("  fi");
        break;

      case ForLoop forLoop:
        var initValue = CompileExpression(forLoop.InitValue);
        lines.Add($"  {forLoop.InitVariable}={initValue}");

        var condition = CompileExpression(forLoop.Condition);
        if (condition.StartsWith("[ ") && condition.EndsWith(" ]"))
        {
          condition = condition.Substring(2, condition.Length - 4);
        }
        lines.Add($"  while [ {condition} ]; do");

        foreach (var bodyStmt in forLoop.Body)
        {
          var innerLines = CompileBlock(bodyStmt);
          foreach (var line in innerLines)
          {
            lines.Add($"  {line}"); // Add extra indentation for nested content
          }
        }

        switch (forLoop.UpdateOperator)
        {
          case "++":
            lines.Add($"    {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} + 1))");
            break;
          case "--":
            lines.Add($"    {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} - 1))");
            break;
          case "+=":
            var addValue = CompileExpression(forLoop.UpdateValue!);
            lines.Add($"    {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} + {addValue}))");
            break;
          case "-=":
            var subValue = CompileExpression(forLoop.UpdateValue!);
            lines.Add($"    {forLoop.UpdateVariable}=$(({forLoop.UpdateVariable} - {subValue}))");
            break;
        }

        lines.Add("  done");
        break;

      case ForInLoop forInLoop:
        lines.Add($"  for {forInLoop.Variable} in \"${{{forInLoop.Iterable}[@]}}\"; do");
        foreach (var bodyStmt in forInLoop.Body)
        {
          var innerLines = CompileBlock(bodyStmt);
          foreach (var line in innerLines)
          {
            lines.Add($"  {line}"); // Add extra indentation for nested content
          }
        }
        lines.Add("  done");
        break;

      case FsReadFile fr:
        // Generate Bash code to read file contents into a variable (inside function block)
        lines.Add($"  {fr.AssignTo}=$(cat \"{fr.FilePath}\")");
        break;

      case FsWriteFile fw:
        // Generate Bash code to write content to a file (inside function block)
        if (fw.Content.StartsWith("${") && fw.Content.EndsWith("}"))
        {
          // Variable reference - use directly
          lines.Add($"  echo \"{fw.Content}\" > \"{fw.FilePath}\"");
        }
        else
        {
          // String literal - wrap in quotes
          lines.Add($"  echo \"{fw.Content}\" > \"{fw.FilePath}\"");
        }
        break;

      case FsDirname fd:
        // Generate Bash code to get directory name (inside function block)
        lines.Add($"  {fd.AssignTo}=$(dirname \"{fd.FilePath}\")");
        break;

      case FsParentDirName fpd:
        // Generate Bash code to get parent directory name (inside function block)
        lines.Add($"  {fpd.AssignTo}=$(basename $(dirname \"{fpd.FilePath}\"))");
        break;

      case FsExtension fe:
        // Generate Bash code to get file extension (inside function block)
        // Use a temporary variable to store the path, then extract extension
        lines.Add($"  _temp_path=\"{fe.FilePath}\"");
        lines.Add($"  {fe.AssignTo}=\"${{_temp_path##*.}}\"");
        break;

      case FsFileName fn:
        // Generate Bash code to get file name (inside function block)
        lines.Add($"  {fn.AssignTo}=$(basename \"{fn.FilePath}\")");
        break;

      case ProcessId pi:
        // Get process ID using ps command (inside function block)
        lines.Add($"  {pi.AssignTo}=$(ps -o pid -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessCpu pc:
        // Get CPU usage percentage using ps command (inside function block)
        lines.Add($"  {pc.AssignTo}=$(ps -o pcpu -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessMemory pm:
        // Get memory usage percentage using ps command (inside function block)
        lines.Add($"  {pm.AssignTo}=$(ps -o pmem -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessElapsedTime pet:
        // Get elapsed time using ps command (inside function block)
        lines.Add($"  {pet.AssignTo}=$(ps -o etime -p $$ --no-headers | tr -d ' ')");
        break;

      case ProcessCommand pcmd:
        // Get command line using ps command (inside function block)
        lines.Add($"  {pcmd.AssignTo}=$(ps -o cmd= -p $$)");
        break;

      case ProcessStatus pstat:
        // Get process status using ps command (inside function block)
        lines.Add($"  {pstat.AssignTo}=$(ps -o stat= -p $$)");
        break;
    }

    return lines;
  }

  private string CompileExpression(Expression expr)
  {
    return expr switch
    {
      LiteralExpression lit => CompileLiteralExpression(lit),
      VariableExpression var => CompileVariableExpression(var),
      BinaryExpression bin => CompileBinaryExpression(bin),
      UnaryExpression un => CompileUnaryExpression(un),
      TernaryExpression tern => CompileTernaryExpression(tern),
      ParenthesizedExpression paren => CompileParenthesizedExpression(paren),
      ArrayLiteral arr => CompileArrayLiteral(arr),
      ArrayAccess acc => CompileArrayAccess(acc),
      ArrayLength len => CompileArrayLength(len),
      FunctionCall func => CompileFunctionCallExpression(func),
      ConsoleIsSudoExpression sudo => CompileConsoleIsSudoExpression(sudo),
      ConsolePromptYesNoExpression prompt => CompileConsolePromptYesNoExpression(prompt),
      _ => throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported")
    };
  }

  private string CompileConsoleIsSudoExpression(ConsoleIsSudoExpression sudo)
  {
    return "$([ \"$(id -u)\" -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileConsolePromptYesNoExpression(ConsolePromptYesNoExpression prompt)
  {
    // Generate bash code that prompts the user and returns true/false based on yes/no response
    return $"$(while true; do read -p \"{prompt.PromptText} (y/n): \" yn; case $yn in [Yy]* ) echo \"true\"; break;; [Nn]* ) echo \"false\"; break;; * ) echo \"Please answer yes or no.\";; esac; done)";
  }

  private string CompileLiteralExpression(LiteralExpression lit)
  {
    return lit.Type switch
    {
      "string" => $"\"{lit.Value}\"",
      "number" => lit.Value,
      "boolean" => lit.Value,
      _ => $"\"{lit.Value}\""
    };
  }

  private string CompileVariableExpression(VariableExpression var)
  {
    return $"${{{var.Name}}}";
  }

  private string CompileBinaryExpression(BinaryExpression bin)
  {
    var left = CompileExpression(bin.Left);
    var right = CompileExpression(bin.Right);

    return bin.Operator switch
    {
      "+" when IsStringConcatenation(bin) => CompileStringConcatenation(left, right),
      "+" => $"$(({ExtractVariableName(left)} + {ExtractVariableName(right)}))",
      "-" => $"$(({ExtractVariableName(left)} - {ExtractVariableName(right)}))",
      "*" => $"$(({ExtractVariableName(left)} * {ExtractVariableName(right)}))",
      "/" => $"$(({ExtractVariableName(left)} / {ExtractVariableName(right)}))",
      "%" => $"$(({ExtractVariableName(left)} % {ExtractVariableName(right)}))",
      "==" => $"[ {left} = {right} ]",
      "!=" => $"[ {left} != {right} ]",
      "<" => $"[ {left} -lt {right} ]",
      "<=" => $"[ {left} -le {right} ]",
      ">" => $"[ {left} -gt {right} ]",
      ">=" => $"[ {left} -ge {right} ]",
      "&&" => $"{left} && {right}",
      "||" => $"{left} || {right}",
      _ => throw new NotSupportedException($"Binary operator {bin.Operator} is not supported")
    };
  }

  private string CompileUnaryExpression(UnaryExpression un)
  {
    var operand = CompileExpression(un.Operand);
    return un.Operator switch
    {
      "!" => $"! {operand}",
      "-" => $"$((-${operand}))",
      _ => throw new NotSupportedException($"Unary operator {un.Operator} is not supported")
    };
  }

  private string CompileTernaryExpression(TernaryExpression tern)
  {
    var condition = CompileExpression(tern.Condition);
    var trueExpr = CompileExpression(tern.TrueExpression);
    var falseExpr = CompileExpression(tern.FalseExpression);

    // Handle nested ternary by avoiding nested command substitution
    if (tern.FalseExpression is TernaryExpression)
    {
      // Directly compile the nested ternary as a conditional chain
      var nestedCondition = CompileExpression(((TernaryExpression)tern.FalseExpression).Condition);
      var nestedTrue = CompileExpression(((TernaryExpression)tern.FalseExpression).TrueExpression);
      var nestedFalse = CompileExpression(((TernaryExpression)tern.FalseExpression).FalseExpression);

      return $"$({condition} && echo {trueExpr} || ({nestedCondition} && echo {nestedTrue} || echo {nestedFalse}))";
    }

    return $"$({condition} && echo {trueExpr} || echo {falseExpr})";
  }

  private string CompileParenthesizedExpression(ParenthesizedExpression paren)
  {
    return $"({CompileExpression(paren.Inner)})";
  }

  private string CompileStringConcatenation(string left, string right)
  {
    // Remove quotes from string literals
    var leftPart = left.StartsWith("\"") && left.EndsWith("\"") ? left[1..^1] : left;

    // Handle variable expressions - extract and format properly
    string rightPart;
    if (right.StartsWith("${") && right.EndsWith("}"))
    {
      // Already in ${var} format, use as is
      rightPart = right;
    }
    else if (right.StartsWith("$"))
    {
      // Old $var format, convert to ${var}
      var varName = right.TrimStart('$');
      rightPart = $"${{{varName}}}";
    }
    else if (right.StartsWith("\"") && right.EndsWith("\""))
    {
      rightPart = right[1..^1];
    }
    else
    {
      rightPart = right;
    }

    return $"\"{leftPart}{rightPart}\"";
  }

  private bool IsStringConcatenation(BinaryExpression bin)
  {
    // Simple heuristic: if either operand is a string literal or we're adding strings
    return (bin.Left is LiteralExpression leftLit && leftLit.Type == "string") ||
           (bin.Right is LiteralExpression rightLit && rightLit.Type == "string");
  }

  private string CompileArrayLiteral(ArrayLiteral arr)
  {
    // Bash arrays are declared and initialized like: arr=("item1" "item2" "item3")
    var elements = arr.Elements.Select(CompileExpression).ToList();
    return $"({string.Join(" ", elements)})";
  }

  private string CompileArrayAccess(ArrayAccess acc)
  {
    var arrayName = ExtractVariableName(CompileExpression(acc.Array));
    var index = CompileExpression(acc.Index);

    // In bash, array access is ${arrayName[index]}
    return $"\"${{{arrayName}[{ExtractVariableName(index)}]}}\"";
  }

  private string CompileArrayLength(ArrayLength len)
  {
    var arrayName = ExtractVariableName(CompileExpression(len.Array));

    // In bash, array length is ${#arrayName[@]}
    return $"\"${{#{arrayName}[@]}}\"";
  }

  private string CompileFunctionCallExpression(FunctionCall func)
  {
    // In bash, function calls in expressions use command substitution
    var bashArgs = func.Arguments.Select(a =>
    {
      var trimmed = a.Trim();
      if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
      {
        // String literal - use as-is
        return trimmed;
      }
      else if (double.TryParse(trimmed, out _))
      {
        // Number literal - use as-is
        return trimmed;
      }
      else
      {
        // Variable reference - add $ prefix
        return $"\"${trimmed}\"";
      }
    }).ToList();

    if (bashArgs.Count > 0)
    {
      return $"$({func.Name} {string.Join(" ", bashArgs)})";
    }
    else
    {
      return $"$({func.Name})";
    }
  }

  private string ExtractVariableName(string varExpr)
  {
    // Handle both $var and ${var} formats
    if (varExpr.StartsWith("${") && varExpr.EndsWith("}"))
    {
      return varExpr[2..^1]; // Remove ${ and }
    }
    else if (varExpr.StartsWith("$"))
    {
      return varExpr[1..]; // Remove $
    }
    return varExpr;
  }
}
