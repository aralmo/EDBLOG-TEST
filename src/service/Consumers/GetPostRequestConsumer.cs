using EDBlog.Domain.Contracts;
using MassTransit;

public class GetPostRequestConsumer : 
    IConsumer<GetPostRequestContract>
{

    public GetPostRequestConsumer()
    {
    }

    public async Task Consume(ConsumeContext<GetPostRequestContract> context)
    {
        await context.RespondAsync<GetPostResponseContract>(new {
            Found = false
        });
    }
}