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
}