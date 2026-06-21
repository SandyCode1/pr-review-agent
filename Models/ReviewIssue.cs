namespace PRReviewAgent.Models
{
    public class ReviewIssue
    {
        public string Severity { get; set; } = string.Empty;

        public string File { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Recommendation { get; set; } = string.Empty;
    }
}
