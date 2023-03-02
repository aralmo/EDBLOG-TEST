using System.Text;
using System.Text.Json;
using EDBlog.Core.Abstractions;
using EDBlog.Domain.Contracts;
using EDBlog.Domain.Entities;
using EDBlog.WebAPI;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace WebAPI.UnitTests;

[Trait("type", "unit")] //no external dependencies, marking as unit
public class PostControllerGETShould
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
                    .Setup(m => m.Request<GetPostRequestContract, GetPostResponseContract>(It.IsAny<object>()))
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
        var fakeResponse = new fakeResponse()
        {
            Found = true,
            Post = new fakePost()
            {
                AuthorId = Guid.NewGuid(),
                Content = "post content",
                Description = "post description",
                Title = "post title"
            }
        };

        using var client = clientFactory
            .Arrange(opt =>
            {
                mediator = opt.MockRequired<IMediator>();
                mediator
                    .Setup(m => m.Request<GetPostRequestContract, GetPostResponseContract>(It.IsAny<object>()))
                    .ReturnsAsync(fakeResponse);
            })
            .CreateClient();

        var r = await client.GetAsync($"/post/{Guid.Empty}");
        r.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        string contentString = await r.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<getpostResponse>(contentString, serializerOptions);

        content.Should().BeEquivalentTo(new getpostResponse()
        {
            Title = fakeResponse.Post.Title,
            Content = fakeResponse.Post.Content,
            Links = new[]{
                new Link($"/author/{fakeResponse.Post.AuthorId}", "post author", HttpMethod.Get)
            },
            Description = fakeResponse.Post.Description
        });
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
                    .Setup(m => m.Request<GetPostRequestContract, GetPostResponseContract>(It.IsAny<object>()))
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

    JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    record getpostResponse
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? Content { get; init; }
        public IEnumerable<Link>? Links { get; init; }
    }

    class fakeResponse : GetPostResponseContract
    {
        public bool Found { get; set; }
        public Post? Post { get; set; }
    }
    class fakePost : Post
    {
        public Guid AuthorId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public Guid Identifier { get; set; }
    }
}
