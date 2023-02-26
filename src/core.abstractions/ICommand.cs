namespace EDBlog.Core.Abstractions;

///<summary>
/// This interface needs to be used to decorate all commands.
/// This would allow us to control the contracts that can be published and 
/// also performing health checks against the deployed topology.
///</summary>
public interface ICommand
{
    // this could have some properties like bus name 
    // and be checked for uniqueness on a unit test

    // leaving it to masstransit defaults for this test
}