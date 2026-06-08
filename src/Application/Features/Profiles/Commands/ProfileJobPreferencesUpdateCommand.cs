using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.UserJobTypeReferences;
using Domain.Entities.Profiles.UserPreferredJobFunctions;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Job;
using DTO.Enums.Profile.EeoData;
using DTO.Enums.Profile.UserProfile;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.Commands;

public sealed record ProfileJobPreferencesUpdateCommand(
    IReadOnlyList<int> JobFunctionIds,
    IReadOnlyList<JobType> JobTypes,
    LocationMode LocationMode,
    string? Location,
    bool OpenToRemote,
    bool H1BSponsorship,
    WorkAuthorization? WorkAuthorization
    ) : ICommand, IUserJobPreferencesUpdateData;

public sealed class ProfileJobPreferencesUpdateCommandHandler : ICommandHandler<ProfileJobPreferencesUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileJobPreferencesUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProfileJobPreferencesUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.PreferredJobFunctions)
            .Include(p => p.JobTypePreferences)
            .Include(p => p.EeoData)
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        profile.UpdateJobPreferences(command);

        if (command.WorkAuthorization.HasValue)
        {
            if (profile.EeoData == null)
            {
                var eeoData = EeoData.Create(profile.Id);
                eeoData.SetWorkAuthorization(command.WorkAuthorization);
                await _dbContext.EeoData.AddAsync(eeoData, cancellationToken);
            }
            else
            {
                profile.EeoData.SetWorkAuthorization(command.WorkAuthorization);
            }
        }

        await ReplaceJobFunctions(profile.PreferredJobFunctions, command.JobFunctionIds, profile.Id, cancellationToken);
        await ReplaceJobTypes(profile.JobTypePreferences, command.JobTypes, profile.Id, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ReplaceJobFunctions(ICollection<UserPreferredJobFunction> current, IReadOnlyList<int> ids, int profileId, CancellationToken cancellationToken)
    {
        _dbContext.UserPreferredJobFunction.RemoveRange(current);
        var rows = ids.Select(id => UserPreferredJobFunction.Create(profileId, id));
        await _dbContext.UserPreferredJobFunction.AddRangeAsync(rows, cancellationToken);
    }

    private async Task ReplaceJobTypes(ICollection<UserJobTypePreference> current, IReadOnlyList<JobType> types, int profileId, CancellationToken cancellationToken)
    {
        _dbContext.UserJobTypePreference.RemoveRange(current);
        var rows = types.Select(t => UserJobTypePreference.Create(profileId, t));
        await _dbContext.UserJobTypePreference.AddRangeAsync(rows, cancellationToken);
    }
}

public sealed class ProfileJobPreferencesUpdateCommandValidator : AbstractValidator<ProfileJobPreferencesUpdateCommand>
{
    public ProfileJobPreferencesUpdateCommandValidator()
    {
        RuleFor(c => c.JobFunctionIds).NotEmpty();
        RuleFor(c => c.JobTypes).NotEmpty();
        RuleFor(c => c.LocationMode).IsInEnum();
        RuleFor(c => c.Location).MaximumLength(200);
        RuleFor(c => c.Location)
            .NotEmpty()
            .When(c => c.LocationMode == LocationMode.Specific);
    }
}
