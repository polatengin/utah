using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;
using System.Text.RegularExpressions;

var app = new UtahApp();
await app.RunAsync(args);

class UtahApp
{
  private const string AllowRemoteFlag = "--allow-remote";

  private static readonly bool s_useColor =
    Environment.GetEnvironmentVariable("NO_COLOR") is null && !Console.IsOutputRedirected;

  private static string C(string text, string color) => s_useColor ? $"{color}{text}\x1b[0m" : text;
  private static string Bold(string text) => C(text, "\x1b[1m");
  private static string Yellow(string text) => C(text, "\x1b[33m");
  private static string Green(string text) => C(text, "\x1b[32m");
  private static string Cyan(string text) => C(text, "\x1b[36m");
  private static string Dim(string text) => C(text, "\x1b[2m");

  private static readonly HttpClient s_httpClient = CreateHttpClient();
  private static readonly HashSet<string> s_formatExcludedDirectories = new(StringComparer.OrdinalIgnoreCase)
  {
    ".git",
    "bin",
    "dist",
    "node_modules",
    "obj"
  };

  private sealed record ScriptExecutionRequest(string ScriptPath, string[] ScriptArgs, bool AllowRemoteExecution);

  public async Task RunAsync(string[] args)
  {
    try
    {
      Environment.ExitCode = await RunCoreAsync(args);
    }
    catch (InvalidOperationException ex)
    {
      Console.WriteLine($"❌ {ex.Message}");
      Environment.ExitCode = 1;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Unexpected error: {ex.Message}");
      Environment.ExitCode = 1;
    }
  }

  private async Task<int> RunCoreAsync(string[] args)
  {
    if (args.Length == 0)
    {
      PrintUsage();
      return 0;
    }

    if (args.Length > 1 && args[0] == AllowRemoteFlag)
    {
      return await ExecuteShxFileAsync(ParseScriptExecutionRequest(args, 0));
    }

    var argumentType = DetectArgumentType(args[0]);

    switch (argumentType)
    {
      case ArgumentType.KnownCommand:
        return await HandleKnownCommandAsync(args);
      case ArgumentType.InlineCommand:
        return await HandleInlineCommandAsync(args);
      case ArgumentType.File:
        return await ExecuteShxFileAsync(ParseScriptExecutionRequest(args, 0));
      case ArgumentType.Unknown:
      default:
        PrintUsage();
        return 0;
    }
  }

  private ArgumentType DetectArgumentType(string firstArg)
  {
    if (IsKnownCommand(firstArg)) return ArgumentType.KnownCommand;

    if (firstArg == "--command" || firstArg == "-c") return ArgumentType.InlineCommand;

    if (IsValidUrl(firstArg) && firstArg.EndsWith(".shx")) return ArgumentType.File;

    if (firstArg.EndsWith(".shx") || File.Exists(firstArg)) return ArgumentType.File;

    return ArgumentType.Unknown;
  }

  private bool IsKnownCommand(string command)
  {
    return command switch
    {
      "lsp" or "compile" or "run" or "format" or "--version" or "-v" or "version" => true,
      _ => false
    };
  }

  private bool IsValidUrl(string input)
  {
    return Uri.TryCreate(input, UriKind.Absolute, out var uri) &&
           (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
  }

  private async Task<string> DownloadFileContentAsync(string url)
  {
    try
    {
      var response = await s_httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"✅ Downloaded {content.Length} characters from {url}");

      return content;
    }
    catch (HttpRequestException ex)
    {
      throw new InvalidOperationException($"HTTP error downloading {url}: {ex.Message}", ex);
    }
    catch (TaskCanceledException ex)
    {
      throw new InvalidOperationException($"Timeout downloading {url}: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Error downloading {url}: {ex.Message}", ex);
    }
  }

