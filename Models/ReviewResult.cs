namespace PRReviewAgent.Models;

public class ReviewResult
{
    public string Summary { get; set; } = string.Empty;

    public string Verdict { get; set; } = string.Empty;

    public List<ReviewIssue> Issues { get; set; } = [];
}