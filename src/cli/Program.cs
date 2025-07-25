using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;
using System.Reflection;

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
          ExecuteShxFile(args[0]);
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
    // Known commands
    if (IsKnownCommand(firstArg)) return ArgumentType.KnownCommand;

    // Command flags
    if (firstArg == "--command" || firstArg == "-c") return ArgumentType.InlineCommand;

    // File detection
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

  private async Task HandleKnownCommandAsync(string[] args)
  {
    switch (args[0])
    {
      case "lsp":
        await StartLanguageServerAsync();
        break;
      case "compile":
        if (args.Length < 2 || !args[1].EndsWith(".shx"))
        {
          Console.WriteLine("Usage: utah compile <file.shx> [-o <output.sh>]");
          return;
        }
        var inputPath = args[1];
        var outputPath = GetOutputPath(args);
        CompileFile(inputPath, outputPath);
        break;
      case "run":
        if (args.Length < 2)
        {
          Console.WriteLine("Usage: utah run <file.shx>");
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
        else if (args.Length == 2 && args[1].EndsWith(".shx"))
        {
          ExecuteShxFile(args[1]);
        }
        else
        {
          Console.WriteLine("Usage: utah run <file.shx>");
          Console.WriteLine("       utah run -c <command>");
          Console.WriteLine("       utah run --command <command>");
          return;
        }
        break;
      case "format":
        if (args.Length < 2)
        {
          // No file provided, format all .shx files recursively
          FormatAllFiles(args);
        }
        else if (args[1].StartsWith("-"))
        {
          // Options provided without file, format all .shx files recursively with options
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

  private void ExecuteShxFile(string filePath)
  {
    RunFile(filePath);
  }

  private void ExecuteInlineCommand(string command)
  {
    RunCommand(command);
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
      if (args[i] == "-o")
      {
        return args[i + 1];
      }
    }
    return null;
  }

  private void CompileFile(string inputPath, string? outputPath = null)
  {
    if (!File.Exists(inputPath))
    {
      Console.WriteLine($"File not found: {inputPath}");
      return;
    }

    try
    {
      var input = ResolveImports(inputPath);
      var parser = new Parser(input);
      var ast = parser.Parse();
      var compiler = new Compiler();
      var output = compiler.Compile(ast);

      var finalOutputPath = outputPath ?? Path.ChangeExtension(inputPath, ".sh");
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

  private void RunFile(string inputPath)
  {
    if (!File.Exists(inputPath))
    {
      Console.WriteLine($"File not found: {inputPath}");
      return;
    }

    try
    {
      var input = ResolveImports(inputPath);
      var parser = new Parser(input);
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

  private void RunCommand(string command)
  {
    try
    {
      // Create a temporary .shx content with the command
      var shxContent = command;

      // Parse and compile the command
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

    // Parse additional arguments
    for (int i = 2; i < args.Length; i++)
    {
      switch (args[i])
      {
        case "-o":
        case "--output":
          if (i + 1 < args.Length)
          {
            outputPath = args[i + 1];
            i++; // Skip the next argument as it's the output path
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
      // Get formatting options from EditorConfig
      var options = FormattingOptions.FromEditorConfig(inputPath);
      var formatter = new Formatter(options);

      // Format the file
      var formattedContent = formatter.Format(inputPath);

      if (checkOnly)
      {
        // Check if file is already formatted
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

      // Determine output path
      var finalOutputPath = outputPath ?? (inPlace ? inputPath : Path.ChangeExtension(inputPath, ".formatted.shx"));

      // Write formatted content
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

    // Parse arguments (skip the first "format" argument)
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
      // Find all .shx files recursively starting from current directory
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

          // Get formatting options from EditorConfig
          var options = FormattingOptions.FromEditorConfig(filePath);
          var formatter = new Formatter(options);

          // Format the file
          var formattedContent = formatter.Format(filePath);

          if (checkOnly)
          {
            // Check if file is already formatted
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
            // Check if formatting is needed
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
      return; // Avoid circular imports
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
        // Parse import statement
        var match = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"import\s+([""']?)([^""';]+)\1;?");
        if (match.Success)
        {
          var importPath = match.Groups[2].Value;
          var fullImportPath = Path.IsPathRooted(importPath)
            ? importPath
            : Path.Combine(baseDirectory, importPath);

          // Resolve the imported file recursively
          ResolveImportsRecursive(fullImportPath, resolvedFiles, result);
        }
      }
      else
      {
        // Add non-import lines to result
        result.Add(line);
      }
    }
  }

  private void PrintUsage()
  {
    Console.WriteLine("Usage: utah <command|file.shx>");
    Console.WriteLine();
    Console.WriteLine("Direct Execution:");
    Console.WriteLine("  <file.shx>                   Compile and run a .shx file directly.");
    Console.WriteLine("  -c, --command <command>      Run a single shx command directly.");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  run <file.shx>               Compile and run a .shx file.");
    Console.WriteLine("  run -c, --command <command>  Run a single shx command directly.");
    Console.WriteLine("  compile <file.shx>           Compile a .shx file to a .sh file.");
    Console.WriteLine("    Options:");
    Console.WriteLine("      -o <file>                Write formatted output to a specific file (single file only).");
    Console.WriteLine("  format [file.shx]            Format .shx file(s) according to EditorConfig rules.");
    Console.WriteLine("    Options:");
    Console.WriteLine("      (no file)                Format all .shx files recursively from current directory.");
    Console.WriteLine("      -o, --output <file>      Write formatted output to a specific file (single file only).");
    Console.WriteLine("      --in-place               Format the file(s) in place (overwrite original).");
    Console.WriteLine("      --check                  Check if file(s) are formatted (exit 1 if not).");
    Console.WriteLine("  lsp                          Run the language server.");
    Console.WriteLine("  version (--version, -v)      Show version information.");
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
