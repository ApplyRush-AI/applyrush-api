using Application.Common.Interfaces.Services;
using Application.Features.JobOffers.Models;
using Domain.Interfaces;
using DTO.Enums.Job;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Services.JobSync;

public sealed class JSearchJobProvider : IJobProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JSearchOptions _options;
    private readonly IDateTime _dateTime;
    private readonly ILogger<JSearchJobProvider> _logger;

    public JSearchJobProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<JSearchOptions> options,
        IDateTime dateTime,
        ILogger<JSearchJobProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _dateTime = dateTime;
        _logger = logger;
    }

    public async Task<IReadOnlyList<JobListingData>> FetchJobsAsync(string query, int page, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("JSearch");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-rapidapi-key", _options.ApiKey);
        client.DefaultRequestHeaders.Add("x-rapidapi-host", "jsearch.p.rapidapi.com");

        var url = $"search-v2?query={Uri.EscapeDataString(query)}&page={page}&num_pages=1&country=us&date_posted=all";
        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var node = JsonNode.Parse(json);
        var data = node?["data"]?["jobs"] as JsonArray;

        if (data == null || data.Count == 0)
            return [];

        var results = new List<JobListingData>();

        foreach (var item in data)
        {
            if (item == null) continue;

            try
            {
                results.Add(MapJob(item));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[JSearch] Failed to map job item. Skipping.");
            }
        }

        return results;
    }

    private JobListingData MapJob(JsonNode item)
    {
        var jobId = item["job_id"]?.GetValue<string>() ?? Guid.NewGuid().ToString();
        var title = item["job_title"]?.GetValue<string>() ?? "Untitled";
        var company = item["employer_name"]?.GetValue<string>() ?? "Unknown";
        var logoUrl = item["employer_logo"]?.GetValue<string>();
        var description = item["job_description"]?.GetValue<string>() ?? string.Empty;
        var applyLink = item["job_apply_link"]?.GetValue<string>() ?? string.Empty;
        var industry = item["employer_company_type"]?.GetValue<string>();

        var city = item["job_city"]?.GetValue<string>();
        var state = item["job_state"]?.GetValue<string>();
        var country = item["job_country"]?.GetValue<string>();
        var location = BuildLocation(city, state, country);

        var isRemote = item["job_is_remote"]?.GetValue<bool>() ?? false;
        var workModel = isRemote ? WorkModel.Remote : WorkModel.Onsite;

        var employmentType = item["job_employment_type"]?.GetValue<string>() ?? "FULLTIME";
        var jobType = MapJobType(employmentType);

        var requiredExpMonths = item["job_required_experience"]?["required_experience_in_months"]?.GetValue<int?>();
        var yearsRequired = requiredExpMonths.HasValue ? (int?)Math.Round(requiredExpMonths.Value / 12.0) : null;
        var experienceLevel = MapExperienceLevel(yearsRequired);

        var salaryMin = item["job_min_salary"]?.GetValue<decimal?>();
        var salaryMax = item["job_max_salary"]?.GetValue<decimal?>();
        var currency = item["job_salary_currency"]?.GetValue<string>();

        var highlights = item["job_highlights"];
        var qualifications = JoinList(highlights?["Qualifications"] as JsonArray);
        var responsibilities = JoinList(highlights?["Responsibilities"] as JsonArray);
        var benefits = JoinList(item["job_benefits_strings"] as JsonArray)
            ?? JoinList(highlights?["Benefits"] as JsonArray);

        var requiredSkillsArray = item["job_required_skills"] as JsonArray;
        string? requiredSkillsJson = null;
        if (requiredSkillsArray != null && requiredSkillsArray.Count > 0)
        {
            var skills = requiredSkillsArray
                .Select(s => s?.GetValue<string>())
                .Where(s => s != null)
                .ToList();
            requiredSkillsJson = JsonSerializer.Serialize(skills);
        }

        DateTime postedAt;
        var postedAtRaw = item["job_posted_at_datetime_utc"]?.GetValue<string>();
        postedAt = DateTime.TryParse(postedAtRaw, out var parsed) ? parsed.ToUniversalTime() : _dateTime.Now;

        return new JobListingData
        {
            ExternalId = jobId,
            Source = JobSource.JSearch,
            Title = title,
            Company = company,
            CompanyLogoUrl = logoUrl,
            Description = description,
            About = null,
            Responsibilities = responsibilities,
            Requirements = qualifications,
            Benefits = benefits,
            RequiredSkills = requiredSkillsJson,
            Industry = industry,
            Location = location,
            WorkModel = workModel,
            JobType = jobType,
            ExperienceLevel = experienceLevel,
            SalaryMin = salaryMin,
            SalaryMax = salaryMax,
            Currency = currency,
            YearsRequired = yearsRequired,
            ApplicantCount = null,
            PostedAt = postedAt,
            ExpiresAt = null,
            ApplyUrl = applyLink,
            H1BSupported = false,
            AiSummary = null,
            LastSyncedAt = _dateTime.Now
        };
    }

    private static string BuildLocation(string? city, string? state, string? country)
    {
        var parts = new[] { city, state, country }.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(", ", parts).Trim() is { Length: > 0 } loc ? loc : "Unknown";
    }

    private static JobType MapJobType(string employmentType) => employmentType.ToUpperInvariant() switch
    {
        "PARTTIME" or "PART_TIME" => JobType.PartTime,
        "CONTRACTOR" or "CONTRACT" => JobType.Contract,
        "INTERN" or "INTERNSHIP" => JobType.Internship,
        _ => JobType.FullTime
    };

    private static ExperienceLevel MapExperienceLevel(int? years) => years switch
    {
        null or 0 => ExperienceLevel.EntryLevel,
        <= 2 => ExperienceLevel.EntryLevel,
        <= 4 => ExperienceLevel.MidLevel,
        <= 7 => ExperienceLevel.Senior,
        <= 10 => ExperienceLevel.Lead,
        _ => ExperienceLevel.Executive
    };

    private static string? JoinList(JsonArray? array)
    {
        if (array == null || array.Count == 0) return null;
        var items = array.Select(i => i?.GetValue<string>()).Where(s => s != null);
        return string.Join("\n", items);
    }
}
