using System.Linq.Expressions;

namespace EFDataAccessLib.Repos
{
    public interface IBasicRepoRead<TCol, UKeyType>
    {
        Task<int> CountAsync();

        Task<int> CountWhereAsync(Expression<Func<TCol, bool>> predicate);

        Task<TCol?> GetByIdAsync(UKeyType id);

        Task<TCol?> FirstOrDefaultAsync(Expression<Func<TCol, bool>> predicate);

        Task<IEnumerable<TCol>> GetAllAsync();

        Task<IEnumerable<TCol>> GetManyAsync(List<UKeyType> manyList);

        Task<IEnumerable<TCol>> GetWhereAsync(Expression<Func<TCol, bool>> predicate);
    }
}