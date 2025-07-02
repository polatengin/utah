public abstract class Node { }

public class ProgramNode : Node
{
    public List<Node> Statements { get; } = new();
}

public class VariableDeclaration : Node
{
  public bool IsConst = false; // true for const, false for let
}

public class FunctionDeclaration : Node
{
    public string Name = "";
    public List<(string Name, string Type)> Parameters = new();
    public List<Node> Body = new();
}

public class FunctionCall : Node
{
    public string Name = "";
    public List<string> Arguments = new();
}

public class ConsoleLog : Node
{
    public string Message = "";
}

public class ReturnStatement : Node
{
    public string Value = "";
}

public class IfStatement : Node
{
    public string ConditionCall = "";
    public List<Node> ThenBody = new();
    public List<Node> ElseBody = new();
}

public class ExitStatement : Node
{
    public int ExitCode;
}
