using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Ai;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Resume;
using Infrastructure.AI.Claude;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Services;

public sealed class ResumeTailoringAiService : IResumeTailoringAiService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClaudeOptions _options;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private const string TailorSystemPrompt =
        "You are a professional resume optimization expert. " +
        "Given a candidate's profile and a job listing, rewrite the work experience bullets to match the job requirements " +
        "and inject the requested keywords naturally. Score how well the profile matches the job before and after. " +
        "Return ONLY a valid JSON object — no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"ScoreBefore\": <decimal 0-100>, \"ScoreAfter\": <decimal 0-100>, \"Summary\": \"<brief summary of changes>\", " +
        "\"Changes\": [\"<one concise sentence describing each key change made>\"], " +
        "\"Experience\": [{ \"Id\": <work experience id as int>, \"Bullets\": [\"<bullet text>\"] }], " +
        "\"HighlightedSkills\": [\"<skill>\"], \"MissingSkills\": [\"<skill>\"] }";

    private const string RewriteSystemPrompt =
        "You are a resume writing expert. Rewrite the provided resume content according to the given instruction. " +
        "Return ONLY a valid JSON object — no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"Section\": \"<name of the section that was rewritten>\", \"UpdatedContent\": \"<the rewritten content>\" }";

    public ResumeTailoringAiService(
        IApplicationDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IOptions<ClaudeOptions> options)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<ResumeTailoringAiResult> TailorAsync(ResumeTailoringAiOptions options, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
                .ThenInclude(w => w.Bullets)
            .Include(p => p.Educations)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == options.UserId, cancellationToken);

        var job = await _dbContext.JobListing
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == options.JobId, cancellationToken);

        var userMessage = BuildTailorPrompt(profile, job, options);
        var responseText = await CallClaudeAsync(TailorSystemPrompt, userMessage, cancellationToken);
        return ParseTailorResult(responseText);
    }

    public async Task<ResumeTailoringAiRewriteResult> RewriteAsync(ResumeTailoringAiRewriteOptions options, CancellationToken cancellationToken)
    {
        var tailoring = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == options.TailoringId, cancellationToken);

        var currentContent = tailoring?.TailoredContent ?? "{}";
        var instruction = BuildRewriteInstruction(options);
        var userMessage = $"Current tailored resume content:\n{currentContent}\n\nInstruction: {instruction}";

        var responseText = await CallClaudeAsync(RewriteSystemPrompt, userMessage, cancellationToken);
        return ParseRewriteResult(responseText);
    }

    private static string BuildTailorPrompt(UserProfile? profile, JobListing? job, ResumeTailoringAiOptions options)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== JOB LISTING ===");
        if (job != null)
        {
            sb.AppendLine($"Title: {job.Title}");
            sb.AppendLine($"Company: {job.Company}");
            if (!string.IsNullOrWhiteSpace(job.Description))
                sb.AppendLine($"Description: {job.Description}");
            if (!string.IsNullOrWhiteSpace(job.Responsibilities))
                sb.AppendLine($"Responsibilities: {job.Responsibilities}");
            if (!string.IsNullOrWhiteSpace(job.Requirements))
                sb.AppendLine($"Requirements: {job.Requirements}");
            if (!string.IsNullOrWhiteSpace(job.RequiredSkills))
                sb.AppendLine($"Required Skills: {job.RequiredSkills}");
        }

        sb.AppendLine("\n=== CANDIDATE PROFILE ===");
        if (profile != null)
        {
            if (!string.IsNullOrWhiteSpace(profile.Title))
                sb.AppendLine($"Professional Title: {profile.Title}");
            if (!string.IsNullOrWhiteSpace(profile.Bio))
                sb.AppendLine($"Bio: {profile.Bio}");

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
                    sb.AppendLine($"  [Id:{exp.Id}] {exp.JobTitle} at {exp.Company} ({exp.StartDate:yyyy-MM} – {end})");
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
        }

        if (options.SectionsToEnhance.Count > 0)
            sb.AppendLine($"\nSections to enhance: {string.Join(", ", options.SectionsToEnhance)}");
        if (options.KeywordsToInject.Count > 0)
            sb.AppendLine($"Keywords to inject: {string.Join(", ", options.KeywordsToInject)}");

        return sb.ToString();
    }

    private static string BuildRewriteInstruction(ResumeTailoringAiRewriteOptions options) =>
        options.Action switch
        {
            AiRewriteAction.StrongerActionVerbs =>
                "Rewrite the experience bullets using stronger, more impactful action verbs " +
                "(e.g., achieved, delivered, spearheaded, optimized, reduced, increased, led).",
            AiRewriteAction.ShortenSummary =>
                "Make the content more concise. Remove filler words and redundant phrases while preserving all key information.",
            AiRewriteAction.RemoveUnrelatedSkills =>
                "Remove skills and keywords that are not relevant to the job requirements present in the tailored content.",
            AiRewriteAction.FreeForm =>
                string.IsNullOrWhiteSpace(options.FreeFormInstruction)
                    ? "Improve the content quality and clarity."
                    : options.FreeFormInstruction,
            _ => "Improve the content quality and clarity."
        };

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

    private static ResumeTailoringAiResult ParseTailorResult(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            if (node == null) return FallbackTailorResult(json);

            var experience = node["Experience"]?.AsArray()
                .Select(e => new TailoringExperienceAiItem
                {
                    Id = e?["Id"]?.GetValue<int>() ?? 0,
                    Bullets = e?["Bullets"]?.AsArray()
                        .Select(b => b?.GetValue<string>() ?? string.Empty)
                        .Where(b => !string.IsNullOrWhiteSpace(b))
                        .ToList() ?? []
                })
                .ToList() ?? [];

            var highlighted = node["HighlightedSkills"]?.AsArray()
                .Select(s => s?.GetValue<string>() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? [];

            var missing = node["MissingSkills"]?.AsArray()
                .Select(s => s?.GetValue<string>() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? [];

            var changes = node["Changes"]?.AsArray()
                .Select(s => s?.GetValue<string>() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList() ?? [];

            return new ResumeTailoringAiResult
            {
                TailoredContent = json,
                ScoreBefore = node["ScoreBefore"]?.GetValue<decimal>() ?? 0,
                ScoreAfter = node["ScoreAfter"]?.GetValue<decimal>() ?? 0,
                Summary = node["Summary"]?.GetValue<string>(),
                Experience = experience,
                HighlightedSkills = highlighted,
                MissingSkills = missing,
                Changes = changes
            };
        }
        catch
        {
            return FallbackTailorResult(json);
        }
    }

    private static ResumeTailoringAiResult FallbackTailorResult(string rawJson) => new()
    {
        TailoredContent = rawJson,
        ScoreBefore = 0,
        ScoreAfter = 0,
        Experience = [],
        HighlightedSkills = [],
        MissingSkills = []
    };

    private static ResumeTailoringAiRewriteResult ParseRewriteResult(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            return new ResumeTailoringAiRewriteResult
            {
                Section = node?["Section"]?.GetValue<string>() ?? string.Empty,
                UpdatedContent = node?["UpdatedContent"]?.GetValue<string>() ?? string.Empty
            };
        }
        catch
        {
            return new ResumeTailoringAiRewriteResult
            {
                Section = string.Empty,
                UpdatedContent = json
            };
        }
    }
}
