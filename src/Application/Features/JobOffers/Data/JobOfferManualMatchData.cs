using Domain.Entities.Jobs.UserJobMatches;
using DTO.Enums.Job;

namespace Application.Features.JobOffers.Data;

internal sealed record JobOfferManualMatchData(
    int UserId,
    int JobId,
    DateTime CalculatedAt
) : IUserJobMatchUpsertData
{
    public decimal OverallScore => 100;
    public decimal ExperienceScore => 100;
    public decimal SkillScore => 100;
    public decimal TitleScore => 100;
    public decimal IndustryScore => 100;
    public string MatchedSkillsJson => "[]";
    public MatchTier MatchTier => MatchTier.StrongMatch;
}
