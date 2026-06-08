using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.UserSkills;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.UserSkills.Commands;

public sealed record UserSkillsReplaceCommand(
    IReadOnlyList<string> Skills
    ) : ICommand;

public sealed class UserSkillsReplaceCommandHandler : ICommandHandler<UserSkillsReplaceCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UserSkillsReplaceCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserSkillsReplaceCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        _dbContext.UserSkill.RemoveRange(profile.Skills);

        var newSkills = command.Skills
            .Select((name, idx) => UserSkill.Create(profile.Id, name, idx));
        await _dbContext.UserSkill.AddRangeAsync(newSkills, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class UserSkillsReplaceCommandValidator : AbstractValidator<UserSkillsReplaceCommand>
{
    public UserSkillsReplaceCommandValidator()
    {
        RuleFor(c => c.Skills).NotNull();
        RuleForEach(c => c.Skills).NotEmpty().MaximumLength(100);
    }
}
