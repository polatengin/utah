public partial class Compiler
{
  public string Compile(ProgramNode program)
  {
    var lines = new List<string>
    {
      "#!/bin/sh",
      "" // Empty line after shebang
    };

    foreach (var stmt in program.Statements)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    return string.Join('\n', lines) + '\n';
  }
}
