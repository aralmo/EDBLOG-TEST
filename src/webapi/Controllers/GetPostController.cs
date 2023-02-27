using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;
using MassTransit;
using EDBlog.Domain.Entities;

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
            .Request<GetPostRequestContract, GetPostResponseContract>(
                new GetPostRequest()
                {
                    PostId = postId
                });

        return result.Found switch
        {
            true when result.Post != null => Results.Ok(MapResponse(result.Post)),
            false => Results.NotFound(),
            _ => Results.NoContent()
        };
    }

    GetPostResponse MapResponse(Post post) 
        => new GetPostResponse()
        {
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            Links = new[]
            {
                //todo: proper HATEOAS
                new Link(href: $"/author/{post.AuthorId}", rel: "post author", HttpMethod.Get)
            }
        };

    public record GetPostResponse
    {
        public required string Title {get;init;}
        public string? Description {get;init;}
        public string? Content {get;init;}
        public required IEnumerable<Link> Links {get;init;}
    }

    record GetPostRequest : GetPostRequestContract
    {
        public Guid PostId { get; init; }
    }
}