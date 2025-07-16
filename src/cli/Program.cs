using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;
using System.Reflection;

if (args.Length > 0)
{
  switch (args[0])
  {
    case "lsp":
      await StartLanguageServer();
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
      if (args.Length != 2 || !args[1].EndsWith(".shx"))
      {
        Console.WriteLine("Usage: utah run <file.shx>");
        return;
      }
      RunFile(args[1]);
      break;
    case "format":
      if (args.Length < 2 || !args[1].EndsWith(".shx"))
      {
        Console.WriteLine("Usage: utah format <file.shx> [-o <output.shx>] [--in-place] [--check]");
        return;
      }
      FormatFile(args);
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
else
{
  PrintUsage();
}

static async Task StartLanguageServer()
{
  var server = await LanguageServer.From(options =>
      options
        .WithInput(Console.OpenStandardInput())
        .WithOutput(Console.OpenStandardOutput())
        .AddHandler<CompletionHandler>());
  await server.WaitForExit;
}

static string? GetOutputPath(string[] args)
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

static void CompileFile(string inputPath, string? outputPath = null)
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

static void RunFile(string inputPath)
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

static void FormatFile(string[] args)
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

static string ResolveImports(string filePath)
{
  var resolvedFiles = new HashSet<string>();
  var result = new List<string>();

  ResolveImportsRecursive(filePath, resolvedFiles, result);

  return string.Join(Environment.NewLine, result);
}

static void ResolveImportsRecursive(string filePath, HashSet<string> resolvedFiles, List<string> result)
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

static void PrintUsage()
{
  Console.WriteLine("Usage: utah <command>");
  Console.WriteLine("Commands:");
  Console.WriteLine("  run <file.shx>           Compile and run a .shx file.");
  Console.WriteLine("  compile <file.shx>       Compile a .shx file to a .sh file.");
  Console.WriteLine("  format <file.shx>        Format a .shx file according to EditorConfig rules.");
  Console.WriteLine("    Options:");
  Console.WriteLine("      -o <output.shx>      Write formatted output to a specific file.");
  Console.WriteLine("      --in-place          Format the file in place (overwrite original).");
  Console.WriteLine("      --check             Check if file is formatted (exit 1 if not).");
  Console.WriteLine("  lsp                      Run the language server.");
  Console.WriteLine("  version (--version, -v)  Show version information.");
}

static void PrintVersion()
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
