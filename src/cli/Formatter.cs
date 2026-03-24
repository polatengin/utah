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
      var originalContent = File.ReadAllText(inputPath);
      return FormatWithComments(originalContent);
    }
    catch (InvalidOperationException)
    {
      throw;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Unable to format '{inputPath}': {ex.Message}", ex);
    }
  }

  public string FormatContent(string content)
  {
    return FormatWithComments(content);
  }

  private string FormatWithComments(string content)
  {
    var lines = content.Split('\n');
    var comments = new List<(int lineIndex, string content)>();
    var nonCommentContent = new StringBuilder();

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];
      var trimmed = line.Trim();

      if (trimmed.StartsWith("//"))
      {
        comments.Add((i, line));
      }
      else if (!string.IsNullOrEmpty(trimmed))
      {
        nonCommentContent.AppendLine(line);
      }
      else
      {
        nonCommentContent.AppendLine();
      }
    }

    string formattedContent;
    try
    {
      var parser = new Parser(nonCommentContent.ToString());
      var ast = parser.Parse();
      var visitor = new FormatterVisitor(_options);
      formattedContent = visitor.Visit(ast).Trim();
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Input could not be parsed for formatting.", ex);
    }

    var formattedLines = formattedContent.Split('\n').ToList();
    var result = new List<string>();

    int commentIndex = 0;
    int totalOriginalLines = lines.Length;
    int totalFormattedLines = formattedLines.Count;

    for (int i = 0; i < formattedLines.Count; i++)
    {
      while (commentIndex < comments.Count)
      {
        var (originalLineIndex, commentContent) = comments[commentIndex];
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

      result.Add(formattedLines[i]);
    }

    while (commentIndex < comments.Count)
    {
      var (_, commentContent) = comments[commentIndex];
      result.Add(commentContent);
      commentIndex++;
    }

    var formattedResult = string.Join("\n", result);
    return ApplyFinalFormatting(formattedResult);
  }

  private string ApplyFinalFormatting(string content)
  {
    var lines = content.Split('\n');
    var result = new List<string>();

    foreach (var line in lines)
    {
      var processedLine = line;

      if (_options.TrimTrailingWhitespace)
      {
        processedLine = processedLine.TrimEnd();
      }

      result.Add(processedLine);
    }

    var lineEnding = _options.GetLineEnding();
    var finalContent = string.Join(lineEnding, result);

    if (_options.InsertFinalNewline && !finalContent.EndsWith(lineEnding))
    {
      finalContent += lineEnding;
    }

    return finalContent;
  }
}
