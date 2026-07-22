using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.JobOffers.Commands;

public sealed record JobOfferBackfillIndustriesCommand : ICommand<int>;

public sealed class JobOfferBackfillIndustriesCommandHandler : ICommandHandler<JobOfferBackfillIndustriesCommand, int>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobOfferBackfillIndustriesCommandHandler> _logger;

    public JobOfferBackfillIndustriesCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ILogger<JobOfferBackfillIndustriesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> Handle(JobOfferBackfillIndustriesCommand command, CancellationToken cancellationToken)
    {
        var rootNameByFunctionId = await BuildRootCategoryMapAsync(cancellationToken);

        var jobs = await _dbContext.JobListing
            .Include(j => j.JobFunctions)
            .Where(j => j.Status == Status.Active
                        && (j.Industry == null || j.Industry == "")
                        && j.JobFunctions.Any())
            .ToListAsync(cancellationToken);

        var updated = 0;
        foreach (var job in jobs)
        {
            var industry = job.JobFunctions
                .Select(jf => rootNameByFunctionId.GetValueOrDefault(jf.JobFunctionId))
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));

            if (string.IsNullOrWhiteSpace(industry))
                continue;

            job.SetIndustry(industry);
            updated++;
        }

        if (updated > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[JobIndustryBackfill] Updated {Updated} of {Candidates} candidate jobs.", updated, jobs.Count);

        return updated;
    }

    // Walks every job function up to its top-level ancestor, so a job tagged with a leaf function
    // (e.g. "Backend Engineer") resolves to its category (e.g. "Software/Internet/AI").
    private async Task<Dictionary<int, string>> BuildRootCategoryMapAsync(CancellationToken cancellationToken)
    {
        var functions = await _dbContext.JobFunction
            .AsNoTracking()
            .Select(f => new { f.Id, f.ParentId, f.Name })
            .ToListAsync(cancellationToken);

        var byId = functions.ToDictionary(f => f.Id);
        var rootNameByFunctionId = new Dictionary<int, string>();

        foreach (var function in functions)
        {
            var current = function;
            var guard = 0;

            while (current.ParentId.HasValue && byId.TryGetValue(current.ParentId.Value, out var parent) && guard++ < 20)
                current = parent;

            rootNameByFunctionId[function.Id] = current.Name;
        }

        return rootNameByFunctionId;
    }
}
