namespace Application.Common.Interfaces.Services;

public interface ICreditService
{
    /// <summary>
    /// Returns the number of remaining tailoring credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetTailoringCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the number of remaining analysis credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetAnalysisCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the number of remaining autofill credits for the user.
    /// Returns -1 for unlimited (Premium tier).
    /// </summary>
    Task<int> GetAutofillCreditsRemainingAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one tailoring credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductTailoringCreditAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one analysis credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductAnalysisCreditAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deducts one autofill credit for the user. Throws InsufficientCreditsException if none remain.
    /// </summary>
    Task DeductAutofillCreditAsync(int userId, CancellationToken cancellationToken);
}
