using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Interfaces;
using DTO.Enums.Profile.EeoData;
using DTO.Enums.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Profiles.EeoDatas.Commands;

public sealed record EeoDataUpdateCommand(
    WorkAuthorization? WorkAuthorization,
    bool? SponsorshipNeeded,
    DisabilityStatus? Disability,
    VeteranStatus? VeteranStatus,
    Gender? Gender,
    bool? IsLgbtq,
    Race? Race,
    bool? IsHispanicLatino,
    SexualOrientation? SexualOrientation
    ) : ICommand, IEeoDataUpdateData;

public sealed class EeoDataUpdateCommandHandler : ICommandHandler<EeoDataUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public EeoDataUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EeoDataUpdateCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.EeoData)
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        if (profile.EeoData == null)
        {
            var eeoData = EeoData.Create(profile.Id);
            eeoData.Update(command);
            await _dbContext.EeoData.AddAsync(eeoData, cancellationToken);
        }
        else
        {
            profile.EeoData.Update(command);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class EeoDataUpdateCommandValidator : AbstractValidator<EeoDataUpdateCommand>
{
    public EeoDataUpdateCommandValidator()
    {
        RuleFor(c => c.Race).IsInEnum().When(c => c.Race.HasValue);
        RuleFor(c => c.SexualOrientation).IsInEnum().When(c => c.SexualOrientation.HasValue);
    }
}
