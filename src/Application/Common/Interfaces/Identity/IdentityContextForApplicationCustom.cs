namespace Application.Identity;

public record IdentityContextForApplicationCustom(IUserInfo CurrentUser) : IIdentityContext;
