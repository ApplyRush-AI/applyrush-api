using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkExperiences.WorkExperienceBullets.Commands;

public sealed record WorkExperienceBulletDeleteCommand(int WorkExperienceId, int BulletId) : ICommand;

public sealed class WorkExperienceBulletDeleteCommandHandler : ICommandHandler<WorkExperienceBulletDeleteCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public WorkExperienceBulletDeleteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(WorkExperienceBulletDeleteCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var workExperience = await _dbContext.WorkExperience
            .FirstOrDefaultAsync(w => w.Id == command.WorkExperienceId && w.UserProfileId == profile.Id, cancellationToken)
            ?? throw NotFoundException.New<WorkExperience>(command.WorkExperienceId);

        var bullet = await _dbContext.WorkExperienceBullet
            .FirstOrDefaultAsync(b => b.Id == command.BulletId && b.WorkExperienceId == command.WorkExperienceId, cancellationToken)
            ?? throw NotFoundException.New<WorkExperienceBullet>(command.BulletId);

        _dbContext.WorkExperienceBullet.Remove(bullet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class WorkExperienceBulletDeleteCommandValidator : AbstractValidator<WorkExperienceBulletDeleteCommand>
{
    public WorkExperienceBulletDeleteCommandValidator()
    {
        RuleFor(c => c.WorkExperienceId).GreaterThan(0);
        RuleFor(c => c.BulletId).GreaterThan(0);
    }
}
