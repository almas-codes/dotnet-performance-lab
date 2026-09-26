namespace PerformanceLab.Analysis;

public static class BenchmarkResultAnalyzer
{
    public static bool IsStable(double meanNs, double standardDeviationNs, double maxRelativeSpread = 0.35)
    {
        if (meanNs <= 0)
        {
            return false;
        }

        return (standardDeviationNs / meanNs) <= maxRelativeSpread;
    }

    public static double PercentDifference(double baseline, double current)
    {
        if (baseline == 0)
        {
            return 0;
        }

        return ((current - baseline) / baseline) * 100d;
    }
}
