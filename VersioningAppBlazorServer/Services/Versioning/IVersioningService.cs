using VersioningAppBlazorServer.Models;
using VersioningAppBlazorServer.Models.UI;
using ReturnTypeWrapper;

namespace VersioningAppBlazorServer.Services.Versioning;

public interface IVersioningService
{
    Task<MessageResult<KeyValuePair<int, int>>> AddNewApplication(ApplicationDTO application);
    Task<MessageResult<int>> AddNewVersion(AppVersionDTO appVersion, bool doInheritCompatibilityOfPreviousVersions);
    Task<MessageResult> AddNewVersionCompatibilities(int versionId, List<AppCompatibilityDTO> compatibilities);
    Task<MessageResult<ApplicationDTO>> GetApplicationById(int id);
    Task<MessageResult<List<DropDownItem>>> GetAllApplicationsNames();
    Task<MessageResult<List<KeyValuePair<int, string>>>> GetSelectedAppVersions(int appId);
    Task<MessageResult<IList<ApplicationDTO>>> GetCompatibilityApplications(int appVersionId);
    Task<MessageResult<AppVersionDTO>> GetAppVersion(int appVersionId);
    Task<MessageResult<IList<ApplicationDTO>>> GetAllApplications();
    Task<MessageResult<IList<AppCompatibilityDTO>>> GetVersionCompatibilities(int appVersionId);
    Task<MessageResult> DeleteAppCompatibility(int appVersionId, int appVersionIdToDelete);
    Task<MessageResult> DeleteApplication(int appId);
    Task<MessageResult> DeleteApplicationVersion(int appVersionId);
    Task<MessageResult> UpgradeApplicationVersionCompatibility(int appId, int oldVersionId, int newVersionId);
    Task<MessageResult> DowngradeApplicationVersionCompatibility(int appId, int oldVersionId, int newVersionId);
    Task<MessageResult<IList<ViewChangelog>>> LoadLastChangelogs(int appId, int versionId);
    Task<MessageResult> SetVersionToProduction(int appId, int versionId, bool isProduction);
    Task<MessageResult<List<ApplicationDTO>>> GetOtherAppsUsingThisVersion(int versionId);
}
