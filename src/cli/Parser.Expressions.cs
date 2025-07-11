using System.Text.RegularExpressions;
using System.Linq;

public partial class Parser
{
  public Expression ParseExpression(string input)
  {
    return ParseAssignmentExpression(input.Trim());
  }

  private bool IsPartOfOperator(string input, int index)
  {
    if (index > 0)
    {
      char prevChar = input[index - 1];
      if (prevChar == '!' || prevChar == '=' || prevChar == '<' || prevChar == '>')
      {
        return true;
      }
    }
    if (index < input.Length - 1)
    {
      char nextChar = input[index + 1];
      if (nextChar == '=')
      {
        return true;
      }
    }
    return false;
  }

  private Expression ParseAssignmentExpression(string input)
  {
    var parts = input.Split(new[] { '=' }, 2);
    if (parts.Length == 2 && !IsPartOfOperator(input, input.IndexOf('=')))
    {
      var left = ParseTernaryExpression(parts[0].Trim());
      var right = ParseAssignmentExpression(parts[1].Trim()); // Right-associative
      return new AssignmentExpression(left, right);
    }
    return ParseTernaryExpression(input);
  }

  private Expression ParseTernaryExpression(string input)
  {
    // Check for ternary operator (condition ? true : false)
    var parts = SplitTernary(input);
    if (parts.Count == 3)
    {
      return new TernaryExpression(
        ParseLogicalOrExpression(parts[0]),
        ParseLogicalOrExpression(parts[1]),
        ParseTernaryExpression(parts[2]) // Recursively parse for nested ternaries
      );
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
        left = new BinaryExpression(
          left,
          ParseLogicalAndExpression(parts[i]),
          "||"
        );
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
        left = new BinaryExpression(
          left,
          ParseEqualityExpression(parts[i]),
          "&&"
        );
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
        return new BinaryExpression(
          ParseRelationalExpression(parts[0]),
          ParseRelationalExpression(parts[1]),
          op
        );
      }
    }
    return ParseRelationalExpression(input);
  }

  private Expression ParseRelationalExpression(string input)
  {
    // Check longer operators first to avoid splitting on parts of compound operators
    var operators = new[] { "<=", ">=", "<", ">" };
    foreach (var op in operators)
    {
      var parts = SplitByOperator(input, op);
      if (parts.Count == 2)
      {
        return new BinaryExpression(
          ParseAdditiveExpression(parts[0]),
          ParseAdditiveExpression(parts[1]),
          op
        );
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
        left = new BinaryExpression(
          left,
          ParseMultiplicativeExpression(parts[i]),
          op
        );
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
        left = new BinaryExpression(
          left,
          ParseUnaryExpression(parts[i]),
          op
        );
      }
      return left;
    }
    return ParseUnaryExpression(input);
  }

  private Expression ParseUnaryExpression(string input)
  {
    // Pre-increment
    if (input.StartsWith("++"))
    {
      return new PreIncrementExpression(ParseUnaryExpression(input.Substring(2).Trim()));
    }
    // Pre-decrement
    if (input.StartsWith("--"))
    {
      return new PreDecrementExpression(ParseUnaryExpression(input.Substring(2).Trim()));
    }
    if (input.StartsWith("!"))
    {
      return new UnaryExpression(ParseUnaryExpression(input.Substring(1).Trim()), "!");
    }
    if (input.StartsWith("-"))
    {
      return new UnaryExpression(ParseUnaryExpression(input.Substring(1).Trim()), "-");
    }

    // Check for post-increment/decrement
    if (input.EndsWith("++"))
    {
      return new PostIncrementExpression(ParsePrimaryExpression(input.Substring(0, input.Length - 2).Trim()));
    }
    if (input.EndsWith("--"))
    {
      return new PostDecrementExpression(ParsePrimaryExpression(input.Substring(0, input.Length - 2).Trim()));
    }

    return ParsePrimaryExpression(input);
  }

  private Expression ParsePrimaryExpression(string input)
  {
    input = input.Trim();

    // Parenthesized expression
    if (input.StartsWith("(") && input.EndsWith(")"))
    {
      return new ParenthesizedExpression(ParseExpression(input.Substring(1, input.Length - 2)));
    }

    // String literal
    if (input.StartsWith("\"") && input.EndsWith("\""))
    {
      return new LiteralExpression(input.Substring(1, input.Length - 2), "string");
    }

    // Template literal
    if (input.StartsWith("`") && input.EndsWith("`"))
    {
      return ParseTemplateLiteral(input);
    }

    // Number literal
    if (double.TryParse(input, out _))
    {
      return new LiteralExpression(input, "number");
    }

    // Boolean literal
    if (input == "true" || input == "false")
    {
      return new LiteralExpression(input, "boolean");
    }

    // Array literal: [1, 2, 3] or ["a", "b", "c"]
    if (input.StartsWith("[") && input.EndsWith("]"))
    {
      var content = input.Substring(1, input.Length - 2).Trim();
      var elements = new List<Expression>();
      var elementType = "any";

      if (!string.IsNullOrEmpty(content))
      {
        var elementStrings = SplitByComma(content);
        foreach (var element in elementStrings)
        {
          var elementExpr = ParseExpression(element.Trim());
          elements.Add(elementExpr);

          // Determine element type from first element
          if (elementType == "any" && elementExpr is LiteralExpression literal)
          {
            elementType = literal.Type;
          }
        }
      }

      return new ArrayLiteral(elements, elementType);
    }

    // Array access: arrayName[index]
    if (input.Contains("[") && input.EndsWith("]"))
    {
      var bracketIndex = input.IndexOf('[');
      var arrayName = input.Substring(0, bracketIndex).Trim();
      var indexContent = input.Substring(bracketIndex + 1, input.Length - bracketIndex - 2).Trim();

      return new ArrayAccess(new VariableExpression(arrayName), ParseExpression(indexContent));
    }

    // Array/String methods: arrayName.length, arrayName.method()
    if (input.Contains("."))
    {
      var dotIndex = input.IndexOf('.');
      var objectName = input.Substring(0, dotIndex).Trim();
      var methodPart = input.Substring(dotIndex + 1).Trim();

      // Special case for web.get() - handle this as a function call
      if (objectName == "web" && methodPart.StartsWith("get(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(4, methodPart.Length - 5).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("web.get() requires a URL argument");
        }
        else
        {
          // Parse the URL argument
          var args = argsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(arg => arg.Trim())
                                .ToList();
          if (args.Count == 1)
          {
            // One argument: web.get(url)
            var urlExpr = ParseExpression(args[0]);
            return new WebGetExpression(urlExpr);
          }
          else
          {
            throw new InvalidOperationException($"web.get() accepts exactly 1 argument (URL), got {args.Count}");
          }
        }
      }

      // Handle .length property
      if (methodPart == "length" || methodPart == "length()")
      {
        var targetExpr = new VariableExpression(objectName);
        // Heuristic to guess if it's an array or string.
        // This should be improved with a symbol table in the future.
        if (IsLikelyArray(objectName))
        {
          return new ArrayLength(targetExpr);
        }
        return new StringLengthExpression(targetExpr);
      }

      // Handle .isEmpty() method
      if (methodPart == "isEmpty()")
      {
        return new ArrayIsEmpty(new VariableExpression(objectName));
      }

      // Handle .contains() method
      if (methodPart.StartsWith("contains(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(9, methodPart.Length - 10).Trim();
        var itemExpr = ParseExpression(argsContent);
        return new ArrayContains(new VariableExpression(objectName), itemExpr);
      }

      // Handle .reverse() method
      if (methodPart == "reverse()")
      {
        return new ArrayReverse(new VariableExpression(objectName));
      }

      // Handle .join() method
      if (methodPart.StartsWith("join(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(5, methodPart.Length - 6).Trim();
        var separatorExpr = ParseExpression(argsContent);
        return new ArrayJoinExpression(new VariableExpression(objectName), separatorExpr);
      }

      // Handle .split() method for strings
      if (methodPart.StartsWith("split(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(6, methodPart.Length - 7).Trim();
        var separatorExpr = ParseExpression(argsContent);
        var targetExpr = ParseExpression(objectName);
        return new StringSplitExpression(targetExpr, separatorExpr);
      }

      // Handle .toUpperCase() method for strings
      if (methodPart == "toUpperCase()")
      {
        var targetExpr = ParseExpression(objectName);
        return new StringToUpperCaseExpression(targetExpr);
      }

      // Handle .toLowerCase() method for strings
      if (methodPart == "toLowerCase()")
      {
        var targetExpr = ParseExpression(objectName);
        return new StringToLowerCaseExpression(targetExpr);
      }

      // Handle .startsWith() method for strings
      if (methodPart.StartsWith("startsWith(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(11, methodPart.Length - 12).Trim();
        var prefixExpr = ParseExpression(argsContent);
        var targetExpr = ParseExpression(objectName);
        return new StringStartsWithExpression(targetExpr, prefixExpr);
      }

      // Handle .endsWith() method for strings
      if (methodPart.StartsWith("endsWith(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(9, methodPart.Length - 10).Trim();
        var suffixExpr = ParseExpression(argsContent);
        var targetExpr = ParseExpression(objectName);
        return new StringEndsWithExpression(targetExpr, suffixExpr);
      }

      // Handle .substring() method for strings
      if (methodPart.StartsWith("substring(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(10, methodPart.Length - 11).Trim();
        var args = SplitByComma(argsContent);
        var targetExpr = ParseExpression(objectName);

        if (args.Count == 1)
        {
          var startIndexExpr = ParseExpression(args[0]);
          return new StringSubstringExpression(targetExpr, startIndexExpr);
        }
        else if (args.Count == 2)
        {
          var startIndexExpr = ParseExpression(args[0]);
          var lengthExpr = ParseExpression(args[1]);
          return new StringSubstringExpression(targetExpr, startIndexExpr, lengthExpr);
        }
        else
        {
          throw new InvalidOperationException("substring() requires 1 or 2 arguments (startIndex, length)");
        }
      }

      // Handle .indexOf() method for strings
      if (methodPart.StartsWith("indexOf(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(8, methodPart.Length - 9).Trim();
        var searchValueExpr = ParseExpression(argsContent);
        var targetExpr = ParseExpression(objectName);
        return new StringIndexOfExpression(targetExpr, searchValueExpr);
      }

      // Handle .push() method for arrays
      if (methodPart.StartsWith("push(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(5, methodPart.Length - 6).Trim();
        var itemExpr = ParseExpression(argsContent);
        var targetExpr = new VariableExpression(objectName);
        return new ArrayPushExpression(targetExpr, itemExpr);
      }

      // Handle timer methods
      if (objectName == "timer")
      {
        if (methodPart == "stop()")
        {
          return new TimerStopExpression();
        }
        if (methodPart == "current()")
        {
          return new TimerCurrentExpression();
        }
      }
    }

    // Function call: functionName(arg1, arg2, ...)
    if (input.Contains("(") && input.EndsWith(")"))
    {
      var parenIndex = input.IndexOf('(');
      var functionName = input.Substring(0, parenIndex).Trim();
      var argsContent = input.Substring(parenIndex + 1, input.Length - parenIndex - 2).Trim();

      // Special handling for fs.readFile()
      if (functionName == "fs.readFile")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var filePathExpr = ParseExpression(args[0]);
          return new FsReadFileExpression(filePathExpr);
        }
        throw new InvalidOperationException("fs.readFile() requires exactly 1 argument (filePath)");
      }

      // Special handling for fs.writeFile()
      if (functionName == "fs.writeFile")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var filePathExpr = ParseExpression(args[0]);
          var contentExpr = ParseExpression(args[1]);
          // This should be a statement, but we're parsing an expression.
          // We'll create a temporary expression node and handle it in the statement parser.
          return new FsWriteFileExpressionPlaceholder(filePathExpr, contentExpr);
        }
        throw new InvalidOperationException("fs.writeFile() requires exactly 2 arguments (filePath, content)");
      }

      // Special handling for fs.dirname()
      if (functionName == "fs.dirname")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsDirnameExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.dirname() requires exactly 1 argument (path)");
      }

      // Special handling for fs.fileName()
      if (functionName == "fs.fileName")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsFileNameExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.fileName() requires exactly 1 argument (path)");
      }

      // Special handling for fs.extension()
      if (functionName == "fs.extension")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsExtensionExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.extension() requires exactly 1 argument (path)");
      }

      // Special handling for fs.parentDirName()
      if (functionName == "fs.parentDirName")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsParentDirNameExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.parentDirName() requires exactly 1 argument (path)");
      }

      // Special handling for console.isSudo()
      if (functionName == "console.isSudo" && string.IsNullOrEmpty(argsContent))
      {
        return new ConsoleIsSudoExpression();
      }

      // Special handling for console.promptYesNo()
      if (functionName == "console.promptYesNo" && !string.IsNullOrEmpty(argsContent))
      {
        // Extract the prompt text from the argument (should be a string literal)
        var promptTextExpr = ParseExpression(argsContent.Trim());
        return new ConsolePromptYesNoExpression(promptTextExpr);
      }

      // Special handling for args.has()
      if (functionName == "args.has" && !string.IsNullOrEmpty(argsContent))
      {
        var flagExpr = ParseExpression(argsContent.Trim());
        if (flagExpr is LiteralExpression literal)
        {
          return new ArgsHasExpression(literal.Value);
        }
        throw new InvalidOperationException("args.has() requires a string literal argument");
      }

      // Special handling for args.get()
      if (functionName == "args.get" && !string.IsNullOrEmpty(argsContent))
      {
        var flagExpr = ParseExpression(argsContent.Trim());
        if (flagExpr is LiteralExpression literal)
        {
          return new ArgsGetExpression(literal.Value);
        }
        throw new InvalidOperationException("args.get() requires a string literal argument");
      }

      // Special handling for args.all()
      if (functionName == "args.all" && string.IsNullOrEmpty(argsContent))
      {
        return new ArgsAllExpression();
      }

      // Special handling for os.isInstalled()
      if (functionName == "os.isInstalled" && !string.IsNullOrEmpty(argsContent))
      {
        // Extract the app name from the argument (should be a string literal)
        var appNameExpr = ParseExpression(argsContent.Trim());
        return new OsIsInstalledExpression(appNameExpr);
      }

      // Special handling for process.elapsedTime()
      if (functionName == "process.elapsedTime" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessElapsedTimeExpression();
      }

      // Special handling for process.id()
      if (functionName == "process.id" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessIdExpression();
      }

      // Special handling for process.cpu()
      if (functionName == "process.cpu" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessCpuExpression();
      }

      // Special handling for process.memory()
      if (functionName == "process.memory" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessMemoryExpression();
      }

      // Special handling for process.command()
      if (functionName == "process.command" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessCommandExpression();
      }

      // Special handling for process.status()
      if (functionName == "process.status" && string.IsNullOrEmpty(argsContent))
      {
        return new ProcessStatusExpression();
      }

      // Special handling for os.getLinuxVersion()
      if (functionName == "os.getLinuxVersion" && string.IsNullOrEmpty(argsContent))
      {
        return new OsGetLinuxVersionExpression();
      }

      // Special handling for os.getOS()
      if (functionName == "os.getOS" && string.IsNullOrEmpty(argsContent))
      {
        return new OsGetOSExpression();
      }

      // Special handling for utility.random()
      if (functionName == "utility.random")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          // No arguments: utility.random()
          return new UtilityRandomExpression(null, null);
        }
        else
        {
          // Parse arguments
          var args = argsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(arg => arg.Trim())
                                .ToList();
          if (args.Count == 1)
          {
            // One argument: utility.random(max)
            var maxExpr = ParseExpression(args[0]);
            return new UtilityRandomExpression(null, maxExpr);
          }
          else if (args.Count == 2)
          {
            // Two arguments: utility.random(min, max)
            var minExpr = ParseExpression(args[0]);
            var maxExpr = ParseExpression(args[1]);
            return new UtilityRandomExpression(minExpr, maxExpr);
          }
          else
          {
            throw new InvalidOperationException($"utility.random() accepts 0, 1, or 2 arguments, got {args.Count}");
          }
        }
      }

      // Check for parallel keyword
      if (input.TrimStart().StartsWith("parallel "))
      {
        var parallelInput = input.TrimStart().Substring("parallel ".Length).Trim();
        var match = Regex.Match(parallelInput, @"^(\w+)\((.*)\)$");
        if (match.Success)
        {
          var parallelFunctionName = match.Groups[1].Value;
          var parallelArgsContent = match.Groups[2].Value;
          var parallelArguments = new List<Expression>();
          if (!string.IsNullOrEmpty(parallelArgsContent))
          {
            parallelArguments.AddRange(SplitByComma(parallelArgsContent).Select(arg => ParseExpression(arg.Trim())));
          }
          return new ParallelFunctionCall(parallelFunctionName, parallelArguments);
        }
      }
      // Regular function call (not special built-in)
      if (Regex.IsMatch(functionName, @"^[\w\.]+$"))
      {
        var arguments = new List<Expression>();
        if (!string.IsNullOrEmpty(argsContent))
        {
          arguments.AddRange(SplitByComma(argsContent).Select(arg => ParseExpression(arg.Trim())));
        }
        return new FunctionCall(functionName, arguments);
      }
    }

    // Variable reference
    if (Regex.IsMatch(input, @"^\w+$"))
    {
      return new VariableExpression(input);
    }

    // Fallback to literal
    return new LiteralExpression(input, "unknown");
  }

  private List<string> SplitTernary(string input)
  {
    var parts = new List<string>();
    int depth = 0;
    int questionIndex = -1;
    int colonIndex = -1;
    bool inQuotes = false;
    char quoteChar = '\0';

    for (int i = 0; i < input.Length; i++)
    {
      var ch = input[i];

      // Handle quotes
      if (!inQuotes && (ch == '"' || ch == '\'' || ch == '`'))
      {
        inQuotes = true;
        quoteChar = ch;
      }
      else if (inQuotes && ch == quoteChar)
      {
        inQuotes = false;
        quoteChar = '\0';
      }
      else if (!inQuotes)
      {
        if (ch == '(') depth++;
        else if (ch == ')') depth--;
        else if (depth == 0 && ch == '?' && questionIndex == -1)
        {
          questionIndex = i;
        }
        else if (depth == 0 && ch == ':' && questionIndex != -1 && colonIndex == -1)
        {
          colonIndex = i;
          break; // Found the first colon at the same level, this is our split point
        }
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

  private TemplateLiteralExpression ParseTemplateLiteral(string input)
  {
    var parts = new List<object>();
    var content = input.Substring(1, input.Length - 2); // Remove backticks

    var regex = new Regex(@"\$\{(.+?)\}");
    var matches = regex.Matches(content);
    var lastIndex = 0;

    foreach (Match match in matches)
    {
      if (match.Index > lastIndex)
      {
        parts.Add(content.Substring(lastIndex, match.Index - lastIndex));
      }
      parts.Add(ParseExpression(match.Groups[1].Value));
      lastIndex = match.Index + match.Length;
    }

    if (lastIndex < content.Length)
    {
      parts.Add(content.Substring(lastIndex));
    }

    return new TemplateLiteralExpression(parts);
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

    // Sort operators by length descending to check longer ones first
    var sortedOps = operators.OrderByDescending(op => op.Length).ToArray();

    for (int i = 0; i < input.Length; i++)
    {
      char c = input[i];

      if (!inString && (c == '"' || c == '\'' || c == '`'))
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
          foreach (var op in sortedOps)
          {
            if (i + op.Length <= input.Length && input.Substring(i, op.Length) == op)
            {
              // Special case: don't split on + or - if they are part of ++ or --
              if (op == "+" && i + 1 < input.Length && input[i + 1] == '+')
                continue;
              if (op == "-" && i + 1 < input.Length && input[i + 1] == '-')
                continue;
              if (op == "+" && i > 0 && input[i - 1] == '+')
                continue;
              if (op == "-" && i > 0 && input[i - 1] == '-')
                continue;

              parts.Add(input.Substring(start, i - start).Trim());
              start = i + op.Length;
              i += op.Length - 1; // Skip the operator
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

  private bool IsLikelyArray(string variableName)
  {
    // Simple heuristic: if the variable name suggests it's an array
    // This should be replaced with a proper symbol table lookup
    return variableName.EndsWith("s") ||
           variableName.EndsWith("es") ||
           variableName.EndsWith("ies") ||
           variableName.Contains("array", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("list", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("history", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("items", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("entries", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("values", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("data", StringComparison.OrdinalIgnoreCase);
  }
}
