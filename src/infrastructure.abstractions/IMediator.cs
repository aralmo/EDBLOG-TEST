//ToDo: move to infrastructure abstractions?

public interface IMediator
{
    /// <summary>
    /// Publishes a command into it's setup message bus or service.
    /// </summary>
    void Publish<TMessage>(TMessage message) 
        where TMessage : ICommand;
}