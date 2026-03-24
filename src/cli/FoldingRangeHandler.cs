using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class FoldingRangeHandler : IFoldingRangeHandler
{
  public FoldingRangeRegistrationOptions GetRegistrationOptions(FoldingRangeCapability capability, ClientCapabilities clientCapabilities)
  {
    return new FoldingRangeRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      })
    };
  }

  public async Task<Container<FoldingRange>?> Handle(FoldingRangeRequestParam request, CancellationToken cancellationToken)
  {
    try
    {
      var uri = request.TextDocument.Uri;
      var text = DocumentManager.Instance.Get(uri.ToString());
      if (text == null)
      {
        var path = uri.GetFileSystemPath();
        if (File.Exists(path))
          text = await File.ReadAllTextAsync(path, cancellationToken);
        else
          return null;
      }

      var ranges = ExtractFoldingRanges(text);
      return new Container<FoldingRange>(ranges);
    }
    catch
    {
      return null;
    }
  }

  private static List<FoldingRange> ExtractFoldingRanges(string text)
  {
    var ranges = new List<FoldingRange>();
    var lines = text.Split('\n');

    // Brace-based folding for code blocks
    AddBraceRanges(lines, ranges);

    // Comment block folding (consecutive single-line comments)
    AddCommentBlockRanges(lines, ranges);

    // Multiline string folding
    AddMultilineStringRanges(lines, ranges);

    return ranges;
  }

  private static void AddBraceRanges(string[] lines, List<FoldingRange> ranges)
  {
    var braceStack = new Stack<int>();

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];
      bool inSingleQuote = false;
      bool inDoubleQuote = false;
      bool inBacktick = false;

      for (int j = 0; j < line.Length; j++)
      {
        var c = line[j];

        // Skip escaped characters
        if (c == '\\' && j + 1 < line.Length)
        {
          j++;
          continue;
        }

        // Track string state
        if (c == '\'' && !inDoubleQuote && !inBacktick) inSingleQuote = !inSingleQuote;
        else if (c == '"' && !inSingleQuote && !inBacktick) inDoubleQuote = !inDoubleQuote;
        else if (c == '`' && !inSingleQuote && !inDoubleQuote) inBacktick = !inBacktick;

        if (inSingleQuote || inDoubleQuote || inBacktick)
          continue;

        // Skip line comments
        if (c == '/' && j + 1 < line.Length && line[j + 1] == '/')
          break;

        if (c == '{')
        {
          braceStack.Push(i);
        }
        else if (c == '}' && braceStack.Count > 0)
        {
          var startLine = braceStack.Pop();
          if (i > startLine)
          {
            ranges.Add(new FoldingRange
            {
              StartLine = startLine,
              StartCharacter = lines[startLine].IndexOf('{'),
              EndLine = i,
              EndCharacter = line.IndexOf('}'),
              Kind = FoldingRangeKind.Region
            });
          }
        }
      }
    }
  }

  private static void AddCommentBlockRanges(string[] lines, List<FoldingRange> ranges)
  {
    int commentStart = -1;

    for (int i = 0; i <= lines.Length; i++)
    {
      bool isComment = i < lines.Length && lines[i].TrimStart().StartsWith("//");

      if (isComment && commentStart < 0)
      {
        commentStart = i;
      }
      else if (!isComment && commentStart >= 0)
      {
        var commentEnd = i - 1;
        if (commentEnd > commentStart)
        {
          ranges.Add(new FoldingRange
          {
            StartLine = commentStart,
            EndLine = commentEnd,
            Kind = FoldingRangeKind.Comment
          });
        }
        commentStart = -1;
      }
    }
  }

  private static void AddMultilineStringRanges(string[] lines, List<FoldingRange> ranges)
  {
    bool inBacktick = false;
    int backtickStart = -1;

    for (int i = 0; i < lines.Length; i++)
    {
      var line = lines[i];
      for (int j = 0; j < line.Length; j++)
      {
        if (line[j] == '\\' && j + 1 < line.Length)
        {
          j++;
          continue;
        }

        if (line[j] == '`')
        {
          if (!inBacktick)
          {
            inBacktick = true;
            backtickStart = i;
          }
          else
          {
            if (i > backtickStart)
            {
              ranges.Add(new FoldingRange
              {
                StartLine = backtickStart,
                EndLine = i,
                Kind = FoldingRangeKind.Region
              });
            }
            inBacktick = false;
            backtickStart = -1;
          }
        }
      }
    }
  }
}
