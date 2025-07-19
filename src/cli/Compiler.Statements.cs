using System.Text.RegularExpressions;

public partial class Compiler
{
  private List<string> CompileStatement(Statement stmt)
  {
    var lines = new List<string>();

    switch (stmt)
    {
      case RawStatement raw:
        if (_inTryBlock)
        {
          // In try blocks, ensure raw statements properly exit on failure
          lines.Add($"{raw.Content} || exit 1");
        }
        else
        {
          lines.Add(raw.Content);
        }
        break;

      case ImportStatement import:
        // Import statements are resolved at preprocessing time, so we don't generate any code
        break;

      case VariableDeclaration v:
        // Special handling for StringSplitExpression
        if (v.Value is StringSplitExpression split)
        {
          string targetValue;
          if (split.Target is VariableExpression varE2)
          {
            targetValue = $"${{{varE2.Name}}}";
          }
          else if (split.Target is LiteralExpression literal && literal.Type == "string")
          {
            // For string literals, use the value directly without quotes since we'll add them back
            targetValue = literal.Value;
          }
          else
          {
            // For other expressions, compile and extract the result
            var compiled = CompileExpression(split.Target);
            // If it's a quoted string, remove the quotes since we'll add them back
            if (compiled.StartsWith("\"") && compiled.EndsWith("\""))
            {
              targetValue = compiled.Substring(1, compiled.Length - 2);
            }
            else
            {
              targetValue = $"${{{ExtractVariableName(compiled)}}}";
            }
          }

          var separator = CompileExpression(split.Separator);
          // Remove quotes from separator if it's a string literal
          if (separator.StartsWith("\"") && separator.EndsWith("\""))
          {
            separator = separator[1..^1];
          }
          lines.Add($"IFS='{separator}' read -ra {v.Name} <<< \"{targetValue}\"");
        }
        // Special handling for timer.stop() in variable declarations
        else if (v.Value is TimerStopExpression)
        {
          lines.Add("_utah_timer_end=$(date +%s%3N)");
          if (v.IsConst)
          {
            lines.Add($"readonly {v.Name}=$((_utah_timer_end - _utah_timer_start))");
          }
          else
          {
            lines.Add($"{v.Name}=$((_utah_timer_end - _utah_timer_start))");
          }
        }
        else
        {
          var expressionValue = CompileExpression(v.Value);
          if (v.Value is ArrayAccess)
          {
            expressionValue = $"\"{expressionValue}\"";
          }
          if (v.Value is ArrayLength)
          {
            expressionValue = $"\"{expressionValue}\"";
          }
          // For boolean assignments from binary expressions, wrap in command substitution
          if (v.Value is BinaryExpression binExpr && IsBooleanComparison(binExpr))
          {
            expressionValue = $"$({expressionValue} && echo \"true\" || echo \"false\")";
          }
          if (v.IsConst)
          {
            lines.Add($"readonly {v.Name}={expressionValue}");
          }
          else
          {
            lines.Add($"{v.Name}={expressionValue}");
          }
        }
        break;

      case ExpressionStatement exprStmt:
        // Handle fs.writeFile() placeholder
        if (exprStmt.Expression is FsWriteFileExpressionPlaceholder fsWritePlaceholder)
        {
          var writeFilePath = CompileExpression(fsWritePlaceholder.FilePath);
          var writeContent = CompileExpression(fsWritePlaceholder.Content);

          // Always add quotes around content if not already present
          if (!writeContent.StartsWith("\""))
          {
            writeContent = $"\"{writeContent}\"";
          }

          lines.Add($"echo {writeContent} > {writeFilePath}");
        }
        // For function calls in statement context, don't wrap in $()
        else if (exprStmt.Expression is FunctionCall funcCall)
        {
          // Special handling for env.set() function calls
          if (funcCall.Name == "env.set" && funcCall.Arguments.Count == 2)
          {
            var varName = CompileExpression(funcCall.Arguments[0]);
            var value = CompileExpression(funcCall.Arguments[1]);
            // Remove quotes from varName if it's a string literal
            if (varName.StartsWith("\"") && varName.EndsWith("\""))
            {
              varName = varName[1..^1];
            }
            lines.Add($"export {varName}={value}");
          }
          // Special handling for env.load() function calls
          else if (funcCall.Name == "env.load" && funcCall.Arguments.Count == 1)
          {
            var envFilePath = CompileExpression(funcCall.Arguments[0]);
            lines.Add($"[ -f {envFilePath} ] && source {envFilePath}");
          }
          // Special handling for env.delete() function calls
          else if (funcCall.Name == "env.delete" && funcCall.Arguments.Count == 1)
          {
            var varName = CompileExpression(funcCall.Arguments[0]);
            // Remove quotes from varName if it's a string literal
            if (varName.StartsWith("\"") && varName.EndsWith("\""))
            {
              varName = varName[1..^1];
            }
            lines.Add($"unset {varName}");
          }
          // Special handling for git.undoLastCommit() function calls
          else if (funcCall.Name == "git.undoLastCommit")
          {
            lines.Add("git reset --soft HEAD~1");
          }
          else
          {
            var bashArgs = funcCall.Arguments.Select(CompileExpression).ToList();
            if (bashArgs.Count > 0)
            {
              lines.Add($"{funcCall.Name} {string.Join(" ", bashArgs)}");
            }
            else
            {
              lines.Add(funcCall.Name);
            }
          }
        }
        // Special handling for scheduler.cron() expressions
        else if (exprStmt.Expression is SchedulerCronExpression schedulerCron)
        {
          lines.AddRange(CompileSchedulerCronStatement(schedulerCron));
        }
        // Special handling for git.undoLastCommit() expressions in statement context
        else if (exprStmt.Expression is GitUndoLastCommitExpression)
        {
          lines.Add("git reset --soft HEAD~1");
        }
        // Parallel function call
        else if (exprStmt.Expression is ParallelFunctionCall parallelCall)
        {
          var bashArgs = parallelCall.Arguments.Select(CompileExpression).ToList();
          if (bashArgs.Count > 0)
          {
            lines.Add($"{parallelCall.Name} {string.Join(" ", bashArgs)} &");
          }
          else
          {
            lines.Add($"{parallelCall.Name} &");
          }
        }
        // Special handling for increment/decrement expressions in statement context
        else if (exprStmt.Expression is PostIncrementExpression postInc && postInc.Operand is VariableExpression incVar)
        {
          lines.Add($"{incVar.Name}=$(({incVar.Name} + 1))");
        }
        else if (exprStmt.Expression is PostDecrementExpression postDec && postDec.Operand is VariableExpression decVar)
        {
          lines.Add($"{decVar.Name}=$(({decVar.Name} - 1))");
        }
        else if (exprStmt.Expression is PreIncrementExpression preInc && preInc.Operand is VariableExpression preIncVar)
        {
          lines.Add($"{preIncVar.Name}=$(({preIncVar.Name} + 1))");
        }
        else if (exprStmt.Expression is PreDecrementExpression preDec && preDec.Operand is VariableExpression preDecVar)
        {
          lines.Add($"{preDecVar.Name}=$(({preDecVar.Name} - 1))");
        }
        else
        {
          lines.Add(CompileExpression(exprStmt.Expression));
        }
        break;

      case FunctionDeclaration f:
        lines.Add($"{f.Name}() {{");
        for (int j = 0; j < f.Parameters.Count; j++)
        {
          var (paramName, _) = f.Parameters[j];
          lines.Add($"  local {paramName}=\"${j + 1}\"");
        }
        foreach (var b in f.Body)
          lines.AddRange(CompileStatement(b).Select(l => "  " + l));
        lines.Add("}");
        break;

      case ConsoleLog log:
        var compiledExpr = CompileExpression(log.Message);
        // If the expression is already quoted (like string concatenation), don't add extra quotes
        if (compiledExpr.StartsWith("\"") && compiledExpr.EndsWith("\""))
        {
          lines.Add($"echo {compiledExpr}");
        }
        else
        {
          lines.Add($"echo \"{compiledExpr}\"");
        }
        break;

      case ReturnStatement ret:
        if (ret.Value != null)
        {
          var returnValue = CompileExpression(ret.Value);
          // Remove quotes if the value is already quoted (like string literals)
          // or if it's an arithmetic expression
          if ((returnValue.StartsWith("\"") && returnValue.EndsWith("\"")) ||
              (returnValue.StartsWith("$((") && returnValue.EndsWith("))")))
          {
            lines.Add($"echo {returnValue}");
          }
          else
          {
            lines.Add($"echo \"{returnValue}\"");
          }
        }
        else
        {
          lines.Add("return");
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
        // Handle boolean function calls (ArrayIsEmpty, ArrayContains, ConsoleIsSudo, etc.)
        else if (ifs.Condition is ArrayIsEmpty || ifs.Condition is ArrayContains || ifs.Condition is ConsoleIsSudoExpression || ifs.Condition is ConsolePromptYesNoExpression || ifs.Condition is OsIsInstalledExpression || ifs.Condition is ArgsHasExpression)
        {
          // For boolean functions, check if the result equals "true"
          ifCondition = $"\"{ifCondition}\" = \"true\"";
        }
        // Handle unary expressions with boolean functions
        else if (ifs.Condition is UnaryExpression unary && unary.Operator == "!" &&
                 (unary.Operand is ArrayIsEmpty || unary.Operand is ArrayContains || unary.Operand is ConsoleIsSudoExpression || unary.Operand is ConsolePromptYesNoExpression || unary.Operand is OsIsInstalledExpression || unary.Operand is ArgsHasExpression))
        {
          var operandCondition = CompileExpression(unary.Operand);
          // For negated boolean functions, check if the result equals "false"
          ifCondition = $"\"{operandCondition}\" = \"false\"";
        }
        else if (ifCondition.StartsWith("[ ") && ifCondition.EndsWith(" ]"))
        {
          ifCondition = ifCondition.Substring(2, ifCondition.Length - 4);
        }
        else if (ifCondition.StartsWith("[[ ") && ifCondition.EndsWith(" ]]"))
        {
          // For [[ ]] expressions, use them directly without wrapping in [ ]
          lines.Add($"if {ifCondition}; then");
          foreach (var b in ifs.ThenBody)
            lines.AddRange(CompileStatement(b).Select(l => "  " + l));

          // Handle else if (elif) cases for [[ ]] expressions
          if (ifs.ElseBody.Count == 1 && ifs.ElseBody[0] is IfStatement nestedElseIf)
          {
            // This is an else if - generate elif
            var nestedElifCondition = CompileExpression(nestedElseIf.Condition);

            // Handle [[ ]] elif conditions
            if (nestedElifCondition.StartsWith("[[ ") && nestedElifCondition.EndsWith(" ]]"))
            {
              lines.Add($"elif {nestedElifCondition}; then");
            }
            else
            {
              // Handle boolean variable conditions for elif
              if (nestedElseIf.Condition is VariableExpression nestedElifVarExpr)
              {
                nestedElifCondition = $"\"${{{nestedElifVarExpr.Name}}}\" = \"true\"";
              }
              else if (nestedElseIf.Condition is UnaryExpression nestedElifUnary && nestedElifUnary.Operator == "!" && nestedElifUnary.Operand is VariableExpression nestedElifNegVarExpr)
              {
                nestedElifCondition = $"\"${{{nestedElifNegVarExpr.Name}}}\" = \"false\"";
              }
              lines.Add($"elif [ {nestedElifCondition} ]; then");
            }

            foreach (var b in nestedElseIf.ThenBody)
              lines.AddRange(CompileStatement(b).Select(l => "  " + l));

            if (nestedElseIf.ElseBody.Count > 0)
            {
              lines.Add("else");
              foreach (var b in nestedElseIf.ElseBody)
                lines.AddRange(CompileStatement(b).Select(l => "  " + l));
            }
          }
          else if (ifs.ElseBody.Count > 0)
          {
            lines.Add("else");
            foreach (var b in ifs.ElseBody)
              lines.AddRange(CompileStatement(b).Select(l => "  " + l));
          }

          lines.Add("fi");
          break;
        }

        lines.Add($"if [ {ifCondition} ]; then");
        foreach (var b in ifs.ThenBody)
          lines.AddRange(CompileStatement(b).Select(l => "  " + l));

        // Handle else if (elif) cases
        if (ifs.ElseBody.Count == 1 && ifs.ElseBody[0] is IfStatement elseIf)
        {
          // This is an else if - generate elif recursively
          CompileElseIfChain(elseIf, lines);
        }
        // Only add else clause if there are statements in the else body and it's not an else if
        else if (ifs.ElseBody.Count > 0)
        {
          lines.Add("else");
          foreach (var b in ifs.ElseBody)
            lines.AddRange(CompileStatement(b).Select(l => "  " + l));
        }

        lines.Add("fi");
        break;

      case ForLoop forLoop:
        var initLine = CompileStatement(forLoop.Initializer).First();
        lines.Add(initLine);

        var condition = CompileExpression(forLoop.Condition);
        if (condition.StartsWith("[ ") && condition.EndsWith(" ]"))
        {
          condition = condition.Substring(2, condition.Length - 4);
        }
        lines.Add($"while [ {condition} ]; do");

        foreach (var bodyStmt in forLoop.Body)
          lines.AddRange(CompileStatement(bodyStmt).Select(l => "  " + l));

        // The update expression is compiled and executed.
        var updateString = CompileForLoopUpdate(forLoop.Update);
        lines.Add($"  {updateString}");

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
          lines.AddRange(CompileStatement(bodyStmt).Select(l => "  " + l));
        }
        lines.Add("done");
        break;

      case BreakStatement breakStmt:
        lines.Add("break");
        break;

      case ConsoleClearStatement:
        lines.Add("clear");
        break;

      case TimerStartStatement:
        lines.Add("_utah_timer_start=$(date +%s%3N)");
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

      case ScriptDescriptionStatement scriptDesc:
        lines.Add($"__UTAH_SCRIPT_DESCRIPTION=\"{scriptDesc.Description}\"");
        break;

      case ArgsDefineStatement argsDefine:
        CompileArgsDefineStatement(argsDefine, lines);
        break;

      case ArgsShowHelpStatement:
        lines.Add("__utah_show_help \"$@\"");
        break;

      case ForInLoop forInLoop:
        var iterableName = forInLoop.Iterable is VariableExpression varE ? varE.Name : ExtractVariableName(CompileExpression(forInLoop.Iterable));
        lines.Add($"for {forInLoop.VariableName} in \"${{{iterableName}[@]}}\"; do");

        foreach (var bodyStmt in forInLoop.Body)
          lines.AddRange(CompileStatement(bodyStmt).Select(l => "  " + l));

        lines.Add("done");
        break;

      case SwitchStatement sw:
        var switchExpr = CompileExpression(sw.Expression);
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

      case ExitStatement e:
        lines.Add($"exit {CompileExpression(e.ExitCode)}");
        break;

      case TryCatchStatement tryStmt:
        CompileTryCatchStatement(tryStmt, lines);
        break;

      case FsWriteFileStatement fws:
        var filePath = CompileExpression(fws.FilePath);
        var content = CompileExpression(fws.Content);
        lines.Add($"echo {content} > {filePath}");
        break;

      case FsCopyFileStatement fcs:
        var sourcePathStmt = CompileExpression(fcs.SourcePath);
        var targetPathStmt = CompileExpression(fcs.TargetPath);
        lines.Add($"mkdir -p $(dirname {targetPathStmt})");
        lines.Add($"cp {sourcePathStmt} {targetPathStmt}");
        break;

      case FsMoveFileStatement fms:
        var sourceMovePathStmt = CompileExpression(fms.SourcePath);
        var targetMovePathStmt = CompileExpression(fms.TargetPath);
        lines.Add($"mkdir -p $(dirname {targetMovePathStmt})");
        lines.Add($"mv {sourceMovePathStmt} {targetMovePathStmt}");
        break;

      case TemplateUpdateStatement templateUpdate:
        var sourceFile = CompileExpression(templateUpdate.SourceFilePath);
        var targetFile = CompileExpression(templateUpdate.TargetFilePath);
        lines.Add($"envsubst < {sourceFile} > {targetFile}");
        break;
    }

    return lines;
  }

