using System.Collections.Concurrent;

public class DocumentManager
{
  public static readonly DocumentManager Instance = new();

  private readonly ConcurrentDictionary<string, string> _documents = new();

  public void Update(string uri, string text) => _documents[uri] = text;

  public void Remove(string uri) => _documents.TryRemove(uri, out _);

  public string? Get(string uri) => _documents.TryGetValue(uri, out var text) ? text : null;
}
