using System.Text.RegularExpressions;
using System.Text;

public partial class Parser
{
  private readonly string[] _lines;
  private readonly HashSet<string> _constVariables = new HashSet<string>();

  public Parser(string input)
  {
    // Pre-process to remove comments, but preserve strings
    var processed = RemoveComments(input);
    _lines = processed.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
  }

  private string RemoveComments(string input)
  {
    var result = new StringBuilder();
    bool inString = false;
    char stringChar = '\0';

    for (int i = 0; i < input.Length; i++)
    {
      char current = input[i];

      // Handle string literals
      if (!inString && (current == '"' || current == '\''))
      {
        inString = true;
        stringChar = current;
        result.Append(current);
      }
      else if (inString && current == stringChar)
      {
        // Check if it's escaped
        int backslashCount = 0;
        for (int j = i - 1; j >= 0 && input[j] == '\\'; j--)
          backslashCount++;

        if (backslashCount % 2 == 0) // Even number of backslashes means not escaped
        {
          inString = false;
        }
        result.Append(current);
      }
      else if (inString)
      {
        result.Append(current);
      }
      else
      {
        // Not in string - check for comments
        if (current == '/' && i + 1 < input.Length)
        {
          if (input[i + 1] == '/') // Line comment
          {
            // Skip to end of line
            while (i < input.Length && input[i] != '\n')
              i++;
            if (i < input.Length)
              result.Append(input[i]); // Include the newline
          }
          else if (input[i + 1] == '*') // Block comment
          {
            i += 2; // Skip /*
            while (i + 1 < input.Length && !(input[i] == '*' && input[i + 1] == '/'))
              i++;
            if (i + 1 < input.Length)
              i++; // Skip */
          }
          else
          {
            result.Append(current);
          }
        }
        else if (current == '#') // Bash comment
        {
          // Skip to end of line
          while (i < input.Length && input[i] != '\n')
            i++;
          if (i < input.Length)
            result.Append(input[i]); // Include the newline
        }
        else
        {
          result.Append(current);
        }
      }
    }

    return result.ToString();
  }

  public ProgramNode Parse()
  {
    var program = new ProgramNode();

    for (int i = 0; i < _lines.Length; i++)
    {
      var line = _lines[i].Trim();

      if (line.StartsWith("let "))
      {
        // Try typed let first: let variable: type = value;
        var typedMatch = Regex.Match(line, @"let (\w+): ([\w\[\]]+) = (.+);");
        if (typedMatch.Success)
        {
          var value = typedMatch.Groups[3].Value;

          var envNode = ParseEnvFunction(typedMatch.Groups[1].Value, value);
          if (envNode != null)
          {
            program.Statements.Add(envNode);
          }
          else
          {
            var osNode = ParseOsFunction(typedMatch.Groups[1].Value, value);
            if (osNode != null)
            {
              program.Statements.Add(osNode);
            }
            else
            {
              var fsNode = ParseFsFunction(typedMatch.Groups[1].Value, value);
              if (fsNode != null)
              {
                program.Statements.Add(fsNode);
              }
              else
              {
                var stringFuncNode = ParseStringFunction(typedMatch.Groups[1].Value, value);
                if (stringFuncNode != null)
                {
                  program.Statements.Add(stringFuncNode);
                }
                else
                {
                  var processNode = ParseProcessFunction(typedMatch.Groups[1].Value, value);
                  if (processNode != null)
                  {
                    program.Statements.Add(processNode);
                  }
                  else
                  {
                    var timerNode = ParseTimerFunction(typedMatch.Groups[1].Value, value);
                    if (timerNode != null)
                    {
                      program.Statements.Add(timerNode);
                    }
                    else
                    {
                      var declaredType = typedMatch.Groups[2].Value;
                      var expression = ParseExpression(value);

                      // Validate array type if it's an array declaration
                      if (declaredType.EndsWith("[]") && expression is ArrayLiteral arrayLiteral)
                      {
                        var expectedElementType = declaredType.Substring(0, declaredType.Length - 2);
                        ValidateArrayElementTypes(arrayLiteral, expectedElementType, typedMatch.Groups[1].Value);
                      }

                      program.Statements.Add(new VariableDeclarationExpression
                      {
                        Name = typedMatch.Groups[1].Value,
                        Type = declaredType,
                        Value = expression ?? new LiteralExpression { Value = "", Type = "string" },
                        IsConst = false
                      });
                    }
                  }
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
            program.Statements.Add(new VariableDeclarationExpression
            {
              Name = untypedMatch.Groups[1].Value,
              Type = "number", // Default type inference
              Value = expression,
              IsConst = false
            });
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
          else if (inner == "console.clear();")
          {
            func.Body.Add(new ConsoleClearStatement());
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
        var ifStmt = new IfStatement { Condition = ParseExpression(cond) };

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
          else if (inner == "console.clear();")
          {
            ifStmt.ThenBody.Add(new ConsoleClearStatement());
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
            else if (inner == "console.clear();")
            {
              ifStmt.ElseBody.Add(new ConsoleClearStatement());
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
      else if (line.StartsWith("while ("))
      {
        var match = Regex.Match(line, @"while \((.+)\) \{");
        if (match.Success)
        {
          var whileStmt = new WhileStatement
          {
            Condition = ParseExpression(match.Groups[1].Value)
          };

          i++;
          while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
          {
            var inner = _lines[i].Trim();
            if (inner == "break;")
            {
              whileStmt.Body.Add(new BreakStatement());
              i++;
            }
            else
            {
              var statement = ParseStatementOrBlock(inner, ref i);
              if (statement != null)
              {
                whileStmt.Body.Add(statement);
              }
              else
              {
                i++;
              }
            }
          }
          program.Statements.Add(whileStmt);
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
          // Template literal - check if it contains expressions
          var msg = raw[1..^1];
          if (msg.Contains("${"))
          {
            // Parse as template literal since it contains template expressions
            var expr = ParseTemplateLiteral(raw);
            program.Statements.Add(new ConsoleLog { Expression = expr, IsExpression = true });
          }
          else
          {
            // Simple template literal without expressions
            program.Statements.Add(new ConsoleLog { Message = msg, IsExpression = false });
          }
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
      else if (line == "console.clear();")
      {
        // Parse console.clear() statement
        program.Statements.Add(new ConsoleClearStatement());
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
      else if (line.StartsWith("script."))
      {
        // Handle script control functions
        var scriptStatement = ParseStatement(line);
        if (scriptStatement != null)
        {
          program.Statements.Add(scriptStatement);
        }
      }
      else if (line.Contains("(") && line.Contains(");"))
      {
        var match = Regex.Match(line, @"([\w\.]+)\(([^)]*)\);");
        if (match.Success)
        {
          program.Statements.Add(new FunctionCall
          {
            Name = match.Groups[1].Value,
            Arguments = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(a => a.Trim()).ToList()
          });
        }
      }
      else
      {
        // If no other syntax matches, treat it as a raw line of shell script
        program.Statements.Add(new RawStatement { Content = line });
      }
    }

    return program;
  }

  // Function parsing methods have been moved to Parser.Functions.cs
  // Utility methods have been moved to Parser.Utilities.cs

}
