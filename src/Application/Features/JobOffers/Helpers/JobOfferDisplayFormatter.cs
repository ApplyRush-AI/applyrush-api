namespace Application.Features.JobOffers.Helpers;

public static class JobOfferDisplayFormatter
{
    // Salaries arrive in mixed units (hourly e.g. 41.25, annual e.g. 137000). Values >= 1000 are treated
    // as annual and abbreviated to "k"; smaller values are shown raw so hourly rates don't collapse to "$0k".
    public static string? FormatSalary(decimal? min, decimal? max, string? currency)
    {
        if (!min.HasValue && !max.HasValue) return null;
        var symbol = currency == "USD" ? "$" : (currency ?? "$");
        static string FormatAmount(decimal v) => v >= 1000 ? $"{v / 1000:0}k" : v.ToString("0");
        if (min.HasValue && max.HasValue)
            return $"{symbol}{FormatAmount(min.Value)} – {symbol}{FormatAmount(max.Value)}";
        if (min.HasValue)
            return $"{symbol}{FormatAmount(min.Value)}+";
        return $"Up to {symbol}{FormatAmount(max!.Value)}";
    }

    public static string FormatTimeAgo(DateTime postedAt, DateTime now)
    {
        var diff = now - postedAt;
        return diff.TotalDays >= 1
            ? $"{(int)diff.TotalDays}d ago"
            : diff.TotalHours >= 1
                ? $"{(int)diff.TotalHours}h ago"
                : "Just now";
    }
}
