using Microsoft.AspNetCore.Mvc.Testing;

namespace PerformanceLab.IntegrationTests;

public class WebDashboardTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebDashboardTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HomePage_Lists_Benchmark_Scenarios()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("dictionary-vs-list", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("List vs Dictionary Lookup", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Benchmark Catalog", content, StringComparison.OrdinalIgnoreCase);
    }
}
