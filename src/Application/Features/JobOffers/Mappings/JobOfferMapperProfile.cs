using AutoMapper;
using Application.Features.JobOffers.Helpers;
using Domain.Entities.Jobs.JobListings;
using Domain.Interfaces;
using DTO.JobOffers;
using DTO.Response;
using System.Text.Json;

namespace Application.Features.JobOffers.Mappings;

public sealed class JobOfferMapperProfile : Profile
{
    private readonly IDateTime _dateTime;

    public JobOfferMapperProfile(IDateTime dateTime)
    {
        _dateTime = dateTime;
        CreateMap<JobListing, JobOfferFeedItemResponse>()
            .ForMember(d => d.WorkModel, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.WorkModel, Name = s.WorkModel.ToString() }))
            .ForMember(d => d.JobType, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.JobType, Name = s.JobType.ToString() }))
            .ForMember(d => d.ExperienceLevel, opt => opt.MapFrom(s =>
                new ListItemBaseResponse { Id = (int)s.ExperienceLevel, Name = s.ExperienceLevel.ToString() }))
            .ForMember(d => d.Salary, opt => opt.MapFrom(s => JobOfferDisplayFormatter.FormatSalary(s.SalaryMin, s.SalaryMax, s.Currency)))
            .ForMember(d => d.PostedAt, opt => opt.MapFrom(s => s.PostedAt))
            .ForMember(d => d.PostedAgo, opt => opt.MapFrom(s => JobOfferDisplayFormatter.FormatTimeAgo(s.PostedAt, _dateTime.Now)))
            .ForMember(d => d.MatchScore, opt => opt.MapFrom((s, _, _, ctx) =>
                s.UserMatches.FirstOrDefault()?.OverallScore))
            .ForMember(d => d.Scores, opt => opt.MapFrom((s, _, _, ctx) =>
            {
                var m = s.UserMatches.FirstOrDefault();
                return m == null ? null : new MatchScoresResponse
                {
                    Experience = m.ExperienceScore,
                    Skills = m.SkillScore,
                    Title = m.TitleScore,
                    Industry = m.IndustryScore
                };
            }))
            .ForMember(d => d.MatchedSkills, opt => opt.MapFrom((s, _, _, ctx) =>
            {
                var m = s.UserMatches.FirstOrDefault();
                if (m == null) return (IReadOnlyList<string>)Array.Empty<string>();
                return string.IsNullOrEmpty(m.MatchedSkillsJson)
                    ? (IReadOnlyList<string>)Array.Empty<string>()
                    : JsonSerializer.Deserialize<List<string>>(m.MatchedSkillsJson) ?? new List<string>();
            }))
            .ForMember(d => d.MatchTier, opt => opt.MapFrom((s, _, _, ctx) =>
            {
                var m = s.UserMatches.FirstOrDefault();
                return m == null ? null : new ListItemBaseResponse { Id = (int)m.MatchTier, Name = m.MatchTier.ToString() };
            }))
            .ForMember(d => d.IsSaved, opt => opt.MapFrom((s, _, _, ctx) =>
                s.SavedByUsers.Any()))
            .ForMember(d => d.CompanyShort, opt => opt.MapFrom(s =>
                s.Company.Length <= 2 ? s.Company : string.Concat(s.Company.Where(char.IsUpper).Take(2))))
            .ForMember(d => d.LogoColor, opt => opt.Ignore())
            .ForMember(d => d.RequiredSkills, opt => opt.MapFrom((s, _) =>
                (IReadOnlyList<string>)SafeDeserializeList(s.RequiredSkills)))
            .ForMember(d => d.JobFunctions, opt => opt.MapFrom(s =>
                (IReadOnlyList<ListItemBaseResponse>)s.JobFunctions
                    .Select(jf => new ListItemBaseResponse { Id = jf.JobFunctionId, Name = jf.JobFunction.Name })
                    .ToList()));

        CreateMap<JobListing, JobOfferDetailResponse>()
            .IncludeBase<JobListing, JobOfferFeedItemResponse>()
            .ForMember(d => d.Description, opt => opt.MapFrom((s, _) => new JobOfferDescriptionResponse
            {
                About = s.About,
                Responsibilities = SafeDeserializeList(s.Responsibilities),
                Requirements = SafeDeserializeList(s.Requirements),
                Benefits = SafeDeserializeList(s.Benefits)
            }));
    }

    private static IReadOnlyList<string> SafeDeserializeList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }

}
