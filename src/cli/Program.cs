using OmniSharp.Extensions.LanguageServer.Server;
using System.Diagnostics;

if (args.Length > 0)
{
  switch (args[0])
  {
    case "lsp":
      await StartLanguageServer();
      break;
    case "compile":
      if (args.Length != 2 || !args[1].EndsWith(".shx"))
      {
        Console.WriteLine("Usage: utah compile <file.shx>");
        return;
      }
      CompileFile(args[1]);
      break;
    case "run":
      if (args.Length != 2 || !args[1].EndsWith(".shx"))
      {
        Console.WriteLine("Usage: utah run <file.shx>");
        return;
      }
      RunFile(args[1]);
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
        .WithOutput(Console.OpenStandardOutput()));
  await server.WaitForExit;
}

static void CompileFile(string inputPath)
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

    var outputPath = Path.ChangeExtension(inputPath, ".sh");
    File.WriteAllText(outputPath, output);
    Console.WriteLine($"✅ Compiled: {outputPath}");
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
  Console.WriteLine("  run <file.shx>        Compile and run a .shx file.");
  Console.WriteLine("  compile <file.shx>    Compile a .shx file to a .sh file.");
  Console.WriteLine("  lsp                   Run the language server.");
}
