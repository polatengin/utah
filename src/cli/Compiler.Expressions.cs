using System.Text;

public partial class Compiler
{
  private static int _randomCounter = 0;

  private string CompileExpression(Expression expr)
  {
    return expr switch
    {
      LiteralExpression lit => CompileLiteralExpression(lit),
      VariableExpression var => CompileVariableExpression(var),
      BinaryExpression bin => CompileBinaryExpression(bin),
      UnaryExpression un => CompileUnaryExpression(un),
      TernaryExpression tern => CompileTernaryExpression(tern),
      ParenthesizedExpression paren => CompileParenthesizedExpression(paren),
      TemplateLiteralExpression template => CompileTemplateLiteralExpression(template),
      ArrayLiteral arr => CompileArrayLiteral(arr),
      ArrayAccess acc => CompileArrayAccess(acc),
      ArrayLength len => CompileArrayLength(len),
      ArrayIsEmpty isEmpty => CompileArrayIsEmpty(isEmpty),
      ArrayReverse reverse => CompileArrayReverse(reverse),
      FunctionCall func => CompileFunctionCallExpression(func),
      ConsoleIsSudoExpression sudo => CompileConsoleIsSudoExpression(sudo),
      ConsolePromptYesNoExpression prompt => CompileConsolePromptYesNoExpression(prompt),
      OsIsInstalledExpression osInstalled => CompileOsIsInstalledExpression(osInstalled),
      UtilityRandomExpression rand => CompileUtilityRandomExpression(rand),
      WebGetExpression webGet => CompileWebGetExpression(webGet),
      _ => throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported")
    };
  }

  private string CompileConsoleIsSudoExpression(ConsoleIsSudoExpression sudo)
  {
    return "$([ \"$(id -u)\" -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileConsolePromptYesNoExpression(ConsolePromptYesNoExpression prompt)
  {
    // Generate bash code that prompts the user and returns true/false based on yes/no response
    return $"$(while true; do read -p \"{prompt.PromptText} (y/n): \" yn; case $yn in [Yy]* ) echo \"true\"; break;; [Nn]* ) echo \"false\"; break;; * ) echo \"Please answer yes or no.\";; esac; done)";
  }

  private string CompileOsIsInstalledExpression(OsIsInstalledExpression osInstalled)
  {
    // Generate bash code that checks if a command exists and returns true/false
    // AppName could be a variable reference like "app" or a string literal like "docker"
    var appReference = osInstalled.AppName;

    // If it's a variable reference (no quotes), wrap it in ${}
    if (!appReference.StartsWith("\"") && !appReference.StartsWith("'"))
    {
      appReference = $"${{{appReference}}}";
    }

    return $"$(command -v {appReference} &> /dev/null && echo \"true\" || echo \"false\")";
  }

  private string CompileUtilityRandomExpression(UtilityRandomExpression rand)
  {
    // Generate unique variable names to avoid conflicts
    _randomCounter++;
    var minVar = $"_utah_random_min_{_randomCounter}";
    var maxVar = $"_utah_random_max_{_randomCounter}";

    var minValue = "0";
    var maxValue = "32767";

    // Handle parameters
    if (rand.MinValue != null && rand.MaxValue != null)
    {
      // Both min and max provided: utility.random(min, max)
      minValue = CompileExpression(rand.MinValue);
      maxValue = CompileExpression(rand.MaxValue);
    }
    else if (rand.MinValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MinValue);
    }
    // If no parameters, use defaults (0, 32767)

    // Remove quotes from numeric values if present
    minValue = minValue.Trim('"');
    maxValue = maxValue.Trim('"');

    // For variable references, we need to use the actual variable values
    // If the value starts with ${, it's a variable reference
    if (minValue.StartsWith("${") && minValue.EndsWith("}"))
    {
      // Already in correct format ${varName}
    }
    else if (!int.TryParse(minValue, out _))
    {
      // It's a variable name without ${}, add $ prefix
      minValue = $"${minValue}";
    }

    if (maxValue.StartsWith("${") && maxValue.EndsWith("}"))
    {
      // It's a variable, use it directly
    }
    else if (!int.TryParse(maxValue, out _))
    {
      // It's not a valid integer, maybe it's a variable name without ${}
      maxValue = $"${{{maxValue}}}";
    }

    // Return the bash command substitution that generates the random number with validation
    return $"$({minVar}={minValue}; {maxVar}={maxValue}; if [ ${minVar} -gt ${maxVar} ]; then echo \"Error: min value (${minVar}) cannot be greater than max value (${maxVar}) in utility.random()\" >&2; exit 100; fi; echo $((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar})))";
  }

