using EDBlog.Domain.Contracts;

namespace EDBlog.WebAPI;

internal record CreatePostCommand : CreatePostCommandContract
{
    public required Guid AuthorId {get;init;}

    public required Guid PostId {get;init;}

    public required string Title {get;init;}

    public string? Description {get;init;}

    public string? Content {get;init;}
}
