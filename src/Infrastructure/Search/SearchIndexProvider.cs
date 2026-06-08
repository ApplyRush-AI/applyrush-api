using Application.Common.Search;
using Application.Features.JobOffers.Search;
using Application.Features.Notifications.Search;
using Application.Features.Users.Search;

namespace Infrastructure.Search;

public class SearchIndexProvider : ISearchIndexProvider
{
    public string GetIndex<T>() where T : ISearchable
    {
        return typeof(T) switch
        {
            _ when typeof(T) == typeof(UserSearchable) => SearchIndex.User,
            _ when typeof(T) == typeof(NotificationSearchable) => SearchIndex.Notification,
            _ when typeof(T) == typeof(JobOfferSearchable) => SearchIndex.JobOffer,
            _ => SearchIndex.Default
        };
    }
}
