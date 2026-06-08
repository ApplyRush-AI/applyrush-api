namespace Infrastructure.Services.JobSync;

public sealed class JSearchOptions
{
    public const string SectionName = "JSearch";
    public string ApiKey { get; set; } = string.Empty;
    public int PagesPerQuery { get; set; } = 3;
}
