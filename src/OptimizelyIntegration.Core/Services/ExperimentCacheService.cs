using Microsoft.Extensions.Caching.Memory;
using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core.Services;

public class ExperimentCacheService : IExperimentCacheService
{
    private readonly IOptimizelyClient _client;
    private readonly IMemoryCache _cache;

    public ExperimentCacheService(IOptimizelyClient client, IMemoryCache cache)
    {
        _client = client;
        _cache = cache;
    }

    public async Task<List<Experiment>> GetExperimentsAsync(long projectId, string token, ExperimentStatus status)
    {
        var cacheKey = GetCacheKey(projectId, status);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
            return await _client.GetExperimentsAsync(projectId, token, status);
        }) ?? new List<Experiment>();
    }

    public void RefreshCache(long projectId, ExperimentStatus status)
    {
        var cacheKey = GetCacheKey(projectId, status);
        _cache.Remove(cacheKey);
    }

    private string GetCacheKey(long projectId, ExperimentStatus status)
    {
        return $"experiments_{projectId}_{(int)status}";
    }
}
