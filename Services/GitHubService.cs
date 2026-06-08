using Octokit;

namespace PRReviewAgent.Services;

public class GitHubService
{
    public async Task<PullRequest> GetPullRequestAsync(
        string owner,
        string repo,
        int prNumber,
        string token)
    {
        var client = new GitHubClient(
            new ProductHeaderValue("PRReviewAgent"));

        client.Credentials =
            new Credentials(token);

        return await client.PullRequest.Get(
            owner,
            repo,
            prNumber);
    }
}