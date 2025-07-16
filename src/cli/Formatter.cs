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
      // Read the original source with comments preserved
      var originalContent = File.ReadAllText(inputPath);
      
      // Format with comment preservation
      return FormatWithComments(originalContent);
    }
    catch (Exception ex)
    {
      throw new Exception($"Formatting failed: {ex.Message}", ex);
    }
  }
  
  private string FormatWithComments(string content)
  {
    // Parse the content to identify comments and their positions
    var lines = content.Split('\n');
    var comments = new List<(int lineIndex, string content)>();
    var nonCommentContent = new StringBuilder();
    
    // Extract comments and build non-comment content
    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i].TrimEnd();
      var trimmed = line.Trim();
      
      if (trimmed.StartsWith("//"))
      {
        // Store comment with its relative position
        comments.Add((i, line));
      }
      else if (!string.IsNullOrEmpty(trimmed))
      {
        // Add non-comment line to content for AST processing
        nonCommentContent.AppendLine(line);
      }
      else
      {
        // Handle empty lines - they'll be managed by the formatter
        nonCommentContent.AppendLine();
      }
    }
    
    // Format the non-comment content using AST
    string formattedContent;
    try
    {
      var parser = new Parser(nonCommentContent.ToString());
      var ast = parser.Parse();
      var visitor = new FormatterVisitor(_options);
      formattedContent = visitor.Visit(ast).Trim();
    }
    catch
    {
      // If formatting fails, return original content
      return content;
    }
    
    // Reinsert comments at appropriate positions
    var formattedLines = formattedContent.Split('\n').ToList();
    var result = new List<string>();
    
    // Calculate relative positions for comments based on original structure
    int commentIndex = 0;
    int totalOriginalLines = lines.Length;
    int totalFormattedLines = formattedLines.Count;
    
    for (int i = 0; i < formattedLines.Count; i++)
    {
      // Check if we should insert any comments before this line
      while (commentIndex < comments.Count)
      {
        var (originalLineIndex, commentContent) = comments[commentIndex];
        
        // Calculate the relative position of this comment in the formatted output
        double relativePosition = (double)originalLineIndex / totalOriginalLines;
        int targetFormattedLine = (int)(relativePosition * totalFormattedLines);
        
        if (targetFormattedLine <= i)
        {
          result.Add(commentContent);
          commentIndex++;
        }
        else
        {
          break;
        }
      }
      
      // Add the formatted line
      result.Add(formattedLines[i]);
    }
    
    // Add any remaining comments at the end
    while (commentIndex < comments.Count)
    {
      var (_, commentContent) = comments[commentIndex];
      result.Add(commentContent);
      commentIndex++;
    }
    
    // Join the result and ensure it ends with a newline
    var formattedResult = string.Join("\n", result);
    if (!formattedResult.EndsWith("\n"))
    {
      formattedResult += "\n";
    }
    
    return formattedResult;
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
