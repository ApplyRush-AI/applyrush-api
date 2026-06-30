using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Interfaces;
using DTO.Enums;
using DTO.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Commands;

public sealed record JobFunctionLinkAllCommand : ICommand<int>;

public sealed class JobFunctionLinkAllCommandHandler : ICommandHandler<JobFunctionLinkAllCommand, int>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public JobFunctionLinkAllCommandHandler(IApplicationDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(JobFunctionLinkAllCommand command, CancellationToken cancellationToken)
    {
        var lookup = await LoadActiveLeafJobFunctionsAsync(cancellationToken);
        return await LinkAllAsync(lookup, cancellationToken);
    }

    private async Task<List<ListItemBaseResponse>> LoadActiveLeafJobFunctionsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.JobFunction
            .Where(f => f.Status == Status.Active && !f.Children.Any())
            .Select(f => new ListItemBaseResponse { Id = f.Id, Name = f.Name })
            .ToListAsync(cancellationToken);
    }

    private async Task<int> LinkAllAsync(List<ListItemBaseResponse> lookup, CancellationToken cancellationToken)
    {
        var listings = await _dbContext.JobListing
            .Include(j => j.JobFunctions)
            .ToListAsync(cancellationToken);

        foreach (var listing in listings)
            listing.SetJobFunctions(MatchJobFunctions(listing.Title, lookup));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return listings.Count;
    }

    private static IEnumerable<int> MatchJobFunctions(string title, List<ListItemBaseResponse> jobFunctions)
    {
        var titleLower = title.ToLowerInvariant();
        return jobFunctions
            .Where(f => titleLower.Contains(f.Name.ToLowerInvariant()))
            .Select(f => f.Id);
    }
}
