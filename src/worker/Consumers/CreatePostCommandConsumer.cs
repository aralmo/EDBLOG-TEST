using EDBlog.Domain.Contracts;
using MassTransit;

namespace EDBlog.Worker.Consumers;

public class CreatePostCommandConsumer : IConsumer<CreatePostCommand>
{
    readonly ILogger<CreatePostCommandConsumer> _logger;
    public CreatePostCommandConsumer(ILogger<CreatePostCommandConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<CreatePostCommand> context)
    {
        _logger.LogInformation("Received New Post: {Title}", context.Message.Title);
        return Task.CompletedTask;
    }
}