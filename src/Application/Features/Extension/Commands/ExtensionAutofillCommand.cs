using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Features.Extension.Data;
using AutoMapper;
using Domain.Entities.Extension.ExtensionSessions;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Interfaces;
using DTO.Enums;
using DTO.Extension;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Extension.Commands;

public sealed record ExtensionAutofillCommand(string JobUrl) : ICommand<ExtensionAutofillResponse>;

public sealed class ExtensionAutofillCommandHandler : ICommandHandler<ExtensionAutofillCommand, ExtensionAutofillResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreditService _creditService;
    private readonly IMapper _mapper;

    public ExtensionAutofillCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICreditService creditService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _creditService = creditService;
        _mapper = mapper;
    }

    public async Task<ExtensionAutofillResponse> Handle(ExtensionAutofillCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var creditsRemaining = await _creditService.GetAutofillCreditsRemainingAsync(userId, cancellationToken);
        if (creditsRemaining == 0)
            throw new InsufficientCreditsException();

        var profile = await _dbContext.UserProfile
            .AsNoTracking()
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences.Where(w => w.Status != Status.Deleted).OrderBy(w => w.OrderIndex))
                .ThenInclude(w => w.Bullets.OrderBy(b => b.OrderIndex))
            .Include(p => p.Educations.Where(e => e.Status != Status.Deleted).OrderBy(e => e.OrderIndex))
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        await _creditService.DeductAutofillCreditAsync(userId, cancellationToken);

        var creditsUsed = creditsRemaining == -1 ? 0 : 1;

        var session = ExtensionSession.Create(new ExtensionSessionInsertData(userId, command.JobUrl, creditsUsed));
        _dbContext.ExtensionSession.Add(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCreditsRemaining = creditsRemaining == -1 ? -1 : creditsRemaining - 1;

        return new ExtensionAutofillResponse
        {
            SessionId = session.Id,
            CreditsRemaining = newCreditsRemaining,
            Profile = _mapper.Map<ExtensionProfileResponse>(profile)
        };
    }
}
