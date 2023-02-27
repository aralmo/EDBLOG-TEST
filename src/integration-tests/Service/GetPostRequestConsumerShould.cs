using System.Text;
using System.Text.Json;
using EDBlog.Domain.Contracts;
using EDBlog.Worker.Consumers;
using EventStore.Client;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTests.Service;

public class GetPostRequestConsumerShould
{
    ServiceProvider serviceProvider;
    GetPostRequestConsumer consumer
        => serviceProvider
            .GetRequiredService<GetPostRequestConsumer>();

    public GetPostRequestConsumerShould()
    {
        var services = new ServiceCollection();
        services.AddEventStoreClient("esdb://admin:changeit@localhost:2213?tls=false");
        services.AddLogging();
        services.AddTransient<GetPostRequestConsumer>();
        serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async void ReturnPost_IfExists()
    {
        //arrange
        var esClient = serviceProvider.GetRequiredService<EventStoreClient>();
        Guid postid = Guid.NewGuid();
        
        var stub = new 
                {
                    AuthorId = Guid.NewGuid(),
                    Title = "some title",
                    Description = "post description",
                    Content = "post contents"
                };

        await esClient.AppendToStreamAsync($"post-{postid}", StreamState.Any, new[]
        {            
            new EventData(Uuid.NewUuid(),"NewPost",
                JsonSerializer.SerializeToUtf8Bytes(stub))
        });
        
        var mock = new Mock<ConsumeContext<GetPostRequestContract>>();
        
        mock.Setup(m => m.Message).Returns(new contract(){
            PostId = postid
        });

        //act
        await consumer.Consume(mock.Object);

        //todo: check content
        
        mock.Verify(m => m.RespondAsync<GetPostResponseContract>(It.IsAny<GetPostResponseContract>()),Times.Once);
        GetPostResponseContract response =(GetPostResponseContract) mock.Invocations.Last().Arguments[0];
        response.Post.Should().BeEquivalentTo(stub);
    }

    record contract : GetPostRequestContract
    {
        public Guid PostId {get;set;}
    }
}
