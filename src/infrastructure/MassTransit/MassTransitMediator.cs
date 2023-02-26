namespace EDBlog.Infrastructure;
using MassTransit;

internal class MassTransitMediator : IMediator
{
    private readonly IPublishEndpoint publishEndpoint;

    public MassTransitMediator(IPublishEndpoint publishEndpoint)
    {
        this.publishEndpoint = publishEndpoint;
    }

    public Task Publish<TMessage>(TMessage message) where TMessage : ICommand
    => publishEndpoint.Publish(message);

    public Task Publish<TMessage>(TMessage message,CancellationToken cancellation) where TMessage : ICommand
    => publishEndpoint.Publish(message,cancellation);
}
