using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core.Services;

public interface IExperimentFilterService
{
    List<Experiment> FilterBySubpath(List<Experiment> experiments, List<string> subpaths, bool matchIfNoTargeting = false);
}
