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

    public Task Publish<TMessage>(TMessage message) where TMessage : ICommand
    => publishEndpoint.Publish(message);

    public Task Publish<TMessage>(TMessage message, CancellationToken cancellation) where TMessage : ICommand
    => publishEndpoint.Publish(message, cancellation);

}

internal class MassTransitRequestClient<TRequest, TResponse>: IRequestClient<TRequest, TResponse>
    where TRequest : class, IRequestFor<TResponse>
    where TResponse : class
{
    private readonly IRequestClient<TRequest> mtclient;

    public MassTransitRequestClient(MassTransit.IRequestClient<TRequest> mtclient)
    {
        this.mtclient = mtclient;
    }

    public async Task<TResponse> Request(TRequest message)
    {
        var response = await mtclient.GetResponse<TResponse>(message);
        return response.Message;
    }
}
