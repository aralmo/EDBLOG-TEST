using EDBlog.Infrastructure;
using EDBlog.Infrastructure.RabbitMQ;
using EventStore.Client;

namespace EDBlog.Worker;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                //setup eventstoredb
                var esConnString = hostContext.Configuration
                        .GetRequiredSection("EventStoreDB")
                        .GetValue<string>("ConnectionString") 
                        ?? throw new Exception("EventStoreDB connection string not found in configuration");
                        
                services.AddEventStoreClient(esConnString);
                
                //setup rabbitmq and masstransit
                var rabbitCfg = hostContext.Configuration
                        .GetRequiredSection("RabbitMQ")
                        .Get<RabbitMQConfiguration>();

                services.SetupConsumers(rabbitCfg, typeof(Program).Assembly);

                //setup opentelemetry
                var zipkinUri = hostContext.Configuration
                    .GetValue<Uri>("ZipkinEndpoint");
                    
                services.SetupOpenTelemetry("BlogService", "1", zipkinUri);
            });
}
