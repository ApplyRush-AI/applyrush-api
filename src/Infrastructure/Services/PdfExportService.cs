using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.WorkExpeciences;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text.Json.Nodes;

namespace Infrastructure.Services;

public sealed class PdfExportService : IPdfExportService
{
    private const string Ink = "#111827";
    private const string Muted = "#6b7280";
    private const string Rule = "#e5e7eb";
    private const string Accent = "#1d4ed8";
    private const string Font = "Lato"; // bundled with QuestPDF and available in the container image

    static PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private readonly IApplicationDbContext _dbContext;

    public PdfExportService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stream> ExportProfileAsPdfAsync(int userId, CancellationToken cancellationToken)
    {
        var profile = await LoadProfileAsync(userId, cancellationToken);
        return new MemoryStream(Render(profile, summaryOverride: null, tailoredBullets: null));
    }

    public async Task<Stream> ExportTailoringAsPdfAsync(int tailoringId, CancellationToken cancellationToken)
    {
        var tailoring = await _dbContext.ResumeTailoring
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tailoringId, cancellationToken);

        var profile = tailoring != null
            ? await LoadProfileAsync(tailoring.UserId, cancellationToken)
            : null;

        // The tailored content overrides the summary and per-experience bullets; everything else
        // (contact header, titles, dates, education) still comes from the user's profile.
        var (summary, bulletsByExperienceId) = ParseTailoredContent(tailoring?.TailoredContent);
        return new MemoryStream(Render(profile, summary, bulletsByExperienceId));
    }

    private async Task<UserProfile?> LoadProfileAsync(int userId, CancellationToken cancellationToken) =>
        await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences).ThenInclude(w => w.Bullets)
            .Include(p => p.Educations)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    // Parses the AI-produced tailored content JSON: { "Summary": "...", "Experience": [ { "Id": 1, "Bullets": ["..."] } ] }.
    private static (string? Summary, IReadOnlyDictionary<int, IReadOnlyList<string>> Bullets) ParseTailoredContent(string? json)
    {
        var empty = new Dictionary<int, IReadOnlyList<string>>();
        if (string.IsNullOrWhiteSpace(json)) return (null, empty);

        try
        {
            var node = JsonNode.Parse(json);
            if (node == null) return (null, empty);

            var summary = node["Summary"]?.GetValue<string>();
            var map = new Dictionary<int, IReadOnlyList<string>>();

            foreach (var item in node["Experience"]?.AsArray() ?? [])
            {
                var id = item?["Id"]?.GetValue<int>() ?? 0;
                if (id == 0) continue;

                var bullets = item?["Bullets"]?.AsArray()
                    .Select(b => b?.GetValue<string>() ?? string.Empty)
                    .Where(b => !string.IsNullOrWhiteSpace(b))
                    .ToList() ?? [];

                if (bullets.Count > 0) map[id] = bullets;
            }

            return (summary, map);
        }
        catch
        {
            return (null, empty);
        }
    }

    private static byte[] Render(UserProfile? profile, string? summaryOverride, IReadOnlyDictionary<int, IReadOnlyList<string>>? tailoredBullets) =>
        Document.Create(doc => doc.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.MarginVertical(40);
            page.MarginHorizontal(48);
            page.DefaultTextStyle(t => t.FontFamily(Font).FontSize(10).LineHeight(1.25f).FontColor(Ink));

            page.Content().Column(col =>
            {
                col.Spacing(14);
                RenderHeader(col, profile);

                var summary = !string.IsNullOrWhiteSpace(summaryOverride) ? summaryOverride : profile?.Bio;
                if (!string.IsNullOrWhiteSpace(summary))
                    RenderTextSection(col, "Summary", summary!);

                var skills = profile?.Skills.Select(s => s.Name).Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? [];
                if (skills.Count > 0)
                    RenderSkills(col, skills);

                var experiences = profile?.WorkExperiences
                    .Where(w => w.Status == Status.Active)
                    .OrderBy(w => w.OrderIndex)
                    .ToList() ?? [];
                if (experiences.Count > 0)
                    RenderExperience(col, experiences, tailoredBullets);

                var educations = profile?.Educations
                    .Where(e => e.Status == Status.Active)
                    .OrderBy(e => e.OrderIndex)
                    .ToList() ?? [];
                if (educations.Count > 0)
                    RenderEducation(col, educations);
            });
        })).GeneratePdf();

    private static void RenderHeader(ColumnDescriptor col, UserProfile? profile)
    {
        var name = $"{profile?.FirstName} {profile?.LastName}".Trim();
        var contacts = string.Join("   |   ", new[] { profile?.Email, profile?.Phone, profile?.LinkedInUrl, profile?.Location }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        col.Item().Column(h =>
        {
            h.Item().Text(string.IsNullOrWhiteSpace(name) ? "Resume" : name).FontSize(20).Bold().FontColor(Accent);
            if (!string.IsNullOrWhiteSpace(profile?.Title))
                h.Item().Text(profile!.Title!).FontSize(11).FontColor(Muted);
            if (!string.IsNullOrWhiteSpace(contacts))
                h.Item().Text(contacts).FontSize(9).FontColor(Muted);
        });
    }

    private static void SectionTitle(ColumnDescriptor col, string title)
    {
        col.Item().PaddingTop(4).Text(title.ToUpperInvariant()).FontSize(11).Bold().FontColor(Accent).LetterSpacing(0.05f);
        col.Item().PaddingBottom(2).LineHorizontal(0.75f).LineColor(Rule);
    }

    private static void RenderTextSection(ColumnDescriptor col, string title, string body) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, title);
            c.Item().Text(body);
        });

    private static void RenderSkills(ColumnDescriptor col, IReadOnlyList<string> skills) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Skills");
            c.Item().Text(string.Join("   •   ", skills));
        });

    private static void RenderExperience(
        ColumnDescriptor col,
        IReadOnlyList<WorkExperience> experiences,
        IReadOnlyDictionary<int, IReadOnlyList<string>>? tailoredBullets) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Experience");
            foreach (var exp in experiences)
            {
                c.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span(exp.JobTitle).FontSize(11).Bold();
                        if (!string.IsNullOrWhiteSpace(exp.Company))
                            t.Span($"   —   {exp.Company}").FontSize(11).FontColor("#374151");
                    });
                    row.ConstantItem(130).AlignRight()
                        .Text(DateRange(exp.StartDate, exp.EndDate, exp.IsCurrent)).FontSize(9).FontColor(Muted);
                });

                if (!string.IsNullOrWhiteSpace(exp.Location))
                    c.Item().Text(exp.Location).FontSize(9).FontColor(Muted);

                // Prefer AI-tailored bullets for this experience; otherwise fall back to the stored bullets.
                var bullets = tailoredBullets != null && tailoredBullets.TryGetValue(exp.Id, out var tailored)
                    ? tailored
                    : exp.Bullets.OrderBy(b => b.OrderIndex).Select(b => b.Content).ToList();

                foreach (var bullet in bullets.Where(b => !string.IsNullOrWhiteSpace(b)))
                    c.Item().PaddingLeft(12).Text($"•   {bullet}");
            }
        });

    private static void RenderEducation(ColumnDescriptor col, IReadOnlyList<Education> educations) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Education");
            foreach (var edu in educations)
            {
                var degree = string.Join(", ", new[] { edu.DegreeType.ToString(), edu.Major }.Where(s => !string.IsNullOrWhiteSpace(s)));

                c.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span(edu.School).FontSize(11).Bold();
                        if (!string.IsNullOrWhiteSpace(degree))
                            t.Span($"   —   {degree}").FontSize(11).FontColor("#374151");
                    });
                    row.ConstantItem(130).AlignRight()
                        .Text(DateRange(edu.StartDate, edu.EndDate, false)).FontSize(9).FontColor(Muted);
                });
            }
        });

    private static string DateRange(DateOnly? start, DateOnly? end, bool isCurrent)
    {
        var s = start.HasValue ? start.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture) : null;
        var e = isCurrent ? "Present" : end.HasValue ? end.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture) : null;
        return string.Join(" – ", new[] { s, e }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
