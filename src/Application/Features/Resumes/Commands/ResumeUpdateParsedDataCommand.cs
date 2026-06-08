using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Resumes;
using Domain.Entities.Resumes.Resume;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeUpdateParsedDataCommand(int ResumeId, ResumeParseData ParsedData) : ICommand;

public sealed class ResumeUpdateParsedDataCommandHandler : ICommandHandler<ResumeUpdateParsedDataCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeUpdateParsedDataCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResumeUpdateParsedDataCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => r.Id == command.ResumeId && r.UserId == userId && r.Status == Status.Active, cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        resume.SetParsedData(command.ParsedData);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
