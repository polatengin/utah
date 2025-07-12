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
    var statements = new List<Statement>();

    for (int i = 0; i < _lines.Length; i++)
    {
      var line = _lines[i].Trim();
      if (string.IsNullOrEmpty(line)) continue;

      var statement = ParseStatement(line, ref i);
      if (statement != null)
      {
        statements.Add(statement);
      }
    }

    return new ProgramNode(statements);
  }

  private Statement? ParseStatement(string line, ref int i)
  {
    if (line.StartsWith("let ") || line.StartsWith("const "))
    {
      return ParseVariableDeclaration(line, ref i);
    }
    if (line.StartsWith("function "))
    {
      return ParseFunctionDeclaration(line, ref i);
    }
    if (line.StartsWith("if ("))
    {
      return ParseIfStatement(line, ref i);
    }
    if (line.StartsWith("for ("))
    {
      // Disambiguate between for and for-in loops
      var forInMatch = Regex.Match(line, @"for \((let|const) (\w+)(?::\s*(\w+))? in (.+)\) \{");
      if (forInMatch.Success)
      {
        return ParseForInLoop(line, ref i);
      }
      return ParseForLoop(line, ref i);
    }
    if (line.StartsWith("while ("))
    {
      return ParseWhileStatement(line, ref i);
    }
    if (line.StartsWith("switch ("))
    {
      return ParseSwitchStatement(line, ref i);
    }
    if (line.StartsWith("try {"))
    {
      return ParseTryCatchStatement(line, ref i);
    }
    if (line.StartsWith("return "))
    {
      var valueStr = line.Substring(7).TrimEnd(';');
      var value = string.IsNullOrEmpty(valueStr) ? null : ParseExpression(valueStr);
      return new ReturnStatement(value);
    }
    if (line.StartsWith("exit "))
    {
      var codeStr = line.Substring(5).TrimEnd(';');
      return new ExitStatement(ParseExpression(codeStr));
    }
    if (line.StartsWith("exit(") && line.EndsWith(");"))
    {
      var codeStr = line.Substring(5, line.Length - 7);
      return new ExitStatement(ParseExpression(codeStr));
    }
    if (line == "break;")
    {
      return new BreakStatement();
    }
    if (line == "continue;")
    {
      return new ContinueStatement();
    }
    if (line == "console.clear();")
    {
      return new ConsoleClearStatement();
    }
    if (line.StartsWith("console.log"))
    {
      var match = Regex.Match(line, @"console\.log\((.+)\);");
      if (match.Success)
      {
        var raw = match.Groups[1].Value.Trim();
        return new ConsoleLog(ParseExpression(raw));
      }
    }
    if (line == "timer.start();")
    {
      return new TimerStartStatement();
    }
    if (line.StartsWith("script."))
    {
      return ParseScriptControlStatement(line);
    }
    if (line.StartsWith("args."))
    {
      return ParseArgsStatement(line);
    }

    // Check for assignment statements
    if (line.Contains(" = ") && line.EndsWith(";") && !line.StartsWith("let ") && !line.StartsWith("const "))
    {
      // Updated regex to handle both simple variables and array access patterns
      var match = Regex.Match(line, @"([a-zA-Z_][a-zA-Z0-9_]*(?:\[[^\]]+\])?)\s*=\s*(.+);");
      if (match.Success)
      {
        var leftSide = match.Groups[1].Value;

        // Extract variable name for const checking
        var variableName = leftSide.Contains('[') ? leftSide.Substring(0, leftSide.IndexOf('[')) : leftSide;

        // Check if trying to reassign a const variable
        if (_constVariables.Contains(variableName))
        {
          throw new InvalidOperationException($"Error: Cannot reassign const variable '{variableName}'. Const variables are immutable once declared.");
        }

        // Parse as regular assignment expression
        var expression = ParseExpression(line.TrimEnd(';'));
        return new ExpressionStatement(expression);
      }
    }

    // Check for raw bash patterns that should not be parsed as Utah syntax
    if (IsRawBashStatement(line))
    {
      return new RawStatement(line);
    }

    // Fallback to expression statement
    try
    {
      var expression = ParseExpression(line.TrimEnd(';'));
      // Check if the expression is a placeholder that should be a statement
      if (expression is FsWriteFileExpressionPlaceholder placeholder)
      {
        return new FsWriteFileStatement(placeholder.FilePath, placeholder.Content);
      }
      return new ExpressionStatement(expression);
    }
    catch
    {
      // If it's not a valid expression, it might be a raw statement or an error
      // For now, we'll treat it as a raw statement if it doesn't look like an incomplete expression
      if (!line.EndsWith(";") && !line.EndsWith("{"))
      {
        return new RawStatement(line);
      }
    }


    return null;
  }

  private VariableDeclaration ParseVariableDeclaration(string line, ref int i)
  {
    var isConst = line.StartsWith("const ");

    // Check if this is a multi-line array literal
    if (line.Contains("= [") && !line.TrimEnd().EndsWith("];"))
    {
      // This is a multi-line array literal, collect all lines until we find the closing ]
      var fullDeclaration = line;
      var originalI = i;

      i++;
      while (i < _lines.Length)
      {
        var nextLine = _lines[i].Trim();
        fullDeclaration += " " + nextLine;

        if (nextLine.EndsWith("];"))
        {
          break;
        }
        i++;
      }

      // Now parse the complete declaration
      var match = Regex.Match(fullDeclaration, @"(let|const) (\w+)(?::\s*([\w\[\]]+))?\s*=\s*(.+);");
      if (match.Success)
      {
        var name = match.Groups[2].Value;
        var type = match.Groups[3].Value;
        var valueStr = match.Groups[4].Value;

        if (isConst)
        {
          _constVariables.Add(name);
        }

        var value = ParseExpression(valueStr);

        // Validate array element types if this is an array type
        if (type.EndsWith("[]") && value is ArrayLiteral arrayLiteral)
        {
          var elementType = type.Substring(0, type.Length - 2); // Remove []
          ValidateArrayElementTypes(arrayLiteral, elementType, name);
        }

        return new VariableDeclaration(name, type, value, isConst);
      }
      throw new Exception($"Invalid multi-line variable declaration: {fullDeclaration}");
    }

    // Single-line variable declaration
    var singleLineMatch = Regex.Match(line, @"(let|const) (\w+)(?::\s*([\w\[\]]+))?\s*=\s*(.+);");
    if (singleLineMatch.Success)
    {
      var name = singleLineMatch.Groups[2].Value;
      var type = singleLineMatch.Groups[3].Value;
      var valueStr = singleLineMatch.Groups[4].Value;

      if (isConst)
      {
        _constVariables.Add(name);
      }

      var value = ParseExpression(valueStr);

      // Validate array element types if this is an array type
      if (type.EndsWith("[]") && value is ArrayLiteral arrayLiteral)
      {
        var elementType = type.Substring(0, type.Length - 2); // Remove []
        ValidateArrayElementTypes(arrayLiteral, elementType, name);
      }

      return new VariableDeclaration(name, type, value, isConst);
    }
    throw new Exception($"Invalid variable declaration: {line}");
  }

  private VariableDeclaration ParseSingleLineVariableDeclaration(string line)
  {
    var isConst = line.StartsWith("const ");
    var match = Regex.Match(line, @"(let|const) (\w+)(?::\s*([\w\[\]]+))?\s*=\s*(.+);");
    if (match.Success)
    {
      var name = match.Groups[2].Value;
      var type = match.Groups[3].Value;
      var valueStr = match.Groups[4].Value;

      if (isConst)
      {
        _constVariables.Add(name);
      }

      var value = ParseExpression(valueStr);

      // Validate array element types if this is an array type
      if (type.EndsWith("[]") && value is ArrayLiteral arrayLiteral)
      {
        var elementType = type.Substring(0, type.Length - 2); // Remove []
        ValidateArrayElementTypes(arrayLiteral, elementType, name);
      }

      return new VariableDeclaration(name, type, value, isConst);
    }
    throw new Exception($"Invalid variable declaration: {line}");
  }

  private FunctionDeclaration ParseFunctionDeclaration(string line, ref int i)
  {
    var headerMatch = Regex.Match(line, @"function (\w+)\(([^)]*)\)(?::\s*(\w+))?\s*\{");
    if (!headerMatch.Success) throw new Exception("Invalid function declaration");

    var name = headerMatch.Groups[1].Value;
    var returnType = headerMatch.Groups[3].Value;
    var parameters = new List<(string Name, string Type)>();
    var paramList = headerMatch.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
    foreach (var p in paramList)
    {
      var parts = p.Trim().Split(':');
      parameters.Add((parts[0].Trim(), parts[1].Trim()));
    }

    var body = new List<Statement>();
    i++;
    while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
    {
      var innerLine = _lines[i].Trim();
      if (!string.IsNullOrEmpty(innerLine))
      {
        var statement = ParseStatement(innerLine, ref i);
        if (statement != null)
        {
          body.Add(statement);
        }
      }
      i++;
    }
    return new FunctionDeclaration(name, parameters, body, returnType);
  }

  private IfStatement ParseIfStatement(string line, ref int i)
  {
    var match = Regex.Match(line, @"if \((.+)\) \{");
    if (!match.Success) throw new Exception("Invalid if statement");

    var condition = ParseExpression(match.Groups[1].Value);
    var thenBody = new List<Statement>();
    var elseBody = new List<Statement>();

    // Check if this is a single-line if statement (content and closing brace on same line)
    if (line.Contains("} ") || line.EndsWith("}"))
    {
      // Extract the content between { and }
      var startBrace = line.IndexOf('{');
      var endBrace = line.LastIndexOf('}');
      if (startBrace >= 0 && endBrace > startBrace)
      {
        var content = line.Substring(startBrace + 1, endBrace - startBrace - 1).Trim();
        if (!string.IsNullOrEmpty(content))
        {
          // Parse the single-line content as a statement
          var tempIndex = 0;
          var statement = ParseStatement(content, ref tempIndex);
          if (statement != null)
          {
            thenBody.Add(statement);
          }
        }

        // Check if there's an else clause on the next line
        if (i + 1 < _lines.Length)
        {
          var nextLine = _lines[i + 1].Trim();
          if (nextLine.StartsWith("else {"))
          {
            // Handle single-line else
            i++; // Move to the else line
            var elseStartBrace = nextLine.IndexOf('{');
            var elseEndBrace = nextLine.LastIndexOf('}');
            if (elseStartBrace >= 0 && elseEndBrace > elseStartBrace)
            {
              var elseContent = nextLine.Substring(elseStartBrace + 1, elseEndBrace - elseStartBrace - 1).Trim();
              if (!string.IsNullOrEmpty(elseContent))
              {
                var tempIndex = 0;
                var elseStatement = ParseStatement(elseContent, ref tempIndex);
                if (elseStatement != null)
                {
                  elseBody.Add(elseStatement);
                }
              }
            }
          }
          else if (nextLine.StartsWith("else if ("))
          {
            // Handle else if as a nested if statement
            i++; // Move to the else if line
            var elseIfStatement = ParseIfStatement(nextLine, ref i);
            elseBody.Add(elseIfStatement);
          }
        }

        return new IfStatement(condition, thenBody, elseBody);
      }
    }

    i++;
    // Parse the then body until we hit a closing } (possibly followed by else)
    while (i < _lines.Length)
    {
      var innerLine = _lines[i].Trim();

      // Check if this line ends the then body
      if (innerLine == "}" || innerLine == "} else {" || innerLine.StartsWith("} else if ("))
      {
        // Handle the else clause if present on the same line
        if (innerLine == "} else {")
        {
          // Parse the else body
          i++; // Move past the } else { line
          while (i < _lines.Length && _lines[i].Trim() != "}")
          {
            var elseLine = _lines[i].Trim();
            if (!string.IsNullOrEmpty(elseLine))
            {
              var statement = ParseStatement(elseLine, ref i);
              if (statement != null)
              {
                elseBody.Add(statement);
              }
            }
            i++;
          }
        }
        else if (innerLine.StartsWith("} else if ("))
        {
          // Handle else if
          var elseIfLine = innerLine.Substring(2); // Remove "} " prefix
          var elseIfStatement = ParseIfStatement(elseIfLine, ref i);
          elseBody.Add(elseIfStatement);
        }
        break; // Exit the then body parsing loop
      }

      // Parse regular statements in the then body
      if (!string.IsNullOrEmpty(innerLine))
      {
        var statement = ParseStatement(innerLine, ref i);
        if (statement != null)
        {
          thenBody.Add(statement);
        }
      }
      i++;
    }

    // Check if there's an else clause on the next line (for multi-line format)
    if (elseBody.Count == 0 && i + 1 < _lines.Length)
    {
      var nextLine = _lines[i + 1].Trim();

      if (nextLine.StartsWith("else if ("))
      {
        // Handle else if as a nested if statement
        i++; // Move to the else if line
        var elseIfStatement = ParseIfStatement(nextLine, ref i);
        elseBody.Add(elseIfStatement);
      }
      else if (nextLine == "else {")
      {
        // Handle else clause
        i++; // Move to the else line
        i++; // Move past the opening brace of else
        while (i < _lines.Length && _lines[i].Trim() != "}")
        {
          var elseLine = _lines[i].Trim();
          if (!string.IsNullOrEmpty(elseLine))
          {
            var statement = ParseStatement(elseLine, ref i);
            if (statement != null)
            {
              elseBody.Add(statement);
            }
          }
          i++;
        }
      }
    }

    return new IfStatement(condition, thenBody, elseBody);
  }

  private ForInLoop ParseForInLoop(string line, ref int i)
  {
    var forInMatch = Regex.Match(line, @"for \((let|const) (\w+)(?::\s*(\w+))? in (.+)\) \{");
    if (forInMatch.Success)
    {
      var variable = forInMatch.Groups[2].Value;
      var iterable = ParseExpression(forInMatch.Groups[4].Value);
      var body = new List<Statement>();
      i++;
      while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
      {
        var innerLine = _lines[i].Trim();
        if (!string.IsNullOrEmpty(innerLine))
        {
          var statement = ParseStatement(innerLine, ref i);
          if (statement != null)
          {
            body.Add(statement);
          }
        }
        i++;
      }
      return new ForInLoop(variable, iterable, body);
    }
    throw new Exception("Invalid for-in loop syntax");
  }

  private ForLoop ParseForLoop(string line, ref int i)
  {
    var forMatch = Regex.Match(line, @"for \((.+); (.+); (.+)\) \{");
    if (forMatch.Success)
    {
      var initStr = forMatch.Groups[1].Value.Trim();
      var initVar = (VariableDeclaration)ParseSingleLineVariableDeclaration(initStr + ";");

      var condition = ParseExpression(forMatch.Groups[2].Value);
      var update = ParseExpression(forMatch.Groups[3].Value);

      var body = new List<Statement>();
      i++;
      while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
      {
        var innerLine = _lines[i].Trim();
        if (!string.IsNullOrEmpty(innerLine))
        {
          var statement = ParseStatement(innerLine, ref i);
          if (statement != null)
          {
            body.Add(statement);
          }
        }
        i++;
      }
      return new ForLoop(initVar, condition, update, body);
    }
    throw new Exception("Invalid for loop syntax");
  }

  private WhileStatement ParseWhileStatement(string line, ref int i)
  {
    var match = Regex.Match(line, @"while \((.+)\) \{");
    if (!match.Success) throw new Exception("Invalid while statement");

    var condition = ParseExpression(match.Groups[1].Value);
    var body = new List<Statement>();
    i++;
    while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
    {
      var innerLine = _lines[i].Trim();
      if (!string.IsNullOrEmpty(innerLine))
      {
        var statement = ParseStatement(innerLine, ref i);
        if (statement != null)
        {
          body.Add(statement);
        }
      }
      i++;
    }
    return new WhileStatement(condition, body);
  }

  private SwitchStatement ParseSwitchStatement(string line, ref int i)
  {
    var match = Regex.Match(line, @"switch \((.+)\) \{");
    if (!match.Success) throw new Exception("Invalid switch statement");

    var expression = ParseExpression(match.Groups[1].Value);
    var cases = new List<CaseClause>();
    DefaultClause? defaultClause = null;

    i++;
    while (i < _lines.Length && _lines[i].Trim() != "}")
    {
      var innerLine = _lines[i].Trim();
      if (innerLine.StartsWith("case "))
      {
        // Collect all consecutive case values (for fall-through)
        var values = new List<Expression>();
        while (i < _lines.Length && _lines[i].Trim().StartsWith("case "))
        {
          var caseMatch = Regex.Match(_lines[i].Trim(), @"case (.+):");
          values.Add(ParseExpression(caseMatch.Groups[1].Value));
          i++;
        }

        // Now parse the body until we hit a break, another case, default, or end
        var body = new List<Statement>();
        bool hasBreak = false;
        while (i < _lines.Length && !_lines[i].Trim().StartsWith("case ") && !_lines[i].Trim().StartsWith("default:") && _lines[i].Trim() != "}")
        {
          var caseBodyLine = _lines[i].Trim();
          if (caseBodyLine == "break;")
          {
            hasBreak = true;
            i++;
            break;
          }
          if (!string.IsNullOrEmpty(caseBodyLine))
          {
            var statement = ParseStatement(caseBodyLine, ref i);
            if (statement != null)
            {
              body.Add(statement);
            }
          }
          i++;
        }
        cases.Add(new CaseClause(values, body, hasBreak));
        continue;
      }
      if (innerLine.StartsWith("default:"))
      {
        var body = new List<Statement>();
        bool hasBreak = false;
        i++;
        while (i < _lines.Length && _lines[i].Trim() != "}")
        {
          var defaultBodyLine = _lines[i].Trim();
          if (defaultBodyLine == "break;")
          {
            hasBreak = true;
            break;
          }
          if (!string.IsNullOrEmpty(defaultBodyLine))
          {
            var statement = ParseStatement(defaultBodyLine, ref i);
            if (statement != null)
            {
              body.Add(statement);
            }
          }
          i++;
        }
        defaultClause = new DefaultClause(body, hasBreak);
        if (hasBreak) i++;
        continue;
      }
      i++;
    }
    return new SwitchStatement(expression, cases, defaultClause);
  }

  private TryCatchStatement ParseTryCatchStatement(string line, ref int i)
  {
    if (!line.StartsWith("try {")) throw new Exception("Invalid try statement");

    var tryBody = new List<Statement>();
    var catchBody = new List<Statement>();

    // Parse try block
    i++;
    while (i < _lines.Length && _lines[i].Trim() != "}")
    {
      var innerLine = _lines[i].Trim();
      if (!string.IsNullOrEmpty(innerLine))
      {
        var statement = ParseStatement(innerLine, ref i);
        if (statement != null)
        {
          tryBody.Add(statement);
        }
      }
      i++;
    }

    // Expect "catch {" on the next line
    i++;
    if (i >= _lines.Length || !_lines[i].Trim().StartsWith("catch"))
    {
      throw new Exception("Try statement must be followed by catch block");
    }

    var catchLine = _lines[i].Trim();
    string? errorMessage = null;

    // Check if catch has a specific error message: catch("error message") {
    var catchWithMessage = Regex.Match(catchLine, @"catch\s*\(\s*""([^""]*)""\s*\)\s*\{");
    if (catchWithMessage.Success)
    {
      errorMessage = catchWithMessage.Groups[1].Value;
    }
    else if (!catchLine.StartsWith("catch {"))
    {
      throw new Exception("Invalid catch statement syntax");
    }

    // Parse catch block
    i++;
    while (i < _lines.Length && _lines[i].Trim() != "}")
    {
      var innerLine = _lines[i].Trim();
      if (!string.IsNullOrEmpty(innerLine))
      {
        var statement = ParseStatement(innerLine, ref i);
        if (statement != null)
        {
          catchBody.Add(statement);
        }
      }
      i++;
    }

    return new TryCatchStatement(tryBody, catchBody, errorMessage);
  }

  private Statement ParseScriptControlStatement(string line)
  {
    // Handle script.description() with parameter
    if (line.StartsWith("script.description(") && line.EndsWith(");"))
    {
      var paramStart = line.IndexOf('(') + 1;
      var paramEnd = line.LastIndexOf(')');
      var parameter = line.Substring(paramStart, paramEnd - paramStart).Trim();

      // Remove quotes if present
      if (parameter.StartsWith("\"") && parameter.EndsWith("\""))
      {
        parameter = parameter.Substring(1, parameter.Length - 2);
      }

      return new ScriptDescriptionStatement(parameter);
    }

    return line switch
    {
      "script.enableDebug();" => new ScriptEnableDebugStatement(),
      "script.disableDebug();" => new ScriptDisableDebugStatement(),
      "script.enableGlobbing();" => new ScriptEnableGlobbingStatement(),
      "script.disableGlobbing();" => new ScriptDisableGlobbingStatement(),
      "script.exitOnError();" => new ScriptExitOnErrorStatement(),
      "script.continueOnError();" => new ScriptContinueOnErrorStatement(),
      _ => new RawStatement(line)
    };
  }

  private Statement ParseArgsStatement(string line)
  {
    // Handle args.define() with parameters
    if (line.StartsWith("args.define(") && line.EndsWith(");"))
    {
      var paramStart = line.IndexOf('(') + 1;
      var paramEnd = line.LastIndexOf(')');
      var parametersStr = line.Substring(paramStart, paramEnd - paramStart).Trim();

      // Parse the parameters - expecting: "longFlag", "shortFlag", "description", "type", required, defaultValue
      var parameters = ParseFunctionParameters(parametersStr);

      if (parameters.Count < 3)
        throw new Exception("args.define() requires at least 3 parameters: longFlag, shortFlag, description");

      var longFlag = StripQuotes(parameters[0]);
      var shortFlag = StripQuotes(parameters[1]);
      var description = StripQuotes(parameters[2]);
      var type = parameters.Count > 3 ? StripQuotes(parameters[3]) : "string";
      var isRequired = parameters.Count > 4 ? bool.Parse(parameters[4]) : false;
      Expression? defaultValue = parameters.Count > 5 ? ParseExpression(parameters[5]) : null;

      return new ArgsDefineStatement(longFlag, shortFlag, description, type, isRequired, defaultValue);
    }

    if (line == "args.showHelp();")
    {
      return new ArgsShowHelpStatement();
    }

    return new RawStatement(line);
  }

  private List<string> ParseFunctionParameters(string parametersStr)
  {
    var parameters = new List<string>();
    var current = new StringBuilder();
    var inQuotes = false;
    var quoteChar = '\0';
    var parenCount = 0;

    for (int i = 0; i < parametersStr.Length; i++)
    {
      var c = parametersStr[i];

      if (!inQuotes && (c == '"' || c == '\''))
      {
        inQuotes = true;
        quoteChar = c;
        current.Append(c);
      }
      else if (inQuotes && c == quoteChar)
      {
        inQuotes = false;
        current.Append(c);
      }
      else if (!inQuotes && c == '(')
      {
        parenCount++;
        current.Append(c);
      }
      else if (!inQuotes && c == ')')
      {
        parenCount--;
        current.Append(c);
      }
      else if (!inQuotes && c == ',' && parenCount == 0)
      {
        parameters.Add(current.ToString().Trim());
        current.Clear();
      }
      else
      {
        current.Append(c);
      }
    }

    if (current.Length > 0)
    {
      parameters.Add(current.ToString().Trim());
    }

    return parameters;
  }

  private string StripQuotes(string str)
  {
    str = str.Trim();
    if ((str.StartsWith("\"") && str.EndsWith("\"")) || (str.StartsWith("'") && str.EndsWith("'")))
    {
      return str.Substring(1, str.Length - 2);
    }
    return str;
  }

  private void ValidateArrayElementTypes(ArrayLiteral arrayLiteral, string expectedElementType, string arrayName)
  {
    for (int i = 0; i < arrayLiteral.Elements.Count; i++)
    {
      var element = arrayLiteral.Elements[i];
      if (element is LiteralExpression literal)
      {
        var actualType = literal.Type;
        if (actualType != expectedElementType)
        {
          throw new InvalidOperationException($"Type mismatch in array '{arrayName}' at index {i}. Expected '{expectedElementType}' but found '{actualType}' for element '{literal.Value}'.");
        }
      }
      // Add more checks for other expression types if needed
    }
  }

  private bool IsRawBashStatement(string line)
  {
    // Common bash patterns that should not be parsed as Utah syntax
    return line.StartsWith("if [ ") ||
           line.StartsWith("while [ ") ||
           line.StartsWith("elif [ ") ||
           line.StartsWith("until [ ") ||
           line == "fi" ||
           line == "done" ||
           line == "else" ||
           line.StartsWith("case ") ||
           line.StartsWith("esac") ||
           line.StartsWith("echo ") ||
           line.StartsWith("export ") ||
           line.StartsWith("source ") ||
           line.StartsWith(". ") ||
           line.Contains("() {") ||  // Function definition
           line.StartsWith("#!/") ||  // Shebang
           line.StartsWith("set ") ||
           line.StartsWith("unset ") ||
           line.StartsWith("alias ") ||
           line.StartsWith("cd ") ||
           line.StartsWith("pwd") ||
           line.StartsWith("ls ") ||
           line.StartsWith("cat ") ||
           line.StartsWith("grep ") ||
           line.StartsWith("sed ") ||
           line.StartsWith("awk ") ||
           line.StartsWith("find ") ||
           line.StartsWith("xargs ") ||
           line.StartsWith("sort ") ||
           line.StartsWith("uniq ") ||
           line.StartsWith("head ") ||
           line.StartsWith("tail ") ||
           line.StartsWith("wc ") ||
           line.StartsWith("tr ") ||
           line.StartsWith("cut ") ||
           line.StartsWith("join ") ||
           line.StartsWith("paste ") ||
           line.StartsWith("tee ") ||
           line.StartsWith("xargs ") ||
           line.StartsWith("mkdir ") ||
           line.StartsWith("rmdir ") ||
           line.StartsWith("rm ") ||
           line.StartsWith("cp ") ||
           line.StartsWith("mv ") ||
           line.StartsWith("ln ") ||
           line.StartsWith("chmod ") ||
           line.StartsWith("chown ") ||
           line.StartsWith("touch ") ||
           line.EndsWith("=$((") ||  // Arithmetic assignments
           (line.Contains("=") && !line.StartsWith("let ") && !line.StartsWith("const ") && !line.Contains(" = ")) ||  // Variable assignments (bash style without spaces)
           Regex.IsMatch(line, @"^\w+=[^=\s].*") ||  // Simple variable assignment (bash style without spaces)
           line.Contains("$((") ||  // Arithmetic substitution
           line.Contains("$(") ||   // Command substitution
           line.Contains("${") ||   // Parameter expansion
           line.Contains("&&") ||   // Logical AND
           line.Contains("||") ||   // Logical OR
           line.Contains(">>") ||   // Append redirection
           line.Contains("<<") ||   // Here document
           line.Contains("2>&1") || // Error redirection
           line.Contains(">/dev/null") || // Null redirection
           line.Contains("|");      // Pipe
  }
}
