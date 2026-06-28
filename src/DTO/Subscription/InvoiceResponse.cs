namespace DTO.Subscription;

public sealed record InvoiceResponse(
    string Id,
    DateTime Date,
    decimal Amount,
    string Status,
    string DownloadUrl
);
