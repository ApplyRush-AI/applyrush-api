using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DTO.Enums;

namespace Application.Features.WorkExperiences.Commands;

public sealed record WorkExperienceDeleteCommand(int Id) : ICommand;

public sealed class WorkExperienceDeleteCommandHandler : ICommandHandler<WorkExperienceDeleteCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public WorkExperienceDeleteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(WorkExperienceDeleteCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var workExperience = await _dbContext.WorkExperience
            .FirstOrDefaultAsync(w => w.Id == command.Id && w.UserProfileId == profile.Id && w.Status != Status.Deleted, cancellationToken)
            ?? throw NotFoundException.New<WorkExperience>(command.Id);

        workExperience.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class WorkExperienceDeleteCommandValidator : AbstractValidator<WorkExperienceDeleteCommand>
{
    public WorkExperienceDeleteCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}
