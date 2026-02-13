using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core.Services;

public interface IExperimentCacheService
{
    Task<List<Experiment>> GetExperimentsAsync(long projectId, string token, ExperimentStatus status);
    void RefreshCache(long projectId, ExperimentStatus status);
}
