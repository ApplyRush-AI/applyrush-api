using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums;
using DTO.Profile;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.Queries;

public sealed record ProfileCompletionGetQuery : IQuery<ProfileCompletionResponse>;

public sealed class ProfileCompletionGetQueryHandler : IQueryHandler<ProfileCompletionGetQuery, ProfileCompletionResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public ProfileCompletionGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }

    public async Task<ProfileCompletionResponse> Handle(ProfileCompletionGetQuery request, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.Educations)
            .Include(p => p.EeoData)
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var hasPersonal = !string.IsNullOrWhiteSpace(profile.Title) && !string.IsNullOrWhiteSpace(profile.Location);
        var hasExperience = profile.WorkExperiences.Any(w => w.Status != Status.Deleted);
        var hasEducation = profile.Educations.Any(e => e.Status != Status.Deleted);
        var hasSkills = profile.Skills.Any();
        var hasEeo = profile.EeoData != null;

        var completedSections = new[] { hasPersonal, hasExperience, hasEducation, hasSkills, hasEeo }
            .Count(b => b);
        var percentage = completedSections * 20;

        return new ProfileCompletionResponse
        {
            Percentage = percentage,
            Breakdown = new ProfileCompletionBreakdown
            {
                Personal = hasPersonal,
                Experience = hasExperience,
                Education = hasEducation,
                Skills = hasSkills,
                Eeo = hasEeo
            }
        };
    }
}
