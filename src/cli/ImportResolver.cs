using System.Text.RegularExpressions;

internal static class ImportResolver
{
  private static readonly Regex s_importPattern = new(@"import\s+([""']?)([^""';]+)\1;?", RegexOptions.Compiled);

  public static string ResolveImports(string filePath)
  {
    var resolvedFiles = new HashSet<string>(StringComparer.Ordinal);
    var result = new List<string>();

    ResolveImportsRecursive(filePath, resolvedFiles, result);
    return string.Join(Environment.NewLine, result);
  }

  private static void ResolveImportsRecursive(string filePath, HashSet<string> resolvedFiles, List<string> result)
  {
    var absolutePath = Path.GetFullPath(filePath);
    if (!resolvedFiles.Add(absolutePath))
    {
      return;
    }

    if (!File.Exists(absolutePath))
    {
      throw new InvalidOperationException($"Import file not found: {filePath}");
    }

    var content = File.ReadAllText(absolutePath);
    var lines = content.Split('\n');
    var baseDirectory = Path.GetDirectoryName(absolutePath) ?? "";

    foreach (var line in lines)
    {
      var trimmedLine = line.Trim();
      if (trimmedLine.StartsWith("import "))
      {
        var match = s_importPattern.Match(trimmedLine);
        if (match.Success)
        {
          var importPath = match.Groups[2].Value;
          var fullImportPath = Path.IsPathRooted(importPath)
            ? importPath
            : Path.Combine(baseDirectory, importPath);

          ResolveImportsRecursive(fullImportPath, resolvedFiles, result);
        }

        continue;
      }

      result.Add(line);
    }
  }
}
