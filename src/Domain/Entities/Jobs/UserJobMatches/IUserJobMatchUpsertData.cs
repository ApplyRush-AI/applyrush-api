using DTO.Enums.Job;

namespace Domain.Entities.Jobs.UserJobMatches;

public interface IUserJobMatchUpsertData
{
    int UserId { get; }
    int JobId { get; }
    decimal OverallScore { get; }
    decimal ExperienceScore { get; }
    decimal SkillScore { get; }
    decimal TitleScore { get; }
    decimal IndustryScore { get; }
    string MatchedSkillsJson { get; }
    MatchTier MatchTier { get; }
    DateTime CalculatedAt { get; }
}
