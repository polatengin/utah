using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

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
    var operators = new[] { "===", "==", "!=" };
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
      var content = input.Substring(1, input.Length - 2);

      // Check if the string contains ${...} interpolation
      if (content.Contains("${"))
      {
        return ParseStringInterpolation(input);
      }

      return new LiteralExpression(content, "string");
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

      // Special case for web.delete() - handle this as a function call
      if (objectName == "web" && methodPart.StartsWith("delete(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(7, methodPart.Length - 8).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("web.delete() requires a URL argument");
        }
        else
        {
          // Parse the URL argument and optional options
          var args = ParseFunctionArguments(argsContent);
          if (args.Count == 1)
          {
            // One argument: web.delete(url)
            var urlExpr = ParseExpression(args[0]);
            return new WebDeleteExpression(urlExpr);
          }
          else if (args.Count == 2)
          {
            // Two arguments: web.delete(url, options)
            var urlExpr = ParseExpression(args[0]);
            var optionsExpr = ParseExpression(args[1]);
            return new WebDeleteExpression(urlExpr, optionsExpr);
          }
          else
          {
            throw new InvalidOperationException($"web.delete() accepts 1-2 arguments (URL, optional options), got {args.Count}");
          }
        }
      }

      // Special case for web.post() - handle this as a function call
      if (objectName == "web" && methodPart.StartsWith("post(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(5, methodPart.Length - 6).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("web.post() requires URL and data arguments");
        }
        else
        {
          // Parse the URL, data, and optional options arguments
          var args = ParseFunctionArguments(argsContent);
          if (args.Count == 2)
          {
            // Two arguments: web.post(url, data)
            var urlExpr = ParseExpression(args[0]);
            var dataExpr = ParseExpression(args[1]);
            return new WebPostExpression(urlExpr, dataExpr);
          }
          else if (args.Count == 3)
          {
            // Three arguments: web.post(url, data, options)
            var urlExpr = ParseExpression(args[0]);
            var dataExpr = ParseExpression(args[1]);
            var optionsExpr = ParseExpression(args[2]);
            return new WebPostExpression(urlExpr, dataExpr, optionsExpr);
          }
          else
          {
            throw new InvalidOperationException($"web.post() accepts 2-3 arguments (URL, data, optional options), got {args.Count}");
          }
        }
      }

      // Special case for web.put() - handle this as a function call
      if (objectName == "web" && methodPart.StartsWith("put(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(4, methodPart.Length - 5).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("web.put() requires URL and data arguments");
        }
        else
        {
          // Parse the URL, data, and optional options arguments
          var args = ParseFunctionArguments(argsContent);
          if (args.Count == 2)
          {
            // Two arguments: web.put(url, data)
            var urlExpr = ParseExpression(args[0]);
            var dataExpr = ParseExpression(args[1]);
            return new WebPutExpression(urlExpr, dataExpr);
          }
          else if (args.Count == 3)
          {
            // Three arguments: web.put(url, data, options)
            var urlExpr = ParseExpression(args[0]);
            var dataExpr = ParseExpression(args[1]);
            var optionsExpr = ParseExpression(args[2]);
            return new WebPutExpression(urlExpr, dataExpr, optionsExpr);
          }
          else
          {
            throw new InvalidOperationException($"web.put() accepts 2-3 arguments (URL, data, optional options), got {args.Count}");
          }
        }
      }

      // Special case for web.speedtest() - handle this as a function call
      if (objectName == "web" && methodPart.StartsWith("speedtest(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(10, methodPart.Length - 11).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("web.speedtest() requires a URL argument");
        }
        else
        {
          // Parse the URL argument and optional options
          var args = ParseFunctionArguments(argsContent);
          if (args.Count == 1)
          {
            // One argument: web.speedtest(url)
            var urlExpr = ParseExpression(args[0]);
            return new WebSpeedtestExpression(urlExpr);
          }
          else if (args.Count == 2)
          {
            // Two arguments: web.speedtest(url, options)
            var urlExpr = ParseExpression(args[0]);
            var optionsExpr = ParseExpression(args[1]);
            return new WebSpeedtestExpression(urlExpr, optionsExpr);
          }
          else
          {
            throw new InvalidOperationException($"web.speedtest() accepts 1-2 arguments (URL, optional options), got {args.Count}");
          }
        }
      }

      // Special case for template.update() - handle this as a function call
      if (objectName == "template" && methodPart.StartsWith("update(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(7, methodPart.Length - 8).Trim();
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("template.update() requires source and target file path arguments");
        }
        else
        {
          // Parse the file path arguments
          var args = argsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(arg => arg.Trim())
                                .ToList();
          if (args.Count == 2)
          {
            // Two arguments: template.update(sourceFilePath, targetFilePath)
            var sourceFilePathExpr = ParseExpression(args[0]);
            var targetFilePathExpr = ParseExpression(args[1]);
            return new TemplateUpdateExpression(sourceFilePathExpr, targetFilePathExpr);
          }
          else
          {
            throw new InvalidOperationException($"template.update() accepts exactly 2 arguments (sourceFilePath, targetFilePath), got {args.Count}");
          }
        }
      }

      // Special case for string.* namespace functions
      if (objectName == "string" && methodPart.Contains("(") && methodPart.EndsWith(")"))
      {
        var parenIndex = methodPart.IndexOf('(');
        var stringFunctionName = methodPart.Substring(0, parenIndex).Trim();
        var argsContent = methodPart.Substring(parenIndex + 1, methodPart.Length - parenIndex - 2).Trim();

        var arguments = new List<Expression>();
        if (!string.IsNullOrEmpty(argsContent))
        {
          arguments.AddRange(SplitByComma(argsContent).Select(arg => ParseExpression(arg.Trim())));
        }
        return new StringNamespaceCallExpression(stringFunctionName, arguments);
      }

      // Special case for array.* namespace functions (but not for functions with dedicated AST nodes)
      if (objectName == "array" && methodPart.Contains("(") && methodPart.EndsWith(")"))
      {
        var parenIndex = methodPart.IndexOf('(');
        var arrayFunctionName = methodPart.Substring(0, parenIndex).Trim();

        // Skip functions that have dedicated AST node handling
        if (arrayFunctionName != "sort" && arrayFunctionName != "shuffle" && arrayFunctionName != "forEach")
        {
          var argsContent = methodPart.Substring(parenIndex + 1, methodPart.Length - parenIndex - 2).Trim();

          var arguments = new List<Expression>();
          if (!string.IsNullOrEmpty(argsContent))
          {
            arguments.AddRange(SplitByComma(argsContent).Select(arg => ParseExpression(arg.Trim())));
          }
          return new ArrayNamespaceCallExpression(arrayFunctionName, arguments);
        }
      }

      // Handle .length property
      if (methodPart == "length" || methodPart == "length()")
      {
        var targetExpr = new VariableExpression(objectName);
        // Arrays should use array.length() namespace function
        if (IsLikelyArray(objectName))
        {
          return new ArrayLength(targetExpr);
        }
        // Strings should use string.length() namespace function
        throw new InvalidOperationException($"Use string.length({objectName}) instead of {objectName}.length()");
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
        Expression separatorExpr;

        // If no arguments provided, use default comma separator
        if (string.IsNullOrEmpty(argsContent))
        {
          separatorExpr = new LiteralExpression(",", "string");
        }
        else
        {
          separatorExpr = ParseExpression(argsContent);
        }

        return new ArrayJoinExpression(new VariableExpression(objectName), separatorExpr);
      }

      // Handle array.sort() namespace function calls
      if (objectName == "array" && methodPart.StartsWith("sort(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(5, methodPart.Length - 6).Trim();
        var args = SplitByComma(argsContent).Select(arg => ParseExpression(arg.Trim())).ToList();

        Expression arrayExpr = args[0]; // First argument is the array
        Expression? sortOrderExpr = args.Count > 1 ? args[1] : null; // Second argument is optional sort order

        return new ArraySortExpression(arrayExpr, sortOrderExpr);
      }

      // Handle array.shuffle() namespace function calls
      if (objectName == "array" && methodPart == "shuffle()")
      {
        // This shouldn't happen since shuffle requires an argument, but handle empty case
        throw new InvalidOperationException("array.shuffle() requires an array argument");
      }

      if (objectName == "array" && methodPart.StartsWith("shuffle(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(8, methodPart.Length - 9).Trim();
        var arrayExpr = ParseExpression(argsContent);

        return new ArrayShuffleExpression(arrayExpr);
      }

      // Handle .sort() method on variables (e.g., myArray.sort())
      if (methodPart.StartsWith("sort(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(5, methodPart.Length - 6).Trim();
        Expression? sortOrderExpr = null;

        // If no arguments provided, use default ascending sort
        if (!string.IsNullOrEmpty(argsContent))
        {
          sortOrderExpr = ParseExpression(argsContent);
        }

        return new ArraySortExpression(new VariableExpression(objectName), sortOrderExpr);
      }

      // Handle .shuffle() method on variables (e.g., myArray.shuffle())
      if (methodPart == "shuffle()")
      {
        return new ArrayShuffleExpression(new VariableExpression(objectName));
      }

      // Handle array.forEach() namespace function calls
      if (objectName == "array" && methodPart.StartsWith("forEach(") && methodPart.EndsWith(")"))
      {
        var argsContent = methodPart.Substring(8, methodPart.Length - 9).Trim();
        var args = SplitByComma(argsContent);

        if (args.Count != 2)
          throw new InvalidOperationException("array.forEach() requires exactly 2 arguments: array and callback");

        var arrayExpr = ParseExpression(args[0].Trim());
        var lambdaExpr = ParseLambdaExpression(args[1].Trim());

        return new ArrayForEachExpression(arrayExpr, lambdaExpr);
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

      // Special handling for fs.copy()
      if (functionName == "fs.copy")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var sourcePathExpr = ParseExpression(args[0]);
          var targetPathExpr = ParseExpression(args[1]);
          return new FsCopyExpression(sourcePathExpr, targetPathExpr);
        }
        throw new InvalidOperationException("fs.copy() requires exactly 2 arguments (sourcePath, targetPath)");
      }

      // Special handling for fs.move()
      if (functionName == "fs.move")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var sourcePathExpr = ParseExpression(args[0]);
          var targetPathExpr = ParseExpression(args[1]);
          return new FsMoveExpression(sourcePathExpr, targetPathExpr);
        }
        throw new InvalidOperationException("fs.move() requires exactly 2 arguments (sourcePath, targetPath)");
      }

      // Special handling for fs.rename()
      if (functionName == "fs.rename")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var oldNameExpr = ParseExpression(args[0]);
          var newNameExpr = ParseExpression(args[1]);
          return new FsRenameExpression(oldNameExpr, newNameExpr);
        }
        throw new InvalidOperationException("fs.rename() requires exactly 2 arguments (oldName, newName)");
      }

      // Special handling for fs.delete()
      if (functionName == "fs.delete")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsDeleteExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.delete() requires exactly 1 argument (path)");
      }

      // Special handling for fs.chmod()
      if (functionName == "fs.chmod")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var pathExpr = ParseExpression(args[0]);
          var permissionsExpr = ParseExpression(args[1]);
          return new FsChmodExpression(pathExpr, permissionsExpr);
        }
        throw new InvalidOperationException("fs.chmod() requires exactly 2 arguments (path, permissions)");
      }

      // Special handling for fs.chown()
      if (functionName == "fs.chown")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var pathExpr = ParseExpression(args[0]);
          var ownerExpr = ParseExpression(args[1]);
          return new FsChownExpression(pathExpr, ownerExpr);
        }
        else if (args.Count == 3)
        {
          var pathExpr = ParseExpression(args[0]);
          var ownerExpr = ParseExpression(args[1]);
          var groupExpr = ParseExpression(args[2]);
          return new FsChownExpression(pathExpr, ownerExpr, groupExpr);
        }
        throw new InvalidOperationException("fs.chown() requires 2 or 3 arguments (path, owner[, group])");
      }

      // Special handling for fs.find()
      if (functionName == "fs.find")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var searchPathExpr = ParseExpression(args[0]);
          return new FsFindExpression(searchPathExpr, null);
        }
        if (args.Count == 2)
        {
          var searchPathExpr = ParseExpression(args[0]);
          var namePatternExpr = ParseExpression(args[1]);
          return new FsFindExpression(searchPathExpr, namePatternExpr);
        }
        throw new InvalidOperationException("fs.find() requires 1 or 2 arguments (path, name?)");
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

      // Special handling for fs.exists()
      if (functionName == "fs.exists")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var pathExpr = ParseExpression(args[0]);
          return new FsExistsExpression(pathExpr);
        }
        throw new InvalidOperationException("fs.exists() requires exactly 1 argument (path)");
      }

      // Special handling for fs.createTempFolder()
      if (functionName == "fs.createTempFolder")
      {
        var args = string.IsNullOrWhiteSpace(argsContent) ? new List<string>() : SplitByComma(argsContent);
        if (args.Count == 0)
        {
          return new FsCreateTempFolderExpression(null, null);
        }
        if (args.Count == 1)
        {
          return new FsCreateTempFolderExpression(ParseExpression(args[0]), null);
        }
        if (args.Count == 2)
        {
          return new FsCreateTempFolderExpression(ParseExpression(args[0]), ParseExpression(args[1]));
        }
        throw new InvalidOperationException("fs.createTempFolder() requires 0 to 2 arguments (prefix?, baseDir?)");
      }

      // Special handling for console.isSudo()
      if (functionName == "console.isSudo" && string.IsNullOrEmpty(argsContent))
      {
        return new ConsoleIsSudoExpression();
      }

      // Special handling for console.isInteractive()
      if (functionName == "console.isInteractive" && string.IsNullOrEmpty(argsContent))
      {
        return new ConsoleIsInteractiveExpression();
      }

      // Special handling for console.getShell()
      if (functionName == "console.getShell" && string.IsNullOrEmpty(argsContent))
      {
        return new ConsoleGetShellExpression();
      }

      // Special handling for console.promptYesNo()
      if (functionName == "console.promptYesNo" && !string.IsNullOrEmpty(argsContent))
      {
        // Extract the prompt text from the argument (should be a string literal)
        var promptTextExpr = ParseExpression(argsContent.Trim());
        return new ConsolePromptYesNoExpression(promptTextExpr);
      }

      // Dialog functions
      if (functionName == "console.showMessage")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 2) throw new InvalidOperationException("console.showMessage() requires exactly 2 arguments (title, message)");
        return new ConsoleShowMessageExpression(ParseExpression(args[0]), ParseExpression(args[1]));
      }

      if (functionName == "console.showInfo")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 2) throw new InvalidOperationException("console.showInfo() requires exactly 2 arguments (title, message)");
        return new ConsoleShowInfoExpression(ParseExpression(args[0]), ParseExpression(args[1]));
      }

      if (functionName == "console.showWarning")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 2) throw new InvalidOperationException("console.showWarning() requires exactly 2 arguments (title, message)");
        return new ConsoleShowWarningExpression(ParseExpression(args[0]), ParseExpression(args[1]));
      }

      if (functionName == "console.showError")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 2) throw new InvalidOperationException("console.showError() requires exactly 2 arguments (title, message)");
        return new ConsoleShowErrorExpression(ParseExpression(args[0]), ParseExpression(args[1]));
      }

      if (functionName == "console.showSuccess")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 2) throw new InvalidOperationException("console.showSuccess() requires exactly 2 arguments (title, message)");
        return new ConsoleShowSuccessExpression(ParseExpression(args[0]), ParseExpression(args[1]));
      }

      if (functionName == "console.showChoice")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 3 || args.Count > 4) throw new InvalidOperationException("console.showChoice() requires 3 or 4 arguments (title, message, options, [defaultIndex])");
        var title = ParseExpression(args[0]);
        var message = ParseExpression(args[1]);
        var options = ParseExpression(args[2]);
        var defaultIndex = args.Count == 4 ? ParseExpression(args[3]) : null;
        return new ConsoleShowChoiceExpression(title, message, options, defaultIndex);
      }

      if (functionName == "console.showMultiChoice")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 3 || args.Count > 4) throw new InvalidOperationException("console.showMultiChoice() requires 3 or 4 arguments (title, message, options, [defaultSelected])");
        var title = ParseExpression(args[0]);
        var message = ParseExpression(args[1]);
        var options = ParseExpression(args[2]);
        var defaultSelected = args.Count == 4 ? ParseExpression(args[3]) : null;
        return new ConsoleShowMultiChoiceExpression(title, message, options, defaultSelected);
      }

      if (functionName == "console.showConfirm")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 2 || args.Count > 3) throw new InvalidOperationException("console.showConfirm() requires 2 or 3 arguments (title, message, [defaultButton])");
        var title = ParseExpression(args[0]);
        var message = ParseExpression(args[1]);
        var defaultButton = args.Count == 3 ? ParseExpression(args[2]) : null;
        return new ConsoleShowConfirmExpression(title, message, defaultButton);
      }

      if (functionName == "console.showProgress")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 3 || args.Count > 4) throw new InvalidOperationException("console.showProgress() requires 3 or 4 arguments (title, message, percent, [canCancel])");
        var title = ParseExpression(args[0]);
        var message = ParseExpression(args[1]);
        var percent = ParseExpression(args[2]);
        var canCancel = args.Count == 4 ? ParseExpression(args[3]) : null;
        return new ConsoleShowProgressExpression(title, message, percent, canCancel);
      }

      if (functionName == "console.promptText")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 1 || args.Count > 3) throw new InvalidOperationException("console.promptText() requires 1 to 3 arguments (prompt, [defaultValue], [validationPattern])");
        var prompt = ParseExpression(args[0]);
        var defaultValue = args.Count >= 2 ? ParseExpression(args[1]) : null;
        var validation = args.Count == 3 ? ParseExpression(args[2]) : null;
        return new ConsolePromptTextExpression(prompt, defaultValue, validation);
      }

      if (functionName == "console.promptPassword")
      {
        var args = SplitByComma(argsContent);
        if (args.Count != 1) throw new InvalidOperationException("console.promptPassword() requires exactly 1 argument (prompt)");
        return new ConsolePromptPasswordExpression(ParseExpression(args[0]));
      }

      if (functionName == "console.promptNumber")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 1 || args.Count > 4) throw new InvalidOperationException("console.promptNumber() requires 1 to 4 arguments (prompt, [min], [max], [defaultValue])");
        var prompt = ParseExpression(args[0]);
        var minValue = args.Count >= 2 ? ParseExpression(args[1]) : null;
        var maxValue = args.Count >= 3 ? ParseExpression(args[2]) : null;
        var defaultValue = args.Count == 4 ? ParseExpression(args[3]) : null;
        return new ConsolePromptNumberExpression(prompt, minValue, maxValue, defaultValue);
      }

      if (functionName == "console.promptFile")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 1 || args.Count > 2) throw new InvalidOperationException("console.promptFile() requires 1 or 2 arguments (prompt, [filter])");
        var prompt = ParseExpression(args[0]);
        var filter = args.Count == 2 ? ParseExpression(args[1]) : null;
        return new ConsolePromptFileExpression(prompt, filter);
      }

      if (functionName == "console.promptDirectory")
      {
        var args = SplitByComma(argsContent);
        if (args.Count < 1 || args.Count > 2) throw new InvalidOperationException("console.promptDirectory() requires 1 or 2 arguments (prompt, [defaultPath])");
        var prompt = ParseExpression(args[0]);
        var defaultPath = args.Count == 2 ? ParseExpression(args[1]) : null;
        return new ConsolePromptDirectoryExpression(prompt, defaultPath);
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

      // Special handling for utility.uuid()
      if (functionName == "utility.uuid" && string.IsNullOrEmpty(argsContent))
      {
        return new UtilityUuidExpression();
      }

      // Special handling for utility.hash()
      if (functionName == "utility.hash")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("utility.hash() requires at least one argument (text)");
        }

        var args = argsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(arg => arg.Trim())
                              .ToList();

        if (args.Count == 1)
        {
          // One argument: utility.hash(text) - defaults to SHA256
          var textExpr = ParseExpression(args[0]);
          return new UtilityHashExpression(textExpr, null);
        }
        else if (args.Count == 2)
        {
          // Two arguments: utility.hash(text, algorithm)
          var textExpr = ParseExpression(args[0]);
          var algorithmExpr = ParseExpression(args[1]);
          return new UtilityHashExpression(textExpr, algorithmExpr);
        }
        else
        {
          throw new InvalidOperationException($"utility.hash() accepts 1 or 2 arguments, got {args.Count}");
        }
      }

      // Special handling for utility.base64Encode()
      if (functionName == "utility.base64Encode")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("utility.base64Encode() requires one argument (text)");
        }

        var textExpr = ParseExpression(argsContent);
        return new UtilityBase64EncodeExpression(textExpr);
      }

      // Special handling for utility.base64Decode()
      if (functionName == "utility.base64Decode")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          throw new InvalidOperationException("utility.base64Decode() requires one argument (text)");
        }

        var textExpr = ParseExpression(argsContent);
        return new UtilityBase64DecodeExpression(textExpr);
      }

      // Special handling for git.undoLastCommit()
      if (functionName == "git.undoLastCommit" && string.IsNullOrEmpty(argsContent))
      {
        return new GitUndoLastCommitExpression();
      }

      // Special handling for ssh.connect()
      if (functionName == "ssh.connect")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          // ssh.connect("host") - basic connection using SSH config or defaults
          var hostExpr = ParseExpression(args[0]);
          return new SshConnectExpression(hostExpr, null, null, null, null, null);
        }
        if (args.Count == 2)
        {
          // ssh.connect("host", {options}) - connection with options object
          var hostExpr = ParseExpression(args[0]);
          var optionsExpr = ParseExpression(args[1]);

          // For now, we'll parse this as a simple object literal
          // The actual options parsing will be handled in the compiler
          return new SshConnectExpression(hostExpr, optionsExpr, null, null, null, null);
        }
        throw new InvalidOperationException("ssh.connect() requires 1-2 arguments (host, options?)");
      }

      // Special handling for json.parse()
      if (functionName == "json.parse")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var jsonStringExpr = ParseExpression(args[0]);
          return new JsonParseExpression(jsonStringExpr);
        }
        throw new InvalidOperationException("json.parse() requires exactly 1 argument (jsonString)");
      }

      // Special handling for json.stringify()
      if (functionName == "json.stringify")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          return new JsonStringifyExpression(jsonObjectExpr);
        }
        throw new InvalidOperationException("json.stringify() requires exactly 1 argument (jsonObject)");
      }

      // Special handling for json.isValid()
      if (functionName == "json.isValid")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var jsonStringExpr = ParseExpression(args[0]);
          return new JsonIsValidExpression(jsonStringExpr);
        }
        throw new InvalidOperationException("json.isValid() requires exactly 1 argument (jsonString)");
      }

      // Special handling for json.get()
      if (functionName == "json.get")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new JsonGetExpression(jsonObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("json.get() requires exactly 2 arguments (jsonObject, path)");
      }

      // Special handling for json.set()
      if (functionName == "json.set")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 3)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          var valueExpr = ParseExpression(args[2]);
          return new JsonSetExpression(jsonObjectExpr, pathExpr, valueExpr);
        }
        throw new InvalidOperationException("json.set() requires exactly 3 arguments (jsonObject, path, value)");
      }

      // Special handling for json.has()
      if (functionName == "json.has")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new JsonHasExpression(jsonObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("json.has() requires exactly 2 arguments (jsonObject, path)");
      }

      // Special handling for json.delete()
      if (functionName == "json.delete")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new JsonDeleteExpression(jsonObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("json.delete() requires exactly 2 arguments (jsonObject, path)");
      }

      // Special handling for json.keys()
      if (functionName == "json.keys")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          return new JsonKeysExpression(jsonObjectExpr);
        }
        throw new InvalidOperationException("json.keys() requires exactly 1 argument (jsonObject)");
      }

      // Special handling for json.values()
      if (functionName == "json.values")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var jsonObjectExpr = ParseExpression(args[0]);
          return new JsonValuesExpression(jsonObjectExpr);
        }
        throw new InvalidOperationException("json.values() requires exactly 1 argument (jsonObject)");
      }

      // Special handling for json.merge()
      if (functionName == "json.merge")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var jsonObject1Expr = ParseExpression(args[0]);
          var jsonObject2Expr = ParseExpression(args[1]);
          return new JsonMergeExpression(jsonObject1Expr, jsonObject2Expr);
        }
        throw new InvalidOperationException("json.merge() requires exactly 2 arguments (jsonObject1, jsonObject2)");
      }

      // Special handling for json.installDependencies()
      if (functionName == "json.installDependencies")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          return new JsonInstallDependenciesExpression();
        }
        throw new InvalidOperationException("json.installDependencies() requires no arguments");
      }

      // Special handling for yaml.parse()
      if (functionName == "yaml.parse")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var yamlStringExpr = ParseExpression(args[0]);
          return new YamlParseExpression(yamlStringExpr);
        }
        throw new InvalidOperationException("yaml.parse() requires exactly 1 argument (yamlString)");
      }

      // Special handling for yaml.stringify()
      if (functionName == "yaml.stringify")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          return new YamlStringifyExpression(yamlObjectExpr);
        }
        throw new InvalidOperationException("yaml.stringify() requires exactly 1 argument (yamlObject)");
      }

      // Special handling for yaml.isValid()
      if (functionName == "yaml.isValid")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var yamlStringExpr = ParseExpression(args[0]);
          return new YamlIsValidExpression(yamlStringExpr);
        }
        throw new InvalidOperationException("yaml.isValid() requires exactly 1 argument (yamlString)");
      }

      // Special handling for yaml.get()
      if (functionName == "yaml.get")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new YamlGetExpression(yamlObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("yaml.get() requires exactly 2 arguments (yamlObject, path)");
      }

      // Special handling for yaml.set()
      if (functionName == "yaml.set")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 3)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          var valueExpr = ParseExpression(args[2]);
          return new YamlSetExpression(yamlObjectExpr, pathExpr, valueExpr);
        }
        throw new InvalidOperationException("yaml.set() requires exactly 3 arguments (yamlObject, path, value)");
      }

      // Special handling for yaml.has()
      if (functionName == "yaml.has")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new YamlHasExpression(yamlObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("yaml.has() requires exactly 2 arguments (yamlObject, path)");
      }

      // Special handling for yaml.delete()
      if (functionName == "yaml.delete")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          var pathExpr = ParseExpression(args[1]);
          return new YamlDeleteExpression(yamlObjectExpr, pathExpr);
        }
        throw new InvalidOperationException("yaml.delete() requires exactly 2 arguments (yamlObject, path)");
      }

      // Special handling for yaml.keys()
      if (functionName == "yaml.keys")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          return new YamlKeysExpression(yamlObjectExpr);
        }
        throw new InvalidOperationException("yaml.keys() requires exactly 1 argument (yamlObject)");
      }

      // Special handling for yaml.values()
      if (functionName == "yaml.values")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var yamlObjectExpr = ParseExpression(args[0]);
          return new YamlValuesExpression(yamlObjectExpr);
        }
        throw new InvalidOperationException("yaml.values() requires exactly 1 argument (yamlObject)");
      }

      // Special handling for yaml.merge()
      if (functionName == "yaml.merge")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var yamlObject1Expr = ParseExpression(args[0]);
          var yamlObject2Expr = ParseExpression(args[1]);
          return new YamlMergeExpression(yamlObject1Expr, yamlObject2Expr);
        }
        throw new InvalidOperationException("yaml.merge() requires exactly 2 arguments (yamlObject1, yamlObject2)");
      }

      // Special handling for yaml.installDependencies()
      if (functionName == "yaml.installDependencies")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          return new YamlInstallDependenciesExpression();
        }
        throw new InvalidOperationException("yaml.installDependencies() requires no arguments");
      }

      // Special handling for validate.isEmail()
      if (functionName == "validate.isEmail")
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 1)
        {
          var emailExpr = ParseExpression(args[0]);
          return new ValidateIsEmailExpression(emailExpr);
        }
        throw new InvalidOperationException("validate.isEmail() requires exactly 1 argument (email)");
      }

      // Special handling for scheduler.cron()
      if (functionName == "scheduler.cron" && !string.IsNullOrEmpty(argsContent))
      {
        var args = SplitByComma(argsContent);
        if (args.Count == 2)
        {
          var cronPatternExpr = ParseExpression(args[0]);
          var lambdaExpr = ParseLambdaExpression(args[1].Trim());
          return new SchedulerCronExpression(cronPatternExpr, lambdaExpr);
        }
        throw new InvalidOperationException("scheduler.cron() requires exactly 2 arguments: (cronPattern, lambda)");
      }

      // Special handling for string.* namespace functions
      if (functionName.StartsWith("string."))
      {
        var stringFunctionName = functionName.Substring(7); // Remove "string." prefix
        var arguments = new List<Expression>();
        if (!string.IsNullOrEmpty(argsContent))
        {
          arguments.AddRange(SplitByComma(argsContent).Select(arg => ParseExpression(arg.Trim())));
        }
        return new StringNamespaceCallExpression(stringFunctionName, arguments);
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

  private StringInterpolationExpression ParseStringInterpolation(string input)
  {
    var parts = new List<object>();
    var content = input.Substring(1, input.Length - 2); // Remove quotes

    var i = 0;
    var currentString = "";

    while (i < content.Length)
    {
      if (i < content.Length - 1 && content[i] == '$' && content[i + 1] == '{')
      {
        // Add any accumulated string before the expression
        if (currentString.Length > 0)
        {
          parts.Add(currentString);
          currentString = "";
        }

        // Find the matching closing brace, handling nested braces and quotes
        i += 2; // Skip '${'
        var braceDepth = 1;
        var exprStart = i;
        var inQuotes = false;
        var quoteChar = '\0';

        while (i < content.Length && braceDepth > 0)
        {
          var currentChar = content[i];

          if (!inQuotes)
          {
            if (currentChar == '"' || currentChar == '\'')
            {
              inQuotes = true;
              quoteChar = currentChar;
            }
            else if (currentChar == '{')
            {
              braceDepth++;
            }
            else if (currentChar == '}')
            {
              braceDepth--;
            }
          }
          else
          {
            if (currentChar == quoteChar)
            {
              // Check if it's escaped
              int backslashCount = 0;
              for (int j = i - 1; j >= 0 && content[j] == '\\'; j--)
                backslashCount++;

              if (backslashCount % 2 == 0) // Even number of backslashes means not escaped
              {
                inQuotes = false;
              }
            }
          }

          i++;
        }

        if (braceDepth == 0)
        {
          var exprContent = content.Substring(exprStart, i - exprStart - 1);
          parts.Add(ParseExpression(exprContent));
        }
        else
        {
          // Unmatched braces - treat as literal text
          currentString += "${" + content.Substring(exprStart);
          break;
        }
      }
      else
      {
        currentString += content[i];
        i++;
      }
    }

    // Add any remaining string
    if (currentString.Length > 0)
    {
      parts.Add(currentString);
    }

    return new StringInterpolationExpression(parts);
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

  private LambdaExpression ParseLambdaExpression(string input)
  {
    input = input.Trim();

    // Simple lambda: () => { statements }
    var match = Regex.Match(input, @"^\(\s*\)\s*=>\s*\{(.*)\}$", RegexOptions.Singleline);
    if (match.Success)
    {
      var bodyContent = match.Groups[1].Value.Trim();
      var statements = ParseStatementBlock(bodyContent);
      return new LambdaExpression(new List<string>(), statements);
    }

    // Lambda with parameters: (param1, param2) => { statements }
    match = Regex.Match(input, @"^\(([^)]*)\)\s*=>\s*\{(.*)\}$", RegexOptions.Singleline);
    if (match.Success)
    {
      var paramString = match.Groups[1].Value.Trim();
      var bodyContent = match.Groups[2].Value.Trim();

      var parameters = new List<string>();
      if (!string.IsNullOrEmpty(paramString))
      {
        parameters.AddRange(paramString.Split(',').Select(p => p.Trim()));
      }

      var statements = ParseStatementBlock(bodyContent);
      return new LambdaExpression(parameters, statements);
    }

    throw new InvalidOperationException($"Invalid lambda expression: {input}");
  }

  private List<Statement> ParseStatementBlock(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
    {
      return new List<Statement>();
    }

    // Create a temporary parser for the block content
    var parser = new Parser(input);
    var program = parser.Parse();
    return program.Statements;
  }

  private bool IsLikelyArray(string variableName)
  {
    // Simple heuristic: variable names ending with 's' or containing 'Array', 'List', etc.
    // This is a basic implementation - in a full language, we'd use a symbol table
    return variableName.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("Array", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("List", StringComparison.OrdinalIgnoreCase) ||
           variableName.Contains("Items", StringComparison.OrdinalIgnoreCase);
  }

  private List<string> ParseFunctionArguments(string argsContent)
  {
    var args = new List<string>();
    var current = new StringBuilder();
    var inQuotes = false;
    var quoteChar = '\0';
    var depth = 0;

    for (int i = 0; i < argsContent.Length; i++)
    {
      char c = argsContent[i];

      if (!inQuotes)
      {
        if (c == '"' || c == '\'')
        {
          inQuotes = true;
          quoteChar = c;
          current.Append(c);
        }
        else if (c == '(' || c == '[' || c == '{')
        {
          depth++;
          current.Append(c);
        }
        else if (c == ')' || c == ']' || c == '}')
        {
          depth--;
          current.Append(c);
        }
        else if (c == ',' && depth == 0)
        {
          // Found a separator at the top level
          args.Add(current.ToString().Trim());
          current.Clear();
        }
        else
        {
          current.Append(c);
        }
      }
      else
      {
        current.Append(c);
        if (c == quoteChar && (i == 0 || argsContent[i - 1] != '\\'))
        {
          inQuotes = false;
          quoteChar = '\0';
        }
      }
    }

    // Add the last argument
    if (current.Length > 0)
    {
      args.Add(current.ToString().Trim());
    }

    return args;
  }
}
