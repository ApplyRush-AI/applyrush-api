namespace Application.Common.Exceptions;

public sealed class InsufficientCreditsException : Exception
{
    public InsufficientCreditsException()
        : base("Insufficient credits to perform this operation.")
    {
    }

    public InsufficientCreditsException(string message)
        : base(message)
    {
    }
}
