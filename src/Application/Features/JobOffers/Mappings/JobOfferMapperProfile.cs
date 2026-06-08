using AutoMapper;
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
            .ForMember(d => d.Salary, opt => opt.MapFrom(s => FormatSalary(s.SalaryMin, s.SalaryMax, s.Currency)))
            .ForMember(d => d.PostedAgo, opt => opt.MapFrom(s => FormatTimeAgo(s.PostedAt)))
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
            .ForMember(d => d.LogoColor, opt => opt.Ignore());

        CreateMap<JobListing, JobOfferDetailResponse>()
            .IncludeBase<JobListing, JobOfferFeedItemResponse>()
            .ForMember(d => d.RequiredSkills, opt => opt.MapFrom((s, _) =>
                string.IsNullOrEmpty(s.RequiredSkills)
                    ? (IReadOnlyList<string>)Array.Empty<string>()
                    : JsonSerializer.Deserialize<List<string>>(s.RequiredSkills) ?? new List<string>()))
            .ForMember(d => d.Description, opt => opt.MapFrom((s, _) => new JobOfferDescriptionResponse
            {
                About = s.About,
                Responsibilities = string.IsNullOrEmpty(s.Responsibilities)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<List<string>>(s.Responsibilities) ?? new List<string>(),
                Requirements = string.IsNullOrEmpty(s.Requirements)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<List<string>>(s.Requirements) ?? new List<string>(),
                Benefits = string.IsNullOrEmpty(s.Benefits)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<List<string>>(s.Benefits) ?? new List<string>()
            }));
    }

    private static string? FormatSalary(decimal? min, decimal? max, string? currency)
    {
        if (!min.HasValue && !max.HasValue) return null;
        var symbol = currency == "USD" ? "$" : (currency ?? "$");
        static string FormatAmount(decimal v) => v >= 1000 ? $"{v / 1000:0}k" : v.ToString("0");
        if (min.HasValue && max.HasValue)
            return $"{symbol}{FormatAmount(min.Value)} – {symbol}{FormatAmount(max.Value)}";
        if (min.HasValue)
            return $"{symbol}{FormatAmount(min.Value)}+";
        return $"Up to {symbol}{FormatAmount(max!.Value)}";
    }

    private string FormatTimeAgo(DateTime postedAt)
    {
        var diff = _dateTime.Now - postedAt;
        return diff.TotalDays >= 1
            ? $"{(int)diff.TotalDays}d ago"
            : diff.TotalHours >= 1
                ? $"{(int)diff.TotalHours}h ago"
                : "Just now";
    }
}
