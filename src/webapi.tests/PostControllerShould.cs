
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EDBlog.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;

[Trait("type", "integration")]
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
    public async void Return_BadRequest_IfAuthorInvalid(string authorId)
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
    public async void Return_BadRequest_IfNoModelSent()
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
    public async void Return_BadRequest_IfTitleInvalid(string title)
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