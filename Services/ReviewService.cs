using System.Text.Json;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using PRReviewAgent.Models;

namespace PRReviewAgent.Services;

public class ReviewService
{
    private readonly AzureOpenAIClient _client;
    private readonly IConfiguration _configuration;

    public ReviewService(
        AzureOpenAIClient client,
        IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
    }

    public async Task<ReviewResult> GetAIReview(string diff)
    {
        var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

        var prompt = BuildReviewPrompt(diff);

        ChatClient chatClient = _client.GetChatClient(deploymentName);

        var response = await chatClient.CompleteChatAsync(
        [
            new SystemChatMessage("""
                You are a senior .NET architect and GitHub Pull Request reviewer.

                Return ONLY valid JSON.
                No markdown.
                No code fences.
                Always include:
                - summary
                - verdict
                - issues (array, even if empty)
            """),

            new UserChatMessage(prompt)
        ]);

        var json = response.Value.Content[0].Text?.Trim() ?? "";

        // ✅ SAFE JSON EXTRACTION (IMPORTANT FIX)
        var start = json.IndexOf('{');
        var end = json.LastIndexOf('}');

        if (start >= 0 && end > start)
        {
            json = json[start..(end + 1)];
        }

        try
        {
            var result = JsonSerializer.Deserialize<ReviewResult>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result ?? new ReviewResult
            {
                Summary = "No review generated",
                Verdict = "Manual Review Required",
                Issues = []
            };
        }
        catch (Exception ex)
        {
            return new ReviewResult
            {
                Summary = $"Unable to parse AI response. Error: {ex.Message}",
                Verdict = "Manual Review Required",
                Issues = []
            };
        }
    }

    private string BuildReviewPrompt(string diff)
    {
        var promptPath = Path.Combine(
            AppContext.BaseDirectory,
            "Prompts",
            "PRReviewPrompt.txt");

        var template = File.ReadAllText(promptPath);

        return template.Replace("{{$diff}}", diff);
    }
}