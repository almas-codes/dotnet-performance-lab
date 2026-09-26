using System.Runtime.InteropServices;
using System.Text.Json;
using PerformanceLab.Abstractions.Models;

namespace PerformanceLab.Core.Environment;

public static class EnvironmentFingerprint
{
    public static EnvironmentProfile Capture()
    {
        return new EnvironmentProfile(
            OperatingSystem: RuntimeInformation.OSDescription,
            Architecture: RuntimeInformation.OSArchitecture.ToString(),
            Processor: GetProcessorDescription(),
            LogicalCores: global::System.Environment.ProcessorCount,
            PhysicalCores: global::System.Environment.ProcessorCount,
            Runtime: RuntimeInformation.FrameworkDescription,
            RuntimeVersion: global::System.Environment.Version.ToString(),
            Jit: RuntimeInformation.ProcessArchitecture.ToString());
    }

    public static string ToJson(bool indented = true)
    {
        var payload = new
        {
            capturedAtUtc = DateTime.UtcNow,
            sdk = global::System.Environment.GetEnvironmentVariable("DOTNET_ROOT") ?? "default",
            profile = Capture()
        };

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = indented });
    }

    public static async Task WriteAsync(string path, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(path, ToJson(), cancellationToken);
    }

    private static string GetProcessorDescription()
    {
        try
        {
            return global::System.Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
                ?? RuntimeInformation.ProcessArchitecture.ToString();
        }
        catch
        {
            return "unknown";
        }
    }
}
