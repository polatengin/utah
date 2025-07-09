using System.Text.RegularExpressions;
using System.Text;

public partial class Parser
{
  private StringFunction? ParseStringFunction(string targetVariable, string expression)
  {
    // Parse string.length() -> StringLength
    var lengthMatch = Regex.Match(expression, @"(\w+)\.length\(\)");
    if (lengthMatch.Success)
    {
      return new StringLength
      {
        TargetVariable = targetVariable,
        SourceString = lengthMatch.Groups[1].Value
      };
    }

    // Parse string.slice(start, end?) -> StringSlice
    var sliceMatch = Regex.Match(expression, @"(\w+)\.slice\((\d+)(?:,\s*(\d+))?\)");
    if (sliceMatch.Success)
    {
      var endIndex = sliceMatch.Groups[3].Success ? int.Parse(sliceMatch.Groups[3].Value) : (int?)null;
      return new StringSlice
      {
        TargetVariable = targetVariable,
        SourceString = sliceMatch.Groups[1].Value,
        StartIndex = int.Parse(sliceMatch.Groups[2].Value),
        EndIndex = endIndex
      };
    }

    // Parse string.replace(search, replace) -> StringReplace
    var replaceMatch = Regex.Match(expression, @"(\w+)\.replace\(""([^""]+)"",\s*""([^""]*)""\)");
    if (replaceMatch.Success)
    {
      return new StringReplace
      {
        TargetVariable = targetVariable,
        SourceString = replaceMatch.Groups[1].Value,
        SearchPattern = replaceMatch.Groups[2].Value,
        ReplaceWith = replaceMatch.Groups[3].Value,
        ReplaceAll = false
      };
    }

    // Parse string.replaceAll(search, replace) -> StringReplace
    var replaceAllMatch = Regex.Match(expression, @"(\w+)\.replaceAll\(""([^""]+)"",\s*""([^""]*)""\)");
    if (replaceAllMatch.Success)
    {
      return new StringReplace
      {
        TargetVariable = targetVariable,
        SourceString = replaceAllMatch.Groups[1].Value,
        SearchPattern = replaceAllMatch.Groups[2].Value,
        ReplaceWith = replaceAllMatch.Groups[3].Value,
        ReplaceAll = true
      };
    }

    // Parse string.toUpperCase() -> StringToUpper
    var upperMatch = Regex.Match(expression, @"(\w+)\.toUpperCase\(\)");
    if (upperMatch.Success)
    {
      return new StringToUpper
      {
        TargetVariable = targetVariable,
        SourceString = upperMatch.Groups[1].Value
      };
    }

    // Parse string.toLowerCase() -> StringToLower
    var lowerMatch = Regex.Match(expression, @"(\w+)\.toLowerCase\(\)");
    if (lowerMatch.Success)
    {
      return new StringToLower
      {
        TargetVariable = targetVariable,
        SourceString = lowerMatch.Groups[1].Value
      };
    }

    // Parse string.trim() -> StringTrim
    var trimMatch = Regex.Match(expression, @"(\w+)\.trim\(\)");
    if (trimMatch.Success)
    {
      return new StringTrim
      {
        TargetVariable = targetVariable,
        SourceString = trimMatch.Groups[1].Value
      };
    }

    // Parse string.startsWith("prefix") -> StringStartsWith
    var startsWithMatch = Regex.Match(expression, @"(\w+)\.startsWith\(""([^""]+)""\)");
    if (startsWithMatch.Success)
    {
      return new StringStartsWith
      {
        TargetVariable = targetVariable,
        SourceString = startsWithMatch.Groups[1].Value,
        Prefix = startsWithMatch.Groups[2].Value
      };
    }

    // Parse string.endsWith("suffix") -> StringEndsWith
    var endsWithMatch = Regex.Match(expression, @"(\w+)\.endsWith\(""([^""]+)""\)");
    if (endsWithMatch.Success)
    {
      return new StringEndsWith
      {
        TargetVariable = targetVariable,
        SourceString = endsWithMatch.Groups[1].Value,
        Suffix = endsWithMatch.Groups[2].Value
      };
    }

    // Parse string.includes("substring") -> StringContains
    var includesMatch = Regex.Match(expression, @"(\w+)\.includes\(""([^""]+)""\)");
    if (includesMatch.Success)
    {
      return new StringContains
      {
        TargetVariable = targetVariable,
        SourceString = includesMatch.Groups[1].Value,
        Substring = includesMatch.Groups[2].Value
      };
    }

    // Parse string.split("delimiter") -> StringSplit
    var splitMatch = Regex.Match(expression, @"(\w+)\.split\(""([^""]*)""\)");
    if (splitMatch.Success)
    {
      return new StringSplit
      {
        TargetVariable = targetVariable,
        SourceString = splitMatch.Groups[1].Value,
        Delimiter = splitMatch.Groups[2].Value,
        ResultArrayName = targetVariable
      };
    }

    // Parse string literal.split("delimiter") -> StringSplit
    var literalSplitMatch = Regex.Match(expression, @"""([^""]*)""\.split\(""([^""]*)""\)");
    if (literalSplitMatch.Success)
    {
      return new StringSplit
      {
        TargetVariable = targetVariable,
        SourceString = literalSplitMatch.Groups[1].Value,
        Delimiter = literalSplitMatch.Groups[2].Value,
        ResultArrayName = targetVariable
      };
    }

    return null;
  }

  private Node? ParseEnvFunction(string targetVariable, string expression)
  {
    // Parse env.get("VAR_NAME", "default") -> EnvGet
    var envGetMatch = Regex.Match(expression, @"env\.get\(""([^""]+)"",\s*""([^""]*)""\)");
    if (envGetMatch.Success)
    {
      return new EnvGet
      {
        AssignTo = targetVariable,
        VariableName = envGetMatch.Groups[1].Value,
        DefaultValue = envGetMatch.Groups[2].Value
      };
    }

    // Parse boolean expressions like env.get("DEBUG", "false") === "true"
    var boolEnvMatch = Regex.Match(expression, @"env\.get\(""([^""]+)"",\s*""([^""]*)""\)\s*===\s*""([^""]*)""");
    if (boolEnvMatch.Success)
    {
      // For boolean expressions, we'll create a special variable assignment
      return new VariableDeclaration
      {
        Name = targetVariable,
        Type = "boolean",
        Value = $"[ \"${{{boolEnvMatch.Groups[1].Value}:-{boolEnvMatch.Groups[2].Value}}}\" = \"{boolEnvMatch.Groups[3].Value}\" ]",
        IsConst = false
      };
    }

    return null;
  }

