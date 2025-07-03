using OmniSharp.Extensions.LanguageServer.Server;
using System.Threading.Tasks;

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

static void PrintUsage()
{
  Console.WriteLine("Usage: utah <command>");
  Console.WriteLine("Commands:");
  Console.WriteLine("  compile <file.shx>  Compile a .shx file to a .sh file.");
  Console.WriteLine("  lsp                 Run the language server.");
}
