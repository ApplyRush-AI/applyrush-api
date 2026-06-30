using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Resume;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Infrastructure.Services;

public sealed class CustomResumePdfService : ICustomResumePdfService
{
    private const string Ink = "#111827";
    private const string Muted = "#6b7280";
    private const string Rule = "#e5e7eb";

    static CustomResumePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private readonly IApplicationDbContext _dbContext;

    public CustomResumePdfService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stream> RenderAsync(TailoredResumeContent content, TailoredResumeStyle? style, int userId, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var resolved = Resolve(style ?? new TailoredResumeStyle());
        var bytes = Render(profile, content, resolved);
        return new MemoryStream(bytes);
    }

    // Maps the font-family enum to the actual OSS family installed in the image (see Dockerfile / fonts doc).
    private static string FontName(ResumeFontFamily family) => family switch
    {
        ResumeFontFamily.Arial => "Arimo",
        ResumeFontFamily.TimesNewRoman => "Tinos",
        ResumeFontFamily.Calibri => "Carlito",
        ResumeFontFamily.Cambria => "Caladea",
        _ => "Lato"
    };

    private sealed record ResolvedStyle(
        PageSize Page, string Font, string Accent, ResumeTemplate Template,
        float NameSize, float SectionSize, float SubSize, float BodySize,
        ResumeDateFormat DateFormat, string Bullet, ResumeHeaderAlignment Header,
        EducationDisplayOrder EduOrder, SkillsLayout Skills,
        float SectionSpacing, float EntrySpacing, float LineHeight, float MarginV, float MarginH,
        bool Justify);

    private static ResolvedStyle Resolve(TailoredResumeStyle s)
    {
        var compact = s.Template == ResumeTemplate.Compact ? 0.78f : 1f;
        var fit = s.FitToOnePage ? 0.85f : 1f;
        var scale = compact * fit;

        return new ResolvedStyle(
            Page: s.PaperSize == PaperSize.A4 ? PageSizes.A4 : PageSizes.Letter,
            Font: FontName(s.FontFamily),
            Accent: SanitizeHex(s.AccentColor),
            Template: s.Template,
            NameSize: (float)s.FontSizeName * fit,
            SectionSize: (float)s.FontSizeSectionHeaders * fit,
            SubSize: (float)s.FontSizeSubHeaders * fit,
            BodySize: (float)s.FontSizeBodyText * fit,
            DateFormat: s.DateFormat,
            Bullet: s.BulletIcon == BulletIcon.Dash ? "–" : "•",
            Header: s.HeaderAlignment,
            EduOrder: s.EducationDisplayOrder,
            Skills: s.SkillsLayout,
            SectionSpacing: Map(s.SectionSpacing, 6, 22) * scale,
            EntrySpacing: Map(s.EntrySpacing, 2, 12) * scale,
            LineHeight: Map(s.LineSpacing, 1.0f, 1.6f),
            MarginV: Map(s.TopBottomMargin, 20, 64) * fit,
            MarginH: Map(s.SideMargins, 20, 64),
            Justify: s.JustifyText);
    }

    private static float Map(int value, float min, float max)
    {
        var v = Math.Clamp(value, 0, 100);
        return min + (max - min) * v / 100f;
    }

