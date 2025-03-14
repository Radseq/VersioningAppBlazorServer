namespace EFDataAccessLib.Repos
{
    public interface IBasicRepoWrite<T>
    {
        Task<T> AddOneAsync(T entity);

        T Modify(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}