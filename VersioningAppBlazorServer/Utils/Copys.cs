using VersioningAppBlazorServer.Models;

namespace VersioningAppBlazorServer.Utils;

public static class Copies
{
    public static ApplicationPicked Copy(ApplicationDTO app, AppVersionDTO appVersion)
    {
        return new ApplicationPicked()
        {
            Id = app.Id,
            Name = app.Name,
            Version = Copy(appVersion)
        };
    }

    public static ApplicationDTO Copy(ApplicationDTO app)
    {
        return new ApplicationDTO()
        {
            Id = app.Id,
            Name = app.Name,
            Description = app.Description,
            Versions = app.Versions.ConvertAll(Copy)
        };
    }

    public static AppVersionDTO Copy(AppVersionDTO appVersion)
    {
        return new AppVersionDTO()
        {
            AppId = appVersion.AppId,
            Id = appVersion.Id,
            Major = appVersion.Major,
            Minor = appVersion.Minor,
            Patch = appVersion.Patch,
            Description = appVersion.Description,
            Compatibilities = appVersion.Compatibilities.ConvertAll(Copy)
        };
    }

    public static AppCompatibilityDTO Copy(AppCompatibilityDTO appCompatibility)
    {
        return new AppCompatibilityDTO()
        {
            VersionId = appCompatibility.VersionId,
            CompatibleWithVersionId = appCompatibility.CompatibleWithVersionId,
            Id = appCompatibility.Id
        };
    }
}
