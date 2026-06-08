using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Profile.UserProfile;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.Commands;

public sealed record ProfileUpdateCommand(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Country,
    string? City,
    string? County,
    string? PostalCode,
    string? AddressLine1,
    string? LinkedInUrl,
    string? GitHubUrl,
    string? WebsiteUrl,
    string? Title,
    string? Bio,
    LocationMode LocationMode,
    string? Location,
    bool OpenToRemote,
    bool H1BSponsorship
    ) : ICommand, IUserProfileUpdateData;

public sealed class ProfileUpdateCommandHandler : ICommandHandler<ProfileUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ProfileUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        profile.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ProfileUpdateCommandValidator : AbstractValidator<ProfileUpdateCommand>
{
    public ProfileUpdateCommandValidator()
    {
        RuleFor(c => c.Phone).MaximumLength(20);
        RuleFor(c => c.Title).MaximumLength(100);
        RuleFor(c => c.Bio).MaximumLength(2000);
        RuleFor(c => c.LinkedInUrl).MaximumLength(500);
        RuleFor(c => c.GitHubUrl).MaximumLength(500);
        RuleFor(c => c.WebsiteUrl).MaximumLength(500);
        RuleFor(c => c.Country).MaximumLength(100);
        RuleFor(c => c.City).MaximumLength(100);
        RuleFor(c => c.County).MaximumLength(100);
        RuleFor(c => c.PostalCode).MaximumLength(20);
        RuleFor(c => c.AddressLine1).MaximumLength(200);
        RuleFor(c => c.Location).MaximumLength(200);
        RuleFor(c => c.LocationMode).IsInEnum();
    }
}
