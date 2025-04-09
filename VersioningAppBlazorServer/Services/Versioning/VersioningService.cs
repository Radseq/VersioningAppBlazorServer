using EFDataAccessLib.Repos;
using EFDataAccessLib.Repos.Versioning;
using Newtonsoft.Json;
using VersioningAppBlazorServer.Models;
using VersioningAppBlazorServer.Models.UI;
using VersioningAppBlazorServer.Utils;
using ReturnTypeWrapper;
using EFDataAccessLib.Models;

namespace VersioningAppBlazorServer.Services.Versioning;

public class VersioningService : IVersioningService
{
    private readonly ILogger<VersioningService> logger;
    private readonly IRepoApplication repoApplication;
    private readonly IRepoAppVersion repoAppVersion;
    private readonly IRepoAppCompatibility repoAppCompatibility;
    private readonly IDatabaseTransactionOperation databaseTransactionOperation;

    public VersioningService(ILogger<VersioningService> _logger, IRepoApplication _repoApplication,
        IRepoAppVersion _repoAppVersion, IRepoAppCompatibility _repoAppCompatibility,
        IDatabaseTransactionOperation _databaseTransactionOperation)
    {
        logger = _logger;
        repoApplication = _repoApplication;
        repoAppVersion = _repoAppVersion;
        repoAppCompatibility = _repoAppCompatibility;
        databaseTransactionOperation = _databaseTransactionOperation;
    }

