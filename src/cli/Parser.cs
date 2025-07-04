using System.Text.RegularExpressions;

public class Parser
{
  private readonly string[] _lines;
  private readonly HashSet<string> _constVariables = new HashSet<string>();

  public Parser(string input)
  {
    _lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
  }

  public ProgramNode Parse()
  {
    var program = new ProgramNode();

    for (int i = 0; i < _lines.Length; i++)
    {
      var line = _lines[i].Trim();

      if (line.StartsWith("let "))
      {
        var match = Regex.Match(line, @"let (\w+): ([\w\[\]]+) = (.+);");
        if (match.Success)
        {
          var value = match.Groups[3].Value;
          var envNode = ParseEnvFunction(match.Groups[1].Value, value);
          if (envNode != null)
          {
            program.Statements.Add(envNode);
          }
          else
          {
            var osNode = ParseOsFunction(match.Groups[1].Value, value);
            if (osNode != null)
            {
              program.Statements.Add(osNode);
            }
            else
            {
              var fsNode = ParseFsFunction(match.Groups[1].Value, value);
              if (fsNode != null)
              {
                program.Statements.Add(fsNode);
              }
              else
              {
                var stringFuncNode = ParseStringFunction(match.Groups[1].Value, value);
                if (stringFuncNode != null)
                {
                  program.Statements.Add(stringFuncNode);
                }
                else
                {
                  var processNode = ParseProcessFunction(match.Groups[1].Value, value);
                  if (processNode != null)
                  {
                    program.Statements.Add(processNode);
                  }
                  else
                  {
                    var timerNode = ParseTimerFunction(match.Groups[1].Value, value);
                    if (timerNode != null)
                    {
                      program.Statements.Add(timerNode);
                    }
                    else
                    {
                      var declaredType = match.Groups[2].Value;
                      var expression = ParseExpression(value);

                      // Validate array type if it's an array declaration
                      if (declaredType.EndsWith("[]") && expression is ArrayLiteral arrayLiteral)
                      {
                        var expectedElementType = declaredType.Substring(0, declaredType.Length - 2);
                        ValidateArrayElementTypes(arrayLiteral, expectedElementType, match.Groups[1].Value);
                      }

                      program.Statements.Add(new VariableDeclarationExpression
                      {
                        Name = match.Groups[1].Value,
                        Type = declaredType,
                        Value = expression,
                        IsConst = false
                      });
                    }
                  }
                }
              }
            }
          }
        }
      }
      else if (line.StartsWith("const "))
      {
        var match = Regex.Match(line, @"const (\w+): ([\w\[\]]+) = (.+);");
        if (match.Success)
        {
          var varName = match.Groups[1].Value;
          _constVariables.Add(varName); // Track const variable
          var value = match.Groups[3].Value;
          var envNode = ParseEnvFunction(varName, value);
          if (envNode != null)
          {
            program.Statements.Add(envNode);
          }
          else
          {
            var osNode = ParseOsFunction(varName, value);
            if (osNode != null)
            {
              program.Statements.Add(osNode);
            }
            else
            {
              var stringFuncNode = ParseStringFunction(varName, value);
              if (stringFuncNode != null)
              {
                program.Statements.Add(stringFuncNode);
              }
              else
              {
                var processNode = ParseProcessFunction(varName, value);
                if (processNode != null)
                {
                  program.Statements.Add(processNode);
                }
                else
                {
                  var timerNode = ParseTimerFunction(varName, value);
                  if (timerNode != null)
                  {
                    program.Statements.Add(timerNode);
                  }
                  else
                  {
                    var declaredType = match.Groups[2].Value;
                    var expression = ParseExpression(value);

                    // Validate array type if it's an array declaration
                    if (declaredType.EndsWith("[]") && expression is ArrayLiteral arrayLiteral)
                    {
                      var expectedElementType = declaredType.Substring(0, declaredType.Length - 2);
                      ValidateArrayElementTypes(arrayLiteral, expectedElementType, varName);
                    }

                    program.Statements.Add(new VariableDeclarationExpression
                    {
                      Name = varName,
                      Type = declaredType,
                      Value = expression,
                      IsConst = true
                    });
                  }
                }
              }
            }
          }
        }
      }
      else if (line.StartsWith("function "))
      {
        var headerMatch = Regex.Match(line, @"function (\w+)\(([^)]*)\): \w+ \{");
        var func = new FunctionDeclaration
        {
          Name = headerMatch.Groups[1].Value
        };

        var paramList = headerMatch.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var p in paramList)
        {
          var parts = p.Trim().Split(':');
          func.Parameters.Add((parts[0].Trim(), parts[1].Trim()));
        }

        i++;
        while (!_lines[i].Trim().StartsWith("}"))
        {
          string inner = _lines[i].Trim();

          if (inner.StartsWith("let "))
          {
            var match = Regex.Match(inner, @"let (\w+): (\w+) = (.+);");
            if (match.Success)
            {
              var value = match.Groups[3].Value;
              var stringFuncNode = ParseStringFunction(match.Groups[1].Value, value);
              if (stringFuncNode != null)
              {
                func.Body.Add(stringFuncNode);
              }
              else
              {
                func.Body.Add(new VariableDeclaration
                {
                  Name = match.Groups[1].Value,
                  Type = match.Groups[2].Value,
                  Value = value.Trim('"'),
                  IsConst = false
                });
              }
            }
          }
          else if (inner.StartsWith("const "))
          {
            var match = Regex.Match(inner, @"const (\w+): (\w+) = (.+);");
            if (match.Success)
            {
              var varName = match.Groups[1].Value;
              _constVariables.Add(varName); // Track const variable
              var value = match.Groups[3].Value;
              var stringFuncNode = ParseStringFunction(varName, value);
              if (stringFuncNode != null)
              {
                func.Body.Add(stringFuncNode);
              }
              else
              {
                func.Body.Add(new VariableDeclaration
                {
                  Name = varName,
                  Type = match.Groups[2].Value,
                  Value = value.Trim('"'),
                  IsConst = true
                });
              }
            }
          }
          else if (inner.StartsWith("console.log"))
          {
            var match = Regex.Match(inner, @"console\.log\((.+)\);");
            var raw = match.Groups[1].Value.Trim();

            // Parse as expression
            var expression = ParseExpression(raw);
            func.Body.Add(new ConsoleLog
            {
              IsExpression = true,
              Expression = expression
            });
          }
          else if (inner.StartsWith("return "))
          {
            var val = inner.Substring(7).TrimEnd(';');
            func.Body.Add(new ReturnStatement { Value = ParseExpression(val) });
          }
          else if (inner.StartsWith("exit "))
          {
            int code = int.Parse(inner.Substring(5).TrimEnd(';'));
            func.Body.Add(new ExitStatement { ExitCode = code });
          }
          else if (inner.StartsWith("exit(") && inner.EndsWith(");"))
          {
            var exitMatch = Regex.Match(inner, @"exit\((\d+)\);");
            if (exitMatch.Success)
            {
              int code = int.Parse(exitMatch.Groups[1].Value);
              func.Body.Add(new ExitStatement { ExitCode = code });
            }
          }

          i++;
        }

        program.Statements.Add(func);
      }
      else if (line.StartsWith("if ("))
      {
        var match = Regex.Match(line, @"if \((.+)\) \{");
        var cond = match.Groups[1].Value;
        var ifStmt = new IfStatement { ConditionCall = cond };

        i++;
        while (!_lines[i].Trim().StartsWith("}"))
        {
          string inner = _lines[i].Trim();
          if (inner.StartsWith("console.log"))
          {
            var m = Regex.Match(inner, @"console\.log\((.+)\);");
            var raw = m.Groups[1].Value.Trim();

            if (raw.StartsWith("`"))
            {
              // Template literal - use the old way
              var msg = raw[1..^1];
              ifStmt.ThenBody.Add(new ConsoleLog { Message = msg });
            }
            else if (raw.StartsWith("\"") && raw.EndsWith("\"") && !raw.Contains("+"))
            {
              // Simple string literal - use the old way
              var msg = raw[1..^1];
              ifStmt.ThenBody.Add(new ConsoleLog { Message = msg });
            }
            else
            {
              // Expression - parse it properly
              var expression = ParseExpression(raw);
              ifStmt.ThenBody.Add(new ConsoleLog { IsExpression = true, Expression = expression });
            }
          }
          else if (inner.StartsWith("let "))
          {
            var m = Regex.Match(inner, @"let (\w+): (\w+) = (.+);");
            if (m.Success)
            {
              var value = m.Groups[3].Value;
              var stringFuncNode = ParseStringFunction(m.Groups[1].Value, value);
              if (stringFuncNode != null)
              {
                ifStmt.ThenBody.Add(stringFuncNode);
              }
              else
              {
                ifStmt.ThenBody.Add(new VariableDeclaration
                {
                  Name = m.Groups[1].Value,
                  Type = m.Groups[2].Value,
                  Value = value.Trim('"'),
                  IsConst = false
                });
              }
            }
          }
          else if (inner.StartsWith("const "))
          {
            var m = Regex.Match(inner, @"const (\w+): (\w+) = (.+);");
            if (m.Success)
            {
              var value = m.Groups[3].Value;
              var stringFuncNode = ParseStringFunction(m.Groups[1].Value, value);
              if (stringFuncNode != null)
              {
                ifStmt.ThenBody.Add(stringFuncNode);
              }
              else
              {
                ifStmt.ThenBody.Add(new VariableDeclaration
                {
                  Name = m.Groups[1].Value,
                  Type = m.Groups[2].Value,
                  Value = value.Trim('"'),
                  IsConst = true
                });
              }
            }
          }
          else if (inner.StartsWith("exit "))
          {
            int code = int.Parse(inner.Substring(5).TrimEnd(';'));
            ifStmt.ThenBody.Add(new ExitStatement { ExitCode = code });
          }
          else if (inner.StartsWith("exit(") && inner.EndsWith(");"))
          {
            var exitMatch = Regex.Match(inner, @"exit\((\d+)\);");
            if (exitMatch.Success)
            {
              int code = int.Parse(exitMatch.Groups[1].Value);
              ifStmt.ThenBody.Add(new ExitStatement { ExitCode = code });
            }
          }
          i++;
        }

        // Check if we have an else clause
        var currentLine = _lines[i].Trim();
        if (currentLine == "} else {" || currentLine == "else {")
        {
          i++;
          while (!_lines[i].Trim().StartsWith("}"))
          {
            string inner = _lines[i].Trim();
            if (inner.StartsWith("console.log"))
            {
              var m = Regex.Match(inner, @"console\.log\((.+)\);");
              var raw = m.Groups[1].Value.Trim();

              if (raw.StartsWith("`"))
              {
                // Template literal - use the old way
                var msg = raw[1..^1];
                ifStmt.ElseBody.Add(new ConsoleLog { Message = msg });
              }
              else if (raw.StartsWith("\"") && raw.EndsWith("\"") && !raw.Contains("+"))
              {
                // Simple string literal - use the old way
                var msg = raw[1..^1];
                ifStmt.ElseBody.Add(new ConsoleLog { Message = msg });
              }
              else
              {
                // Expression - parse it properly
                var expression = ParseExpression(raw);
                ifStmt.ElseBody.Add(new ConsoleLog { IsExpression = true, Expression = expression });
              }
            }
            else if (inner.StartsWith("exit "))
            {
              int code = int.Parse(inner.Substring(5).TrimEnd(';'));
              ifStmt.ElseBody.Add(new ExitStatement { ExitCode = code });
            }
            else if (inner.StartsWith("exit(") && inner.EndsWith(");"))
            {
              var exitMatch = Regex.Match(inner, @"exit\((\d+)\);");
              if (exitMatch.Success)
              {
                int code = int.Parse(exitMatch.Groups[1].Value);
                ifStmt.ElseBody.Add(new ExitStatement { ExitCode = code });
              }
            }
            i++;
          }
        }

        program.Statements.Add(ifStmt);
      }
      else if (line.StartsWith("for ("))
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

          i++;
          while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
          {
            string inner = _lines[i].Trim();
            var statement = ParseStatementOrBlock(inner, ref i);
            if (statement != null)
            {
              forInLoop.Body.Add(statement);
            }
            else
            {
              i++;
            }
          }

          program.Statements.Add(forInLoop);
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

            i++;
            while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
            {
              string inner = _lines[i].Trim();
              var statement = ParseStatementOrBlock(inner, ref i);
              if (statement != null)
              {
                forLoop.Body.Add(statement);
              }
              else
              {
                i++;
              }
            }

            program.Statements.Add(forLoop);
          }
        }
      }
      else if (line.StartsWith("exit "))
      {
        int code = int.Parse(line.Substring(5).TrimEnd(';'));
        program.Statements.Add(new ExitStatement { ExitCode = code });
      }
      else if (line.StartsWith("exit(") && line.EndsWith(");"))
      {
        var exitMatch = Regex.Match(line, @"exit\((\d+)\);");
        if (exitMatch.Success)
        {
          int code = int.Parse(exitMatch.Groups[1].Value);
          program.Statements.Add(new ExitStatement { ExitCode = code });
        }
      }
      else if (line.StartsWith("console.log"))
      {
        var match = Regex.Match(line, @"console\.log\((.+)\);");
        var raw = match.Groups[1].Value.Trim();

        if (raw.StartsWith("`"))
        {
          // Template literal
          var msg = raw[1..^1];
          program.Statements.Add(new ConsoleLog { Message = msg, IsExpression = false });
        }
        else if (raw.StartsWith("\"") && raw.EndsWith("\""))
        {
          // String literal
          var msg = raw[1..^1];
          program.Statements.Add(new ConsoleLog { Message = msg, IsExpression = false });
        }
        else
        {
          // Parse as expression
          var expr = ParseExpression(raw);
          program.Statements.Add(new ConsoleLog { Expression = expr, IsExpression = true });
        }
      }
      else if (line.StartsWith("env."))
      {
        // Parse standalone env function calls
        var envNode = ParseStandaloneEnvFunction(line);
        if (envNode != null)
        {
          program.Statements.Add(envNode);
        }
      }
      else if (line.StartsWith("fs."))
      {
        // Parse standalone fs function calls
        var fsNode = ParseStandaloneFsFunction(line);
        if (fsNode != null)
        {
          program.Statements.Add(fsNode);
        }
      }
      else if (line.StartsWith("timer."))
      {
        // Parse standalone timer function calls
        var timerNode = ParseStandaloneTimerFunction(line);
        if (timerNode != null)
        {
          program.Statements.Add(timerNode);
        }
      }
      else if (line.StartsWith("switch ("))
      {
        var match = Regex.Match(line, @"switch \((.+)\) \{");
        if (match.Success)
        {
          var switchExpr = ParseExpression(match.Groups[1].Value);
          var switchStmt = new SwitchStatement
          {
            SwitchExpression = switchExpr
          };

          i++;
          CaseClause? currentCase = null;

          while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
          {
            var currentLine = _lines[i].Trim();

            if (currentLine.StartsWith("case "))
            {
              // If we have a previous case and it's a fall-through (no break), add this case value to it
              var caseMatch = Regex.Match(currentLine, @"case (.+):");
              if (caseMatch.Success)
              {
                var caseValue = ParseExpression(caseMatch.Groups[1].Value);

                // If we have an existing case without a break, add this as a fall-through
                if (currentCase != null && !currentCase.HasBreak)
                {
                  currentCase.Values.Add(caseValue);
                }
                else
                {
                  // Start a new case
                  currentCase = new CaseClause();
                  currentCase.Values.Add(caseValue);
                  switchStmt.Cases.Add(currentCase);
                }
              }
            }
            else if (currentLine.StartsWith("default:"))
            {
              currentCase = null; // End current case
              var defaultClause = new DefaultClause();

              i++;
              while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
              {
                var bodyLine = _lines[i].Trim();
                if (bodyLine == "break;")
                {
                  defaultClause.HasBreak = true;
                  i++;
                  break;
                }
                else if (!string.IsNullOrEmpty(bodyLine))
                {
                  var bodyNode = ParseStatement(bodyLine);
                  if (bodyNode != null)
                  {
                    defaultClause.Body.Add(bodyNode);
                  }
                }
                i++;
              }

              switchStmt.DefaultCase = defaultClause;
              i--; // Adjust for the outer loop increment
            }
            else if (currentLine == "break;")
            {
              if (currentCase != null)
              {
                currentCase.HasBreak = true;
                currentCase = null; // End current case
              }
            }
            else if (!string.IsNullOrEmpty(currentLine) && currentCase != null)
            {
              // Add statement to current case body
              var bodyNode = ParseStatement(currentLine);
              if (bodyNode != null)
              {
                currentCase.Body.Add(bodyNode);
              }
            }

            i++;
          }

          program.Statements.Add(switchStmt);
        }
      }
      else if (line.Contains(" = ") && line.EndsWith(";"))
      {
        // Handle variable assignment (variable = value;)
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
          program.Statements.Add(new AssignmentStatement
          {
            VariableName = variableName,
            Value = value
          });
        }
      }
      else if (line.Contains("(") && line.Contains(");"))
      {
        var match = Regex.Match(line, @"(\w+)\(([^)]*)\);");
        program.Statements.Add(new FunctionCall
        {
          Name = match.Groups[1].Value,
          Arguments = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim()).ToList()
        });
      }
    }

    return program;
  }

  private StringFunction? ParseStringFunction(string targetVariable, string expression)
  {
    // Parse string.length() -> StringLength
    var lengthMatch = Regex.Match(expression, @"(\w+)\.length\(\)");
    if (lengthMatch.Success)
    {
      return new StringLength
      {
        TargetVariable = targetVariable,
        SourceString = lengthMatch.Groups[1].Value
      };
    }

    // Parse string.slice(start, end?) -> StringSlice
    var sliceMatch = Regex.Match(expression, @"(\w+)\.slice\((\d+)(?:,\s*(\d+))?\)");
    if (sliceMatch.Success)
    {
      var endIndex = sliceMatch.Groups[3].Success ? int.Parse(sliceMatch.Groups[3].Value) : (int?)null;
      return new StringSlice
      {
        TargetVariable = targetVariable,
        SourceString = sliceMatch.Groups[1].Value,
        StartIndex = int.Parse(sliceMatch.Groups[2].Value),
        EndIndex = endIndex
      };
    }

    // Parse string.replace(search, replace) -> StringReplace
    var replaceMatch = Regex.Match(expression, @"(\w+)\.replace\(""([^""]+)"",\s*""([^""]*)""\)");
    if (replaceMatch.Success)
    {
      return new StringReplace
      {
        TargetVariable = targetVariable,
        SourceString = replaceMatch.Groups[1].Value,
        SearchPattern = replaceMatch.Groups[2].Value,
        ReplaceWith = replaceMatch.Groups[3].Value,
        ReplaceAll = false
      };
    }

    // Parse string.replaceAll(search, replace) -> StringReplace
    var replaceAllMatch = Regex.Match(expression, @"(\w+)\.replaceAll\(""([^""]+)"",\s*""([^""]*)""\)");
    if (replaceAllMatch.Success)
    {
      return new StringReplace
      {
        TargetVariable = targetVariable,
        SourceString = replaceAllMatch.Groups[1].Value,
        SearchPattern = replaceAllMatch.Groups[2].Value,
        ReplaceWith = replaceAllMatch.Groups[3].Value,
        ReplaceAll = true
      };
    }

    // Parse string.toUpperCase() -> StringToUpper
    var upperMatch = Regex.Match(expression, @"(\w+)\.toUpperCase\(\)");
    if (upperMatch.Success)
    {
      return new StringToUpper
      {
        TargetVariable = targetVariable,
        SourceString = upperMatch.Groups[1].Value
      };
    }

    // Parse string.toLowerCase() -> StringToLower
    var lowerMatch = Regex.Match(expression, @"(\w+)\.toLowerCase\(\)");
    if (lowerMatch.Success)
    {
      return new StringToLower
      {
        TargetVariable = targetVariable,
        SourceString = lowerMatch.Groups[1].Value
      };
    }

    // Parse string.trim() -> StringTrim
    var trimMatch = Regex.Match(expression, @"(\w+)\.trim\(\)");
    if (trimMatch.Success)
    {
      return new StringTrim
      {
        TargetVariable = targetVariable,
        SourceString = trimMatch.Groups[1].Value
      };
    }

    // Parse string.startsWith("prefix") -> StringStartsWith
    var startsWithMatch = Regex.Match(expression, @"(\w+)\.startsWith\(""([^""]+)""\)");
    if (startsWithMatch.Success)
    {
      return new StringStartsWith
      {
        TargetVariable = targetVariable,
        SourceString = startsWithMatch.Groups[1].Value,
        Prefix = startsWithMatch.Groups[2].Value
      };
    }

    // Parse string.endsWith("suffix") -> StringEndsWith
    var endsWithMatch = Regex.Match(expression, @"(\w+)\.endsWith\(""([^""]+)""\)");
    if (endsWithMatch.Success)
    {
      return new StringEndsWith
      {
        TargetVariable = targetVariable,
        SourceString = endsWithMatch.Groups[1].Value,
        Suffix = endsWithMatch.Groups[2].Value
      };
    }

    // Parse string.includes("substring") -> StringContains
    var includesMatch = Regex.Match(expression, @"(\w+)\.includes\(""([^""]+)""\)");
    if (includesMatch.Success)
    {
      return new StringContains
      {
        TargetVariable = targetVariable,
        SourceString = includesMatch.Groups[1].Value,
        Substring = includesMatch.Groups[2].Value
      };
    }

    // Parse string.split("delimiter") -> StringSplit
    var splitMatch = Regex.Match(expression, @"(\w+)\.split\(""([^""]*)""\)");
    if (splitMatch.Success)
    {
      return new StringSplit
      {
        TargetVariable = targetVariable,
        SourceString = splitMatch.Groups[1].Value,
        Delimiter = splitMatch.Groups[2].Value,
        ResultArrayName = targetVariable
      };
    }

    // Parse string literal.split("delimiter") -> StringSplit
    var literalSplitMatch = Regex.Match(expression, @"""([^""]*)""\.split\(""([^""]*)""\)");
    if (literalSplitMatch.Success)
    {
      return new StringSplit
      {
        TargetVariable = targetVariable,
        SourceString = literalSplitMatch.Groups[1].Value,
        Delimiter = literalSplitMatch.Groups[2].Value,
        ResultArrayName = targetVariable
      };
    }

    return null;
  }

  private Node? ParseEnvFunction(string targetVariable, string expression)
  {
    // Parse env.get("VAR_NAME", "default") -> EnvGet
    var envGetMatch = Regex.Match(expression, @"env\.get\(""([^""]+)"",\s*""([^""]*)""\)");
    if (envGetMatch.Success)
    {
      return new EnvGet
      {
        AssignTo = targetVariable,
        VariableName = envGetMatch.Groups[1].Value,
        DefaultValue = envGetMatch.Groups[2].Value
      };
    }

    // Parse boolean expressions like env.get("DEBUG", "false") === "true"
    var boolEnvMatch = Regex.Match(expression, @"env\.get\(""([^""]+)"",\s*""([^""]*)""\)\s*===\s*""([^""]*)""");
    if (boolEnvMatch.Success)
    {
      // For boolean expressions, we'll create a special variable assignment
      return new VariableDeclaration
      {
        Name = targetVariable,
        Type = "boolean",
        Value = $"[ \"${{{boolEnvMatch.Groups[1].Value}:-{boolEnvMatch.Groups[2].Value}}}\" = \"{boolEnvMatch.Groups[3].Value}\" ]",
        IsConst = false
      };
    }

    return null;
  }

  private Node? ParseStandaloneEnvFunction(string line)
  {
    // Parse env.set("VAR_NAME", "value");
    var envSetMatch = Regex.Match(line, @"env\.set\(""([^""]+)"",\s*""([^""]*)""\);");
    if (envSetMatch.Success)
    {
      return new EnvSet
      {
        VariableName = envSetMatch.Groups[1].Value,
        Value = envSetMatch.Groups[2].Value
      };
    }

    // Parse env.load("filepath");
    var envLoadMatch = Regex.Match(line, @"env\.load\(""([^""]+)""\);");
    if (envLoadMatch.Success)
    {
      return new EnvLoad
      {
        FilePath = envLoadMatch.Groups[1].Value
      };
    }

    // Parse env.delete("VAR_NAME");
    var envDeleteMatch = Regex.Match(line, @"env\.delete\(""([^""]+)""\);");
    if (envDeleteMatch.Success)
    {
      return new EnvDelete
      {
        VariableName = envDeleteMatch.Groups[1].Value
      };
    }

    return null;
  }

  private Node? ParseOsFunction(string targetVariable, string expression)
  {
    // Parse os.isInstalled("app_name") -> OsIsInstalled
    var osIsInstalledMatch = Regex.Match(expression, @"os\.isInstalled\(""([^""]+)""\)");
    if (osIsInstalledMatch.Success)
    {
      return new OsIsInstalled
      {
        AppName = osIsInstalledMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse os.getOS() -> OsGetOS
    if (expression.Trim() == "os.getOS()")
    {
      return new OsGetOS { AssignTo = targetVariable };
    }

    // Parse os.getLinuxVersion() -> OsGetLinuxVersion
    if (expression.Trim() == "os.getLinuxVersion()")
    {
      return new OsGetLinuxVersion { AssignTo = targetVariable };
    }

    return null;
  }

  private Node? ParseStandaloneFsFunction(string line)
  {
    // Parse fs.writeFile("filepath", "content"); (string literal)
    var fsWriteStringMatch = Regex.Match(line, @"fs\.writeFile\(""([^""]+)"",\s*""([^""]*)""\);");
    if (fsWriteStringMatch.Success)
    {
      return new FsWriteFile
      {
        FilePath = fsWriteStringMatch.Groups[1].Value,
        Content = fsWriteStringMatch.Groups[2].Value
      };
    }

    // Parse fs.writeFile("filepath", variableName); (variable)
    var fsWriteVarMatch = Regex.Match(line, @"fs\.writeFile\(""([^""]+)"",\s*(\w+)\);");
    if (fsWriteVarMatch.Success)
    {
      return new FsWriteFile
      {
        FilePath = fsWriteVarMatch.Groups[1].Value,
        Content = $"${{{fsWriteVarMatch.Groups[2].Value}}}" // Mark as variable reference
      };
    }

    return null;
  }

  private Node? ParseFsFunction(string targetVariable, string expression)
  {
    // Parse fs.readFile("filepath") -> FsReadFile
    var fsReadMatch = Regex.Match(expression, @"fs\.readFile\(""([^""]+)""\)");
    if (fsReadMatch.Success)
    {
      return new FsReadFile
      {
        FilePath = fsReadMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.dirname("filepath") -> FsDirname
    var fsDirnameMatch = Regex.Match(expression, @"fs\.dirname\(""([^""]+)""\)");
    if (fsDirnameMatch.Success)
    {
      return new FsDirname
      {
        FilePath = fsDirnameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.parentDirName("filepath") -> FsParentDirName
    var fsParentDirNameMatch = Regex.Match(expression, @"fs\.parentDirName\(""([^""]+)""\)");
    if (fsParentDirNameMatch.Success)
    {
      return new FsParentDirName
      {
        FilePath = fsParentDirNameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.extension("filepath") -> FsExtension
    var fsExtensionMatch = Regex.Match(expression, @"fs\.extension\(""([^""]+)""\)");
    if (fsExtensionMatch.Success)
    {
      return new FsExtension
      {
        FilePath = fsExtensionMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.fileName("filepath") -> FsFileName
    var fsFileNameMatch = Regex.Match(expression, @"fs\.fileName\(""([^""]+)""\)");
    if (fsFileNameMatch.Success)
    {
      return new FsFileName
      {
        FilePath = fsFileNameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    return null;
  }

  public Expression ParseExpression(string input)
  {
    return ParseTernaryExpression(input.Trim());
  }

  private Expression ParseTernaryExpression(string input)
  {
    // Check for ternary operator (condition ? true : false)
    var parts = SplitTernary(input);
    if (parts.Count == 3)
    {
      return new TernaryExpression
      {
        Condition = ParseLogicalOrExpression(parts[0]),
        TrueExpression = ParseLogicalOrExpression(parts[1]),
        FalseExpression = ParseLogicalOrExpression(parts[2])
      };
    }
    return ParseLogicalOrExpression(input);
  }

  private Expression ParseLogicalOrExpression(string input)
  {
    var parts = SplitByOperator(input, "||");
    if (parts.Count > 1)
    {
      var left = ParseLogicalAndExpression(parts[0]);
      for (int i = 1; i < parts.Count; i++)
      {
        left = new BinaryExpression
        {
          Left = left,
          Operator = "||",
          Right = ParseLogicalAndExpression(parts[i])
        };
      }
      return left;
    }
    return ParseLogicalAndExpression(input);
  }

  private Expression ParseLogicalAndExpression(string input)
  {
    var parts = SplitByOperator(input, "&&");
    if (parts.Count > 1)
    {
      var left = ParseEqualityExpression(parts[0]);
      for (int i = 1; i < parts.Count; i++)
      {
        left = new BinaryExpression
        {
          Left = left,
          Operator = "&&",
          Right = ParseEqualityExpression(parts[i])
        };
      }
      return left;
    }
    return ParseEqualityExpression(input);
  }

  private Expression ParseEqualityExpression(string input)
  {
    var operators = new[] { "==", "!=" };
    foreach (var op in operators)
    {
      var parts = SplitByOperator(input, op);
      if (parts.Count == 2)
      {
        return new BinaryExpression
        {
          Left = ParseRelationalExpression(parts[0]),
          Operator = op,
          Right = ParseRelationalExpression(parts[1])
        };
      }
    }
    return ParseRelationalExpression(input);
  }

  private Expression ParseRelationalExpression(string input)
  {
    var operators = new[] { "<=", ">=", "<", ">" };
    foreach (var op in operators)
    {
      var parts = SplitByOperator(input, op);
      if (parts.Count == 2)
      {
        return new BinaryExpression
        {
          Left = ParseAdditiveExpression(parts[0]),
          Operator = op,
          Right = ParseAdditiveExpression(parts[1])
        };
      }
    }
    return ParseAdditiveExpression(input);
  }

  private Expression ParseAdditiveExpression(string input)
  {
    var parts = SplitByOperator(input, "+", "-");
    if (parts.Count > 1)
    {
      var left = ParseMultiplicativeExpression(parts[0]);
      for (int i = 1; i < parts.Count; i++)
      {
        var op = GetOperatorBetween(input, parts[i - 1], parts[i]);
        left = new BinaryExpression
        {
          Left = left,
          Operator = op,
          Right = ParseMultiplicativeExpression(parts[i])
        };
      }
      return left;
    }
    return ParseMultiplicativeExpression(input);
  }

  private Expression ParseMultiplicativeExpression(string input)
  {
    var parts = SplitByOperator(input, "*", "/", "%");
    if (parts.Count > 1)
    {
      var left = ParseUnaryExpression(parts[0]);
      for (int i = 1; i < parts.Count; i++)
      {
        var op = GetOperatorBetween(input, parts[i - 1], parts[i]);
        left = new BinaryExpression
        {
          Left = left,
          Operator = op,
          Right = ParseUnaryExpression(parts[i])
        };
      }
      return left;
    }
    return ParseUnaryExpression(input);
  }

  private Expression ParseUnaryExpression(string input)
  {
    if (input.StartsWith("!"))
    {
      return new UnaryExpression
      {
        Operator = "!",
        Operand = ParseUnaryExpression(input.Substring(1).Trim())
      };
    }
    if (input.StartsWith("-"))
    {
      return new UnaryExpression
      {
        Operator = "-",
        Operand = ParseUnaryExpression(input.Substring(1).Trim())
      };
    }
    return ParsePrimaryExpression(input);
  }

  private Expression ParsePrimaryExpression(string input)
  {
    input = input.Trim();

    // Parenthesized expression
    if (input.StartsWith("(") && input.EndsWith(")"))
    {
      return new ParenthesizedExpression
      {
        Inner = ParseExpression(input.Substring(1, input.Length - 2))
      };
    }

    // String literal
    if (input.StartsWith("\"") && input.EndsWith("\""))
    {
      return new LiteralExpression
      {
        Value = input.Substring(1, input.Length - 2),
        Type = "string"
      };
    }

    // Number literal
    if (double.TryParse(input, out _))
    {
      return new LiteralExpression
      {
        Value = input,
        Type = "number"
      };
    }

    // Boolean literal
    if (input == "true" || input == "false")
    {
      return new LiteralExpression
      {
        Value = input,
        Type = "boolean"
      };
    }

    // Array literal: [1, 2, 3] or ["a", "b", "c"]
    if (input.StartsWith("[") && input.EndsWith("]"))
    {
      var content = input.Substring(1, input.Length - 2).Trim();
      var arrayLiteral = new ArrayLiteral();

      if (!string.IsNullOrEmpty(content))
      {
        var elements = SplitByComma(content);
        foreach (var element in elements)
        {
          var elementExpr = ParseExpression(element.Trim());
          arrayLiteral.Elements.Add(elementExpr);

          // Determine element type from first element
          if (arrayLiteral.ElementType == string.Empty && elementExpr is LiteralExpression literal)
          {
            arrayLiteral.ElementType = literal.Type;
          }
        }
      }

      return arrayLiteral;
    }

    // Array access: arrayName[index]
    if (input.Contains("[") && input.EndsWith("]"))
    {
      var bracketIndex = input.IndexOf('[');
      var arrayName = input.Substring(0, bracketIndex).Trim();
      var indexContent = input.Substring(bracketIndex + 1, input.Length - bracketIndex - 2).Trim();

      return new ArrayAccess
      {
        Array = new VariableExpression { Name = arrayName },
        Index = ParseExpression(indexContent)
      };
    }

    // Array/String methods: arrayName.length, arrayName.method()
    if (input.Contains("."))
    {
      var dotIndex = input.IndexOf('.');
      var objectName = input.Substring(0, dotIndex).Trim();
      var methodPart = input.Substring(dotIndex + 1).Trim();

      // Handle .length property
      if (methodPart == "length")
      {
        return new ArrayLength
        {
          Array = new VariableExpression { Name = objectName }
        };
      }

      // Handle string methods (existing string function parsing)
      // This will be handled by existing string function parsing
    }

    // Function call: functionName(arg1, arg2, ...)
    if (input.Contains("(") && input.EndsWith(")"))
    {
      var parenIndex = input.IndexOf('(');
      var functionName = input.Substring(0, parenIndex).Trim();
      var argsContent = input.Substring(parenIndex + 1, input.Length - parenIndex - 2).Trim();

      // Special handling for console.isSudo()
      if (functionName == "console.isSudo" && string.IsNullOrEmpty(argsContent))
      {
        return new ConsoleIsSudoExpression();
      }

      // Special handling for console.promptYesNo()
      if (functionName == "console.promptYesNo" && !string.IsNullOrEmpty(argsContent))
      {
        // Extract the prompt text from the argument (should be a string literal)
        var promptText = argsContent.Trim();
        if (promptText.StartsWith("\"") && promptText.EndsWith("\""))
        {
          promptText = promptText[1..^1]; // Remove quotes
        }
        return new ConsolePromptYesNoExpression { PromptText = promptText };
      }

      // Make sure it's a simple function name or a timer function (not a complex expression)
      if (Regex.IsMatch(functionName, @"^\w+$") || functionName == "timer.stop")
      {
        var functionCall = new FunctionCall
        {
          Name = functionName
        };

        if (!string.IsNullOrEmpty(argsContent))
        {
          var args = SplitByComma(argsContent);
          foreach (var arg in args)
          {
            functionCall.Arguments.Add(arg.Trim());
          }
        }

        return functionCall;
      }
    }

    // Variable reference
    if (Regex.IsMatch(input, @"^\w+$"))
    {
      return new VariableExpression
      {
        Name = input
      };
    }

    // Fallback to literal
    return new LiteralExpression
    {
      Value = input,
      Type = "unknown"
    };
  }

  private List<string> SplitTernary(string input)
  {
    var parts = new List<string>();
    int depth = 0;
    int questionIndex = -1;
    int colonIndex = -1;

    for (int i = 0; i < input.Length; i++)
    {
      if (input[i] == '(') depth++;
      else if (input[i] == ')') depth--;
      else if (depth == 0 && input[i] == '?' && questionIndex == -1)
      {
        questionIndex = i;
      }
      else if (depth == 0 && input[i] == ':' && questionIndex != -1 && colonIndex == -1)
      {
        colonIndex = i;
      }
    }

    if (questionIndex != -1 && colonIndex != -1)
    {
      parts.Add(input.Substring(0, questionIndex).Trim());
      parts.Add(input.Substring(questionIndex + 1, colonIndex - questionIndex - 1).Trim());
      parts.Add(input.Substring(colonIndex + 1).Trim());
    }

    return parts;
  }

  private List<string> SplitByComma(string input)
  {
    var parts = new List<string>();
    var current = "";
    var depth = 0;
    var inQuotes = false;

    for (int i = 0; i < input.Length; i++)
    {
      var ch = input[i];

      if (ch == '"' && (i == 0 || input[i - 1] != '\\'))
      {
        inQuotes = !inQuotes;
      }

      if (!inQuotes)
      {
        if (ch == '(' || ch == '[')
          depth++;
        else if (ch == ')' || ch == ']')
          depth--;
      }

      if (ch == ',' && depth == 0 && !inQuotes)
      {
        parts.Add(current.Trim());
        current = "";
      }
      else
      {
        current += ch;
      }
    }

    if (!string.IsNullOrEmpty(current))
    {
      parts.Add(current.Trim());
    }

    return parts;
  }

  private List<string> SplitByOperator(string input, params string[] operators)
  {
    var parts = new List<string>();
    int depth = 0;
    int start = 0;
    bool inString = false;
    char stringChar = '\0';

    for (int i = 0; i < input.Length; i++)
    {
      char c = input[i];

      if (!inString && (c == '"' || c == '\''))
      {
        inString = true;
        stringChar = c;
      }
      else if (inString && c == stringChar)
      {
        inString = false;
      }
      else if (!inString)
      {
        if (c == '(') depth++;
        else if (c == ')') depth--;
        else if (depth == 0)
        {
          foreach (var op in operators)
          {
            if (i + op.Length <= input.Length && input.Substring(i, op.Length) == op)
            {
              parts.Add(input.Substring(start, i - start).Trim());
              start = i + op.Length;
              i += op.Length - 1;
              break;
            }
          }
        }
      }
    }

    if (start < input.Length)
    {
      parts.Add(input.Substring(start).Trim());
    }

    return parts.Count > 1 ? parts : new List<string> { input };
  }

  private string GetOperatorBetween(string input, string leftPart, string rightPart)
  {
    int leftEnd = input.IndexOf(leftPart) + leftPart.Length;
    int rightStart = input.IndexOf(rightPart, leftEnd);
    return input.Substring(leftEnd, rightStart - leftEnd).Trim();
  }

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
    else if (line.StartsWith("let "))
    {
      var match = Regex.Match(line, @"let (\w+): (\w+) = (.+);");
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
            var timerNode = ParseTimerFunction(match.Groups[1].Value, value);
            if (timerNode != null)
            {
              return timerNode;
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
                var expression = ParseExpression(value);
                return new VariableDeclarationExpression
                {
                  Name = match.Groups[1].Value,
                  Type = match.Groups[2].Value,
                  Value = expression,
                  IsConst = false
                };
              }
            }
          }
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
    else if (line.StartsWith("for ("))
    {
      // For nested loops, we need to handle them specially
      // For now, return null to indicate this needs special handling
      return null;
    }
    else if (line.Contains(" = ") && line.EndsWith(";"))
    {
      // Handle variable assignment (variable = value;)
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
    else if (line.Contains("(") && line.Contains(");"))
    {
      var match = Regex.Match(line, @"(\w+)\(([^)]*)\);");
      if (match.Success)
      {
        return (match.Groups[1].Value, match.Groups[2].Value) switch
        {
          ("console", "isSudo") => new ConsoleIsSudoExpression(),
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

  private void ValidateArrayElementTypes(ArrayLiteral arrayLiteral, string expectedElementType, string variableName)
  {
    for (int i = 0; i < arrayLiteral.Elements.Count; i++)
    {
      var element = arrayLiteral.Elements[i];
      string actualType = GetExpressionType(element);

      if (actualType != expectedElementType)
      {
        throw new InvalidOperationException($"Error: Array element at index {i} in variable '{variableName}' has type '{actualType}' but expected '{expectedElementType}'. All elements in a {expectedElementType}[] array must be of type {expectedElementType}.");
      }
    }
  }

  private string GetExpressionType(Expression expression)
  {
    return expression switch
    {
      LiteralExpression literal => literal.Type,
      VariableExpression _ => "variable", // We could track variable types, but for now assume it's correct
      BinaryExpression _ => "expression", // Could infer type from operation
      _ => "unknown"
    };
  }

  // Timer function parsing methods
  private Node? ParseStandaloneTimerFunction(string line)
  {
    line = line.Trim();

    if (line == "timer.start();")
    {
      return new TimerStart();
    }

    return null;
  }

  private Node? ParseTimerFunction(string assignTo, string value)
  {
    value = value.Trim();

    // Parse timer.stop() -> TimerStop
    if (value == "timer.stop()")
    {
      return new TimerStop { AssignTo = assignTo };
    }

    return null;
  }

  private Node? ParseProcessFunction(string assignTo, string value)
  {
    value = value.Trim();

    if (value == "process.id()")
    {
      return new ProcessId { AssignTo = assignTo };
    }

    if (value == "process.cpu()")
    {
      return new ProcessCpu { AssignTo = assignTo };
    }

    if (value == "process.memory()")
    {
      return new ProcessMemory { AssignTo = assignTo };
    }

    if (value == "process.elapsedTime()")
    {
      return new ProcessElapsedTime { AssignTo = assignTo };
    }

    if (value == "process.command()")
    {
      return new ProcessCommand { AssignTo = assignTo };
    }

    if (value == "process.status()")
    {
      return new ProcessStatus { AssignTo = assignTo };
    }

    return null;
  }
}
