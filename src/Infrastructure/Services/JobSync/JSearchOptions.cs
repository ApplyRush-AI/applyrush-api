namespace Infrastructure.Services.JobSync;

public sealed class JSearchOptions
{
    public const string SectionName = "JSearch";
    public string ApiKey { get; set; } = string.Empty;
    public int PagesPerQuery { get; set; } = 3;

    /// <summary>Minimum spacing between JSearch requests. RapidAPI rejects bursts with 429 well before the monthly quota is reached.</summary>
    public int RequestDelayMs { get; set; } = 1100;

    /// <summary>How many times a 429/5xx response is retried before the page is given up.</summary>
    public int RetryAttempts { get; set; } = 3;
}
