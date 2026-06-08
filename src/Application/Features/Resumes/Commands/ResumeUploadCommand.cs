using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.MessageBroker;
using Application.Features.Medias.Validators;
using Application.Features.Resumes.Data;
using ByteSizeLib;
using Domain.Entities.Medias;
using Domain.Entities.Resumes.Resume;
using Domain.Interfaces;
using DTO.Enums;
using DTO.Enums.Resume;
using DTO.MessageBroker.Messages.Resumes;
using DTO.Resumes;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeUploadCommand(IFormFile File, string Name, bool SetAsPrimary) : ICommand<ResumeUploadResponse>;

public sealed class ResumeUploadCommandHandler : ICommandHandler<ResumeUploadCommand, ResumeUploadResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaStorage _mediaStorage;
    private readonly IMessagePublisher _messagePublisher;

    public ResumeUploadCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMediaStorage mediaStorage,
        IMessagePublisher messagePublisher)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mediaStorage = mediaStorage;
        _messagePublisher = messagePublisher;
    }

    public async Task<ResumeUploadResponse> Handle(ResumeUploadCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;
        var extension = Path.GetExtension(command.File.FileName).ToLowerInvariant();
        var fileType = extension == ".pdf" ? ResumeFileType.Pdf : ResumeFileType.Word;

        var hasPrimary = await _dbContext.Resume
            .AnyAsync(r => r.UserId == userId && r.IsPrimary && r.Status == Status.Active, cancellationToken);

        var isPrimary = command.SetAsPrimary || !hasPrimary;

        if (isPrimary && !command.SetAsPrimary && !hasPrimary)
            isPrimary = true;

        // Clear primary flag if we're setting a new primary
        if (isPrimary)
        {
            var currentPrimary = await _dbContext.Resume
                .Where(r => r.UserId == userId && r.IsPrimary && r.Status == Status.Active)
                .ToListAsync(cancellationToken);

            foreach (var r in currentPrimary)
                r.ClearPrimary();
        }

        var resume = Resume.Create(new ResumeInsertData(userId, command.Name, isPrimary, fileType));
        _dbContext.Resume.Add(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var mediaData = new MediaCreateData(command.File, true, 1);
        await resume.Media.Save(mediaData, resume.Id, _mediaStorage);
        _dbContext.Resume.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _messagePublisher.PublishAsync(new ResumeUploadedMessage
        {
            ResumeId = resume.Id,
            UserId = userId
        });

        return new ResumeUploadResponse
        {
            ResumeId = resume.Id,
            ParsedData = new ResumeParseDataResponse()
        };
    }
}


public sealed class ResumeUploadCommandValidator : AbstractValidator<ResumeUploadCommand>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".docx"];
    private static readonly ByteSize MaxSize = ByteSize.FromMegaBytes(5);

    public ResumeUploadCommandValidator(
        FileSizeValidator fileSizeValidator,
        FileExtensionValidator fileExtensionValidator)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.File)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => FileSizeValidatorData.FromFile(x.File, MaxSize))
                    .SetValidator(fileSizeValidator);

                RuleFor(x => FileExtensionValidatorData.FromFile(x.File, AllowedExtensions))
                    .SetValidator(fileExtensionValidator);
            });
    }
}

