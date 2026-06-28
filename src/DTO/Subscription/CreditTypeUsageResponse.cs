namespace DTO.Subscription;

public sealed record CreditTypeUsageResponse(
    int Used,
    int Remaining,
    int Total
);
