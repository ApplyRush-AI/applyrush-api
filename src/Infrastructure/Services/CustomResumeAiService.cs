using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.CustomResume;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums;
using DTO.Enums.Resume;
using DTO.Resumes;
using Infrastructure.AI.Claude;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Services;

public sealed class CustomResumeAiService : ICustomResumeAiService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClaudeOptions _options;

    private const string GenerateSystemPrompt =
        "You are a professional resume optimization expert. " +
        "Given a candidate's profile and a job listing, rewrite the work experience bullets to match the job requirements " +
        "and inject the requested keywords naturally. Score how well the profile matches the job before and after. " +
        "Return ONLY a valid JSON object — no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"ScoreBefore\": <decimal 0-100>, \"ScoreAfter\": <decimal 0-100>, \"Summary\": \"<tailored professional summary>\", " +
        "\"Changes\": [\"<one concise sentence describing each key change made>\"], " +
        "\"Experience\": [{ \"Id\": <work experience id as int>, \"Bullets\": [\"<bullet text>\"] }], " +
        "\"Skills\": [\"<job-relevant skill>\"] }";

    private const string RewriteSystemPrompt =
        "You are a resume writing expert. Rewrite the provided tailored resume content according to the given instruction. " +
        "Preserve every work experience Id exactly. Do not invent jobs or education. " +
        "Return ONLY a valid JSON object — no markdown, no explanation, no code fences. Use exactly these fields: " +
        "{ \"Summary\": \"<updated summary>\", \"Skills\": [\"<skill>\"], " +
        "\"Experience\": [{ \"Id\": <work experience id as int>, \"Bullets\": [\"<bullet text>\"] }] }";

    public CustomResumeAiService(
        IApplicationDbContext dbContext,
        IHttpClientFactory httpClientFactory,
        IOptions<ClaudeOptions> options)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<CustomResumeAiResult> GenerateAsync(CustomResumeGenerateOptions options, CancellationToken cancellationToken)
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

        var experienceFacts = BuildExperienceFacts(profile);
        var education = BuildEducation(profile);

        var userMessage = BuildGeneratePrompt(profile, job, options);
        var responseText = await CallClaudeAsync(GenerateSystemPrompt, userMessage, cancellationToken);
        var node = ParseJsonSafe(responseText);

        var content = Assemble(
            node?["Summary"]?.GetValue<string>(),
            ReadStringArray(node?["Skills"]),
            experienceFacts,
            education,
            ReadBulletsByExperienceId(node));

        return new CustomResumeAiResult
        {
            Content = content,
            ScoreBefore = node?["ScoreBefore"]?.GetValue<decimal>() ?? 0,
            ScoreAfter = node?["ScoreAfter"]?.GetValue<decimal>() ?? 0,
            Changes = ReadStringArray(node?["Changes"])
        };
    }

    public async Task<TailoredResumeContent> RewriteAsync(TailoredResumeContent current, AiRewriteAction action, string? freeFormInstruction, CancellationToken cancellationToken)
    {
        var instruction = BuildRewriteInstruction(action, freeFormInstruction);
        var userMessage = $"Current tailored resume content:\n{JsonSerializer.Serialize(current)}\n\nInstruction: {instruction}";

        var responseText = await CallClaudeAsync(RewriteSystemPrompt, userMessage, cancellationToken);
        var node = ParseJsonSafe(responseText);

        var skills = ReadStringArray(node?["Skills"]);
        return Assemble(
            node?["Summary"]?.GetValue<string>() ?? current.Summary,
            skills.Count > 0 ? skills : current.Skills,
            current.Experience,
            current.Education,
            ReadBulletsByExperienceId(node));
    }

    // Applies AI-rewritten bullets over the authoritative experience facts (title/company/dates stay from source).
    private static TailoredResumeContent Assemble(
        string? summary,
        IReadOnlyList<string> skills,
        IReadOnlyList<TailoredResumeExperience> experienceFacts,
        IReadOnlyList<TailoredResumeEducation> education,
        IReadOnlyDictionary<int, List<string>> bulletsByExperienceId)
    {
        var experience = experienceFacts
            .Select(e => bulletsByExperienceId.TryGetValue(e.Id, out var bullets) && bullets.Count > 0
                ? e with { Bullets = bullets }
                : e)
            .ToList();

        return new TailoredResumeContent
        {
            Summary = summary,
            Skills = skills,
            Experience = experience,
            Education = education
        };
    }

    private static List<TailoredResumeExperience> BuildExperienceFacts(UserProfile? profile)
    {
        if (profile == null) return [];
        return profile.WorkExperiences
            .Where(w => w.Status == Status.Active)
            .OrderBy(w => w.OrderIndex)
            .Select(w => new TailoredResumeExperience
            {
                Id = w.Id,
                Title = w.JobTitle,
                Company = w.Company,
                Location = w.Location,
                StartDate = w.StartDate.ToString("yyyy-MM"),
                EndDate = w.IsCurrent ? null : w.EndDate?.ToString("yyyy-MM"),
                IsCurrent = w.IsCurrent,
                Bullets = w.Bullets.OrderBy(b => b.OrderIndex).Select(b => b.Content).ToList()
            })
            .ToList();
    }

    private static List<TailoredResumeEducation> BuildEducation(UserProfile? profile)
    {
        if (profile == null) return [];
        return profile.Educations
            .Where(e => e.Status == Status.Active)
            .OrderBy(e => e.OrderIndex)
            .Select(e => new TailoredResumeEducation
            {
                Id = e.Id,
                School = e.School,
                Degree = e.DegreeType.ToString(),
                Major = e.Major,
                StartDate = e.StartDate?.ToString("yyyy-MM"),
                EndDate = e.EndDate?.ToString("yyyy")
            })
            .ToList();
    }

    private static JsonNode? ParseJsonSafe(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        // Claude sometimes wraps the JSON in code fences or adds prose despite the prompt — extract the object.
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start < 0 || end <= start) return null;

        try { return JsonNode.Parse(text.Substring(start, end - start + 1)); }
        catch { return null; }
    }

    private static IReadOnlyList<string> ReadStringArray(JsonNode? node)
    {
        return node?.AsArray()
            .Select(s => s?.GetValue<string>() ?? string.Empty)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList() ?? [];
    }

    private static Dictionary<int, List<string>> ReadBulletsByExperienceId(JsonNode? node)
    {
        var result = new Dictionary<int, List<string>>();
        var experience = node?["Experience"]?.AsArray();
        if (experience == null) return result;

        foreach (var item in experience)
        {
            var id = item?["Id"]?.GetValue<int>();
            if (id == null) continue;
            result[id.Value] = item!["Bullets"]?.AsArray()
                .Select(b => b?.GetValue<string>() ?? string.Empty)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .ToList() ?? [];
        }

        return result;
    }

    private static string BuildGeneratePrompt(UserProfile? profile, JobListing? job, CustomResumeGenerateOptions options)
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
                .Where(w => w.Status == Status.Active)
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
                .Where(e => e.Status == Status.Active)
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

        sb.AppendLine(options.WorkMode == TailoringWorkMode.Quick
            ? "Work mode: QUICK — only rewrite the first 2 work experiences; omit the rest from the Experience array (their original bullets are kept)."
            : "Work mode: FULL — rewrite all work experiences.");

        return sb.ToString();
    }

    private static string BuildRewriteInstruction(AiRewriteAction action, string? freeFormInstruction) =>
        action switch
        {
            AiRewriteAction.StrongerActionVerbs =>
                "Rewrite the experience bullets using stronger, more impactful action verbs " +
                "(e.g., achieved, delivered, spearheaded, optimized, reduced, increased, led).",
            AiRewriteAction.ShortenSummary =>
                "Make the content more concise. Remove filler words and redundant phrases while preserving all key information.",
            AiRewriteAction.RemoveUnrelatedSkills =>
                "Remove skills and keywords that are not relevant to the job requirements present in the tailored content.",
            AiRewriteAction.FreeForm =>
                string.IsNullOrWhiteSpace(freeFormInstruction)
                    ? "Improve the content quality and clarity."
                    : freeFormInstruction,
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
}
