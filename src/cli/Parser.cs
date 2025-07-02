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
      string line = _lines[i].Trim();

      if (line.StartsWith("let "))
      {
        var match = Regex.Match(line, @"let (\w+): (\w+) = (.+);");
        if (match.Success)
        {
          program.Statements.Add(new VariableDeclaration
          {
            Name = match.Groups[1].Value,
            Type = match.Groups[2].Value,
            Value = match.Groups[3].Value.Trim('"'),
            IsConst = false
          });
        }
      }
      else if (line.StartsWith("const "))
      {
        var match = Regex.Match(line, @"const (\w+): (\w+) = (.+);");
        if (match.Success)
        {
          program.Statements.Add(new VariableDeclaration
          {
            Name = match.Groups[1].Value,
            Type = match.Groups[2].Value,
            Value = match.Groups[3].Value.Trim('"'),
            IsConst = true
          });
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
            func.Body.Add(new VariableDeclaration
            {
              Name = match.Groups[1].Value,
              Type = match.Groups[2].Value,
              Value = match.Groups[3].Value.Trim('"'),
              IsConst = false
            });
          }
          else if (inner.StartsWith("const "))
          {
            var match = Regex.Match(inner, @"const (\w+): (\w+) = (.+);");
            func.Body.Add(new VariableDeclaration
            {
              Name = match.Groups[1].Value,
              Type = match.Groups[2].Value,
              Value = match.Groups[3].Value.Trim('"'),
              IsConst = true
            });
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
            ifStmt.ThenBody.Add(new VariableDeclaration
            {
              Name = m.Groups[1].Value,
              Type = m.Groups[2].Value,
              Value = m.Groups[3].Value.Trim('"'),
              IsConst = false
            });
          }
          else if (inner.StartsWith("const "))
          {
            var m = Regex.Match(inner, @"const (\w+): (\w+) = (.+);");
            ifStmt.ThenBody.Add(new VariableDeclaration
            {
              Name = m.Groups[1].Value,
              Type = m.Groups[2].Value,
              Value = m.Groups[3].Value.Trim('"'),
              IsConst = true
            });
          }
          else if (inner.StartsWith("exit "))
          {
            int code = int.Parse(inner.Substring(5).TrimEnd(';'));
            ifStmt.ThenBody.Add(new ExitStatement { ExitCode = code });
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
            i++;
          }
        }

        program.Statements.Add(ifStmt);
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
    }

    return program;
  }
}
