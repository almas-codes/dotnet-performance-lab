using PerformanceLab.Analysis;

namespace PerformanceLab.Analysis.Tests;

public class BenchmarkResultAnalyzerTests
{
    [Fact]
    public void IsStable_Returns_True_For_Low_Variance()
    {
        Assert.True(BenchmarkResultAnalyzer.IsStable(meanNs: 100, standardDeviationNs: 10));
    }

    [Fact]
    public void IsStable_Returns_False_For_High_Variance()
    {
        Assert.False(BenchmarkResultAnalyzer.IsStable(meanNs: 100, standardDeviationNs: 80));
    }

    [Fact]
    public void PercentDifference_Calculates_Expected_Value()
    {
        var delta = BenchmarkResultAnalyzer.PercentDifference(baseline: 100, current: 120);
        Assert.Equal(20, delta, precision: 5);
    }
}
