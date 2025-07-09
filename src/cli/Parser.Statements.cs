using System.Text.RegularExpressions;

public partial class Parser
{
  // Helper method to parse individual statements recursively
  private Node? ParseStatement(string line)
  {
    line = line.Trim();

    if (line.StartsWith("console.log"))
    {
      var match = Regex.Match(line, @"console\.log\((.+)\);");
      var raw = match.Groups[1].Value.Trim();

      if (raw.StartsWith("`"))
      {
        // Template literal - use the old way
        var msg = raw[1..^1];
        return new ConsoleLog { Message = msg };
      }
      else if (raw.StartsWith("\"") && raw.EndsWith("\"") && !raw.Contains("+"))
      {
        // Simple string literal - use the old way
        var msg = raw[1..^1];
        return new ConsoleLog { Message = msg };
      }
      else
      {
        // Expression - parse it properly
        var expression = ParseExpression(raw);
        return new ConsoleLog { IsExpression = true, Expression = expression };
      }
    }
    else if (line == "console.clear();")
    {
      return new ConsoleClearStatement();
    }
    else if (line.StartsWith("let "))
    {
      // Try typed let first: let variable: type = value;
      var typedMatch = Regex.Match(line, @"let (\w+): (\w+) = (.+);");
      if (typedMatch.Success)
      {
        var value = typedMatch.Groups[3].Value;
        var envNode = ParseEnvFunction(typedMatch.Groups[1].Value, value);
        if (envNode != null)
        {
          return envNode;
        }
        else
        {
          var stringFuncNode = ParseStringFunction(typedMatch.Groups[1].Value, value);
          if (stringFuncNode != null)
          {
            return stringFuncNode;
          }
          else
          {
            var timerNode = ParseTimerFunction(typedMatch.Groups[1].Value, value);
            if (timerNode != null)
            {
              return timerNode;
            }
            else
            {
              var processNode = ParseProcessFunction(typedMatch.Groups[1].Value, value);
              if (processNode != null)
              {
                return processNode;
              }
              else
              {
                var expression = ParseExpression(value);
                return new VariableDeclarationExpression
                {
                  Name = typedMatch.Groups[1].Value,
                  Type = typedMatch.Groups[2].Value,
                  Value = expression,
                  IsConst = false
                };
              }
            }
          }
        }
      }
      else
      {
        // Try untyped let: let variable = value;
        var untypedMatch = Regex.Match(line, @"let (\w+) = (.+);");
        if (untypedMatch.Success)
        {
          var value = untypedMatch.Groups[2].Value;
          var expression = ParseExpression(value);
          return new VariableDeclarationExpression
          {
            Name = untypedMatch.Groups[1].Value,
            Type = "number", // Default type inference
            Value = expression,
            IsConst = false
          };
        }
      }
    }
    else if (line.StartsWith("const "))
    {
      var match = Regex.Match(line, @"const (\w+): (\w+) = (.+);");
      if (match.Success)
      {
        var value = match.Groups[3].Value;
        var envNode = ParseEnvFunction(match.Groups[1].Value, value);
        if (envNode != null)
        {
          return envNode;
        }
        else
        {
          var stringFuncNode = ParseStringFunction(match.Groups[1].Value, value);
          if (stringFuncNode != null)
          {
            return stringFuncNode;
          }
          else
          {
            var processNode = ParseProcessFunction(match.Groups[1].Value, value);
            if (processNode != null)
            {
              return processNode;
            }
            else
            {
              var timerNode = ParseTimerFunction(match.Groups[1].Value, value);
              if (timerNode != null)
              {
                return timerNode;
              }
              else
              {
                var expression = ParseExpression(value);
                return new VariableDeclarationExpression
                {
                  Name = match.Groups[1].Value,
                  Type = match.Groups[2].Value,
                  Value = expression,
                  IsConst = true
                };
              }
            }
          }
        }
      }
    }
    else if (line.StartsWith("exit "))
    {
      int code = int.Parse(line.Substring(5).TrimEnd(';'));
      return new ExitStatement { ExitCode = code };
    }
    else if (line.StartsWith("exit(") && line.EndsWith(");"))
    {
      var exitMatch = Regex.Match(line, @"exit\((\d+)\);");
      if (exitMatch.Success)
      {
        int code = int.Parse(exitMatch.Groups[1].Value);
        return new ExitStatement { ExitCode = code };
      }
    }
    else if (line.StartsWith("if ("))
    {
      // For nested if statements, we need to handle them specially
      return null;
    }
    else if (line.StartsWith("for ("))
    {
      // For nested loops, we need to handle them specially
      // For now, return null to indicate this needs special handling
      return null;
    }
    else if (line.StartsWith("while ("))
    {
      // For nested while loops, we need to handle them specially
      return null;
    }
    else if (line.Contains(" = ") && line.EndsWith(";"))
    {
      // Check for array assignment first (arrayName[index] = value;)
      var arrayMatch = Regex.Match(line, @"(\w+)\[(.+)\] = (.+);");
      if (arrayMatch.Success)
      {
        var arrayName = arrayMatch.Groups[1].Value;
        var indexExpr = ParseExpression(arrayMatch.Groups[2].Value);
        var valueExpr = ParseExpression(arrayMatch.Groups[3].Value);

        return new ArrayAssignment
        {
          ArrayName = arrayName,
          Index = indexExpr,
          Value = valueExpr
        };
      }

      // Handle regular variable assignment (variable = value;)
      var match = Regex.Match(line, @"(\w+) = (.+);");
      if (match.Success)
      {
        var variableName = match.Groups[1].Value;

        // Check if trying to reassign a const variable
        if (_constVariables.Contains(variableName))
        {
          throw new InvalidOperationException($"Error: Cannot reassign const variable '{variableName}'. Const variables are immutable once declared.");
        }

        var value = ParseExpression(match.Groups[2].Value);
        return new AssignmentStatement
        {
          VariableName = variableName,
          Value = value
        };
      }
    }
    else if (line == "break;")
    {
      return new BreakStatement();
    }
    else if (line.StartsWith("script."))
    {
      // Parse script function calls
      if (line == "script.enableDebug();")
      {
        return new ScriptEnableDebugStatement();
      }
      else if (line == "script.disableDebug();")
      {
        return new ScriptDisableDebugStatement();
      }
      else if (line == "script.disableGlobbing();")
      {
        return new ScriptDisableGlobbingStatement();
      }
      else if (line == "script.enableGlobbing();")
      {
        return new ScriptEnableGlobbingStatement();
      }
      else if (line == "script.exitOnError();")
      {
        return new ScriptExitOnErrorStatement();
      }
      else if (line == "script.continueOnError();")
      {
        return new ScriptContinueOnErrorStatement();
      }
    }
    else if (line.Contains("(") && line.Contains(");"))
    {
      var match = Regex.Match(line, @"([\w\.]+)\(([^)]*)\);");
      if (match.Success)
      {
        return (match.Groups[1].Value, match.Groups[2].Value) switch
        {
          ("console.isSudo", "") => new ConsoleIsSudoExpression(),
          _ => new FunctionCall
          {
            Name = match.Groups[1].Value,
            Arguments = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(a => a.Trim()).ToList()
          }
        };
      }
    }

    return null;
  }

