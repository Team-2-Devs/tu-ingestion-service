namespace Ingestion.Domain.Factories;

public static class ObjectKeyFactory
{
  /// <summary>Generates standardized object storage keys for uploaded images.</summary>
  /// <param name="filename">The original filename provided by the client.</param>
  /// <param name="nowUtc">The UTC timestamp used to structure the key path.</param>
  /// <returns>A normalized storage key in the format "images/yyyy/MM/dd/{guid}.{ext}".</returns>
  public static string ForImage(string filename, DateTimeOffset nowUtc)
  {
    var ext = Path.GetExtension(filename)
      .TrimStart('.')
      .ToLowerInvariant();

    if (string.IsNullOrEmpty(ext)) ext = "jpg";

    var guid = Guid.NewGuid().ToString();

    return $"images/{nowUtc:yyyy}/{nowUtc:MM}/{nowUtc:dd}/{guid}.{ext}";
  }
}
