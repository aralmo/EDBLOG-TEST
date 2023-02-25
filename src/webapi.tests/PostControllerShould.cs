
using EDBlog.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

public class PostControllerShould
{
    PostController Controller = new PostController();

    [Theory]
    [InlineData("some-random-text-nonparseble-to-guid")]
    [InlineData("")]
    [InlineData(null)]
    public void BadRequest_IfAuthorInvalid(string authorId) 
        => Controller
            .POST(new PostController.NewPostRequest(authorId,ValidPost))
            .Should()
            .BeOfType<BadRequest>();

    static PostController.Post ValidPost 
        => new ("post title", "post description", "post contents");
}