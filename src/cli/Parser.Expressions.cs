using System.Text.RegularExpressions;

public partial class Parser
{
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

    // Template literal
    if (input.StartsWith("`") && input.EndsWith("`"))
    {
      return ParseTemplateLiteral(input);
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
            return new WebGetExpression { Url = urlExpr };
          }
          else
          {
            throw new InvalidOperationException($"web.get() accepts exactly 1 argument (URL), got {args.Count}");
          }
        }
      }

      // Handle .length property
      if (methodPart == "length")
      {
        return new ArrayLength
        {
          Array = new VariableExpression { Name = objectName }
        };
      }

      // Handle .isEmpty() method
      if (methodPart == "isEmpty()")
      {
        return new ArrayIsEmpty
        {
          Array = new VariableExpression { Name = objectName }
        };
      }

      // Handle .reverse() method
      if (methodPart == "reverse()")
      {
        return new ArrayReverse
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

      // Special handling for utility.random()
      if (functionName == "utility.random")
      {
        if (string.IsNullOrEmpty(argsContent))
        {
          // No arguments: utility.random()
          return new UtilityRandomExpression();
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
            return new UtilityRandomExpression { MinValue = maxExpr };
          }
          else if (args.Count == 2)
          {
            // Two arguments: utility.random(min, max)
            var minExpr = ParseExpression(args[0]);
            var maxExpr = ParseExpression(args[1]);
            return new UtilityRandomExpression { MinValue = minExpr, MaxValue = maxExpr };
          }
          else
          {
            throw new InvalidOperationException($"utility.random() accepts 0, 1, or 2 arguments, got {args.Count}");
          }
        }
      }

      // Regular function call (not special built-in)
      if (Regex.IsMatch(functionName, @"^[\w\.]+$"))
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
              // Check if this is part of a compound operator (like <= or >=)
              bool isPartOfCompound = false;
              if (op == "<" && i + 1 < input.Length && input[i + 1] == '=')
                isPartOfCompound = true;
              if (op == ">" && i + 1 < input.Length && input[i + 1] == '=')
                isPartOfCompound = true;
              if (op == "=" && i > 0 && (input[i - 1] == '<' || input[i - 1] == '>' || input[i - 1] == '!' || input[i - 1] == '='))
                isPartOfCompound = true;
              if (op == "=" && i + 1 < input.Length && input[i + 1] == '=')
                isPartOfCompound = true;

              if (!isPartOfCompound)
              {
                parts.Add(input.Substring(start, i - start).Trim());
                start = i + op.Length;
                i += op.Length - 1; // Skip the operator
                break;
              }
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

  private Expression ParseTemplateLiteral(string input)
  {
    // Remove the backticks
    var content = input.Substring(1, input.Length - 2);

    // If no expressions, return a simple string literal
    if (!content.Contains("${"))
    {
      return new LiteralExpression
      {
        Value = content,
        Type = "string"
      };
    }

    // Parse template literal with expressions
    return new TemplateLiteralExpression
    {
      Template = content
    };
  }
}
