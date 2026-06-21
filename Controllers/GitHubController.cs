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
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var payloadJson = await reader.ReadToEndAsync();

        var payload = JsonSerializer.Deserialize<PullRequestWebhookPayload>(payloadJson);

        if (payload is null)
            return BadRequest("Invalid payload");

        if (payload.Action != "opened" && payload.Action != "synchronize")
            return Ok();

        var owner = _configuration["GitHub:Owner"];
        var repo = _configuration["GitHub:Repository"];
        var token = _configuration["GitHub:Token"];

        var diff = await _gitHubService.GetPullRequestDiffAsync(
            owner!, repo!, payload.PullRequest.Number, token!);

        var review = await _reviewService.GetAIReview(diff);

        // ✅ FORMAT ISSUES PROPERLY
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
            owner!,
            repo!,
            payload.PullRequest.Number,
            token!,
            comment);

        return Ok();
    }
}