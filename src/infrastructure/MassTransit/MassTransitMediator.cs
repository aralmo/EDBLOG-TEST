namespace EDBlog.Infrastructure;
using MassTransit;

internal class MassTransitMediator : IMediator
{
    private readonly IPublishEndpoint publishEndpoint;
    private readonly IServiceProvider serviceProvider;

    public MassTransitMediator(IPublishEndpoint publishEndpoint, IServiceProvider provider)
    {
        this.publishEndpoint = publishEndpoint;
        this.serviceProvider = provider;
    }

    public Task Publish<TMessage>(object message) where TMessage : ICommand
    => publishEndpoint.Publish(message);

    public Task Publish<TMessage>(object message, CancellationToken cancellation) where TMessage : ICommand
    => publishEndpoint.Publish(message, cancellation);

    public async Task<TResponse> Request<TRequest, TResponse>(object message)
        where TRequest : class, IRequestFor<TResponse>
        where TResponse : class
    {
        var response = await serviceProvider
            .CreateRequestClient<TRequest>()
            .GetResponse<TResponse>(message);
        
        return response.Message;
        
    }
}
