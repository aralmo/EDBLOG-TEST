using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EDBlog.WebAPI.Controllers;

[Route("post")]
public class PostController : Controller
{
    [HttpPost]
    public IResult POST([FromBody] NewPostRequest request)
    {
        return Results.Ok();
    }

    [HttpGet, Route("{postId}")]
    public IResult GET(Guid postId)
        => throw new NotImplementedException();

    public record NewPostRequest(Guid AuthorId,Post post);
    
    public record Post(
        [MinLength(10), MaxLength(100)] 
        string Title, 
        string? Description, 
        string? Content);
}