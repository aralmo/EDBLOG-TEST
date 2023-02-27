using EDBlog.Domain.Entities;
namespace EDBlog.Domain.Contracts;

public interface CreatePostCommandContract : ICommand
{
    Guid AuthorId {get;}
    Guid PostId {get;}
    string Title{get;}
    string? Description {get;}
    string? Content {get;}
}
