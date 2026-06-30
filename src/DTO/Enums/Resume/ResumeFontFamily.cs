using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeFontFamily
{
    [LocalizationKey("enum.resumeFontFamily.lato")]
    Lato = 1,
    [LocalizationKey("enum.resumeFontFamily.arial")]
    Arial = 2,
    [LocalizationKey("enum.resumeFontFamily.timesNewRoman")]
    TimesNewRoman = 3,
    [LocalizationKey("enum.resumeFontFamily.calibri")]
    Calibri = 4,
    [LocalizationKey("enum.resumeFontFamily.cambria")]
    Cambria = 5
}
