using PerformanceLab.Core.Environment;

namespace PerformanceLab.Core.Tests;

public class EnvironmentFingerprintTests
{
    [Fact]
    public void Capture_Returns_NonEmpty_Runtime()
    {
        var profile = EnvironmentFingerprint.Capture();

        Assert.False(string.IsNullOrWhiteSpace(profile.Runtime));
        Assert.True(profile.LogicalCores > 0);
    }

    [Fact]
    public void ToJson_Returns_Valid_Json()
    {
        var json = EnvironmentFingerprint.ToJson();

        Assert.Contains("capturedAtUtc", json);
        Assert.Contains("profile", json);
    }
}
