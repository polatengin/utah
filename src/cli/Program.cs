if (args.Length != 1 || !args[0].EndsWith(".shx"))
{
  Console.WriteLine("Usage: typedbashc <file.shx>");
  return;
}

var inputPath = args[0];
if (!File.Exists(inputPath))
{
  Console.WriteLine($"File not found: {inputPath}");
  return;
}

var input = File.ReadAllText(inputPath);
var parser = new Parser(input);
var ast = parser.Parse();
var transpiler = new Transpiler();
var output = transpiler.Transpile(ast);

var outputPath = Path.ChangeExtension(inputPath, ".sh");
File.WriteAllText(outputPath, output);
Console.WriteLine($"✅ Transpiled: {outputPath}");