  // Helper method to parse statements or blocks (including nested for loops)
  private Node? ParseStatementOrBlock(string line, ref int lineIndex)
  {
    line = line.Trim();

    if (line.StartsWith("for ("))
    {
      // Check if it's a for-in loop: for (let item in array) {
      var forInMatch = Regex.Match(line, @"for \((let|const) (\w+): (\w+) in (\w+)\) \{");
      if (forInMatch.Success)
      {
        var forInLoop = new ForInLoop
        {
          Variable = forInMatch.Groups[2].Value,
          VariableType = forInMatch.Groups[3].Value,
          Iterable = forInMatch.Groups[4].Value
        };

        lineIndex++;
        while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
        {
          string inner = _lines[lineIndex].Trim();
          var statement = ParseStatementOrBlock(inner, ref lineIndex);
          if (statement != null)
          {
            forInLoop.Body.Add(statement);
          }
          else
          {
            lineIndex++;
          }
        }

        // Skip over the closing brace
        lineIndex++;

        return forInLoop;
      }
      else
      {
        // Traditional for loop: for (let i: number = 0; i < 10; i++) {
        var forMatch = Regex.Match(line, @"for \((let|const) (\w+): (\w+) = (.+); (.+); (.+)\) \{");
        if (forMatch.Success)
        {
          var initValue = ParseExpression(forMatch.Groups[4].Value);
          var condition = ParseExpression(forMatch.Groups[5].Value);

          var updateExpr = forMatch.Groups[6].Value.Trim();
          string updateOp;
          Expression? updateValue = null;
          string updateVar = forMatch.Groups[2].Value;

          if (updateExpr.EndsWith("++"))
          {
            updateOp = "++";
          }
          else if (updateExpr.EndsWith("--"))
          {
            updateOp = "--";
          }
          else if (updateExpr.Contains("+="))
          {
            updateOp = "+=";
            var valueStr = updateExpr.Split("+=")[1].Trim();
            updateValue = ParseExpression(valueStr);
          }
          else if (updateExpr.Contains("-="))
          {
            updateOp = "-=";
            var valueStr = updateExpr.Split("-=")[1].Trim();
            updateValue = ParseExpression(valueStr);
          }
          else
          {
            updateOp = "++";
          }

          var forLoop = new ForLoop
          {
            InitVariable = forMatch.Groups[2].Value,
            InitType = forMatch.Groups[3].Value,
            InitValue = initValue,
            Condition = condition,
            UpdateVariable = updateVar,
            UpdateOperator = updateOp,
            UpdateValue = updateValue
          };

          lineIndex++;
          while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
          {
            string inner = _lines[lineIndex].Trim();
            var statement = ParseStatementOrBlock(inner, ref lineIndex);
            if (statement != null)
            {
              forLoop.Body.Add(statement);
            }
            else
            {
              lineIndex++;
            }
          }

          // Skip over the closing brace
          lineIndex++;

          return forLoop;
        }
      }
    }
    else if (line.StartsWith("while ("))
    {
      var match = Regex.Match(line, @"while \((.+)\) \{");
      if (match.Success)
      {
        var whileStmt = new WhileStatement
        {
          Condition = ParseExpression(match.Groups[1].Value)
        };

        lineIndex++;
        while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
        {
          var inner = _lines[lineIndex].Trim();
          if (inner == "break;")
          {
            whileStmt.Body.Add(new BreakStatement());
            lineIndex++;
          }
          else
          {
            var statement = ParseStatementOrBlock(inner, ref lineIndex);
            if (statement != null)
            {
              whileStmt.Body.Add(statement);
            }
            else
            {
              lineIndex++;
            }
          }
        }

        // Skip over the closing brace
        lineIndex++;

        return whileStmt;
      }
    }
    else if (line.StartsWith("if ("))
    {
      var match = Regex.Match(line, @"if \((.+)\) \{");
      if (match.Success)
      {
        var ifStmt = new IfStatement
        {
          Condition = ParseExpression(match.Groups[1].Value)
        };

        lineIndex++;
        while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
        {
          var inner = _lines[lineIndex].Trim();
          var statement = ParseStatementOrBlock(inner, ref lineIndex);
          if (statement != null)
          {
            ifStmt.ThenBody.Add(statement);
          }
          else
          {
            lineIndex++;
          }
        }

        // Skip over the closing brace
        lineIndex++;

        return ifStmt;
      }
    }
    else
    {
      // For single-line statements, use the existing ParseStatement method and increment lineIndex
      var statement = ParseStatement(line);
      if (statement != null)
      {
        lineIndex++;
        return statement;
      }
    }

    return null;
  }

