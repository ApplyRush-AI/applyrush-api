using Domain.Entities.Jobs.UserJobMatches;
using DTO.Enums.Job;

namespace Infrastructure.Services.Data;

internal sealed record UserJobMatchUpsertData(
    int UserId,
    int JobId,
    decimal OverallScore,
    decimal ExperienceScore,
    decimal SkillScore,
    decimal TitleScore,
    decimal IndustryScore,
    string MatchedSkillsJson,
    MatchTier MatchTier,
    DateTime CalculatedAt) : IUserJobMatchUpsertData;
