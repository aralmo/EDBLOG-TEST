
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.UnitTests;

[Trait("type", "unit")] //no external dependencies, marking as unit
public class PostControllerShould
    : IClassFixture<WebApplicationFactory<EDBlog.WebAPI.Program>>
{
    private readonly WebApplicationFactory<EDBlog.WebAPI.Program> clientFactory;
    public PostControllerShould(WebApplicationFactory<EDBlog.WebAPI.Program> clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("some-random-text-nonparseble-to-guid")]
    public async void ReturnBadRequest_WhenAuthorInvalid(string authorId)
    {
        var client = clientFactory.CreateClient();

        var result = await client
            .PostAsync("/post", JsonFor(
                new 
                {
                    AuthorId = authorId
                }));

        var content = await result.Content.ReadAsStringAsync();


        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        content.Should().Contain(".AuthorId"); //
    }

    [Fact]
    public async void ReturnBadRequest_WhenNoModelSent()
    {
        var client = clientFactory.CreateClient();

        var result = await client
            .PostAsync("/post",new StringContent("", Encoding.UTF8, "application/json"));

        var content = await result.Content.ReadAsStringAsync();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("less/10")]
    [InlineData(null)]
    public async void ReturnBadRequest_WhenTitleInvalid(string title)
    {
        var client = clientFactory.CreateClient();

        var result = await client
            .PostAsync("/post", JsonFor(
                new 
                {
                    AuthorId = Guid.NewGuid().ToString(), 
                    Post = new {
                        Title = title
                    }
                }));

        var content = await result.Content.ReadAsStringAsync();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        content.Should().Contain(".Title"); //
    }

    static HttpContent JsonFor(object request)
        => new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
}