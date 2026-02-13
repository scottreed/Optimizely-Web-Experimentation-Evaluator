using System.Net.Http.Json;
using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core.Services;

public class OptimizelyClient : IOptimizelyClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OptimizelyClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<Experiment>> GetExperimentsAsync(long projectId, string token, ExperimentStatus statusFilter = ExperimentStatus.Active | ExperimentStatus.Running)
    {
        var client = _httpClientFactory.CreateClient("OptimizelyClient");
        
        // Ensure the token prevents leading/trailing whitespace issues if pasted by user
        var cleanToken = token.Trim();
        
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.optimizely.com/v2/experiments?project_id=" + projectId);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cleanToken);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var experiments = await response.Content.ReadFromJsonAsync<List<Experiment>>();
        
        if (experiments == null)
        {
            return new List<Experiment>();
        }

        // Requirement: filter the collection based upon the status enum flags.
        return experiments.Where(e => ShouldIncludeExperiment(e, statusFilter)).ToList();
    }

    private bool ShouldIncludeExperiment(Experiment experiment, ExperimentStatus statusFilter)
    {
        if (string.IsNullOrWhiteSpace(experiment.Status)) return false;

        if (statusFilter.HasFlag(ExperimentStatus.Running) && experiment.Status.Equals("running", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.Paused) && experiment.Status.Equals("paused", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.Archived) && experiment.Status.Equals("archived", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.CampaignPaused) && experiment.Status.Equals("campaign_paused", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.Concluded) && experiment.Status.Equals("concluded", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.NotStarted) && experiment.Status.Equals("not_started", StringComparison.OrdinalIgnoreCase)) return true;
        if (statusFilter.HasFlag(ExperimentStatus.Active) && experiment.Status.Equals("active", StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    }
}
