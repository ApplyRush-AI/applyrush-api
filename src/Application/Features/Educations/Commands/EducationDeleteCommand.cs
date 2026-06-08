using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DomainEducation = Domain.Entities.Profiles.Educations.Education;

namespace Application.Features.Educations.Commands;

public sealed record EducationDeleteCommand(int Id) : ICommand;

public sealed class EducationDeleteCommandHandler : ICommandHandler<EducationDeleteCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public EducationDeleteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EducationDeleteCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var education = await _dbContext.Education
            .FirstOrDefaultAsync(e => e.Id == command.Id && e.UserProfileId == profile.Id && e.Status != DTO.Enums.Status.Deleted, cancellationToken)
            ?? throw NotFoundException.New<DomainEducation>(command.Id);

        education.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class EducationDeleteCommandValidator : AbstractValidator<EducationDeleteCommand>
{
    public EducationDeleteCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}