  private List<string> CompileUtilityRandomDeclaration(string variableName, UtilityRandomExpression rand, bool isConst)
  {
    var lines = new List<string>();

    // Generate unique variable names to avoid conflicts
    _randomCounter++;
    var minVar = $"_utah_random_min_{_randomCounter}";
    var maxVar = $"_utah_random_max_{_randomCounter}";

    var minValue = "0";
    var maxValue = "32767";

    // Handle parameters
    if (rand.MinValue != null && rand.MaxValue != null)
    {
      // Both min and max provided: utility.random(min, max)
      minValue = CompileExpression(rand.MinValue);
      maxValue = CompileExpression(rand.MaxValue);
    }
    else if (rand.MinValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MinValue);
    }
    // If no parameters, use defaults (0, 32767)

    // Remove quotes from numeric values if present
    minValue = minValue.Trim('"');
    maxValue = maxValue.Trim('"');

    // For variable references, we need to use the actual variable values
    if (minValue.StartsWith("${") && minValue.EndsWith("}"))
    {
      // Already in correct format ${varName}
    }
    else if (!int.TryParse(minValue, out _))
    {
      // It's a variable name without ${}, add $ prefix
      minValue = $"${minValue}";
    }

    if (maxValue.StartsWith("${") && maxValue.EndsWith("}"))
    {
      // It's a variable, use it directly
    }
    else if (!int.TryParse(maxValue, out _))
    {
      // It's not a valid integer, maybe it's a variable name without ${}
      maxValue = $"${{{maxValue}}}";
    }

    // Generate validation and assignment as separate statements
    lines.Add($"{minVar}={minValue}");
    lines.Add($"{maxVar}={maxValue}");
    lines.Add($"if [ ${minVar} -gt ${maxVar} ]; then");
    lines.Add($"  echo \"Error: min value (${minVar}) cannot be greater than max value (${maxVar}) in utility.random()\" >&2");
    lines.Add($"  exit 100");
    lines.Add($"fi");

    // Generate the assignment
    if (isConst)
    {
      lines.Add($"readonly {variableName}=$((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar}))");
    }
    else
    {
      lines.Add($"{variableName}=$((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar}))");
    }

