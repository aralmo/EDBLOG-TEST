namespace EDBlog.Infrastructure.RabbitMQ;

public readonly struct RabbitMQConfiguration
{
    public string? Host{get; init;}
    public string? User{get;init;}
    public string? Password{get;init;}
}