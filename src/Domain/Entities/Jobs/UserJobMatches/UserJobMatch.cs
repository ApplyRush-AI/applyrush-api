using Domain.Entities.Base;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.User;
using DTO.Enums.Job;

namespace Domain.Entities.Jobs.UserJobMatches;

public sealed class UserJobMatch : BaseEntity
{
    private UserJobMatch() { }

    public int UserId { get; private set; }
    public int JobId { get; private set; }
    public decimal OverallScore { get; private set; }
    public decimal ExperienceScore { get; private set; }
    public decimal SkillScore { get; private set; }
    public decimal TitleScore { get; private set; }
    public decimal IndustryScore { get; private set; }
    public string MatchedSkillsJson { get; private set; } = null!;
    public MatchTier MatchTier { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    public ApplicationUser User { get; } = null!;
    public JobListing Job { get; } = null!;

    public static UserJobMatch Create(IUserJobMatchUpsertData data)
    {
        return new UserJobMatch
        {
            UserId = data.UserId,
            JobId = data.JobId,
            OverallScore = data.OverallScore,
            ExperienceScore = data.ExperienceScore,
            SkillScore = data.SkillScore,
            TitleScore = data.TitleScore,
            IndustryScore = data.IndustryScore,
            MatchedSkillsJson = data.MatchedSkillsJson,
            MatchTier = data.MatchTier,
            CalculatedAt = data.CalculatedAt
        };
    }

    public void Update(IUserJobMatchUpsertData data)
    {
        OverallScore = data.OverallScore;
        ExperienceScore = data.ExperienceScore;
        SkillScore = data.SkillScore;
        TitleScore = data.TitleScore;
        IndustryScore = data.IndustryScore;
        MatchedSkillsJson = data.MatchedSkillsJson;
        MatchTier = data.MatchTier;
        CalculatedAt = data.CalculatedAt;
    }
}
