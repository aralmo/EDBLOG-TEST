using EDBlog.Infrastructure;
using EDBlog.Infrastructure.RabbitMQ;

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
                var rabbitCfg = hostContext.Configuration
                        .GetRequiredSection("RabbitMQ")
                        .Get<RabbitMQConfiguration>();

                services.SetupConsumers(rabbitCfg, typeof(Program).Assembly);

                var zipkinUri = hostContext.Configuration
                    .GetValue<Uri>("ZipkinEndpoint");
                    
                services.SetupOpenTelemetry("BlogService", "1", zipkinUri);
            });
}
