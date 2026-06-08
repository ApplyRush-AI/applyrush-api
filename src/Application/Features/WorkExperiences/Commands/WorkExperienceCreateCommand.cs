using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.WorkExperiences.Data;
using AutoMapper;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using DTO.Enums;
using DTO.Enums.Job;
using DTO.Profile.WorkExperiences;
using Application.Common.Localization.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkExperiences.Commands;

public sealed record WorkExperienceCreateCommand(
    string JobTitle,
    string Company,
    JobType? JobType,
    string? Location,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    string? Summary
    ) : ICommand<WorkExperienceItemResponse>;

public sealed class WorkExperienceCreateCommandHandler : ICommandHandler<WorkExperienceCreateCommand, WorkExperienceItemResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkExperienceCreateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<WorkExperienceItemResponse> Handle(WorkExperienceCreateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var orderIndex = await _dbContext.WorkExperience
            .CountAsync(w => w.UserProfileId == profile.Id && w.Status != Status.Deleted, cancellationToken);

        var data = new WorkExperienceCreateData(
            profile.Id,
            command.JobTitle,
            command.Company,
            command.JobType,
            command.Location,
            command.StartDate,
            command.EndDate,
            command.IsCurrent,
            command.Summary);

        var workExperience = WorkExperience.Create(data, orderIndex);
        await _dbContext.WorkExperience.AddAsync(workExperience, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkExperienceItemResponse>(workExperience);
    }
}

public sealed class WorkExperienceCreateCommandValidator : AbstractValidator<WorkExperienceCreateCommand>
{
    public WorkExperienceCreateCommandValidator()
    {
        RuleFor(c => c.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Company).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Location).MaximumLength(200);
        RuleFor(c => c.Summary).MaximumLength(2000);
        RuleFor(c => c.EndDate)
            .Must((cmd, endDate) => !endDate.HasValue || endDate > cmd.StartDate)
            .WithLocalizationKey("workExperience.endDateBeforeStartDate.error.message")
            .When(c => !c.IsCurrent);
    }
}
