if (args.Length != 1 || !args[0].EndsWith(".shx"))
{
  Console.WriteLine("Usage: typedbashc <file.shx>");
  return;
}

string inputPath = args[0];
if (!File.Exists(inputPath))
{
  Console.WriteLine($"File not found: {inputPath}");
  return;
}

string input = File.ReadAllText(inputPath);
var parser = new Parser(input);
var ast = parser.Parse();
var transpiler = new Transpiler();
string output = transpiler.Transpile(ast);

string outputPath = Path.ChangeExtension(inputPath, ".sh");
File.WriteAllText(outputPath, output);
Console.WriteLine($"✅ Transpiled: {outputPath}");
