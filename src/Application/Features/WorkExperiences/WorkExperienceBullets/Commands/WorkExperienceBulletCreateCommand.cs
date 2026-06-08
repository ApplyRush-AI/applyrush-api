using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.WorkExperiences.WorkExperienceBullets.Data;
using AutoMapper;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using DTO.Profile.WorkExperiences;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DTO.Profile.WorkExperiences.WorkExperienceBullets;
using DTO.Enums;
using Domain.Entities.Profiles.WorkExpeciences;

namespace Application.Features.WorkExperiences.WorkExperienceBullets.Commands;

public sealed record WorkExperienceBulletCreateCommand(int WorkExperienceId, string Content) : ICommand<WorkExperienceBulletResponse>;

public sealed class WorkExperienceBulletCreateCommandHandler : ICommandHandler<WorkExperienceBulletCreateCommand, WorkExperienceBulletResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WorkExperienceBulletCreateCommandHandler(
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

    public async Task<WorkExperienceBulletResponse> Handle(WorkExperienceBulletCreateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var workExperience = await _dbContext.WorkExperience
            .FirstOrDefaultAsync(w => w.Id == command.WorkExperienceId && w.UserProfileId == profile.Id && w.Status != Status.Deleted, cancellationToken)
            ?? throw NotFoundException.New<WorkExperience>(command.WorkExperienceId);

        var orderIndex = await _dbContext.WorkExperienceBullet
            .CountAsync(b => b.WorkExperienceId == command.WorkExperienceId, cancellationToken);

        var data = new WorkExperienceBulletCreateData(command.WorkExperienceId, command.Content);
        var bullet = WorkExperienceBullet.Create(data, orderIndex);
        await _dbContext.WorkExperienceBullet.AddAsync(bullet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WorkExperienceBulletResponse>(bullet);
    }
}

public sealed class WorkExperienceBulletCreateCommandValidator : AbstractValidator<WorkExperienceBulletCreateCommand>
{
    public WorkExperienceBulletCreateCommandValidator()
    {
        RuleFor(c => c.WorkExperienceId).GreaterThan(0);
        RuleFor(c => c.Content).NotEmpty().MaximumLength(500);
    }
}
