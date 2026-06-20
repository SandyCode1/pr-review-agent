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
        var deploymentName =
            _configuration["AzureOpenAI:DeploymentName"];

        var prompt =
            BuildReviewPrompt(diff);

        ChatClient chatClient =
            _client.GetChatClient(deploymentName);

        var response =
            await chatClient.CompleteChatAsync(
            [
                new SystemChatMessage(
                    """
                    You are a senior .NET architect and GitHub Pull Request reviewer.

                    Always return valid JSON.
                    Never return markdown.
                    Never return code fences.
                    """
                ),

                new UserChatMessage(prompt)
            ]);

        var json =
            response.Value.Content[0].Text;

        json = json
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        try
        {
            var result =
                JsonSerializer.Deserialize<ReviewResult>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return result ?? new ReviewResult
            {
                Summary = "No review generated",
                Verdict = "Manual Review Required"
            };
        }
        catch (Exception ex)
        {
            return new ReviewResult
            {
                Summary = $"Unable to parse AI response. Error: {ex.Message}",
                Verdict = "Manual Review Required"
            };
        }
    }

    private string BuildReviewPrompt(string diff)
    {
        var promptPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Prompts",
                "PRReviewPrompt.txt");

        var template =
            File.ReadAllText(promptPath);

        return template.Replace(
            "{{$diff}}",
            diff);
    }
}