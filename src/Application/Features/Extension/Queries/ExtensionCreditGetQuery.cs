using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using DTO.Extension;

namespace Application.Features.Extension.Queries;

public sealed record ExtensionCreditGetQuery : IQuery<ExtensionCreditResponse>;

public sealed class ExtensionCreditGetQueryHandler : IQueryHandler<ExtensionCreditGetQuery, ExtensionCreditResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ICreditService _creditService;

    public ExtensionCreditGetQueryHandler(
        ICurrentUserService currentUserService,
        ICreditService creditService)
    {
        _currentUserService = currentUserService;
        _creditService = creditService;
    }

    public async Task<ExtensionCreditResponse> Handle(ExtensionCreditGetQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var remaining = await _creditService.GetAutofillCreditsRemainingAsync(userId, cancellationToken);

        // Stub values — will be replaced with real UserCredit entity reads in MS5
        var total = remaining == -1 ? -1 : 4;
        var used = remaining == -1 ? 0 : total - remaining;
        var resetsAt = DateTime.UtcNow.Date.AddDays(1);

        return new ExtensionCreditResponse
        {
            Used = used,
            Remaining = remaining,
            Total = total,
            ResetsAt = resetsAt
        };
    }
}
