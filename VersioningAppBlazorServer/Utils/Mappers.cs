using VersioningAppBlazorServer.Models;

namespace VersioningAppBlazorServer.Utils;

public static class Mappers
{
    public static ApplicationView MapApplication_to_ApplicationView(ApplicationDTO app)
    {
        return new ApplicationView()
        {
            Id = app.Id,
            Name = app.Name,
            Description = app.Description,
            Versions = app.Versions.ConvertAll(Copys.Copy)
        };
    }

    public static EFDataAccessLib.Models.AppCompatibility MapToDB(AppCompatibilityDTO appCompatibility)
    {
        return new EFDataAccessLib.Models.AppCompatibility()
        {
            SourceVersionId = appCompatibility.VersionId,
            TargetVersionId = appCompatibility.CompatibleWithVersionId
        };
    }

    public static EFDataAccessLib.Models.AppVersion MapToDB(AppVersionDTO appVersion)
    {
        return new EFDataAccessLib.Models.AppVersion()
        {
            Major = appVersion.Major,
            Minor = appVersion.Minor,
            Patch = appVersion.Patch,
            Description = appVersion.Description,
            ApplicationId = appVersion.AppId,
            CompatibilityTargetVersions = appVersion.Compatibilities.ToList().ConvertAll(MapToDB)
        };
    }
    public static AppVersionDTO MapToDTO(EFDataAccessLib.Models.AppVersion appVersion)
    {
        return new AppVersionDTO()
        {
            AppId = appVersion.ApplicationId,
            Id = appVersion.Id,
            Major = appVersion.Major,
            Minor = appVersion.Minor,
            Patch = appVersion.Patch,
            Description = appVersion.Description,
            Compatibilities = appVersion.CompatibilitySourceVersions.ToList().ConvertAll(MapToDTO)
        };
    }

    public static AppCompatibilityDTO MapToDTO(EFDataAccessLib.Models.AppCompatibility appCompatibility)
    {
        return new AppCompatibilityDTO()
        {
            Id = appCompatibility.Id,
            CompatibleWithVersionId = appCompatibility.TargetVersionId,
            VersionId = appCompatibility.SourceVersionId
        };
    }

    public static ApplicationDTO MapToDTO(EFDataAccessLib.Models.Application app)
    {
        return new ApplicationDTO()
        {
            Id = app.Id,
            Name = app.Name,
            Description = app.Description!,
            Versions = app.AppVersions.ToList().ConvertAll(MapToDTO)
        };
    }
}