  private static int _tryCatchCounter = 0;
  private static bool _inTryBlock = false;

  private void CompileTryCatchStatement(TryCatchStatement tryStmt, List<string> lines)
  {
    // Generate unique function name for this try block
    _tryCatchCounter++;
    var functionName = $"_utah_try_block_{_tryCatchCounter}";

    // Generate the try function
    lines.Add($"{functionName}() {{");
    lines.Add("  (");
    lines.Add("    set -e");

    // Set flag to indicate we're in a try block
    var wasInTryBlock = _inTryBlock;
    _inTryBlock = true;

    // Compile try block statements inside subshell
    foreach (var stmt in tryStmt.TryBody)
    {
      var stmtLines = CompileStatement(stmt);
      foreach (var stmtLine in stmtLines)
      {
        lines.Add($"    {stmtLine}");
      }
    }

    // Restore previous try block state
    _inTryBlock = wasInTryBlock;

    lines.Add("  )");
    lines.Add("}");

    // Generate the catch block
    lines.Add($"utah_catch_{_tryCatchCounter}() {{");

    if (tryStmt.CatchBody.Any())
    {
      // User defined catch behavior
      foreach (var stmt in tryStmt.CatchBody)
      {
        var stmtLines = CompileStatement(stmt);
        foreach (var stmtLine in stmtLines)
        {
          lines.Add($"  {stmtLine}");
        }
      }
    }
    else if (!string.IsNullOrEmpty(tryStmt.ErrorMessage))
    {
      // Custom error message only
      lines.Add($"  echo \"⚠️ Error: {tryStmt.ErrorMessage}\"");
    }
    else
    {
      // Default catch behavior if no catch body provided
      lines.Add($"  echo \"⚠️ Error: An error occurred\"");
    }

    lines.Add("}");

    // Generate the try/catch invocation
    lines.Add($"{functionName} || utah_catch_{_tryCatchCounter}");
  }

