using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Interfaces;
using DTO.Admin;
using DTO.Enums.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Admin.Queries;

public sealed record AdminDashboardGetQuery : IQuery<AdminDashboardResponse>;

public sealed class AdminDashboardGetQueryHandler : IQueryHandler<AdminDashboardGetQuery, AdminDashboardResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTime _dateTime;

    public AdminDashboardGetQueryHandler(IApplicationDbContext dbContext, IDateTime dateTime)
    {
        _dbContext = dbContext;
        _dateTime = dateTime;
    }

    public async Task<AdminDashboardResponse> Handle(AdminDashboardGetQuery query, CancellationToken cancellationToken)
    {
        var today = _dateTime.Now.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var totalUsers = await _dbContext.User.CountAsync(cancellationToken);
        var activeSubscriptions = await _dbContext.UserSubscription
            .CountAsync(s => s.Plan != SubscriptionPlan.Free && s.Status == SubscriptionStatus.Active, cancellationToken);
        var freeUsers = await _dbContext.UserSubscription.CountAsync(s => s.Plan == SubscriptionPlan.Free, cancellationToken);
        var proUsers = await _dbContext.UserSubscription.CountAsync(s => s.Plan == SubscriptionPlan.Pro, cancellationToken);
        var premiumUsers = await _dbContext.UserSubscription.CountAsync(s => s.Plan == SubscriptionPlan.Premium, cancellationToken);
        var totalApplications = await _dbContext.JobApplication.CountAsync(cancellationToken);
        var newSignupsToday = await _dbContext.User.CountAsync(u => u.Created.Date >= today, cancellationToken);
        var newSignupsThisWeek = await _dbContext.User.CountAsync(u => u.Created.Date >= weekStart, cancellationToken);
        var newSignupsThisMonth = await _dbContext.User.CountAsync(u => u.Created.Date >= monthStart, cancellationToken);
        var totalJobs = await _dbContext.JobListing.CountAsync(cancellationToken);
        var lastSync = await _dbContext.JobListing
            .OrderByDescending(j => j.LastModified)
            .Select(j => (DateTime?)j.LastModified)
            .FirstOrDefaultAsync(cancellationToken);

        return new AdminDashboardResponse(
            totalUsers,
            activeSubscriptions,
            freeUsers,
            proUsers,
            premiumUsers,
            totalJobs,
            lastSync,
            totalApplications,
            newSignupsToday,
            newSignupsThisWeek,
            newSignupsThisMonth);
    }
}
