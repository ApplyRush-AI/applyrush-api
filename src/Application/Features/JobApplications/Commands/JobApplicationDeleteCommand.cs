using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Jobs.JobApplications;
using DTO.Enums.JobApplication;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobApplications.Commands;

public sealed record JobApplicationDeleteCommand(int JobId) : ICommand;

public sealed class JobApplicationDeleteCommandHandler : ICommandHandler<JobApplicationDeleteCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobApplicationDeleteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobApplicationDeleteCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var application = await _dbContext.JobApplication
            .FirstOrDefaultAsync(a => a.UserId == userId && a.JobId == command.JobId && a.Status != ApplicationStatus.Deleted, cancellationToken)
            ?? throw NotFoundException.New<JobApplication>();

        application.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
