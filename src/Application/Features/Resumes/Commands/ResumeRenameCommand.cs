using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Resumes.Resume;
using DTO.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeRenameCommand(
    int ResumeId, 
    string Name) : ICommand;

public sealed class ResumeRenameCommandHandler : ICommandHandler<ResumeRenameCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeRenameCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResumeRenameCommand command, CancellationToken cancellationToken)
    {
        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => 
            r.Id == command.ResumeId && 
            r.UserId == _currentUserService.UserId!.Value && 
            r.Status == Status.Active, 
            cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        resume.Rename(command.Name);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class ResumeRenameCommandValidator : AbstractValidator<ResumeRenameCommand>
{
    public ResumeRenameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
