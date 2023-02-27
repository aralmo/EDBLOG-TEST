using System.Text.Json;
using EDBlog.Domain.Contracts;
using EventStore.Client;
using MassTransit;

namespace EDBlog.Worker.Consumers;

public class CreatePostCommandConsumer : IConsumer<CreatePostCommand>
{
    const string NEWPOST_EVENT_NAME = "NewPost";
    private readonly EventStoreClient client;

    public CreatePostCommandConsumer(EventStoreClient eventStoreClient)
    {
        this.client = eventStoreClient;
    }

    public Task Consume(ConsumeContext<CreatePostCommand> context)
        => Task.WhenAll(new[]{

                // create an entry in the author stream to track author posts
                client.AppendToStreamAsync(
                    $"authorposts-{context.Message.AuthorId}",
                    StreamState.Any,
                    new[]{
                    new EventData(
                        Uuid.NewUuid(),
                        NEWPOST_EVENT_NAME,
                        JsonSerializer.SerializeToUtf8Bytes(new {
                            context.Message.PostId,
                            context.Message.Title,
                            context.Message.Description
                        })
                    )}),

                // create an entry for the specific post stream to track changes
                client.AppendToStreamAsync(
                    $"post-{context.Message.PostId}",
                    StreamState.Any,
                    new[]{
                    new EventData(
                        Uuid.NewUuid(),
                        NEWPOST_EVENT_NAME,
                        JsonSerializer.SerializeToUtf8Bytes(new {
                            context.Message.AuthorId,
                            context.Message.Title,
                            context.Message.Description,
                            context.Message.Content
                        })
                    )})});
}