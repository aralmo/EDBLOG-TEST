using EDBlog.Domain.Entities;
namespace EDBlog.Domain.Contracts;

public interface CreatePostCommand : ICommand
{
    Guid AuthorId {get;}
    Post Post {get; }
}