  private async Task<int> HandleKnownCommandAsync(string[] args)
  {
    switch (args[0])
    {
      case "lsp":
        await StartLanguageServerAsync();
        return 0;
      case "compile":
        if (args.Length < 2 || (!args[1].EndsWith(".shx") && !IsValidUrl(args[1])))
        {
          PrintCompileUsage();
          return 1;
        }

        return await CompileFileAsync(args[1], GetOutputPath(args));
      case "run":
        if (args.Length < 2)
        {
          PrintRunUsage();
          return 1;
        }

        if (args[1] == "-c" || args[1] == "--command")
        {
          if (args.Length < 3)
          {
            PrintInlineRunUsage();
            return 1;
          }

          var command = string.Join(" ", args.Skip(2));
          return await ExecuteInlineCommandAsync(command);
        }

        var request = ParseScriptExecutionRequest(args, 1);
        if (!request.ScriptPath.EndsWith(".shx") && !IsValidUrl(request.ScriptPath))
        {
          PrintRunUsage();
          return 1;
        }

        return await ExecuteShxFileAsync(request);
      case "format":
        if (args.Length < 2 || args[1].StartsWith("-"))
        {
          return FormatAllFiles(args);
        }

        if (!args[1].EndsWith(".shx"))
        {
          PrintFormatUsage();
          return 1;
        }

        return FormatFile(args);
      case "--version":
      case "-v":
      case "version":
        PrintVersion();
        return 0;
      default:
        PrintUsage();
        return 0;
    }
  }

  private async Task<int> HandleInlineCommandAsync(string[] args)
  {
    if (args.Length < 2)
    {
      PrintDirectCommandUsage();
      return 1;
    }

    var command = string.Join(" ", args.Skip(1));
    return await ExecuteInlineCommandAsync(command);
  }

  private ScriptExecutionRequest ParseScriptExecutionRequest(string[] args, int startIndex)
  {
    var separatorIndex = Array.IndexOf(args, "--", startIndex);
    var optionEndIndex = separatorIndex == -1 ? args.Length : separatorIndex;
    string? scriptPath = null;
    bool allowRemoteExecution = false;

    for (int i = startIndex; i < optionEndIndex; i++)
    {
      var arg = args[i];
      if (arg == AllowRemoteFlag)
      {
        allowRemoteExecution = true;
        continue;
      }

      if (scriptPath == null)
      {
        scriptPath = arg;
        continue;
      }

      throw new InvalidOperationException($"Unexpected argument '{arg}'. Use '--' before script arguments.");
    }

    if (string.IsNullOrWhiteSpace(scriptPath))
    {
      throw new InvalidOperationException("Missing script path. Use 'utah run <file.shx|url>' or 'utah <file.shx>'.");
    }

    var scriptArgs = separatorIndex == -1
      ? Array.Empty<string>()
      : args[(separatorIndex + 1)..];

    return new ScriptExecutionRequest(scriptPath, scriptArgs, allowRemoteExecution);
  }

  private async Task<int> ExecuteShxFileAsync(ScriptExecutionRequest request)
  {
    EnsureRemoteExecutionAllowed(request.ScriptPath, request.AllowRemoteExecution);
    return await RunFileAsync(request.ScriptPath, request.ScriptArgs);
  }

  private async Task<int> ExecuteInlineCommandAsync(string command)
  {
    var preparedCommand = PrepareInlineCommand(command);
    var ast = PromoteInlineExpressionResult(ParseShxContent(preparedCommand));
    return await ExecuteCompiledScriptAsync(CompileShxAst(ast), Array.Empty<string>());
  }

  private string PrepareInlineCommand(string command)
  {
    var normalizedCommand = command.Trim();
    if (string.IsNullOrWhiteSpace(normalizedCommand))
    {
      return string.Empty;
    }

    if (!normalizedCommand.EndsWith(";") && !normalizedCommand.EndsWith("}"))
    {
      normalizedCommand += ";";
    }

    return FormatInlineCommandForFile(normalizedCommand);
  }

