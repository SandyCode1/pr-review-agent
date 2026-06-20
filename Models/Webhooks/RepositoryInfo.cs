using System.Text.Json.Serialization;

namespace PRReviewAgent.Models.Webhooks;

public class RepositoryInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}