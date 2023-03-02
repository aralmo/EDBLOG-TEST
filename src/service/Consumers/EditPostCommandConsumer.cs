using System.Text.Json;
using EDBlog.Domain.Contracts;
using EventStore.Client;
using MassTransit;

namespace EDBlog.Worker.Consumers;

public class EditPostCommandConsumer : IConsumer<EditPostCommandContract>
{
    const string EDITPOST_EVENT_NAME = "EditPost";
    private readonly EventStoreClient client;

    public EditPostCommandConsumer(EventStoreClient eventStoreClient)
    {
        this.client = eventStoreClient;
    }

    public Task Consume(ConsumeContext<EditPostCommandContract> context)
        => Task.WhenAll(new[]{

                // create an entry for the specific post stream to track changes
                client.AppendToStreamAsync(
                    $"post-{context.Message.PostId}",
                    StreamState.Any,
                    new[]{
                    new EventData(
                        Uuid.NewUuid(),
                        EDITPOST_EVENT_NAME,
                        JsonSerializer.SerializeToUtf8Bytes(new {
                            context.Message.Title,
                            context.Message.Description,
                            context.Message.Content
                        })
                    )})});
}