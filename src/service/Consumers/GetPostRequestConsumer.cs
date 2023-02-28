using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Domain.Entities;
using EventStore.Client;
using MassTransit;

public class GetPostRequestConsumer :
    IConsumer<GetPostRequestContract>
{
    private readonly EventStoreClient eventStoreClient;

    public GetPostRequestConsumer(EventStoreClient eventStoreClient)
    {
        this.eventStoreClient = eventStoreClient;
    }

    public async Task Consume(ConsumeContext<GetPostRequestContract> context)
    {
        response response;

        //ToDo: tidy up this code
        try
        {
            var newpostevt = await eventStoreClient.ReadStreamAsync(
                Direction.Backwards,
                $"post-{context.Message.PostId}",
                StreamPosition.Start).FirstOrDefaultAsync(evt => evt.Event.EventType == "NewPost");

            if (newpostevt.Event != null)
                response = new response()
                {
                    Found = true,
                    Post = JsonSerializer.Deserialize<post>(newpostevt.Event.Data.ToArray())
                };
            else
            {
                response = new response()
                {
                    Found = false
                };
            }
        }
        catch (StreamNotFoundException)
        {
            response = new response()
            {
                Found = false
            };
        }

        await context.RespondAsync<GetPostResponseContract>(response);
    }

    record response : GetPostResponseContract
    {
        public bool Found { get; init; }

        public Post? Post { get; init; }
    }
    record post : Post
    {
        public Guid AuthorId { get; init; }

        public string Title { get; init; }

        public string? Description { get; init; }

        public string? Content { get; init; }

        public Guid Identifier { get; init; }
    }
}