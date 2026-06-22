using System.Text.Json.Serialization;

namespace PRReviewAgent.Models.Webhooks;

public class PullRequestInfo
{
    [JsonPropertyName("number")]
    public int Number { get; set; }
}