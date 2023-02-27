using System.Text;
using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Worker.Consumers;
using EventStore.Client;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Service;

public class GetPostRequestConsumerShould
{
    ServiceProvider serviceProvider;
    GetPostRequestConsumer consumer
        => serviceProvider
            .GetRequiredService<GetPostRequestConsumer>();

    public GetPostRequestConsumerShould()
    {
        var services = new ServiceCollection();
        services.AddEventStoreClient("esdb://admin:changeit@localhost:2213?tls=false");
        services.AddLogging();
        services.AddTransient<GetPostRequestConsumer>();
        serviceProvider = services.BuildServiceProvider();
    }

}
