using System.Text;

public class Formatter
{
  private readonly FormattingOptions _options;

  public Formatter(FormattingOptions options)
  {
    _options = options;
  }

  public string Format(string inputPath)
  {
    if (!File.Exists(inputPath))
    {
      throw new FileNotFoundException($"File not found: {inputPath}");
    }

    try
    {
      // Read and parse the Utah source file
      var input = ResolveImports(inputPath);
      var parser = new Parser(input);
      var ast = parser.Parse();

      // Format the AST back to text
      var visitor = new FormatterVisitor(_options);
      var formatted = visitor.Visit(ast);

      // Apply final formatting rules
      return ApplyFinalFormatting(formatted);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Formatting failed: {ex.Message}", ex);
    }
  }

  private string ResolveImports(string filePath)
  {
    var resolvedFiles = new HashSet<string>();
    var result = new List<string>();

    ResolveImportsRecursive(filePath, resolvedFiles, result);
    return string.Join(Environment.NewLine, result);
  }

  private void ResolveImportsRecursive(string filePath, HashSet<string> resolvedFiles, List<string> result)
  {
    var absolutePath = Path.GetFullPath(filePath);

    if (resolvedFiles.Contains(absolutePath))
    {
      return; // Avoid circular imports
    }

    resolvedFiles.Add(absolutePath);

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
        // Parse import statement
        var match = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"import\s+([""']?)([^""';]+)\1;?");
        if (match.Success)
        {
          var importPath = match.Groups[2].Value;
          var fullImportPath = Path.IsPathRooted(importPath)
              ? importPath
              : Path.Combine(baseDirectory, importPath);

          // Resolve the imported file recursively
          ResolveImportsRecursive(fullImportPath, resolvedFiles, result);
        }
      }
      else
      {
        // Add non-import lines to result
        result.Add(line);
      }
    }
  }

  private string ApplyFinalFormatting(string content)
  {
    var lines = content.Split('\n');
    var result = new List<string>();

    foreach (var line in lines)
    {
      var processedLine = line;

      // Trim trailing whitespace if enabled
      if (_options.TrimTrailingWhitespace)
      {
        processedLine = processedLine.TrimEnd();
      }

      result.Add(processedLine);
    }

    // Apply line ending style
    var lineEnding = _options.GetLineEnding();
    var finalContent = string.Join(lineEnding, result);

    // Add final newline if enabled
    if (_options.InsertFinalNewline && !finalContent.EndsWith(lineEnding))
    {
      finalContent += lineEnding;
    }

    return finalContent;
  }
}
