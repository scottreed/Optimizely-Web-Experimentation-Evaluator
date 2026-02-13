using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OptimizelyIntegration.Core.Models;
using OptimizelyIntegration.Core.Services;
using OptimizelyIntegration.Core;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OptimizelyIntegration.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IExperimentCacheService _experimentCacheService;
    private readonly IExperimentFilterService _experimentFilterService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IExperimentCacheService experimentCacheService, IExperimentFilterService experimentFilterService, IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _experimentCacheService = experimentCacheService;
        _experimentFilterService = experimentFilterService;
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public long? ProjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Subpath { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Subpath2 { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IncludeNoTargeting { get; set; }

    public List<Experiment>? Experiments { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        if (ProjectId.HasValue)
        {
            var token = _configuration["Optimizely:BearerToken"];
            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Bearer token is not configured in appsettings.json.";
                return;
            }

            try
            {
                Experiments = await _experimentCacheService.GetExperimentsAsync(ProjectId.Value, token, ExperimentConstants.ValidationStatus);

                if (Experiments != null)
                {
                    var subpaths = new List<string>();
                    if (!string.IsNullOrWhiteSpace(Subpath)) subpaths.Add(Subpath);
                    if (!string.IsNullOrWhiteSpace(Subpath2)) subpaths.Add(Subpath2);

                    if (subpaths.Count > 0 || IncludeNoTargeting)
                    {
                        Experiments = _experimentFilterService.FilterBySubpath(Experiments, subpaths, IncludeNoTargeting);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching experiments");
                ErrorMessage = $"Error fetching experiments: {ex.Message}";
            }
        }
    }

    public async Task<IActionResult> OnPostRefreshAsync()
    {
        if (ProjectId.HasValue)
        {
             _experimentCacheService.RefreshCache(ProjectId.Value, ExperimentConstants.ValidationStatus);
             
             // Optionally fetch immediately to warm cache, though redirection will trigger OnGetAsync anyway
        }

        return RedirectToPage(new { ProjectId, Subpath, Subpath2, IncludeNoTargeting });
    }
}
