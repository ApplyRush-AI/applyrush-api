using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using DTO.Enums.Resume;
using DTO.Resumes;
using Infrastructure.AI.Claude;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Services;

public sealed class ResumeAnalysisAiService : IResumeAnalysisAiService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClaudeOptions _options;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string AnalyzeSystemPrompt =
        "You are a professional resume reviewer. Analyze the candidate profile and identify specific issues, weaknesses, and improvement opportunities. " +
        "Assign an overall grade: A (excellent, 85-100), B (good, 70-84), C (fair, 50-69), D (needs significant work, 0-49). " +
        "Return ONLY a valid JSON object - no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"Grade\": \"<A|B|C|D>\", \"Issues\": [ { " +
        "\"Id\": \"<unique string like issue-001>\", " +
        "\"Title\": \"<short issue title>\", " +
        "\"Description\": \"<detailed explanation and how to fix it>\", " +
        "\"Severity\": { \"Id\": <1=Urgent|2=Critical|3=Optional>, \"Name\": \"<Urgent|Critical|Optional>\" }, " +
        "\"Section\": \"<Summary|Experience|Education|Skills|Contact|Other>\" } ] }";

    private const string FixIssueSystemPrompt =
        "You are a professional resume writer. Given a specific resume issue and the candidate profile, rewrite or improve the affected section to resolve the issue. " +
        "Return ONLY a valid JSON object - no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"Section\": \"<section name>\", \"UpdatedContent\": \"<the corrected and improved content>\" }";

    private const string FixAllSystemPrompt =
        "You are a professional resume writer. Given all identified issues and the candidate profile, provide fixes for each issue. " +
        "Return ONLY a valid JSON array - no markdown, no explanation, no code fences. Each element: " +
        "{ \"Section\": \"<section name>\", \"UpdatedContent\": \"<the corrected content>\" }";

    public ResumeAnalysisAiService(
        IApplicationDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IOptions<ClaudeOptions> options)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<ResumeAnalysisAiResult> AnalyzeAsync(ResumeAnalysisAiOptions options, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
                .ThenInclude(w => w.Bullets)
            .Include(p => p.Educations)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == options.UserId, cancellationToken);

        var userMessage = BuildProfileSummary(profile);
        var responseText = await CallClaudeAsync(AnalyzeSystemPrompt, userMessage, cancellationToken);
        return ParseAnalysisResult(responseText);
    }

    public async Task<ResumeAnalysisFixAiResult> FixIssueAsync(ResumeAnalysisFixAiOptions options, CancellationToken cancellationToken)
    {
        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == options.AnalysisId, cancellationToken);

        var profile = analysis != null
            ? await _dbContext.UserProfile
                .Include(p => p.WorkExperiences).ThenInclude(w => w.Bullets)
                .Include(p => p.Skills)
                .Include(p => p.Educations)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == analysis.UserId, cancellationToken)
            : null;

        var issues = ParseIssues(analysis?.Issues ?? "[]");
        var issue = issues.FirstOrDefault(i => i.Id == options.IssueId);

        var userMessage = new StringBuilder();
        userMessage.AppendLine("Issue to fix:");
        if (issue != null)
        {
            userMessage.AppendLine($"  Title: {issue.Title}");
            userMessage.AppendLine($"  Section: {issue.Section}");
            userMessage.AppendLine($"  Description: {issue.Description}");
            userMessage.AppendLine($"  Severity: {issue.Severity?.Name}");
        }
        userMessage.AppendLine();
        userMessage.AppendLine("Candidate Profile:");
        userMessage.Append(BuildProfileSummary(profile));

        var responseText = await CallClaudeAsync(FixIssueSystemPrompt, userMessage.ToString(), cancellationToken);
        return ParseFixResult(responseText);
    }

    public async Task<IReadOnlyList<ResumeAnalysisFixAiResult>> FixAllIssuesAsync(ResumeAnalysisFixAllAiOptions options, CancellationToken cancellationToken)
    {
        var analysis = await _dbContext.ResumeAnalysis
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == options.AnalysisId, cancellationToken);

        var profile = analysis != null
            ? await _dbContext.UserProfile
                .Include(p => p.WorkExperiences).ThenInclude(w => w.Bullets)
                .Include(p => p.Skills)
                .Include(p => p.Educations)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == analysis.UserId, cancellationToken)
            : null;

        var issues = ParseIssues(analysis?.Issues ?? "[]");

        var userMessage = new StringBuilder();
        userMessage.AppendLine("All issues to fix:");
        foreach (var issue in issues)
        {
            userMessage.AppendLine($"  [{issue.Severity?.Name}] {issue.Title} (Section: {issue.Section})");
            userMessage.AppendLine($"    {issue.Description}");
        }
        userMessage.AppendLine();
        userMessage.AppendLine("Candidate Profile:");
        userMessage.Append(BuildProfileSummary(profile));

        var responseText = await CallClaudeAsync(FixAllSystemPrompt, userMessage.ToString(), cancellationToken);
        return ParseFixAllResults(responseText);
    }

    private static string BuildProfileSummary(Domain.Entities.Profiles.UserProfiles.UserProfile? profile)
    {
        if (profile == null) return "(no profile data available)";

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(profile.Title))
            sb.AppendLine($"Professional Title: {profile.Title}");
        if (!string.IsNullOrWhiteSpace(profile.Bio))
            sb.AppendLine($"Bio/Summary: {profile.Bio}");

        var skills = profile.Skills.Select(s => s.Name).ToList();
        if (skills.Count > 0)
            sb.AppendLine($"Skills: {string.Join(", ", skills)}");

        var experiences = profile.WorkExperiences
            .Where(w => w.Status == DTO.Enums.Status.Active)
            .OrderBy(w => w.OrderIndex)
            .ToList();

        if (experiences.Count > 0)
        {
            sb.AppendLine("\nWork Experience:");
            foreach (var exp in experiences)
            {
                var end = exp.IsCurrent ? "Present" : exp.EndDate?.ToString("yyyy-MM") ?? "";
                sb.AppendLine($"  {exp.JobTitle} at {exp.Company} ({exp.StartDate:yyyy-MM} - {end})");
                if (!string.IsNullOrWhiteSpace(exp.Summary))
                    sb.AppendLine($"    Summary: {exp.Summary}");
                foreach (var bullet in exp.Bullets.OrderBy(b => b.OrderIndex))
                    sb.AppendLine($"    - {bullet.Content}");
            }
        }

        var educations = profile.Educations
            .Where(e => e.Status == DTO.Enums.Status.Active)
            .OrderBy(e => e.OrderIndex)
            .ToList();

        if (educations.Count > 0)
        {
            sb.AppendLine("\nEducation:");
            foreach (var edu in educations)
                sb.AppendLine($"  {edu.DegreeType} in {edu.Major ?? "N/A"} from {edu.School}");
        }

        return sb.ToString();
    }

    private async Task<string> CallClaudeAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            system = systemPrompt,
            messages = new[] { new { role = "user", content = userMessage } }
        };

        var client = _httpClientFactory.CreateClient("Claude");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", _options.AnthropicVersion);

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("v1/messages", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var node = JsonNode.Parse(responseJson);
        return node?["content"]?[0]?["text"]?.GetValue<string>() ?? "{}";
    }

    private static ResumeAnalysisAiResult ParseAnalysisResult(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            if (node == null) return FallbackAnalysisResult();

            var gradeStr = node["Grade"]?.GetValue<string>() ?? "D";
            var grade = gradeStr.ToUpperInvariant() switch
            {
                "A" => ResumeAnalysisGrade.A,
                "B" => ResumeAnalysisGrade.B,
                "C" => ResumeAnalysisGrade.C,
                _ => ResumeAnalysisGrade.D
            };

            var issues = node["Issues"]?.AsArray()
                .Select(i => new ResumeAnalysisIssueResponse
                {
                    Id = i?["Id"]?.GetValue<string>() ?? Guid.NewGuid().ToString("N")[..8],
                    Title = i?["Title"]?.GetValue<string>() ?? string.Empty,
                    Description = i?["Description"]?.GetValue<string>() ?? string.Empty,
                    Severity = new DTO.Response.ListItemBaseResponse
                    {
                        Id = i?["Severity"]?["Id"]?.GetValue<int>() ?? (int)IssueSeverity.Optional,
                        Name = i?["Severity"]?["Name"]?.GetValue<string>() ?? IssueSeverity.Optional.ToString()
                    },
                    Section = i?["Section"]?.GetValue<string>()
                })
                .ToList() ?? [];

            return new ResumeAnalysisAiResult
            {
                OverallGrade = grade,
                UrgentFixCount = issues.Count(i => i.Severity?.Id == (int)IssueSeverity.Urgent),
                CriticalFixCount = issues.Count(i => i.Severity?.Id == (int)IssueSeverity.Critical),
                OptionalFixCount = issues.Count(i => i.Severity?.Id == (int)IssueSeverity.Optional),
                IssuesJson = JsonSerializer.Serialize(issues)
            };
        }
        catch
        {
            return FallbackAnalysisResult();
        }
    }

    private static ResumeAnalysisAiResult FallbackAnalysisResult() => new()
    {
        OverallGrade = ResumeAnalysisGrade.D,
        UrgentFixCount = 0,
        CriticalFixCount = 0,
        OptionalFixCount = 0,
        IssuesJson = "[]"
    };

    private static ResumeAnalysisFixAiResult ParseFixResult(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            return new ResumeAnalysisFixAiResult
            {
                Section = node?["Section"]?.GetValue<string>() ?? string.Empty,
                UpdatedContent = node?["UpdatedContent"]?.GetValue<string>() ?? string.Empty
            };
        }
        catch
        {
            return new ResumeAnalysisFixAiResult { Section = string.Empty, UpdatedContent = json };
        }
    }

    private static IReadOnlyList<ResumeAnalysisFixAiResult> ParseFixAllResults(string json)
    {
        try
        {
            var array = JsonNode.Parse(json)?.AsArray();
            if (array == null) return [];

            return array
                .Select(item => new ResumeAnalysisFixAiResult
                {
                    Section = item?["Section"]?.GetValue<string>() ?? string.Empty,
                    UpdatedContent = item?["UpdatedContent"]?.GetValue<string>() ?? string.Empty
                })
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    private static List<ResumeAnalysisIssueResponse> ParseIssues(string issuesJson)
    {
        try
        {
            return JsonSerializer.Deserialize<List<ResumeAnalysisIssueResponse>>(issuesJson, _jsonOptions) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
