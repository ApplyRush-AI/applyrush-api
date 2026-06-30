using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeDateFormat
{
    [LocalizationKey("enum.resumeDateFormat.shortMonthYear")]
    ShortMonthYear = 1,
    [LocalizationKey("enum.resumeDateFormat.numericMonthYear")]
    NumericMonthYear = 2,
    [LocalizationKey("enum.resumeDateFormat.yearOnly")]
    YearOnly = 3
}