    private static string SanitizeHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return Ink;
        var h = hex.Trim();
        if (!h.StartsWith('#')) h = "#" + h;
        var body = h[1..];
        var valid = (body.Length == 6 || body.Length == 3) && body.All(Uri.IsHexDigit);
        return valid ? h : Ink;
    }

    private static byte[] Render(UserProfile? profile, TailoredResumeContent content, ResolvedStyle st) =>
        Document.Create(doc => doc.Page(page =>
        {
            page.Size(st.Page);
            page.MarginVertical(st.MarginV);
            page.MarginHorizontal(st.MarginH);
            page.DefaultTextStyle(t => t.FontFamily(st.Font).FontSize(st.BodySize).LineHeight(st.LineHeight).FontColor(Ink));

            page.Content().Column(col =>
            {
                col.Spacing(st.SectionSpacing);
                RenderHeader(col, profile, st);
                if (!string.IsNullOrWhiteSpace(content.Summary))
                    RenderTextSection(col, "Summary", content.Summary!, st);
                if (content.Skills.Count > 0)
                    RenderSkills(col, content.Skills, st);
                if (content.Experience.Count > 0)
                    RenderExperience(col, content.Experience, st);
                if (content.Education.Count > 0)
                    RenderEducation(col, content.Education, st);
            });
        })).GeneratePdf();

    private static void RenderHeader(ColumnDescriptor col, UserProfile? profile, ResolvedStyle st)
    {
        var name = $"{profile?.FirstName} {profile?.LastName}".Trim();
        var contacts = string.Join("   |   ", new[] { profile?.Email, profile?.Phone, profile?.LinkedInUrl, profile?.Location }
            .Where(s => !string.IsNullOrWhiteSpace(s)));

        col.Item().Element(c => Align(c, st.Header)).Column(h =>
        {
            h.Item().Text(string.IsNullOrWhiteSpace(name) ? "Resume" : name).FontSize(st.NameSize).Bold().FontColor(st.Accent);
            if (!string.IsNullOrWhiteSpace(profile?.Title))
                h.Item().Text(profile!.Title!).FontSize(st.SubSize).FontColor(Muted);
            if (!string.IsNullOrWhiteSpace(contacts))
                h.Item().Text(contacts).FontSize(st.BodySize - 1).FontColor(Muted);
        });
    }

    private static IContainer Align(IContainer c, ResumeHeaderAlignment a) => a switch
    {
        ResumeHeaderAlignment.Center => c.AlignCenter(),
        ResumeHeaderAlignment.Right => c.AlignRight(),
        _ => c.AlignLeft()
    };

    private static void SectionTitle(ColumnDescriptor col, string title, ResolvedStyle st)
    {
        col.Item().PaddingTop(st.EntrySpacing * 0.6f).Text(title.ToUpperInvariant()).FontSize(st.SectionSize).Bold().FontColor(st.Accent).LetterSpacing(0.05f);
        col.Item().PaddingBottom(2).LineHorizontal(0.75f).LineColor(Rule);
    }

    private static void RenderTextSection(ColumnDescriptor col, string title, string body, ResolvedStyle st) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, title, st);
            c.Item().Text(t => { if (st.Justify) t.Justify(); t.Span(body); });
        });

    private static void RenderSkills(ColumnDescriptor col, IReadOnlyList<string> skills, ResolvedStyle st) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Skills", st);
            switch (st.Skills)
            {
                case SkillsLayout.Bulleted:
                    foreach (var s in skills)
                        c.Item().PaddingLeft(12).Text($"{st.Bullet}   {s}");
                    break;
                case SkillsLayout.TwoColumn:
                    var mid = (skills.Count + 1) / 2;
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Column(cc => { foreach (var s in skills.Take(mid)) cc.Item().Text($"{st.Bullet}   {s}"); });
                        row.RelativeItem().Column(cc => { foreach (var s in skills.Skip(mid)) cc.Item().Text($"{st.Bullet}   {s}"); });
                    });
                    break;
                default:
                    c.Item().Text(string.Join("   •   ", skills));
                    break;
            }
        });

    private static void RenderExperience(ColumnDescriptor col, IReadOnlyList<TailoredResumeExperience> experience, ResolvedStyle st) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Experience", st);
            foreach (var exp in experience)
            {
                c.Item().PaddingTop(st.EntrySpacing).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span(exp.Title).FontSize(st.SubSize).Bold();
                        if (!string.IsNullOrWhiteSpace(exp.Company))
                            t.Span($"   —   {exp.Company}").FontSize(st.SubSize).FontColor("#374151");
                    });
                    row.ConstantItem(140).AlignRight().Text(DateRange(exp.StartDate, exp.EndDate, exp.IsCurrent, st.DateFormat)).FontSize(st.BodySize - 1).FontColor(Muted);
                });
                if (!string.IsNullOrWhiteSpace(exp.Location))
                    c.Item().Text(exp.Location).FontSize(st.BodySize - 1).FontColor(Muted);
                foreach (var bullet in exp.Bullets)
                    c.Item().PaddingLeft(12).Text(t => { if (st.Justify) t.Justify(); t.Span($"{st.Bullet}   {bullet}"); });
            }
        });

    private static void RenderEducation(ColumnDescriptor col, IReadOnlyList<TailoredResumeEducation> education, ResolvedStyle st) =>
        col.Item().Column(c =>
        {
            SectionTitle(c, "Education", st);
            foreach (var edu in education)
            {
                var degree = string.Join(", ", new[] { edu.Degree, edu.Major }.Where(s => !string.IsNullOrWhiteSpace(s)));
                var primary = st.EduOrder == EducationDisplayOrder.InstitutionFirst ? edu.School : degree;
                var secondary = st.EduOrder == EducationDisplayOrder.InstitutionFirst ? degree : edu.School;

                c.Item().PaddingTop(st.EntrySpacing).Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.Span(primary).FontSize(st.SubSize).Bold();
                        if (!string.IsNullOrWhiteSpace(secondary))
                            t.Span($"   —   {secondary}").FontSize(st.SubSize).FontColor("#374151");
                    });
                    row.ConstantItem(140).AlignRight().Text(DateRange(edu.StartDate, edu.EndDate, false, st.DateFormat)).FontSize(st.BodySize - 1).FontColor(Muted);
                });
            }
        });

    private static string DateRange(string? start, string? end, bool isCurrent, ResumeDateFormat format)
    {
        var s = FormatDate(start, format);
        var e = isCurrent ? "Present" : FormatDate(end, format);
        return string.Join(" – ", new[] { s, e }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static string? FormatDate(string? raw, ResumeDateFormat format)
    {
        if (string.IsNullOrWhiteSpace(raw)) return raw;
        DateTime dt;
        if (DateTime.TryParseExact(raw, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
            DateTime.TryParseExact(raw, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
        {
            return format switch
            {
                ResumeDateFormat.NumericMonthYear => dt.ToString("MM/yyyy", CultureInfo.InvariantCulture),
                ResumeDateFormat.YearOnly => dt.ToString("yyyy", CultureInfo.InvariantCulture),
                _ => dt.ToString("MMM yyyy", CultureInfo.InvariantCulture)
            };
        }
        return raw;
    }
}
