using System.Text.RegularExpressions;

public abstract record UtahType
{
  public static readonly UtahType String = new StringType();
  public static readonly UtahType Number = new NumberType();
  public static readonly UtahType Boolean = new BooleanType();
  public static readonly UtahType Object = new ObjectType();
  public static readonly UtahType Any = new AnyType();
  public static readonly UtahType Unknown = new UnknownType();
  public static readonly UtahType Void = new VoidType();
  public static readonly UtahType SshConnection = new SshConnectionType();

  public static UtahType? Parse(string? typeStr)
  {
    if (string.IsNullOrWhiteSpace(typeStr))
    {
      return null;
    }

    typeStr = Regex.Replace(typeStr.Trim(), @"\s+", "");

    if (typeStr.EndsWith("[]"))
    {
      var elementType = Parse(typeStr[..^2]) ?? Unknown;
      return new ArrayType(elementType);
    }

    if (typeStr.StartsWith("set<") && typeStr.EndsWith(">"))
    {
      var elementType = Parse(typeStr[4..^1]) ?? Unknown;
      return new SetType(elementType);
    }

    if (typeStr.StartsWith("map<") && typeStr.EndsWith(">"))
    {
      var content = typeStr[4..^1];
      var parts = SplitGenericParams(content);
      if (parts.Count == 2)
      {
        return new MapType(Parse(parts[0]) ?? Unknown, Parse(parts[1]) ?? Unknown);
      }

      return new MapType(Unknown, Unknown);
    }

    if (typeStr.StartsWith("dictionary<") && typeStr.EndsWith(">"))
    {
      var content = typeStr[11..^1];
      var parts = SplitGenericParams(content);
      if (parts.Count == 2)
      {
        return new DictionaryType(Parse(parts[0]) ?? Unknown, Parse(parts[1]) ?? Unknown);
      }

      return new DictionaryType(Unknown, Unknown);
    }

    return typeStr switch
    {
      "string" => String,
      "number" => Number,
      "boolean" => Boolean,
      "object" => Object,
      "any" => Any,
      "unknown" => Unknown,
      "void" => Void,
      "sshConnection" => SshConnection,
      _ => new StructuredType(typeStr)
    };
  }

  private static List<string> SplitGenericParams(string content)
  {
    var parts = new List<string>();
    var depth = 0;
    var start = 0;
    for (int i = 0; i < content.Length; i++)
    {
      if (content[i] == '<') depth++;
      else if (content[i] == '>') depth--;
      else if (content[i] == ',' && depth == 0)
      {
        parts.Add(content[start..i].Trim());
        start = i + 1;
      }
    }

    parts.Add(content[start..].Trim());
    return parts;
  }
}

public sealed record StringType() : UtahType
{
  public override string ToString() => "string";
}

public sealed record NumberType() : UtahType
{
  public override string ToString() => "number";
}

public sealed record BooleanType() : UtahType
{
  public override string ToString() => "boolean";
}

public sealed record ObjectType() : UtahType
{
  public override string ToString() => "object";
}

public sealed record AnyType() : UtahType
{
  public override string ToString() => "any";
}

public sealed record UnknownType() : UtahType
{
  public override string ToString() => "unknown";
}

public sealed record VoidType() : UtahType
{
  public override string ToString() => "void";
}

public sealed record SshConnectionType() : UtahType
{
  public override string ToString() => "sshConnection";
}

public sealed record ArrayType(UtahType ElementType) : UtahType
{
  public override string ToString() => $"{ElementType}[]";
}

public sealed record SetType(UtahType ElementType) : UtahType
{
  public override string ToString() => $"set<{ElementType}>";
}

public sealed record MapType(UtahType KeyType, UtahType ValueType) : UtahType
{
  public override string ToString() => $"map<{KeyType},{ValueType}>";
}

public sealed record DictionaryType(UtahType KeyType, UtahType ValueType) : UtahType
{
  public override string ToString() => $"dictionary<{KeyType},{ValueType}>";
}

public sealed record StructuredType(string Name) : UtahType
{
  public override string ToString() => Name;
}
