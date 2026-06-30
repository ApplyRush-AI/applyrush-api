using Application.Features.JobOffers.Data;
using Application.Features.JobOffers.Helpers;
using AutoMapper;
using Domain.Interfaces;
using DTO.Enums.Job;
using DTO.MessageBroker.Messages.Jobs;

namespace Application.Features.JobOffers.Mappings;

public sealed class JobDigestMapperProfile : Profile
{
    private readonly IDateTime _dateTime;

    public JobDigestMapperProfile(IDateTime dateTime)
    {
        _dateTime = dateTime;

        CreateMap<ScoredJob, JobDigestItem>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Job.Title))
            .ForMember(d => d.Company, o => o.MapFrom(s => s.Job.Company))
            .ForMember(d => d.Industry, o => o.MapFrom(s => s.Job.Industry))
            .ForMember(d => d.Salary, o => o.MapFrom(s => JobOfferDisplayFormatter.FormatSalary(s.Job.SalaryMin, s.Job.SalaryMax, s.Job.Currency)))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.Job.WorkModel == WorkModel.Remote ? "Remote" : s.Job.Location))
            .ForMember(d => d.MatchScore, o => o.MapFrom(s => (int)Math.Round(s.Score)))
            .ForMember(d => d.PostedAgo, o => o.MapFrom(s => JobOfferDisplayFormatter.FormatTimeAgo(s.Job.PostedAt, _dateTime.Now)))
            .ForMember(d => d.ApplyUrl, o => o.MapFrom(s => s.Job.ApplyUrl));
    }
}
