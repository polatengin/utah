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
  public string Name { get; set; } = string.Empty;
  public List<(string Name, string Type)> Parameters = new();
  public List<Node> Body = new();
}

public class FunctionCall : Expression
{
  public string Name = string.Empty;
  public List<string> Arguments = new();
}

public class ConsoleLog : Node
{
  public string Message = string.Empty;
  public bool IsExpression;
  public Expression? Expression;
}

public class ConsoleIsSudoExpression : Expression { } // Represents console.isSudo()

public class ConsolePromptYesNoExpression : Expression
{
  public string PromptText = string.Empty; // The text to display to the user
}

public class UtilityRandomExpression : Expression
{
  public Expression? MinValue = null; // Optional minimum value
  public Expression? MaxValue = null; // Optional maximum value
}

public class ReturnStatement : Node
{
  public Expression? Value;
}

public class IfStatement : Node
{
  public Expression Condition { get; set; } = null!;
  public List<Node> ThenBody = new();
  public List<Node> ElseBody = new();
}

public class ExitStatement : Node
{
  public int ExitCode;
}

// Array-related nodes
public class ArrayLiteral : Expression
{
  public List<Expression> Elements = new();
  public string ElementType = string.Empty; // "string", "number", "boolean"
}

public class ArrayAccess : Expression
{
  public Expression Array = null!;
  public Expression Index = null!;
}

public class ArrayLength : Expression
{
  public Expression Array = null!;
}

// For loops
public class ForLoop : Node
{
  public string InitVariable = string.Empty;
  public string InitType = string.Empty;
  public Expression InitValue = null!;
  public Expression Condition = null!;
  public string UpdateVariable = string.Empty;
  public string UpdateOperator = string.Empty; // "++", "--", "+=", "-="
  public Expression? UpdateValue = null; // For += and -= operations
  public List<Node> Body = new();
}

public class ForInLoop : Node
{
  public string Variable { get; set; } = string.Empty;
  public string VariableType { get; set; } = string.Empty;
  public string Iterable { get; set; } = string.Empty; // Variable name or array
  public List<Node> Body { get; set; } = new List<Node>();
}

public class WhileStatement : Node
{
  public Expression Condition { get; set; } = null!;
  public List<Node> Body { get; set; } = new List<Node>();
}

public class BreakStatement : Node { }

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

// Environment variable operations
public class EnvGet : Node
{
  public string VariableName = string.Empty;
  public string DefaultValue = string.Empty;
  public string AssignTo = string.Empty; // Target variable name
}

public class EnvSet : Node
{
  public string VariableName = string.Empty;
  public string Value = string.Empty;
}

public class EnvLoad : Node
{
  public string FilePath = string.Empty;
}

public class EnvDelete : Node
{
  public string VariableName = string.Empty;
}

public class OsIsInstalled : Node
{
  public string AppName = string.Empty;
  public string AssignTo = string.Empty; // Target variable name
}

public class OsGetOS : Node
{
  public string AssignTo = string.Empty; // Target variable name
}

public class OsGetLinuxVersion : Node
{
  public string AssignTo = string.Empty; // Target variable name
}

// File system operations
public class FsReadFile : Node
{
  public string FilePath = string.Empty;
  public string AssignTo = string.Empty; // Target variable name for the file content
}

public class FsWriteFile : Node
{
  public string FilePath = string.Empty;
  public string Content = string.Empty;
}

public class FsDirname : Node
{
  public string FilePath = string.Empty;
  public string AssignTo = string.Empty; // Target variable name for the directory path
}

public class FsParentDirName : Node
{
  public string FilePath = string.Empty;
  public string AssignTo = string.Empty; // Target variable name for the parent directory name
}

public class FsExtension : Node
{
  public string FilePath = string.Empty;
  public string AssignTo = string.Empty; // Target variable name for the file extension
}

public class FsFileName : Node
{
  public string FilePath = string.Empty;
  public string AssignTo = string.Empty; // Target variable name for the file name
}

public class TimerStart : Node
{
  // No properties needed - just starts the timer
}

public class TimerStop : Node
{
  public string AssignTo = string.Empty; // Target variable name for the elapsed time in milliseconds
}

// Process information operations
public class ProcessId : Node
{
  public string AssignTo = string.Empty; // Target variable name for the process ID
}

public class ProcessCpu : Node
{
  public string AssignTo = string.Empty; // Target variable name for the CPU usage percentage
}

public class ProcessMemory : Node
{
  public string AssignTo = string.Empty; // Target variable name for the memory usage percentage
}

public class ProcessElapsedTime : Node
{
  public string AssignTo = string.Empty; // Target variable name for the elapsed time
}

public class ProcessCommand : Node
{
  public string AssignTo = string.Empty; // Target variable name for the command line
}

public class ProcessStatus : Node
{
  public string AssignTo = string.Empty; // Target variable name for the process status
}

// Expression nodes for complex expressions and operators
public abstract class Expression : Node { }

public class LiteralExpression : Expression
{
  public string Value = string.Empty;
  public string Type = string.Empty; // "number", "string", "boolean"
}

public class VariableExpression : Expression
{
  public string Name = string.Empty;
}

public class BinaryExpression : Expression
{
  public Expression Left = null!;
  public Expression Right = null!;
  public string Operator = string.Empty; // +, -, *, /, %, ==, !=, <, >, <=, >=, &&, ||
}

public class UnaryExpression : Expression
{
  public Expression Operand = null!;
  public string Operator = string.Empty; // !, -
}

public class TernaryExpression : Expression
{
  public Expression Condition = null!;
  public Expression TrueExpression = null!;
  public Expression FalseExpression = null!;
}

public class ParenthesizedExpression : Expression
{
  public Expression Inner = null!;
}

// Switch/Case/Default nodes
public class SwitchStatement : Node
{
  public Expression SwitchExpression = null!;
  public List<CaseClause> Cases = new();
  public DefaultClause? DefaultCase = null;
}

public class CaseClause : Node
{
  public List<Expression> Values = new(); // Support multiple case values for fall-through
  public List<Node> Body = new();
  public bool HasBreak = false;
}

public class DefaultClause : Node
{
  public List<Node> Body = new();
  public bool HasBreak = false;
}

// Assignment statement (variable = value)
public class AssignmentStatement : Node
{
  public string VariableName = string.Empty;
  public Expression Value = null!;
}

// Updated VariableDeclaration to support expressions
public class VariableDeclarationExpression : Node
{
  public string Name = string.Empty;
  public string Type = string.Empty;
  public Expression Value = null!;
  public bool IsConst = false; // true for const, false for let
}

public class RawStatement : Node
{
  public string Content { get; set; } = string.Empty;
}
