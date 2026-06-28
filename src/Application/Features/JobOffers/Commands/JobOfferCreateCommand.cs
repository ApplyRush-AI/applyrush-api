using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Localization;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.JobOffers.Data;
using AutoMapper;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Interfaces;
using DTO.Enums.Job;
using DTO.JobOffers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Application.Common.Localization.Extensions;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferCreateCommand(
    int UserId,
    string Title,
    string Company,
    string? CompanyLogoUrl,
    string Description,
    string? About,
    string? Responsibilities,
    string? Requirements,
    string? Benefits,
    string? RequiredSkills,
    string? Industry,
    string Location,
    WorkModel WorkModel,
    JobType JobType,
    ExperienceLevel ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Currency,
    int? YearsRequired,
    int? ApplicantCount,
    DateTime PostedAt,
    DateTime? ExpiresAt,
    string ApplyUrl,
    bool H1BSupported,
    string? AiSummary
) : ICommand<JobOfferDetailResponse>;

public sealed class JobOfferCreateCommandHandler : ICommandHandler<JobOfferCreateCommand, JobOfferDetailResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTime _dateTime;
    private readonly ILocalizationService _localizationService;

    public JobOfferCreateCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IDateTime dateTime,
        ILocalizationService localizationService)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTime = dateTime;
        _localizationService = localizationService;
    }

    public async Task<JobOfferDetailResponse> Handle(JobOfferCreateCommand command, CancellationToken cancellationToken)
    {
        await EnsureUserExistsAsync(command.UserId, cancellationToken);

        var job = CreateJobListing(command);
        _dbContext.JobListing.Add(job);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await CreateUserMatchAsync(command.UserId, job.Id, cancellationToken);

        return _mapper.Map<JobOfferDetailResponse>(job);
    }

    private async Task EnsureUserExistsAsync(int userId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.User
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (!exists)
            throw new NotFoundException(_localizationService.GetValue("user.notFound.error.message"));
    }

    private JobListing CreateJobListing(JobOfferCreateCommand command)
    {
        var data = _mapper.Map<JobOfferCreateData>(command) with
        {
            ExternalId = $"manual-{Guid.NewGuid():N}",
            Source = JobSource.Manual,
            LastSyncedAt = _dateTime.Now
        };

        return JobListing.Create(data);
    }

    private async Task CreateUserMatchAsync(int userId, int jobId, CancellationToken cancellationToken)
    {
        var match = UserJobMatch.Create(new JobOfferManualMatchData(userId, jobId, _dateTime.Now));
        _dbContext.UserJobMatch.Add(match);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class JobOfferCreateCommandValidator : AbstractValidator<JobOfferCreateCommand>
{
    public JobOfferCreateCommandValidator()
    {
        RuleFor(c => c.UserId)
            .GreaterThan(0)
            .WithLocalizationKey("jobOffer.userId.required.message");

        RuleFor(c => c.Title)
            .NotEmpty()
            .WithLocalizationKey("jobOffer.title.required.message");

        RuleFor(c => c.Company)
            .NotEmpty()
            .WithLocalizationKey("jobOffer.company.required.message");

        RuleFor(c => c.Description)
            .NotEmpty()
            .WithLocalizationKey("jobOffer.description.required.message");

        RuleFor(c => c.Location)
            .NotEmpty()
            .WithLocalizationKey("jobOffer.location.required.message");

        RuleFor(c => c.ApplyUrl)
            .NotEmpty()
            .WithLocalizationKey("jobOffer.applyUrl.required.message");
    }
}