  private string CompileForLoopUpdate(Expression update)
  {
    return update switch
    {
      PostIncrementExpression postInc when postInc.Operand is VariableExpression varExpr =>
        $"{varExpr.Name}=$(({varExpr.Name} + 1))",
      PostDecrementExpression postDec when postDec.Operand is VariableExpression varExpr =>
        $"{varExpr.Name}=$(({varExpr.Name} - 1))",
      PreIncrementExpression preInc when preInc.Operand is VariableExpression varExpr =>
        $"{varExpr.Name}=$(({varExpr.Name} + 1))",
      PreDecrementExpression preDec when preDec.Operand is VariableExpression varExpr =>
        $"{varExpr.Name}=$(({varExpr.Name} - 1))",
      _ => CompileExpression(update)
    };
  }

  private void CompileArgsDefineStatement(ArgsDefineStatement argsDefine, List<string> lines)
  {
    // Add this argument definition to the arrays
    lines.Add($"__UTAH_ARG_NAMES+=(\"{argsDefine.LongFlag}\")");
    lines.Add($"__UTAH_ARG_SHORT_NAMES+=(\"{argsDefine.ShortFlag}\")");
    lines.Add($"__UTAH_ARG_DESCRIPTIONS+=(\"{argsDefine.Description}\")");
    lines.Add($"__UTAH_ARG_TYPES+=(\"{argsDefine.Type}\")");
    lines.Add($"__UTAH_ARG_REQUIRED+=(\"{(argsDefine.IsRequired ? "true" : "false")}\")");

    var defaultValue = "";
    if (argsDefine.DefaultValue != null)
    {
      defaultValue = CompileExpression(argsDefine.DefaultValue);
      // Remove quotes if they exist since we'll add them
      if (defaultValue.StartsWith("\"") && defaultValue.EndsWith("\""))
      {
        defaultValue = defaultValue.Substring(1, defaultValue.Length - 2);
      }
    }
    lines.Add($"__UTAH_ARG_DEFAULTS+=(\"{defaultValue}\")");
    lines.Add("");
  }

