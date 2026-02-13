using System.Text.Json.Serialization;

namespace OptimizelyIntegration.Web.Models;

public record OptimizelyWebhookPayload(
    [property: JsonPropertyName("project_id")] long ProjectId,
    [property: JsonPropertyName("timestamp")] long Timestamp,
    [property: JsonPropertyName("event")] string Event,
    [property: JsonPropertyName("data")] object Data
);
