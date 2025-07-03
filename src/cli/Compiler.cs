using System.Text.RegularExpressions;

public class Compiler
{
  public string Compile(ProgramNode program)
  {
    var lines = new List<string>();

    lines.Add("#!/bin/sh");
    lines.Add(""); // Empty line after shebang

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
          lines.Add($"echo {compiledExpr}");
        }
        else
        {
          lines.Add($"echo \"{log.Message}\"");
        }
        break;

      case IfStatement ifs:
        // Check if condition is a simple variable (no parentheses) or a function call
        if (ifs.ConditionCall.Contains("(") && ifs.ConditionCall.Contains(")"))
        {
          // Function call - use command substitution
          lines.Add($"if [ \"$( {ifs.ConditionCall} )\" = \"true\" ]; then");
        }
        else
        {
          // Simple variable - use direct variable reference
          lines.Add($"if [ \"${{{ifs.ConditionCall}}}\" = \"true\" ]; then");
        }
        foreach (var b in ifs.ThenBody)
          lines.AddRange(CompileBlock(b));
        lines.Add("else");
        foreach (var b in ifs.ElseBody)
          lines.AddRange(CompileBlock(b));
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

      case EnvGet eg:
        lines.Add($"{eg.AssignTo}=\"${{{eg.VariableName}:-{eg.DefaultValue}}}\"");
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
          lines.Add($"  echo {compiledExpr}");
        }
        else
        {
          lines.Add($"  echo \"{log.Message}\"");
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
      _ => throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported")
    };
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
    return $"${var.Name}";
  }

  private string CompileBinaryExpression(BinaryExpression bin)
  {
    var left = CompileExpression(bin.Left);
    var right = CompileExpression(bin.Right);

    return bin.Operator switch
    {
      "+" when IsStringConcatenation(bin) => $"\"{left.Trim('\"')}{right.Trim('\"')}\"",
      "+" => $"$(({left.TrimStart('$')} + {right.TrimStart('$')}))",
      "-" => $"$(({left.TrimStart('$')} - {right.TrimStart('$')}))",
      "*" => $"$(({left.TrimStart('$')} * {right.TrimStart('$')}))",
      "/" => $"$(({left.TrimStart('$')} / {right.TrimStart('$')}))",
      "%" => $"$(({left.TrimStart('$')} % {right.TrimStart('$')}))",
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
    var arrayName = CompileExpression(acc.Array).TrimStart('$'); // Remove $ prefix if present
    var index = CompileExpression(acc.Index);

    // In bash, array access is ${arrayName[index]}
    return $"\"${{{arrayName}[{index.TrimStart('$')}]}}\"";
  }

  private string CompileArrayLength(ArrayLength len)
  {
    var arrayName = CompileExpression(len.Array).TrimStart('$'); // Remove $ prefix if present

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
}