  private void CompileElseIfChain(IfStatement elseIf, List<string> lines)
  {
    // Generate elif condition
    var elifCondition = CompileExpression(elseIf.Condition);

    // Handle boolean variable conditions for elif
    if (elseIf.Condition is VariableExpression elifVarExpr)
    {
      elifCondition = $"\"${{{elifVarExpr.Name}}}\" = \"true\"";
    }
    else if (elseIf.Condition is UnaryExpression elifUnary && elifUnary.Operator == "!" && elifUnary.Operand is VariableExpression elifNegVarExpr)
    {
      elifCondition = $"\"${{{elifNegVarExpr.Name}}}\" = \"false\"";
    }
    else if (elseIf.Condition is ArrayIsEmpty || elseIf.Condition is ConsoleIsSudoExpression || elseIf.Condition is ConsolePromptYesNoExpression || elseIf.Condition is OsIsInstalledExpression)
    {
      elifCondition = $"\"{elifCondition}\" = \"true\"";
    }
    else if (elseIf.Condition is UnaryExpression elifUnary2 && elifUnary2.Operator == "!" &&
             (elifUnary2.Operand is ArrayIsEmpty || elifUnary2.Operand is ConsoleIsSudoExpression || elifUnary2.Operand is ConsolePromptYesNoExpression || elifUnary2.Operand is OsIsInstalledExpression))
    {
      var operandCondition = CompileExpression(elifUnary2.Operand);
      elifCondition = $"\"{operandCondition}\" = \"false\"";
    }
    else if (elifCondition.StartsWith("[ ") && elifCondition.EndsWith(" ]"))
    {
      elifCondition = elifCondition.Substring(2, elifCondition.Length - 4);
    }

    lines.Add($"elif [ {elifCondition} ]; then");
    foreach (var b in elseIf.ThenBody)
      lines.AddRange(CompileStatement(b).Select(l => "  " + l));

    // Recursively handle nested else if or final else
    if (elseIf.ElseBody.Count == 1 && elseIf.ElseBody[0] is IfStatement nestedElseIf)
    {
      // Continue the elif chain recursively
      CompileElseIfChain(nestedElseIf, lines);
    }
    else if (elseIf.ElseBody.Count > 0)
    {
      // Final else clause
      lines.Add("else");
      foreach (var b in elseIf.ElseBody)
        lines.AddRange(CompileStatement(b).Select(l => "  " + l));
    }
  }

