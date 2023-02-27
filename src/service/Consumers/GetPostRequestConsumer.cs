using EDBlog.Domain.Contracts;
using MassTransit;

public class GetPostRequestConsumer : 
    IConsumer<GetPostRequest>
{

    public GetPostRequestConsumer()
    {
    }

    public async Task Consume(ConsumeContext<GetPostRequest> context)
    {
        await context.RespondAsync<GetPostResponse>(new {
            Found = false
        });
    }
}