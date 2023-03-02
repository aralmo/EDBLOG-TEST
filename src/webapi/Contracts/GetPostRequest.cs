using EDBlog.Domain.Contracts;
using EDBlog.Domain.Entities;

namespace EDBlog.WebAPI;

internal record GetPostRequest : GetPostRequestContract
{
    required public Guid PostId {get;init;}
}
