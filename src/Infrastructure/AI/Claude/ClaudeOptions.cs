namespace Infrastructure.AI.Claude;

public sealed class ClaudeOptions
{
    public const string SectionName = "Claude";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "claude-sonnet-4-5";
    public int MaxTokens { get; set; } = 4096;
    public string AnthropicVersion { get; set; } = "2023-06-01";
    public string ResumeParseSystemPrompt { get; set; } = string.Empty;
}
