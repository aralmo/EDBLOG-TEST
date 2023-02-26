using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;

namespace EDBlog.WebAPI.Controllers;

[Route("post")]
public class PostController : Controller
{
    private readonly IMediator mediator;

    public PostController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<IResult> POST([FromBody] NewPostRequest request, CancellationToken cancellationToken)
    {
        //since it's ED, author id should either be from an auth token or validated for existance
        //for this test, if the author doesn't exist will return a placeholder one
        await mediator.Publish<CreatePostCommand>(new CreatePostContract()
        {
            AuthorId = request.AuthorId,
            PostId = Guid.NewGuid(),
            Title = request.post.Title,
            Description = request.post.Description,
            Content = request.post.Content
        });

        return Results.Accepted();
    }

    [HttpGet, Route("{postId}")]
    public IResult GET(Guid postId)
        => throw new NotImplementedException();

    //Contracts
    internal record CreatePostContract : CreatePostCommand
    {
        public required Guid AuthorId { get; init; }

        public required Guid PostId { get; init; }

        public required string Title { get; init; }

        public string? Description { get; init; }

        public string? Content { get; init; }
    }

    //Models

    public record NewPostRequest(Guid AuthorId, NewPost post);

    public record NewPost(
        [MinLength(10), MaxLength(100)]
        string Title,
        string? Description,
        string? Content);
}