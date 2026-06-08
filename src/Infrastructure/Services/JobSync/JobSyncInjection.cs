using Application.Common.Interfaces.Services;
using Infrastructure.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.JobSync;

internal static class JobSyncInjection
{
    internal static IServiceCollection AddJobSync(this IServiceCollection services)
    {
        services.AddConfigurationBoundOptions<JSearchOptions>(JSearchOptions.SectionName);

        services.AddHttpClient("JSearch", client =>
        {
            client.BaseAddress = new Uri("https://jsearch.p.rapidapi.com/");
        });

        services.AddScoped<IJobProvider, JSearchJobProvider>();
        services.AddScoped<IJobSyncState, JobSyncState>();

        return services;
    }
}
