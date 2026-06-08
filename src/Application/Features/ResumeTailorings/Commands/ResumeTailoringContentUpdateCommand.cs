using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities.Tailoring.ResumeTailorings;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ResumeTailorings.Commands;

public sealed record ResumeTailoringContentUpdateCommand(int Id, string TailoredContent) : ICommand<ResumeTailoringResponse>;

public sealed class ResumeTailoringContentUpdateCommandHandler : ICommandHandler<ResumeTailoringContentUpdateCommand, ResumeTailoringResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ResumeTailoringContentUpdateCommandHandler(
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

    public async Task<ResumeTailoringResponse> Handle(ResumeTailoringContentUpdateCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var tailoring = await _dbContext.ResumeTailoring
            .FirstOrDefaultAsync(t => t.Id == command.Id && t.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(ResumeTailoring), command.Id);

        tailoring.UpdateContent(command.TailoredContent);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ResumeTailoringResponse>(tailoring);
    }
}
