using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using Application.Features.ResumeTailorings.Data;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using Domain.Interfaces;
using DTO.Enums.Resume;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringProcessCommand(int TailoringId) : ICommand;

public sealed class ResumeTailoringProcessCommandHandler : ICommandHandler<ResumeTailoringProcessCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeTailoringAiService _aiService;
    private readonly IMapper _mapper;

    public ResumeTailoringProcessCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IResumeTailoringAiService aiService,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
        _mapper = mapper;
    }

    public async Task Handle(ResumeTailoringProcessCommand command, CancellationToken cancellationToken)
    {
        var tailoring = await _dbContext.ResumeTailoring
            .FirstOrDefaultAsync(t => t.Id == command.TailoringId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeTailoring), command.TailoringId);

        if (tailoring.Status != TailoringStatus.Processing)
            return;

        try
        {
            var options = _mapper.Map<ResumeTailoringAiOptions>(tailoring);
            var result = await _aiService.TailorAsync(options, cancellationToken);

            var completeData = _mapper.Map<ResumeTailoringCompleteData>(result);
            tailoring.Complete(completeData with { Status = TailoringStatus.Completed });
        }
        catch
        {
            tailoring.MarkFailed();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
