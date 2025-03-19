using EFDataAccessLib.Models;

namespace EFDataAccessLib.Repos.Versioning;

public interface IRepoAppVersion : IBasicRepoWrite<AppVersion>, IBasicRepoRead<AppVersion, int>
{
    Task<int> MaxId(int appId);
    Task<List<AppVersion>> GetLastVersionsUntil(int appId, int versionId, int count);
}
