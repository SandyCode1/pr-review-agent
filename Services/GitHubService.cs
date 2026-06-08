using Octokit;
using System.Net.Http.Headers;

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
            new Octokit.ProductHeaderValue("PRReviewAgent"));

        client.Credentials =
            new Credentials(token);

        return await client.PullRequest.Get(
            owner,
            repo,
            prNumber);
    }

    public async Task<IReadOnlyList<PullRequestFile>> GetPullRequestFilesAsync(
    string owner,
    string repo,
    int prNumber,
    string token)
    {
        var client = new GitHubClient(
            new Octokit.ProductHeaderValue("PRReviewAgent"));

        client.Credentials =
            new Credentials(token);

        return await client.PullRequest.Files(
            owner,
            repo,
            prNumber);
    }

    public async Task<string> GetPullRequestDiffAsync(
    string owner,
    string repo,
    int prNumber,
    string token)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "PRReviewAgent");

        httpClient.DefaultRequestHeaders.Accept.Clear();

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/vnd.github.v3.diff"));

        var url =
            $"https://api.github.com/repos/{owner}/{repo}/pulls/{prNumber}";

        return await httpClient.GetStringAsync(url);
    }
}