    return lines;
  }

  private string CompileLiteralExpression(LiteralExpression lit)
  {
    // For string literals, ensure they are quoted
    return lit.Type switch
    {
      "string" => $"\"{lit.Value}\"",
      "number" => lit.Value,
      "boolean" => lit.Value,
      _ => $"\"{lit.Value}\""
    };
  }

  private string CompileVariableExpression(VariableExpression var)
  {
    return $"${{{var.Name}}}";
  }

  private string CompileBinaryExpression(BinaryExpression bin)
  {
    var left = CompileExpression(bin.Left);
    var right = CompileExpression(bin.Right);

    return bin.Operator switch
    {
      "+" when IsStringConcatenation(bin) => CompileStringConcatenation(left, right),
      "+" => $"$(({ExtractVariableName(left)} + {ExtractVariableName(right)}))",
      "-" => $"$(({ExtractVariableName(left)} - {ExtractVariableName(right)}))",
      "*" => $"$(({ExtractVariableName(left)} * {ExtractVariableName(right)}))",
      "/" => $"$(({ExtractVariableName(left)} / {ExtractVariableName(right)}))",
      "%" => $"$(({ExtractVariableName(left)} % {ExtractVariableName(right)}))",
      "==" => IsNumericLiteral(bin.Right) ? $"[ {left} -eq {right} ]" : $"[ {left} = {right} ]",
      "!=" => $"[ {left} != {right} ]",
      "<" => $"[ {left} -lt {right} ]",
      "<=" => $"[ {left} -le {right} ]",
      ">" => $"[ {left} -gt {right} ]",
      ">=" => $"[ {left} -ge {right} ]",
      "&&" => $"{left} && {right}",
      "||" => $"{left} || {right}",
      _ => throw new NotSupportedException($"Binary operator {bin.Operator} is not supported")
    };
  }

  private string CompileUnaryExpression(UnaryExpression un)
  {
    var operand = CompileExpression(un.Operand);
    return un.Operator switch
    {
      "!" => $"! {operand}",
      "-" => $"$((-${operand}))",
      _ => throw new NotSupportedException($"Unary operator {un.Operator} is not supported")
    };
  }

  private string CompileTernaryExpression(TernaryExpression tern)
  {
    // Helper to compile the condition part of a ternary
    string CompileTernaryCondition(Expression expr)
    {
      if (expr is FunctionCall fc && fc.Name == "env.get" && fc.Arguments.Count == 1)
      {
        var varName = fc.Arguments[0].Trim('"');
        return $"[ -n \"${{{varName}}}\" ]";
      }
      if (expr is VariableExpression varExpr)
      {
        return $"[ \"${{{varExpr.Name}}}\" = \"true\" ]";
      }

      // For other expressions, compile them and if they don't start with '[', wrap them
      var compiledExpr = CompileExpression(expr);

      if (compiledExpr.StartsWith("[ ") && compiledExpr.EndsWith(" ]"))
      {
        return compiledExpr;
      }
      else
      {
        // Assume it's a truthy check
        return $"[ -n {compiledExpr} ]";
      }
    }

    // Helper to compile the value part of a ternary
    string CompileTernaryValue(Expression expr)
    {
      if (expr is FunctionCall fc && fc.Name == "env.get" && fc.Arguments.Count == 1)
      {
        var varName = fc.Arguments[0].Trim('"');
        return $"\"${{{varName}}}\"";
      }
      return CompileExpression(expr);
    }

    var condition = CompileTernaryCondition(tern.Condition);
    var trueExpr = CompileTernaryValue(tern.TrueExpression);

    // Handle nested ternary by avoiding nested command substitution
    if (tern.FalseExpression is TernaryExpression nestedTern)
    {
      // Recursively build the chain, removing the outer `$()` from the nested compilation
      var nestedTernaryStr = CompileTernaryExpression(nestedTern);
      var nestedChain = nestedTernaryStr.Substring(2, nestedTernaryStr.Length - 3);
      return $"$({condition} && echo {trueExpr} || {nestedChain})";
    }

    var falseExpr = CompileTernaryValue(tern.FalseExpression);

    return $"$({condition} && echo {trueExpr} || echo {falseExpr})";
  }

  private string CompileParenthesizedExpression(ParenthesizedExpression paren)
  {
    return $"({CompileExpression(paren.Inner)})";
  }

  private string CompileStringConcatenation(string left, string right)
  {
    // Remove quotes from string literals
    var leftPart = left.StartsWith("\"") && left.EndsWith("\"") ? left[1..^1] : left;

    // Handle variable expressions - extract and format properly
    string rightPart;
    if (right.StartsWith("${") && right.EndsWith("}"))
    {
      // Already in ${var} format, use as is
      rightPart = right;
    }
    else if (right.StartsWith("$"))
    {
      // Old $var format, convert to ${var}
      var varName = right.TrimStart('$');
      rightPart = $"${{{varName}}}";
    }
    else if (right.StartsWith("\"") && right.EndsWith("\""))
    {
      rightPart = right[1..^1];
    }
    else
    {
      rightPart = right;
    }

    return $"\"{leftPart}{rightPart}\"";
  }

  private bool IsStringConcatenation(BinaryExpression bin)
  {
    // Simple heuristic: if either operand is a string literal or we're adding strings
    return (bin.Left is LiteralExpression leftLit && leftLit.Type == "string") ||
           (bin.Right is LiteralExpression rightLit && rightLit.Type == "string");
  }

  private string CompileArrayLiteral(ArrayLiteral arr)
  {
    // Bash arrays are declared and initialized like: arr=("item1" "item2" "item3")
    var elements = arr.Elements.Select(CompileExpression).ToList();
    return $"({string.Join(" ", elements)})";
  }

  private string CompileArrayAccess(ArrayAccess acc)
  {
    var arrayName = ExtractVariableName(CompileExpression(acc.Array));
    var index = CompileExpression(acc.Index);

    // In bash, array access is ${arrayName[index]}
    return $"\"${{{arrayName}[{ExtractVariableName(index)}]}}\"";
  }

  private string CompileArrayLength(ArrayLength len)
  {
    var arrayName = ExtractVariableName(CompileExpression(len.Array));

    // In bash, array length is ${#arrayName[@]}
    return $"\"${{#{arrayName}[@]}}\"";
  }

  private string CompileArrayIsEmpty(ArrayIsEmpty isEmpty)
  {
    var arrayName = ExtractVariableName(CompileExpression(isEmpty.Array));

    // In bash, check if array length is zero: [ ${#arrayName[@]} -eq 0 ]
    return $"$([ ${{#{arrayName}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileArrayReverse(ArrayReverse reverse)
  {
    var arrayName = ExtractVariableName(CompileExpression(reverse.Array));

    // In bash, reverse an array by creating a new array with elements in reverse order
    // We use readarray to convert the output into an array
    return $"($(for ((i=${{#{arrayName}[@]}}-1; i>=0; i--)); do echo \"${{{arrayName}[i]}}\"; done))";
  }

  private string CompileFunctionCallExpression(FunctionCall func)
  {
    // Special handling for env.get() function calls
    if (func.Name == "env.get" && func.Arguments.Count == 1)
    {
      var varName = func.Arguments[0].Trim('"');
      return $"\"${{{varName}}}\"";
    }

    // In bash, function calls in expressions use command substitution
    var bashArgs = func.Arguments.Select(a =>
    {
      var trimmed = a.Trim();
      if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
      {
        // String literal - use as-is
        return trimmed;
      }
      else if (double.TryParse(trimmed, out _))
      {
        // Number literal - use as-is
        return trimmed;
      }
      else
      {
        // Variable reference - add $ prefix
        return $"\"${trimmed}\"";
      }
    }).ToList();

    if (bashArgs.Count > 0)
    {
      return $"$({func.Name} {string.Join(" ", bashArgs)})";
    }
    else
    {
      return $"$({func.Name})";
    }
  }

  private string ExtractVariableName(string varExpr)
  {
    // Handle both $var and ${var} formats
    if (varExpr.StartsWith("${") && varExpr.EndsWith("}"))
    {
      return varExpr[2..^1]; // Remove ${ and }
    }
    else if (varExpr.StartsWith("$"))
    {
      return varExpr[1..]; // Remove $
    }
    return varExpr;
  }

  private bool IsNumericLiteral(Expression expr)
  {
    return expr is LiteralExpression lit && lit.Type == "number";
  }

  private string CompileWebGetExpression(WebGetExpression webGet)
  {
    var url = CompileExpression(webGet.Url);

    // Handle different types of URL expressions
    string curlUrl;
    if (url.StartsWith("${") && url.EndsWith("}"))
    {
      // It's a variable reference like ${varName}
      curlUrl = url;
    }
    else if (url.StartsWith("\"") && url.EndsWith("\""))
    {
      // It's a string literal like "http://example.com"
      curlUrl = url; // Keep the quotes for curl
    }
    else
    {
      // It's a variable name without ${}, add $ prefix for bash
      curlUrl = $"${{{url}}}";
    }

    // Return a bash command substitution that uses curl to make the GET request
    return $"$(curl -s {curlUrl} 2>/dev/null || echo \"\")";
  }

  private string CompileTemplateLiteralExpression(TemplateLiteralExpression template)
  {
    var content = template.Template;

    // Process ${...} expressions in the template
    var result = new StringBuilder();
    var i = 0;

    while (i < content.Length)
    {
      if (i < content.Length - 1 && content[i] == '$' && content[i + 1] == '{')
      {
        // Find the closing brace
        var braceCount = 1;
        var j = i + 2;
        while (j < content.Length && braceCount > 0)
        {
          if (content[j] == '{') braceCount++;
          else if (content[j] == '}') braceCount--;
          j++;
        }

        if (braceCount == 0)
        {
          // Extract and compile the expression
          var exprContent = content.Substring(i + 2, j - i - 3);

          // Special handling for .length() method on strings
          if (exprContent.Contains(".length()"))
          {
            var varName = exprContent.Split('.')[0];
            result.Append($"${{#{varName}}}");
          }
          else
          {
            // For other expressions, use variable substitution
            result.Append($"${{{exprContent}}}");
          }

          i = j;
        }
        else
        {
          result.Append(content[i]);
          i++;
        }
      }
      else
      {
        result.Append(content[i]);
        i++;
      }
    }

    return $"\"{result}\"";
  }
}
