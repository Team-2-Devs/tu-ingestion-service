using FluentAssertions;
using Ingestion.Domain.Factories;

namespace Ingestion.UnitTests.Domain.Factories;

public sealed class ObjectKeyFactoryTests
{
  [Fact]
  public void ForImage_UsesDatePathAndLowercaseExtension()
  {
    var now = new DateTimeOffset(2025, 11, 9, 0, 0, 0, TimeSpan.Zero);

    var key = ObjectKeyFactory.ForImage("Photo.PNG", now);

    key.Should().StartWith("images/2025/11/09/");
    var extension = Path.GetExtension(key);
    extension.Should().Be(".png");
  }

  [Fact]
  public void ForImage_WithoutExtension_DefaultsToJpg()
  {
    var now = new DateTimeOffset(2025, 11, 9, 0, 0, 0, TimeSpan.Zero);

    var key = ObjectKeyFactory.ForImage("filename", now);

    key.Should().StartWith("images/2025/11/09/");
    var extension = Path.GetExtension(key);
    extension.Should().Be(".jpg");
  }


  // Note:
  // Guid randomness is not asserted; only testing for shape and behavior.
}
