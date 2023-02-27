namespace EDBlog.Domain.Contracts;

public interface GetPostRequestContract : IRequestFor<GetPostResponseContract>
{
    Guid PostId {get;}
}

public interface GetPostResponseContract
{
    bool Found {get;}
    Post? Post {get;}
}