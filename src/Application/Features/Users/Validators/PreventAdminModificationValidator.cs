using Application.Common.Localization.Extensions;
using Application.Common.Validation;
using Domain.Entities.User;
using DTO.Enums.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Validators;

public sealed record PreventAdminModificationValidatorData(ApplicationUser User);

public sealed class PreventAdminModificationValidator : BaseAbstractValidator<PreventAdminModificationValidatorData>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PreventAdminModificationValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(data => data.User)
            .MustAsync(async (user, cancellation) =>
            {
                var roles = await _userManager.GetRolesAsync(user);
                return !roles.Contains(UserRole.Administrator);
            })
            .WithLocalizationKey("preventAdminModificationValidator.message")
            .WithErrorCode("ADMIN_MODIFICATION_FORBIDDEN");
    }
}
