using EditorConfig.Core;

public class FormattingOptions
{
  public string IndentStyle { get; set; } = "space";
  public int IndentSize { get; set; } = 2;
  public string EndOfLine { get; set; } = "lf";
  public bool InsertFinalNewline { get; set; } = true;
  public bool TrimTrailingWhitespace { get; set; } = true;
  public string Charset { get; set; } = "utf-8";

  // Utah-specific formatting options
  public string BraceStyle { get; set; } = "same_line"; // same_line or new_line
  public bool SpaceBeforeParen { get; set; } = true;    // if( vs if (
  public int MaxLineLength { get; set; } = 120;

  public static FormattingOptions FromEditorConfig(string filePath)
  {
    var options = new FormattingOptions();

    try
    {
      var parser = new EditorConfigParser();
      var config = parser.Parse(filePath);

      if (config.Properties.TryGetValue("indent_style", out var indentStyle))
        options.IndentStyle = indentStyle;

      if (config.Properties.TryGetValue("indent_size", out var indentSizeStr) &&
          int.TryParse(indentSizeStr, out var indentSize))
        options.IndentSize = indentSize;

      if (config.Properties.TryGetValue("end_of_line", out var endOfLine))
        options.EndOfLine = endOfLine;

      if (config.Properties.TryGetValue("insert_final_newline", out var insertFinalNewlineStr) &&
          bool.TryParse(insertFinalNewlineStr, out var insertFinalNewline))
        options.InsertFinalNewline = insertFinalNewline;

      if (config.Properties.TryGetValue("trim_trailing_whitespace", out var trimTrailingWhitespaceStr) &&
          bool.TryParse(trimTrailingWhitespaceStr, out var trimTrailingWhitespace))
        options.TrimTrailingWhitespace = trimTrailingWhitespace;

      if (config.Properties.TryGetValue("charset", out var charset))
        options.Charset = charset;

      // Utah-specific options
      if (config.Properties.TryGetValue("utah_brace_style", out var braceStyle))
        options.BraceStyle = braceStyle;

      if (config.Properties.TryGetValue("utah_space_before_paren", out var spaceBeforeParenStr) &&
          bool.TryParse(spaceBeforeParenStr, out var spaceBeforeParen))
        options.SpaceBeforeParen = spaceBeforeParen;

      if (config.Properties.TryGetValue("utah_max_line_length", out var maxLineLengthStr) &&
          int.TryParse(maxLineLengthStr, out var maxLineLength))
        options.MaxLineLength = maxLineLength;
    }
    catch (Exception)
    {
      // If EditorConfig parsing fails, use defaults
    }

    return options;
  }

  public string GetIndent(int level)
  {
    var singleIndent = IndentStyle == "tab" ? "\t" : new string(' ', IndentSize);
    return string.Concat(Enumerable.Repeat(singleIndent, level));
  }

  public string GetLineEnding()
  {
    return EndOfLine switch
    {
      "crlf" => "\r\n",
      "cr" => "\r",
      _ => "\n"
    };
  }
}