  private static ProgramNode PromoteInlineExpressionResult(ProgramNode program)
  {
    if (program.Statements.Count != 1 || program.Statements[0] is not ExpressionStatement exprStmt)
    {
      return program;
    }

    if (!ShouldEchoInlineExpression(exprStmt.Expression))
    {
      return program;
    }

    return new ProgramNode(new List<Statement> { new ConsoleLog(exprStmt.Expression) });
  }

  private static bool ShouldEchoInlineExpression(Expression expression)
  {
    return expression switch
    {
      FunctionCall or ParallelFunctionCall => false,
      ArrayForEachExpression or
      ConsoleShowMessageExpression or
      ConsoleShowInfoExpression or
      ConsoleShowWarningExpression or
      ConsoleShowErrorExpression or
      ConsoleShowSuccessExpression or
      ConsoleShowProgressExpression or
      FsWriteFileExpressionPlaceholder or
      FsCopyExpression or
      FsMoveExpression or
      FsRenameExpression or
      FsDeleteExpression or
      FsChmodExpression or
      FsChownExpression or
      FsWatchExpression or
      GitUndoLastCommitExpression or
      JsonInstallDependenciesExpression or
      ProcessStartExpression or
      ProcessKillExpression or
      ProcessWaitForExitExpression or
      SchedulerCronExpression or
      SshConnectExpression or
      SshExecuteExpression or
      SshUploadExpression or
      SshDownloadExpression or
      TemplateUpdateExpression or
      YamlInstallDependenciesExpression => false,
      _ => true
    };
  }

  private async Task<int> RunFileAsync(string inputPath, string[] scriptArgs)
  {
    var content = await LoadShxContentAsync(inputPath);
    return await CompileAndRunShxContentAsync(content, scriptArgs);
  }

  private async Task<string> LoadShxContentAsync(string inputPath)
  {
    if (IsValidUrl(inputPath))
    {
      Console.WriteLine($"📥 Downloading: {inputPath}");
      return await DownloadFileContentAsync(inputPath);
    }

    if (!File.Exists(inputPath))
    {
      throw new InvalidOperationException($"File not found: {inputPath}");
    }

    return ImportResolver.ResolveImports(inputPath);
  }

  private static ProgramNode ParseShxContent(string shxContent)
  {
    try
    {
      var parser = new Parser(shxContent);
      return parser.Parse();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Compilation failed: {ex.Message}", ex);
    }
  }

  private static string CompileShxAst(ProgramNode ast)
  {
    try
    {
      var compiler = new Compiler();
      return compiler.Compile(ast);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Compilation failed: {ex.Message}", ex);
    }
  }

  private static string CompileShxContent(string shxContent)
  {
    return CompileShxAst(ParseShxContent(shxContent));
  }

  private async Task<int> CompileAndRunShxContentAsync(string shxContent, string[] scriptArgs)
  {
    var output = CompileShxContent(shxContent);
    return await ExecuteCompiledScriptAsync(output, scriptArgs);
  }

