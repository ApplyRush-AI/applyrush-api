using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.WorkExperiences.WorkExperienceBullets.Data;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkExperiences.WorkExperienceBullets.Commands;

public sealed record WorkExperienceBulletUpdateCommand(int WorkExperienceId, int BulletId, string Content) : ICommand;

public sealed class WorkExperienceBulletUpdateCommandHandler : ICommandHandler<WorkExperienceBulletUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public WorkExperienceBulletUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(WorkExperienceBulletUpdateCommand command, CancellationToken cancellationToken)
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

        bullet.Update(new WorkExperienceBulletUpdateData(command.Content));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class WorkExperienceBulletUpdateCommandValidator : AbstractValidator<WorkExperienceBulletUpdateCommand>
{
    public WorkExperienceBulletUpdateCommandValidator()
    {
        RuleFor(c => c.WorkExperienceId).GreaterThan(0);
        RuleFor(c => c.BulletId).GreaterThan(0);
        RuleFor(c => c.Content).NotEmpty().MaximumLength(500);
    }
}
