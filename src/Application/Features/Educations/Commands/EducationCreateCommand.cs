using Application.Common.Localization.Extensions;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Educations.Data;
using AutoMapper;
using Domain.Entities.Profiles.UserProfiles;
using DomainEducation = Domain.Entities.Profiles.Educations.Education;
using DTO.Enums.Profile.Education;
using DTO.Profile.Educations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Educations.Commands;

public sealed record EducationCreateCommand(
    string School,
    string? Major,
    DegreeType DegreeType,
    decimal? Gpa,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool IsCurrent,
    GpaScale GpaScale = GpaScale.FourPoint
    ) : ICommand<EducationItemResponse>;

public sealed class EducationCreateCommandHandler : ICommandHandler<EducationCreateCommand, EducationItemResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EducationCreateCommandHandler(
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

    public async Task<EducationItemResponse> Handle(EducationCreateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var orderIndex = await _dbContext.Education
            .CountAsync(e => e.UserProfileId == profile.Id && e.Status != DTO.Enums.Status.Deleted, cancellationToken);

        var data = new EducationCreateData(
            profile.Id,
            command.School,
            command.Major,
            command.DegreeType,
            command.Gpa,
            command.GpaScale,
            command.StartDate,
            command.EndDate,
            command.IsCurrent);

        var education = DomainEducation.Create(data, orderIndex);
        await _dbContext.Education.AddAsync(education, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EducationItemResponse>(education);
    }
}

public sealed class EducationCreateCommandValidator : AbstractValidator<EducationCreateCommand>
{
    public EducationCreateCommandValidator()
    {
        RuleFor(c => c.School).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Major).MaximumLength(200);
        RuleFor(c => c.DegreeType).IsInEnum();
        RuleFor(c => c.GpaScale).IsInEnum();
        RuleFor(c => c.Gpa).GreaterThanOrEqualTo(0).When(c => c.Gpa.HasValue);
        RuleFor(c => c.Gpa)
            .Must((cmd, gpa) => gpa <= cmd.GpaScale.MaxValue())
            .When(c => c.Gpa.HasValue && c.GpaScale.MaxValue() != null)
            .WithLocalizationKey("education.gpa.scaleRange.message", c => new object[] { c.GpaScale.MaxValue()! });
    }
}