  // Helper method to parse for loops with body parsing - needs line iterator
  private ForLoop? ParseForLoop(string line, ref int lineIndex)
  {
    var forMatch = Regex.Match(line, @"for \((let|const) (\w+): (\w+) = (.+); (.+); (.+)\) \{");
    if (!forMatch.Success) return null;

    var initValue = ParseExpression(forMatch.Groups[4].Value);
    var condition = ParseExpression(forMatch.Groups[5].Value);

    var updateExpr = forMatch.Groups[6].Value.Trim();
    string updateOp;
    Expression? updateValue = null;
    string updateVar = forMatch.Groups[2].Value;

    if (updateExpr.EndsWith("++"))
    {
      updateOp = "++";
    }
    else if (updateExpr.EndsWith("--"))
    {
      updateOp = "--";
    }
    else if (updateExpr.Contains("+="))
    {
      updateOp = "+=";
      var valueStr = updateExpr.Split("+=")[1].Trim();
      updateValue = ParseExpression(valueStr);
    }
    else if (updateExpr.Contains("-="))
    {
      updateOp = "-=";
      var valueStr = updateExpr.Split("-=")[1].Trim();
      updateValue = ParseExpression(valueStr);
    }
    else
    {
      updateOp = "++";
    }

    var forLoop = new ForLoop
    {
      InitVariable = forMatch.Groups[2].Value,
      InitType = forMatch.Groups[3].Value,
      InitValue = initValue,
      Condition = condition,
      UpdateVariable = updateVar,
      UpdateOperator = updateOp,
      UpdateValue = updateValue
    };

    lineIndex++;
    while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
    {
      string inner = _lines[lineIndex].Trim();
      var statement = ParseStatementOrBlock(inner, ref lineIndex);
      if (statement != null)
      {
        forLoop.Body.Add(statement);
      }
      else
      {
        lineIndex++;
      }
    }

    return forLoop;
  }

  // Helper method to parse for-in loops with body parsing - needs line iterator
  private ForInLoop? ParseForInLoop(string line, ref int lineIndex)
  {
    var forInMatch = Regex.Match(line, @"for \((let|const) (\w+): (\w+) in (\w+)\) \{");
    if (!forInMatch.Success) return null;

    var forInLoop = new ForInLoop
    {
      Variable = forInMatch.Groups[2].Value,
      VariableType = forInMatch.Groups[3].Value,
      Iterable = forInMatch.Groups[4].Value
    };

    lineIndex++;
    while (lineIndex < _lines.Length && !_lines[lineIndex].Trim().StartsWith("}"))
    {
      string inner = _lines[lineIndex].Trim();
      var statement = ParseStatementOrBlock(inner, ref lineIndex);
      if (statement != null)
      {
        forInLoop.Body.Add(statement);
      }
      else
      {
        lineIndex++;
      }
    }

    return forInLoop;
  }
}
