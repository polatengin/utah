using System.Text;

public class FormatterVisitor
{
  private readonly FormattingOptions _options;
  private readonly StringBuilder _output;
  private int _indentLevel = 0;

  public FormatterVisitor(FormattingOptions options)
  {
    _options = options;
    _output = new StringBuilder();
  }

  public string Visit(ProgramNode program)
  {
    _output.Clear();
    _indentLevel = 0;

    for (int i = 0; i < program.Statements.Count; i++)
    {
      var statement = program.Statements[i];
      var prevStatement = i > 0 ? program.Statements[i - 1] : null;

      // Add blank line between different statement types
      if (i > 0 && ShouldAddBlankLineBetween(prevStatement!, statement))
      {
        _output.AppendLine();
      }

      VisitStatement(statement);

      // Add blank line after functions and major blocks (except last statement)
      if (i < program.Statements.Count - 1 && ShouldAddBlankLineAfter(statement))
      {
        _output.AppendLine();
      }
    }

    return _output.ToString();
  }

  private bool ShouldAddBlankLineBetween(Statement prevStatement, Statement currentStatement)
  {
    // Always add blank line before major control structures
    if (currentStatement is FunctionDeclaration || currentStatement is ImportStatement ||
        currentStatement is IfStatement || currentStatement is ForLoop || 
        currentStatement is ForInLoop || currentStatement is WhileStatement ||
        currentStatement is SwitchStatement || currentStatement is TryCatchStatement)
    {
      return true;
    }

    // Add blank line between variable declarations and other statement types
    if (prevStatement is VariableDeclaration && !(currentStatement is VariableDeclaration))
    {
      return true;
    }

    // Add blank line after control structures
    if (prevStatement is IfStatement || prevStatement is ForLoop || 
        prevStatement is ForInLoop || prevStatement is WhileStatement ||
        prevStatement is SwitchStatement || prevStatement is TryCatchStatement)
    {
      return true;
    }

    return false;
  }

  private bool ShouldAddBlankLineBefore(Statement statement)
  {
    return statement is FunctionDeclaration ||
           statement is ImportStatement ||
           (statement is IfStatement) ||
           (statement is ForLoop) ||
           (statement is ForInLoop) ||
           (statement is WhileStatement) ||
           (statement is SwitchStatement) ||
           (statement is TryCatchStatement);
  }

  private bool ShouldAddBlankLineAfter(Statement statement)
  {
    return statement is FunctionDeclaration ||
           statement is ImportStatement;
  }

