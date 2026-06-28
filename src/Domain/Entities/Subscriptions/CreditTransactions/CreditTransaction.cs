using Domain.Entities.Base;
using Domain.Entities.User;
using DTO.Enums.Subscription;

namespace Domain.Entities.Subscriptions.CreditTransactions;

public sealed class CreditTransaction : BaseAuditableEntity
{
    private CreditTransaction() { }

    public int UserId { get; private set; }
    public CreditTransactionType Type { get; private set; }
    public int Amount { get; private set; }
    public int? ReferenceId { get; private set; }
    public string? ReferenceType { get; private set; }

    public ApplicationUser User { get; } = null!;

    public static CreditTransaction Create(ICreditTransactionInsertData data)
    {
        return new CreditTransaction
        {
            UserId = data.UserId,
            Type = data.Type,
            Amount = data.Amount,
            ReferenceId = data.ReferenceId,
            ReferenceType = data.ReferenceType
        };
    }
}
