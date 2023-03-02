using System.Text;
using System.Text.Json;
using EDBlog.Core.Abstractions;
using EDBlog.Domain.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace WebAPI.UnitTests;

[Trait("type", "unit")] //no external dependencies, marking as unit
public class PostControllerPATCHShould
{
    private readonly WebApplicationFactory<EDBlog.WebAPI.Program> clientFactory = new();

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("some-random-text-nonparseble-to-guid")]
    public async void ReturnBadRequest_WhenPostIdInvalid(string postId)
    {
        using var client = clientFactory
            .Arrange(opt => opt.MockRequired<IMediator>())
            .CreateClient();

        var result = await client
            .PatchAsync($"/post/{postId}", JsonFor(
                new
                {
                    Title = "some title"
                }));

        var content = await result.Content.ReadAsStringAsync();
        result.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async void ReturnBadRequest_WhenNoContentSent()
    {
        using var client = clientFactory
            .Arrange(opt => opt.MockRequired<IMediator>())
            .CreateClient();

        var postid = Guid.NewGuid();
        var result = await client
            .PatchAsync($"/post/{postid}", new StringContent("", Encoding.UTF8, "application/json"));

        var content = await result.Content.ReadAsStringAsync();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("less/10")]
    public async void ReturnBadRequest_WhenTitleInvalid(string title)
    {
        using var client = clientFactory
            .Arrange(opt => opt.MockRequired<IMediator>())
            .CreateClient();

        var result = await client
            .PatchAsync($"/post/{Guid.Empty}", JsonFor(
                new
                {
                    PostId = Guid.NewGuid().ToString(),
                    Title = title
                }));

        var content = await result.Content.ReadAsStringAsync();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        content.Should().Contain("Title"); //
    }

    [Fact]
    public async void ReturnNoContent_WhenNothingSet()
    {
        using var client = clientFactory
            .Arrange(opt => opt.MockRequired<IMediator>())
            .CreateClient();

        var result = await client
            .PatchAsync($"/post/{Guid.Empty}", JsonFor(
                new
                {
                    PostId = Guid.NewGuid().ToString()
                }));

        var content = await result.Content.ReadAsStringAsync();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void Publish_EditPostCommand()
    {
        //arrange
        Mock<IMediator> mediator = null!;
        using var client = clientFactory
            .Arrange(opt =>
            {
                mediator = opt.MockRequired<IMediator>();
                mediator
                    .Setup(m => m.Publish<EditPostCommandContract>(It.IsAny<EditPostCommandContract>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();
            })
            .CreateClient();

        var postid = Guid.NewGuid();
        //act and assert
        await client.PatchAsync($"post/{postid}", JsonFor(new
        {
            Title = "new post title",
            Description = "new post description",
            Content = "new post content"
        }));

        mediator.Verify();

        //get the published command and ensures it's single invocation
        var command = mediator.Invocations.Single(i => i.IsVerified).Arguments[0];

        command.Should().BeEquivalentTo(new
        {
            PostId = postid,
            Title = "new post title",
            Description = "new post description",
            Content = "new post content"
        });
    }

    static HttpContent JsonFor(object request)
        => new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
}
