namespace EDBlog.Domain.Entities;

public interface Post : IEntity
{
    /// <summary>
    /// Identifier for the post author.
    /// </summary>
    public Guid AuthorId {get;}
    /// <summary>
    /// Post title to be shown in post lists.
    /// </summary>
    /// <value></value>
    public string Title{get;}
    /// <summary>
    /// Brief description for the POST contents
    /// </summary>
    /// <value></value>
    public string? Description{get;}
    /// <summary>
    /// Post contents
    /// </summary>
    /// <value></value>
    public string? Content{get;}
}