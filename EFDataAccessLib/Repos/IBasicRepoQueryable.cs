using System.Linq.Expressions;

namespace EFDataAccessLib.Repos;

public interface IBasicRepoQueryable<TCol>
{
    IQueryable<TCol> Queryable();
    Task<TCol?> ExecuteQueryableFirstOrDefault(IQueryable<TCol> queryableObject, Expression<Func<TCol, bool>> expression);
    Task<TCol?> ExecuteQueryableFirstOrDefault(IQueryable<TCol> queryableObject);
    IQueryable<TCol> GetWhereQueryable(Expression<Func<TCol, bool>> expression);
    Task<List<TCol>> ExecuteQueryable(IQueryable<TCol> queryableObject);
}
