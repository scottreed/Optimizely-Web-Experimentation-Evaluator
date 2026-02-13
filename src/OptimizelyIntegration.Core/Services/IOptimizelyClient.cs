namespace OptimizelyIntegration.Core.Services;

using OptimizelyIntegration.Core.Models;

public interface IOptimizelyClient
{
    Task<List<Experiment>> GetExperimentsAsync(long projectId, string token, ExperimentStatus statusFilter = ExperimentStatus.Active | ExperimentStatus.Running);
}
