namespace Application.Common.Interfaces.Services;

public interface ICityProvider
{
    IReadOnlyList<string> Search(string? query, int limit);
    bool Exists(string display);
}
