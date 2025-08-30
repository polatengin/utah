using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;
using System.Text.RegularExpressions;

var app = new UtahApp();
await app.RunAsync(args);

class UtahApp
{
  public async Task RunAsync(string[] args)
  {
    if (args.Length > 0)
    {
      var argumentType = DetectArgumentType(args[0]);

      switch (argumentType)
      {
        case ArgumentType.KnownCommand:
          await HandleKnownCommandAsync(args);
          break;
        case ArgumentType.InlineCommand:
          HandleInlineCommand(args);
          break;
        case ArgumentType.File:
          await ExecuteShxFileAsync(args[0]);
          break;
        case ArgumentType.Unknown:
          PrintUsage();
          break;
      }
    }
    else
    {
      PrintUsage();
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
           (uri.Scheme == "http" || uri.Scheme == "https");
  }

  private async Task<string> DownloadFileContentAsync(string url)
  {
    try
    {
      using var httpClient = new HttpClient();
      httpClient.Timeout = TimeSpan.FromSeconds(30);

      httpClient.DefaultRequestHeaders.Add("User-Agent", "Utah-CLI/1.0");

      var response = await httpClient.GetAsync(url);
      response.EnsureSuccessStatusCode();

      var content = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"✅ Downloaded {content.Length} characters from {url}");

      return content;
    }
    catch (HttpRequestException ex)
    {
      Console.WriteLine($"❌ HTTP error downloading {url}: {ex.Message}");
      Environment.Exit(1);
      return string.Empty;
    }
    catch (TaskCanceledException ex)
    {
      Console.WriteLine($"❌ Timeout downloading {url}: {ex.Message}");
      Environment.Exit(1);
      return string.Empty;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Error downloading {url}: {ex.Message}");
      Environment.Exit(1);
      return string.Empty;
    }
  }

  private async Task HandleKnownCommandAsync(string[] args)
  {
    switch (args[0])
    {
      case "lsp":
        await StartLanguageServerAsync();
        break;
      case "compile":
        if (args.Length < 2 || (!args[1].EndsWith(".shx") && !IsValidUrl(args[1])))
        {
          Console.WriteLine("Usage: utah compile <file.shx|url> [-o, --output <output.sh>]");
          return;
        }
        var inputPath = args[1];
        var outputPath = GetOutputPath(args);
        await CompileFileAsync(inputPath, outputPath);
        break;
      case "run":
        if (args.Length < 2)
        {
          Console.WriteLine("Usage: utah run <file.shx> [-- script-args...]");
          Console.WriteLine("       utah run -c <command>");
          Console.WriteLine("       utah run --command <command>");
          return;
        }

        if (args[1] == "-c" || args[1] == "--command")
        {
          if (args.Length < 3)
          {
            Console.WriteLine("Usage: utah run -c <command>");
            Console.WriteLine("       utah run --command <command>");
            return;
          }
          var command = string.Join(" ", args.Skip(2));
          ExecuteInlineCommand(command);
        }
        else if (args[1].EndsWith(".shx") || IsValidUrl(args[1]))
        {
          var (scriptPath, scriptArgs) = ParseRunArguments(args);
          await ExecuteShxFileAsync(scriptPath, scriptArgs);
        }
        else
        {
          Console.WriteLine("Usage: utah run <file.shx> [-- script-args...]");
          Console.WriteLine("       utah run -c <command>");
          Console.WriteLine("       utah run --command <command>");
          return;
        }
        break;
      case "format":
        if (args.Length < 2)
        {
          FormatAllFiles(args);
        }
        else if (args[1].StartsWith("-"))
        {
          FormatAllFiles(args);
        }
        else if (!args[1].EndsWith(".shx"))
        {
          Console.WriteLine("Usage: utah format [file.shx] [-o, --output <output.shx>] [--in-place] [--check]");
          Console.WriteLine("       utah format                   # Format all .shx files recursively");
          return;
        }
        else
        {
          FormatFile(args);
        }
        break;
      case "--version":
      case "-v":
      case "version":
        PrintVersion();
        break;
      default:
        PrintUsage();
        break;
    }
  }

  private void HandleInlineCommand(string[] args)
  {
    if (args.Length < 2)
    {
      Console.WriteLine("Usage: utah -c <command>");
      Console.WriteLine("       utah --command <command>");
      return;
    }

    var command = string.Join(" ", args.Skip(1));
    ExecuteInlineCommand(command);
  }

