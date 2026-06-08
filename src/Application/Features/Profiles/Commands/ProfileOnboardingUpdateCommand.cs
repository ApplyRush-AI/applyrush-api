using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserJobTypeReferences;
using Domain.Entities.Profiles.UserPreferredJobFunctions;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Job;
using DTO.Enums.Profile.UserProfile;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.Commands;

public sealed record ProfileOnboardingUpdateCommand(
    IReadOnlyList<int> JobFunctionIds,
    IReadOnlyList<JobType> JobTypes,
    LocationMode LocationMode,
    string? Location,
    bool OpenToRemote,
    bool H1BSponsorship
    ) : ICommand, IUserOnboardingUpdateData;

public sealed class ProfileOnboardingUpdateCommandHandler : ICommandHandler<ProfileOnboardingUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileOnboardingUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProfileOnboardingUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.PreferredJobFunctions)
            .Include(p => p.JobTypePreferences)
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        profile.UpdateOnboarding(command);

        await ReplaceJobFunctions(profile.PreferredJobFunctions, command.JobFunctionIds, profile.Id, cancellationToken);

        await ReplaceJobTypes(profile.JobTypePreferences, command.JobTypes, profile.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ReplaceJobFunctions(ICollection<UserPreferredJobFunction> preferredJobFunctions, IReadOnlyList<int> jobFunctionsIds, int profileId, CancellationToken cancellationToken = default)
    {
        _dbContext.UserPreferredJobFunction.RemoveRange(preferredJobFunctions);
        var newJobFunctions = jobFunctionsIds
            .Select(jfId => UserPreferredJobFunction.Create(profileId, jfId));
        await _dbContext.UserPreferredJobFunction.AddRangeAsync(newJobFunctions, cancellationToken);
    }

    private async Task ReplaceJobTypes(ICollection<UserJobTypePreference> jobTypeReferences, IReadOnlyList<JobType> jobTypes, int profileId, CancellationToken cancellationToken = default)
    {
        _dbContext.UserJobTypePreference.RemoveRange(jobTypeReferences);
        var newJobTypes = jobTypes
            .Select(jt => UserJobTypePreference.Create(profileId, jt));
        await _dbContext.UserJobTypePreference.AddRangeAsync(newJobTypes, cancellationToken);

    }
}

public sealed class ProfileOnboardingUpdateCommandValidator : AbstractValidator<ProfileOnboardingUpdateCommand>
{
    public ProfileOnboardingUpdateCommandValidator()
    {
        RuleFor(c => c.JobFunctionIds).NotEmpty();
        RuleFor(c => c.JobTypes).NotEmpty();
        RuleFor(c => c.LocationMode).IsInEnum();
        RuleFor(c => c.Location).MaximumLength(200);
    }
}
