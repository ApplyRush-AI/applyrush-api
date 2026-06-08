using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums;
using DTO.Enums.Profile.Education;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Educations.Commands;

public sealed record EducationUpdateCommand(
    int Id,
    string School,
    string? Major,
    DegreeType DegreeType,
    decimal? Gpa,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool IsCurrent
    ) : ICommand, IEducationUpdateData;

public sealed class EducationUpdateCommandHandler : ICommandHandler<EducationUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public EducationUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EducationUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var education = await _dbContext.Education
            .FirstOrDefaultAsync(e => e.Id == command.Id && e.UserProfileId == profile.Id && e.Status != Status.Deleted, cancellationToken)
            ?? throw NotFoundException.New<Education>(command.Id);

        education.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class EducationUpdateCommandValidator : AbstractValidator<EducationUpdateCommand>
{
    public EducationUpdateCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.School).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Major).MaximumLength(200);
        RuleFor(c => c.Gpa).InclusiveBetween(0, 4).When(c => c.Gpa.HasValue);
    }
}
