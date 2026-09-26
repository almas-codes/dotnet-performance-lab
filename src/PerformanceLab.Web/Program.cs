using PerformanceLab.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register our Performance Lab Domain Services
builder.Services.AddSingleton<PerformanceLab.Abstractions.Catalog.IBenchmarkCatalog>(
    new PerformanceLab.Core.Catalog.ReflectionBenchmarkCatalog(typeof(PerformanceLab.Benchmarks.DataAccess.EfCoreVsDapperVsAdoNetScenario).Assembly)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
