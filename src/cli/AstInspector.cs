using System.Collections;
using System.Reflection;

internal static class AstInspector
{
  public static bool ContainsArgsUsage(ProgramNode program)
  {
    return ContainsArgsUsage((object?)program);
  }

  private static bool ContainsArgsUsage(object? value)
  {
    switch (value)
    {
      case null:
        return false;
      case ArgsDefineStatement:
      case ArgsShowHelpStatement:
      case ArgsHasExpression:
      case ArgsGetExpression:
      case ArgsAllExpression:
        return true;
      case string:
        return false;
      case IEnumerable enumerable:
        foreach (var item in enumerable)
        {
          if (ContainsArgsUsage(item))
          {
            return true;
          }
        }

        return false;
      case Node node:
        foreach (var property in node.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
          if (ContainsArgsUsage(property.GetValue(node)))
          {
            return true;
          }
        }

        return false;
      default:
        return false;
    }
  }
}
