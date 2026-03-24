using System.Text.RegularExpressions;
using System.Text;
using System.Linq;

public partial class Parser
{
  private string[] _lines;
  private readonly HashSet<string> _constVariables = new HashSet<string>();
  private readonly Stack<UtahType> _functionReturnTypeStack = new Stack<UtahType>();
  private readonly Dictionary<string, StructuredTypeDeclaration> _structuredTypes = new(StringComparer.Ordinal);
  private readonly Stack<Dictionary<string, UtahType>> _variableTypeScopes = new();
  private int _currentLineIndex;
  private int[] _originalLineNumbers = Array.Empty<int>();

  public Parser(string input)
  {
    // Pre-process to remove comments, but preserve strings
    var processed = RemoveComments(input);
    var allLines = processed.Split('\n');

    var lines = new List<string>();
    var lineMap = new List<int>();
    for (int idx = 0; idx < allLines.Length; idx++)
    {
      var trimmed = allLines[idx].Trim();
      if (!string.IsNullOrEmpty(trimmed))
      {
        lines.Add(trimmed);
        lineMap.Add(idx);
      }
    }
    _lines = lines.ToArray();
    _originalLineNumbers = lineMap.ToArray();
    _variableTypeScopes.Push(new Dictionary<string, UtahType>(StringComparer.Ordinal));
  }

  public int GetOriginalLineNumber()
  {
    if (_currentLineIndex >= 0 && _currentLineIndex < _originalLineNumbers.Length)
      return _originalLineNumbers[_currentLineIndex];
    return 0;
  }

  public record ParseError(int Line, string Message);

  public (ProgramNode? Program, List<ParseError> Errors) TryParse()
  {
    try
    {
      var result = Parse();
      return (result, new List<ParseError>());
    }
    catch (Exception ex)
    {
      var line = GetOriginalLineNumber();
      return (null, new List<ParseError> { new ParseError(line, ex.Message) });
    }
  }

