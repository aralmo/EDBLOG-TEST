using System.ComponentModel.DataAnnotations;
using EDBlog.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using EDBlog.Domain.Contracts;
using MassTransit;
using System.Diagnostics;

namespace EDBlog.WebAPI.Controllers;

public class NewPostController : Controller
{
    private readonly IMediator mediator;

    public NewPostController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost, Route("post")]
    public async Task<IResult> POST([FromBody] NewPostRequest request, CancellationToken cancellationToken)
    {
        Guid postId = Guid.NewGuid();

        //since it's ED, author id should either be from an auth token or validated for existance
        //for this test, if the author doesn't exist will return a placeholder one
        await mediator.Publish<CreatePostCommandContract>(new CreatePostContract()
        {
            AuthorId = request.AuthorId,
            PostId = postId,
            Title = request.post.Title,
            Description = request.post.Description,
            Content = request.post.Content
        });

        return Results.Accepted(value: new
        {
            PostId = postId,
            TraceId = Activity.Current?.TraceId
        });
    }

    record NewPostResponse
    {
        public string? TraceId { get; init; }
        public string PostId { get; init; }
    }

    //Contracts
    internal record CreatePostContract : CreatePostCommandContract
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