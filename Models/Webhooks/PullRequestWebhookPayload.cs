using System.Text.Json.Serialization;

namespace PRReviewAgent.Models.Webhooks;

public class PullRequestWebhookPayload
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("pull_request")]
    public PullRequestInfo PullRequest { get; set; } = new();

    [JsonPropertyName("repository")]
    public RepositoryInfo Repository { get; set; } = new();
}