    public async Task<MessageResult<KeyValuePair<int, int>>> AddNewApplication(ApplicationDTO application)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(application.Name))
                return MessageResult<KeyValuePair<int, int>>.FailureErrorNumberExtract(ErrorList._204);

            var nameExists = await repoApplication.FirstOrDefaultAsync(x => x.Name.ToLower() == application.Name.ToLower());
            if (nameExists != null)
                return MessageResult<KeyValuePair<int, int>>.FailureErrorNumberExtract(ErrorList._203);

            await databaseTransactionOperation.StartAsync();

            var newApplication = await repoApplication.AddOneAsync(new Application()
            {
                Name = application.Name,
                Description = application.Description
            });

            await databaseTransactionOperation.SaveAsync();

            var versionId = 0;

            if (application.Versions.Count > 0)
            {
                var newAppVersion = Mappers.MapToDB(application.Versions[0]);
                newAppVersion.CompatibilitySourceVersions.Clear();
                newAppVersion.CompatibilityTargetVersions.Clear();
                newAppVersion.ApplicationId = newApplication.Id;

                newAppVersion = await repoAppVersion.AddOneAsync(newAppVersion);

                await databaseTransactionOperation.SaveAsync();

                versionId = newAppVersion.Id;

                foreach (var item in application.Versions[0].Compatibilities)
                {
                    var compAdd = Mappers.MapToDB(item);
                    compAdd.SourceVersionId = newAppVersion.Id;
                    var comp = await repoAppCompatibility.AddOneAsync(compAdd);
                }
            }

            await databaseTransactionOperation.SaveAsync();

            await databaseTransactionOperation.EndAsync();

            return MessageResult<KeyValuePair<int, int>>.Success(new KeyValuePair<int, int>(newApplication.Id, versionId));
        }
        catch (Exception ex)
        {
            await databaseTransactionOperation.RollBackAsync();

            logger.LogError(ex, "Can't add new application, json = {json}", JsonConvert.SerializeObject(application));
            return MessageResult<KeyValuePair<int, int>>.FailureErrorNumberExtract(ErrorList._201);
        }
    }

    public async Task<MessageResult<int>> AddNewVersion(AppVersionDTO appVersion, bool doInheritCompatibilityOfPreviousVersions)
    {
        try
        {
            var application = await repoApplication.GetByIdAsync(appVersion.AppId);
            if (application == null)
                return MessageResult<int>.FailureErrorNumberExtract(ErrorList._202);

            await databaseTransactionOperation.StartAsync();

            // unmark previous production version
            if (appVersion.IsProduction)
            {
                var previousVersions = await repoAppVersion.GetWhereAsync(x => x.ApplicationId == appVersion.AppId);
                foreach (var item in previousVersions)
                {
                    if (item.IsProduction == true)
                    {
                        item.IsProduction = false;
                        repoAppVersion.Modify(item);
                    }
                    else continue;
                }
            }

            await databaseTransactionOperation.SaveAsync();

            var inheritAppCompatibilitiesVersionList = new List<int>();

            // inherit
            if (doInheritCompatibilityOfPreviousVersions)
            {
                var previousVersionId = await repoAppVersion.MaxId(appVersion.AppId);

                var getCompatibilitiesPreviousVersion = await repoAppCompatibility.GetWhereAsync(x => x.SourceVersionId == previousVersionId);

                var toAddCompatibilities = appVersion.Compatibilities.Select(x => x.CompatibleWithVersionId).ToList();

                var toAddCompatibilitiesVersions = await repoAppVersion.GetManyAsync(toAddCompatibilities);
                var toAddCompatibilitiesApps = toAddCompatibilitiesVersions.Select(x => x.Application);

                foreach (var item in getCompatibilitiesPreviousVersion)
                {
                    var toCompatibilityVersions = toAddCompatibilitiesApps.FirstOrDefault(x => x.Id == item.TargetVersion.ApplicationId);
                    if (toCompatibilityVersions == null)
                        inheritAppCompatibilitiesVersionList.Add(item.TargetVersionId);
                }
            }

            var newAppVersion = Mappers.MapToDB(appVersion);
            newAppVersion.CompatibilityTargetVersions.Clear();
            newAppVersion.CompatibilitySourceVersions.Clear();
            newAppVersion.ApplicationId = application.Id;

            newAppVersion = await repoAppVersion.AddOneAsync(newAppVersion);

            await databaseTransactionOperation.SaveAsync();

            foreach (var item in appVersion.Compatibilities)
            {
                var compAdd = Mappers.MapToDB(item);
                compAdd.SourceVersionId = newAppVersion.Id;
                var comp = await repoAppCompatibility.AddOneAsync(compAdd);
            }

            await databaseTransactionOperation.SaveAsync();

            foreach (var versionId in inheritAppCompatibilitiesVersionList)
            {
                var newCompatibility = new AppCompatibility()
                {
                    SourceVersionId = newAppVersion.Id,
                    TargetVersionId = versionId
                };
                var comp = await repoAppCompatibility.AddOneAsync(newCompatibility);
            }

            await databaseTransactionOperation.SaveAsync();

            await databaseTransactionOperation.EndAsync();

            return MessageResult<int>.Success(newAppVersion.Id);
        }
        catch (Exception ex)
        {
            await databaseTransactionOperation.RollBackAsync();

            logger.LogError(ex, "Can't add new application version, json = {json}", JsonConvert.SerializeObject(appVersion));
            return MessageResult<int>.FailureErrorNumberExtract(ErrorList._301);
        }
    }

    public async Task<MessageResult> AddNewVersionCompatibilities(int versionId, List<AppCompatibilityDTO> compatibilities)
    {
        try
        {
            var applicationVersion = await repoAppVersion.GetByIdAsync(versionId);
            if (applicationVersion == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._302);

            foreach (var item in compatibilities)
            {
                var compAdd = Mappers.MapToDB(item);
                var comp = await repoAppCompatibility.AddOneAsync(compAdd);
            }
            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't add new application version compatibilities, " +
                "compatibilities json = {json}, versionId = {versionId}", JsonConvert.SerializeObject(compatibilities), versionId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._401);
        }
    }

    public async Task<MessageResult> DeleteAppCompatibility(int appVersionId, int appVersionIdToDelete)
    {
        try
        {
            var compatibilityVersion = await repoAppCompatibility.FirstOrDefaultAsync(x => x.SourceVersionId == appVersionId &&
             x.TargetVersionId == appVersionIdToDelete);

            if (compatibilityVersion == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._403);

            repoAppCompatibility.Remove(compatibilityVersion);

            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Delete App Compatibility app Version Id = {appVersionId}, " +
                "app Version Id To Delete = {appVersionIdToDelete}", appVersionId, appVersionIdToDelete);
            return MessageResult.FailureErrorNumberExtract(ErrorList._402);
        }
    }

    public async Task<MessageResult> DeleteApplication(int appId)
    {
        try
        {
            var application = await repoApplication.GetByIdAsync(appId);
            if (application == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._202);

            for (int i = 0; i < application.AppVersions.Count; i++)
            {
                var deleteResult = await DeleteApplicationVersion(application.AppVersions[i].Id);
                if (deleteResult.HasFailed)
                    return MessageResult.Failure(deleteResult.ErrorData!);
            }

            repoApplication.Remove(application);

            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Delete Application, id = {id}", appId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._205);
        }
    }

    public async Task<MessageResult> DeleteApplicationVersion(int appVersionId)
    {
        try
        {
            var appVersion = await repoAppVersion.GetByIdAsync(appVersionId);
            if (appVersion == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._302);

            repoAppCompatibility.RemoveRange(appVersion.CompatibilitySourceVersions);
            repoAppCompatibility.RemoveRange(appVersion.CompatibilityTargetVersions);
            repoAppVersion.Remove(appVersion);

            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Delete Application version, id = {id}", appVersionId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._303);
        }
    }

    public async Task<MessageResult<IList<ApplicationDTO>>> GetAllApplications()
    {
        try
        {
            var result = new List<ApplicationDTO>();

            var applicationsDb = await repoApplication.GetAllAsync();

            return MessageResult<IList<ApplicationDTO>>.Success(applicationsDb.ToList().ConvertAll(Mappers.MapToDTO));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get all Applications");
            return MessageResult<IList<ApplicationDTO>>.FailureErrorNumberExtract(ErrorList._206);
        }
    }

    public async Task<MessageResult<List<DropDownItem>>> GetAllApplicationsNames()
    {
        try
        {
            var result = new List<DropDownItem>();

            var applicationsDb = await repoApplication.GetAllAsync();

            foreach (var item in applicationsDb)
            {
                result.Add(new DropDownItem()
                {
                    Id = item.Id,
                    Value = item.Name
                });
            }

            result = result.OrderBy(x => x.Value).ToList();

            return MessageResult<List<DropDownItem>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get all Applications Names");
            return MessageResult<List<DropDownItem>>.FailureErrorNumberExtract(ErrorList._207);
        }
    }

    public async Task<MessageResult<ApplicationDTO>> GetApplicationById(int id)
    {
        try
        {
            var application = await repoApplication.GetByIdAsync(id);
            if (application == null)
                return MessageResult<ApplicationDTO>.FailureErrorNumberExtract(ErrorList._202);

            application.AppVersions = application.AppVersions.OrderByDescending(x => x.Id).ToList();

            return MessageResult<ApplicationDTO>.Success(Mappers.MapToDTO(application));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get Application by id = {id}", id);
            return MessageResult<ApplicationDTO>.FailureErrorNumberExtract(ErrorList._202);
        }
    }

    public async Task<MessageResult<AppVersionDTO>> GetAppVersion(int appVersionId)
    {
        try
        {
            var applicationVersion = await repoAppVersion.GetByIdAsync(appVersionId);
            if (applicationVersion == null)
                return MessageResult<AppVersionDTO>.FailureErrorNumberExtract(ErrorList._302);

            return MessageResult<AppVersionDTO>.Success(Mappers.MapToDTO(applicationVersion));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get Application version by id = {id}", appVersionId);
            return MessageResult<AppVersionDTO>.FailureErrorNumberExtract(ErrorList._302);
        }
    }

    public async Task<MessageResult<IList<ApplicationDTO>>> GetCompatibilityApplications(int appVersionId)
    {
        try
        {
            var applicationVersion = await repoAppVersion.GetByIdAsync(appVersionId);
            if (applicationVersion == null)
                return MessageResult<IList<ApplicationDTO>>.FailureErrorNumberExtract(ErrorList._302);

            var AppCompatibilities = await repoAppCompatibility.GetWhereAsync(x => x.SourceVersionId == appVersionId);
            var withCompatibleAppVersionIds = AppCompatibilities.Select(f => f.TargetVersionId).ToList();

            var versions = await repoAppVersion.GetManyAsync(withCompatibleAppVersionIds.ToList());
            var versionsIds = versions.Select(x => x.ApplicationId);
            var withCompatibleApps = await repoApplication.GetManyAsync(versionsIds.ToList());

            return MessageResult<IList<ApplicationDTO>>.Success(withCompatibleApps.ToList().ConvertAll(Mappers.MapToDTO));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Get Compatibility Applications by version by id = {id}", appVersionId);
            return MessageResult<IList<ApplicationDTO>>.FailureErrorNumberExtract(ErrorList._404);
        }
    }

    public async Task<MessageResult<IList<AppCompatibilityDTO>>> GetVersionCompatibilities(int appVersionId)
    {
        try
        {
            var applicationVersion = await repoAppVersion.GetByIdAsync(appVersionId);
            if (applicationVersion == null)
                return MessageResult<IList<AppCompatibilityDTO>>.FailureErrorNumberExtract(ErrorList._302);

            var AppCompatibilities = await repoAppCompatibility.GetWhereAsync(x => x.SourceVersionId == appVersionId);

            return MessageResult<IList<AppCompatibilityDTO>>.Success(AppCompatibilities.ToList().ConvertAll(Mappers.MapToDTO));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Get Compatibility Applications by version by id = {id}", appVersionId);
            return MessageResult<IList<AppCompatibilityDTO>>.FailureErrorNumberExtract(ErrorList._404);
        }
    }

    public async Task<MessageResult<List<KeyValuePair<int, string>>>> GetSelectedAppVersions(int appId)
    {
        try
        {
            var application = await repoApplication.GetByIdAsync(appId);
            if (application == null)
                return MessageResult<List<KeyValuePair<int, string>>>.FailureErrorNumberExtract(ErrorList._202);

            var versions = application.AppVersions.ToList().ConvertAll(Mappers.MapToDTO);

            var result = new List<KeyValuePair<int, string>>();
            foreach (var appVersion in versions)
            {
                result.Add(new KeyValuePair<int, string>(appVersion.Id, appVersion.Version));
            }

            return MessageResult<List<KeyValuePair<int, string>>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Get Selected App Versions by app id = {id}", appId);
            return MessageResult<List<KeyValuePair<int, string>>>.FailureErrorNumberExtract(ErrorList._304);
        }
    }

    public async Task<MessageResult> UpgradeApplicationVersionCompatibility(int appId, int sourceAppVersionId, int oldVersionId, int newVersionId)
    {
        try
        {
            var app = await repoApplication.GetByIdAsync(appId);
            if (app == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._202);

            var oldAppVersionCompatibilities = await repoAppCompatibility.GetWhereAsync(x => x.SourceVersionId == sourceAppVersionId);

            var applicationVersionOld = await repoAppVersion.FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Id == oldVersionId);
            if (applicationVersionOld == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._405);

            var applicationVersionNew = await repoAppVersion.FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Id == newVersionId);
            if (applicationVersionNew == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._406);

            await databaseTransactionOperation.StartAsync();

            foreach (var item in oldAppVersionCompatibilities)
            {
                var targetVersionId = app.AppVersions.FirstOrDefault(x => x.Id == item.TargetVersionId);
                if (targetVersionId != null)
                    repoAppCompatibility.Remove(item);
            }
            await databaseTransactionOperation.SaveAsync();

            await repoAppCompatibility.AddOneAsync(new AppCompatibility()
            {
                SourceVersionId = sourceAppVersionId,
                TargetVersionId = newVersionId
            });

            await databaseTransactionOperation.SaveAsync();

            await databaseTransactionOperation.EndAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Upgrade application id = {appId}, compatibility version id = {oldVersionId}" +
                " to new version id = {newVersionId}", appId, oldVersionId, newVersionId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._407);
        }
    }

    public async Task<MessageResult> DowngradeApplicationVersionCompatibility(int appId, int oldVersionId, int newVersionId)
    {
        try
        {
            var applicationVersionOld = await repoAppVersion.FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Id == oldVersionId);
            if (applicationVersionOld == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._405);

            var applicationVersionNew = await repoAppVersion.FirstOrDefaultAsync(x => x.ApplicationId == appId && x.Id == newVersionId);
            if (applicationVersionNew == null)
                return MessageResult.FailureErrorNumberExtract(ErrorList._406);

            var oldAppVersionCompatibility = applicationVersionOld.CompatibilitySourceVersions.FirstOrDefault(x => x.SourceVersionId == oldVersionId);
            if (oldAppVersionCompatibility != null)
            {
                oldAppVersionCompatibility.SourceVersionId = oldVersionId;
                repoAppCompatibility.Modify(oldAppVersionCompatibility);
            }

            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't Upgrade application id = {appId}, compatibility version id = {oldVersionId}" +
                " to new version id = {newVersionId}", appId, oldVersionId, newVersionId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._407);
        }
    }

    public async Task<MessageResult<IList<ViewChangelog>>> LoadLastChangelogs(int appId, int versionId)
    {
        try
        {
            var lastVersions = await repoAppVersion.GetLastVersionsUntil(appId, versionId, 10);

            var result = new List<ViewChangelog>();

            foreach (var item in lastVersions)
            {
                result.Add(new ViewChangelog()
                {
                    ChangelogText = item.Description,
                    Version = $"{item.Major}.{item.Minor}.{item.Patch}"
                });
            }

            return MessageResult<IList<ViewChangelog>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get last changelogs app id = {appId}, up to version Id = {versionId}", appId, versionId);
            return MessageResult<IList<ViewChangelog>>.FailureErrorNumberExtract(ErrorList._305);
        }
    }

    public async Task<MessageResult> SetVersionToProduction(int appId, int versionId, bool isProduction)
    {
        try
        {
            var result = false;
            var previousVersions = await repoAppVersion.GetWhereAsync(x => x.ApplicationId == appId);
            foreach (var item in previousVersions)
            {
                if (item.Id == versionId)
                {
                    item.IsProduction = isProduction;
                    repoAppVersion.Modify(item);
                    result = true;
                    continue;
                }
                if (isProduction == true)
                {
                    result = true;
                    item.IsProduction = false;
                    repoAppVersion.Modify(item);
                }
            }
            if (!result)
                return MessageResult.FailureErrorNumberExtract(ErrorList._306);

            await databaseTransactionOperation.SaveAsync();

            return MessageResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't mark version to production one, version Id = {versionId}, api id = {apiId}", versionId, appId);
            return MessageResult.FailureErrorNumberExtract(ErrorList._306);
        }
    }
    public async Task<MessageResult<List<ApplicationDTO>>> GetOtherAppsUsingThisVersion(int versionId)
    {
        try
        {
            var appsCompatibilityWithThisVersion = await repoAppCompatibility.GetWhereAsync(x => x.TargetVersionId == versionId);

            var appsVersions = await repoAppVersion.GetWhereAsync(x =>
                appsCompatibilityWithThisVersion.Select(x => x.SourceVersionId).Contains(x.Id));

            var apps = appsVersions.Select(x => x.Application).Distinct().ToList().ConvertAll(Mappers.MapToDTO);

            for (int i = 0; i < apps.Count; i++)
            {
                var versions = appsVersions.Where(x => x.ApplicationId == apps[i].Id).OrderByDescending(x => x.Id);
                apps[i].Versions = versions.ToList().ConvertAll(Mappers.MapToDTO);
            }

            return MessageResult<List<ApplicationDTO>>.Success(apps);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Can't get other applications using this version, version Id = {versionId}", versionId);
            return MessageResult<List<ApplicationDTO>>.FailureErrorNumberExtract(ErrorList._208);
        }
    }
}
