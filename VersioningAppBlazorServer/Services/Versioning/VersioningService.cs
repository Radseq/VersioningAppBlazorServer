using VersioningAppBlazorServer.Models;
using VersioningAppBlazorServer.Models.UI;
using ReturnTypeWrapper;

namespace VersioningAppBlazorServer.Services.Versioning;

public class VersioningService : IVersioningService
{
    private List<Application> Applications = [];
    private List<AppVersion> AppVersions = [];
    private List<AppCompatibility> AppCompatibilities = [];

    public VersioningService()
    {
        if (Applications.Count == 0 || AppVersions.Count == 0 || AppCompatibilities.Count == 0)
        {
            // Create 3 applications
            var app1 = new Application { Id = 1, Name = "www" };
            var app2 = new Application { Id = 2, Name = "api" };
            var app3 = new Application { Id = 3, Name = "api2" };

            // Add versions to App1
            var app1Version1 = new AppVersion { Id = 1, AppId = 1, Major = 1, Minor = 0, Patch = 0 };
            var app1Version2 = new AppVersion { Id = 2, AppId = 1, Major = 1, Minor = 1, Patch = 0 };
            app1.Versions.Add(app1Version1);
            app1.Versions.Add(app1Version2);
            app1.Versions = app1.Versions.OrderByDescending(x => x.Major).ThenByDescending(g => g.Minor).ThenByDescending(r => r.Patch).ToList();

            // Add versions to App2
            var app2Version1 = new AppVersion { Id = 3, AppId = 2, Major = 2, Minor = 0, Patch = 0 };
            var app2Version2 = new AppVersion { Id = 4, AppId = 2, Major = 2, Minor = 1, Patch = 0 };
            app2.Versions.Add(app2Version1);
            app2.Versions.Add(app2Version2);
            app2.Versions = app2.Versions.OrderByDescending(x => x.Major).ThenByDescending(g => g.Minor).ThenByDescending(r => r.Patch).ToList();

            // Add versions to App3
            var app3Version1 = new AppVersion { Id = 5, AppId = 3, Major = 3, Minor = 0, Patch = 0 };
            var app3Version2 = new AppVersion { Id = 6, AppId = 3, Major = 3, Minor = 1, Patch = 0 };
            app3.Versions.Add(app3Version1);
            app3.Versions.Add(app3Version2);
            app3.Versions = app3.Versions.OrderByDescending(x => x.Major).ThenByDescending(g => g.Minor).ThenByDescending(r => r.Patch).ToList();

            // Add compatibilities
            var compatibility1 = new AppCompatibility { Id = 1, AppVersionId = 3, CompatibleWithAppId = 2 };
            var compatibility2 = new AppCompatibility { Id = 2, AppVersionId = 6, CompatibleWithAppId = 3 };
            app1Version1.Compatibilities.Add(compatibility1);
            app1Version1.Compatibilities.Add(compatibility2);

            var compatibility3 = new AppCompatibility { Id = 3, AppVersionId = 1, CompatibleWithAppId = 1 };
            app2Version1.Compatibilities.Add(compatibility3);

            var compatibility4 = new AppCompatibility { Id = 4, AppVersionId = 1, CompatibleWithAppId = 1 };
            app3Version2.Compatibilities.Add(compatibility4);

            Applications = [app1, app2, app3];
            AppVersions = [app1Version1, app1Version2, app2Version1, app2Version2, app3Version1, app3Version2];
            AppCompatibilities = [compatibility1, compatibility2, compatibility3, compatibility4];
        }
    }

