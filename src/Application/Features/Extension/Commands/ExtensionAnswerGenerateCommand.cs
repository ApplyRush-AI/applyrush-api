using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using AutoMapper;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Extension;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Extension.Commands;

public sealed record ExtensionAnswerGenerateCommand(
    string Question,
    string? JobDescription) : ICommand<ExtensionAnswerResponse>;

public sealed class ExtensionAnswerGenerateCommandHandler : ICommandHandler<ExtensionAnswerGenerateCommand, ExtensionAnswerResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IExtensionAnswerAiService _answerAiService;
    private readonly IMapper _mapper;

    public ExtensionAnswerGenerateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IExtensionAnswerAiService answerAiService,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _answerAiService = answerAiService;
        _mapper = mapper;
    }

    public async Task<ExtensionAnswerResponse> Handle(ExtensionAnswerGenerateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var profile = await _dbContext.UserProfile
            .AsNoTracking()
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken)
            ?? throw NotFoundException.New<UserProfile>();

        var options = _mapper.Map<ExtensionAnswerAiOptions>(command)
            with { 
                UserTitle = profile.Title,
                Skills = profile.Skills.Select(s => s.Name).ToList(), 
                Summary = profile.Bio 
                };

        var result = await _answerAiService.GenerateAnswerAsync(options, cancellationToken);

        return _mapper.Map<ExtensionAnswerResponse>(result);
    }
}
