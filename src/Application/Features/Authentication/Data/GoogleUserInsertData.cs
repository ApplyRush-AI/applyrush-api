using Domain.Entities.User;

namespace Application.Features.Authentication.Data;

internal sealed record GoogleUserInsertData(
    string FirstName,
    string LastName,
    string Email) : IUserInsertData
{
    public string Password => string.Empty;
    public string? PhoneNumber => null;
}