  private async Task<int> ExecuteCompiledScriptAsync(string output, string[] scriptArgs)
  {
    var tempFile = Path.GetTempFileName();

    try
    {
      await File.WriteAllTextAsync(tempFile, output);

      using var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };

      process.StartInfo.ArgumentList.Add(tempFile);
      foreach (var scriptArg in scriptArgs)
      {
        process.StartInfo.ArgumentList.Add(scriptArg);
      }

      process.OutputDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
      process.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      await process.WaitForExitAsync();
      process.WaitForExit();

      return process.ExitCode;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Failed to execute compiled script: {ex.Message}", ex);
    }
    finally
    {
      DeleteTempFile(tempFile);
    }
  }

  private void EnsureRemoteExecutionAllowed(string inputPath, bool allowRemoteExecution)
  {
    if (IsValidUrl(inputPath) && !allowRemoteExecution)
    {
      throw new InvalidOperationException($"Refusing to execute remote script '{inputPath}' without {AllowRemoteFlag}. Re-run with {AllowRemoteFlag} to acknowledge the risk.");
    }
  }

  private static void DeleteTempFile(string tempFile)
  {
    if (!File.Exists(tempFile))
    {
      return;
    }

    try
    {
      File.Delete(tempFile);
    }
    catch (IOException ex)
    {
      Console.Error.WriteLine($"⚠️ Unable to delete temporary file '{tempFile}': {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
      Console.Error.WriteLine($"⚠️ Unable to delete temporary file '{tempFile}': {ex.Message}");
    }
  }

  private static HttpClient CreateHttpClient()
  {
    var client = new HttpClient
    {
      Timeout = TimeSpan.FromSeconds(30)
    };

    client.DefaultRequestHeaders.Add("User-Agent", "Utah-CLI/1.0");
    return client;
  }

  private string FormatInlineCommandForFile(string command)
  {
    command = FormatFunctionDefinitions(command);
    command = ExpandInlineControlBlocks(command);

    if (command.Contains("defer ") && !command.Contains("function "))
    {
      command = $"function main() {{\n  {command}\n}}\nmain();";
      command = FormatFunctionDefinitions(command);
      command = ExpandInlineControlBlocks(command);
    }

    command = string.Join("\n", SplitStatements(command).Where(statement => !string.IsNullOrWhiteSpace(statement)));
    command = EnsureStatementTerminators(command);

    command = Regex.Replace(command, @"\n\s*\n", "\n");

    return command;
  }

  private string ExpandInlineControlBlocks(string command)
  {
    string previous;
    do
    {
      previous = command;
      command = Regex.Replace(command,
        @"if\s*\(([^{}]+?)\)\s*\{\s*([^{}]+?)\s*\}\s*else\s*\{\s*([^{}]+?)\s*\}",
        "if ($1) {\n  $2\n}\nelse {\n  $3\n}",
        RegexOptions.Singleline);
      command = Regex.Replace(command,
        @"if\s*\(([^{}]+?)\)\s*\{\s*([^{}]+?)\s*\}",
        "if ($1) {\n  $2\n}",
        RegexOptions.Singleline);
      command = Regex.Replace(command,
        @"for\s*\(([^{}]+?)\)\s*\{\s*([^{}]+?)\s*\}",
        "for ($1) {\n  $2\n}",
        RegexOptions.Singleline);
      command = Regex.Replace(command,
        @"while\s*\(([^{}]+?)\)\s*\{\s*([^{}]+?)\s*\}",
        "while ($1) {\n  $2\n}",
        RegexOptions.Singleline);
      command = Regex.Replace(command,
        @"try\s*\{\s*([^{}]+?)\s*\}\s*catch\s*\{\s*([^{}]+?)\s*\}",
        "try {\n  $1\n}\ncatch {\n  $2\n}",
        RegexOptions.Singleline);
    } while (command != previous);

    return command;
  }

  private string EnsureStatementTerminators(string command)
  {
    var lines = command.Split('\n');
    for (int i = 0; i < lines.Length; i++)
    {
      var trimmedLine = lines[i].Trim();
      if (!NeedsInlineStatementTerminator(trimmedLine))
      {
        continue;
      }

      lines[i] = $"{lines[i].TrimEnd()};";
    }

    return string.Join("\n", lines);
  }

  private static bool NeedsInlineStatementTerminator(string trimmedLine)
  {
    if (string.IsNullOrWhiteSpace(trimmedLine))
    {
      return false;
    }

    if (trimmedLine.EndsWith(";") ||
        trimmedLine.EndsWith("{") ||
        trimmedLine == "}" ||
        trimmedLine.StartsWith("}") ||
        trimmedLine == "else {" ||
        trimmedLine == "catch {" ||
        trimmedLine.StartsWith("case ") ||
        trimmedLine == "default:")
    {
      return false;
    }

    return true;
  }

  private string FormatFunctionDefinitions(string command)
  {
    var functionPattern = @"function\s+(\w+)\s*\(([^)]*)\)\s*\{";
    var match = Regex.Match(command, functionPattern);

    if (!match.Success) return command;

    var funcName = match.Groups[1].Value;
    var parameters = match.Groups[2].Value;
    var startIndex = match.Index;
    var openBraceIndex = match.Index + match.Length - 1;

    var braceCount = 1;
    var endIndex = openBraceIndex + 1;

    while (endIndex < command.Length && braceCount > 0)
    {
      if (command[endIndex] == '{')
        braceCount++;
      else if (command[endIndex] == '}')
        braceCount--;
      endIndex++;
    }

    if (braceCount == 0)
    {
      var functionBody = command.Substring(openBraceIndex + 1, endIndex - openBraceIndex - 2);

      var statements = SplitStatements(functionBody.Trim());
      var formattedStatements = string.Join("\n  ", statements.Where(s => !string.IsNullOrWhiteSpace(s)));

      var formattedFunction = $"function {funcName}({parameters}) {{\n  {formattedStatements}\n}}";

      return command.Substring(0, startIndex) + formattedFunction + command.Substring(endIndex);
    }

    return command;
  }

  private List<string> SplitStatements(string body)
  {
    var statements = new List<string>();
    var current = "";
    var braceCount = 0;
    var bracketCount = 0;
    var parenCount = 0;
    var inString = false;
    var escapeNext = false;

    for (int i = 0; i < body.Length; i++)
    {
      var c = body[i];

      if (escapeNext)
      {
        current += c;
        escapeNext = false;
        continue;
      }

      if (c == '\\')
      {
        escapeNext = true;
        current += c;
        continue;
      }

      if (c == '"' && !escapeNext)
      {
        inString = !inString;
        current += c;
        continue;
      }

      if (inString)
      {
        current += c;
        continue;
      }

      if (c == '{')
      {
        braceCount++;
        current += c;
      }
      else if (c == '[')
      {
        bracketCount++;
        current += c;
      }
      else if (c == '(')
      {
        parenCount++;
        current += c;
      }
      else if (c == '}')
      {
        braceCount--;
        current += c;

        if (braceCount == 0 && (i == body.Length - 1 || IsStatementBoundary(body, i + 1)))
        {
          statements.Add(current.Trim());
          current = "";
        }
      }
      else if (c == ']')
      {
        bracketCount--;
        current += c;
      }
      else if (c == ')')
      {
        parenCount--;
        current += c;
      }
      else if (c == ';' && braceCount == 0 && bracketCount == 0 && parenCount == 0)
      {
        current += c;
        statements.Add(current.Trim());
        current = "";
      }
      else
      {
        current += c;
      }
    }

    if (!string.IsNullOrWhiteSpace(current))
    {
      statements.Add(current.Trim());
    }

    return statements;
  }

  private bool IsStatementBoundary(string text, int index)
  {
    while (index < text.Length && char.IsWhiteSpace(text[index]))
      index++;

    if (index >= text.Length) return true;

    var remaining = text.Substring(index);
    return remaining.StartsWith("if ") || remaining.StartsWith("return ") ||
           remaining.StartsWith("else") || remaining.StartsWith("catch") ||
           remaining.StartsWith("for ") || remaining.StartsWith("while ") ||
           remaining.StartsWith("let ") || remaining.StartsWith("const ") ||
           remaining.StartsWith("throw ") || remaining.StartsWith("try ") ||
           remaining.StartsWith("console.") ||
           Regex.IsMatch(remaining, @"^[A-Za-z_]\w*\s*\(");
  }

  private async Task StartLanguageServerAsync()
  {
    var server = await LanguageServer.From(options =>
        options
          .WithInput(Console.OpenStandardInput())
          .WithOutput(Console.OpenStandardOutput())
          .AddHandler<CompletionHandler>());
    await server.WaitForExit;
  }

  private string? GetOutputPath(string[] args)
  {
    for (int i = 0; i < args.Length - 1; i++)
    {
      if (args[i] == "-o" || args[i] == "--output")
      {
        return args[i + 1];
      }
    }

    return null;
  }

  private async Task<int> CompileFileAsync(string inputPath, string? outputPath = null)
  {
    var content = await LoadShxContentAsync(inputPath);
    var output = CompileShxContent(content);

    var finalOutputPath = outputPath ?? (IsValidUrl(inputPath)
      ? Path.ChangeExtension(Path.GetFileName(inputPath), ".sh")
      : Path.ChangeExtension(inputPath, ".sh"));

    EnsureOutputDirectoryExists(finalOutputPath);
    await File.WriteAllTextAsync(finalOutputPath, output);
    Console.WriteLine($"✅ Compiled: {finalOutputPath}");
    return 0;
  }

  private int FormatFile(string[] args)
  {
    var inputPath = args[1];
    string? outputPath = null;
    bool inPlace = false;
    bool checkOnly = false;

    for (int i = 2; i < args.Length; i++)
    {
      switch (args[i])
      {
        case "-o":
        case "--output":
          if (i + 1 < args.Length)
          {
            outputPath = args[i + 1];
            i++;
          }
          break;
        case "--in-place":
          inPlace = true;
          break;
        case "--check":
          checkOnly = true;
          break;
      }
    }

    if (!File.Exists(inputPath))
    {
      throw new InvalidOperationException($"File not found: {inputPath}");
    }

    var options = FormattingOptions.FromEditorConfig(inputPath);
    var formatter = new Formatter(options);
    var formattedContent = formatter.Format(inputPath);

    if (checkOnly)
    {
      var originalContent = File.ReadAllText(inputPath);
      if (originalContent != formattedContent)
      {
        Console.WriteLine($"❌ File is not formatted: {inputPath}");
        return 1;
      }

      Console.WriteLine($"✅ File is properly formatted: {inputPath}");
      return 0;
    }

    var finalOutputPath = outputPath ?? (inPlace ? inputPath : Path.ChangeExtension(inputPath, ".formatted.shx"));
    EnsureOutputDirectoryExists(finalOutputPath);
    File.WriteAllText(finalOutputPath, formattedContent);

    if (inPlace)
    {
      Console.WriteLine($"✅ Formatted in place: {inputPath}");
    }
    else
    {
      Console.WriteLine($"✅ Formatted: {finalOutputPath}");
    }

    return 0;
  }

  private int FormatAllFiles(string[] args)
  {
    bool inPlace = false;
    bool checkOnly = false;

    for (int i = 1; i < args.Length; i++)
    {
      switch (args[i])
      {
        case "--in-place":
          inPlace = true;
          break;
        case "--check":
          checkOnly = true;
          break;
        case "-o":
        case "--output":
          throw new InvalidOperationException("-o/--output option is not supported when formatting all files. Use --in-place instead.");
      }
    }

    var currentDirectory = Directory.GetCurrentDirectory();
    var shxFiles = EnumerateShxFiles(currentDirectory).ToList();

    if (shxFiles.Count == 0)
    {
      Console.WriteLine("No .shx files found in current directory and supported subdirectories.");
      return 0;
    }

    Console.WriteLine($"Found {shxFiles.Count} .shx file(s) to format:");

    int formattedCount = 0;
    int alreadyFormattedCount = 0;
    int errorCount = 0;

    foreach (var filePath in shxFiles)
    {
      try
      {
        var relativePath = Path.GetRelativePath(currentDirectory, filePath);
        Console.Write($"  {relativePath}... ");

        var options = FormattingOptions.FromEditorConfig(filePath);
        var formatter = new Formatter(options);
        var formattedContent = formatter.Format(filePath);
        var originalContent = File.ReadAllText(filePath);

        if (checkOnly)
        {
          if (originalContent != formattedContent)
          {
            Console.WriteLine("❌ not formatted");
            formattedCount++;
          }
          else
          {
            Console.WriteLine("✅ already formatted");
            alreadyFormattedCount++;
          }

          continue;
        }

        if (originalContent != formattedContent)
        {
          if (inPlace)
          {
            File.WriteAllText(filePath, formattedContent);
            Console.WriteLine("✅ formatted");
          }
          else
          {
            var outputPath = Path.ChangeExtension(filePath, ".formatted.shx");
            EnsureOutputDirectoryExists(outputPath);
            File.WriteAllText(outputPath, formattedContent);
            Console.WriteLine($"✅ formatted -> {Path.GetRelativePath(currentDirectory, outputPath)}");
          }

          formattedCount++;
        }
        else
        {
          Console.WriteLine("✅ already formatted");
          alreadyFormattedCount++;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"❌ error: {ex.Message}");
        errorCount++;
      }
    }

    Console.WriteLine();
    if (checkOnly)
    {
      Console.WriteLine($"Summary: {alreadyFormattedCount} properly formatted, {formattedCount} need formatting, {errorCount} errors");
      if (formattedCount > 0)
      {
        Console.WriteLine("Run 'utah format --in-place' to format all files.");
      }

      return formattedCount > 0 || errorCount > 0 ? 1 : 0;
    }

    Console.WriteLine($"Summary: {formattedCount} formatted, {alreadyFormattedCount} already formatted, {errorCount} errors");
    return errorCount > 0 ? 1 : 0;
  }

  private static IEnumerable<string> EnumerateShxFiles(string rootDirectory)
  {
    foreach (var file in Directory.EnumerateFiles(rootDirectory, "*.shx"))
    {
      yield return file;
    }

    foreach (var directory in Directory.EnumerateDirectories(rootDirectory))
    {
      if (ShouldSkipDirectory(directory))
      {
        continue;
      }

      foreach (var file in EnumerateShxFiles(directory))
      {
        yield return file;
      }
    }
  }

  private static bool ShouldSkipDirectory(string directoryPath)
  {
    var directoryName = Path.GetFileName(directoryPath);
    if (s_formatExcludedDirectories.Contains(directoryName))
    {
      return true;
    }

    try
    {
      return File.GetAttributes(directoryPath).HasFlag(FileAttributes.ReparsePoint);
    }
    catch (IOException)
    {
      return true;
    }
    catch (UnauthorizedAccessException)
    {
      return true;
    }
  }

  private static void EnsureOutputDirectoryExists(string outputPath)
  {
    var directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
    if (!string.IsNullOrEmpty(directory))
    {
      Directory.CreateDirectory(directory);
    }
  }

  private void PrintCompileUsage()
  {
    Console.WriteLine("Usage: utah compile <file.shx|url> [-o, --output <output.sh>]");
  }

  private void PrintRunUsage()
  {
    Console.WriteLine($"Usage: utah run [{AllowRemoteFlag}] <file.shx|url> [-- script-args...]");
    Console.WriteLine("       utah run -c <command>");
    Console.WriteLine("       utah run --command <command>");
  }

  private void PrintInlineRunUsage()
  {
    Console.WriteLine("Usage: utah run -c <command>");
    Console.WriteLine("       utah run --command <command>");
  }

  private void PrintDirectCommandUsage()
  {
    Console.WriteLine("Usage: utah -c <command>");
    Console.WriteLine("       utah --command <command>");
  }

  private void PrintFormatUsage()
  {
    Console.WriteLine("Usage: utah format [file.shx] [-o, --output <output.shx>] [--in-place] [--check]");
    Console.WriteLine("       utah format                   # Format all .shx files recursively");
  }

  private void PrintUsage()
  {
    Console.WriteLine($"{Bold("Usage:")} utah {Cyan("<command|file.shx|url>")}");
    Console.WriteLine();
    Console.WriteLine(Yellow("Direct Execution:"));
    Console.WriteLine($"  {Green("<https://url/file.shx>")} [{Cyan(AllowRemoteFlag)}] [-- script-args...]");
    Console.WriteLine($"  {Green("<file.shx>")} [-- script-args...]     {Dim("Compile and run a .shx file directly.")}");
    Console.WriteLine($"  {Green("-c, --command")} <command>            {Dim("Run a single shx command directly.")}");
    Console.WriteLine();
    Console.WriteLine(Yellow("Commands:"));
    Console.WriteLine($"  {Green("run")} [{Cyan(AllowRemoteFlag)}] <file.shx|url> [-- script-args...]");
    Console.WriteLine($"  {Green("run")} {Green("-c, --command")} <command>  {Dim("Run a single shx command directly.")}");
    Console.WriteLine($"  {Green("compile")} <file.shx|url>       {Dim("Compile a .shx file or URL to a .sh file.")}");
    Console.WriteLine($"    {Yellow("Options:")}");
    Console.WriteLine($"      {Cyan("-o, --output")} <file>      {Dim("Write compiled output to a specific file.")}");
    Console.WriteLine($"  {Green("format")} [file.shx]            {Dim("Format .shx file(s) according to EditorConfig rules.")}");
    Console.WriteLine($"    {Yellow("Options:")}");
    Console.WriteLine($"      {Dim("(no file)")}                {Dim("Format all .shx files recursively from current directory.")}");
    Console.WriteLine($"      {Cyan("-o, --output")} <file>      {Dim("Write formatted output to a specific file (single file only).")}");
    Console.WriteLine($"      {Cyan("--in-place")}               {Dim("Format the file(s) in place (overwrite original).")}");
    Console.WriteLine($"      {Cyan("--check")}                  {Dim("Check if file(s) are formatted (exit 1 if not).")}");
    Console.WriteLine($"      {Cyan(AllowRemoteFlag)}           {Dim("Acknowledge and allow executing a remote URL.")}");
    Console.WriteLine($"  {Green("lsp")}                          {Dim("Run the language server.")}");
    Console.WriteLine($"  {Green("version")} (--version, -v)      {Dim("Show version information.")}");
    Console.WriteLine();
    Console.WriteLine(Yellow("Examples:"));
    Console.WriteLine($"  {Dim("$")} utah script.shx");
    Console.WriteLine($"  {Dim("$")} utah run {AllowRemoteFlag} https://utahshx.com/examples/script.shx");
    Console.WriteLine($"  {Dim("$")} utah compile https://gist.githubusercontent.com/user/hash/raw/script.shx");
  }

  private void PrintVersion()
  {
    var assembly = typeof(Program).Assembly;
    var version = assembly.GetName().Version;
    var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";

    Console.WriteLine($"{Bold("Utah CLI")} {Green(versionString)}");
    Console.WriteLine($"{Dim("Copyright (c) 2025 Utah Project - MIT License")}");
    Console.WriteLine();
    Console.WriteLine(Yellow("Project Information:"));
    Console.WriteLine($"  {"Version:",-16} {Cyan(versionString)}");
    Console.WriteLine();
    Console.WriteLine(Yellow("Runtime Information:"));
    Console.WriteLine($"  {".NET Version:",-16} {Cyan(Environment.Version.ToString())}");
    Console.WriteLine($"  {"OS:",-16} {Cyan(Environment.OSVersion.ToString())}");
    Console.WriteLine($"  {"Architecture:",-16} {Cyan(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString())}");
  }
}

enum ArgumentType
{
  KnownCommand,
  InlineCommand,
  File,
  Unknown
}
