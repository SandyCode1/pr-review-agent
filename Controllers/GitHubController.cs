using Microsoft.AspNetCore.Mvc;
using PRReviewAgent.Configurations;
using PRReviewAgent.Services;

namespace PRReviewAgent.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly GitHubService _gitHubService;

    public GitHubController(
        IConfiguration configuration,
        GitHubService gitHubService)
    {
        _configuration = configuration;
        _gitHubService = gitHubService;
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
}