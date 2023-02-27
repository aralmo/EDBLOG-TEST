using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;
using MassTransit;

namespace EDBlog.WebAPI.Controllers;

public class GetPostController : Controller
{
    private readonly IMediator mediator;

    public GetPostController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet, Route("post/{postId}")]
    public async Task<IResult> GET(Guid postId)
    {
        var result = await mediator
            .Request<GetPostRequest, GetPostResponse>(
                new GetPostRequestContract()
                {
                    PostId = postId
                });

        return result.Found switch
        {
            true when result.Post != null => Results.Ok(result.Post),
            false => Results.NotFound(),
            _ => Results.NoContent()
        };
    }

    record GetPostRequestContract : GetPostRequest
    {
        public Guid PostId { get; init; }
    }
}