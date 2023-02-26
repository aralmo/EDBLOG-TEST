namespace Domain.Entities.Core;

public interface IEntity 
{
    //ToDo: I would like to have a type for identifiers instead of using system's GUID.

     Guid Identifier {get;}
}