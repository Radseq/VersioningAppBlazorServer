namespace VersioningAppBlazorServer.Services;

public interface ICacheService
{
    Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> createItem, TimeSpan expiration);
    public T? Get<T>(string cacheKey);
    Task RemoveAsync(string cacheKey);
    void ClearAll();
    T Create<T>(string cacheKey, T item, TimeSpan expiration);
}
