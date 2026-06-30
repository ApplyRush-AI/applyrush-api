using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Features.JobOffers.Data;
using Domain.Entities.Jobs.UserHiddenJobs;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferHideCommand(int JobId) : ICommand;

public sealed class JobOfferHideCommandHandler : ICommandHandler<JobOfferHideCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public JobOfferHideCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(JobOfferHideCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var alreadyHidden = await _dbContext.UserHiddenJob
            .AnyAsync(h => h.UserId == userId && h.JobId == command.JobId, cancellationToken);

        if (alreadyHidden)
            return;

        _dbContext.UserHiddenJob.Add(UserHiddenJob.Create(new UserHiddenJobInsertData(userId, command.JobId)));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
