using System.Text;
using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Worker.Consumers;
using EventStore.Client;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Service;

public class CreatePostCommandConsumerShould
{
    ServiceProvider serviceProvider;
    CreatePostCommandConsumer consumer
        => serviceProvider
            .GetRequiredService<CreatePostCommandConsumer>();

    public CreatePostCommandConsumerShould()
    {
        var services = new ServiceCollection();
            //run docker compose up in this project root to start dependencies.
        services.AddEventStoreClient("esdb://admin:changeit@localhost:2213?tls=false");
        services.AddLogging();
        services.AddTransient<CreatePostCommandConsumer>();
        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async void CreateEvent_InAuthorPostsStream()
    {
        //Arrange
        var esClient = serviceProvider.GetRequiredService<EventStoreClient>();
        var contract = new Contract()
        {
            AuthorId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Title = "new post title",
            Description = "new post description",
            Content = "new post content"
        };

        //Act
        await consumer.Consume(new FakeConsumeContext<CreatePostCommand>(contract));

        //Assert
        var authorReader = esClient.ReadStreamAsync(
            Direction.Forwards,
            $"authorposts-{contract.AuthorId}",
            StreamPosition.Start);

            // the author stream tracks author posts
        var authorStreamEvent = await authorReader.FirstAsync();
        authorStreamEvent.Should().NotBeNull();
        authorStreamEvent.Event?.EventType?.Should().Be("NewPost");

            // check the contents
        Encoding.UTF8
            .GetString(authorStreamEvent.Event!.Data.ToArray())
            .Should()
            .Be(JsonSerializer.Serialize(new
                {
                    contract.PostId,
                    contract.Title,
                    contract.Description
                }));
    }

     [Fact]
    public async void CreateEvent_InPostStream()
    {
        //Arrange
        var esClient = serviceProvider.GetRequiredService<EventStoreClient>();
        var contract = new Contract()
        {
            AuthorId = Guid.NewGuid(),
            PostId = Guid.NewGuid(),
            Title = "new post title",
            Description = "new post description",
            Content = "new post content"
        };

        //Act
        await consumer.Consume(new FakeConsumeContext<CreatePostCommand>(contract));

        //Assert
        var postReader = esClient.ReadStreamAsync(
            Direction.Forwards,
            $"post-{contract.PostId}",
            StreamPosition.Start);

        var postStreamEvent = await postReader.FirstAsync();
        postStreamEvent.Should().NotBeNull();
        postStreamEvent.Event?.EventType?.Should().Be("NewPost");
        
            // check the contents
        Encoding.UTF8
            .GetString(postStreamEvent.Event!.Data.ToArray())
            .Should()
            .Be(JsonSerializer.Serialize(new
                {
                    contract.AuthorId,
                    contract.Title,
                    contract.Description,
                    contract.Content
                }));

    }

    class Contract : CreatePostCommand
    {
        public Guid AuthorId { get; set; }

        public Guid PostId { get; set; }

        public required string Title { get; init; }

        public string? Description { get; set; }

        public string? Content { get; set; }
    }
    class FakeConsumeContext<T> : ConsumeContext<T> where T : class
    {
        T message;
        public T Message => message;

        public FakeConsumeContext(T message) => this.message = message;

        public ReceiveContext ReceiveContext => throw new NotImplementedException();

        public SerializerContext SerializerContext => throw new NotImplementedException();

        public Task ConsumeCompleted => throw new NotImplementedException();

        public IEnumerable<string> SupportedMessageTypes => throw new NotImplementedException();

        public CancellationToken CancellationToken => throw new NotImplementedException();

        public Guid? MessageId => throw new NotImplementedException();

        public Guid? RequestId => throw new NotImplementedException();

        public Guid? CorrelationId => throw new NotImplementedException();

        public Guid? ConversationId => throw new NotImplementedException();

        public Guid? InitiatorId => throw new NotImplementedException();

        public DateTime? ExpirationTime => throw new NotImplementedException();

        public Uri? SourceAddress => throw new NotImplementedException();

        public Uri? DestinationAddress => throw new NotImplementedException();

        public Uri? ResponseAddress => throw new NotImplementedException();

        public Uri? FaultAddress => throw new NotImplementedException();

        public DateTime? SentTime => throw new NotImplementedException();

        public Headers Headers => throw new NotImplementedException();

        public HostInfo Host => throw new NotImplementedException();

        public void AddConsumeTask(Task task)
        {
            throw new NotImplementedException();
        }

        public T1 AddOrUpdatePayload<T1>(PayloadFactory<T1> addFactory, UpdatePayloadFactory<T1> updateFactory) where T1 : class
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            throw new NotImplementedException();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            throw new NotImplementedException();
        }

        public T1 GetOrAddPayload<T1>(PayloadFactory<T1> payloadFactory) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public bool HasMessageType(Type messageType)
        {
            throw new NotImplementedException();
        }

        public bool HasPayloadType(Type payloadType)
        {
            throw new NotImplementedException();
        }

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            throw new NotImplementedException();
        }

        public Task NotifyConsumed<T1>(ConsumeContext<T1> context, TimeSpan duration, string consumerType) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task NotifyFaulted<T1>(ConsumeContext<T1> context, TimeSpan duration, string consumerType, Exception exception) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(T1 message, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(T1 message, IPipe<PublishContext<T1>> publishPipe, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(T1 message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(object values, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(object values, IPipe<PublishContext<T1>> publishPipe, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<T1>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T1 : class
        {
            throw new NotImplementedException();
        }

        public void Respond<T1>(T1 message) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(T1 message) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(T1 message, IPipe<SendContext<T1>> sendPipe) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(T1 message, IPipe<SendContext> sendPipe) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(object values) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(object values, IPipe<SendContext<T1>> sendPipe) where T1 : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T1>(object values, IPipe<SendContext> sendPipe) where T1 : class
        {
            throw new NotImplementedException();
        }

        public bool TryGetMessage<T1>(out ConsumeContext<T1>? consumeContext) where T1 : class
        {
            throw new NotImplementedException();
        }

        public bool TryGetPayload<T1>(out T1 payload) where T1 : class
        {
            throw new NotImplementedException();
        }
    }
}