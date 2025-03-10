using VersioningAppBlazorServer.Models;

namespace VersioningAppBlazorServer.Utils;

public static class Copys
{
    public static ApplicationPicked Copy(Application app, AppVersion appVersion)
    {
        return new ApplicationPicked()
        {
            Id = app.Id,
            Name = app.Name,
            Version = Copy(appVersion)
        };
    }

    public static Application Copy(Application app)
    {
        return new Application()
        {
            Id = app.Id,
            Name = app.Name,
            Versions = app.Versions.ConvertAll(Copy)
        };
    }

    public static AppVersion Copy(AppVersion appVersion)
    {
        return new AppVersion()
        {
            AppId = appVersion.AppId,
            Id = appVersion.Id,
            Major = appVersion.Major,
            Minor = appVersion.Minor,
            Patch = appVersion.Patch,
            Compatibilities = appVersion.Compatibilities.ConvertAll(Copy)
        };
    }

    public static AppCompatibility Copy(AppCompatibility appCompatibility)
    {
        return new AppCompatibility()
        {
            AppVersionId = appCompatibility.AppVersionId,
            CompatibleWithAppId = appCompatibility.CompatibleWithAppId,
            Id = appCompatibility.Id
        };
    }
}
