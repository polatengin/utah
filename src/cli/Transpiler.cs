public class Transpiler
{
    public string Transpile(ProgramNode program)
    {
        var lines = new List<string>();

        foreach (var stmt in program.Statements)
        {
            lines.AddRange(TranspileStatement(stmt));
        }

        return string.Join('\n', lines);
    }

    private List<string> TranspileStatement(Node stmt)
    {
        var lines = new List<string>();

        if (v.IsConst)
        {
          lines.Add($"readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"{v.Name}=\"{v.Value}\"");
        }

            case FunctionDeclaration f:
                lines.Add($"{f.Name}() {{");
                for (int j = 0; j < f.Parameters.Count; j++)
                {
                    var (paramName, _) = f.Parameters[j];
                    lines.Add($"  local {paramName}=\"${j + 1}\"");
                }
                foreach (var b in f.Body)
                    lines.AddRange(TranspileBlock(b));
                lines.Add("}");
                break;

            case FunctionCall c:
                var bashArgs = c.Arguments.Select(a => a.Contains("\"") ? a : $"\"${a}\"").ToList();
                lines.Add($"{c.Name} {string.Join(" ", bashArgs)}");
                break;

            case ConsoleLog log:
                lines.Add($"echo \"{log.Message}\"");
                break;

            case IfStatement ifs:
                lines.Add($"if [ \"$( {ifs.ConditionCall} )\" = \"true\" ]; then");
                foreach (var b in ifs.ThenBody)
                    lines.AddRange(TranspileBlock(b));
                lines.Add("else");
                foreach (var b in ifs.ElseBody)
                    lines.AddRange(TranspileBlock(b));
                lines.Add("fi");
                break;
        }

        return lines;
    }

    private List<string> TranspileBlock(Node stmt)
    {
        var lines = new List<string>();

        if (v.IsConst)
        {
          lines.Add($"  readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"  local {v.Name}=\"{v.Value}\"");
        }

        return lines;
    }
}
