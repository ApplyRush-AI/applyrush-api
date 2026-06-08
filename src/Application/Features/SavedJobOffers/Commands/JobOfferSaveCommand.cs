using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.SavedJobOffers.Data;
using Domain.Entities.Jobs.UserSavedJobs;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.SavedJobOffers.Commands;

public sealed record JobOfferSaveCommand(int JobId) : ICommand;

public sealed class JobOfferSaveCommandHandler : ICommandHandler<JobOfferSaveCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobOfferSaveCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobOfferSaveCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var existing = await _dbContext.UserSavedJob
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JobId == command.JobId, cancellationToken);

        if (existing is not null)
        {
            if (existing.Status == Status.Active) return;

            existing.Delete();
            _dbContext.UserSavedJob.Remove(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            existing = null;
        }

        if (existing is null)
        {
            var saved = UserSavedJob.Create(new UserSavedJobInsertData(userId, command.JobId));
            _dbContext.UserSavedJob.Add(saved);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