  private bool IsBooleanComparison(BinaryExpression binExpr)
  {
    return binExpr.Operator switch
    {
      "==" or "===" or "!=" or "<" or "<=" or ">" or ">=" => true,
      _ => false
    };
  }

  private List<string> CompileSchedulerCronStatement(SchedulerCronExpression schedulerCron)
  {
    var cronPattern = CompileExpression(schedulerCron.CronPattern);

    // Remove quotes from cron pattern if it's a string literal
    if (cronPattern.StartsWith("\"") && cronPattern.EndsWith("\""))
    {
      cronPattern = cronPattern[1..^1];
    }

    var lines = new List<string>
    {
      "# Create Utah cron directory",
      "_utah_cron_dir=\"$HOME/.utah/cron\"",
      "mkdir -p \"${_utah_cron_dir}\"",
      $"_utah_cron_script=\"${{_utah_cron_dir}}/job_$(date +%s)_$$.sh\"",
      "",
      "# Generate the cron job script",
      "cat > \"${_utah_cron_script}\" << 'EOF'",
      "#!/bin/bash",
      $"# Generated by Utah - scheduler.cron(\"{cronPattern}\")"
    };

    // Compile the lambda body
    foreach (var stmt in schedulerCron.Job.Body)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    lines.AddRange(new[]
    {
      "EOF",
      "",
      "# Make the script executable",
      "chmod +x \"${_utah_cron_script}\"",
      "",
      "# Check if similar cron job already exists",
      $"_utah_cron_pattern=\"{cronPattern.Replace("*", "\\*")}.*utah.*job_\"",
      "if ! crontab -l 2>/dev/null | grep -q \"${_utah_cron_pattern}\"; then",
      "    # Add to crontab",
      $"    (crontab -l 2>/dev/null; echo \"{cronPattern} ${{_utah_cron_script}}\") | crontab -",
      $"    echo \"Cron job installed: {cronPattern} ${{_utah_cron_script}}\"",
      "else",
      "    echo \"Similar cron job already exists\"",
      "fi"
    });

    return lines;
  }
}
