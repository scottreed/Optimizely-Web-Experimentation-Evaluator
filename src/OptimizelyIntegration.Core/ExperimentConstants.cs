using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core;

public static class ExperimentConstants
{
    public static readonly ExperimentStatus ValidationStatus = ExperimentStatus.Active | 
                                                             ExperimentStatus.Running | 
                                                             ExperimentStatus.NotStarted | 
                                                             ExperimentStatus.Archived;
}
