public abstract class Node { }

public class ProgramNode : Node
{
  public List<Node> Statements { get; } = new();
}

public class VariableDeclaration : Node
{
  public string Name = string.Empty;
  public string Type = string.Empty;
  public string Value = string.Empty;
  public bool IsConst = false; // true for const, false for let
}

public class FunctionDeclaration : Node
{
  public string Name = string.Empty;
  public List<(string Name, string Type)> Parameters = new();
  public List<Node> Body = new();
}

public class FunctionCall : Node
{
  public string Name = string.Empty;
  public List<string> Arguments = new();
}

public class ConsoleLog : Node
{
  public string Message = string.Empty;
}

public class ReturnStatement : Node
{
  public string Value = string.Empty;
}

public class IfStatement : Node
{
  public string ConditionCall = string.Empty;
  public List<Node> ThenBody = new();
  public List<Node> ElseBody = new();
}

public class ExitStatement : Node
{
  public int ExitCode;
}

public abstract class StringFunction : Node
{
  public string TargetVariable = string.Empty;
  public string SourceString = string.Empty;
}

// ${#variable} in bash
public class StringLength : StringFunction
{
}

// ${variable:start:length} in bash
public class StringSlice : StringFunction
{
  public int StartIndex = 0;
  public int? EndIndex = null;
}

// ${variable/pattern/replacement} or ${variable//pattern/replacement} in bash
public class StringReplace : StringFunction
{
  public string SearchPattern = string.Empty;
  public string ReplaceWith = string.Empty;
  public bool ReplaceAll = false;
}

// ${variable^^} in bash
public class StringToUpper : StringFunction
{
}

// ${variable,,} in bash
public class StringToLower : StringFunction
{
}

// Custom function in bash
public class StringTrim : StringFunction
{
  public string TrimChars = " \t\n\r";
}

// [[ $variable == prefix* ]] in bash
public class StringStartsWith : StringFunction
{
  public string Prefix = string.Empty;
}

// [[ $variable == *suffix ]] in bash
public class StringEndsWith : StringFunction
{
  public string Suffix = string.Empty;
}

// [[ $variable == *substring* ]] in bash
public class StringContains : StringFunction
{
  public string Substring = string.Empty;
}

// IFS and read -ra in bash
public class StringSplit : StringFunction
{
  public string Delimiter = string.Empty;
  public string ResultArrayName = string.Empty;
}
