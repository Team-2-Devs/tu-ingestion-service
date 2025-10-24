using System.Text.RegularExpressions;

namespace Ingestion.Domain.Rules;

public static class ChecksumRules
{
  private static readonly Regex Sha256Regex =
    new(@"^sha256:[0-9a-f]{64}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

  public static bool IsValidSha256(string? value) =>
    !string.IsNullOrWhiteSpace(value) &&
    Sha256Regex.IsMatch(value);
}
