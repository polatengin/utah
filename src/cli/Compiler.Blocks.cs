public partial class Compiler
{
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

      case VariableDeclarationExpression ve:
        // Handle special cases that need validation before assignment
        if (ve.Value is UtilityRandomExpression randomExpr)
        {
          // Generate validation and assignment for utility.random()
          var randomLines = CompileUtilityRandomDeclaration(ve.Name, randomExpr, ve.IsConst);
          foreach (var line in randomLines)
          {
            lines.Add($"  {line}");
          }
        }
        else
        {
          var expressionValue = CompileExpression(ve.Value);
          if (ve.IsConst)
          {
            lines.Add($"  readonly {ve.Name}={expressionValue}");
          }
          else
          {
            lines.Add($"  {ve.Name}={expressionValue}");
          }
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

      case WhileStatement whileStmt:
        var whileCondition = CompileExpression(whileStmt.Condition);
        if (whileCondition.StartsWith("[ ") && whileCondition.EndsWith(" ]"))
        {
          whileCondition = whileCondition.Substring(2, whileCondition.Length - 4);
        }
        lines.Add($"  while [ {whileCondition} ]; do");
        foreach (var bodyStmt in whileStmt.Body)
        {
          var innerLines = CompileBlock(bodyStmt);
          foreach (var line in innerLines)
          {
            lines.Add($"    {line}");
          }
        }
        lines.Add("  done");
        break;

      case BreakStatement breakStmt:
        lines.Add("  break");
        break;

      case ConsoleClearStatement:
        lines.Add("  clear");
        break;

      case ScriptEnableDebugStatement:
        lines.Add("  set -x");
        break;

      case ScriptDisableDebugStatement:
        lines.Add("  set +x");
        break;

      case ScriptDisableGlobbingStatement:
        lines.Add("  set -f");
        break;

      case ScriptEnableGlobbingStatement:
        lines.Add("  set +f");
        break;

      case ScriptExitOnErrorStatement:
        lines.Add("  set -e");
        break;

      case ScriptContinueOnErrorStatement:
        lines.Add("  set +e");
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
        // Get CPU usage percentage using ps command (inside function
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

      case IfStatement ifs:
        var ifCondition = CompileExpression(ifs.Condition);

        // Handle boolean variable conditions
        if (ifs.Condition is VariableExpression varExpr)
        {
          // For boolean variables, check if they equal "true"
          ifCondition = $"\"${{{varExpr.Name}}}\" = \"true\"";
        }
        else if (ifCondition.StartsWith("[ ") && ifCondition.EndsWith(" ]"))
        {
          ifCondition = ifCondition.Substring(2, ifCondition.Length - 4);
        }

        lines.Add($"  if [ {ifCondition} ]; then");
        foreach (var b in ifs.ThenBody)
        {
          var innerLines = CompileBlock(b);
          foreach (var line in innerLines)
          {
            lines.Add($"  {line}");
          }
        }

        if (ifs.ElseBody.Count > 0)
        {
          lines.Add("  else");
          foreach (var b in ifs.ElseBody)
          {
            var innerLines = CompileBlock(b);
            foreach (var line in innerLines)
            {
              lines.Add($"  {line}");
            }
          }
        }
        lines.Add("  fi");
        break;

      case AssignmentStatement assign:
        var assignValue = CompileExpression(assign.Value);
        lines.Add($"  {assign.VariableName}={assignValue}");
        break;
    }

    return lines;
  }
}
