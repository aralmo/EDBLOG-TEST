public interface EditPostCommandContract : ICommand
{
    Guid PostId {get;}
    string? Title{get;}
    string? Description {get;}
    string? Content {get;}
}