using Application.Features.JobOffers.Commands;
using Application.Features.JobOffers.Data;
using Application.Features.JobOffers.Search;
using AutoMapper;
using DTO.JobOffers;

namespace Application.Features.JobOffers.Mappings;

public sealed class JobOfferRequestMapperProfile : Profile
{
    public JobOfferRequestMapperProfile()
    {
        CreateMap<JobOfferCreateRequest, JobOfferCreateCommand>()
            .ConstructUsing((src, _) => new JobOfferCreateCommand(
                src.UserId,
                src.Title,
                src.Company,
                src.CompanyLogoUrl,
                src.Description,
                src.About,
                src.Responsibilities,
                src.Requirements,
                src.Benefits,
                src.RequiredSkills,
                src.Industry,
                src.Location,
                src.WorkModel,
                src.JobType,
                src.ExperienceLevel,
                src.SalaryMin,
                src.SalaryMax,
                src.Currency,
                src.YearsRequired,
                src.ApplicantCount,
                src.PostedAt,
                src.ExpiresAt,
                src.ApplyUrl,
                src.H1BSupported,
                src.AiSummary));

        CreateMap<JobOfferFeedItemResponse, JobOfferSearchable>();
        CreateMap<JobOfferDetailResponse, JobOfferSearchable>()
            .IncludeBase<JobOfferFeedItemResponse, JobOfferSearchable>();

        CreateMap<JobOfferCreateCommand, JobOfferCreateData>()
            .ConstructUsing((src, _) => new JobOfferCreateData(
                string.Empty,
                default,
                src.Title,
                src.Company,
                src.CompanyLogoUrl,
                src.Description,
                src.About,
                src.Responsibilities,
                src.Requirements,
                src.Benefits,
                src.RequiredSkills,
                src.Industry,
                src.Location,
                src.WorkModel,
                src.JobType,
                src.ExperienceLevel,
                src.SalaryMin,
                src.SalaryMax,
                src.Currency,
                src.YearsRequired,
                src.ApplicantCount,
                src.PostedAt,
                src.ExpiresAt,
                src.ApplyUrl,
                src.H1BSupported,
                src.AiSummary,
                default));
    }
}
