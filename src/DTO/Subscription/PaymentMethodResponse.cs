namespace DTO.Subscription;

public sealed record PaymentMethodResponse(
    string Last4,
    string Brand,
    int ExpMonth,
    int ExpYear
);
