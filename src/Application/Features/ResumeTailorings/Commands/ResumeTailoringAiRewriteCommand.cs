using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using DTO.Enums.Resume;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringAiRewriteCommand(
    int Id,
    AiRewriteAction Action,
    string? FreeFormInstruction) : ICommand<ResumeTailoringAiRewriteResponse>;

public sealed class ResumeTailoringAiRewriteCommandHandler : ICommandHandler<ResumeTailoringAiRewriteCommand, ResumeTailoringAiRewriteResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IResumeTailoringAiService _aiService;
    private readonly IMapper _mapper;

    public ResumeTailoringAiRewriteCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IResumeTailoringAiService aiService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _aiService = aiService;
        _mapper = mapper;
    }

    public async Task<ResumeTailoringAiRewriteResponse> Handle(ResumeTailoringAiRewriteCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var tailoring = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == command.Id && t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeTailoring), command.Id);

        var options = _mapper.Map<ResumeTailoringAiRewriteOptions>(tailoring);
        var result = await _aiService.RewriteAsync(options with { Action = command.Action, FreeFormInstruction = command.FreeFormInstruction }, cancellationToken);

        return _mapper.Map<ResumeTailoringAiRewriteResponse>(result);
    }
}
