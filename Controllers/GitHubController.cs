using Microsoft.AspNetCore.Mvc;
using PRReviewAgent.Services;
using PRReviewAgent.Models.Webhooks;
using System.Text.Json;

namespace PRReviewAgent.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly GitHubService _gitHubService;
    private readonly ReviewService _reviewService;

    public GitHubController(
        IConfiguration configuration,
        GitHubService gitHubService,
        ReviewService reviewService)
    {
        _configuration = configuration;
        _gitHubService = gitHubService;
        _reviewService = reviewService;
    }

    [HttpPost("webhook")]
    public IActionResult Webhook()
    {
        // ✅ MUST return immediately to avoid GitHub timeout
        _ = Task.Run(ProcessWebhookAsync);

        return Ok();
    }

    private async Task ProcessWebhookAsync()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var payloadJson = await reader.ReadToEndAsync();

            Console.WriteLine("===== WEBHOOK RECEIVED =====");
            Console.WriteLine(payloadJson);

            var payload = JsonSerializer.Deserialize<PullRequestWebhookPayload>(payloadJson);

            if (payload is null)
            {
                Console.WriteLine("Invalid payload");
                return;
            }

            if (payload.Action != "opened" && payload.Action != "synchronize")
            {
                Console.WriteLine($"Ignoring action: {payload.Action}");
                return;
            }

            Console.WriteLine($"ACTION: {payload.Action}");
            Console.WriteLine($"PR NUMBER: {payload.PullRequest.Number}");

            var owner = _configuration["GitHub:Owner"];
            var repo = _configuration["GitHub:Repository"];
            var token = _configuration["GitHub:Token"];

            var diff = await _gitHubService.GetPullRequestDiffAsync(
                owner!, repo!, payload.PullRequest.Number, token!);

            Console.WriteLine($"DIFF LENGTH: {diff.Length}");

            var review = await _reviewService.GetAIReview(diff);

            Console.WriteLine("===== AI REVIEW =====");
            Console.WriteLine($"Summary: {review.Summary}");
            Console.WriteLine($"Verdict: {review.Verdict}");

            var issuesText = review.Issues != null && review.Issues.Count > 0
                ? string.Join("\n\n", review.Issues.Select(i =>
$"""
### 🚨 {i.Severity}
- File: {i.File}
- Issue: {i.Description}
- Fix: {i.Recommendation}
"""))
                : "No issues found 🎉";

            var comment = $"""
## 🤖 AI Pull Request Review

### 📌 Summary
{review.Summary}

### 🎯 Verdict
{review.Verdict}

---

## 🧠 Issues
{issuesText}

---
_This review was generated automatically by PRReviewAgent_
""";

            await _gitHubService.PostPullRequestCommentAsync(
                owner!, repo!, payload.PullRequest.Number, token!, comment);

            Console.WriteLine("===== COMMENT POSTED =====");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🔥 ERROR IN WEBHOOK: {ex.Message}");
        }
    }
}