using System.Diagnostics;
using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Domain.Entities;
using EventStore.Client;
using MassTransit;
using static EventStore.Client.EventStoreClient;

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
        response response = new response(){};

        //ToDo: tidy up this code
        try
        {
            var post = await eventStoreClient
                .ReadStreamAsync(
                    Direction.Forwards,
                    $"post-{context.Message.PostId}",
                    StreamPosition.Start)

                    .AggregateAsync(new PostAggregator(), (acc, x) =>
                    {
                        var doc = JsonSerializer.Deserialize<PostAggregator>(x.Event.Data.ToArray());
                        return acc with
                        {
                            AuthorId = doc?.AuthorId ?? acc.AuthorId,
                            Content = doc?.Content ?? acc.Content,
                            Description = doc?.Description ?? acc.Description,
                            Title = doc?.Title ?? acc.Title
                        };
                    });
            
            if (post != null)
                response = new response()
                {
                    Found = true,
                    Post = new post()
                    {
                        AuthorId = post.AuthorId??Guid.Empty,
                        Title = post.Title??string.Empty,
                        Description = post.Description,
                        Content = post.Content
                    }
                };
        }
        catch (StreamNotFoundException ex)
        {
            Activity.Current.AddException(ex, $"post stream not found {context.Message.PostId}");
        }

        await context.RespondAsync<GetPostResponseContract>(response);
    }

    enum PostEvents
    {
        Unknown,
        NewPost,
        EditPost
    }


    record PostAggregator()
    {
        public Guid? AuthorId { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? Content { get; init; }
    }


    record response : GetPostResponseContract
    {
        public bool Found { get; init; }

        public Post? Post { get; init; }
    }
    record post : Post
    {
        public Guid AuthorId { get; init; }

        public required string Title { get; init; }

        public string? Description { get; init; }

        public string? Content { get; init; }

        public Guid Identifier { get; init; }
    }
}