using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.JobFunctions;
using Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobFunctions.Commands;

public sealed record JobFunctionUpdateCommand(int Id, string Name, int? ParentId) : ICommand, IJobFunctionUpsertData;

public sealed class JobFunctionUpdateCommandHandler : ICommandHandler<JobFunctionUpdateCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobFunctionUpdateCommandHandler(IApplicationDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobFunctionUpdateCommand command, CancellationToken cancellationToken)
    {
        var jobFunction = await _dbContext.JobFunction
            .FirstOrDefaultAsync(jf => jf.Id == command.Id, cancellationToken)
            ?? throw NotFoundException.New<JobFunction>(command.Id);

        jobFunction.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class JobFunctionUpdateCommandValidator : AbstractValidator<JobFunctionUpdateCommand>
{
    public JobFunctionUpdateCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
    }
}
