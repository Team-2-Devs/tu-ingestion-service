namespace Ingestion.Domain.Rules;

public static class ContentTypeRules
{
  private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase) 
  { 
    "image/jpeg", 
    "image/png", 
    "image/webp"
  };

  public static bool IsAllowed(string? contentType) => 
    !string.IsNullOrWhiteSpace(contentType) && 
    Allowed.Contains(contentType);
}
