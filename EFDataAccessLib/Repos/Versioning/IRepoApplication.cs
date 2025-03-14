using EFDataAccessLib.Models;

namespace EFDataAccessLib.Repos.Versioning;

public interface IRepoApplication : IBasicRepoWrite<Application>, IBasicRepoRead<Application, int>
{
}
