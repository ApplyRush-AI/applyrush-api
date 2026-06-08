using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Settings;
using DTO.Enums.Job;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Settings.JobPreferences.Commands;

public sealed record JobPreferenceUpdateCommand(
    string? DesiredTitle,
    decimal? SalaryMin,
    string? Locations,
    WorkModel? WorkModel,
    string? IndustryWeights
    ) : ICommand, IUserJobPreferenceUpdateData;

public sealed class JobPreferenceUpdateCommandHandler : ICommandHandler<JobPreferenceUpdateCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobPreferenceUpdateCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(JobPreferenceUpdateCommand command, CancellationToken cancellationToken)
    {
        var pref = await _dbContext.UserJobPreference
            .FirstOrDefaultAsync(p => p.UserId == _currentUserService.UserId, cancellationToken)
            ?? throw NotFoundException.New<UserJobPreference>();

        pref.Update(command);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class JobPreferenceUpdateCommandValidator : AbstractValidator<JobPreferenceUpdateCommand>
{
    public JobPreferenceUpdateCommandValidator()
    {
        RuleFor(c => c.DesiredTitle).MaximumLength(200);
        RuleFor(c => c.SalaryMin).GreaterThanOrEqualTo(0).When(c => c.SalaryMin.HasValue);
    }
}
