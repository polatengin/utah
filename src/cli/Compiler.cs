public class Compiler
{
  public string Compile(ProgramNode program)
  {
    var lines = new List<string>();

    foreach (var stmt in program.Statements)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    return string.Join('\n', lines);
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
        lines.Add($"echo \"{log.Message}\"");
        break;

      case IfStatement ifs:
        lines.Add($"if [ \"$( {ifs.ConditionCall} )\" = \"true\" ]; then");
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
        lines.Add($"IFS='{sp.Delimiter}' read -ra {sp.ResultArrayName} <<< \"${{{sp.SourceString}}}\"");
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
        lines.Add($"  echo \"{log.Message}\"");
        break;
      case ReturnStatement ret:
        lines.Add($"  echo \"${ret.Value}\"");
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
      "+" when IsStringConcatenation(bin) => $"\"{left}{right}\"".Replace("\"\"", ""),
      "+" => $"$((${left} + ${right}))",
      "-" => $"$((${left} - ${right}))",
      "*" => $"$((${left} * ${right}))",
      "/" => $"$((${left} / ${right}))",
      "%" => $"$((${left} % ${right}))",
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
}
