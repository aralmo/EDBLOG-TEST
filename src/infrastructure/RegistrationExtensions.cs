using System.Reflection;
using EDBlog.Infrastructure.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EDBlog.Infrastructure;

public static class RegistrationExtensions
{
    public static IServiceCollection SetupPublisher(
        this IServiceCollection services,
        RabbitMQConfiguration rabbitMQConfiguration)
    {
        services.AddMassTransit(cfg =>
        {
            cfg.UsingRabbitMq((ctx, rcfg) =>
            {
                rcfg.Host(rabbitMQConfiguration.Host ?? "localhost", rhcfg =>
                {
                    rhcfg.Username(rabbitMQConfiguration.User ?? "guest");
                    rhcfg.Password(rabbitMQConfiguration.Password ?? "guest");
                });
                rcfg.ConfigureEndpoints(ctx);
            });
        });        

        services.AddScoped<IMediator, MassTransitMediator>();
        return services;
    }

    public static IServiceCollection SetupConsumers(
        this IServiceCollection services,
        RabbitMQConfiguration rabbitMQConfiguration,
        Assembly consumersAssembly)
    {
        services.AddMassTransit(x =>
        {
            x.AddDelayedMessageScheduler();
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(consumersAssembly);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMQConfiguration.Host ?? "localhost", rhcfg =>
                {
                    rhcfg.Username(rabbitMQConfiguration.User ?? "guest");
                    rhcfg.Password(rabbitMQConfiguration.Password ?? "guest");
                });

                //Built-in scheduler for retry strategies.
                //Requires masstransit/rabbitmq image
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

//ToDo: cleanup parameters
    public static IServiceCollection SetupOpenTelemetry(
        this IServiceCollection services,
        string serviceName,
        string serviceVersion,
        Uri? zipkinEndpoint,
        bool addAspNetInstrumentation = false)
    {
        services
            .AddOpenTelemetry()
            .WithTracing(options =>
            {
                options
                    .AddSource(serviceName)
                    .AddSource("MassTransit")
                    .AddConsoleExporter()
                    .AddZipkinExporter(opt =>
                    {
                        if (zipkinEndpoint != null)
                            opt.Endpoint = zipkinEndpoint;
                    })
                    .SetResourceBuilder(ResourceBuilder
                            .CreateDefault()
                            .AddService(serviceName: serviceName, serviceVersion: serviceVersion));
                    
                if (addAspNetInstrumentation)
                    options.AddAspNetCoreInstrumentation();
            })
            .StartWithHost();

        return services;
    }
}