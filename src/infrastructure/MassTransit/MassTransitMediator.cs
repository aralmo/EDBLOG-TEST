namespace EDBlog.Infrastructure.MassTransit;

internal class MassTransitMediator : IMediator
{
    public void Publish<TMessage>(TMessage message) where TMessage : ICommand
    {
        throw new NotImplementedException();
    }
}
