namespace EDBlog.Domain.Contracts;

public interface GetPostRequest : IRequestFor<GetPostResponse>
{
    Guid PostId {get;}
}

public interface GetPostResponse
{
    bool Found {get;}
    Post? Post {get;}
}