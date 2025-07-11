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
    var input = File.ReadAllText(inputPath);
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
    var input = File.ReadAllText(inputPath);
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

static void PrintUsage()
{
  Console.WriteLine("Usage: utah <command>");
  Console.WriteLine("Commands:");
  Console.WriteLine("  run <file.shx>           Compile and run a .shx file.");
  Console.WriteLine("  compile <file.shx>       Compile a .shx file to a .sh file.");
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
