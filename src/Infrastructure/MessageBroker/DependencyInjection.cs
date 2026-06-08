using Application.Common.MessageBroker;
using Infrastructure.MessageBroker.Config;
using Infrastructure.MessageBroker.Extensions;
using Infrastructure.MessageBroker.Filters;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Infrastructure.MessageBroker;

public static class DependencyInjection
{
    public static void AddMessageBroker(this IServiceCollection services, Assembly assembly, bool useIdentity = true)
    {
        //services.AddScoped<IConsumeContextAccessor, ConsumeContextAccessor>();
        services.AddSingleton<IConsumeContextAccessor, ConsumeContextAccessor>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddOptions<RabbitMqConfiguration>().BindConfiguration(RabbitMqConfiguration.SectionName);
        services.AddMassTransit(
       x =>
       {
           x.AddConsumers(assembly);
           x.UsingRabbitMq(
               (context, cfg) =>
               {
                   var config = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;
                   var logger = context.GetRequiredService<ILogger<RabbitMqBusObserver>>();

                   if (config.Enabled)
                   {
                       CreateHost(cfg, config, p =>
                       {
                           p.Username(config.Username);
                           p.Password(config.Password);
                       });

                       cfg.ConnectBusObserver(new RabbitMqBusObserver(logger, config.HostName, config.Port));

                       cfg.UseConsumeFilter(typeof(FillConsumeContextAccessorConsumeFilter<>), context);

                       cfg.ConfigureEndpoints(context);

                       if (useIdentity)
                       {
                           cfg.UseAuthFilters(context);
                       }
                   }
                   else
                   {
                       logger.LogWarning("RabbitMQ is disabled via configuration — no consumers or publishers will be active");
                   }
               });
       });
    }

    private static void CreateHost(IRabbitMqBusFactoryConfigurator cfg, RabbitMqConfiguration config, Action<IRabbitMqHostConfigurator>? configure = null)
    {
        if (Uri.TryCreate(config.HostName, UriKind.Absolute, out var uri))
        {
            cfg.Host(uri, config.VirtualHost, configure);
            return;
        }

        cfg.Host(config.HostName, config.Port, config.VirtualHost, configure);
    }
}
