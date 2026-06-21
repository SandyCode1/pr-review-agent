using Microsoft.AspNetCore.Mvc;
using PRReviewAgent.Configurations;
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

    [HttpGet("pr/{prNumber}")]
    public async Task<IActionResult> GetPullRequest(
        int prNumber)
    {
        var owner =
            _configuration["GitHub:Owner"];

        var repo =
            _configuration["GitHub:Repository"];

        var token =
            _configuration["GitHub:Token"];

        var pr =
            await _gitHubService.GetPullRequestAsync(
                owner!,
                repo!,
                prNumber,
                token!);

        return Ok(new
        {
            pr.Number,
            pr.Title,
            State = pr.State.StringValue
        });
    }


    [HttpGet("pr/{prNumber}/files")]
    public async Task<IActionResult> GetPullRequestFiles(
    int prNumber)
    {
        var owner =
            _configuration["GitHub:Owner"];

        var repo =
            _configuration["GitHub:Repository"];

        var token =
            _configuration["GitHub:Token"];

        var files =
            await _gitHubService.GetPullRequestFilesAsync(
                owner!,
                repo!,
                prNumber,
                token!);

        //return Ok(files.Select(f => new
        //{
        //    f.FileName,
        //    f.Status,
        //    f.Additions,
        //    f.Deletions
        //}));
        return Ok(files.Select(f => new
        {
            f.FileName,
            f.Status,
            f.Additions,
            f.Deletions,
            f.BlobUrl,
            f.RawUrl
        }));
    }

    [HttpGet("pr/{prNumber}/diff")]
    public async Task<IActionResult> GetPullRequestDiff(
    int prNumber)
    {
        var owner = _configuration["GitHub:Owner"];
        var repo = _configuration["GitHub:Repository"];
        var token = _configuration["GitHub:Token"];

        var diff =
            await _gitHubService.GetPullRequestDiffAsync(
                owner!,
                repo!,
                prNumber,
                token!);

        //return Ok(new
        //{
        //    Diff = diff
        //});

        return Content(diff, "text/plain");
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader =
            new StreamReader(Request.Body);

        var payloadJson =
            await reader.ReadToEndAsync();

        var payload =
            JsonSerializer.Deserialize<PullRequestWebhookPayload>(
                payloadJson);

        if (payload is null)
        {
            return BadRequest("Invalid payload");
        }

        if (payload.Action != "opened" && payload.Action != "synchronize")
        {
            Console.WriteLine(
                $"Ignoring action: {payload.Action}");

            return Ok();
        }

        Console.WriteLine("===== WEBHOOK RECEIVED =====");

        Console.WriteLine($"Action: {payload.Action}");
        Console.WriteLine($"PR Number: {payload.PullRequest.Number}");
        Console.WriteLine($"Repository: {payload.Repository.Name}");

        var owner =
            _configuration["GitHub:Owner"];

        var repo =
            _configuration["GitHub:Repository"];

        var token =
            _configuration["GitHub:Token"];

        var diff =
            await _gitHubService.GetPullRequestDiffAsync(
                owner!,
                repo!,
                payload.PullRequest.Number,
                token!);

        Console.WriteLine();
        Console.WriteLine("===== DIFF RECEIVED =====");
        Console.WriteLine($"Diff Length: {diff.Length}");

        var review =
    await _reviewService.GetAIReview(diff);

        Console.WriteLine();
        Console.WriteLine("===== AI REVIEW =====");

        Console.WriteLine($"Summary: {review.Summary}");
        Console.WriteLine($"Verdict: {review.Verdict}");

        var comment = $"""
                        ## 🤖 AI Pull Request Review

                        ### Summary
                        {review.Summary}

                        ### Verdict
                        {review.Verdict}

                        ---
                        Generated automatically by PRReviewAgent
                        """;

        await _gitHubService.PostPullRequestCommentAsync(
            owner!,
            repo!,
            payload.PullRequest.Number,
            token!,
            comment);

        Console.WriteLine();
        Console.WriteLine("===== COMMENT POSTED =====");



        return Ok();
    }


    //for testing only
    [HttpPost("pr/{prNumber}/comment")]
    public async Task<IActionResult> AddComment(int prNumber)
    {
        var owner =
            _configuration["GitHub:Owner"];

        var repo =
            _configuration["GitHub:Repository"];

        var token =
            _configuration["GitHub:Token"];

        await _gitHubService.PostPullRequestCommentAsync(
            owner!,
            repo!,
            prNumber,
            token!,
            "🤖 Test comment from AI Review Agent");

        return Ok();
    }
}