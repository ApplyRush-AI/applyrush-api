using Domain.Entities.Jobs.UserJobMatches;

namespace Application.Common.Interfaces.Services;

public interface IMatchScoringService
{
    Task<UserJobMatch> ComputeAndSaveAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken);
}
