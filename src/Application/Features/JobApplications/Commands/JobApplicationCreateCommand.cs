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
    int UserId, 
    int JobId)
    : ICommand<JobApplicationResponse>, IJobApplicationInsertData;

public sealed class JobApplicationCreateCommandHandler : ICommandHandler<JobApplicationCreateCommand, JobApplicationResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationCreateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobApplicationResponse> Handle(JobApplicationCreateCommand command, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.JobApplication
            .FirstOrDefaultAsync(a => a.UserId == command.UserId && a.JobId == command.JobId && a.Status != ApplicationStatus.Deleted, cancellationToken);

        if (existing is not null)
            return _mapper.Map<JobApplicationResponse>(existing);

        var application = JobApplication.Create(command);
        _dbContext.JobApplication.Add(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobApplicationResponse>(application);
    }
}
