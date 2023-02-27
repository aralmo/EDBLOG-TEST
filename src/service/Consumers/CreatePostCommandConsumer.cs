using System.Text.Json;
using EDBlog.Domain.Contracts;
using EventStore.Client;
using MassTransit;

namespace EDBlog.Worker.Consumers;

public class CreatePostCommandConsumer : IConsumer<CreatePostCommand>
{
    readonly ILogger<CreatePostCommandConsumer> _logger;
    private readonly EventStoreClient client;

    public CreatePostCommandConsumer(ILogger<CreatePostCommandConsumer> logger, EventStoreClient eventStoreClient)
    {
        _logger = logger;
        this.client = eventStoreClient;
    }

    public Task Consume(ConsumeContext<CreatePostCommand> context)
    {
        _logger.LogInformation("Received New Post: {Title}", context.Message.Title);
        
        return client.AppendToStreamAsync(
            $"authorposts-{context.Message.AuthorId}",
            StreamState.Any,
            new[]{
            new EventData(
				Uuid.NewUuid(),
				"NewPost",
				JsonSerializer.SerializeToUtf8Bytes(new {
                    context.Message.Title,
                    context.Message.Description
                })
			)});
    }
}