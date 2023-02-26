using EDBlog.Domain.Entities;
namespace Domain.Contracts;

public interface CreatePostCommand
{
    Guid AuthorId {get;}
    Post Post {get; }
}
