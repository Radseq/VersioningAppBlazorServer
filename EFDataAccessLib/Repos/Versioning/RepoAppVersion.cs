using EFDataAccessLib.DataAccess;
using EFDataAccessLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDataAccessLib.Repos.Versioning;

public class RepoAppVersion : IRepoAppVersion
{
    private readonly ProgDbContext dataContext;

    public RepoAppVersion(ProgDbContext _dbContext)
    {
        dataContext = _dbContext;
    }

    public async Task<AppVersion> AddOneAsync(AppVersion entity)
    {
        await dataContext.AppVersions.AddAsync(entity);
        return entity;
    }

    public async Task<int> CountAsync()
    {
        return await dataContext.AppVersions.CountAsync();
    }

    public async Task<int> CountWhereAsync(Expression<Func<AppVersion, bool>> predicate)
    {
        return await dataContext.AppVersions.Where(predicate).CountAsync();
    }

    public async Task<AppVersion?> FirstOrDefaultAsync(Expression<Func<AppVersion, bool>> predicate)
    {
        return await dataContext.AppVersions.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<AppVersion>> GetAllAsync()
    {
        return await dataContext.AppVersions.ToListAsync();
    }
    public async Task<AppVersion?> GetByIdAsync(int id)
    {
        return await dataContext.AppVersions.Include(x => x.CompatibilitySourceVersions)
            .Include(x => x.CompatibilityTargetVersions).FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<AppVersion>> GetManyAsync(List<int> manyList)
    {
        return await dataContext.AppVersions.Include(x => x.Application).Where(i => manyList.Contains(i.Id)).ToListAsync();
    }

    public async Task<IEnumerable<AppVersion>> GetWhereAsync(Expression<Func<AppVersion, bool>> predicate)
    {
        return await dataContext.AppVersions.Where(predicate).Include(x => x.Application).ToListAsync();
    }

    public AppVersion Modify(AppVersion entity)
    {
        dataContext.AppVersions.Update(entity);
        return entity;
    }

    public void Remove(AppVersion entity)
    {
        dataContext.AppVersions.Remove(entity);
    }

    public void RemoveRange(IEnumerable<AppVersion> entities)
    {
        dataContext.AppVersions.RemoveRange(entities);
    }

    public async Task<int?> MaxId(int appId)
    {
        var versions = await dataContext.AppVersions.Where(x => x.AppId == appId).ToListAsync();
        if (versions.Count > 0)
        {
            return versions.Max(x => x.Id);
        }
        return null;
    }

    public async Task<List<AppVersion>> GetLastVersionsUntil(int appId, int versionId, int count)
    {
        return await dataContext.AppVersions
            .Where(x => x.ApplicationId == appId)
            .OrderByDescending(e => e.Id).Take(count)
            .ToListAsync();
    }
}
