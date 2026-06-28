using DTO.Enums.Subscription;

namespace Domain.Entities.Subscriptions.CreditTransactions;

public interface ICreditTransactionInsertData
{
    int UserId { get; }
    CreditTransactionType Type { get; }
    int Amount { get; }
    int? ReferenceId { get; }
    string? ReferenceType { get; }
}
