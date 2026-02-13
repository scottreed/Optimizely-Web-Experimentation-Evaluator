namespace OptimizelyIntegration.Core.Models;

[Flags]
public enum ExperimentStatus
{
    None = 0,
    Running = 1,
    Paused = 2,
    Archived = 4,
    CampaignPaused = 8,
    Concluded = 16,
    NotStarted = 32,
    Active = 64
}
