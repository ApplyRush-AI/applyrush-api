using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Features.Skills.Queries;

public sealed record SkillSearchQuery(string? Query, int Take = 20) : IQuery<IReadOnlyList<string>>;

public sealed class SkillSearchQueryHandler : IQueryHandler<SkillSearchQuery, IReadOnlyList<string>>
{
    private const int MaxTake = 50;

    private readonly IApplicationDbContext _dbContext;

    public SkillSearchQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<string>> Handle(SkillSearchQuery query, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(query.Take, 1, MaxTake);
        var known = await LoadKnownSkillsAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(query.Query))
            return known.OrderBy(s => s).Take(take).ToList();

        var term = query.Query.Trim();

        // Prefix matches are the most useful for an autocomplete, so they come first.
        return known
            .Where(s => s.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(s => s.StartsWith(term, StringComparison.OrdinalIgnoreCase))
            .ThenBy(s => s)
            .Take(take)
            .ToList();
    }

    // Skills are not a curated catalogue: they are collected from what is already stored -
    // the skills required by job listings plus the skills users have on their profiles.
    private async Task<List<string>> LoadKnownSkillsAsync(CancellationToken cancellationToken)
    {
        var jobSkillsJson = await _dbContext.JobListing
            .AsNoTracking()
            .Where(j => j.Status == Status.Active && j.RequiredSkills != null && j.RequiredSkills != "")
            .Select(j => j.RequiredSkills!)
            .Distinct()
            .ToListAsync(cancellationToken);

        var profileSkills = await _dbContext.UserSkill
            .AsNoTracking()
            .Select(s => s.Name)
            .Distinct()
            .ToListAsync(cancellationToken);

        return jobSkillsJson
            .SelectMany(ParseSkills)
            .Concat(profileSkills)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .DistinctBy(s => s.ToLowerInvariant())
            .ToList();
    }

    private static IEnumerable<string> ParseSkills(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
