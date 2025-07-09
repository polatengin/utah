using System.Text.RegularExpressions;

public partial class Compiler
{
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
        // Handle special cases that need validation before assignment
        if (ve.Value is UtilityRandomExpression randomExpr)
        {
          // Generate validation and assignment for utility.random()
          lines.AddRange(CompileUtilityRandomDeclaration(ve.Name, randomExpr, ve.IsConst));
        }
        else
        {
          var expressionValue = CompileExpression(ve.Value);
          if (ve.IsConst)
          {
            lines.Add($"readonly {ve.Name}={expressionValue}");
          }
          else
          {
            lines.Add($"{ve.Name}={expressionValue}");
          }
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
        var ifCondition = CompileExpression(ifs.Condition);

        // Handle boolean variable conditions
        if (ifs.Condition is VariableExpression varExpr)
        {
          // For boolean variables, check if they equal "true"
          ifCondition = $"\"${{{varExpr.Name}}}\" = \"true\"";
        }
        // Handle negated boolean variable conditions
        else if (ifs.Condition is UnaryExpression unary1 && unary1.Operator == "!" && unary1.Operand is VariableExpression negVarExpr)
        {
          // For negated boolean variables, check if they equal "false"
          ifCondition = $"\"${{{negVarExpr.Name}}}\" = \"false\"";
        }
        // Handle boolean function calls (ArrayIsEmpty, ConsoleIsSudo, etc.)
        else if (ifs.Condition is ArrayIsEmpty || ifs.Condition is ConsoleIsSudoExpression || ifs.Condition is ConsolePromptYesNoExpression || ifs.Condition is OsIsInstalledExpression)
        {
          // For boolean functions, check if the result equals "true"
          ifCondition = $"\"{ifCondition}\" = \"true\"";
        }
        // Handle unary expressions with boolean functions
        else if (ifs.Condition is UnaryExpression unary && unary.Operator == "!" &&
                 (unary.Operand is ArrayIsEmpty || unary.Operand is ConsoleIsSudoExpression || unary.Operand is ConsolePromptYesNoExpression || unary.Operand is OsIsInstalledExpression))
        {
          var operandCondition = CompileExpression(unary.Operand);
          // For negated boolean functions, check if the result equals "false"
          ifCondition = $"\"{operandCondition}\" = \"false\"";
        }
        else if (ifCondition.StartsWith("[ ") && ifCondition.EndsWith(" ]"))
        {
          ifCondition = ifCondition.Substring(2, ifCondition.Length - 4);
        }

        lines.Add($"if [ {ifCondition} ]; then");
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

      case WhileStatement whileStmt:
        var whileCondition = CompileExpression(whileStmt.Condition);
        if (whileCondition.StartsWith("[ ") && whileCondition.EndsWith(" ]"))
        {
          whileCondition = whileCondition.Substring(2, whileCondition.Length - 4);
        }
        lines.Add($"while [ {whileCondition} ]; do");
        foreach (var bodyStmt in whileStmt.Body)
        {
          lines.AddRange(CompileBlock(bodyStmt));
        }
        lines.Add("done");
        break;

      case BreakStatement breakStmt:
        lines.Add("break");
        break;

      case ConsoleClearStatement:
        lines.Add("clear");
        break;

      case ScriptEnableDebugStatement:
        lines.Add("set -x");
        break;

      case ScriptDisableDebugStatement:
        lines.Add("set +x");
        break;

      case ScriptDisableGlobbingStatement:
        lines.Add("set -f");
        break;

      case ScriptEnableGlobbingStatement:
        lines.Add("set +f");
        break;

      case ScriptExitOnErrorStatement:
        lines.Add("set -e");
        break;

      case ScriptContinueOnErrorStatement:
        lines.Add("set +e");
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
            if (compiled.StartsWith('\"') && compiled.EndsWith('\"'))
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

      case ArrayAssignment arrayAssign:
        var arrayIndex = CompileExpression(arrayAssign.Index);
        var arrayValue = CompileExpression(arrayAssign.Value);
        // In bash, array assignment syntax is: arrayName[index]=value
        lines.Add($"{arrayAssign.ArrayName}[{arrayIndex}]={arrayValue}");
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
}
