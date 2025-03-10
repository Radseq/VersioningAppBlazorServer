using Microsoft.Extensions.Caching.Memory;

namespace VersioningAppBlazorServer.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T?> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> createItem, TimeSpan expiration)
    {
        if (!_memoryCache.TryGetValue(cacheKey, out T? cacheEntry))
        {
            cacheEntry = await createItem();
            _memoryCache.Set(cacheKey, cacheEntry, expiration);
        }
        return cacheEntry;
    }

    public T Create<T>(string cacheKey, T item, TimeSpan expiration)
    {
        return _memoryCache.Set(cacheKey, item, expiration); ;
    }

    public T? Get<T>(string cacheKey)
    {
        _memoryCache.TryGetValue(cacheKey, out T? cacheEntry);

        return cacheEntry;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
        return Task.CompletedTask;
    }

    public void ClearAll()
    {
        _memoryCache.Dispose();
    }
}