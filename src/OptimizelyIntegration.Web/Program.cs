var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("OptimizelyClient");
builder.Services.AddTransient<OptimizelyIntegration.Core.Services.IOptimizelyClient, OptimizelyIntegration.Core.Services.OptimizelyClient>();
builder.Services.AddScoped<OptimizelyIntegration.Core.Services.IExperimentFilterService, OptimizelyIntegration.Core.Services.ExperimentFilterService>();
builder.Services.AddScoped<OptimizelyIntegration.Core.Services.IExperimentCacheService, OptimizelyIntegration.Core.Services.ExperimentCacheService>();
builder.Services.AddMemoryCache();


var app = builder.Build();

app.MapPost("/refresh-experiments", async ([Microsoft.AspNetCore.Mvc.FromBody] OptimizelyIntegration.Web.Models.OptimizelyWebhookPayload payload, OptimizelyIntegration.Core.Services.IExperimentCacheService cacheService, IConfiguration config) => 
{
    // Check if token exists (simple validation)
    var token = config["Optimizely:BearerToken"];
    if (string.IsNullOrEmpty(token)) return Results.Problem("Token missing");

    var projectId = payload.ProjectId;
    
    // We assume the same status flags used in the main app
    var status = OptimizelyIntegration.Core.ExperimentConstants.ValidationStatus;

    cacheService.RefreshCache(projectId, status);
    
    // Re-populate cache immediately
    await cacheService.GetExperimentsAsync(projectId, token, status);
    
    return Results.Ok($"Cache refreshed for Project {projectId}");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
