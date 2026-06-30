using Application.Common.Interfaces;
using Application.Features.Credits.Data;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Settings;
using Domain.Entities.Subscriptions.UserCredits;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Helpers;

public static class UserFoundationDataHelper
{
    // Free-tier allocations, kept in sync with DailyCreditResetTask.GetAllocations(SubscriptionPlan.Free)
    private const int FreeTailoringCredits = 2;
    private const int FreeAnalysisCredits = 1;
    private const int FreeAutofillCredits = 4;

    public static async Task CreateAndSaveAsync(
        int userId,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IDateTime dateTime,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.UserJobPreference.AnyAsync(x => x.UserId == userId, cancellationToken))
            return;

        var userProfile = UserProfile.Create(userId);
        var notificationPref = UserNotificationPreference.Create(userId);
        var jobPref = UserJobPreference.Create(userId);
        var credit = UserCredit.Create(new UserCreditInsertData(
            userId, FreeTailoringCredits, FreeAnalysisCredits, FreeAutofillCredits, dateTime.Now));

        await dbContext.UserProfile.AddAsync(userProfile, cancellationToken);
        await dbContext.UserNotificationPreference.AddAsync(notificationPref, cancellationToken);
        await dbContext.UserJobPreference.AddAsync(jobPref, cancellationToken);
        await dbContext.UserCredit.AddAsync(credit, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
