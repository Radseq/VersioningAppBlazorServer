using EFDataAccessLib.DataAccess;
using EFDataAccessLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFDataAccessLib.Repos.Versioning;

public class RepoAppCompatibility : IRepoAppCompatibility
{
    private readonly ProgDbContext dataContext;

    public RepoAppCompatibility(ProgDbContext _dbContext)
    {
        dataContext = _dbContext;
    }

    public async Task<AppCompatibility> AddOneAsync(AppCompatibility entity)
    {
        await dataContext.AppCompatibilities.AddAsync(entity);
        return entity;
    }

    public async Task<int> CountAsync()
    {
        return await dataContext.AppCompatibilities.CountAsync();
    }

    public async Task<int> CountWhereAsync(Expression<Func<AppCompatibility, bool>> predicate)
    {
        return await dataContext.AppCompatibilities.Where(predicate).CountAsync();
    }

    public async Task<AppCompatibility?> FirstOrDefaultAsync(Expression<Func<AppCompatibility, bool>> predicate)
    {
        return await dataContext.AppCompatibilities.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<AppCompatibility>> GetAllAsync()
    {
        return await dataContext.AppCompatibilities.ToListAsync();
    }
    public async Task<AppCompatibility?> GetByIdAsync(int id)
    {
        return await dataContext.AppCompatibilities.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<AppCompatibility>> GetManyAsync(List<int> manyList)
    {
        return await dataContext.AppCompatibilities.Where(i => manyList.Contains(i.Id)).ToListAsync();
    }

    public async Task<IEnumerable<AppCompatibility>> GetWhereAsync(Expression<Func<AppCompatibility, bool>> predicate)
    {
        return await dataContext.AppCompatibilities.Where(predicate).ToListAsync();
    }

    public AppCompatibility Modify(AppCompatibility entity)
    {
        dataContext.AppCompatibilities.Update(entity);
        return entity;
    }

    public void Remove(AppCompatibility entity)
    {
        dataContext.AppCompatibilities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<AppCompatibility> entities)
    {
        dataContext.AppCompatibilities.RemoveRange(entities);
    }
}