  private void VisitStatement(Statement statement)
  {
    switch (statement)
    {
      case ImportStatement import:
        VisitImportStatement(import);
        break;
      case VariableDeclaration varDecl:
        VisitVariableDeclaration(varDecl);
        break;
      case FunctionDeclaration funcDecl:
        VisitFunctionDeclaration(funcDecl);
        break;
      case IfStatement ifStmt:
        VisitIfStatement(ifStmt);
        break;
      case ForLoop forLoop:
        VisitForLoop(forLoop);
        break;
      case ForInLoop forInLoop:
        VisitForInLoop(forInLoop);
        break;
      case WhileStatement whileStmt:
        VisitWhileStatement(whileStmt);
        break;
      case SwitchStatement switchStmt:
        VisitSwitchStatement(switchStmt);
        break;
      case TryCatchStatement tryCatch:
        VisitTryCatchStatement(tryCatch);
        break;
      case ConsoleLog consoleLog:
        VisitConsoleLog(consoleLog);
        break;
      case ConsoleClearStatement:
        WriteIndentedLine("console.clear();");
        break;
      case ExitStatement exitStmt:
        VisitExitStatement(exitStmt);
        break;
      case ReturnStatement returnStmt:
        VisitReturnStatement(returnStmt);
        break;
      case BreakStatement:
        WriteIndentedLine("break;");
        break;
      case ContinueStatement:
        WriteIndentedLine("continue;");
        break;
      case ExpressionStatement exprStmt:
        WriteIndentedLine($"{VisitExpression(exprStmt.Expression)};");
        break;
      case ScriptDescriptionStatement scriptDesc:
        WriteIndentedLine($"script.description(\"{scriptDesc.Description}\");");
        break;
      case ScriptEnableDebugStatement:
        WriteIndentedLine("script.enableDebug();");
        break;
      case ScriptDisableDebugStatement:
        WriteIndentedLine("script.disableDebug();");
        break;
      case ScriptEnableGlobbingStatement:
        WriteIndentedLine("script.enableGlobbing();");
        break;
      case ScriptDisableGlobbingStatement:
        WriteIndentedLine("script.disableGlobbing();");
        break;
      case ScriptExitOnErrorStatement:
        WriteIndentedLine("script.exitOnError();");
        break;
      case ScriptContinueOnErrorStatement:
        WriteIndentedLine("script.continueOnError();");
        break;
      case TimerStartStatement:
        WriteIndentedLine("timer.start();");
        break;
      case FsWriteFileStatement fsWrite:
        WriteIndentedLine($"fs.writeFile({VisitExpression(fsWrite.FilePath)}, {VisitExpression(fsWrite.Content)});");
        break;
      case ArgsDefineStatement argsDefine:
        VisitArgsDefineStatement(argsDefine);
        break;
      case ArgsShowHelpStatement:
        WriteIndentedLine("args.showHelp();");
        break;
      case RawStatement raw:
        WriteIndentedLine(raw.Content);
        break;
      default:
        // Handle unknown statement types
        WriteIndentedLine($"// Unknown statement: {statement.GetType().Name}");
        break;
    }
  }

  private void VisitImportStatement(ImportStatement import)
  {
    WriteIndentedLine($"import \"{import.FilePath}\";");
  }

  private void VisitVariableDeclaration(VariableDeclaration varDecl)
  {
    var keyword = varDecl.IsConst ? "const" : "let";
    var type = !string.IsNullOrEmpty(varDecl.Type) ? $": {varDecl.Type}" : "";
    var value = varDecl.Value != null ? $" = {VisitExpression(varDecl.Value)}" : "";

    WriteIndentedLine($"{keyword} {varDecl.Name}{type}{value};");
  }

