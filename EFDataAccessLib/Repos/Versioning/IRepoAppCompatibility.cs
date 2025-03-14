using EFDataAccessLib.Models;

namespace EFDataAccessLib.Repos.Versioning;

public interface IRepoAppCompatibility : IBasicRepoWrite<AppCompatibility>, IBasicRepoRead<AppCompatibility, int>
{
}
