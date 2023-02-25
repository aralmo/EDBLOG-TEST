using Microsoft.AspNetCore.Mvc;

namespace EDBlog.WebAPI.Controllers;

[Route("post")]
public class PostController:Controller
{
    [HttpPost]
    public IActionResult POST(NewPostRequest request){
        throw new NotImplementedException();
    }

    public record NewPostRequest(string AuthorId, Post post);
    public record Post(string Title, string Description, string Content);

}