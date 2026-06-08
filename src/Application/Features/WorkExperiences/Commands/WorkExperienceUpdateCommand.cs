using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExperiences;
using DTO.Enums;
using DTO.Enums.Job;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkExperiences.Commands;

public sealed record WorkExperienceUpdateCommand(
    int Id,
    string JobTitle,
    string Company,
    JobType? JobType,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Summary
    ) : ICommand, IWorkExperienceUpdateData;

public sealed class WorkExperienceUpdateCommandHandler : ICommandHandler<WorkExperienceUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public WorkExperienceUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(WorkExperienceUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var workExperience = await _dbContext.WorkExperience
            .FirstOrDefaultAsync(w => w.Id == command.Id && w.UserProfileId == profile.Id && w.Status != Status.Deleted, cancellationToken)
            ?? throw NotFoundException.New<WorkExperience>(command.Id);

        workExperience.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class WorkExperienceUpdateCommandValidator : AbstractValidator<WorkExperienceUpdateCommand>
{
    public WorkExperienceUpdateCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Company).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Location).MaximumLength(200);
        RuleFor(c => c.Summary).MaximumLength(2000);
    }
}
