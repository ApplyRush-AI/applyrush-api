using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Jobs.JobApplications;
using DTO.Enums.JobApplication;
using DTO.JobApplications;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobApplications.Commands;

public sealed record JobApplicationCreateCommand(
    int JobId)
    : ICommand<JobApplicationResponse>;

internal sealed record JobApplicationInsertData(int UserId, int JobId) : IJobApplicationInsertData;

public sealed class JobApplicationCreateCommandHandler : ICommandHandler<JobApplicationCreateCommand, JobApplicationResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationCreateCommandHandler(
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

    public async Task<JobApplicationResponse> Handle(JobApplicationCreateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var existing = await _dbContext.JobApplication
            .FirstOrDefaultAsync(a => a.UserId == userId && a.JobId == command.JobId && a.Status != ApplicationStatus.Deleted, cancellationToken);

        if (existing is not null)
            return _mapper.Map<JobApplicationResponse>(existing);

        var data = new JobApplicationInsertData(userId, command.JobId);
        var application = JobApplication.Create(data);
        _dbContext.JobApplication.Add(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobApplicationResponse>(application);
    }
}