  private (string scriptPath, string[] scriptArgs) ParseRunArguments(string[] args)
  {
    var scriptPath = args[1];
    var scriptArgs = new string[0];

    var separatorIndex = Array.IndexOf(args, "--");
    if (separatorIndex != -1 && separatorIndex < args.Length - 1)
    {
      scriptArgs = args.Skip(separatorIndex + 1).ToArray();
    }

    return (scriptPath, scriptArgs);
  }

  private string EscapeShellArgument(string arg)
  {
    if (string.IsNullOrEmpty(arg))
    {
      return "\"\"";
    }

    if (arg.Contains(' ') || arg.Contains('"') || arg.Contains('\'') || arg.Contains('$') || arg.Contains('`') || arg.Contains('\\'))
    {
      return "\"" + arg.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }

    return arg;
  }

  private async Task ExecuteShxFileAsync(string filePath)
  {
    await RunFileAsync(filePath, Array.Empty<string>());
  }

  private async Task ExecuteShxFileAsync(string filePath, string[] scriptArgs)
  {
    await RunFileAsync(filePath, scriptArgs);
  }

  private void ExecuteInlineCommand(string command)
  {
    var normalizedCommand = command.Trim();
    if (!normalizedCommand.EndsWith(";"))
    {
      normalizedCommand += ";";
    }

    if (ContainsComplexConstructs(normalizedCommand))
    {
      ExecuteInlineCommandAsFile(normalizedCommand);
    }
    else
    {
      RunCommand(normalizedCommand);
    }
  }

  private bool ContainsComplexConstructs(string command)
  {
    return command.Contains("function ") ||
           command.Contains("try {") ||
           command.Contains("defer ") ||
           command.Contains("catch ");
  }

  private void ExecuteInlineCommandAsFile(string command)
  {
    var tempFile = Path.GetTempFileName() + ".shx";
    try
    {
      var formattedCommand = FormatInlineCommandForFile(command);
      File.WriteAllText(tempFile, formattedCommand);

      RunFileSynchronously(tempFile);
    }
    finally
    {
      if (File.Exists(tempFile))
      {
        File.Delete(tempFile);
      }
    }
  }