  private string RemoveComments(string input)
  {
    var result = new StringBuilder();
    bool inString = false;
    char stringChar = '\0';

    for (int i = 0; i < input.Length; i++)
    {
      char current = input[i];

      // Handle string literals and interpolated strings
      if (!inString && (current == '"' || current == '\'' || current == '`'))
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
            {
              if (input[i] == '\n')
                result.Append('\n'); // Preserve newlines for line number tracking
              i++;
            }
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
    // Preprocess lines to handle malformed code
    _lines = PreprocessMalformedCode(_lines);

    var statements = new List<Statement>();

    for (int i = 0; i < _lines.Length; i++)
    {
      _currentLineIndex = i;
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
    _currentLineIndex = i;
    var sourceLine = GetOriginalLineNumber() + 1; // 1-based

    var result = ParseStatementInner(line, ref i);

    if (result != null && result.SourceLine < 0)
    {
      result.SourceLine = sourceLine;
      result.SourceText = line;
    }

    return result;
  }

  private Statement? ParseStatementInner(string line, ref int i)
  {
    if (line.StartsWith("import "))
    {
      return ParseImportStatement(line);
    }
    if (line.StartsWith("interface "))
    {
      return ParseStructuredTypeDeclaration(line, ref i, isRecord: false);
    }
    if (line.StartsWith("record "))
    {
      return ParseStructuredTypeDeclaration(line, ref i, isRecord: true);
    }
    if (line.StartsWith("let ") || line.StartsWith("const "))
    {
      return ParseVariableDeclaration(line, ref i);
    }
    if (line.StartsWith("function "))
    {
      return ParseFunctionDeclaration(line, ref i);
    }
    if (line.StartsWith("if(") || line.StartsWith("if ("))
    {
      return ParseIfStatement(line, ref i);
    }
    if (line.StartsWith("for(") || line.StartsWith("for ("))
    {
      // Disambiguate between for and for-in loops
      var forInMatch = Regex.Match(line, @"for\s*\((let|const)\s+(\w+)(?::\s*(\w+))?\s+in\s+(.+)\)\s*\{");
      if (forInMatch.Success)
      {
        return ParseForInLoop(line, ref i);
      }
      return ParseForLoop(line, ref i);
    }
    if (line.StartsWith("while(") || line.StartsWith("while ("))
    {
      return ParseWhileStatement(line, ref i);
    }
    if (line.StartsWith("switch(") || line.StartsWith("switch ("))
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

      // Validate return type if we're inside a function
      if (_functionReturnTypeStack.Count > 0)
      {
        var expectedReturnType = _functionReturnTypeStack.Peek();
        ValidateReturnType(value, expectedReturnType);
      }

      return new ReturnStatement(value);
    }
    if (line == "return;")
    {
      // Validate void return if we're inside a function
      if (_functionReturnTypeStack.Count > 0)
      {
        var expectedReturnType = _functionReturnTypeStack.Peek();
        if (expectedReturnType is not VoidType)
        {
          throw new InvalidOperationException($"Function expects return type '{expectedReturnType}' but found void return.");
        }
      }

      return new ReturnStatement(null);
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
    if (line.StartsWith("array.forEach"))
    {
      // Parse multi-line array.forEach statements
      // Handle the opening line: array.forEach(array, (params) => {
      var forEachMatch = Regex.Match(line, @"array\.forEach\((.+),\s*\(([^)]*)\)\s*=>\s*\{");
      if (forEachMatch.Success)
      {
        var arrayArg = forEachMatch.Groups[1].Value.Trim();
        var paramsStr = forEachMatch.Groups[2].Value.Trim();

        // Parse lambda parameters
        var parameters = new List<string>();
        if (!string.IsNullOrEmpty(paramsStr))
        {
          parameters.AddRange(paramsStr.Split(',').Select(p => p.Trim()));
        }

        // Parse the lambda body (multi-line)
        var body = new List<Statement>();
        i++; // Move to next line

        while (i < _lines.Length && !_lines[i].Trim().StartsWith("}"))
        {
          var bodyLine = _lines[i].Trim();
          if (!string.IsNullOrEmpty(bodyLine) && !bodyLine.StartsWith("//"))
          {
            // Avoid infinite recursion by creating a temporary parser for the body
            var tempParser = new Parser(bodyLine);
            var tempProgram = tempParser.Parse();
            if (tempProgram.Statements.Count > 0)
            {
              body.Add(tempProgram.Statements[0]);
            }
          }
          i++;
        }

        // Skip the closing }); line
        if (i < _lines.Length && _lines[i].Trim().StartsWith("}"))
        {
          i++;
        }

        var arrayExpr = ParseExpression(arrayArg);
        var lambdaExpr = new LambdaExpression(parameters, body);
        return new ExpressionStatement(new ArrayForEachExpression(arrayExpr, lambdaExpr));
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
    if (line.StartsWith("template.update("))
    {
      return ParseTemplateUpdateStatement(line);
    }
    if (line.StartsWith("defer "))
    {
      return ParseDeferStatement(line);
    }
    if (line.StartsWith("scheduler.cron("))
    {
      return ParseSchedulerCronStatement(line, ref i);
    }
    if (line == "git.undoLastCommit();")
    {
      return new ExpressionStatement(new GitUndoLastCommitExpression());
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
      if (expression is FsCopyExpression copyExpr)
      {
        return new FsCopyStatement(copyExpr.SourcePath, copyExpr.TargetPath);
      }
      if (expression is FsMoveExpression moveExpr)
      {
        return new FsMoveStatement(moveExpr.SourcePath, moveExpr.TargetPath);
      }
      if (expression is FsRenameExpression renameExpr)
      {
        return new FsRenameStatement(renameExpr.OldName, renameExpr.NewName);
      }
      if (expression is FsDeleteExpression deleteExpr)
      {
        return new FsDeleteStatement(deleteExpr.Path);
      }
      if (expression is ArrayForEachExpression forEachExpr)
      {
        return new ExpressionStatement(forEachExpr);
      }
      return new ExpressionStatement(expression);
    }
    catch (InvalidOperationException)
    {
      throw;
    }
    catch
    {
      // Never convert Utah variable declarations to raw statements
      if (line.StartsWith("let ") || line.StartsWith("const "))
      {
        throw; // Re-throw the original exception
      }

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
      var match = Regex.Match(fullDeclaration, @"(let|const)\s+(\w+)(?:\s*:\s*([^=]+?))?\s*=\s*(.+)\s*;");
      if (match.Success)
      {
        var name = match.Groups[2].Value;
        var type = UtahType.Parse(match.Groups[3].Value);
        var valueStr = match.Groups[4].Value;

        if (isConst)
        {
          _constVariables.Add(name);
        }

        var value = ParseExpression(valueStr);

        if (type != null) ValidateVariableTypeAssignment(value, type, name);
        var resolvedType = type ?? InferExpressionType(value);
        RegisterVariableType(name, resolvedType);

        return new VariableDeclaration(name, type, value, isConst);
      }
      throw new Exception($"Invalid multi-line variable declaration: {fullDeclaration}");
    }

    // Check if this is a multiline string literal starting with """
    if (line.Contains("= \"\"\"") && !line.TrimEnd().EndsWith("\"\"\";"))
    {
      // This is a multi-line string literal, collect all lines until we find the closing """
      var fullDeclaration = line;
      var originalI = i;

      i++;
      while (i < _lines.Length)
      {
        var nextLine = _lines[i];
        fullDeclaration += "\n" + nextLine;

        if (nextLine.TrimEnd().EndsWith("\"\"\";"))
        {
          break;
        }
        i++;
      }

      // Ensure we found the closing """ - if not, this might not be a multiline string
      if (i >= _lines.Length)
      {
        // Reset index and fall through to regular parsing
        i = originalI;
      }
      else
      {
        // Now parse the complete declaration
        var match = Regex.Match(fullDeclaration, @"(let|const)\s+(\w+)(?:\s*:\s*([^=]+?))?\s*=\s*(.+)\s*;", RegexOptions.Singleline);
        if (match.Success)
        {
          var name = match.Groups[2].Value;
          var type = UtahType.Parse(match.Groups[3].Value);
          var valueStr = match.Groups[4].Value;

          if (isConst)
          {
            _constVariables.Add(name);
          }

          // Ensure the value string represents a multiline string
          if (valueStr.StartsWith("\"\"\"") && valueStr.EndsWith("\"\"\""))
          {
            var value = ParseExpression(valueStr);

            if (type != null) ValidateVariableTypeAssignment(value, type, name);
            var resolvedType = type ?? InferExpressionType(value);
            RegisterVariableType(name, resolvedType);

            return new VariableDeclaration(name, type, value, isConst);
          }
        }

        // If we get here, the multiline parsing failed, reset and fall through
        i = originalI;
      }
    }

    // Single-line variable declaration
    var singleLineMatch = Regex.Match(line, @"(let|const)\s+(\w+)(?:\s*:\s*([^=]+?))?\s*=\s*(.+)\s*;");
    if (singleLineMatch.Success)
    {
      var name = singleLineMatch.Groups[2].Value;
      var type = UtahType.Parse(singleLineMatch.Groups[3].Value);
      var valueStr = singleLineMatch.Groups[4].Value;

      if (isConst)
      {
        _constVariables.Add(name);
      }

      var value = ParseExpression(valueStr);

      if (type != null) ValidateVariableTypeAssignment(value, type, name);
      var resolvedType = type ?? InferExpressionType(value);
      RegisterVariableType(name, resolvedType);

      return new VariableDeclaration(name, type, value, isConst);
    }
    throw new Exception($"Invalid variable declaration: {line}");
  }

  private VariableDeclaration ParseSingleLineVariableDeclaration(string line)
  {
    var isConst = line.StartsWith("const ");
    var match = Regex.Match(line, @"(let|const)\s+(\w+)(?:\s*:\s*([^=]+?))?\s*=\s*(.+)\s*;");
    if (match.Success)
    {
      var name = match.Groups[2].Value;
      var type = UtahType.Parse(match.Groups[3].Value);
      var valueStr = match.Groups[4].Value;

      if (isConst)
      {
        _constVariables.Add(name);
      }

      var value = ParseExpression(valueStr);

      if (type != null) ValidateVariableTypeAssignment(value, type, name);
      var resolvedType = type ?? InferExpressionType(value);
      RegisterVariableType(name, resolvedType);

      return new VariableDeclaration(name, type, value, isConst);
    }
    throw new Exception($"Invalid variable declaration: {line}");
  }

  private FunctionDeclaration ParseFunctionDeclaration(string line, ref int i)
  {
    var headerMatch = Regex.Match(line, @"function\s+(\w+)\s*\(([^)]*)\)\s*(?::\s*([^{]+))?\s*\{");
    if (!headerMatch.Success) throw new Exception("Invalid function declaration");

    var name = headerMatch.Groups[1].Value;
    var returnType = UtahType.Parse(headerMatch.Groups[3].Value);
    var parameters = new List<(string Name, UtahType Type)>();
    var paramList = SplitByComma(headerMatch.Groups[2].Value)
      .Where(p => !string.IsNullOrWhiteSpace(p))
      .ToList();
    foreach (var p in paramList)
    {
      var colonIndex = p.IndexOf(':');
      var paramName = colonIndex >= 0 ? p[..colonIndex].Trim() : p.Trim();
      var paramType = colonIndex >= 0 ? UtahType.Parse(p[(colonIndex + 1)..].Trim()) ?? UtahType.Any : UtahType.Any;
      parameters.Add((paramName, paramType));
    }

    // Push return type onto stack for validation
    _functionReturnTypeStack.Push(returnType ?? UtahType.Void);
    PushVariableTypeScope();
    foreach (var (paramName, paramType) in parameters)
    {
      RegisterVariableType(paramName, paramType);
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

    // Pop return type from stack
    _functionReturnTypeStack.Pop();
    PopVariableTypeScope();

    return new FunctionDeclaration(name, parameters, body, returnType);
  }

  private StructuredTypeDeclaration ParseStructuredTypeDeclaration(string line, ref int i, bool isRecord)
  {
    var headerMatch = Regex.Match(line, @"(interface|record)\s+(\w+)\s*\{(.*)");
    if (!headerMatch.Success)
      throw new InvalidOperationException($"Invalid {(isRecord ? "record" : "interface")} declaration: {line}");

    var name = headerMatch.Groups[2].Value;
    var inlineBody = headerMatch.Groups[3].Value;
    var bodyBuilder = new StringBuilder();

    if (inlineBody.Contains("}"))
    {
      bodyBuilder.Append(inlineBody[..inlineBody.LastIndexOf('}')]);
    }
    else
    {
      i++;
      while (i < _lines.Length)
      {
        var currentLine = _lines[i].Trim();

        if (currentLine.StartsWith("}"))
        {
          break;
        }

        if (currentLine.Contains("}"))
        {
          bodyBuilder.AppendLine(currentLine[..currentLine.IndexOf('}')]);
          break;
        }

        bodyBuilder.AppendLine(currentLine);
        i++;
      }

      if (i >= _lines.Length)
      {
        throw new InvalidOperationException($"Unterminated {(isRecord ? "record" : "interface")} declaration for '{name}'.");
      }
    }

    var fields = ParseStructuredTypeFields(bodyBuilder.ToString());
    var declaration = new StructuredTypeDeclaration(name, fields, isRecord);
    _structuredTypes[name] = declaration;
    return declaration;
  }

  private List<StructuredTypeField> ParseStructuredTypeFields(string body)
  {
    var fields = new List<StructuredTypeField>();
    var entries = body
      .Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
      .Where(entry => !string.IsNullOrWhiteSpace(entry));

    foreach (var entry in entries)
    {
      var colonIndex = entry.IndexOf(':');
      if (colonIndex <= 0)
      {
        throw new InvalidOperationException($"Invalid structured type field declaration: {entry}");
      }

      var fieldName = entry[..colonIndex].Trim();
      var fieldType = UtahType.Parse(entry[(colonIndex + 1)..].Trim());

      if (string.IsNullOrEmpty(fieldName) || fieldType == null)
      {
        throw new InvalidOperationException($"Invalid structured type field declaration: {entry}");
      }

      if (fields.Any(field => field.Name == fieldName))
      {
        throw new InvalidOperationException($"Duplicate field '{fieldName}' in structured type declaration.");
      }

      fields.Add(new StructuredTypeField(fieldName, fieldType));
    }

    return fields;
  }

  private IfStatement ParseIfStatement(string line, ref int i)
  {
    var match = Regex.Match(line, @"if\s*\((.+)\)\s*\{");
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
          if (Regex.IsMatch(nextLine, @"^else\s*\{"))
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
          else if (Regex.IsMatch(nextLine, @"^else\s+if\s*\("))
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
      if (innerLine == "}" || Regex.IsMatch(innerLine, @"^\}\s*else\s*\{") || Regex.IsMatch(innerLine, @"^\}\s*else\s+if\s*\("))
      {
        // Handle the else clause if present on the same line
        if (Regex.IsMatch(innerLine, @"^\}\s*else\s*\{"))
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
        else if (Regex.IsMatch(innerLine, @"^\}\s*else\s+if\s*\("))
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

      if (Regex.IsMatch(nextLine, @"^else\s+if\s*\("))
      {
        // Handle else if as a nested if statement
        i++; // Move to the else if line
        var elseIfStatement = ParseIfStatement(nextLine, ref i);
        elseBody.Add(elseIfStatement);
      }
      else if (Regex.IsMatch(nextLine, @"^else\s*\{"))
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
    var forInMatch = Regex.Match(line, @"for\s*\((let|const)\s+(\w+)(?::\s*(\w+))?\s+in\s+(.+)\)\s*\{");
    if (forInMatch.Success)
    {
      var variable = forInMatch.Groups[2].Value;
      var iterable = ParseExpression(forInMatch.Groups[4].Value); // Back to Groups[4] since we removed (in|of) group
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
    var forMatch = Regex.Match(line, @"for\s*\((.+);\s*(.+);\s*(.+)\)\s*\{");
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
    var match = Regex.Match(line, @"while\s*\((.+)\)\s*\{");
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
    var match = Regex.Match(line, @"switch\s*\((.+)\)\s*\{");
    if (!match.Success) throw new Exception("Invalid switch statement");

    var expression = ParseExpression(match.Groups[1].Value);
    var cases = new List<CaseClause>();
    DefaultClause? defaultClause = null;

    i++;
    while (i < _lines.Length && _lines[i].Trim() != "}")
    {
      var innerLine = _lines[i].Trim();
      if (innerLine.StartsWith("case ") || innerLine.StartsWith("case\"") || innerLine.StartsWith("case'"))
      {
        // Collect all consecutive case values (for fall-through)
        var values = new List<Expression>();
        while (i < _lines.Length && (_lines[i].Trim().StartsWith("case ") || _lines[i].Trim().StartsWith("case\"") || _lines[i].Trim().StartsWith("case'")))
        {
          var caseMatch = Regex.Match(_lines[i].Trim(), @"case\s*(.+):");
          values.Add(ParseExpression(caseMatch.Groups[1].Value));
          i++;
        }

        // Now parse the body until we hit a break, another case, default, or end
        var body = new List<Statement>();
        bool hasBreak = false;
        while (i < _lines.Length && !_lines[i].Trim().StartsWith("case ") && !_lines[i].Trim().StartsWith("case\"") && !_lines[i].Trim().StartsWith("case'") && !_lines[i].Trim().StartsWith("default:") && _lines[i].Trim() != "}")
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
      var type = parameters.Count > 3 ? UtahType.Parse(StripQuotes(parameters[3])) ?? UtahType.String : UtahType.String;
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

  private Statement ParseTemplateUpdateStatement(string line)
  {
    // Handle template.update() with parameters
    if (line.StartsWith("template.update(") && line.EndsWith(");"))
    {
      var paramStart = line.IndexOf('(') + 1;
      var paramEnd = line.LastIndexOf(')');
      var parametersStr = line.Substring(paramStart, paramEnd - paramStart).Trim();

      // Parse the parameters - expecting: sourceFilePath, targetFilePath
      var parameters = ParseFunctionParameters(parametersStr);

      if (parameters.Count != 2)
        throw new Exception("template.update() requires exactly 2 parameters: sourceFilePath, targetFilePath");

      var sourceFilePathExpr = ParseExpression(parameters[0]);
      var targetFilePathExpr = ParseExpression(parameters[1]);

      return new TemplateUpdateStatement(sourceFilePathExpr, targetFilePathExpr);
    }

    return new RawStatement(line);
  }

  private Statement ParseDeferStatement(string line)
  {
    // Extract the statement after "defer "
    var deferredStatement = line.Substring(5).Trim(); // Remove "defer "

    if (string.IsNullOrEmpty(deferredStatement))
    {
      throw new Exception("defer statement cannot be empty");
    }

    // Parse the deferred statement recursively
    int tempIndex = 0;
    var statement = ParseStatement(deferredStatement, ref tempIndex);

    if (statement == null)
    {
      throw new Exception($"Invalid statement in defer: {deferredStatement}");
    }

    return new DeferStatement(statement);
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

  private void PushVariableTypeScope()
  {
    _variableTypeScopes.Push(new Dictionary<string, UtahType>(StringComparer.Ordinal));
  }

  private void PopVariableTypeScope()
  {
    if (_variableTypeScopes.Count > 1)
    {
      _variableTypeScopes.Pop();
    }
  }

  private void RegisterVariableType(string variableName, UtahType? type)
  {
    if (string.IsNullOrWhiteSpace(variableName) || type == null || _variableTypeScopes.Count == 0)
    {
      return;
    }

    _variableTypeScopes.Peek()[variableName] = type;
  }

  private UtahType? LookupVariableType(string variableName)
  {
    foreach (var scope in _variableTypeScopes)
    {
      if (scope.TryGetValue(variableName, out var type))
      {
        return type;
      }
    }

    return null;
  }

  private void ValidateArrayElementTypes(ArrayLiteral arrayLiteral, UtahType expectedElementType, string arrayName)
  {
    for (int i = 0; i < arrayLiteral.Elements.Count; i++)
    {
      if (!IsTypeCompatible(arrayLiteral.Elements[i], expectedElementType))
      {
        var actualType = InferExpressionType(arrayLiteral.Elements[i]);
        throw new InvalidOperationException($"Type mismatch in array '{arrayName}' at index {i}. Expected '{expectedElementType}' but found '{actualType?.ToString() ?? "unresolved"}'.");
      }
    }
  }

  private void ValidateVariableTypeAssignment(Expression value, UtahType expectedType, string variableName)
  {
    if (!IsTypeCompatible(value, expectedType))
    {
      var actualType = InferExpressionType(value);
      throw new InvalidOperationException($"Type mismatch in variable '{variableName}'. Expected '{expectedType}' but found '{actualType?.ToString() ?? "unresolved"}'.");
    }
  }

  private void ValidateReturnType(Expression? value, UtahType expectedReturnType)
  {
    if (expectedReturnType is VoidType)
      return;

    if (value == null)
    {
      throw new InvalidOperationException($"Function expects return type '{expectedReturnType}' but found void return.");
    }

    if (!IsTypeCompatible(value, expectedReturnType))
    {
      var actualType = InferExpressionType(value);
      throw new InvalidOperationException($"Function return type mismatch. Expected '{expectedReturnType}' but found '{actualType?.ToString() ?? "unresolved"}'.");
    }
  }

  private UtahType? InferExpressionType(Expression expression)
  {
    return expression switch
    {
      LiteralExpression literal => literal.Type,
      MultilineStringExpression => UtahType.String,
      ArrayLiteral arrayLiteral => new ArrayType(arrayLiteral.ElementType),
      ObjectLiteralExpression => UtahType.Object,
      VariableExpression variable => LookupVariableType(variable.Name),
      SshConnectExpression => UtahType.SshConnection,
      JsonParseExpression => UtahType.Object,
      YamlParseExpression => UtahType.Object,
      JsonGetExpression => null,
      YamlGetExpression => null,
      ArrayMapExpression => new ArrayType(UtahType.Any),
      ArrayFilterExpression filterExpression => InferExpressionType(filterExpression.Array),
      ArrayFindExpression findExpression => GetCollectionElementType(InferExpressionType(findExpression.Array)),
      ArrayReduceExpression => null,
      ArraySomeExpression => UtahType.Boolean,
      ArrayEveryExpression => UtahType.Boolean,
      FunctionCall functionCall => InferFunctionCallType(functionCall),
      ObjectPropertyAccessExpression propertyAccess => InferObjectPropertyType(propertyAccess),
      _ => null
    };
  }

  private UtahType? InferFunctionCallType(FunctionCall functionCall)
  {
    return functionCall.Name switch
    {
      "schema.validate" => functionCall.Arguments.Count >= 2
        ? UtahType.Parse(ResolveSchemaName(functionCall.Arguments[1])) ?? UtahType.Object
        : UtahType.Object,
      "schema.isValid" => UtahType.Boolean,
      "map.get" or "dictionary.get" => functionCall.Arguments.Count > 0
        ? GetMapValueType(InferExpressionType(functionCall.Arguments[0]))
        : null,
      "map.has" or "dictionary.has" => UtahType.Boolean,
      "set.has" => UtahType.Boolean,
      "set.add" or "set.remove" or "set.union" or "set.intersection" or "set.difference" => functionCall.Arguments.Count > 0
        ? InferExpressionType(functionCall.Arguments[0])
        : null,
      _ => null
    };
  }

  private UtahType? InferObjectPropertyType(ObjectPropertyAccessExpression propertyAccess)
  {
    if (!TryGetPropertyPath(propertyAccess, out var rootName, out var path))
    {
      return null;
    }

    UtahType? currentType = LookupVariableType(rootName);
    if (currentType == null) return null;

    foreach (var segment in path)
    {
      currentType = GetStructuredFieldType(currentType, segment);
      if (currentType == null) return null;
    }

    return currentType;
  }

  private bool TryGetPropertyPath(Expression expression, out string rootName, out List<string> path)
  {
    rootName = "";
    path = new List<string>();

    if (expression is VariableExpression variableExpression)
    {
      rootName = variableExpression.Name;
      return true;
    }

    if (expression is ObjectPropertyAccessExpression propertyAccess &&
        TryGetPropertyPath(propertyAccess.Object, out rootName, out path))
    {
      path.Add(propertyAccess.PropertyName);
      return true;
    }

    return false;
  }

  private UtahType? GetStructuredFieldType(UtahType? typeName, string fieldName)
  {
    if (typeName is StructuredType st && _structuredTypes.TryGetValue(st.Name, out var declaration))
    {
      return declaration.Fields.FirstOrDefault(field => field.Name == fieldName)?.Type;
    }

    if (typeName is MapType or DictionaryType)
    {
      return GetMapValueType(typeName);
    }

    return null;
  }

  private bool IsTypeCompatible(Expression value, UtahType expectedType)
  {
    if (expectedType is AnyType)
    {
      return true;
    }

    if (value is ArrayLiteral arrayLiteral)
    {
      if (expectedType is ArrayType arrayExpected)
      {
        ValidateArrayElementTypes(arrayLiteral, arrayExpected.ElementType, "array literal");
        return true;
      }

      if (expectedType is SetType setExpected)
      {
        foreach (var element in arrayLiteral.Elements)
        {
          if (!IsTypeCompatible(element, setExpected.ElementType))
          {
            return false;
          }
        }

        return true;
      }

      return false;
    }

    if (value is ObjectLiteralExpression objectLiteral)
    {
      if (expectedType is ObjectType)
      {
        return true;
      }

      if (expectedType is StructuredType st && _structuredTypes.ContainsKey(st.Name))
      {
        return ValidateStructuredObjectLiteral(objectLiteral, st.Name);
      }

      if (expectedType is MapType or DictionaryType)
      {
        var valueType = GetMapValueType(expectedType) ?? UtahType.Any;
        return objectLiteral.Properties.All(property => IsTypeCompatible(property.Value, valueType));
      }

      return false;
    }

    if (value is JsonParseExpression or YamlParseExpression)
    {
      return expectedType is ObjectType ||
             (expectedType is StructuredType st2 && _structuredTypes.ContainsKey(st2.Name)) ||
             expectedType is MapType ||
             expectedType is DictionaryType;
    }

    var actualType = InferExpressionType(value);
    if (actualType == null || actualType is UnknownType or AnyType)
    {
      return true;
    }

    if (actualType == expectedType)
    {
      return true;
    }

    if (expectedType is ObjectType &&
        (actualType is ObjectType or SshConnectionType or MapType or DictionaryType ||
         (actualType is StructuredType stActual && _structuredTypes.ContainsKey(stActual.Name))))
    {
      return true;
    }

    if (expectedType is StructuredType stExpected && _structuredTypes.ContainsKey(stExpected.Name) && actualType is ObjectType)
    {
      return true;
    }

    if (expectedType is MapType or DictionaryType && actualType is ObjectType)
    {
      return true;
    }

    if (expectedType is ArrayType expectedArray && actualType is ArrayType actualArray)
    {
      return expectedArray.ElementType is AnyType || actualArray.ElementType is UnknownType or AnyType || expectedArray.ElementType == actualArray.ElementType;
    }

    if (expectedType is SetType expectedSet && actualType is SetType actualSet)
    {
      return expectedSet.ElementType is AnyType || actualSet.ElementType is UnknownType or AnyType || expectedSet.ElementType == actualSet.ElementType;
    }

    if (expectedType is MapType or DictionaryType && actualType is MapType or DictionaryType)
    {
      return GetMapValueType(expectedType) == GetMapValueType(actualType);
    }

    return false;
  }

  private bool ValidateStructuredObjectLiteral(ObjectLiteralExpression objectLiteral, string structuredTypeName)
  {
    var declaration = _structuredTypes[structuredTypeName];

    foreach (var field in declaration.Fields)
    {
      var property = objectLiteral.Properties.FirstOrDefault(candidate => candidate.Name == field.Name);
      if (property == null)
      {
        throw new InvalidOperationException($"Missing required field '{field.Name}' for type '{structuredTypeName}'.");
      }

      if (!IsTypeCompatible(property.Value, field.Type))
      {
        var actualType = InferExpressionType(property.Value);
        throw new InvalidOperationException($"Field '{field.Name}' in type '{structuredTypeName}' expects '{field.Type}' but found '{actualType?.ToString() ?? "unresolved"}'.");
      }
    }

    foreach (var property in objectLiteral.Properties)
    {
      if (!declaration.Fields.Any(field => field.Name == property.Name))
      {
        throw new InvalidOperationException($"Unknown field '{property.Name}' for type '{structuredTypeName}'.");
      }
    }

    return true;
  }

  private static UtahType? GetCollectionElementType(UtahType? type)
  {
    return type switch
    {
      ArrayType at => at.ElementType,
      SetType st => st.ElementType,
      _ => null
    };
  }

  private static UtahType? GetMapValueType(UtahType? type)
  {
    return type switch
    {
      MapType mt => mt.ValueType,
      DictionaryType dt => dt.ValueType,
      _ => null
    };
  }

  private string? ResolveSchemaName(Expression expression)
  {
    return expression switch
    {
      VariableExpression variable => variable.Name,
      LiteralExpression literal when literal.Type == UtahType.String => literal.Value,
      _ => null
    };
  }

  private bool IsRawBashStatement(string line)
  {
    // Don't treat Utah function calls as raw bash even if they contain command substitution
    if (Regex.IsMatch(line.Trim(), @"^\w+\.\w+\s*\(.*\)\s*;?\s*$"))
    {
      return false;
    }

    // Common bash patterns that should not be parsed as Utah syntax
    return line.StartsWith("if [ ") ||
           line.StartsWith("while [ ") ||
           line.StartsWith("elif [ ") ||
           line.StartsWith("until [ ") ||
           line == "fi" ||
           line == "done" ||
           line == "else" ||
           line.StartsWith("case ") ||
           line.StartsWith("case\"") ||
           line.StartsWith("case'") ||
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
           line.StartsWith("git ") ||
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

  private ImportStatement ParseImportStatement(string line)
  {
    var match = Regex.Match(line, @"import\s+([""']?)([^""';]+)\1;?");
    if (!match.Success)
    {
      throw new InvalidOperationException($"Invalid import statement: {line}");
    }

    var filePath = match.Groups[2].Value;
    return new ImportStatement(filePath);
  }

  private Statement ParseSchedulerCronStatement(string line, ref int i)
  {
    // Extract the first part: scheduler.cron("pattern",
    var match = Regex.Match(line, @"scheduler\.cron\s*\(\s*(.+?)\s*,\s*\(\s*\)\s*=>\s*\{");
    if (!match.Success)
    {
      throw new InvalidOperationException($"Invalid scheduler.cron statement: {line}");
    }

    var cronPattern = match.Groups[1].Value.Trim();
    var cronPatternExpr = ParseExpression(cronPattern);

    // Now read the lambda body until we find the closing braces
    var lambdaBody = new List<Statement>();
    i++; // Move to next line after the opening line

    int braceDepth = 1; // We already have one opening brace from the lambda
    while (i < _lines.Length && braceDepth > 0)
    {
      var bodyLine = _lines[i].Trim();

      // Count braces to handle nested blocks
      foreach (char c in bodyLine)
      {
        if (c == '{') braceDepth++;
        else if (c == '}') braceDepth--;
      }

      // If we're still inside the lambda body, parse the line as a statement
      if (braceDepth > 0 && !string.IsNullOrEmpty(bodyLine))
      {
        var statement = ParseStatement(bodyLine, ref i);
        if (statement != null)
        {
          lambdaBody.Add(statement);
        }
      }

      i++;
    }

    // Move back one line since the main loop will increment i
    i--;

    var lambda = new LambdaExpression(new List<string>(), lambdaBody);
    var schedulerCron = new SchedulerCronExpression(cronPatternExpr, lambda);

    return new ExpressionStatement(schedulerCron);
  }

  private string[] PreprocessMalformedCode(string[] lines)
  {
    var result = new List<string>();

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];
      var trimmedLine = line.Trim();

      // Handle }else { patterns - split into separate } and else { lines
      if (Regex.IsMatch(trimmedLine, @"^\}\s*else\s*\{"))
      {
        result.Add("}");
        result.Add("else {");
        continue;
      }

      // Only normalize critical parsing patterns, don't change spacing of content
      if (!string.IsNullOrEmpty(trimmedLine))
      {
        // Only fix keyword patterns that break parsing recognition
        if (Regex.IsMatch(trimmedLine, @"^if\s*\("))
        {
          // Ensure if statements have proper space for recognition
          trimmedLine = Regex.Replace(trimmedLine, @"^if\s*\(", "if (");
        }
        else if (Regex.IsMatch(trimmedLine, @"^function\s*\w"))
        {
          // Ensure function declarations have proper space for recognition
          trimmedLine = Regex.Replace(trimmedLine, @"^function\s+", "function ");
        }

        result.Add(trimmedLine);
      }
      else
      {
        result.Add(line); // Keep empty lines as-is
      }
    }

    return result.ToArray();
  }
}
