using System.Text;
using System.Text.Json;
using EDBlog.Core.Abstractions;
using EDBlog.Domain.Contracts;
using EDBlog.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace WebAPI.UnitTests;

[Trait("type", "unit")] //no external dependencies, marking as unit
public class GetPostControllerShould
{
    private readonly WebApplicationFactory<EDBlog.WebAPI.Program> clientFactory = new();

    [Fact]
    public async void ReturnNotFound_WhenPostDoesntExist()
    {
        Mock<IMediator>? mediator = null;
        using var client = clientFactory
            .Arrange(opt =>
            {
                mediator = opt.MockRequired<IMediator>();
                mediator
                    .Setup(m => m.Request<GetPostRequest, GetPostResponse>(It.IsAny<GetPostRequest>()))
                    .ReturnsAsync(new fakeResponse());
            })
            .CreateClient();

        var r = await client.GetAsync($"/post/{Guid.Empty}");
        r.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

    }

    [Fact]
    public async void ReturnProperPostFormat_WhenExists()
    {
        Mock<IMediator>? mediator = null;
        using var client = clientFactory
            .Arrange(opt =>
            {
                mediator = opt.MockRequired<IMediator>();
                mediator
                    .Setup(m => m.Request<GetPostRequest, GetPostResponse>(It.IsAny<GetPostRequest>()))
                    .ReturnsAsync(new fakeResponse()
                    {
                        Found = true,
                        Post = new fakePost()
                        {
                            AuthorId = Guid.NewGuid(),
                            Content = "post content",
                            Description = "post description", 
                            Title = "post title"
                        }});
            })
            .CreateClient();

        var r = await client.GetAsync($"/post/{Guid.Empty}");
        r.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

    }

    [Fact]
    public async void Use_MediatorRequest()
    {
        Mock<IMediator>? mediator = null;
        using var client = clientFactory
            .Arrange(opt =>
            {
                mediator = opt.MockRequired<IMediator>();
                mediator
                    .Setup(m => m.Request<GetPostRequest, GetPostResponse>(It.IsAny<GetPostRequest>()))
                    .ReturnsAsync(new fakeResponse()
                    {
                        Found = true,
                        Post = new fakePost()
                        {
                            AuthorId = Guid.NewGuid(),
                            Content = "post content",
                            Description = "post description", 
                            Title = "post title"
                        }
                    })
                    .Verifiable();

            })
            .CreateClient();

        var r = await client.GetAsync($"/post/{Guid.Empty}");
        mediator!.Verify();
    }


    class fakeResponse : GetPostResponse
    {
        public bool Found { get; set; }
        public Post? Post { get; set; }
    }
    class fakePost : Post
    {
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public Guid Identifier { get; set; }
    }
}