    public async Task<MessageResult<KeyValuePair<int, int>>> AddNewApplication(Application application, AppVersion appVersion,
        List<AppCompatibility> compatibilities)
    {
        try
        {
            application.Id = Applications.Max(x => x.Id) + 1;

            var compLastId = AppCompatibilities.Max(x => x.Id);

            foreach (var item in compatibilities)
            {
                compLastId += 1;
                item.Id = compLastId;
            }

            foreach (var item in appVersion.Compatibilities)
            {
                var comp = compatibilities.FirstOrDefault(x => x.Id == item.Id);
                if (comp != null)
                    item.Id = comp.Id;
            }

            appVersion.Id = AppVersions.Max(x => x.Id) + 1;
            appVersion.AppId = application.Id;
            //application.Versions.Clear();
            //application.Versions.Add(appVersion);

            AppVersions.Add(appVersion);

            AppCompatibilities.AddRange(compatibilities);

            Applications.Add(application);

            return MessageResult<KeyValuePair<int, int>>.Success(new KeyValuePair<int, int>(application.Id, appVersion.Id));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<int>> AddNewVersion(AppVersion appVersion, List<AppCompatibility> compatibilities)
    {
        try
        {
            var application = Applications.FirstOrDefault(x => x.Id == appVersion.AppId);
            if (application == null)
                throw new NotImplementedException();

            appVersion.Id = AppVersions.Max(x => x.Id) + 1;

            var compLastId = AppCompatibilities.Max(x => x.Id);

            foreach (var item in compatibilities)
            {
                compLastId += 1;
                item.Id = compLastId;
            }

            foreach (var item in appVersion.Compatibilities)
            {
                var comp = compatibilities.FirstOrDefault(x => x.Id == item.Id);
                if (comp != null)
                    item.Id = comp.Id;
            }

            application.Versions.Add(appVersion);
            AppVersions.Add(appVersion);
            AppCompatibilities.AddRange(compatibilities);

            return MessageResult<int>.Success(appVersion.Id);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<IList<Application>>> GetAllApplications()
    {
        try
        {
            return MessageResult<IList<Application>>.Success(Applications);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<MessageResult<List<DropDownItem>>> GetAllApplicationsNames()
    {
        try
        {
            var result = new List<DropDownItem>();
            foreach (var application in Applications)
            {
                result.Add(new DropDownItem()
                {
                    Id = application.Id,
                    Value = application.Name
                });
            }
            return MessageResult<List<DropDownItem>>.Success(result);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<Application>> GetApplicationById(int id)
    {
        try
        {
            var application = Applications.FirstOrDefault(x => x.Id == id);
            if (application == null)
                throw new NotImplementedException();

            application.Versions = application.Versions.OrderByDescending(x => x.Id).ToList();

            return MessageResult<Application>.Success(application);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<AppVersion>> GetAppVersion(int appVersionId)
    {
        try
        {
            var applicationVersion = AppVersions.FirstOrDefault(x => x.Id == appVersionId);
            if (applicationVersion == null)
                throw new NotImplementedException();

            return MessageResult<AppVersion>.Success(applicationVersion);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<IList<Application>>> GetCompatibilityApplications(int appVersionId)
    {
        try
        {
            var applicationVersion = AppVersions.FirstOrDefault(x => x.Id == appVersionId);
            if (applicationVersion == null)
                throw new NotImplementedException();

            var compatibilitiesForVersion = applicationVersion.Compatibilities ?? [];

            var withCompatibleAppIds = AppCompatibilities.Select(f => f.CompatibleWithAppId).ToList();

            var withCompatibleApps = Applications.Where(x => withCompatibleAppIds.Contains(x.Id)).ToList();

            return MessageResult<IList<Application>>.Success(withCompatibleApps ?? []);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<MessageResult<List<KeyValuePair<int, string>>>> GetSelectedAppVersions(int appId)
    {
        try
        {
            var application = Applications.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Id == appId);
            if (application == null)
                throw new NotImplementedException();

            var result = new List<KeyValuePair<int, string>>();
            foreach (var appVersion in application.Versions)
            {
                result.Add(new KeyValuePair<int, string>(appVersion.Id, appVersion.Version));
            }

            return MessageResult<List<KeyValuePair<int, string>>>.Success(result);
        }
        catch (Exception ex)
        {

            throw;
        }

    }
}
