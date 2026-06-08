using Application.Identity;

namespace Application.Features.Authentication.Data;

public record UserInfoById(int Id) : IUserInfo
{
    public string UserName => string.Empty;
}