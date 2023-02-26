namespace EDBlog.Core.Abstractions;

public interface IEntity 
{
    //ToDo: I would like to have a type for identifiers and id factories instead of using system's GUID.

     Guid Identifier {get;}
}