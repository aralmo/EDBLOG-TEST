using EDBlog.Core.Abstractions;
using EDBlog.Infrastructure.MassTransit;

namespace EDBlog.Tests.Infrastructure;

public class MassTransitMediatorShould
{
    IMediator mediator = new MassTransitMediator();

    [Fact]
    public void Test1()
    {
        throw new NotImplementedException();
    }
}