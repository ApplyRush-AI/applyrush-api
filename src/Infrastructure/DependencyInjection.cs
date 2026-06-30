using Application.Common.Interfaces.Auth.ExternalAuth;
using Application.Common.Interfaces.Identity;
using Application.Common.Interfaces.Services;
using Domain.Interfaces;
using Infrastructure.AI;
using Infrastructure.Caching;
using Infrastructure.Identity;
using Infrastructure.Identity.DependencyInjection;
using Infrastructure.MediaStorage;
using Infrastructure.MessageBroker;
using Infrastructure.Search;
using Infrastructure.Services;
using Infrastructure.Services.Cities;
using Infrastructure.Services.Configuration;
using Infrastructure.Services.JobSync;
using Infrastructure.Common.Extensions;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly assembly)
    {
        services.AddIdentityModule(configuration);
        services.AddScoped<IApplicationUserManager, ApplicationUserManager>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddSingleton<IEncryption, EncryptionService>();
        services.AddScoped<IExternalAuthService, GoogleAuthService>();
        services.AddScoped<IMatchScoringService, MatchScoringService>();
        services.AddScoped<IPdfExportService, PdfExportService>();
        services.AddScoped<IResumeTailoringAiService, ResumeTailoringAiService>();
        services.AddScoped<ICustomResumeAiService, CustomResumeAiService>();
        services.AddScoped<ICustomResumePdfService, CustomResumePdfService>();
        services.AddScoped<IResumeAnalysisAiService, ResumeAnalysisAiService>();
        services.AddScoped<ICreditService, CreditService>();
        services.AddConfigurationBoundOptions<StripeConfig>(StripeConfig.SectionName);
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<IExtensionAnswerAiService, ExtensionAnswerAiService>();
        services.AddSingleton<ICityProvider, EmbeddedCityProvider>();
        services.AddClaude();
        services.AddJobSync();
        services.AddScoped<IResumeParseService, ResumeParseService>();
        services.AddMessageBroker(assembly, true);
        services.AddMediaStorage(configuration);
        services.AddCaching(configuration);
        services.AddSearch();
        return services;
    }
}

