namespace EDBlog.Core.Abstractions;

public interface IMediator
{
    //ToDo: would be nice to have an asyncresult type instead of task to allow for some monads

    /// <summary>
    /// Publishes a command into it's setup message bus or service.
    /// </summary>
    Task Publish<TMessage>(TMessage message)
        where TMessage : ICommand;

    Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : ICommand;
}