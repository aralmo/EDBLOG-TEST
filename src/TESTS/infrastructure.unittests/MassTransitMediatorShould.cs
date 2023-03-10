using EDBlog.Core.Abstractions;
using EDBlog.Infrastructure;
using MassTransit;
using Moq;

namespace Infrastructure.UnitTests;

[Trait("type", "unit")]
public class MassTransitMediatorShould
{
    [Fact]
    public async void PublishThrough_MassTransitPublishEndpoint()
    {
        //arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();

        publishEndpointMock
            .Setup(pem => pem.Publish(It.IsAny<object>(),It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        IMediator mediator = new MassTransitMediator(publishEndpointMock.Object, null!);

        //act and verify
        await mediator.Publish<FakeCmd>(new FakeCmd());
        publishEndpointMock.Verify();
    }

    [Fact]
    public async void PropagateCancellation_ToMassTransitPublishEndpoint()
    {
        //arrange
        var publishEndpointMock = new Mock<IPublishEndpoint>();
        CancellationToken ctoken = new CancellationToken();

        publishEndpointMock
            .Setup(pem => pem.Publish(It.IsAny<object>(),ctoken))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        IMediator mediator = new MassTransitMediator(publishEndpointMock.Object, null!);

        //act and verify
        await mediator.Publish<FakeCmd>(new FakeCmd(),ctoken);
        publishEndpointMock.Verify();
    }

    interface some{}
    interface somein:some{}
    class x:somein{}


    record FakeCmd() : ICommand;
}