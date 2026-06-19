namespace PRReviewAgent.Services;

public class ReviewService
{
    //    public string BuildReviewPrompt(
    //        string diff)
    //    {
    //        return $"""
    //Review the following pull request diff.

    //Focus on:

    //1. Bugs
    //2. Security issues
    //3. Performance issues
    //4. Code quality
    //5. Maintainability

    //Provide findings with severity levels.

    //DIFF:

    //{diff}
    //""";
    //    }

    public string BuildReviewPrompt(string diff)
    {
        return $"""
You are a Senior .NET Tech Lead.

Review this pull request.

Focus on:

- Bugs
- Security vulnerabilities
- Performance issues
- Code quality
- Maintainability
- SOLID principles
- Dependency Injection
- Error Handling
- Logging
- .NET Best Practices

For each issue provide:

Severity: High | Medium | Low

File:
Issue:
Recommendation:

DIFF:

{diff}
""";
    }
}