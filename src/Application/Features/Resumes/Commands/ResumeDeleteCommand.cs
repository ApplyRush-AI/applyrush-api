using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Localization;
using Domain.Entities.Resumes.Resume;
using Domain.Interfaces;
using DTO.Enums;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeDeleteCommand(int ResumeId) : ICommand;

public sealed class ResumeDeleteCommandHandler : ICommandHandler<ResumeDeleteCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaStorage _mediaStorage;
    private readonly ILocalizationService _localizationService;

    public ResumeDeleteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMediaStorage mediaStorage,
        ILocalizationService localizationService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mediaStorage = mediaStorage;
        _localizationService = localizationService;
    }

    public async Task Handle(ResumeDeleteCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => r.Id == command.ResumeId && r.UserId == userId && r.Status == Status.Active, cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        if (resume.IsPrimary)
        {
            var hasOtherActiveResumes = await _dbContext.Resume
                .AnyAsync(r => r.UserId == userId && r.Id != command.ResumeId && r.Status == Status.Active, cancellationToken);

            if (hasOtherActiveResumes)
            {
                var message = _localizationService.GetValue("resume.primaryDelete.error.message");
                throw new ValidationException(
                    new[] { new ValidationFailure("resume", message) });
            }
        }

        resume.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
