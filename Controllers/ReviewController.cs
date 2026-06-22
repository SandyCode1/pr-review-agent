using Microsoft.AspNetCore.Mvc;
using PRReviewAgent.Services;

namespace PRReviewAgent.Controllers;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly GitHubService _gitHubService;
    private readonly ReviewService _reviewService;

    public ReviewController(
        IConfiguration configuration,
        GitHubService gitHubService,
        ReviewService reviewService)
    {
        _configuration = configuration;
        _gitHubService = gitHubService;
        _reviewService = reviewService;
    }

    [HttpGet("pr/{prNumber}")]
    public async Task<IActionResult> ReviewPullRequest(
        int prNumber)
    {
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
            return BadRequest(
                "GitHub configuration missing.");
        }

        var diff =
            await _gitHubService.GetPullRequestDiffAsync(
                owner,
                repo,
                prNumber,
                token);

        var review =
            await _reviewService.GetAIReview(diff);

        return Ok(new
        {
            PullRequestNumber = prNumber,
            Repository = repo,
            Review = review
        });
    }
}