  private void RunFileSynchronously(string filePath)
  {
    try
    {
      var shxContent = File.ReadAllText(filePath);
      var parser = new Parser(shxContent);
      var ast = parser.Parse();
      var compiler = new Compiler();
      var output = compiler.Compile(ast);

      var tempFile = Path.GetTempFileName();
      File.WriteAllText(tempFile, output);

      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          Arguments = tempFile,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };

      process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
      process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      process.WaitForExit();

      File.Delete(tempFile);
    }
    catch (InvalidOperationException ex)
    {
      Console.WriteLine($"❌ Compilation failed: {ex.Message}");
      Environment.Exit(1);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Unexpected error: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private string FormatInlineCommandForFile(string command)
  {
    command = FormatFunctionDefinitions(command);

    command = Regex.Replace(command, @"try\s*\{\s*([^}]+)\s*\}\s*catch\s*\{\s*([^}]+)\s*\}",
                           "try {\n  $1\n}\ncatch {\n  $2\n}");

    if (command.Contains("defer ") && !command.Contains("function "))
    {
      command = $"function main() {{\n  {command}\n}}\nmain();";
    }

    command = Regex.Replace(command, @"\}\s*([a-zA-Z_]\w*)", "}\n$1");

    command = Regex.Replace(command, @";\s*(?=\w)", ";\n");

    command = Regex.Replace(command, @"\n\s*\n", "\n");

    return command;
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
      else if (c == ';' && braceCount == 0)
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
           remaining.StartsWith("let ") || remaining.StartsWith("const ") ||
           remaining.StartsWith("throw ") || remaining.StartsWith("try ") ||
           remaining.StartsWith("console.");
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

  private async Task CompileFileAsync(string inputPath, string? outputPath = null)
  {
    string content;
    bool isUrl = IsValidUrl(inputPath);

    if (isUrl)
    {
      Console.WriteLine($"📥 Downloading: {inputPath}");
      content = await DownloadFileContentAsync(inputPath);
    }
    else
    {
      if (!File.Exists(inputPath))
      {
        Console.WriteLine($"File not found: {inputPath}");
        return;
      }
      content = ResolveImports(inputPath);
    }

    try
    {
      var parser = new Parser(content);
      var ast = parser.Parse();
      var compiler = new Compiler();
      var output = compiler.Compile(ast);

      var finalOutputPath = outputPath ?? (isUrl ? Path.ChangeExtension(Path.GetFileName(inputPath), ".sh") : Path.ChangeExtension(inputPath, ".sh"));

      File.WriteAllText(finalOutputPath, output);
      Console.WriteLine($"✅ Compiled: {finalOutputPath}");
    }
    catch (InvalidOperationException ex)
    {
      Console.WriteLine($"❌ Compilation failed: {ex.Message}");
      Environment.Exit(1);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Unexpected error: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private async Task RunFileAsync(string inputPath, string[] scriptArgs)
  {
    string content;
    string actualPath = inputPath;

    if (IsValidUrl(inputPath))
    {
      Console.WriteLine($"📥 Downloading: {inputPath}");
      content = await DownloadFileContentAsync(inputPath);
      actualPath = $"<remote:{inputPath}>";
    }
    else
    {
      if (!File.Exists(inputPath))
      {
        Console.WriteLine($"File not found: {inputPath}");
        return;
      }
      content = ResolveImports(inputPath);
    }

    try
    {
      var parser = new Parser(content);
      var ast = parser.Parse();
      var compiler = new Compiler();
      var output = compiler.Compile(ast);

      var tempFile = Path.GetTempFileName();
      File.WriteAllText(tempFile, output);

      var bashArgs = tempFile;
      if (scriptArgs.Length > 0)
      {
        var escapedArgs = scriptArgs.Select(EscapeShellArgument);
        bashArgs += " " + string.Join(" ", escapedArgs);
      }

      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          Arguments = bashArgs,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };

      process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
      process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      process.WaitForExit();

      File.Delete(tempFile);
    }
    catch (InvalidOperationException ex)
    {
      Console.WriteLine($"❌ Compilation failed: {ex.Message}");
      Environment.Exit(1);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Unexpected error: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private void RunCommand(string command)
  {
    try
    {
      var shxContent = command;

      var parser = new Parser(shxContent);
      var ast = parser.Parse();
      var compiler = new Compiler();
      var output = compiler.Compile(ast);

      var tempFile = Path.GetTempFileName();
      File.WriteAllText(tempFile, output);

      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "/bin/bash",
          Arguments = tempFile,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          UseShellExecute = false,
          CreateNoWindow = true,
        }
      };

      process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
      process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      process.WaitForExit();

      File.Delete(tempFile);
    }
    catch (InvalidOperationException ex)
    {
      Console.WriteLine($"❌ Compilation failed: {ex.Message}");
      Environment.Exit(1);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Unexpected error: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private void FormatFile(string[] args)
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
      Console.WriteLine($"File not found: {inputPath}");
      Environment.Exit(1);
      return;
    }

    try
    {
      var options = FormattingOptions.FromEditorConfig(inputPath);
      var formatter = new Formatter(options);

      var formattedContent = formatter.Format(inputPath);

      if (checkOnly)
      {
        var originalContent = File.ReadAllText(inputPath);
        if (originalContent != formattedContent)
        {
          Console.WriteLine($"❌ File is not formatted: {inputPath}");
          Environment.Exit(1);
        }
        else
        {
          Console.WriteLine($"✅ File is properly formatted: {inputPath}");
        }
        return;
      }

      var finalOutputPath = outputPath ?? (inPlace ? inputPath : Path.ChangeExtension(inputPath, ".formatted.shx"));

      File.WriteAllText(finalOutputPath, formattedContent);

      if (inPlace)
      {
        Console.WriteLine($"✅ Formatted in place: {inputPath}");
      }
      else
      {
        Console.WriteLine($"✅ Formatted: {finalOutputPath}");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Formatting failed: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private void FormatAllFiles(string[] args)
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
          Console.WriteLine("❌ -o/--output option is not supported when formatting all files. Use --in-place instead.");
          Environment.Exit(1);
          return;
      }
    }

    try
    {
      var currentDirectory = Directory.GetCurrentDirectory();
      var shxFiles = Directory.GetFiles(currentDirectory, "*.shx", SearchOption.AllDirectories);

      if (shxFiles.Length == 0)
      {
        Console.WriteLine("No .shx files found in current directory and subdirectories.");
        return;
      }

      Console.WriteLine($"Found {shxFiles.Length} .shx file(s) to format:");

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

          if (checkOnly)
          {
            var originalContent = File.ReadAllText(filePath);
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
          }
          else
          {
            var originalContent = File.ReadAllText(filePath);
            if (originalContent != formattedContent)
            {
              if (inPlace)
              {
                File.WriteAllText(filePath, formattedContent);
                Console.WriteLine("✅ formatted");
                formattedCount++;
              }
              else
              {
                var outputPath = Path.ChangeExtension(filePath, ".formatted.shx");
                File.WriteAllText(outputPath, formattedContent);
                Console.WriteLine($"✅ formatted -> {Path.GetRelativePath(currentDirectory, outputPath)}");
                formattedCount++;
              }
            }
            else
            {
              Console.WriteLine("✅ already formatted");
              alreadyFormattedCount++;
            }
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
          Environment.Exit(1);
        }
      }
      else
      {
        Console.WriteLine($"Summary: {formattedCount} formatted, {alreadyFormattedCount} already formatted, {errorCount} errors");
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Error finding .shx files: {ex.Message}");
      Environment.Exit(1);
    }
  }

  private string ResolveImports(string filePath)
  {
    var resolvedFiles = new HashSet<string>();
    var result = new List<string>();

    ResolveImportsRecursive(filePath, resolvedFiles, result);

    return string.Join(Environment.NewLine, result);
  }

  private void ResolveImportsRecursive(string filePath, HashSet<string> resolvedFiles, List<string> result)
  {
    var absolutePath = Path.GetFullPath(filePath);

    if (resolvedFiles.Contains(absolutePath))
    {
      return;
    }

    resolvedFiles.Add(absolutePath);

    if (!File.Exists(absolutePath))
    {
      throw new InvalidOperationException($"Import file not found: {filePath}");
    }

    var content = File.ReadAllText(absolutePath);
    var lines = content.Split('\n');
    var baseDirectory = Path.GetDirectoryName(absolutePath) ?? "";

    foreach (var line in lines)
    {
      var trimmedLine = line.Trim();

      if (trimmedLine.StartsWith("import "))
      {
        var match = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"import\s+([""']?)([^""';]+)\1;?");
        if (match.Success)
        {
          var importPath = match.Groups[2].Value;
          var fullImportPath = Path.IsPathRooted(importPath)
            ? importPath
            : Path.Combine(baseDirectory, importPath);

          ResolveImportsRecursive(fullImportPath, resolvedFiles, result);
        }
      }
      else
      {
        result.Add(line);
      }
    }
  }

  private void PrintUsage()
  {
    Console.WriteLine("Usage: utah <command|file.shx|url>");
    Console.WriteLine();
    Console.WriteLine("Direct Execution:");
    Console.WriteLine("  <file.shx>                   Compile and run a .shx file directly.");
    Console.WriteLine("  <https://url/file.shx>       Download and run a .shx file from URL.");
    Console.WriteLine("  -c, --command <command>      Run a single shx command directly.");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  run <file.shx|url>           Compile and run a .shx file or URL.");
    Console.WriteLine("  run -c, --command <command>  Run a single shx command directly.");
    Console.WriteLine("  compile <file.shx|url>       Compile a .shx file or URL to a .sh file.");
    Console.WriteLine("    Options:");
    Console.WriteLine("      -o, --output <file>      Write compiled output to a specific file.");
    Console.WriteLine("  format [file.shx]            Format .shx file(s) according to EditorConfig rules.");
    Console.WriteLine("    Options:");
    Console.WriteLine("      (no file)                Format all .shx files recursively from current directory.");
    Console.WriteLine("      -o, --output <file>      Write formatted output to a specific file (single file only).");
    Console.WriteLine("      --in-place               Format the file(s) in place (overwrite original).");
    Console.WriteLine("      --check                  Check if file(s) are formatted (exit 1 if not).");
    Console.WriteLine("  lsp                          Run the language server.");
    Console.WriteLine("  version (--version, -v)      Show version information.");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  utah script.shx");
    Console.WriteLine("  utah run https://utahshx.com/examples/script.shx");
    Console.WriteLine("  utah compile https://gist.githubusercontent.com/user/hash/raw/script.shx");
  }

  private void PrintVersion()
  {
    var assembly = typeof(Program).Assembly;
    var version = assembly.GetName().Version;
    var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";

    Console.WriteLine("Copyright (c) 2025 Utah Project");
    Console.WriteLine("Licensed under MIT License");
    Console.WriteLine();
    Console.WriteLine("Project Information:");
    Console.WriteLine($"  Version: {versionString}");
    Console.WriteLine();
    Console.WriteLine("Runtime Information:");
    Console.WriteLine($"  DotNet Version: {Environment.Version}");
    Console.WriteLine($"  OS: {Environment.OSVersion}");
    Console.WriteLine($"  Architecture: {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
  }
}

enum ArgumentType
{
  KnownCommand,
  InlineCommand,
  File,
  Unknown
}
