public partial class Parser
{
  private void ValidateArrayElementTypes(ArrayLiteral arrayLiteral, string expectedElementType, string variableName)
  {
    for (int i = 0; i < arrayLiteral.Elements.Count; i++)
    {
      var element = arrayLiteral.Elements[i];
      string actualType = GetExpressionType(element);

      if (actualType != expectedElementType)
      {
        throw new InvalidOperationException($"Error: Array element at index {i} in variable '{variableName}' has type '{actualType}' but expected '{expectedElementType}'. All elements in a {expectedElementType}[] array must be of type {expectedElementType}.");
      }
    }
  }

  private string GetExpressionType(Expression expression)
  {
    return expression switch
    {
      LiteralExpression literal => literal.Type,
      VariableExpression _ => "variable", // We could track variable types, but for now assume it's correct
      BinaryExpression _ => "expression", // Could infer type from operation
      _ => "unknown"
    };
  }
}
