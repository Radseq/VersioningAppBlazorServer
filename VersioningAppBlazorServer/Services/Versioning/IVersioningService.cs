using VersioningAppBlazorServer.Models;
using VersioningAppBlazorServer.Models.UI;
using ReturnTypeWrapper;

namespace VersioningAppBlazorServer.Services.Versioning;

public interface IVersioningService
{
    Task<MessageResult<int>> AddNewVersion(AppVersion appVersion, List<AppCompatibility> compatibilities);
    Task<MessageResult<KeyValuePair<int, int>>> AddNewApplication(Application application, AppVersion appVersion,
        List<AppCompatibility> compatibilities);
    Task<MessageResult<Application>> GetApplicationById(int id);
    Task<MessageResult<List<DropDownItem>>> GetAllApplicationsNames();
    Task<MessageResult<List<KeyValuePair<int, string>>>> GetSelectedAppVersions(int appId);
    Task<MessageResult<IList<Application>>> GetCompatibilityApplications(int appVersionId);
    Task<MessageResult<AppVersion>> GetAppVersion(int appVersionId);
    Task<MessageResult<IList<Application>>> GetAllApplications();
}
