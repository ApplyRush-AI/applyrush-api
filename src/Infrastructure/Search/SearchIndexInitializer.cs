using Application.Features.JobOffers.Commands;
using Application.Features.Notifications.Commands;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Infrastructure.Search;

public static class SearchIndexInitializer
{
    public static async Task InitializeIndexes(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        await InitializeUserIndex(mediatr, logger);
        await InitializeNotificationIndex(mediatr, logger);
        await InitializeJobOfferIndex(mediatr, logger);
    }

    private static async Task InitializeUserIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR USER");
            await mediatr.Send(new UserRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR USER");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR USER");
        }
    }

    private static async Task InitializeNotificationIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR  Notification");
            await mediatr.Send(new NotificationRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR  Notification");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR  Notification");
        }
    }

    private static async Task InitializeJobOfferIndex(ISender mediatr, ILogger<ApplicationDbContextInitialiser> logger)
    {
        try
        {
            logger.LogDebug("STARTED BUILDING SEARCH INDEX FOR JobOffer");
            await mediatr.Send(new JobOfferRebuildSearchIndexCommand());
            logger.LogDebug("FINISHED BUILDING SEARCH INDEX FOR JobOffer");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR WHILE BUILDING SEARCH INDEX FOR JobOffer");
        }
    }
}
