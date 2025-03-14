using EFDataAccessLib.Models;

namespace EFDataAccessLib.Repos.Versioning;

public interface IRepoAppVersion : IBasicRepoWrite<AppVersion>, IBasicRepoRead<AppVersion, int>
{
}
