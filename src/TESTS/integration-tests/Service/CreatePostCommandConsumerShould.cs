using System.Text;
using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Worker.Consumers;
using EventStore.Client;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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
        var mock = new Mock<ConsumeContext<CreatePostCommandContract>>();
        mock.Setup(m => m.Message).Returns(contract);

        await consumer.Consume(mock.Object);

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
                contract.PostId
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
        var mock = new Mock<ConsumeContext<CreatePostCommandContract>>();
        mock.Setup(m => m.Message).Returns(contract);

        await consumer.Consume(mock.Object);
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

    class Contract : CreatePostCommandContract
    {
        public Guid AuthorId { get; set; }

        public Guid PostId { get; set; }

        public required string Title { get; init; }

        public string? Description { get; set; }

        public string? Content { get; set; }
    }
}
