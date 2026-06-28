using Application.Common.Constants;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities.Subscriptions.UserSubscriptions;
using DTO.Admin;
using DTO.Enums.Subscription;
using DTO.Subscription;

namespace Application.Features.Subscriptions.Mappings;

public sealed class SubscriptionMapperProfile : Profile
{
    public SubscriptionMapperProfile()
    {
        CreateMap<UserSubscription, SubscriptionResponse>()
            .ForMember(d => d.RenewalDate, opt => opt.MapFrom(s => s.CurrentPeriodEnd))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => FormatPrice(s.Plan, s.BillingInterval)));

        CreateMap<UserSubscription, AdminSubscriptionListItemResponse>()
            .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User.Email))
            .ForMember(d => d.StartDate, opt => opt.MapFrom(s => s.CurrentPeriodStart));

        CreateMap<UserSubscription, AdminUserListItemResponse>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email))
            .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.User.FirstName))
            .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.User.LastName))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.User.Status))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.User.Created));

        CreateMap<UserSubscription, AdminUserDetailResponse>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.UserId))
            .ForMember(d => d.Email, opt => opt.MapFrom(s => s.User.Email))
            .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.User.FirstName))
            .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.User.LastName))
            .ForMember(d => d.SubscriptionStatus, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.UserStatus, opt => opt.MapFrom(s => s.User.Status))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.User.Created))
            .ForMember(d => d.ResumeCount, opt => opt.Ignore())
            .ForMember(d => d.TailoringCount, opt => opt.Ignore())
            .ForMember(d => d.AnalysisCount, opt => opt.Ignore())
            .ForMember(d => d.ApplicationCount, opt => opt.Ignore())
            .ForMember(d => d.TailoringCreditsRemaining, opt => opt.Ignore())
            .ForMember(d => d.AnalysisCreditsRemaining, opt => opt.Ignore())
            .ForMember(d => d.AutofillCreditsRemaining, opt => opt.Ignore());

        CreateMap<JobSyncStateData, AdminJobSyncStatusResponse>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
    }

    private static string FormatPrice(SubscriptionPlan plan, BillingInterval? interval)
        => plan switch
        {
            SubscriptionPlan.Pro when interval == BillingInterval.Quarterly => SubscriptionPrices.ProQuarterly,
            SubscriptionPlan.Pro => SubscriptionPrices.ProMonthly,
            SubscriptionPlan.Premium when interval == BillingInterval.Quarterly => SubscriptionPrices.PremiumQuarterly,
            SubscriptionPlan.Premium => SubscriptionPrices.PremiumMonthly,
            _ => SubscriptionPrices.Free
        };
}

