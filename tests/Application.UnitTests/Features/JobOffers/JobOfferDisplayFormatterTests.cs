using Application.Features.JobOffers.Helpers;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Features.JobOffers;

public class JobOfferDisplayFormatterTests
{
    private static readonly DateTime Now = new(2026, 8, 12, 12, 37, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(30, "Just now")]        // under a minute
    [InlineData(60, "1m ago")]          // exactly one minute
    [InlineData(23 * 60, "23m ago")]    // the reported case (was collapsing to "Just now")
    [InlineData(59 * 60, "59m ago")]    // just under an hour
    [InlineData(60 * 60, "1h ago")]     // exactly one hour
    [InlineData(150 * 60, "2h ago")]    // 2.5 hours -> whole hours
    [InlineData(24 * 60 * 60, "1d ago")]
    [InlineData(5 * 24 * 60 * 60 + 3600, "5d ago")]
    public void FormatTimeAgo_reports_the_coarsest_matching_unit(int secondsAgo, string expected)
    {
        var postedAt = Now.AddSeconds(-secondsAgo);

        JobOfferDisplayFormatter.FormatTimeAgo(postedAt, Now).Should().Be(expected);
    }

    [Fact]
    public void FormatTimeAgo_future_timestamp_is_just_now()
    {
        JobOfferDisplayFormatter.FormatTimeAgo(Now.AddMinutes(5), Now).Should().Be("Just now");
    }
}
