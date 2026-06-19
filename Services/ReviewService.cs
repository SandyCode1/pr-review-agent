namespace PRReviewAgent.Services;

public class ReviewService
{
    public string BuildReviewPrompt(string diff)
    {
        var promptTemplate =
            File.ReadAllText(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Prompts",
                    "PRReviewPrompt.txt"));

        return promptTemplate.Replace(
            "{{$diff}}",
            diff);
    }
}