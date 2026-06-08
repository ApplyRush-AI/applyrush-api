using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.JobFunctions;
using DTO.Profile.JobFunctions;
using FluentValidation;

namespace Application.Features.JobFunctions.Commands;

public sealed record JobFunctionCreateCommand(string Name, int? ParentId) : ICommand<JobFunctionItemResponse>, IJobFunctionUpsertData;

public sealed class JobFunctionCreateCommandHandler : ICommandHandler<JobFunctionCreateCommand, JobFunctionItemResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobFunctionCreateCommandHandler(IApplicationDbContext dbContext, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobFunctionItemResponse> Handle(JobFunctionCreateCommand command, CancellationToken cancellationToken)
    {
        var jobFunction = JobFunction.Create(command);
        await _dbContext.JobFunction.AddAsync(jobFunction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<JobFunctionItemResponse>(jobFunction);
    }
}

public sealed class JobFunctionCreateCommandValidator : AbstractValidator<JobFunctionCreateCommand>
{
    public JobFunctionCreateCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
    }
}
