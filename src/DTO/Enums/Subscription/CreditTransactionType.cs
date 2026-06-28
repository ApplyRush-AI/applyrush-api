using DTO.Attributes;

namespace DTO.Enums.Subscription;

public enum CreditTransactionType
{
    [LocalizationKey("enum.creditTransactionType.tailoringUsed")]
    TailoringUsed = 1,
    [LocalizationKey("enum.creditTransactionType.analysisUsed")]
    AnalysisUsed = 2,
    [LocalizationKey("enum.creditTransactionType.autofillUsed")]
    AutofillUsed = 3,
    [LocalizationKey("enum.creditTransactionType.dailyReset")]
    DailyReset = 4,
    [LocalizationKey("enum.creditTransactionType.planUpgrade")]
    PlanUpgrade = 5
}
