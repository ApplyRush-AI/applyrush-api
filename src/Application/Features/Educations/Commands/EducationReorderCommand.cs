using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DomainEducation = Domain.Entities.Profiles.Educations.Education;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Educations.Commands;

public sealed record EducationReorderCommand(IReadOnlyList<int> OrderedIds) : ICommand;

public sealed class EducationReorderCommandHandler : ICommandHandler<EducationReorderCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public EducationReorderCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EducationReorderCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var educations = await _dbContext.Education
            .Where(e => e.UserProfileId == profile.Id && e.Status != DTO.Enums.Status.Deleted)
            .ToListAsync(cancellationToken);

        for (int i = 0; i < command.OrderedIds.Count; i++)
        {
            var education = educations.FirstOrDefault(e => e.Id == command.OrderedIds[i])
                ?? throw NotFoundException.New<DomainEducation>(command.OrderedIds[i]);
            education.SetOrderIndex(i);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class EducationReorderCommandValidator : AbstractValidator<EducationReorderCommand>
{
    public EducationReorderCommandValidator()
    {
        RuleFor(c => c.OrderedIds).NotEmpty();
    }
}
