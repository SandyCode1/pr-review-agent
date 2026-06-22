using Microsoft.AspNetCore.Mvc;
using PRReviewAgent.Models.Webhooks;
using PRReviewAgent.Services;
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
        string payloadJson;

        using (var reader = new StreamReader(Request.Body))
        {
            payloadJson = await reader.ReadToEndAsync();
        }

        Console.WriteLine("=====================================");
        Console.WriteLine("WEBHOOK REQUEST RECEIVED");
        Console.WriteLine("=====================================");

        // Return immediately to GitHub
        _ = Task.Run(() => ProcessWebhookAsync(payloadJson));

        return Ok();
    }

    private async Task ProcessWebhookAsync(string payloadJson)
    {
        try
        {
            Console.WriteLine("STEP 1 - PARSING PAYLOAD");

            var payload =
                JsonSerializer.Deserialize<PullRequestWebhookPayload>(
                    payloadJson);

            if (payload is null)
            {
                Console.WriteLine("Payload deserialization failed.");
                return;
            }

            Console.WriteLine($"Action: {payload.Action}");
            Console.WriteLine($"PR Number: {payload.PullRequest.Number}");
            Console.WriteLine($"Repository: {payload.Repository.Name}");

            if (payload.Action != "opened" &&
                payload.Action != "synchronize")
            {
                Console.WriteLine(
                    $"Ignoring action: {payload.Action}");

                return;
            }

            Console.WriteLine("STEP 2 - LOADING CONFIG");

            var owner =
                _configuration["GitHub:Owner"];

            var repo =
                _configuration["GitHub:Repository"];

            var token =
                _configuration["GitHub:Token"];

            if (string.IsNullOrWhiteSpace(owner) ||
                string.IsNullOrWhiteSpace(repo) ||
                string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine(
                    "GitHub configuration missing.");

                return;
            }

            Console.WriteLine("STEP 3 - FETCHING PR DIFF");

            var diff =
                await _gitHubService.GetPullRequestDiffAsync(
                    owner,
                    repo,
                    payload.PullRequest.Number,
                    token);

            Console.WriteLine(
                $"Diff Length: {diff.Length}");

            if (string.IsNullOrWhiteSpace(diff))
            {
                Console.WriteLine("Diff is empty.");
                return;
            }

            Console.WriteLine("STEP 4 - CALLING OPENAI");

            var review =
                await _reviewService.GetAIReview(diff);

            Console.WriteLine("STEP 5 - AI REVIEW COMPLETED");
            Console.WriteLine($"Summary: {review.Summary}");
            Console.WriteLine($"Verdict: {review.Verdict}");

            var issuesText =
                review.Issues != null &&
                review.Issues.Any()
                ? string.Join(
                    "\n\n",
                    review.Issues.Select(i =>
$"""
### 🚨 {i.Severity}

**File:** {i.File}

**Issue:** {i.Description}

**Recommendation:** {i.Recommendation}
"""))
                : "No issues found 🎉";

            var comment =
$"""
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

            Console.WriteLine("STEP 6 - POSTING COMMENT TO GITHUB");

            await _gitHubService.PostPullRequestCommentAsync(
                owner,
                repo,
                payload.PullRequest.Number,
                token,
                comment);

            Console.WriteLine(
                "STEP 7 - COMMENT POSTED SUCCESSFULLY");
        }
        catch (Exception ex)
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("WEBHOOK PROCESSING FAILED");
            Console.WriteLine("=====================================");
            Console.WriteLine(ex.ToString());
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Time = DateTime.UtcNow
        });
    }
}