using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SavedJobOffers.Commands;

public sealed record JobOfferUnsaveCommand(int JobId) : ICommand;

public sealed class JobOfferUnsaveCommandHandler : ICommandHandler<JobOfferUnsaveCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobOfferUnsaveCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobOfferUnsaveCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var saved = await _dbContext.UserSavedJob
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JobId == command.JobId && s.Status == Status.Active, cancellationToken);

        if (saved is null) return;

        saved.Delete();
        _dbContext.UserSavedJob.Remove(saved);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
