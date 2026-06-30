using DTO.Enums.Resume;

namespace DTO.Resumes;

// All fields optional; the defaults below reproduce the renderer's current (pre-style) output,
// so a download request that omits style (or omits any field) is unchanged. Sliders are 0-100.
public sealed record TailoredResumeStyle
{
    public PaperSize PaperSize { get; init; } = PaperSize.Letter;
    public ResumeTemplate Template { get; init; } = ResumeTemplate.Standard;
    public string AccentColor { get; init; } = "#111827";
    public AccentColorScope AccentColorScope { get; init; } = AccentColorScope.AllHeadings;
    public ResumeFontFamily FontFamily { get; init; } = ResumeFontFamily.Lato;
    public decimal FontSizeName { get; init; } = 20;
    public decimal FontSizeSectionHeaders { get; init; } = 11;
    public decimal FontSizeSubHeaders { get; init; } = 11;
    public decimal FontSizeBodyText { get; init; } = 10;
    public ResumeDateFormat DateFormat { get; init; } = ResumeDateFormat.ShortMonthYear;
    public BulletIcon BulletIcon { get; init; } = BulletIcon.Dot;
    public ResumeHeaderAlignment HeaderAlignment { get; init; } = ResumeHeaderAlignment.Left;
    public EducationDisplayOrder EducationDisplayOrder { get; init; } = EducationDisplayOrder.DegreeFirst;
    public SkillsLayout SkillsLayout { get; init; } = SkillsLayout.List;
    public int SectionSpacing { get; init; } = 50;
    public int EntrySpacing { get; init; } = 50;
    public int LineSpacing { get; init; } = 50;
    public int TopBottomMargin { get; init; } = 50;
    public int SideMargins { get; init; } = 50;
    public bool JustifyText { get; init; }
    public bool FitToOnePage { get; init; }
}
