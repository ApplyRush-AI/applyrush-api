namespace DTO.Subscription;

public sealed record UserCreditResponse(
    CreditTypeUsageResponse Tailoring,
    CreditTypeUsageResponse Analysis,
    CreditTypeUsageResponse Autofill,
    DateTime ResetsAt
);
