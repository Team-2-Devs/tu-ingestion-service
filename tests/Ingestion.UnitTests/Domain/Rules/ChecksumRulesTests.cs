using FluentAssertions;
using Ingestion.Domain.Rules;

namespace Ingestion.UnitTests.Domain.Rules;

public sealed class ChecksumRulesTests
{
  private const string ValidSha256 = "sha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

  [Fact]
  public void IsValidSha256_ValidChecksum_ReturnsTrue()
  {
    var checksum = ValidSha256;

    var ok = ChecksumRules.IsValidSha256(checksum);

    ok.Should().BeTrue();
  }

  [Theory]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData(null)]
  [InlineData("sha256:01234")] // too short
  [InlineData("sha256:XYZ3456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")] // invalid hex
  [InlineData("notsha256:0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef")] // wrong prefix
  public void IsValidSha256_InvalidChecksum_ReturnsFalse(string? input)
  {
    var ok = ChecksumRules.IsValidSha256(input);

    ok.Should().BeFalse();
  }
}
