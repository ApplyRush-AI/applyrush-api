namespace DTO.Enums.Profile.Education;

public static class GpaScaleExtensions
{
    // Upper bound a GPA may take on each scale. "Other" has no fixed maximum, so it is not range-checked.
    public static decimal? MaxValue(this GpaScale scale) => scale switch
    {
        GpaScale.FourPoint => 4m,
        GpaScale.FivePoint => 5m,
        GpaScale.TenPoint => 10m,
        _ => null
    };
}