  private Node? ParseStandaloneEnvFunction(string line)
  {
    // Parse env.set("VAR_NAME", "value");
    var envSetMatch = Regex.Match(line, @"env\.set\(""([^""]+)"",\s*""([^""]*)""\);");
    if (envSetMatch.Success)
    {
      return new EnvSet
      {
        VariableName = envSetMatch.Groups[1].Value,
        Value = envSetMatch.Groups[2].Value
      };
    }

    // Parse env.load("filepath");
    var envLoadMatch = Regex.Match(line, @"env\.load\(""([^""]+)""\);");
    if (envLoadMatch.Success)
    {
      return new EnvLoad
      {
        FilePath = envLoadMatch.Groups[1].Value
      };
    }

    // Parse env.delete("VAR_NAME");
    var envDeleteMatch = Regex.Match(line, @"env\.delete\(""([^""]+)""\);");
    if (envDeleteMatch.Success)
    {
      return new EnvDelete
      {
        VariableName = envDeleteMatch.Groups[1].Value
      };
    }

    return null;
  }

  private Node? ParseOsFunction(string targetVariable, string expression)
  {
    // Parse os.isInstalled("app_name") -> OsIsInstalled
    var osIsInstalledMatch = Regex.Match(expression, @"os\.isInstalled\(""([^""]+)""\)");
    if (osIsInstalledMatch.Success)
    {
      return new OsIsInstalled
      {
        AppName = osIsInstalledMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse os.getOS() -> OsGetOS
    if (expression.Trim() == "os.getOS()")
    {
      return new OsGetOS { AssignTo = targetVariable };
    }

    // Parse os.getLinuxVersion() -> OsGetLinuxVersion
    if (expression.Trim() == "os.getLinuxVersion()")
    {
      return new OsGetLinuxVersion { AssignTo = targetVariable };
    }

    return null;
  }

  private Node? ParseStandaloneFsFunction(string line)
  {
    // Parse fs.writeFile("filepath", "content"); (string literal)
    var fsWriteStringMatch = Regex.Match(line, @"fs\.writeFile\(""([^""]+)"",\s*""([^""]*)""\);");
    if (fsWriteStringMatch.Success)
    {
      return new FsWriteFile
      {
        FilePath = fsWriteStringMatch.Groups[1].Value,
        Content = fsWriteStringMatch.Groups[2].Value
      };
    }

    // Parse fs.writeFile("filepath", variableName); (variable)
    var fsWriteVarMatch = Regex.Match(line, @"fs\.writeFile\(""([^""]+)"",\s*(\w+)\);");
    if (fsWriteVarMatch.Success)
    {
      return new FsWriteFile
      {
        FilePath = fsWriteVarMatch.Groups[1].Value,
        Content = $"${{{fsWriteVarMatch.Groups[2].Value}}}" // Mark as variable reference
      };
    }

    return null;
  }

  private Node? ParseFsFunction(string targetVariable, string expression)
  {
    // Parse fs.readFile("filepath") -> FsReadFile
    var fsReadMatch = Regex.Match(expression, @"fs\.readFile\(""([^""]+)""\)");
    if (fsReadMatch.Success)
    {
      return new FsReadFile
      {
        FilePath = fsReadMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.dirname("filepath") -> FsDirname
    var fsDirnameMatch = Regex.Match(expression, @"fs\.dirname\(""([^""]+)""\)");
    if (fsDirnameMatch.Success)
    {
      return new FsDirname
      {
        FilePath = fsDirnameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.parentDirName("filepath") -> FsParentDirName
    var fsParentDirNameMatch = Regex.Match(expression, @"fs\.parentDirName\(""([^""]+)""\)");
    if (fsParentDirNameMatch.Success)
    {
      return new FsParentDirName
      {
        FilePath = fsParentDirNameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.extension("filepath") -> FsExtension
    var fsExtensionMatch = Regex.Match(expression, @"fs\.extension\(""([^""]+)""\)");
    if (fsExtensionMatch.Success)
    {
      return new FsExtension
      {
        FilePath = fsExtensionMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    // Parse fs.fileName("filepath") -> FsFileName
    var fsFileNameMatch = Regex.Match(expression, @"fs\.fileName\(""([^""]+)""\)");
    if (fsFileNameMatch.Success)
    {
      return new FsFileName
      {
        FilePath = fsFileNameMatch.Groups[1].Value,
        AssignTo = targetVariable
      };
    }

    return null;
  }

  // Timer function parsing methods
  private Node? ParseStandaloneTimerFunction(string line)
  {
    line = line.Trim();

    if (line == "timer.start();")
    {
      return new TimerStart();
    }

    return null;
  }

  private Node? ParseTimerFunction(string assignTo, string value)
  {
    value = value.Trim();

    // Parse timer.stop() -> TimerStop
    if (value == "timer.stop()")
    {
      return new TimerStop { AssignTo = assignTo };
    }

    return null;
  }

  private Node? ParseProcessFunction(string assignTo, string value)
  {
    value = value.Trim();

    if (value == "process.id()")
    {
      return new ProcessId { AssignTo = assignTo };
    }

    if (value == "process.cpu()")
    {
      return new ProcessCpu { AssignTo = assignTo };
    }

    if (value == "process.memory()")
    {
      return new ProcessMemory { AssignTo = assignTo };
    }

    if (value == "process.elapsedTime()")
    {
      return new ProcessElapsedTime { AssignTo = assignTo };
    }

    if (value == "process.command()")
    {
      return new ProcessCommand { AssignTo = assignTo };
    }

    if (value == "process.status()")
    {
      return new ProcessStatus { AssignTo = assignTo };
    }

    return null;
  }
}