  private void VisitFunctionDeclaration(FunctionDeclaration funcDecl)
  {
    var parameters = string.Join(", ", funcDecl.Parameters.Select(p => $"{p.Name}: {p.Type}"));
    var returnType = !string.IsNullOrEmpty(funcDecl.ReturnType) ? $": {funcDecl.ReturnType}" : "";
    var openParen = _options.SpaceBeforeParen ? " (" : "(";

    WriteIndentedLine($"function {funcDecl.Name}{openParen}{parameters}){returnType} {{");

    _indentLevel++;
    foreach (var statement in funcDecl.Body)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitIfStatement(IfStatement ifStmt)
  {
    var openParen = _options.SpaceBeforeParen ? " (" : "(";
    WriteIndentedLine($"if{openParen}{VisitExpression(ifStmt.Condition)}) {{");

    _indentLevel++;
    foreach (var statement in ifStmt.ThenBody)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    if (ifStmt.ElseBody.Count > 0)
    {
      WriteIndentedLine("} else {");

      _indentLevel++;
      foreach (var statement in ifStmt.ElseBody)
      {
        VisitStatement(statement);
      }
      _indentLevel--;
    }

    WriteIndentedLine("}");
  }

  private void VisitForLoop(ForLoop forLoop)
  {
    var init = $"let {forLoop.Initializer.Name}: {forLoop.Initializer.Type} = {VisitExpression(forLoop.Initializer.Value)}";
    var condition = VisitExpression(forLoop.Condition);
    var update = VisitExpression(forLoop.Update);
    var openParen = _options.SpaceBeforeParen ? " (" : "(";

    WriteIndentedLine($"for{openParen}{init}; {condition}; {update}) {{");

    _indentLevel++;
    foreach (var statement in forLoop.Body)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitForInLoop(ForInLoop forInLoop)
  {
    var openParen = _options.SpaceBeforeParen ? " (" : "(";
    WriteIndentedLine($"for{openParen}let {forInLoop.VariableName}: string in {VisitExpression(forInLoop.Iterable)}) {{");

    _indentLevel++;
    foreach (var statement in forInLoop.Body)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitWhileStatement(WhileStatement whileStmt)
  {
    var openParen = _options.SpaceBeforeParen ? " (" : "(";
    WriteIndentedLine($"while{openParen}{VisitExpression(whileStmt.Condition)}) {{");

    _indentLevel++;
    foreach (var statement in whileStmt.Body)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitSwitchStatement(SwitchStatement switchStmt)
  {
    var openParen = _options.SpaceBeforeParen ? " (" : "(";
    WriteIndentedLine($"switch{openParen}{VisitExpression(switchStmt.Expression)}) {{");

    _indentLevel++;
    foreach (var caseClause in switchStmt.Cases)
    {
      var values = string.Join(", ", caseClause.Values.Select(VisitExpression));
      WriteIndentedLine($"case {values}:");

      _indentLevel++;
      foreach (var statement in caseClause.Body)
      {
        VisitStatement(statement);
      }
      if (caseClause.HasBreak)
      {
        WriteIndentedLine("break;");
      }
      _indentLevel--;
    }

    if (switchStmt.DefaultCase != null)
    {
      WriteIndentedLine("default:");

      _indentLevel++;
      foreach (var statement in switchStmt.DefaultCase.Body)
      {
        VisitStatement(statement);
      }
      if (switchStmt.DefaultCase.HasBreak)
      {
        WriteIndentedLine("break;");
      }
      _indentLevel--;
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitTryCatchStatement(TryCatchStatement tryCatch)
  {
    WriteIndentedLine("try {");

    _indentLevel++;
    foreach (var statement in tryCatch.TryBody)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("} catch {");

    _indentLevel++;
    foreach (var statement in tryCatch.CatchBody)
    {
      VisitStatement(statement);
    }
    _indentLevel--;

    WriteIndentedLine("}");
  }

  private void VisitConsoleLog(ConsoleLog consoleLog)
  {
    WriteIndentedLine($"console.log({VisitExpression(consoleLog.Message)});");
  }

  private void VisitExitStatement(ExitStatement exitStmt)
  {
    WriteIndentedLine($"exit({VisitExpression(exitStmt.ExitCode)});");
  }

  private void VisitReturnStatement(ReturnStatement returnStmt)
  {
    if (returnStmt.Value != null)
    {
      WriteIndentedLine($"return {VisitExpression(returnStmt.Value)};");
    }
    else
    {
      WriteIndentedLine("return;");
    }
  }

  private string VisitExpression(Expression expr)
  {
    switch (expr)
    {
      case LiteralExpression literal:
        return literal.Type == "string" ? $"\"{literal.Value}\"" : literal.Value;
      case VariableExpression variable:
        return variable.Name;
      case BinaryExpression binary:
        return $"{VisitExpression(binary.Left)} {binary.Operator} {VisitExpression(binary.Right)}";
      case FunctionCall funcCall:
        var args = string.Join(", ", funcCall.Arguments.Select(VisitExpression));
        return $"{funcCall.Name}({args})";
      case ParallelFunctionCall parallelCall:
        var parallelArgs = string.Join(", ", parallelCall.Arguments.Select(VisitExpression));
        return $"parallel {parallelCall.Name}({parallelArgs})";
      case TemplateLiteralExpression template:
        var parts = template.Parts.Select(part => part is Expression partExpr ? "${" + VisitExpression(partExpr) + "}" : part.ToString());
        return $"`{string.Join("", parts)}`";
      case ArrayLiteral array:
        var elements = string.Join(", ", array.Elements.Select(VisitExpression));
        return $"[{elements}]";
      case TernaryExpression ternary:
        return $"{VisitExpression(ternary.Condition)} ? {VisitExpression(ternary.TrueExpression)} : {VisitExpression(ternary.FalseExpression)}";
      case AssignmentExpression assignment:
        return $"{VisitExpression(assignment.Left)} = {VisitExpression(assignment.Right)}";
      case PostIncrementExpression postInc:
        return $"{VisitExpression(postInc.Operand)}++";
      case PostDecrementExpression postDec:
        return $"{VisitExpression(postDec.Operand)}--";
      case PreIncrementExpression preInc:
        return $"++{VisitExpression(preInc.Operand)}";
      case PreDecrementExpression preDec:
        return $"--{VisitExpression(preDec.Operand)}";
      case UnaryExpression unary:
        return $"{unary.Operator}{VisitExpression(unary.Operand)}";
      case ParenthesizedExpression paren:
        return $"({VisitExpression(paren.Inner)})";
      case ArrayAccess arrayAccess:
        return $"{VisitExpression(arrayAccess.Array)}[{VisitExpression(arrayAccess.Index)}]";
      case ArrayLength arrayLength:
        return $"{VisitExpression(arrayLength.Array)}.length";
      case ArrayIsEmpty arrayIsEmpty:
        return $"{VisitExpression(arrayIsEmpty.Array)}.isEmpty()";
      case ArrayContains arrayContains:
        return $"{VisitExpression(arrayContains.Array)}.contains({VisitExpression(arrayContains.Item)})";
      case ArrayReverse arrayReverse:
        return $"{VisitExpression(arrayReverse.Array)}.reverse()";
      case ArrayPushExpression arrayPush:
        return $"{VisitExpression(arrayPush.Array)}.push({VisitExpression(arrayPush.Item)})";
      case StringLengthExpression stringLength:
        return $"{VisitExpression(stringLength.Target)}.length";
      case StringSplitExpression stringSplit:
        return $"{VisitExpression(stringSplit.Target)}.split({VisitExpression(stringSplit.Separator)})";
      case StringToUpperCaseExpression stringUpper:
        return $"{VisitExpression(stringUpper.Target)}.toUpperCase()";
      case StringToLowerCaseExpression stringLower:
        return $"{VisitExpression(stringLower.Target)}.toLowerCase()";
      case StringStartsWithExpression stringStartsWith:
        return $"{VisitExpression(stringStartsWith.Target)}.startsWith({VisitExpression(stringStartsWith.Prefix)})";
      case StringEndsWithExpression stringEndsWith:
        return $"{VisitExpression(stringEndsWith.Target)}.endsWith({VisitExpression(stringEndsWith.Suffix)})";
      case StringIncludesExpression stringIncludes:
        return $"{VisitExpression(stringIncludes.Target)}.includes({VisitExpression(stringIncludes.SearchValue)})";
      case StringSubstringExpression stringSubstring:
        if (stringSubstring.Length != null)
          return $"{VisitExpression(stringSubstring.Target)}.substring({VisitExpression(stringSubstring.StartIndex)}, {VisitExpression(stringSubstring.Length)})";
        else
          return $"{VisitExpression(stringSubstring.Target)}.substring({VisitExpression(stringSubstring.StartIndex)})";
      case StringIndexOfExpression stringIndexOf:
        return $"{VisitExpression(stringIndexOf.Target)}.indexOf({VisitExpression(stringIndexOf.SearchValue)})";
      case StringReplaceExpression stringReplace:
        return $"{VisitExpression(stringReplace.Target)}.replace({VisitExpression(stringReplace.SearchValue)}, {VisitExpression(stringReplace.ReplaceValue)})";
      case UtilityRandomExpression utilityRandom:
        if (utilityRandom.MinValue != null && utilityRandom.MaxValue != null)
          return $"utility.random({VisitExpression(utilityRandom.MinValue)}, {VisitExpression(utilityRandom.MaxValue)})";
        else if (utilityRandom.MaxValue != null)
          return $"utility.random({VisitExpression(utilityRandom.MaxValue)})";
        else
          return "utility.random()";
      case OsIsInstalledExpression osIsInstalled:
        return $"os.isInstalled({VisitExpression(osIsInstalled.AppName)})";
      case OsGetOSExpression:
        return "os.getOS()";
      case OsGetLinuxVersionExpression:
        return "os.getLinuxVersion()";
      case ConsoleIsSudoExpression:
        return "console.isSudo()";
      case ConsolePromptYesNoExpression consolePrompt:
        return $"console.promptYesNo({VisitExpression(consolePrompt.PromptText)})";
      case WebGetExpression webGet:
        return $"web.get({VisitExpression(webGet.Url)})";
      case ProcessIdExpression:
        return "process.id()";
      case ProcessCpuExpression:
        return "process.cpu()";
      case ProcessMemoryExpression:
        return "process.memory()";
      case ProcessElapsedTimeExpression:
        return "process.elapsedTime()";
      case ProcessCommandExpression:
        return "process.command()";
      case ProcessStatusExpression:
        return "process.status()";
      case FsExistsExpression fsExists:
        return $"fs.exists({VisitExpression(fsExists.Path)})";
      case FsReadFileExpression fsReadFile:
        return $"fs.readFile({VisitExpression(fsReadFile.FilePath)})";
      case FsDirnameExpression fsDirname:
        return $"fs.dirname({VisitExpression(fsDirname.Path)})";
      case FsFileNameExpression fsFileName:
        return $"fs.fileName({VisitExpression(fsFileName.Path)})";
      case FsExtensionExpression fsExtension:
        return $"fs.extension({VisitExpression(fsExtension.Path)})";
      case FsParentDirNameExpression fsParentDirName:
        return $"fs.parentDirName({VisitExpression(fsParentDirName.Path)})";
      case TimerStartExpression:
        return "timer.start()";
      case TimerStopExpression:
        return "timer.stop()";
      case ArgsHasExpression argsHas:
        return $"args.has(\"{argsHas.Flag}\")";
      case ArgsGetExpression argsGet:
        return $"args.get(\"{argsGet.Flag}\")";
      case ArgsAllExpression:
        return "args.all()";
      case GitUndoLastCommitExpression:
        return "git.undoLastCommit()";
      case SchedulerCronExpression schedulerCron:
        return $"scheduler.cron({VisitExpression(schedulerCron.CronPattern)}, {VisitExpression(schedulerCron.Job)})";
      case LambdaExpression lambda:
        var parameters = string.Join(", ", lambda.Parameters);
        var bodyText = "";
        if (lambda.Body.Count > 0)
        {
          // Create a temporary visitor to format the lambda body
          var tempVisitor = new FormatterVisitor(_options);
          var tempProgram = new ProgramNode(lambda.Body);
          bodyText = tempVisitor.Visit(tempProgram).Trim();
        }
        return $"({parameters}) => {{ {bodyText} }}";
      default:
        return expr.ToString() ?? "";
    }
  }

  private void VisitArgsDefineStatement(ArgsDefineStatement argsDefine)
  {
    var args = new List<string>
        {
            $"\"{argsDefine.LongFlag}\"",
            $"\"{argsDefine.ShortFlag}\"",
            $"\"{argsDefine.Description}\"",
            $"\"{argsDefine.Type}\"",
            argsDefine.IsRequired.ToString().ToLower()
        };

    if (argsDefine.DefaultValue != null)
    {
      args.Add(VisitExpression(argsDefine.DefaultValue));
    }

    WriteIndentedLine($"args.define({string.Join(", ", args)});");
  }

  private void WriteIndentedLine(string content)
  {
    if (!string.IsNullOrEmpty(content))
    {
      _output.Append(_options.GetIndent(_indentLevel));
    }
    _output.AppendLine(content);
  }
}
