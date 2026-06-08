using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using Domain.Entities.Jobs.JobApplications;
using DTO.Enums.JobApplication;
using DTO.JobApplications;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobApplications.Commands;

public sealed record JobApplicationUpdateNoteCommand(int JobId, string Note) : ICommand<JobApplicationResponse>;

public sealed class JobApplicationUpdateNoteCommandHandler : ICommandHandler<JobApplicationUpdateNoteCommand, JobApplicationResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationUpdateNoteCommandHandler(
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

    public async Task<JobApplicationResponse> Handle(JobApplicationUpdateNoteCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var application = await _dbContext.JobApplication
            .FirstOrDefaultAsync(a => a.UserId == userId && a.JobId == command.JobId && a.Status != ApplicationStatus.Deleted, cancellationToken)
            ?? throw NotFoundException.New<JobApplication>();

        application.UpdateNote(command.Note);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<JobApplicationResponse>(application);
    }
}

public sealed class JobApplicationUpdateNoteCommandValidator : AbstractValidator<JobApplicationUpdateNoteCommand>
{
    public JobApplicationUpdateNoteCommandValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
