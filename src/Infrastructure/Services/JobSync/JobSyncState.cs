using Application.Common.Caching;
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services.JobSync;

public sealed class JobSyncState : IJobSyncState
{
    private const string CacheKey = "job:sync:state";
    private static readonly TimeSpan Expiry = TimeSpan.FromDays(365);

    private readonly ICacheService _cache;

    public JobSyncState(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<JobSyncStateData> GetAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetAsync<JobSyncStateData>(CacheKey, cancellationToken)
               ?? new JobSyncStateData();
    }

    public async Task SetAsync(JobSyncStateData state, CancellationToken cancellationToken)
    {
        await _cache.AddAsync(CacheKey, state, Expiry, cancellationToken);
    }
}
