using Application.Common.Interfaces;
using Domain.Entities.Subscriptions.UserCredits;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Interfaces;
using DTO.Enums.Subscription;
using Infrastructure.TaskScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Worker.Tasks;

public sealed class DailyCreditResetTask : ScheduledTaskBase
{
    protected override string Schedule => "0 0 * * *"; // Every day at midnight UTC
    protected override string Name => "DailyCreditReset";

    public DailyCreditResetTask(IServiceScopeFactory serviceScopeFactory)
        : base(serviceScopeFactory)
    {
    }

    protected override async Task Run(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<IApplicationDbContext>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var dateTime = serviceProvider.GetRequiredService<IDateTime>();

        var resetBefore = dateTime.Now.Date; // Only reset if not already reset today

        var credits = await dbContext.UserCredit
            .Include(c => c.User)
            .Where(c => c.LastResetAt < resetBefore)
            .ToListAsync();

        if (!credits.Any()) return;

        // Load subscription plans in bulk
        var userIds = credits.Select(c => c.UserId).ToList();
        var subscriptionsByUser = await dbContext.UserSubscription
            .AsNoTracking()
            .Where(s => userIds.Contains(s.UserId))
            .ToDictionaryAsync(s => s.UserId);

        var now = dateTime.Now;
        foreach (var credit in credits)
        {
            subscriptionsByUser.TryGetValue(credit.UserId, out var subscription);
            var plan = subscription?.Plan ?? SubscriptionPlan.Free;

            var (tailoring, analysis, autofill) = GetAllocations(plan);
            credit.Reset(tailoring, analysis, autofill, now);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private static (int tailoring, int analysis, int autofill) GetAllocations(SubscriptionPlan plan)
        => plan switch
        {
            SubscriptionPlan.Premium => (-1, -1, -1),
            SubscriptionPlan.Pro => (20, 5, 20),
            _ => (2, 1, 4) // Free
        };
}
