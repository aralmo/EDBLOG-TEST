using EDBlog.Infrastructure.RabbitMQ;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

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
            });
        });

        services.AddScoped<IMediator, MassTransitMediator>();
        return services;
    }
}