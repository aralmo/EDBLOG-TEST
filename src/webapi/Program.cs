using EDBlog.Domain.Contracts;
using EDBlog.Infrastructure;
using MassTransit;

namespace EDBlog.WebAPI;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Setups publishing bus and mediator
        var rabbitConfig = builder.Configuration
            .GetRequiredSection("RabbitMQ")
            .Get<Infrastructure.RabbitMQ.RabbitMQConfiguration>();

        builder.Services.SetupPublisher(rabbitConfig);

        // Setup Open Telemetry
        var zipkinUri = builder.Configuration
            .GetValue<Uri>("ZipkinEndpoint");

        builder.Services.SetupOpenTelemetry(
            serviceName: "WebAPI",
            serviceVersion: "1",
            zipkinEndpoint: zipkinUri,
            addAspNetInstrumentation: true);

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.Run();
    }
}