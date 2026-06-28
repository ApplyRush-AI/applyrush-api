using DTO.Enums.Subscription;
using DTO.Enums.User;

namespace DTO.Admin;

public sealed record AdminUserListItemResponse(
    int Id,
    string Email,
    string? FirstName,
    string? LastName,
    SubscriptionPlan Plan,
    UserStatus Status,
    DateTime CreatedAt
);
