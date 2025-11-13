using FluentAssertions;
using Ingestion.Domain.Rules;

namespace Ingestion.UnitTests.Domain.Rules;

public sealed class ContentTypeRulesTests
{
  [Theory]
  [InlineData("image/jpeg")]
  [InlineData("image/png")]
  [InlineData("image/webp")]
  [InlineData("image/JPEG")]
  public void IsAllowed_AllowedTypes_ReturnsTrue(string input)
  {
    var ok = ContentTypeRules.IsAllowed(input);

    ok.Should().BeTrue();
  }

  [Theory]
  [InlineData("")]
  [InlineData("  ")]
  [InlineData(null)]
  [InlineData("text/plain")]
  [InlineData("image/gif")]
  public void IsAllowed_DisallowedOrEmpty_ReturnsFalse(string? input)
  {
    var ok = ContentTypeRules.IsAllowed(input);

    ok.Should().BeFalse();
  }
}
