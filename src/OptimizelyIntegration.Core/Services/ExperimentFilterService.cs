using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using OptimizelyIntegration.Core.Models;

namespace OptimizelyIntegration.Core.Services;

public class ExperimentFilterService : IExperimentFilterService
{
    private readonly ILogger<ExperimentFilterService> _logger;

    public ExperimentFilterService(ILogger<ExperimentFilterService> logger)
    {
        _logger = logger;
    }

    public List<Experiment> FilterBySubpath(List<Experiment> experiments, List<string> subpaths, bool matchIfNoTargeting = false)
    {
        if (experiments == null)
        {
            return new List<Experiment>();
        }

        if ((subpaths == null || subpaths.Count == 0 || subpaths.All(string.IsNullOrWhiteSpace)) && !matchIfNoTargeting)
        {
            return experiments;
        }

        return experiments.Where(e =>
        {
            // If checking for no targeting is enabled, and the experiment has no targeting, include it.
            if (matchIfNoTargeting && e.UrlTargeting == null)
            {
                return true;
            }

            // If the experiment has targeting, check if ANY of the provided subpaths match.
            if (e.UrlTargeting != null && subpaths != null && subpaths.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                foreach (var subpath in subpaths.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    if (MatchesSubpath(e.UrlTargeting.Conditions, subpath.Trim()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }).ToList();
    }

    private bool MatchesSubpath(string? conditionsJson, string subpath)
    {
        if (string.IsNullOrEmpty(conditionsJson)) return false;

        try
        {
            var node = JsonNode.Parse(conditionsJson);
            return EvaluateCondition(node, subpath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing conditions JSON");
            return false;
        }
    }

    private bool EvaluateCondition(JsonNode? node, string subpath)
    {
        if (node == null) return false;

        // Handle array conditions like ["and", ...] or ["or", ...] or ["not", ...]
        if (node is JsonArray array)
        {
            if (array.Count == 0) return false;

            var op = array[0]?.ToString();

            if (op == "and")
            {
                // All subsequent elements must be true (skip the first element which is the operator)
                for (int i = 1; i < array.Count; i++)
                {
                    if (!EvaluateCondition(array[i], subpath)) return false;
                }
                return true;
            }
            else if (op == "or")
            {
                // Any subsequent element must be true
                for (int i = 1; i < array.Count; i++)
                {
                    if (EvaluateCondition(array[i], subpath)) return true;
                }
                return false;
            }
            else if (op == "not")
            {
                // The next element must be false
                if (array.Count > 1)
                {
                    return !EvaluateCondition(array[1], subpath);
                }
                return false;
            }
        }

        // Handle object conditions like {"match_type": "substring", ...}
        if (node is JsonObject obj)
        {
            if (obj.TryGetPropertyValue("match_type", out var matchTypeNode) && matchTypeNode != null)
            {
                var matchType = matchTypeNode.ToString();
                if (matchType == "substring" || matchType == "simple")
                {
                    if (obj.TryGetPropertyValue("value", out var valueNode) && valueNode != null)
                    {
                        var value = valueNode.ToString();
                        return subpath.Contains(value, StringComparison.OrdinalIgnoreCase);
                    }
                }
                // We only care about substring matches matching the user input for this specific requirement.
                // Other match types fail the "substring match" check.
                return false;
            }
        }

        return false;
    }
}
