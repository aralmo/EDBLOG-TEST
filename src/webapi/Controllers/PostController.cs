using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;
using MassTransit;
using System.Diagnostics;
using EDBlog.Domain.Entities;

namespace EDBlog.WebAPI.Controllers;

public partial class PostController : Controller
{
    private readonly IMediator mediator;

    public PostController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPatch, Route("post/{postId}")]
    public async Task<IResult> PATCH(Guid postId, [FromBody] EditPostRequest request, CancellationToken cancellationToken)
    {
        if (request.Title == null && request.Description == null && request.Content == null)
            return Results.NoContent();

        await mediator.Publish<EditPostCommandContract>(new
        {
            PostId = postId,
            Title = request.Title,
            Description = request.Description,
            Content = request.Content
        });

        return Results.Accepted();
    }

    [HttpPost, Route("post")]
    public async Task<IResult> POST([FromBody] NewPostRequest request, CancellationToken cancellationToken)
    {
        Guid postId = Guid.NewGuid();

        await mediator.Publish<CreatePostCommandContract>(new
        {
            AuthorId = request.AuthorId,
            PostId = postId,
            Title = request.post.Title,
            Description = request.post.Description,
            Content = request.post.Content
        });

        return Results.Accepted(value: new
        {
            PostId = postId
        });
    }

    [HttpGet, Route("post/{postId}")]
    public async Task<IResult> GET(Guid postId)
    {
        var result = await mediator
            .Request<GetPostRequestContract, GetPostResponseContract>(
                new 
                {
                    PostId = postId
                });

        if (result.Found == false)
            return Results.NotFound();
        
        if (result.Post == null)
            return Results.NoContent();
        
        return Results.Ok(new
        {
            Title = result.Post.Title,
            Description = result.Post.Description,
            Content = result.Post.Content,
            Links = new[]
            {
                //todo: proper HATEOAS
                new Link(href: $"/author/{result.Post.AuthorId}", rel: "post author", HttpMethod.Get)
            }
        });
    }

    public record EditPostRequest(
        [MinLength(10), MaxLength(100)]
        string? Title,
        string? Description,
        string? Content);
    
    public record NewPostRequest(Guid AuthorId, NewPost post);
    
    public record NewPost(
        [MinLength(10), MaxLength(100)]
        string Title,
        string? Description,
        string? Content);

}