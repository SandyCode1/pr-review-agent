using Octokit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "PRReviewAgent");

        httpClient.DefaultRequestHeaders.Accept.Clear();

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/vnd.github.v3.diff"));

        var url =
            $"https://api.github.com/repos/{owner}/{repo}/pulls/{prNumber}";

        Console.WriteLine("=====================================");
        Console.WriteLine("FETCHING PR DIFF");
        Console.WriteLine("=====================================");
        Console.WriteLine(url);

        var response =
            await httpClient.GetAsync(url);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(responseBody);

            response.EnsureSuccessStatusCode();
        }

        return responseBody;
    }

    public async Task PostPullRequestCommentAsync(
        string owner,
        string repo,
        int prNumber,
        string token,
        string comment)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "PRReviewAgent");

        var url =
            $"https://api.github.com/repos/{owner}/{repo}/issues/{prNumber}/comments";

        var payload = new
        {
            body = comment
        };

        var json =
            JsonSerializer.Serialize(payload);

        var content =
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

        Console.WriteLine("=====================================");
        Console.WriteLine("POSTING COMMENT TO GITHUB");
        Console.WriteLine("=====================================");
        Console.WriteLine($"Repo: {owner}/{repo}");
        Console.WriteLine($"PR Number: {prNumber}");
        Console.WriteLine($"Comment Length: {comment.Length}");

        var response =
            await httpClient.PostAsync(
                url,
                content);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine("=====================================");
        Console.WriteLine("GITHUB COMMENT RESPONSE");
        Console.WriteLine("=====================================");
        Console.WriteLine($"Status Code: {response.StatusCode}");
        Console.WriteLine(responseBody);

        response.EnsureSuccessStatusCode();
    }
}