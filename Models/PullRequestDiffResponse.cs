using System.Net.Http.Headers;

namespace PRReviewAgent.Models;

public class PullRequestDiffResponse
{
    public string DiffContent { get; set; } = string.Empty;
   
}