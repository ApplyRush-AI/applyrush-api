using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferUnhideCommand(int JobId) : ICommand;

public sealed class JobOfferUnhideCommandHandler : ICommandHandler<JobOfferUnhideCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public JobOfferUnhideCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(JobOfferUnhideCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var hiddenJob = await _dbContext.UserHiddenJob
            .FirstOrDefaultAsync(h => h.UserId == userId && h.JobId == command.JobId, cancellationToken);

        if (hiddenJob == null)
            return;

        _dbContext.UserHiddenJob.Remove(hiddenJob);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
