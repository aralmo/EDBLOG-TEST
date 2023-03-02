using EDBlog.Domain.Contracts;

namespace EDBlog.WebAPI;

public record EditPostCommand : EditPostCommandContract
{
    public required Guid PostId { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Content { get; init; }
}
