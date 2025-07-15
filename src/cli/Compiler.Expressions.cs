using System.Text;
using System.Text.RegularExpressions;

public partial class Compiler
{
  private static int _randomCounter = 0;

  private string CompileExpression(Expression expr)
  {
    switch (expr)
    {
      case AssignmentExpression assign:
        // Handle array assignment specially
        if (assign.Left is ArrayAccess arrayAccess)
        {
          var arrayName = arrayAccess.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(arrayAccess.Array));
          var index = CompileExpression(arrayAccess.Index);
          var value = CompileExpression(assign.Right);

          // For array indices, we need to handle the compiled expression properly
          // If it's an array length expression, it will be quoted and we need to remove quotes
          if (index.StartsWith("\"") && index.EndsWith("\""))
          {
            index = index[1..^1]; // Remove quotes
          }

          return $"{arrayName}[{index}]={value}";
        }
        else
        {
          var varName = assign.Left is VariableExpression varExpr2 ? varExpr2.Name : CompileExpression(assign.Left);
          var value = CompileExpression(assign.Right);
          return $"{varName}={value}";
        }
      case LiteralExpression lit:
        return lit.Type == "string" ? $"\"{lit.Value}\"" : lit.Value;
      case VariableExpression var:
        return $"${{{var.Name}}}";
      case BinaryExpression bin:
        return CompileBinaryExpression(bin);
      case UnaryExpression un:
        return CompileUnaryExpression(un);
      case PostIncrementExpression postInc:
        return CompilePostIncrementExpression(postInc);
      case PostDecrementExpression postDec:
        return CompilePostDecrementExpression(postDec);
      case PreIncrementExpression preInc:
        return CompilePreIncrementExpression(preInc);
      case PreDecrementExpression preDec:
        return CompilePreDecrementExpression(preDec);
      case TernaryExpression tern:
        return CompileTernaryExpression(tern);
      case ParenthesizedExpression paren:
        return CompileParenthesizedExpression(paren);
      case TemplateLiteralExpression template:
        return CompileTemplateLiteralExpression(template);
      case ArrayLiteral arr:
        return CompileArrayLiteral(arr);
      case ArrayAccess acc:
        return CompileArrayAccess(acc);
      case ArrayLength len:
        return CompileArrayLength(len);
      case ArrayIsEmpty isEmpty:
        return CompileArrayIsEmpty(isEmpty);
      case ArrayContains contains:
        return CompileArrayContains(contains);
      case ArrayReverse reverse:
        return CompileArrayReverse(reverse);
      case StringLengthExpression sl:
        return CompileStringLengthExpression(sl);
      case StringSplitExpression ss:
        return CompileStringSplitExpression(ss);
      case FunctionCall func:
        return CompileFunctionCallExpression(func);
      case ConsoleIsSudoExpression sudo:
        return CompileConsoleIsSudoExpression(sudo);
      case ConsolePromptYesNoExpression prompt:
        return CompileConsolePromptYesNoExpression(prompt);
      case ArgsHasExpression argsHas:
        return CompileArgsHasExpression(argsHas);
      case ArgsGetExpression argsGet:
        return CompileArgsGetExpression(argsGet);
      case ArgsAllExpression argsAll:
        return CompileArgsAllExpression(argsAll);
      case OsIsInstalledExpression osInstalled:
        return CompileOsIsInstalledExpression(osInstalled);
      case ProcessElapsedTimeExpression elapsed:
        return CompileProcessElapsedTimeExpression(elapsed);
      case ProcessIdExpression processId:
        return CompileProcessIdExpression(processId);
      case ProcessCpuExpression processCpu:
        return CompileProcessCpuExpression(processCpu);
      case ProcessMemoryExpression processMemory:
        return CompileProcessMemoryExpression(processMemory);
      case ProcessCommandExpression processCommand:
        return CompileProcessCommandExpression(processCommand);
      case ProcessStatusExpression processStatus:
        return CompileProcessStatusExpression(processStatus);
      case TimerStopExpression:
        return CompileTimerStopExpression();
      case OsGetLinuxVersionExpression osLinuxVersion:
        return CompileOsGetLinuxVersionExpression(osLinuxVersion);
      case OsGetOSExpression osGetOS:
        return CompileOsGetOSExpression(osGetOS);
      case UtilityRandomExpression rand:
        return CompileUtilityRandomExpression(rand);
      case WebGetExpression webGet:
        return CompileWebGetExpression(webGet);
      case ArrayJoinExpression arrayJoin:
        return CompileArrayJoinExpression(arrayJoin);
      case FsDirnameExpression fsDirname:
        return CompileFsDirnameExpression(fsDirname);
      case FsFileNameExpression fsFileName:
        return CompileFsFileNameExpression(fsFileName);
      case FsExtensionExpression fsExtension:
        return CompileFsExtensionExpression(fsExtension);
      case FsParentDirNameExpression fsParentDirName:
        return CompileFsParentDirNameExpression(fsParentDirName);
      case FsReadFileExpression fsReadFile:
        return CompileFsReadFileExpression(fsReadFile);
      case StringToUpperCaseExpression stringUpper:
        return CompileStringToUpperCaseExpression(stringUpper);
      case StringToLowerCaseExpression stringLower:
        return CompileStringToLowerCaseExpression(stringLower);
      case StringStartsWithExpression stringStartsWith:
        return CompileStringStartsWithExpression(stringStartsWith);
      case StringEndsWithExpression stringEndsWith:
        return CompileStringEndsWithExpression(stringEndsWith);
      case StringSubstringExpression stringSubstring:
        return CompileStringSubstringExpression(stringSubstring);
      case StringIndexOfExpression stringIndexOf:
        return CompileStringIndexOfExpression(stringIndexOf);
      case StringReplaceExpression stringReplace:
        return CompileStringReplaceExpression(stringReplace);
      case StringIncludesExpression stringIncludes:
        return CompileStringIncludesExpression(stringIncludes);
      case ArrayPushExpression arrayPush:
        return CompileArrayPushExpression(arrayPush);
      case TimerCurrentExpression timerCurrent:
        return CompileTimerCurrentExpression(timerCurrent);
      case GitUndoLastCommitExpression:
        return CompileGitUndoLastCommitExpression();
      case FsWriteFileExpressionPlaceholder fsWriteFile:
        throw new InvalidOperationException("FsWriteFileExpressionPlaceholder should have been converted to a statement.");
      default:
        throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported");
    }
  }

  private string CompileGitUndoLastCommitExpression()
  {
    // Generates bash code for git.undoLastCommit()
    return "$(git reset --soft HEAD~1)";
  }

  private string CompileConsoleIsSudoExpression(ConsoleIsSudoExpression sudo)
  {
    return "$([ \"$(id -u)\" -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileConsolePromptYesNoExpression(ConsolePromptYesNoExpression prompt)
  {
    // Compile the prompt text expression and extract the string value
    var promptText = CompileExpression(prompt.PromptText);
    // Remove quotes if it's a string literal
    if (promptText.StartsWith("\"") && promptText.EndsWith("\""))
    {
      promptText = promptText[1..^1];
    }
    // Generate bash code that prompts the user and returns true/false based on yes/no response
    return $"$(while true; do read -p \"{promptText} (y/n): \" yn; case $yn in [Yy]* ) echo \"true\"; break;; [Nn]* ) echo \"false\"; break;; * ) echo \"Please answer yes or no.\";; esac; done)";
  }

  private string CompileOsIsInstalledExpression(OsIsInstalledExpression osInstalled)
  {
    // Generate bash code that checks if a command exists and returns true/false
    string appReference;

    if (osInstalled.AppName is VariableExpression varExpr)
    {
      appReference = $"${{{varExpr.Name}}}";
    }
    else if (osInstalled.AppName is LiteralExpression literal && literal.Type == "string")
    {
      appReference = $"\"{literal.Value}\"";
    }
    else
    {
      appReference = CompileExpression(osInstalled.AppName);
    }

    return $"$(command -v {appReference} &> /dev/null && echo \"true\" || echo \"false\")";
  }

  private string CompileProcessElapsedTimeExpression(ProcessElapsedTimeExpression elapsed)
  {
    // Generate bash code that gets the elapsed time since the process started
    // Using ps command to get the elapsed time of the current process
    return "$(ps -o etime -p $$ --no-headers | tr -d ' ')";
  }

  private string CompileProcessIdExpression(ProcessIdExpression processId)
  {
    return "$(ps -o pid -p $$ --no-headers | tr -d ' ')";
  }

  private string CompileProcessCpuExpression(ProcessCpuExpression processCpu)
  {
    return "$(ps -o pcpu -p $$ --no-headers | tr -d ' ' | awk '{printf(\"%d\", $1 + 0.5)}')";
  }

  private string CompileProcessMemoryExpression(ProcessMemoryExpression processMemory)
  {
    return "$(ps -o pmem -p $$ --no-headers | tr -d ' ' | awk '{printf(\"%d\", $1 + 0.5)}')";
  }

  private string CompileProcessCommandExpression(ProcessCommandExpression processCommand)
  {
    return "$(ps -o cmd= -p $$)";
  }

  private string CompileProcessStatusExpression(ProcessStatusExpression processStatus)
  {
    return "$(ps -o stat= -p $$)";
  }

  private string CompileOsGetLinuxVersionExpression(OsGetLinuxVersionExpression osLinuxVersion)
  {
    // Generate the complex bash script for getting Linux version
    return "$(" +
           "if [[ -f /etc/os-release ]]; then " +
           "source /etc/os-release; " +
           "echo \"${VERSION_ID}\"; " +
           "elif type lsb_release >/dev/null 2>&1; then " +
           "lsb_release -sr; " +
           "elif [[ -f /etc/lsb-release ]]; then " +
           "source /etc/lsb-release; " +
           "echo \"${DISTRIB_RELEASE}\"; " +
           "else " +
           "echo \"unknown\"; " +
           "fi)";
  }

  private string CompileOsGetOSExpression(OsGetOSExpression osGetOS)
  {
    // Generate the complex bash script for getting OS type
    return "$(" +
           "_uname_os_get_os=$(uname | tr '[:upper:]' '[:lower:]'); " +
           "case $_uname_os_get_os in " +
           "linux*) echo \"linux\" ;; " +
           "darwin*) echo \"mac\" ;; " +
           "msys*|cygwin*|mingw*|nt|win*) echo \"windows\" ;; " +
           "*) echo \"unknown\" ;; " +
           "esac)";
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
    else if (rand.MaxValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MaxValue);
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
    else if (minValue.StartsWith("$((") && minValue.EndsWith("))"))
    {
      // It's an arithmetic expression like $((expr)), use it directly
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
    else if (maxValue.StartsWith("$((") && maxValue.EndsWith("))"))
    {
      // It's an arithmetic expression like $((expr)), use it directly
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
    else if (rand.MaxValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MaxValue);
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
      "+" => $"$(({GetArithmeticValue(left)} + {GetArithmeticValue(right)}))",
      "-" => $"$(({GetArithmeticValue(left)} - {GetArithmeticValue(right)}))",
      "*" => $"$(({GetArithmeticValue(left)} * {GetArithmeticValue(right)}))",
      "/" => $"$(({GetArithmeticValue(left)} / {GetArithmeticValue(right)}))",
      "%" => $"$(({GetArithmeticValue(left)} % {GetArithmeticValue(right)}))",
      "==" => IsNumericLiteral(bin.Right) ? $"[ {left} -eq {right} ]" : $"[ {EnsureQuoted(left)} = {EnsureQuoted(right)} ]",
      "===" => IsNumericLiteral(bin.Right) ? $"[ {left} -eq {right} ]" : $"[ {EnsureQuoted(left)} = {EnsureQuoted(right)} ]",
      "!=" => $"[ {EnsureQuoted(left)} != {EnsureQuoted(right)} ]",
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

  private string CompilePostIncrementExpression(PostIncrementExpression postInc)
  {
    if (postInc.Operand is VariableExpression varExpr)
    {
      // For post-increment, we need to return the old value and increment the variable
      // In bash, this is complex, so we'll just return the variable and rely on
      // the statement context to handle the increment
      return $"${{{varExpr.Name}}}";
    }
    throw new NotSupportedException("Post-increment can only be applied to variables");
  }

  private string CompilePostDecrementExpression(PostDecrementExpression postDec)
  {
    if (postDec.Operand is VariableExpression varExpr)
    {
      return $"${{{varExpr.Name}}}";
    }
    throw new NotSupportedException("Post-decrement can only be applied to variables");
  }

  private string CompilePreIncrementExpression(PreIncrementExpression preInc)
  {
    if (preInc.Operand is VariableExpression varExpr)
    {
      // For pre-increment, we increment first then return the new value
      return $"$(({varExpr.Name} + 1))";
    }
    throw new NotSupportedException("Pre-increment can only be applied to variables");
  }

  private string CompilePreDecrementExpression(PreDecrementExpression preDec)
  {
    if (preDec.Operand is VariableExpression varExpr)
    {
      return $"$(({varExpr.Name} - 1))";
    }
    throw new NotSupportedException("Pre-decrement can only be applied to variables");
  }

  private string CompileTernaryExpression(TernaryExpression tern)
  {
    // Helper to compile the condition part of a ternary
    string CompileTernaryCondition(Expression expr)
    {
      if (expr is FunctionCall fc && fc.Name == "env.get" && fc.Arguments.Count == 1)
      {
        var varName = CompileExpression(fc.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
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
        var varName = CompileExpression(fc.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
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
      if (nestedTernaryStr.StartsWith("$(") && nestedTernaryStr.EndsWith(")"))
      {
        var nestedChain = nestedTernaryStr.Substring(2, nestedTernaryStr.Length - 3);
        return $"$({condition} && echo {trueExpr} || {nestedChain})";
      }
      else
      {
        return $"$({condition} && echo {trueExpr} || {nestedTernaryStr})";
      }
    }

    var falseExpr = CompileTernaryValue(tern.FalseExpression);

    return $"$({condition} && echo {trueExpr} || echo {falseExpr})";
  }

  private string CompileParenthesizedExpression(ParenthesizedExpression paren)
  {
    var inner = CompileExpression(paren.Inner);

    // If the inner expression is already an arithmetic expression (starts with $((, don't add extra parentheses
    if (inner.StartsWith("$((") && inner.EndsWith("))"))
    {
      return inner;
    }

    return $"({inner})";
  }

  private string CompileStringConcatenation(string left, string right)
  {
    // Remove quotes from string literals
    var leftPart = left.StartsWith("\"") && left.EndsWith("\"") ? left[1..^1] : left;

    // Handle variable expressions - extract and format properly
    string rightPart;
    if (right.StartsWith("$(") && right.EndsWith(")"))
    {
      // Command substitution, use as is
      rightPart = right;
    }
    else if (right.StartsWith("${") && right.EndsWith("}"))
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
    // Get the array name directly if it's a variable expression
    var arrayName = acc.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(acc.Array));
    var index = CompileExpression(acc.Index);

    // In bash, array access is ${arrayName[index]}
    return $"${{{arrayName}[{ExtractVariableName(index)}]}}";
  }

  private string CompileArrayLength(ArrayLength len)
  {
    var arrayName = len.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(len.Array));

    // In bash, array length is ${#arrayName[@]}
    // Don't quote it as it's often used in arithmetic contexts
    return $"${{#{arrayName}[@]}}";
  }

  private string CompileArrayIsEmpty(ArrayIsEmpty isEmpty)
  {
    var arrayName = isEmpty.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(isEmpty.Array));

    // In bash, check if array length is zero: [ ${#arrayName[@]} -eq 0 ]
    return $"$([ ${{#{arrayName}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileArrayContains(ArrayContains contains)
  {
    var arrayName = contains.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(contains.Array));
    var item = CompileExpression(contains.Item);

    // Remove quotes from item if it's a string literal for comparison
    var itemValue = item.StartsWith("\"") && item.EndsWith("\"") ? item[1..^1] : item;

    // Use a simpler approach with case statement for better compatibility
    return $"$(case \" ${{{arrayName}[@]}} \" in *\" {itemValue} \"*) echo \"true\" ;; *) echo \"false\" ;; esac)";
  }

  private string CompileArrayReverse(ArrayReverse reverse)
  {
    var arrayName = reverse.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(reverse.Array));

    // In bash, reverse an array by creating a new array with elements in reverse order
    // We use readarray to convert the output into an array
    return $"($(for ((i=${{#{arrayName}[@]}}-1; i>=0; i--)); do echo \"${{{arrayName}[i]}}\"; done))";
  }

  private string CompileStringLengthExpression(StringLengthExpression sl)
  {
    var varName = sl.Target is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(sl.Target));
    return $"${{#{varName}}}";
  }

  private string CompileFunctionCallExpression(FunctionCall func)
  {
    // Special handling for env.get() function calls
    if (func.Name == "env.get")
    {
      if (func.Arguments.Count == 1)
      {
        // env.get("VAR") -> "${VAR}"
        var varName = CompileExpression(func.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        return $"\"${{{varName}}}\"";
      }
      else if (func.Arguments.Count == 2)
      {
        // env.get("VAR", "default") -> "${VAR:-default}"
        var varName = CompileExpression(func.Arguments[0]);
        var defaultValue = CompileExpression(func.Arguments[1]);
        // Remove quotes from varName if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        // Remove quotes from defaultValue if it's a string literal
        if (defaultValue.StartsWith("\"") && defaultValue.EndsWith("\""))
        {
          defaultValue = defaultValue[1..^1];
        }
        return $"\"${{{varName}:-{defaultValue}}}\"";
      }
    }

    // In bash, function calls in expressions use command substitution
    var bashArgs = func.Arguments.Select(arg =>
    {
      var compiled = CompileExpression(arg);
      // Quote variable references for proper word splitting
      if (arg is VariableExpression && compiled.StartsWith("${") && compiled.EndsWith("}"))
      {
        return $"\"{compiled}\"";
      }
      return compiled;
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
    // Handle quoted strings - remove quotes
    if (varExpr.StartsWith("\"") && varExpr.EndsWith("\""))
    {
      varExpr = varExpr[1..^1]; // Remove quotes
    }

    // Handle array length expressions - keep them as-is for arithmetic
    if (varExpr.StartsWith("${#") && varExpr.EndsWith("[@]}"))
    {
      // This is an array length expression like ${#arrayName[@]}
      // In arithmetic contexts, return as-is 
      return varExpr;
    }

    // Handle both $var and ${var} formats
    if (varExpr.StartsWith("${") && varExpr.EndsWith("}"))
    {
      return varExpr[2..^1]; // Remove ${ and }
    }
    else if (varExpr.StartsWith("$"))
    {
      return varExpr[1..]; // Remove $
    }

    // Handle complex expressions that are already unquoted
    if (varExpr.Contains("#") && varExpr.Contains("[@]"))
    {
      // This is an array length expression like #{arrayName[@]}
      return varExpr;
    }

    return varExpr;
  }

  private string GetArithmeticValue(string expression)
  {
    // Handle array length expressions - they should remain as-is in arithmetic contexts
    if (expression.StartsWith("${#") && expression.EndsWith("[@]}"))
    {
      return expression;
    }

    // Handle numeric literals - they should remain as-is
    if (int.TryParse(expression, out _) || double.TryParse(expression, out _))
    {
      return expression;
    }

    // Handle quoted numeric literals
    if (expression.StartsWith("\"") && expression.EndsWith("\""))
    {
      var innerValue = expression[1..^1];
      if (int.TryParse(innerValue, out _) || double.TryParse(innerValue, out _))
      {
        return innerValue;
      }
    }

    // For everything else, extract the variable name
    return ExtractVariableName(expression);
  }

  private bool IsNumericLiteral(Expression expr)
  {
    if (expr is LiteralExpression literal)
    {
      return literal.Type == "number" || int.TryParse(literal.Value, out _) || double.TryParse(literal.Value, out _);
    }
    return false;
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

  private string CompileArrayJoinExpression(ArrayJoinExpression arrayJoin)
  {
    var arrayName = ExtractVariableName(CompileExpression(arrayJoin.Array));
    var separator = CompileExpression(arrayJoin.Separator);

    // In bash, joining an array requires a custom function or a loop
    // Here's a simple implementation using a loop and printf
    return $"$(IFS={separator}; echo \"${{{arrayName}[*]}}\")";
  }

  private string CompileFsDirnameExpression(FsDirnameExpression fsDirname)
  {
    var path = CompileExpression(fsDirname.Path);
    return $"$(dirname {path})";
  }

  private string CompileFsFileNameExpression(FsFileNameExpression fsFileName)
  {
    var path = CompileExpression(fsFileName.Path);
    return $"$(basename {path})";
  }

  private string CompileFsExtensionExpression(FsExtensionExpression fsExtension)
  {
    // Handle string literals specially - extract extension directly
    if (fsExtension.Path is LiteralExpression literal && literal.Type == "string")
    {
      // For string literals, we can extract the extension directly
      var fileName = literal.Value;
      var dotIndex = fileName.LastIndexOf('.');
      if (dotIndex >= 0 && dotIndex < fileName.Length - 1)
      {
        var extension = fileName.Substring(dotIndex + 1);
        return $"\"{extension}\"";
      }
      else
      {
        return "\"\""; // No extension
      }
    }

    // For variables, use bash parameter expansion
    var path = CompileExpression(fsExtension.Path);
    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(path);
    return $"\"${{{varName}##*.}}\"";
  }

  private string CompileFsReadFileExpression(FsReadFileExpression fsReadFile)
  {
    var filePath = CompileExpression(fsReadFile.FilePath);
    return $"$(cat {filePath})";
  }

  private string CompileFsParentDirNameExpression(FsParentDirNameExpression fsParentDirName)
  {
    var path = CompileExpression(fsParentDirName.Path);
    return $"$(basename $(dirname {path}))";
  }

  private string CompileStringToUpperCaseExpression(StringToUpperCaseExpression stringUpper)
  {
    // Extract variable name if it's in ${var} format
    var target = CompileExpression(stringUpper.Target);
    var varName = ExtractVariableName(target);
    return $"\"${{{varName}^^}}\"";
  }

  private string CompileStringToLowerCaseExpression(StringToLowerCaseExpression stringLower)
  {
    // Extract variable name if it's in ${var} format
    var target = CompileExpression(stringLower.Target);
    var varName = ExtractVariableName(target);
    return $"\"${{{varName},,}}\"";
  }

  private string CompileStringStartsWithExpression(StringStartsWithExpression stringStartsWith)
  {
    var target = CompileExpression(stringStartsWith.Target);
    var prefix = CompileExpression(stringStartsWith.Prefix);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from prefix if it's a literal string
    var prefixValue = prefix.StartsWith("\"") && prefix.EndsWith("\"")
      ? prefix[1..^1]
      : prefix;

    return $"[[ \"${{{varName}}}\" == {prefixValue}* ]]";
  }

  private string CompileStringEndsWithExpression(StringEndsWithExpression stringEndsWith)
  {
    var target = CompileExpression(stringEndsWith.Target);
    var suffix = CompileExpression(stringEndsWith.Suffix);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from suffix if it's a literal string
    var suffixValue = suffix.StartsWith("\"") && suffix.EndsWith("\"")
      ? suffix[1..^1]
      : suffix;

    return $"[[ \"${{{varName}}}\" == *{suffixValue} ]]";
  }

  private string CompileStringSubstringExpression(StringSubstringExpression stringSubstring)
  {
    var target = CompileExpression(stringSubstring.Target);
    var start = CompileExpression(stringSubstring.StartIndex);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from start index if it's a literal
    var startValue = start.StartsWith("\"") && start.EndsWith("\"") ? start[1..^1] : start;

    if (stringSubstring.Length != null)
    {
      var length = CompileExpression(stringSubstring.Length);
      var lengthValue = length.StartsWith("\"") && length.EndsWith("\"") ? length[1..^1] : length;
      return $"\"${{{varName}:{startValue}:{lengthValue}}}\"";
    }
    else
    {
      // If no length specified, get substring from start to end
      return $"\"${{{varName}:{startValue}}}\"";
    }
  }

  private string CompileStringIndexOfExpression(StringIndexOfExpression stringIndexOf)
  {
    var target = CompileExpression(stringIndexOf.Target);
    var searchValue = CompileExpression(stringIndexOf.SearchValue);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from search value if it's a literal
    var searchStr = searchValue.StartsWith("\"") && searchValue.EndsWith("\"") ? searchValue[1..^1] : searchValue;

    // Simple approach using grep to find position (returns -1 if not found, 0-based index if found)
    return $"$(echo \"${{{varName}}}\" | awk -v search=\"{searchStr}\" '{{ pos = index($0, search); print (pos > 0 ? pos - 1 : -1) }}')";
  }

  private string CompileArrayPushExpression(ArrayPushExpression arrayPush)
  {
    var array = arrayPush.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(arrayPush.Array));
    var value = CompileExpression(arrayPush.Item);

    // Remove quotes from value if it's a string literal
    var valueStr = value.StartsWith("\"") && value.EndsWith("\"") ? value[1..^1] : value;

    // In bash, to push to an array: array+=(value)
    // Only return the length if this is used in an expression context that needs a value
    // For most cases (statement context), we just want the push operation without echoing
    return $"{array}+=({valueStr})";
  }

  private string CompileTimerCurrentExpression(TimerCurrentExpression timerCurrent)
  {
    // Current timer value is the time elapsed since timer start
    return "$(( $(date +%s%3N) - _utah_timer_start ))";
  }

  private string CompileTemplateLiteralExpression(TemplateLiteralExpression template)
  {
    var parts = new List<string>();
    foreach (var part in template.Parts)
    {
      if (part is string str)
      {
        parts.Add(str);
      }
      else if (part is Expression expr)
      {
        var compiled = CompileExpression(expr);
        // Remove quotes if it's a string literal
        if (compiled.StartsWith("\"") && compiled.EndsWith("\""))
        {
          compiled = compiled[1..^1];
        }
        parts.Add($"${{{ExtractVariableName(compiled)}}}");
      }
    }
    return $"\"{string.Join("", parts)}\"";
  }

  private string CompileStringSplitExpression(StringSplitExpression split)
  {
    var target = CompileExpression(split.Target);
    var separator = CompileExpression(split.Separator);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from separator if it's a literal
    var sepValue = separator.StartsWith("\"") && separator.EndsWith("\"") ? separator[1..^1] : separator;

    // In bash, we can use IFS to split a string into an array
    return $"(IFS='{sepValue}' read -ra SPLIT_ARRAY <<< \"${{{varName}}}\"; echo \"${{SPLIT_ARRAY[@]}}\")";
  }

  private string CompileTimerStopExpression()
  {
    // timer.stop() generates the end timer, calculates elapsed time, and returns it
    return "(_utah_timer_end=$(date +%s%3N); echo $((_utah_timer_end - _utah_timer_start)))";
  }

  private string CompileArgsHasExpression(ArgsHasExpression argsHas)
  {
    return $"$(__utah_has_arg \"{argsHas.Flag}\" \"$@\" && echo \"true\" || echo \"false\")";
  }

  private string CompileArgsGetExpression(ArgsGetExpression argsGet)
  {
    return "$(__utah_get_arg \"" + argsGet.Flag + "\" \"$@\")";
  }

  private string CompileArgsAllExpression(ArgsAllExpression argsAll)
  {
    return "$(__utah_all_args \"$@\")";
  }

  private string CompileStringReplaceExpression(StringReplaceExpression stringReplace)
  {
    var target = CompileExpression(stringReplace.Target);
    var searchValue = CompileExpression(stringReplace.SearchValue);
    var replaceValue = CompileExpression(stringReplace.ReplaceValue);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from search and replace values if they're literals
    var searchStr = searchValue.StartsWith("\"") && searchValue.EndsWith("\"") ? searchValue[1..^1] : searchValue;
    var replaceStr = replaceValue.StartsWith("\"") && replaceValue.EndsWith("\"") ? replaceValue[1..^1] : replaceValue;

    // Use bash parameter expansion for simple string replacement: ${var/search/replace}
    return $"\"${{{varName}/{searchStr}/{replaceStr}}}\"";
  }

  private string CompileStringIncludesExpression(StringIncludesExpression stringIncludes)
  {
    var target = CompileExpression(stringIncludes.Target);
    var searchValue = CompileExpression(stringIncludes.SearchValue);

    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(target);

    // Remove quotes from search value if it's a literal
    var searchStr = searchValue.StartsWith("\"") && searchValue.EndsWith("\"") ? searchValue[1..^1] : searchValue;

    // Use bash case pattern matching to check if string contains substring
    // Returns "true" if found, "false" if not found
    return $"$(case \"${{{varName}}}\" in *{searchStr}*) echo \"true\";; *) echo \"false\";; esac)";
  }

  private string EnsureQuoted(string value)
  {
    // If it's already quoted or is a literal number, don't add quotes
    if (value.StartsWith("\"") && value.EndsWith("\""))
      return value;
    if (IsNumericLiteralValue(value))
      return value;
    
    // For variable expressions like ${var}, wrap in quotes
    return $"\"{value}\"";
  }

  private bool IsNumericLiteralValue(string value)
  {
    return int.TryParse(value, out _) || double.TryParse(value, out _);
  }
}
