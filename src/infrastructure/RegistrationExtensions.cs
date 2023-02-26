using Microsoft.Extensions.DependencyInjection;

namespace EDBlog.Infrastructure;

public static class RegistrationExtensions
{
    public static IServiceCollection SetupPublisher(this IServiceCollection services)
    {
        //ToDo: register mass transit and publisher.
        services.AddSingleton<IMediator>(new MassTransitMediator(null));
        return services;
    }
}