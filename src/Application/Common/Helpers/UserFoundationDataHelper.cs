using Application.Common.Interfaces;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Settings;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Helpers;

public static class UserFoundationDataHelper
{
    public static async Task CreateAndSaveAsync(
        int userId,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        var userProfile = UserProfile.Create(userId);
        var notificationPref = UserNotificationPreference.Create(userId);
        var jobPref = UserJobPreference.Create(userId);

        if (await dbContext.UserJobPreference.AnyAsync(x => x.UserId == userId, cancellationToken))
            return;

        await dbContext.UserProfile.AddAsync(userProfile, cancellationToken);
        await dbContext.UserNotificationPreference.AddAsync(notificationPref, cancellationToken);
        await dbContext.UserJobPreference.AddAsync(jobPref, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
