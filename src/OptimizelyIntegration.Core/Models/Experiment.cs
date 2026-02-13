using System.Text.Json.Serialization;

namespace OptimizelyIntegration.Core.Models;

public class Experiment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("project_id")]
    public long ProjectId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("url_targeting")]
    public UrlTargeting? UrlTargeting { get; set; }
}

public class UrlTargeting
{
    [JsonPropertyName("activation_type")]
    public string? ActivationType { get; set; }

    [JsonPropertyName("conditions")]
    public string? Conditions { get; set; }

    [JsonPropertyName("edit_url")]
    public string? EditUrl { get; set; }
}
