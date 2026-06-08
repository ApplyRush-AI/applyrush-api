using Application.Common.Interfaces.Services;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Services.Cities;

internal sealed class EmbeddedCityProvider : ICityProvider
{
    private readonly IReadOnlyList<string> _cities;
    private readonly HashSet<string> _lookup;

    public EmbeddedCityProvider()
    {
        var assembly = typeof(EmbeddedCityProvider).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("us-cities.json", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Embedded resource us-cities.json not found.");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        _cities = JsonSerializer.Deserialize<List<string>>(stream)
            ?? throw new InvalidOperationException("Failed to deserialize us-cities.json.");
        _lookup = new HashSet<string>(_cities, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<string> Search(string? query, int limit)
    {
        if (limit <= 0) limit = 20;
        if (limit > 100) limit = 100;

        if (string.IsNullOrWhiteSpace(query))
            return _cities.Take(limit).ToList();

        var needle = query.Trim();
        return _cities
            .Where(c => c.Contains(needle, StringComparison.OrdinalIgnoreCase))
            .Take(limit)
            .ToList();
    }

    public bool Exists(string display) => _lookup.Contains(display);
}
