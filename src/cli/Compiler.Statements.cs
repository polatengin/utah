using System.Text.RegularExpressions;

public partial class Compiler
{
  private List<string> CompileStatement(Statement stmt)
  {
    var lines = new List<string>();

    switch (stmt)
    {
      case RawStatement raw:
        lines.Add(raw.Content);
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
          lines.AddRange(CompileStatement(b).Select(l => "  " + l));

        // Only add else clause if there are statements in the else body
        if (ifs.ElseBody.Count > 0)
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

      case FsWriteFileStatement fws:
        var filePath = CompileExpression(fws.FilePath);
        var content = CompileExpression(fws.Content);
        lines.Add($"echo {content} > {filePath}");
        break;
    }

    return lines;
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
}
