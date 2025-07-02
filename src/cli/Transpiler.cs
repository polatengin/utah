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

    switch (stmt)
    {
      case VariableDeclaration v:
        if (v.IsConst)
        {
          lines.Add($"readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"{v.Name}=\"{v.Value}\"");
        }
        break;

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

      case StringLength sl:
        lines.Add($"{sl.TargetVariable}=\"${{#{sl.SourceString}}}\"");
        break;

      case StringSlice ss:
        if (ss.EndIndex.HasValue)
        {
          var length = ss.EndIndex.Value - ss.StartIndex;
          lines.Add($"{ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}:{length}}}\"");
        }
        else
        {
          lines.Add($"{ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}}}\"");
        }
        break;

      case StringReplace sr:
        if (sr.ReplaceAll)
        {
          lines.Add($"{sr.TargetVariable}=\"${{{sr.SourceString}//{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        else
        {
          lines.Add($"{sr.TargetVariable}=\"${{{sr.SourceString}/{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        break;

      case StringToUpper su:
        lines.Add($"{su.TargetVariable}=\"${{{su.SourceString}^^}}\"");
        break;

      case StringToLower sl:
        lines.Add($"{sl.TargetVariable}=\"${{{sl.SourceString},,}}\"");
        break;

      case StringTrim st:
        // Custom bash function for trimming
        lines.Add($"{st.TargetVariable}=$(echo \"${{{st.SourceString}}}\" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')");
        break;

      case StringStartsWith ssw:
        lines.Add($"if [[ \"${{{ssw.SourceString}}}\" == \"{ssw.Prefix}\"* ]]; then");
        lines.Add($"  {ssw.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {ssw.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringEndsWith sew:
        lines.Add($"if [[ \"${{{sew.SourceString}}}\" == *\"{sew.Suffix}\" ]]; then");
        lines.Add($"  {sew.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {sew.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringContains sc:
        lines.Add($"if [[ \"${{{sc.SourceString}}}\" == *\"{sc.Substring}\"* ]]; then");
        lines.Add($"  {sc.TargetVariable}=\"true\"");
        lines.Add("else");
        lines.Add($"  {sc.TargetVariable}=\"false\"");
        lines.Add("fi");
        break;

      case StringSplit sp:
        lines.Add($"IFS='{sp.Delimiter}' read -ra {sp.ResultArrayName} <<< \"${{{sp.SourceString}}}\"");
        break;
    }

    return lines;
  }

  private List<string> TranspileBlock(Node stmt)
  {
    var lines = new List<string>();

    switch (stmt)
    {
      case VariableDeclaration v:
        if (v.IsConst)
        {
          lines.Add($"  readonly {v.Name}=\"{v.Value}\"");
        }
        else
        {
          lines.Add($"  local {v.Name}=\"{v.Value}\"");
        }
        break;
      case ConsoleLog log:
        lines.Add($"  echo \"{log.Message}\"");
        break;
      case ReturnStatement ret:
        lines.Add($"  echo \"${ret.Value}\"");
        break;
      case ExitStatement e:
        lines.Add($"  exit {e.ExitCode}");
        break;

      // String manipulation functions inside blocks
      case StringLength sl:
        lines.Add($"  {sl.TargetVariable}=\"${{#{sl.SourceString}}}\"");
        break;

      case StringSlice ss:
        if (ss.EndIndex.HasValue)
        {
          var length = ss.EndIndex.Value - ss.StartIndex;
          lines.Add($"  {ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}:{length}}}\"");
        }
        else
        {
          lines.Add($"  {ss.TargetVariable}=\"${{{ss.SourceString}:{ss.StartIndex}}}\"");
        }
        break;

      case StringReplace sr:
        if (sr.ReplaceAll)
        {
          lines.Add($"  {sr.TargetVariable}=\"${{{sr.SourceString}//{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        else
        {
          lines.Add($"  {sr.TargetVariable}=\"${{{sr.SourceString}/{sr.SearchPattern}/{sr.ReplaceWith}}}\"");
        }
        break;

      case StringToUpper su:
        lines.Add($"  {su.TargetVariable}=\"${{{su.SourceString}^^}}\"");
        break;

      case StringToLower sl:
        lines.Add($"  {sl.TargetVariable}=\"${{{sl.SourceString},,}}\"");
        break;

      case StringTrim st:
        lines.Add($"  {st.TargetVariable}=$(echo \"${{{st.SourceString}}}\" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')");
        break;

      case StringStartsWith ssw:
        lines.Add($"  if [[ \"${{{ssw.SourceString}}}\" == \"{ssw.Prefix}\"* ]]; then");
        lines.Add($"    {ssw.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {ssw.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringEndsWith sew:
        lines.Add($"  if [[ \"${{{sew.SourceString}}}\" == *\"{sew.Suffix}\" ]]; then");
        lines.Add($"    {sew.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {sew.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringContains sc:
        lines.Add($"  if [[ \"${{{sc.SourceString}}}\" == *\"{sc.Substring}\"* ]]; then");
        lines.Add($"    {sc.TargetVariable}=\"true\"");
        lines.Add("  else");
        lines.Add($"    {sc.TargetVariable}=\"false\"");
        lines.Add("  fi");
        break;

      case StringSplit sp:
        lines.Add($"  IFS='{sp.Delimiter}' read -ra {sp.ResultArrayName} <<< \"${{{sp.SourceString}}}\"");
        break;
    }

    return lines;
  }
}
