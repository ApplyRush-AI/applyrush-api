using Infrastructure.AI.Claude;
using Infrastructure.Common.Extensions;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.AI;

internal static class ClaudeInjection
{
    internal static IServiceCollection AddClaude(this IServiceCollection services)
    {
        services.AddConfigurationBoundOptions<ClaudeOptions>(ClaudeOptions.SectionName);
        services.AddHttpClient("Claude", client =>
        {
            client.BaseAddress = new Uri("https://api.anthropic.com/");
        });
        return services;
    }
}
