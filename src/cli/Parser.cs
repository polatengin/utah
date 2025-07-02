using System.Text.RegularExpressions;

public class Parser
{
  private readonly string[] _lines;

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
        var match = Regex.Match(line, @"let (\w+): (\w+) = (.+);");
        if (match.Success)
        {
          var value = match.Groups[3].Value;
          var stringFuncNode = ParseStringFunction(match.Groups[1].Value, value);
          if (stringFuncNode != null)
          {
            program.Statements.Add(stringFuncNode);
          }
          else
          {
            program.Statements.Add(new VariableDeclaration
            {
              Name = match.Groups[1].Value,
              Type = match.Groups[2].Value,
              Value = value.Trim('"'),
              IsConst = false
            });
          }
        }
      }
      else if (line.StartsWith("const "))
      {
        var match = Regex.Match(line, @"const (\w+): (\w+) = (.+);");
        if (match.Success)
        {
          var value = match.Groups[3].Value;
          var stringFuncNode = ParseStringFunction(match.Groups[1].Value, value);
          if (stringFuncNode != null)
          {
            program.Statements.Add(stringFuncNode);
          }
          else
          {
            program.Statements.Add(new VariableDeclaration
            {
              Name = match.Groups[1].Value,
              Type = match.Groups[2].Value,
              Value = value.Trim('"'),
              IsConst = true
            });
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
                  IsConst = true
                });
              }
            }
          }
          else if (inner.StartsWith("console.log"))
          {
            var match = Regex.Match(inner, @"console\.log\((.+)\);");
            var raw = match.Groups[1].Value.Trim();
            string msg;
            if (raw.StartsWith("`"))
            {
              // Template literal
              msg = raw[1..^1];
            }
            else if (raw.StartsWith("\"") && raw.EndsWith("\""))
            {
              // String literal
              msg = raw[1..^1];
            }
            else
            {
              // Variable reference
              msg = $"${raw}";
            }
            func.Body.Add(new ConsoleLog { Message = msg });
          }
          else if (inner.StartsWith("return "))
          {
            var val = inner.Substring(7).TrimEnd(';');
            func.Body.Add(new ReturnStatement { Value = val });
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
            string msg;
            if (raw.StartsWith("`"))
            {
              // Template literal
              msg = raw[1..^1];
            }
            else if (raw.StartsWith("\"") && raw.EndsWith("\""))
            {
              // String literal
              msg = raw[1..^1];
            }
            else
            {
              // Variable reference
              msg = $"${raw}";
            }
            ifStmt.ThenBody.Add(new ConsoleLog { Message = msg });
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

        i++;
        if (_lines[i].Trim() == "else {")
        {
          i++;
          while (!_lines[i].Trim().StartsWith("}"))
          {
            string inner = _lines[i].Trim();
            if (inner.StartsWith("console.log"))
            {
              var m = Regex.Match(inner, @"console\.log\((.+)\);");
              var raw = m.Groups[1].Value.Trim();
              string msg;
              if (raw.StartsWith("`"))
              {
                // Template literal
                msg = raw[1..^1];
              }
              else if (raw.StartsWith("\"") && raw.EndsWith("\""))
              {
                // String literal
                msg = raw[1..^1];
              }
              else
              {
                // Variable reference
                msg = $"${raw}";
              }
              ifStmt.ElseBody.Add(new ConsoleLog { Message = msg });
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
        string msg;
        if (raw.StartsWith("`"))
        {
          // Template literal
          msg = raw[1..^1];
        }
        else if (raw.StartsWith("\"") && raw.EndsWith("\""))
        {
          // String literal
          msg = raw[1..^1];
        }
        else
        {
          // Variable reference
          msg = $"${raw}";
        }
        program.Statements.Add(new ConsoleLog { Message = msg });
      }
      else if (line.Contains("(") && line.Contains(");"))
      {
        var match = Regex.Match(line, @"(\w+)\(([^)]*)\);");
        program.Statements.Add(new FunctionCall
        {
          Name = match.Groups[1].Value,
          Arguments = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().Trim('"')).ToList()
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

    return null;
  }
}
