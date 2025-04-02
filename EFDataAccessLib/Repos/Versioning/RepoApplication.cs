using EFDataAccessLib.DataAccess;
using EFDataAccessLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDataAccessLib.Repos.Versioning;

public class RepoApplication : IRepoApplication
{
    private readonly ProgDbContext dataContext;

    public RepoApplication(ProgDbContext _dbContext)
    {
        dataContext = _dbContext;
    }

    public async Task<Application> AddOneAsync(Application entity)
    {
        await dataContext.Applications.AddAsync(entity);
        return entity;
    }

    public async Task<int> CountAsync()
    {
        return await dataContext.Applications.CountAsync();
    }

    public async Task<int> CountWhereAsync(Expression<Func<Application, bool>> predicate)
    {
        return await dataContext.Applications.Where(predicate).CountAsync();
    }

    public async Task<Application?> FirstOrDefaultAsync(Expression<Func<Application, bool>> predicate)
    {
        return await dataContext.Applications.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        return await dataContext.Applications.Include(x => x.AppVersions).ThenInclude(x => x.CompatibilitySourceVersions).ToListAsync();
    }
    public async Task<Application?> GetByIdAsync(int id)
    {
        return await dataContext.Applications.Include(x => x.AppVersions).FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<Application>> GetManyAsync(List<int> manyList)
    {
        return await dataContext.Applications.Where(i => manyList.Contains(i.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetWhereAsync(Expression<Func<Application, bool>> predicate)
    {
        return await dataContext.Applications.Where(predicate).ToListAsync();
    }

    public Application Modify(Application entity)
    {
        dataContext.Applications.Update(entity);
        return entity;
    }

    public void Remove(Application entity)
    {
        dataContext.Applications.Remove(entity);
    }

    public void RemoveRange(IEnumerable<Application> entities)
    {
        dataContext.Applications.RemoveRange(entities);
    }
}
