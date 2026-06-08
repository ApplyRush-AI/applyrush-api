using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using DTO.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkExperiences.Commands;

public sealed record WorkExperienceReorderCommand(IReadOnlyList<int> OrderedIds) : ICommand;

public sealed class WorkExperienceReorderCommandHandler : ICommandHandler<WorkExperienceReorderCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public WorkExperienceReorderCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(WorkExperienceReorderCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var workExperiences = await _dbContext.WorkExperience
            .Where(w => w.UserProfileId == profile.Id && w.Status != Status.Deleted)
            .ToListAsync(cancellationToken);

        for (int i = 0; i < command.OrderedIds.Count; i++)
        {
            var workExperience = workExperiences.FirstOrDefault(w => w.Id == command.OrderedIds[i])
                ?? throw NotFoundException.New<WorkExperience>(command.OrderedIds[i]);
            workExperience.SetOrderIndex(i);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class WorkExperienceReorderCommandValidator : AbstractValidator<WorkExperienceReorderCommand>
{
    public WorkExperienceReorderCommandValidator()
    {
        RuleFor(c => c.OrderedIds).NotEmpty();
    }
}
